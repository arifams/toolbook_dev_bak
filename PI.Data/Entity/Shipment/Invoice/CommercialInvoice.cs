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
        public string ShipmentReferenceName { get; set; }
        public string ShipTo { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceTo { get; set; }
        public string VatNo { get; set; }
        public string CustomerNo { get; set; }
        public string TermsOfPayment { get; set; }
        public short ShipmentService { get; set; }
        public string CountryOfOrigin { get; set; }
        public string CountryOfDestination { get; set; }
        public string ModeOfTransport { get; set; }
        public string ImportBroker { get; set; }
        public string Note { get; set; }
        public short ValueCurrency { get; set; }
        public long InvoiceItemId { get; set; }

        [ForeignKey("InvoiceItemId")]
        public virtual InvoiceItem InvoiceItem { get; set; }

        #region Navigation

        public virtual Shipment Shipment { get; set; }

        #endregion

        
    }
}
