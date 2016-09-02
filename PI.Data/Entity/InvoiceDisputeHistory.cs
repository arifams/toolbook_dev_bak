using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class InvoiceDisputeHistory: LongIdBaseEntity
    {
        public string DisputeComment { get; set; }
        public long? InvoiceId { get; set; }       

        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }
    }
}
