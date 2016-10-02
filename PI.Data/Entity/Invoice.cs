using PI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class Invoice : LongIdBaseEntity
    {
        [MaxLength(50)]
        public string InvoiceNumber { get; set; }
        public long? ShipmentId { get; set; }
        public decimal InvoiceValue { get; set; }
        public decimal Sum { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; }
        public string URL { get; set; }
        public DateTime DueDate { get; set; }
        public string Terms { get; set; }

        [ForeignKey("ShipmentId")]
        public virtual Shipment Shipment { get; set; }
        public virtual ICollection<CreditNote> creditNoteList { get; set; }
    }
}
