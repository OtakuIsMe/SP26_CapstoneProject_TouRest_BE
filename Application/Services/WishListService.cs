using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.WishList;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class WishListService : IWishListService
    {
        private readonly IWishListRepository _wishListRepository;

        public WishListService(IWishListRepository wishListRepository)
        {
            _wishListRepository = wishListRepository;
        }

        public async Task<IEnumerable<WishListDTO>> GetAllAsync()
        {
            var items = await _wishListRepository.GetAllAsync();
            return items.Select(MapToDTO);
        }

        public async Task<WishListDTO?> GetByIdAsync(Guid id)
        {
            var item = await _wishListRepository.GetByIdAsync(id);
            if (item == null) return null;

            return MapToDTO(item);
        }

        public async Task<IEnumerable<WishListDTO>> GetByUserIdAsync(Guid userId)
        {
            var items = await _wishListRepository.GetByUserIdAsync(userId);
            return items.Select(MapToDTO);
        }

        public async Task<WishListDTO> CreateAsync(WishListCreateRequest request)
        {
            await ValidateRequestAsync(request.UserId, request.ItemType, request.ItemId);

            var duplicate = await _wishListRepository.GetDuplicateAsync(request.UserId, request.ItemId);
            if (duplicate != null)
                throw new InvalidOperationException("This item already exists in the user's wishlist.");

            var entity = new Wishlist
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                ItemType = request.ItemType,
                ItemId = request.ItemId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _wishListRepository.CreateAsync(entity);
            return MapToDTO(created);
        }

        public async Task<WishListDTO?> UpdateAsync(Guid id, WishListUpdateRequest request)
        {
            var existing = await _wishListRepository.GetByIdAsync(id);
            if (existing == null) return null;

            await ValidateItemExistsAsync(request.ItemType, request.ItemId);

            var duplicate = await _wishListRepository.GetDuplicateAsync(existing.UserId, request.ItemId);
            if (duplicate != null && duplicate.Id != id)
                throw new InvalidOperationException("This item already exists in the user's wishlist.");

            existing.ItemType = request.ItemType;
            existing.ItemId = request.ItemId;
            existing.UpdatedAt = DateTime.UtcNow;

            var updated = await _wishListRepository.UpdateAsync(existing);
            return MapToDTO(updated);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _wishListRepository.DeleteAsync(id);
        }

        private async Task ValidateRequestAsync(Guid userId, WishlistItemType itemType, Guid itemId)
        {
            var userExists = await _wishListRepository.UserExistsAsync(userId);
            if (!userExists)
                throw new KeyNotFoundException("User not found.");

            await ValidateItemExistsAsync(itemType, itemId);
        }

        private async Task ValidateItemExistsAsync(WishlistItemType itemType, Guid itemId)
        {
            switch (itemType)
            {
                case WishlistItemType.Service:
                    if (!await _wishListRepository.ServiceExistsAsync(itemId))
                        throw new KeyNotFoundException("Service not found.");
                    break;

                case WishlistItemType.Package:
                    if (!await _wishListRepository.PackageExistsAsync(itemId))
                        throw new KeyNotFoundException("Package not found.");
                    break;

                default:
                    throw new InvalidOperationException("Invalid wishlist item type.");
            }
        }

        private static WishListDTO MapToDTO(Wishlist item)
        {
            return new WishListDTO
            {
                Id = item.Id,
                UserId = item.UserId,
                ItemType = item.ItemType,
                ItemId = item.ItemId,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            };
        }
    }
}
