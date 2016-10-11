using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Postmen
{
    public class PMData
    {
        public string id { get; set; }
        public string status { get; set; }
        public string ship_date { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string[] tracking_numbers { get; set; }
        public PMFiles files { get; set; }
        public PMRate rate { get; set; }
    }
}
