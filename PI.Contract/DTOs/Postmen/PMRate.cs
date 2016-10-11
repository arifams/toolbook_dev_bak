using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Postmen
{
    public class PMRate
    {

        public PMShipperAccount shipper_account { get; set; }

        public string service_type { get; set; }
        public string service_name { get; set; }
        public string pickup_deadline { get; set; }
        public string booking_cut_off { get; set; }
        public string delivery_date { get; set; }
        public string transit_time { get; set; }
        public string error_message { get; set; }
        public string info_message { get; set; }
        public PMWeight charge_weight { get; set; }
        public PMCharge total_charge { get; set; }
        public List<PMDetailCharge> detailed_charges { get; set; }

    }
}
