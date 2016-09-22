using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class Language: ShortIdBaseEntity
    {
        [MaxLength(2)]
        public string LanguageCode { get; set; }
        [MaxLength(15)]
        public string LanguageName { get; set; }
    }
}
