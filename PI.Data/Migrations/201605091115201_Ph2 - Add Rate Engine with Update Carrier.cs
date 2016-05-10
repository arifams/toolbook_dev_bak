namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ph2AddRateEnginewithUpdateCarrier : DbMigration
    {
        public override void Up()
        {
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
            
            AddColumn("dbo.Shipments", "CarrierId", c => c.Short(nullable: false));
            AlterColumn("dbo.Shipments", "ShipmentMode", c => c.Short(nullable: false));
            CreateIndex("dbo.Shipments", "CarrierId");
            AddForeignKey("dbo.Shipments", "CarrierId", "dbo.Carriers", "Id");
            DropColumn("dbo.Shipments", "CarrierName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Shipments", "CarrierName", c => c.String(maxLength: 200));
            DropForeignKey("dbo.Rates", "TariffTypeId", "dbo.TariffTypes");
            DropForeignKey("dbo.RateZones", "ZoneId", "dbo.Zones");
            DropForeignKey("dbo.TransmitTimes", "ZoneId", "dbo.Zones");
            DropForeignKey("dbo.TransitTimeProducts", "TransmitTimeId", "dbo.TransmitTimes");
            DropForeignKey("dbo.TransmitTimes", "CarrierId", "dbo.CarrierServices");
            DropForeignKey("dbo.Zones", "TariffTypeId", "dbo.TariffTypes");
            DropForeignKey("dbo.Zones", "CarrierId", "dbo.CarrierServices");
            DropForeignKey("dbo.RateZones", "RateId", "dbo.Rates");
            DropForeignKey("dbo.Rates", "CarrierId", "dbo.CarrierServices");
            DropForeignKey("dbo.Shipments", "CarrierId", "dbo.Carriers");
            DropForeignKey("dbo.CarrierServices", "CarrierId", "dbo.Carriers");
            DropIndex("dbo.TransitTimeProducts", new[] { "TransmitTimeId" });
            DropIndex("dbo.TransmitTimes", new[] { "ZoneId" });
            DropIndex("dbo.TransmitTimes", new[] { "CarrierId" });
            DropIndex("dbo.Zones", new[] { "TariffTypeId" });
            DropIndex("dbo.Zones", new[] { "CarrierId" });
            DropIndex("dbo.RateZones", new[] { "ZoneId" });
            DropIndex("dbo.RateZones", new[] { "RateId" });
            DropIndex("dbo.Rates", new[] { "TariffTypeId" });
            DropIndex("dbo.Rates", new[] { "CarrierId" });
            DropIndex("dbo.Shipments", new[] { "CarrierId" });
            DropIndex("dbo.CarrierServices", new[] { "CarrierId" });
            AlterColumn("dbo.Shipments", "ShipmentMode", c => c.String());
            DropColumn("dbo.Shipments", "CarrierId");
            DropTable("dbo.TransitTimeProducts");
            DropTable("dbo.TransmitTimes");
            DropTable("dbo.TariffTypes");
            DropTable("dbo.Zones");
            DropTable("dbo.RateZones");
            DropTable("dbo.Rates");
            DropTable("dbo.CarrierServices");
            DropTable("dbo.Carriers");
        }
    }
}
