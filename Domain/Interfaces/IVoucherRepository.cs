using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;

namespace TouRest.Domain.Interfaces
{
    public interface IVoucherRepository : IBaseRepository<Voucher>
    {
        Task<Voucher?> GetByCodeAsync(string code);
    }
}
