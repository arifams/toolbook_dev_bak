using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class Address:BaseEntity
    {
        public string Country { get; set; } // TODO: Need to check enum or db entry - Hint: Localization
        public string State { get; set; }
    }
}
