using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class TarrifTextCode : LongIdBaseEntity
    {
        public string TarrifText { get; set; }
        public string CountryCode { get; set; }
    }
}
