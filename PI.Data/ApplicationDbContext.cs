using Microsoft.AspNet.Identity.EntityFramework;
using PI.Data.Entity.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private static ApplicationDbContext context = null;

        public ApplicationDbContext()
            : base("name=PIBookingConnectionString", throwIfV1Schema: false)
        {
            Configuration.ProxyCreationEnabled = true;
            Configuration.LazyLoadingEnabled = true;
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public static ApplicationDbContext Get()
        {
            if (context == null)
            {
                context = new ApplicationDbContext();
            }

            return context;
        }

    }
}
