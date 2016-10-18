using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Postmen
{
    public class PMMeta
    {
        public float code { get; set; }
        public string message { get; set; }
        public List<PMDetails> details { get; set; }
        public bool retryable { get; set; }
        public string data { get; set; }
    }
}
