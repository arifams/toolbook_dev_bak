using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Postmen
{
    public class PMItems
    {
        public string description { get; set; }
        public string origin_country { get; set; }
        public float quantity { get; set; }
        public PMPrice price { get; set; }
        public PMWeight weight { get; set; }
        public string sku { get; set; }
        public string hs_code { get; set; }
     
    }
}
