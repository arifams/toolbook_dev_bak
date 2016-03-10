using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Shipment
{
    public class ProductIngredientsDto
    {
        public long Quantity { get; set; }
        public long Weight { get; set; }
        public long Height { get; set; }
        public long Length { get; set; }
        public long Width { get; set; }
    }
}
