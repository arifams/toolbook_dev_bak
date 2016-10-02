using PI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Payment
{
    public class PaymentDto
    {
        /// <summary>
        /// Shipment Id for the payment
        /// </summary>
        public long ShipmentId { get; set; }

        /// <summary>
        /// Amount need to charge from the customer
        /// </summary>
        public decimal ChargeAmount { get; set; }

        /// <summary>
        /// Curreny type (USD)
        /// </summary>
        public string CurrencyType { get; set; }

        /// <summary>
        /// Card nonce which get from the Square after submitting the card details
        /// </summary>
        public string CardNonce { get; set; }

        /// <summary>
        /// User Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Template link -  
        /// </summary>
        public string TemplateLink { get; set; } // TODO : Need to remove this

        /// <summary>
        /// Amount -  
        /// </summary>
        public string Amount { get; set; }

       
    }
}
