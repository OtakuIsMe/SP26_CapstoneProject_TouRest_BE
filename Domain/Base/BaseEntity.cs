using System;
using System.ComponentModel.DataAnnotations;

namespace TouRest.Domain.Base
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
