using PI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs
{
    public class OperationResult
    {
        public OperationResult()
        {
            FieldList = new Dictionary<string, string>();
        }

        public Status Status { get; set; }
        public string Message { get; set; }
        public IDictionary<string,string> FieldList { get; set; }
    }
}
