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
        public bool CmLBS { get; set; }
        public string productTypes { get; set; }
        public decimal TotalVolume { get; set; }
        public bool VolumeCMM { get; set; }
        public List<ProductIngredientsDto> ProductIngredients { get; set; }
        public string HsCode { get; set; }
        public string PreferredCollectionDate { get; set; }      
        public string Instructions { get; set; }
        public string IsInsuared { get; set; }
        public int ValueCurrency { get; set; }
        public decimal DeclaredValue { get; set; }
        public long Count { get; set; }
        public bool IsDG { get; set; }
        public string DGType { get; set; }
        public bool Accessibility { get; set; }

        public short PaymentTypeId { get; set; }
        public string ValueCurrencyString { get; set; }

    }
}
