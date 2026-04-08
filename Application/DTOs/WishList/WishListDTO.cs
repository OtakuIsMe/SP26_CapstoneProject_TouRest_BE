using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.User;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.WishList
{
    //public class WishListDTO
    //{
    //    public UserDTO User { get; set; } = null!;

    //    public WishlistItemType ItemType { get; set; }

    //    public Guid ItemId { get; set; }
    //    public string ItemName { get; set; } = null!;
    //}

    public class WishListDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public WishlistItemType ItemType { get; set; }
        public Guid ItemId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
