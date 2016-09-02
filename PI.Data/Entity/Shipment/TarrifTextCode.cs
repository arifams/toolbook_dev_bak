using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class TarrifTextCode : LongIdBaseEntity
    {
        [MaxLength(50)]
        public string TarrifText { get; set; }
        [MaxLength(2)]
        public string CountryCode { get; set; }
    }
}
