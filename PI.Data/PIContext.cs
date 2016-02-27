﻿using PI.Data.Entity;
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
        //public DbSet<DivisionCostCenter> DivisionCostCenters { get; set; }

        public PIContext()
            : base("name=PIBookingConnectionString")
        {
            Configuration.LazyLoadingEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Configure domain classes using modelBuilder here

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Division>()
                .HasMany<CostCenter>(d => d.CostCenters)
                .WithMany(c => c.Divisions)
                .Map(cs =>
                {
                    cs.MapLeftKey("DivisionId");
                    cs.MapRightKey("CostCenterId");
                    cs.ToTable("DivisionCostCenter");
                });

            //modelBuilder.Entity<CostCenter>()
            // .HasRequired(c => c.Divisions)
            // .WithMany()
            // .WillCascadeOnDelete(false);

            modelBuilder.Entity<Division>()
            .HasRequired(s => s.CostCenter)
            .WithMany()
            .WillCascadeOnDelete(false);
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
