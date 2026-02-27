using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.WishList
{
    public class WishListCreateDTO
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public WishlistItemType ItemType { get; set; }

        [Required]
        public Guid ItemId { get; set; }
    }
}
