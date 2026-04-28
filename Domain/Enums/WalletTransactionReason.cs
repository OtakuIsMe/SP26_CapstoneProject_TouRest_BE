using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Domain.Enums
{
    public enum WalletTransactionReason
    {
        BookingEarning = 1,   
        Refund = 2,           
        Payout = 3,           
        PayoutRejected = 4    
    }

}
