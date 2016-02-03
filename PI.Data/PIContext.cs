using PI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data
{
    public class PIContext : DbContext
    {

        /// <summary>
        /// database context for the current thread
        /// </summary>
        [ThreadStatic]
        private static PIContext context = null;

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Company> Companies { get; set; }

        public PIContext()
            : base("name=PIBookingConnectionString")
        {

        }

        public static PIContext Get()
        {
            if (context == null)
            {
                context = new PIContext();
            }

            return context;
        }


        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        /// <summary>
        /// Ensure that the context doesn't get disposed
        /// if it is being reused by an inner method
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                base.Dispose(disposing);
            }

        }
    }
}
