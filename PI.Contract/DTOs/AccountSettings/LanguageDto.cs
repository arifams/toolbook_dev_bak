using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.AccountSettings
{
    public class LanguageDto
    {
        public short Id { get; set; }
        public string LanguageCode { get; set; }
        public string LanguageName { get; set; }
    }
}
