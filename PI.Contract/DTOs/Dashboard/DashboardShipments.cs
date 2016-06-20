using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Dashboard
{
    public class DashboardShipments
    {
        public int PendingStatusCount;

        public int InTransitStatusCount;

        public int DeliveredStatusCount;

        public int ExceptionStatusCount;

        public int DelayedStatusCount;

        public int BookingConfStatusCount;

    }
}
