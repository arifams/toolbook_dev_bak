using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Postmen
{
    public class AddShipmentResponsePM
    {
              
        public string Awb { get; set; }
        public string DatePickup { get; set; }
        public string CodeShipment { get; set; }
        public string PDF { get; set; }
        public string ErrorMessage { get; set; }
    }
}
