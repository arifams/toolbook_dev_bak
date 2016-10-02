using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Invoice
{
    public class InvoiceDto
    {
        public long Id { get; set; }

        public string InvoiceNumber { get; set; }

        public string ShipmentReference { get; set; }

        public long? ShipmentId { get; set; }

        public decimal InvoiceValue { get; set; }

        public string InvoiceStatus { get; set; }

        public string CreatedBy { get; set; }

        public string InvoiceDate { get; set; }

        public string BusinessOwner { get; set; }

        public string CompanyName { get; set; }

        public string URL { get; set; }

        public string DisputeComment { get; set; }

        public string DueDate { get; set; }

        public string Terms { get; set; }

        public string CreditNoteURL { get; set; }

        public string CardNonce { get; set; }

        public string CreatedOn { get; set; }

        public string UserId { get; set; }

        public string Sum { get; set; }
    }

}
