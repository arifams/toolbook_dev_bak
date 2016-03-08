using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class PackagePayment : BaseEntity
    {
        public decimal CarrierCost { get; set; }
        public decimal InsuranceCost { get; set; }
        public short PaymentTypeId { get; set; }
        [ForeignKey("PaymentTypeId")]
        public PaymentType PaymentType { get; set; }
    }
}
