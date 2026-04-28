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
            if (TimeOnly.TryParseExact(timeString, "HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out var time))
                return time;

            if (TimeOnly.TryParseExact(timeString, "HH:mm", null, System.Globalization.DateTimeStyles.None, out time))
                return time;

            if (TimeOnly.TryParse(timeString, out time))
                return time;

            throw new FormatException($"Invalid time format '{timeString}'. Expected: HH:mm or HH:mm:ss");
        }
    }
}
