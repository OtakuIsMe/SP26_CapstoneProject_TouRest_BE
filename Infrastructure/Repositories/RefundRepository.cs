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
    public class RefundRepository : BaseRepository<Refund>, IRefundRepository
    {
        public RefundRepository(AppDbContext context) : base(context) { }

        public async Task<Refund?> GetByBookingIdAsync(Guid bookingId)
        {
            return await _context.Refunds
                .Include(r => r.Payment)
                .FirstOrDefaultAsync(r => r.BookingId == bookingId);
        }

        public async Task<Refund?> GetByPaymentIdAsync(Guid paymentId)
        {
            return await _context.Refunds
                .FirstOrDefaultAsync(r => r.PaymentId == paymentId);
        }
    }
}
