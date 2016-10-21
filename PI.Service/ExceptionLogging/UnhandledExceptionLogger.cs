using PI.Common;
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

    //public class UnhandledExceptionLogger : ExceptionHandler
    //{
    //    public override void Handle(ExceptionHandlerContext context)
    //    {
    //        if (context.Exception is ArgumentNullException)
    //        {
    //            var result = new HttpResponseMessage(HttpStatusCode.BadRequest)
    //            {
    //                Content = new StringContent(context.Exception.Message),
    //                ReasonPhrase = "ArgumentNullException"
    //            };

    //           // context.Result = new ArgumentNullResult(context.Request, result);
    //        }
    //        else
    //        {
    //            // Handle other exceptions, do other things
    //        }
    //    }
    //}

    public class UnhandledExceptionLogger : ExceptionFilterAttribute
    {
        Contract.ILogger logger = null;
        public UnhandledExceptionLogger()
        {
            this.logger = new Log4NetLogger();
            logger.SetType(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            string exceptionMessage = string.Empty;
            if (actionExecutedContext.Exception.InnerException == null)
            {
                exceptionMessage = actionExecutedContext.Exception.Message;
            }
            else
            {
                exceptionMessage = actionExecutedContext.Exception.InnerException.Message;
            }

            // Log error
            //var l = new Log4NetLogger();
            //l.SetType(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            logger.Error(exceptionMessage);

            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("An unhandled exception was thrown by service."),  
                ReasonPhrase = "Internal Server Error.Please Contact your Administrator."
            };
            actionExecutedContext.Response = response;
        }
    }
}