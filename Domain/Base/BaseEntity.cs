using System;
using System.ComponentModel.DataAnnotations;

namespace TouRest.Domain.Base
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = new Guid();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
