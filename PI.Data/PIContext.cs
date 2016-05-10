using Microsoft.AspNet.Identity.EntityFramework;
using PI.Data.Entity;
using PI.Data.Entity.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PI.Data.Entity;
using PI.Data.Entity.RateEngine;

namespace PI.Data
{
    public class PIContext : IdentityDbContext<ApplicationUser>
    {

        /// <summary>
        /// database context for the current thread
        /// </summary>
        [ThreadStatic]
        private static PIContext context = null;       
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<webpages_Membership> Membership { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<CostCenter> CostCenters { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Entity.TimeZone> TimeZones { get; set; }
        public DbSet<AccountSettings> AccountSettings { get; set; }
        public DbSet<NotificationCriteria> NotificationCriterias { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<DivisionCostCenter> DivisionCostCenters { get; set; }
        public DbSet<UserInDivision> UsersInDivisions { get; set; }
        public DbSet<AddressBook> AddressBooks { get; set; }
        public DbSet<RoleHierarchy> RoleHierarchies { get; set; }

        // Shipment
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ShipmentAddress> ShipmentAddresses { get; set; }
        public DbSet<ShipmentPackage> ShipmentPackages { get; set; }
        public DbSet<PackageProduct> PackageProducts { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<ShipmentLocationHistory> ShipmentLocationHistories { get; set; }
        public DbSet<LocationActivity> LocationActivities { get; set; }


        public DbSet<VolumeMetric> VolumeMetrics { get; set; }
        public DbSet<WeightMetric> WeightMetrics { get; set; }
        public DbSet<ShipmentPayment> ShipmentPayments { get; set; }
        public DbSet<ShipmentDocument> ShipmentDocument { get; set; }

        public DbSet<CommercialInvoice> CommercialInvoices { get; set; }
        
        public DbSet<CarrierService> CarrierService { get; set; }

        public DbSet<Carrier> Carrier { get; set; }

        public DbSet<Rate> Rate { get; set; }

        public DbSet<Zone> Zone { get; set; }

        public DbSet<TransmitTime> TransmitTime { get; set; }

        public DbSet<TariffType> TariffType { get; set; }
        
        public PIContext()
            : base("name=PIBookingConnectionString")
        {
            Configuration.LazyLoadingEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Configure domain classes using modelBuilder here
            base.OnModelCreating(modelBuilder);

            // For defaultcostcenterid
            modelBuilder.Entity<Division>()
            .HasRequired(s => s.CostCenter)
            .WithMany()
            .WillCascadeOnDelete(false);

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }


        public static PIContext Create()
        {
            return new PIContext();
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
