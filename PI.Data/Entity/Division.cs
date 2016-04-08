using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class Division : LongIdBaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public long  DefaultCostCenterId { get; set; }

        public int Status { get; set; }

        public long CompanyId { get; set; }

        public string Type { get; set; }

        #region Navigation Property

        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }

        [ForeignKey("DefaultCostCenterId")]
        public virtual CostCenter CostCenter { get; set; }

        public virtual IList<DivisionCostCenter> DivisionCostCenters { get; set; }

        public virtual IList<UserInDivision> UserInDivisions { get; set; }


        #endregion
    }
}
