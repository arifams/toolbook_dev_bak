using PI.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class Address: LongIdBaseEntity
    {
        [MaxLength(2)]
        public string Country { get; set; }
        [MaxLength(10)]
        public string ZipCode { get; set; }
        [MaxLength(10)]
        public string Number { get; set; }
        public string StreetAddress1 { get; set; }
        public string StreetAddress2 { get; set; }
        [MaxLength(100)]
        public string City { get; set; }
        [MaxLength(50)]
        public string State { get; set; }

    }
}
