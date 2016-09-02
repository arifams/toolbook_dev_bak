using PI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class AuditTrail : ShortIdBaseEntity
    {
        [MaxLength(50)]
        public string ReferenceId { get; set; }
        public AppFunctionality AppFunctionality { get; set; }
        public string Result { get; set; }
        public string Comments { get; set; }     
    }
}
