using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.Common.Helpers
{
    public static class ParseTimeHelper
    {
        public static TimeOnly ParseTime(string timeString)
        {
            if (TimeOnly.TryParseExact(timeString, "HH:mm:ss", out var time))
            {
                return time;
            }
            throw new FormatException("Invalid time format. Expected format: HH:mm:ss");
        }
    }
}
