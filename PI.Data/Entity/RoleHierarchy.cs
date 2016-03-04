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
        public string Name { get; set; }
        public string ParentName { get; set; }
        public short Order { get; set; }
    }
}
