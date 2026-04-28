using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.Interfaces;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class PayoutService : IPayoutService
    {
        private readonly IPayoutRepository _payoutRepository;
        public PayoutService(IPayoutRepository payoutRepository)
        {
            _payoutRepository = payoutRepository;
        }
    }
}
