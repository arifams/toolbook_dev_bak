using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Postmen
{
    public class PMShipment
    {
        public PMAddress ship_from { get; set; }
        public PMAddress ship_to { get; set; }
        public List<PMParcel> parcels { get; set; }


    }
}
