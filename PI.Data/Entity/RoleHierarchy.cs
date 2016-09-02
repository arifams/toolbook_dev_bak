using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class RoleHierarchy
    {
        [Key]
        [MaxLength(30)]
        public string Name { get; set; }
        [MaxLength(30)]
        public string ParentName { get; set; }
        public short Order { get; set; }
    }
}
