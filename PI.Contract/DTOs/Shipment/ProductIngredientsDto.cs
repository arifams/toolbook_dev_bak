using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Shipment
{
    public class ProductIngredientsDto
    {
        public long quantity { get; set; }
        public long weight { get; set; }
        public long height { get; set; }
        public long length { get; set; }
        public long width { get; set; }
    }
}
