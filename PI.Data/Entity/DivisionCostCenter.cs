using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class DivisionCostCenter : LongIdBaseEntity
    {
        
        public long CostCenterId { get; set; }
        
        public long DivisionId { get; set; }
        public bool IsAssigned { get; set; }

        //[ForeignKey("DivisionId")]
        public virtual Division Divisions { get; set; }

        //[ForeignKey("CostCenterId")]
        public virtual CostCenter CostCenters { get; set; }
    }
}
