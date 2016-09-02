using PI.Data.Entity.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class UserInDivision : LongIdBaseEntity
    {
        [MaxLength(50)]
        public string UserId { get; set; }
        public long DivisionId { get; set; }

        public virtual Division Divisions { get; set; }

        public virtual ApplicationUser User { get; set; }
    }

}
