using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Shipment
{
    public class PackageDetailsDto
    {
        public string ShipmentDescription { get; set; }
        public decimal TotalWeight { get; set; }
        public short CmLBS { get; set; }
        public string productTypes { get; set; }
        public decimal TotalVolume { get; set; }
        public short VolumeCMM { get; set; }
        public List<ProductIngredientsDto> ProductIngredients { get; set; }
        public string HsCode { get; set; }
        public DateTime PreferredCollectionDate { get; set; }
        public string Instructions { get; set; }
        public string IsInsuared { get; set; }
        public int ValueCurrency { get; set; }
        public decimal DeclaredValue { get; set; }
        public long Count { get; set; }

        public short PaymentTypeId { get; set; }
       

    }
}
