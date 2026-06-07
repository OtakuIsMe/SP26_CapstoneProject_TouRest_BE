using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.Common.Helpers
{
    public static class ItineraryStatusTransitions
    {
        private static readonly Dictionary<ItineraryStatus, ItineraryStatus[]> AllowedTransitions = new()
        {
            { ItineraryStatus.Draft,    new[] { ItineraryStatus.Active,   ItineraryStatus.Inactive } },
            { ItineraryStatus.Active,   new[] { ItineraryStatus.Inactive, ItineraryStatus.Draft    } },
            { ItineraryStatus.Inactive, new[] { ItineraryStatus.Draft,    ItineraryStatus.Active   } },
        };

        public static bool CanTransition(ItineraryStatus current, ItineraryStatus next)
            // Same status = no actual transition (just saving other fields) → always allowed
            => current == next
            || (AllowedTransitions.TryGetValue(current, out var allowed) && allowed.Contains(next));
    }
}
