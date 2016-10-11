using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Postmen
{
    public class PMFiles
    {
        public PMLabel label { get; set; }
        public string invoice { get; set; }
        public string customs_declaration { get; set; }
        public string certificate { get; set; }
        public string manifest { get; set; }
    }
}
