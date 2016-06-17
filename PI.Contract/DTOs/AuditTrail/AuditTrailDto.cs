using PI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.AuditTrail
{
    public class AuditTrailDto
    {
        public string ReferenceId { get; set; }

        public AppFunctionality AppFunctionality { get; set; }

        public string Result { get; set; }

        public string Comments { get; set; }
    }
}
