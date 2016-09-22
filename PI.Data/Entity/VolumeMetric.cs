using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class VolumeMetric : ShortIdBaseEntity
    {
        [MaxLength(5)]
        public string Name { get; set; }
    }
}
