using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract
{
    public interface ILogger
    {
        void Info(string message);
        void Warn(string message);
        void Error(string message);
        void Fatal(string message);
        void Debug(string message);
    }
}
