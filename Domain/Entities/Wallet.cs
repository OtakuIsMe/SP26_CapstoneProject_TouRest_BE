using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Base;

namespace TouRest.Domain.Entities
{
    [Table("wallets")]
    public class Wallet : BaseEntity
    {
        public Guid? UserId { get; set; }       
        public Guid? AgencyId { get; set; }      
        public Guid? ProviderId { get; set; }
        [Range(0, long.MaxValue)]
        public long Balance { get; set; } = 0;
        [Range(0, long.MaxValue)]
        public long PendingBalance { get; set; } = 0;  

        // Navigation
        public User? User { get; set; }
        public Agency? Agency { get; set; }
        public Provider? Provider { get; set; }
    }
}
