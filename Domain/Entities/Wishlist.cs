using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Entities
{
    [Table("wishlists")]
    public class Wishlist : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public WishlistItemType ItemType { get; set; }

        [Required]
        public Guid ItemId { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
    }
}
