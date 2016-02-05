using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web.Security;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using PI.Data;
using System.Data.Entity.Infrastructure;

namespace PI.Business
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class InitializeSimpleMembershipAttribute : ActionFilterAttribute
    {
        //private static SimpleMembershipInitializer _initializer;
        //private static object _initializerLock = new object();
        //private static bool _isInitialized;

        //public override void OnActionExecuting(HttpActionContext filterContext)
        //{
        //    // Ensure ASP.NET Simple Membership is initialized only once per app start
        //    LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        //}

        //private class SimpleMembershipInitializer
        //{
        //    public SimpleMembershipInitializer()
        //    {
        //        Database.SetInitializer<PIContext>(null);

        //        try
        //        {
        //            using (var context = new PIContext())
        //            {
        //                if (!context.Database.Exists())
        //                {
        //                    // Create the SimpleMembership database without Entity Framework migration schema
        //                    ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
        //                }
        //            }

        //            WebSecurity.InitializeDatabaseConnection("PIBookingConnectionString", "UserProfile", "UserId", "UserName", autoCreateTables: true);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
        //        }
        //    }
        //}
    }
}