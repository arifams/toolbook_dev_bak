namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Refactor_Migration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountSettings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CustomerId = c.Long(nullable: false),
                        DefaultLanguageId = c.Short(nullable: false),
                        DefaultCurrencyId = c.Short(nullable: false),
                        DefaultTimeZoneId = c.Short(nullable: false),
                        WeightMetricId = c.Short(nullable: false),
                        VolumeMetricId = c.Short(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .ForeignKey("dbo.Currencies", t => t.DefaultCurrencyId)
                .ForeignKey("dbo.Languages", t => t.DefaultLanguageId)
                .ForeignKey("dbo.TimeZones", t => t.DefaultTimeZoneId)
                .ForeignKey("dbo.VolumeMetrics", t => t.VolumeMetricId)
                .ForeignKey("dbo.WeightMetrics", t => t.WeightMetricId)
                .Index(t => t.CustomerId)
                .Index(t => t.DefaultLanguageId)
                .Index(t => t.DefaultCurrencyId)
                .Index(t => t.DefaultTimeZoneId)
                .Index(t => t.WeightMetricId)
                .Index(t => t.VolumeMetricId);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Salutation = c.String(),
                        FirstName = c.String(),
                        MiddleName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        SecondaryEmail = c.String(),
                        PhoneNumber = c.String(),
                        MobileNumber = c.String(),
                        IsCorpAddressUseAsBusinessAddress = c.Boolean(nullable: false),
                        JobCapacity = c.String(),
                        AddressId = c.Long(nullable: false),
                        UserId = c.String(maxLength: 128),
                        UserName = c.String(),
                        Password = c.String(),
                        SelectedColour = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.AddressId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.AddressId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Country = c.String(),
                        ZipCode = c.String(),
                        Number = c.String(),
                        StreetAddress1 = c.String(),
                        StreetAddress2 = c.String(),
                        City = c.String(),
                        State = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        TenantId = c.Long(nullable: false),
                        Salutation = c.String(nullable: false, maxLength: 10),
                        FirstName = c.String(nullable: false, maxLength: 100),
                        MiddleName = c.String(maxLength: 100),
                        LastName = c.String(nullable: false, maxLength: 100),
                        Level = c.Byte(nullable: false),
                        JoinDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        LastLoginTime = c.DateTime(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.TenantId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Tenants",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TenancyName = c.String(),
                        IsCorporateAccount = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserInDivisions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        DivisionId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Divisions", t => t.DivisionId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.DivisionId);
            
            CreateTable(
                "dbo.Divisions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        DefaultCostCenterId = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        CompanyId = c.Long(nullable: false),
                        Type = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.CostCenters", t => t.DefaultCostCenterId)
                .Index(t => t.DefaultCostCenterId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        TenantId = c.Long(nullable: false),
                        COCNumber = c.String(),
                        VATNumber = c.String(),
                        CompanyCode = c.String(),
                        IsInvoiceEnabled = c.Boolean(nullable: false),
                        LogoUrl = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.CostCenters",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        PhoneNumber = c.String(),
                        BillingAddressId = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        CompanyId = c.Long(nullable: false),
                        Type = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.BillingAddressId)
                .Index(t => t.BillingAddressId);
            
            CreateTable(
                "dbo.DivisionCostCenters",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CostCenterId = c.Long(nullable: false),
                        DivisionId = c.Long(nullable: false),
                        IsAssigned = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CostCenters", t => t.CostCenterId)
                .ForeignKey("dbo.Divisions", t => t.DivisionId)
                .Index(t => t.CostCenterId)
                .Index(t => t.DivisionId);
            
            CreateTable(
                "dbo.Currencies",
                c => new
                    {
                        Id = c.Short(nullable: false, identity: true),
                        CurrencyCode = c.String(),
                        CurrencyName = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Languages",
                c => new
                    {
                        Id = c.Short(nullable: false, identity: true),
                        LanguageCode = c.String(),
                        LanguageName = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TimeZones",
                c => new
                    {
                        Id = c.Short(nullable: false, identity: true),
                        TimeZoneCode = c.String(),
                        CountryName = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.VolumeMetrics",
                c => new
                    {
                        Id = c.Short(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WeightMetrics",
                c => new
                    {
                        Id = c.Short(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AddressBooks",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CompanyName = c.String(),
                        UserId = c.String(),
                        Salutation = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        EmailAddress = c.String(),
                        PhoneNumber = c.String(),
                        AccountNumber = c.String(),
                        Country = c.String(),
                        ZipCode = c.String(),
                        Number = c.String(),
                        StreetAddress1 = c.String(),
                        StreetAddress2 = c.String(),
                        City = c.String(),
                        State = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AuditTrails",
                c => new
                    {
                        Id = c.Short(nullable: false, identity: true),
                        ReferenceId = c.String(),
                        AppFunctionality = c.Short(nullable: false),
                        Result = c.String(),
                        Comments = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Carriers",
                c => new
                    {
                        Id = c.Short(nullable: false, identity: true),
                        Name = c.String(),
                        CarrierNameLong = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CarrierServices",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CarrierType = c.Short(nullable: false),
                        ServiceLevel = c.String(),
                        CarrierCountryCode = c.String(),
                        CarrierAccountNumber = c.String(),
                        CarrierId = c.Short(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Carriers", t => t.CarrierId)
                .Index(t => t.CarrierId);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Secret = c.String(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        Active = c.Boolean(nullable: false),
                        ApplicationType = c.Int(nullable: false),
                        RefreshTokenLifeTime = c.Int(nullable: false),
                        AllowedOrigin = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CommercialInvoices",
                c => new
                    {
                        ShipmentId = c.Long(nullable: false),
                        ShipmentReferenceName = c.String(),
                        ShipTo = c.String(),
                        InvoiceNo = c.String(),
                        InvoiceTo = c.String(),
                        VatNo = c.String(),
                        CustomerNo = c.String(),
                        TermsOfPayment = c.String(),
                        ShipmentService = c.Short(nullable: false),
                        CountryOfOrigin = c.String(),
                        CountryOfDestination = c.String(),
                        ModeOfTransport = c.String(),
                        ImportBroker = c.String(),
                        Note = c.String(),
                        ValueCurrency = c.Short(nullable: false),
                        InvoiceItemId = c.Long(nullable: false),
                        HSCode = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ShipmentId)
                .ForeignKey("dbo.InvoiceItems", t => t.InvoiceItemId)
                .ForeignKey("dbo.Shipments", t => t.ShipmentId)
                .Index(t => t.ShipmentId)
                .Index(t => t.InvoiceItemId);
            
            CreateTable(
                "dbo.InvoiceItems",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.InvoiceItemLines",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Description = c.String(),
                        Quantity = c.Short(nullable: false),
                        PricePerPiece = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InvoiceItemId = c.Long(nullable: false),
                        HSCode = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.InvoiceItems", t => t.InvoiceItemId)
                .Index(t => t.InvoiceItemId);
            
            CreateTable(
                "dbo.Shipments",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ShipmentName = c.String(),
                        ShipmentReferenceName = c.String(),
                        ShipmentCode = c.String(),
                        DivisionId = c.Long(),
                        CostCenterId = c.Long(),
                        ShipmentMode = c.Short(nullable: false),
                        ShipmentService = c.Short(nullable: false),
                        TarriffType = c.String(),
                        TariffText = c.String(),
                        ServiceLevel = c.String(),
                        PickUpDate = c.DateTime(),
                        Status = c.Short(nullable: false),
                        TrackingNumber = c.String(),
                        ConsignorId = c.Long(nullable: false),
                        ConsigneeId = c.Long(nullable: false),
                        ShipmentPackageId = c.Long(nullable: false),
                        ParentShipmentId = c.Long(),
                        IsParent = c.Boolean(nullable: false),
                        ShipmentPaymentTypeId = c.Short(nullable: false),
                        ManualStatusUpdatedDate = c.DateTime(),
                        IsFavourite = c.Boolean(nullable: false),
                        CarrierId = c.Short(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Carriers", t => t.CarrierId)
                .ForeignKey("dbo.ShipmentAddresses", t => t.ConsigneeId)
                .ForeignKey("dbo.ShipmentAddresses", t => t.ConsignorId)
                .ForeignKey("dbo.CostCenters", t => t.CostCenterId)
                .ForeignKey("dbo.Divisions", t => t.DivisionId)
                .ForeignKey("dbo.Shipments", t => t.ParentShipmentId)
                .ForeignKey("dbo.ShipmentPackages", t => t.ShipmentPackageId)
                .Index(t => t.DivisionId)
                .Index(t => t.CostCenterId)
                .Index(t => t.ConsignorId)
                .Index(t => t.ConsigneeId)
                .Index(t => t.ShipmentPackageId)
                .Index(t => t.ParentShipmentId)
                .Index(t => t.CarrierId);
            
            CreateTable(
                "dbo.ShipmentAddresses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CompanyName = c.String(),
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
                        ContactName = c.String(),
                        CreatedBy = c.String(),
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
                        WeightMetricId = c.Short(nullable: false),
                        TotalVolume = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VolumeMetricId = c.Short(nullable: false),
                        HSCode = c.String(),
                        CollectionDate = c.DateTime(nullable: false),
                        CarrierInstruction = c.String(),
                        IsInsured = c.Boolean(nullable: false),
                        InsuranceDeclaredValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InsuranceCurrencyType = c.Short(nullable: false),
                        IsDG = c.Boolean(nullable: false),
                        Accessibility = c.Boolean(nullable: false),
                        DGType = c.String(),
                        CarrierCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InsuranceCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentTypeId = c.Short(nullable: false),
                        EarliestPickupDate = c.DateTime(),
                        EstDeliveryDate = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Currencies", t => t.InsuranceCurrencyType)
                .ForeignKey("dbo.VolumeMetrics", t => t.VolumeMetricId)
                .ForeignKey("dbo.WeightMetrics", t => t.WeightMetricId)
                .Index(t => t.WeightMetricId)
                .Index(t => t.VolumeMetricId)
                .Index(t => t.InsuranceCurrencyType);
            
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
                        Description = c.String(),
                        ShipmentPackageId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ShipmentPackages", t => t.ShipmentPackageId)
                .Index(t => t.ShipmentPackageId);
            
            CreateTable(
                "dbo.ShipmentPayments",
                c => new
                    {
                        ShipmentId = c.Long(nullable: false),
                        SaleId = c.Long(nullable: false),
                        Status = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ShipmentId)
                .ForeignKey("dbo.Shipments", t => t.ShipmentId)
                .Index(t => t.ShipmentId);
            
            CreateTable(
                "dbo.CreditNotes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CreditNoteNumber = c.String(),
                        InvoiceId = c.Long(nullable: false),
                        CreditNoteValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        URL = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Invoices", t => t.InvoiceId)
                .Index(t => t.InvoiceId);
            
            CreateTable(
                "dbo.Invoices",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        InvoiceNumber = c.String(),
                        ShipmentId = c.Long(),
                        InvoiceValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InvoiceStatus = c.Short(nullable: false),
                        URL = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Shipments", t => t.ShipmentId)
                .Index(t => t.ShipmentId);
            
            CreateTable(
                "dbo.InvoiceDisputeHistories",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        DisputeComment = c.String(),
                        InvoiceId = c.Long(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Invoices", t => t.InvoiceId)
                .Index(t => t.InvoiceId);
            
            CreateTable(
                "dbo.LocationActivities",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Status = c.String(),
                        Date = c.DateTime(nullable: false),
                        Time = c.DateTime(nullable: false),
                        ShipmentLocationHistoryId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ShipmentLocationHistories", t => t.ShipmentLocationHistoryId)
                .Index(t => t.ShipmentLocationHistoryId);
            
            CreateTable(
                "dbo.ShipmentLocationHistories",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ShipmentId = c.Long(nullable: false),
                        Country = c.String(),
                        City = c.String(),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Shipments", t => t.ShipmentId)
                .Index(t => t.ShipmentId);
            
            CreateTable(
                "dbo.webpages_Membership",
                c => new
                    {
                        UserId = c.Long(nullable: false, identity: true),
                        CreateDate = c.DateTime(nullable: false),
                        ConfirmationToken = c.String(),
                        IsConfirmed = c.Boolean(nullable: false),
                        LastPasswordFailureDate = c.DateTime(nullable: false),
                        PasswordFailuresSinceLastSuccess = c.Int(nullable: false),
                        Password = c.String(),
                        PasswordChangedDate = c.DateTime(nullable: false),
                        PasswordSalt = c.String(),
                        PasswordVerificationToken = c.String(),
                        PasswordVerificationTokenExpirationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.NotificationCriterias",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BookingConfirmation = c.Boolean(nullable: false),
                        PickupConfirmation = c.Boolean(nullable: false),
                        ShipmentDelay = c.Boolean(nullable: false),
                        ShipmentException = c.Boolean(nullable: false),
                        NotifyNewSolution = c.Boolean(nullable: false),
                        NotifyDiscountOffer = c.Boolean(nullable: false),
                        CustomerId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .Index(t => t.CustomerId);
            
            CreateTable(
                "dbo.PaymentTypes",
                c => new
                    {
                        Id = c.Short(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Rates",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CountryFrom = c.String(),
                        IsInbound = c.Boolean(nullable: false),
                        Service = c.Short(nullable: false),
                        WeightMin = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WeightMax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Currency = c.Short(nullable: false),
                        CalculationMethod = c.Short(nullable: false),
                        VolumeFactor = c.Int(nullable: false),
                        MaxLength = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaxWeightPerPiece = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SellOrBuy = c.Short(nullable: false),
                        MaxDimension = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CarrierId = c.Long(nullable: false),
                        TariffTypeId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CarrierServices", t => t.CarrierId)
                .ForeignKey("dbo.TariffTypes", t => t.TariffTypeId)
                .Index(t => t.CarrierId)
                .Index(t => t.TariffTypeId);
            
            CreateTable(
                "dbo.RateZones",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RateId = c.Long(nullable: false),
                        ZoneId = c.Long(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Rates", t => t.RateId)
                .ForeignKey("dbo.Zones", t => t.ZoneId)
                .Index(t => t.RateId)
                .Index(t => t.ZoneId);
            
            CreateTable(
                "dbo.Zones",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CountryFrom = c.String(),
                        CountryTo = c.String(),
                        ZoneName = c.String(),
                        LocationFrom = c.String(),
                        LocationTo = c.String(),
                        IsInbound = c.Boolean(nullable: false),
                        CarrierId = c.Long(nullable: false),
                        TariffTypeId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CarrierServices", t => t.CarrierId)
                .ForeignKey("dbo.TariffTypes", t => t.TariffTypeId)
                .Index(t => t.CarrierId)
                .Index(t => t.TariffTypeId);
            
            CreateTable(
                "dbo.TariffTypes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TarrifName = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TransmitTimes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CarrierId = c.Long(nullable: false),
                        CountryFrom = c.String(),
                        CountryTo = c.String(),
                        ZoneId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CarrierServices", t => t.CarrierId)
                .ForeignKey("dbo.Zones", t => t.ZoneId)
                .Index(t => t.CarrierId)
                .Index(t => t.ZoneId);
            
            CreateTable(
                "dbo.TransitTimeProducts",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TransmitTimeId = c.Long(nullable: false),
                        ProductType = c.Short(nullable: false),
                        Days = c.Short(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TransmitTimes", t => t.TransmitTimeId)
                .Index(t => t.TransmitTimeId);
            
            CreateTable(
                "dbo.RefreshTokens",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Subject = c.String(nullable: false, maxLength: 50),
                        ClientId = c.String(nullable: false, maxLength: 50),
                        IssuedUtc = c.DateTime(nullable: false),
                        ExpiresUtc = c.DateTime(nullable: false),
                        ProtectedTicket = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RoleHierarchies",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                        ParentName = c.String(),
                        Order = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.ShipmentDocuments",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TenantId = c.Long(nullable: false),
                        ShipmentId = c.Long(nullable: false),
                        ClientFileName = c.String(),
                        UploadedFileName = c.String(),
                        DocumentType = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Shipments", t => t.ShipmentId)
                .Index(t => t.ShipmentId);
            
            CreateTable(
                "dbo.TarrifTextCodes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TarrifText = c.String(),
                        CountryCode = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserProfiles",
                c => new
                    {
                        UserId = c.Long(nullable: false, identity: true),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShipmentDocuments", "ShipmentId", "dbo.Shipments");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Rates", "TariffTypeId", "dbo.TariffTypes");
            DropForeignKey("dbo.RateZones", "ZoneId", "dbo.Zones");
            DropForeignKey("dbo.TransmitTimes", "ZoneId", "dbo.Zones");
            DropForeignKey("dbo.TransitTimeProducts", "TransmitTimeId", "dbo.TransmitTimes");
            DropForeignKey("dbo.TransmitTimes", "CarrierId", "dbo.CarrierServices");
            DropForeignKey("dbo.Zones", "TariffTypeId", "dbo.TariffTypes");
            DropForeignKey("dbo.Zones", "CarrierId", "dbo.CarrierServices");
            DropForeignKey("dbo.RateZones", "RateId", "dbo.Rates");
            DropForeignKey("dbo.Rates", "CarrierId", "dbo.CarrierServices");
            DropForeignKey("dbo.NotificationCriterias", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.ShipmentLocationHistories", "ShipmentId", "dbo.Shipments");
            DropForeignKey("dbo.LocationActivities", "ShipmentLocationHistoryId", "dbo.ShipmentLocationHistories");
            DropForeignKey("dbo.InvoiceDisputeHistories", "InvoiceId", "dbo.Invoices");
            DropForeignKey("dbo.Invoices", "ShipmentId", "dbo.Shipments");
            DropForeignKey("dbo.CreditNotes", "InvoiceId", "dbo.Invoices");
            DropForeignKey("dbo.CommercialInvoices", "ShipmentId", "dbo.Shipments");
            DropForeignKey("dbo.ShipmentPayments", "ShipmentId", "dbo.Shipments");
            DropForeignKey("dbo.Shipments", "ShipmentPackageId", "dbo.ShipmentPackages");
            DropForeignKey("dbo.ShipmentPackages", "WeightMetricId", "dbo.WeightMetrics");
            DropForeignKey("dbo.ShipmentPackages", "VolumeMetricId", "dbo.VolumeMetrics");
            DropForeignKey("dbo.PackageProducts", "ShipmentPackageId", "dbo.ShipmentPackages");
            DropForeignKey("dbo.ShipmentPackages", "InsuranceCurrencyType", "dbo.Currencies");
            DropForeignKey("dbo.Shipments", "ParentShipmentId", "dbo.Shipments");
            DropForeignKey("dbo.Shipments", "DivisionId", "dbo.Divisions");
            DropForeignKey("dbo.Shipments", "CostCenterId", "dbo.CostCenters");
            DropForeignKey("dbo.Shipments", "ConsignorId", "dbo.ShipmentAddresses");
            DropForeignKey("dbo.Shipments", "ConsigneeId", "dbo.ShipmentAddresses");
            DropForeignKey("dbo.Shipments", "CarrierId", "dbo.Carriers");
            DropForeignKey("dbo.CommercialInvoices", "InvoiceItemId", "dbo.InvoiceItems");
            DropForeignKey("dbo.InvoiceItemLines", "InvoiceItemId", "dbo.InvoiceItems");
            DropForeignKey("dbo.CarrierServices", "CarrierId", "dbo.Carriers");
            DropForeignKey("dbo.AccountSettings", "WeightMetricId", "dbo.WeightMetrics");
            DropForeignKey("dbo.AccountSettings", "VolumeMetricId", "dbo.VolumeMetrics");
            DropForeignKey("dbo.AccountSettings", "DefaultTimeZoneId", "dbo.TimeZones");
            DropForeignKey("dbo.AccountSettings", "DefaultLanguageId", "dbo.Languages");
            DropForeignKey("dbo.AccountSettings", "DefaultCurrencyId", "dbo.Currencies");
            DropForeignKey("dbo.AccountSettings", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.Customers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserInDivisions", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserInDivisions", "DivisionId", "dbo.Divisions");
            DropForeignKey("dbo.Divisions", "DefaultCostCenterId", "dbo.CostCenters");
            DropForeignKey("dbo.DivisionCostCenters", "DivisionId", "dbo.Divisions");
            DropForeignKey("dbo.DivisionCostCenters", "CostCenterId", "dbo.CostCenters");
            DropForeignKey("dbo.CostCenters", "BillingAddressId", "dbo.Addresses");
            DropForeignKey("dbo.Divisions", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Companies", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.AspNetUsers", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Customers", "AddressId", "dbo.Addresses");
            DropIndex("dbo.ShipmentDocuments", new[] { "ShipmentId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.TransitTimeProducts", new[] { "TransmitTimeId" });
            DropIndex("dbo.TransmitTimes", new[] { "ZoneId" });
            DropIndex("dbo.TransmitTimes", new[] { "CarrierId" });
            DropIndex("dbo.Zones", new[] { "TariffTypeId" });
            DropIndex("dbo.Zones", new[] { "CarrierId" });
            DropIndex("dbo.RateZones", new[] { "ZoneId" });
            DropIndex("dbo.RateZones", new[] { "RateId" });
            DropIndex("dbo.Rates", new[] { "TariffTypeId" });
            DropIndex("dbo.Rates", new[] { "CarrierId" });
            DropIndex("dbo.NotificationCriterias", new[] { "CustomerId" });
            DropIndex("dbo.ShipmentLocationHistories", new[] { "ShipmentId" });
            DropIndex("dbo.LocationActivities", new[] { "ShipmentLocationHistoryId" });
            DropIndex("dbo.InvoiceDisputeHistories", new[] { "InvoiceId" });
            DropIndex("dbo.Invoices", new[] { "ShipmentId" });
            DropIndex("dbo.CreditNotes", new[] { "InvoiceId" });
            DropIndex("dbo.ShipmentPayments", new[] { "ShipmentId" });
            DropIndex("dbo.PackageProducts", new[] { "ShipmentPackageId" });
            DropIndex("dbo.ShipmentPackages", new[] { "InsuranceCurrencyType" });
            DropIndex("dbo.ShipmentPackages", new[] { "VolumeMetricId" });
            DropIndex("dbo.ShipmentPackages", new[] { "WeightMetricId" });
            DropIndex("dbo.Shipments", new[] { "CarrierId" });
            DropIndex("dbo.Shipments", new[] { "ParentShipmentId" });
            DropIndex("dbo.Shipments", new[] { "ShipmentPackageId" });
            DropIndex("dbo.Shipments", new[] { "ConsigneeId" });
            DropIndex("dbo.Shipments", new[] { "ConsignorId" });
            DropIndex("dbo.Shipments", new[] { "CostCenterId" });
            DropIndex("dbo.Shipments", new[] { "DivisionId" });
            DropIndex("dbo.InvoiceItemLines", new[] { "InvoiceItemId" });
            DropIndex("dbo.CommercialInvoices", new[] { "InvoiceItemId" });
            DropIndex("dbo.CommercialInvoices", new[] { "ShipmentId" });
            DropIndex("dbo.CarrierServices", new[] { "CarrierId" });
            DropIndex("dbo.DivisionCostCenters", new[] { "DivisionId" });
            DropIndex("dbo.DivisionCostCenters", new[] { "CostCenterId" });
            DropIndex("dbo.CostCenters", new[] { "BillingAddressId" });
            DropIndex("dbo.Companies", new[] { "TenantId" });
            DropIndex("dbo.Divisions", new[] { "CompanyId" });
            DropIndex("dbo.Divisions", new[] { "DefaultCostCenterId" });
            DropIndex("dbo.UserInDivisions", new[] { "DivisionId" });
            DropIndex("dbo.UserInDivisions", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "TenantId" });
            DropIndex("dbo.Customers", new[] { "UserId" });
            DropIndex("dbo.Customers", new[] { "AddressId" });
            DropIndex("dbo.AccountSettings", new[] { "VolumeMetricId" });
            DropIndex("dbo.AccountSettings", new[] { "WeightMetricId" });
            DropIndex("dbo.AccountSettings", new[] { "DefaultTimeZoneId" });
            DropIndex("dbo.AccountSettings", new[] { "DefaultCurrencyId" });
            DropIndex("dbo.AccountSettings", new[] { "DefaultLanguageId" });
            DropIndex("dbo.AccountSettings", new[] { "CustomerId" });
            DropTable("dbo.UserProfiles");
            DropTable("dbo.TarrifTextCodes");
            DropTable("dbo.ShipmentDocuments");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.RoleHierarchies");
            DropTable("dbo.RefreshTokens");
            DropTable("dbo.TransitTimeProducts");
            DropTable("dbo.TransmitTimes");
            DropTable("dbo.TariffTypes");
            DropTable("dbo.Zones");
            DropTable("dbo.RateZones");
            DropTable("dbo.Rates");
            DropTable("dbo.PaymentTypes");
            DropTable("dbo.NotificationCriterias");
            DropTable("dbo.webpages_Membership");
            DropTable("dbo.ShipmentLocationHistories");
            DropTable("dbo.LocationActivities");
            DropTable("dbo.InvoiceDisputeHistories");
            DropTable("dbo.Invoices");
            DropTable("dbo.CreditNotes");
            DropTable("dbo.ShipmentPayments");
            DropTable("dbo.PackageProducts");
            DropTable("dbo.ShipmentPackages");
            DropTable("dbo.ShipmentAddresses");
            DropTable("dbo.Shipments");
            DropTable("dbo.InvoiceItemLines");
            DropTable("dbo.InvoiceItems");
            DropTable("dbo.CommercialInvoices");
            DropTable("dbo.Clients");
            DropTable("dbo.CarrierServices");
            DropTable("dbo.Carriers");
            DropTable("dbo.AuditTrails");
            DropTable("dbo.AddressBooks");
            DropTable("dbo.WeightMetrics");
            DropTable("dbo.VolumeMetrics");
            DropTable("dbo.TimeZones");
            DropTable("dbo.Languages");
            DropTable("dbo.Currencies");
            DropTable("dbo.DivisionCostCenters");
            DropTable("dbo.CostCenters");
            DropTable("dbo.Companies");
            DropTable("dbo.Divisions");
            DropTable("dbo.UserInDivisions");
            DropTable("dbo.Tenants");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Addresses");
            DropTable("dbo.Customers");
            DropTable("dbo.AccountSettings");
        }
    }
}
