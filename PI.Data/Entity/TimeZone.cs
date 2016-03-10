using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class TimeZone: ShortIdBaseEntity
    {
        public string TimeZoneCode { get; set; }
        public string CountryName { get; set; }
    }
}
