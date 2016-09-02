using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class PaymentType : ShortIdBaseEntity
    {
        [MaxLength(20)]
        public string Name { get; set; }
    }
}
