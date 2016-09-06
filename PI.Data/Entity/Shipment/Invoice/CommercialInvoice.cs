using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class CommercialInvoice: BaseEntity
    {
        [Key, ForeignKey("Shipment")]
        public long ShipmentId { get; set; }
        [MaxLength(100)]
        public string ShipmentReferenceName { get; set; }
        public string ShipTo { get; set; }
        [MaxLength(50)]
        public string InvoiceNo { get; set; }
        public string InvoiceTo { get; set; }
        [MaxLength(50)]
        public string VatNo { get; set; }
        [MaxLength(50)]
        public string CustomerNo { get; set; }
        [MaxLength(30)]
        public string TermsOfPayment { get; set; }
        public short ShipmentService { get; set; }
        [MaxLength(2)]
        public string CountryOfOrigin { get; set; }
        [MaxLength(2)]
        public string CountryOfDestination { get; set; }
        [MaxLength(100)]
        public string ModeOfTransport { get; set; }
        [MaxLength(100)]
        public string ImportBroker { get; set; }
        public string Note { get; set; }
        public short ValueCurrency { get; set; }
        public long InvoiceItemId { get; set; }
        [MaxLength(50)]
        public string HSCode { get; set; }

        [ForeignKey("InvoiceItemId")]
        public virtual InvoiceItem InvoiceItem { get; set; }

        #region Navigation

        public virtual Shipment Shipment { get; set; }

        #endregion

        
    }
}
