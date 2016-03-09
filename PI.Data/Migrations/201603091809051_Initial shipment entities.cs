namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initialshipmententities : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AccountSettings", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.AccountSettings", "DefaultCurrencyId", "dbo.Currencies");
            DropForeignKey("dbo.AccountSettings", "DefaultLanguageId", "dbo.Languages");
            DropForeignKey("dbo.AccountSettings", "DefaultTimeZoneId", "dbo.TimeZones");
            DropForeignKey("dbo.Customers", "AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Companies", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.CostCenters", "BillingAddressId", "dbo.Addresses");
            DropForeignKey("dbo.DivisionCostCenters", "CostCenterId", "dbo.CostCenters");
            DropForeignKey("dbo.DivisionCostCenters", "DivisionId", "dbo.Divisions");
            DropForeignKey("dbo.Divisions", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.UserInDivisions", "DivisionId", "dbo.Divisions");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.NotificationCriterias", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            CreateTable(
                "dbo.PackageProducts",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProductTypeId = c.Short(nullable: false),
                        Quantity = c.Short(nullable: false),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Height = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Length = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Width = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ShipmentPackageId = c.Long(nullable: false),
                        CreatedBy = c.Long(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PackageProductTypes", t => t.ProductTypeId)
                .ForeignKey("dbo.ShipmentPackages", t => t.ShipmentPackageId)
                .Index(t => t.ProductTypeId)
                .Index(t => t.ShipmentPackageId);
            
            CreateTable(
                "dbo.PackageProductTypes",
                c => new
                    {
                        Id = c.Short(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedBy = c.Long(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ShipmentPackages",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PackageDescription = c.String(),
                        TotalWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WeightMetric = c.String(),
                        TotalVolume = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VolumeMetric = c.String(),
                        HSCode = c.String(),
                        CollectionDate = c.DateTime(nullable: false),
                        CarrierInstruction = c.String(),
                        IsInsurance = c.Boolean(nullable: false),
                        InsuranceDeclaredValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InsuranceCurrencyType = c.String(),
                        CarrierCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InsuranceCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentTypeId = c.Short(nullable: false),
                        EarliestPickupDate = c.DateTime(nullable: false),
                        EstDeliveryDate = c.DateTime(nullable: false),
                        CreatedBy = c.Long(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PaymentTypes", t => t.PaymentTypeId)
                .Index(t => t.PaymentTypeId);
            
            CreateTable(
                "dbo.PaymentTypes",
                c => new
                    {
                        Id = c.Short(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedBy = c.Long(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ShipmentAddresses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Country = c.String(),
                        ZipCode = c.String(),
                        Number = c.String(),
                        StreetAddress1 = c.String(),
                        StreetAddress2 = c.String(),
                        City = c.String(),
                        State = c.String(),
                        EmailAddress = c.String(),
                        PhoneNumber = c.String(),
                        CreatedBy = c.Long(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ShipmentModes",
                c => new
                    {
                        Id = c.Short(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedBy = c.Long(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Shipments",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        DivisionId = c.Long(nullable: false),
                        CostCenterId = c.Long(nullable: false),
                        ShipmentModeId = c.Short(nullable: false),
                        ShipmentTypeId = c.Short(nullable: false),
                        ShipmentTermId = c.Short(nullable: false),
                        ConsignorId = c.Long(nullable: false),
                        ConsigneeId = c.Long(nullable: false),
                        ShipmentPackageId = c.Long(nullable: false),
                        CreatedBy = c.Long(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ShipmentAddresses", t => t.ConsigneeId)
                .ForeignKey("dbo.ShipmentAddresses", t => t.ConsignorId)
                .ForeignKey("dbo.CostCenters", t => t.CostCenterId)
                .ForeignKey("dbo.Divisions", t => t.DivisionId)
                .ForeignKey("dbo.ShipmentModes", t => t.ShipmentModeId)
                .ForeignKey("dbo.ShipmentPackages", t => t.ShipmentPackageId)
                .ForeignKey("dbo.ShipmentTerms", t => t.ShipmentTermId)
                .ForeignKey("dbo.ShipmentTypes", t => t.ShipmentTypeId)
                .Index(t => t.DivisionId)
                .Index(t => t.CostCenterId)
                .Index(t => t.ShipmentModeId)
                .Index(t => t.ShipmentTypeId)
                .Index(t => t.ShipmentTermId)
                .Index(t => t.ConsignorId)
                .Index(t => t.ConsigneeId)
                .Index(t => t.ShipmentPackageId);
            
            CreateTable(
                "dbo.ShipmentTerms",
                c => new
                    {
                        Id = c.Short(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedBy = c.Long(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ShipmentTypes",
                c => new
                    {
                        Id = c.Short(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedBy = c.Long(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddForeignKey("dbo.AccountSettings", "CustomerId", "dbo.Customers", "Id");
            AddForeignKey("dbo.AccountSettings", "DefaultCurrencyId", "dbo.Currencies", "Id");
            AddForeignKey("dbo.AccountSettings", "DefaultLanguageId", "dbo.Languages", "Id");
            AddForeignKey("dbo.AccountSettings", "DefaultTimeZoneId", "dbo.TimeZones", "Id");
            AddForeignKey("dbo.Customers", "AddressId", "dbo.Addresses", "Id");
            AddForeignKey("dbo.Companies", "TenantId", "dbo.Tenants", "Id");
            AddForeignKey("dbo.CostCenters", "BillingAddressId", "dbo.Addresses", "Id");
            AddForeignKey("dbo.DivisionCostCenters", "CostCenterId", "dbo.CostCenters", "Id");
            AddForeignKey("dbo.DivisionCostCenters", "DivisionId", "dbo.Divisions", "Id");
            AddForeignKey("dbo.Divisions", "CompanyId", "dbo.Companies", "Id");
            AddForeignKey("dbo.UserInDivisions", "DivisionId", "dbo.Divisions", "Id");
            AddForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.AspNetUsers", "TenantId", "dbo.Tenants", "Id");
            AddForeignKey("dbo.NotificationCriterias", "CustomerId", "dbo.Customers", "Id");
            AddForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.NotificationCriterias", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.AspNetUsers", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserInDivisions", "DivisionId", "dbo.Divisions");
            DropForeignKey("dbo.Divisions", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.DivisionCostCenters", "DivisionId", "dbo.Divisions");
            DropForeignKey("dbo.DivisionCostCenters", "CostCenterId", "dbo.CostCenters");
            DropForeignKey("dbo.CostCenters", "BillingAddressId", "dbo.Addresses");
            DropForeignKey("dbo.Companies", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.Customers", "AddressId", "dbo.Addresses");
            DropForeignKey("dbo.AccountSettings", "DefaultTimeZoneId", "dbo.TimeZones");
            DropForeignKey("dbo.AccountSettings", "DefaultLanguageId", "dbo.Languages");
            DropForeignKey("dbo.AccountSettings", "DefaultCurrencyId", "dbo.Currencies");
            DropForeignKey("dbo.AccountSettings", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.Shipments", "ShipmentTypeId", "dbo.ShipmentTypes");
            DropForeignKey("dbo.Shipments", "ShipmentTermId", "dbo.ShipmentTerms");
            DropForeignKey("dbo.Shipments", "ShipmentPackageId", "dbo.ShipmentPackages");
            DropForeignKey("dbo.Shipments", "ShipmentModeId", "dbo.ShipmentModes");
            DropForeignKey("dbo.Shipments", "DivisionId", "dbo.Divisions");
            DropForeignKey("dbo.Shipments", "CostCenterId", "dbo.CostCenters");
            DropForeignKey("dbo.Shipments", "ConsignorId", "dbo.ShipmentAddresses");
            DropForeignKey("dbo.Shipments", "ConsigneeId", "dbo.ShipmentAddresses");
            DropForeignKey("dbo.ShipmentPackages", "PaymentTypeId", "dbo.PaymentTypes");
            DropForeignKey("dbo.PackageProducts", "ShipmentPackageId", "dbo.ShipmentPackages");
            DropForeignKey("dbo.PackageProducts", "ProductTypeId", "dbo.PackageProductTypes");
            DropIndex("dbo.Shipments", new[] { "ShipmentPackageId" });
            DropIndex("dbo.Shipments", new[] { "ConsigneeId" });
            DropIndex("dbo.Shipments", new[] { "ConsignorId" });
            DropIndex("dbo.Shipments", new[] { "ShipmentTermId" });
            DropIndex("dbo.Shipments", new[] { "ShipmentTypeId" });
            DropIndex("dbo.Shipments", new[] { "ShipmentModeId" });
            DropIndex("dbo.Shipments", new[] { "CostCenterId" });
            DropIndex("dbo.Shipments", new[] { "DivisionId" });
            DropIndex("dbo.ShipmentPackages", new[] { "PaymentTypeId" });
            DropIndex("dbo.PackageProducts", new[] { "ShipmentPackageId" });
            DropIndex("dbo.PackageProducts", new[] { "ProductTypeId" });
            DropTable("dbo.ShipmentTypes");
            DropTable("dbo.ShipmentTerms");
            DropTable("dbo.Shipments");
            DropTable("dbo.ShipmentModes");
            DropTable("dbo.ShipmentAddresses");
            DropTable("dbo.PaymentTypes");
            DropTable("dbo.ShipmentPackages");
            DropTable("dbo.PackageProductTypes");
            DropTable("dbo.PackageProducts");
            AddForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles", "Id", cascadeDelete: true);
            AddForeignKey("dbo.NotificationCriterias", "CustomerId", "dbo.Customers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUsers", "TenantId", "dbo.Tenants", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.UserInDivisions", "DivisionId", "dbo.Divisions", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Divisions", "CompanyId", "dbo.Companies", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DivisionCostCenters", "DivisionId", "dbo.Divisions", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DivisionCostCenters", "CostCenterId", "dbo.CostCenters", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CostCenters", "BillingAddressId", "dbo.Addresses", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Companies", "TenantId", "dbo.Tenants", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Customers", "AddressId", "dbo.Addresses", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AccountSettings", "DefaultTimeZoneId", "dbo.TimeZones", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AccountSettings", "DefaultLanguageId", "dbo.Languages", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AccountSettings", "DefaultCurrencyId", "dbo.Currencies", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AccountSettings", "CustomerId", "dbo.Customers", "Id", cascadeDelete: true);
        }
    }
}
