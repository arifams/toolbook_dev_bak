using PI.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data
{
    public class Country: ShortIdBaseEntity
    {
        [MaxLength(10)]
        public string Name { get; set; }
        [MaxLength(10)]
        public string Code { get; set; }
        [MaxLength(10)]
        public string ThreeLetterCode { get; set; }

    }
}
