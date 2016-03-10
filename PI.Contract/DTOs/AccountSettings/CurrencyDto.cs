using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.AccountSettings
{
   public class CurrencyDto
    {
        public short Id { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
    }
}
