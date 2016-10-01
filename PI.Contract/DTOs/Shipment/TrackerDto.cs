using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Shipment
{
    public class TrackerDto
    {
       public string Status { get; set; }
       public List<TrackingDetails> TrackingDetails { get; set; }

    }
}
