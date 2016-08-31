using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PI.Contract;
using log4net;

namespace PI.Common
{
    public class Log4NetLogger : ILogger
    {
        //private static readonly log4net.ILog log =
        //    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ILog log;

        public void SetType(Type type)
        {
            log = LogManager.GetLogger(type);
        }

        public void Debug(string message)
        {
            log.Debug(message);
        }

        public void Debug(string message,Exception exception)
        {
            log.Debug(message, exception);
        }

        public void Error(string message)
        {
            log.Error(message);
        }

        public void Error(string message, Exception exception)
        {
            log.Error(message, exception);
        }

        public void Fatal(string message)
        {
            log.Fatal(message);
        }

        public void Fatal(string message, Exception exception)
        {
            log.Fatal(message, exception);
        }

        public void Info(string message)
        {
            log.Info(message);
        }

        public void Info(string message, Exception exception)
        {
            log.Info(message, exception);
        }

        public void Warn(string message)
        {
            log.Warn(message);
        }

        public void Warn(string message, Exception exception)
        {
            log.Warn(message, exception);
        }
    }
}
