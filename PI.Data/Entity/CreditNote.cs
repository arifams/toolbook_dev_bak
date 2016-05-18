using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
   public class CreditNote : LongIdBaseEntity
    {
        public string CreditNoteNumber { get; set; }

        public long InvoiceId { get; set; }

        public decimal CreditNoteValue { get; set; }

        public string URL { get; set; }

        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }
    }


}
