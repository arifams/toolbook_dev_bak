using PI.Contract.DTOs.CostCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Division
{
    public class DivisionDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public long DefaultCostCenterId { get; set; }

        public int Status { get; set; }

        public long CompanyId { get; set; }

        public string Type { get; set; }

        public IList<CostCenterDto> AssosiatedCostCenters { get; set; }

        public string AssosiatedCostCentersForGrid { get; set; }

        public int NumberOfUsers { get; set; }

        public string EditLink { get; set; }

        public string DeleteLink { get; set; }

        public bool IsAssigned { get; set; }

        public string UserId { get; set; }

        public string StatusString { get; set; }

        public string AssignedSupervisorId { get; set; }

    }
}
