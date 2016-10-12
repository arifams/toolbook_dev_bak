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
        public virtual DbSet<UserProfile> UserProfiles { get; set; }
        public virtual DbSet<webpages_Membership> Membership { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Division> Divisions { get; set; }
        public virtual DbSet<CostCenter> CostCenters { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<Entity.TimeZone> TimeZones { get; set; }
        public virtual DbSet<AccountSettings> AccountSettings { get; set; }
        public virtual DbSet<NotificationCriteria> NotificationCriterias { get; set; }
        public virtual DbSet<Tenant> Tenants { get; set; }
        public virtual DbSet<DivisionCostCenter> DivisionCostCenters { get; set; }
        public virtual DbSet<UserInDivision> UsersInDivisions { get; set; }
        public virtual DbSet<AddressBook> AddressBooks { get; set; }
        public virtual DbSet<RoleHierarchy> RoleHierarchies { get; set; }

        // Shipment
        public virtual DbSet<Shipment> Shipments { get; set; }
        public virtual DbSet<ShipmentAddress> ShipmentAddresses { get; set; }
        public virtual DbSet<ShipmentPackage> ShipmentPackages { get; set; }
        public virtual DbSet<PackageProduct> PackageProducts { get; set; }
        public virtual DbSet<ShipmentLocationHistory> ShipmentLocationHistories { get; set; }
        public virtual DbSet<LocationActivity> LocationActivities { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<InvoiceDisputeHistory> InvoiceDisputeHistories { get; set; }

        public virtual DbSet<VolumeMetric> VolumeMetrics { get; set; }
        public virtual DbSet<WeightMetric> WeightMetrics { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<ShipmentDocument> ShipmentDocument { get; set; }
              
        public virtual DbSet<CommercialInvoice> CommercialInvoices { get; set; }
              
        public virtual DbSet<CarrierService> CarrierService { get; set; }
              
        public virtual DbSet<Carrier> Carrier { get; set; }
             
        public virtual DbSet<Rate> Rate { get; set; }
              
        public virtual DbSet<Zone> Zone { get; set; }
              
        public virtual DbSet<TransmitTime> TransmitTime { get; set; }
              
        public virtual DbSet<TariffType> TariffType { get; set; }
               
        public virtual DbSet<AuditTrail> AuditTrail { get; set; }           
              
        public virtual DbSet<CreditNote> CreditNotes { get; set; }
               
        public virtual DbSet<TarrifTextCode> TarrifTextCodes { get; set; }
               
        public virtual DbSet<Client> Clients { get; set; }
              
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        public virtual DbSet<Country> Countries { get; set; }


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
