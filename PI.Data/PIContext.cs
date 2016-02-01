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
        public PIContext() : base("name=PIBookingConnectionString") 
        {
            
        }

        public DbSet<Customer> Customers { get; set; }
    }
}
