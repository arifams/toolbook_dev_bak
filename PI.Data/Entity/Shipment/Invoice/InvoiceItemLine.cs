using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class InvoiceItemLine : LongIdBaseEntity
    {
        public string Description { get; set; }
        public short Quantity { get; set; }
        public decimal PricePerPiece { get; set; }
        public long InvoiceItemId { get; set; }
        [MaxLength(50)]
        public string HSCode { get; set; }
        #region Navigation properties

        [ForeignKey("InvoiceItemId")]
        public virtual InvoiceItem InvoiceItem { get; set; }

        #endregion
    }
}
