using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class WishListService : IWishListService
    {
        private readonly IWishListRepository _wishListRepository;
        private readonly IMapper _mapper;
        public WishListService(IWishListRepository wishListRepository, IMapper mapper)
        {
            _wishListRepository = wishListRepository;
            _mapper = mapper;
        }
        public async Task<List<Wishlist>> GetWishlists()
        {
            var list = await _wishListRepository.GetAllAsync();
            return _mapper.Map<List<Wishlist>>(list);
        }
         public async Task<Wishlist> GetWishlist(Guid id)
        {
            var wishlist = await _wishListRepository.GetWishList(id);
            return _mapper.Map<Wishlist>(wishlist);
        }
         public async Task<List<Wishlist>> GetWishlists(WishListSearch search)
        {
            var list = await _wishListRepository.GetWishLists(search);
            return _mapper.Map<List<Wishlist>>(list);
        }
         public async Task<Wishlist> AddWishlist(Wishlist create)
        {
            var result = await _wishListRepository.CreateAsync(create);
            return _mapper.Map<Wishlist>(result);
        }
        public async Task<bool> DeleteWishlist(Guid id)
        {
            var result = await _wishListRepository.DeleteAsync(id);
            return result;
        }
    }
}
