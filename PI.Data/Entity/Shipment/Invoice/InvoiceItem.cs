using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class InvoiceItem : LongIdBaseEntity
    {
        public virtual IList<InvoiceItemLine> InvoiceItemLines { get; set; }
    }
}
