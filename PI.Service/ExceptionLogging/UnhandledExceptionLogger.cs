using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;

namespace PI.Service.ExceptionLogging
{
    /// <summary>
    /// Action filter to handle for Global application errors.
    /// </summary>
    //public class UnhandledExceptionLogger : ExceptionLogger
    //{
    //    public override void Log(ExceptionLoggerContext context)
    //    {
    //        var log = context.Exception.ToString();
    //        //Do whatever logging you need to do here.
    //    }
    //}

    public class UnhandledExceptionLogger : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            if (context.Exception is ArgumentNullException)
            {
                var result = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(context.Exception.Message),
                    ReasonPhrase = "ArgumentNullException"
                };

               // context.Result = new ArgumentNullResult(context.Request, result);
            }
            else
            {
                // Handle other exceptions, do other things
            }
        }
    }
}