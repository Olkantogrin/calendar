using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyForm.reader
{
    class DateTimeConverter
    {
        public DateTime ConvertZuluToMezOrMeSz(DateTime dateTimeStart)
        {
            TimeZoneInfo mezTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            DateTime mezTime = TimeZoneInfo.ConvertTimeFromUtc(dateTimeStart, mezTimeZone);

            return mezTime;
        }
    }
}
