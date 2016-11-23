namespace PI.Data.Migrations
{
    using Entity;
    using Entity.Identity;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Diagnostics;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<PI.Data.PIContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }


        private static void BuildClientsList(PI.Data.PIContext context)
        {

            //List<Client> ClientsList = new List<Client>
            //{
            //    //Google
            //    new Client
            //    { Id = "657439870432-g98gvt35aceavp0ou6vsr3b6372m3cmr.apps.googleusercontent.com",
            //        Secret= "WsjF353NEonbaFZMgTyMJl4h",
            //        Name="Parcel International",
            //        Active = true,
            //        RefreshTokenLifeTime = 7200,
            //        AllowedOrigin = "*"
            //    },
            //    //Facebook
            //    new Client
            //    { Id = "1753464874877402",
            //        Secret= "4cbc794bf7555a0dfda6585ef2b6418d",
            //        Name="Parcel International",
            //        Active = true,
            //        RefreshTokenLifeTime = 7200,
            //        AllowedOrigin = "*"
            //    },
            //    //Microsoft
            //    new Client
            //    { Id = "c2c7d7e5-1b51-4170-af5d-c1d584f1f10b",
            //        Secret= "vxfYc5vafgoLaMVwpyBxj5N",
            //        Name="Parcel International",
            //        Active = true,
            //        RefreshTokenLifeTime = 7200,
            //        AllowedOrigin = "*"
            //    },
                
            //};
            //ClientsList.ForEach(x => context.Clients.AddOrUpdate(x));
         
        }

        protected override void Seed(PI.Data.PIContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            //  This method will be called after migrating to the latest version.


            context.Languages.AddOrUpdate(
                x => x.Id,
                new Entity.Language() { Id = 1, LanguageCode = "EN", LanguageName = "ENGLISH", CreatedBy = "1", CreatedDate = DateTime.UtcNow, IsActive = true},
                new Entity.Language() { Id = 2, LanguageCode = "NL", LanguageName = "DUTCH", CreatedBy = "1", CreatedDate = DateTime.UtcNow, IsActive = false },
                new Entity.Language() { Id = 3, LanguageCode = "ZH", LanguageName = "CHINESE", CreatedBy = "1", CreatedDate = DateTime.UtcNow, IsActive = false },
                new Entity.Language() { Id = 4, LanguageCode = "DE", LanguageName = "GERMAN", CreatedBy = "1", CreatedDate = DateTime.UtcNow, IsActive = false }
                );

            context.Currencies.AddOrUpdate(
                x => x.Id,
                new Entity.Currency() { Id = 1, CurrencyCode = "USD", CurrencyName = "USD", CreatedBy = "1", CreatedDate = DateTime.UtcNow, IsActive = true },
                new Entity.Currency() { Id = 2, CurrencyCode = "EUR", CurrencyName = "EURO", CreatedBy = "1", CreatedDate = DateTime.UtcNow, IsActive = false },
                new Entity.Currency() { Id = 3, CurrencyCode = "YN", CurrencyName = "YEN", CreatedBy = "1", CreatedDate = DateTime.UtcNow, IsActive = false },
                new Entity.Currency() { Id = 4, CurrencyCode = "GBP", CurrencyName = "Pound", CreatedBy = "1", CreatedDate = DateTime.UtcNow, IsActive = false }
                );

            // Insert timezone
            //foreach (var info in TimeZoneInfo.GetSystemTimeZones())
            //{
            //    context.TimeZones.AddOrUpdate(x => x.TimeZoneId, new Entity.TimeZone()
            //    {
            //        TimeZoneId = info.Id,
            //        DisplayName = info.DisplayName,
            //        CreatedBy = "1",
            //        CreatedDate = DateTime.UtcNow,
            //        IsActive = true
            //    });
            //}

            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new PIContext()));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new PIContext()));

            context.Tenants.AddOrUpdate(x => x.Id,
                 new Entity.Tenant() { Id = 1, TenancyName = "MC", IsCorporateAccount = true, IsActive = true, IsDelete = true, CreatedBy = "1", CreatedDate = DateTime.UtcNow });


            var user = new ApplicationUser()
            {
                TenantId = 1,
                UserName = "SuperPowerUser",
                Email = "admin@pi.com",
                EmailConfirmed = true,
                Salutation = "Mr",
                FirstName = "Admin",
                LastName = "User",
                Level = 1,
                JoinDate = DateTime.UtcNow.AddYears(-3),
                IsActive = true,
                IsDeleted = false
            };

            //manager.Create(user, "1qaz2wsx@");

            if (roleManager.Roles.Count() == 0)
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
                roleManager.Create(new IdentityRole { Name = "BusinessOwner" });
                roleManager.Create(new IdentityRole { Name = "Manager" });
                roleManager.Create(new IdentityRole { Name = "Supervisor" });
                roleManager.Create(new IdentityRole { Name = "Operator" });
            }

            // Set Roles Hierarchies - Add Id column to maintain the order list.
            context.RoleHierarchies.AddOrUpdate(
            new Entity.RoleHierarchy() { Name = "Admin", ParentName = "", Order = 1 },
            new Entity.RoleHierarchy() { Name = "BusinessOwner", ParentName = "Admin", Order = 2 },
            new Entity.RoleHierarchy() { Name = "Manager", ParentName = "BusinessOwner", Order = 3 },
            new Entity.RoleHierarchy() { Name = "Supervisor", ParentName = "Manager", Order = 4 },
            new Entity.RoleHierarchy() { Name = "Operator", ParentName = "Supervisor", Order = 5 }
            );

            //var adminUser = manager.FindByName("SuperPowerUser");
            //manager.AddToRoles(adminUser.Id, new string[] { "Admin" });

            // Add records for shipments.
            //context.ShipmentTypes.AddOrUpdate(
            //    new Entity.ShipmentType() { Code = "DDP", Name = "Door to Door, Prepaid" },
            //    new Entity.ShipmentType() { Code = "DPP", Name = "Door to Port, Prepaid" },
            //    new Entity.ShipmentType() { Code = "PDP", Name = "Port to Door, Prepaid" },
            //    new Entity.ShipmentType() { Code = "PPP", Name = "Port to Port, Prepaid" },
            //    new Entity.ShipmentType() { Code = "FCA", Name = "Free Carrier" }
            //);

            //context.ShipmentTerms.AddOrUpdate(
            //    new Entity.ShipmentTerm() { Code = "DDU", Name = "Delivered Duty Unpaid" },
            //    new Entity.ShipmentTerm() { Code = "DDP", Name = "Delivered Duty Paid" },
            //    new Entity.ShipmentTerm() { Code = "CIP", Name = "Carriage and Insurance Paid" },
            //    new Entity.ShipmentTerm() { Code = "CPT", Name = "Carriage Paid To" },
            //    new Entity.ShipmentTerm() { Code = "EXW", Name = "Ex Works" }
            //);

            //context.ShipmentModes.AddOrUpdate(
            //    new Entity.ShipmentMode() { Name = "Express" },
            //    new Entity.ShipmentMode() { Name = "Air Freight" },
            //    new Entity.ShipmentMode() { Name = "Sea Freight" },
            //    new Entity.ShipmentMode() { Name = "Road Freight" }
            //);
            
            context.VolumeMetrics.AddOrUpdate(
                x => x.Id,
                new Entity.VolumeMetric() { Id = 1, Name = "kg",IsActive = true,IsDelete = false, CreatedBy = "1", CreatedDate = DateTime.UtcNow },
                new Entity.VolumeMetric() { Id = 2, Name = "lbs", IsActive = true, IsDelete = false , CreatedBy = "1", CreatedDate = DateTime.UtcNow }
                );

            context.WeightMetrics.AddOrUpdate(
                x => x.Id,
                new Entity.WeightMetric() { Id = 1, Name = "cm", IsActive = true, IsDelete = false, CreatedBy = "1", CreatedDate = DateTime.UtcNow },
                new Entity.WeightMetric() { Id = 2, Name = "m", IsActive = true, IsDelete = false , CreatedBy = "1", CreatedDate = DateTime.UtcNow }
                );

            context.Carrier.AddOrUpdate(
                x => x.Id,
                new Entity.Carrier() { Id = 1, Name = "TNT", IsActive = true, IsDelete = false, CreatedBy = "1", CreatedDate = DateTime.UtcNow },
                new Entity.Carrier() { Id = 2, Name = "UPS", IsActive = true, IsDelete = false, CreatedBy = "1", CreatedDate = DateTime.UtcNow },
                new Entity.Carrier() { Id = 3, Name = "FEDEX", IsActive = true, IsDelete = false, CreatedBy = "1", CreatedDate = DateTime.UtcNow },
                new Entity.Carrier() { Id = 4, Name = "USP", CarrierNameLong = "USPS" ,IsActive = true, IsDelete = false, CreatedBy = "1", CreatedDate = DateTime.UtcNow },
                new Entity.Carrier() { Id = 5, Name = "FED", IsActive = true, IsDelete = false, CreatedBy = "1", CreatedDate = DateTime.UtcNow }
            );

            if (context.Clients.Count() > 0)
            {
                return;
            }
            else
            {
                BuildClientsList(context);
            }
        }
    }
}
