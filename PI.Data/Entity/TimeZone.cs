using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class TimeZone: ShortIdBaseEntity
    {
        public string TimeZoneId { get; set; }
        public string DisplayName { get; set; }
    }
}
