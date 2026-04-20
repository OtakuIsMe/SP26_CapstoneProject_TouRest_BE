using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;

namespace TouRest.Domain.Interfaces
{
    public interface IRefundRepository : IBaseRepository<Refund>
    {
        Task<Refund?> GetByBookingIdAsync(Guid bookingId);
        Task<Refund?> GetByPaymentIdAsync(Guid paymentId);
    }
}
