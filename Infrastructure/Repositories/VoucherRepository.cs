using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class VoucherRepository : BaseRepository<Voucher>, IVoucherRepository
    {
        public VoucherRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Voucher?> GetByCodeAsync(string code)
        {
            return await _context.Vouchers
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Code == code);
        }
    }
}
