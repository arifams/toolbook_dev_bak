using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class Language: ShortIdBaseEntity
    {
        public string LanguageCode { get; set; }
        public string LanguageName { get; set; }
    }
}
