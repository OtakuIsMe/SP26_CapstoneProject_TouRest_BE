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


    public class WalletRepository : BaseRepository<Wallet>, IWalletRepository
    {
        public WalletRepository(AppDbContext context) : base(context) { }

        public async Task<Wallet?> GetByUserIdAsync(Guid userId)
            => await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);

        public async Task<List<WalletTransaction>> GetTransactionsByWalletIdAsync(Guid walletId)
            => await _context.WalletTransactions
                .Where(t => t.WalletId == walletId)
                .OrderByDescending(t => t.CreatedAt)
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<Payout>> GetPayoutsByWalletIdAsync(Guid walletId)
            => await _context.Payouts
                .Where(p => p.WalletId == walletId)
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
    }


}
