using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.WishList;

namespace TouRest.Application.Interfaces
{
    public interface IWishListService
    {
        Task<IEnumerable<WishListDTO>> GetAllAsync();
        Task<WishListDTO?> GetByIdAsync(Guid id);
        Task<IEnumerable<WishListDTO>> GetByUserIdAsync(Guid userId);
        Task<WishListDTO> CreateAsync(WishListCreateRequest request);
        Task<WishListDTO?> UpdateAsync(Guid id, WishListUpdateRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}
