namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeShipmentPaymenttoPaymentandupdatefields : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ShipmentPayments", "ShipmentId", "dbo.Shipments");
            DropIndex("dbo.ShipmentPayments", new[] { "ShipmentId" });
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ReferenceId = c.Long(nullable: false),
                        PaymentType = c.Short(nullable: false),
                        PaymentId = c.String(),
                        Status = c.Short(nullable: false),
                        StatusCode = c.String(),
                        CreatedBy = c.String(maxLength: 50),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.ShipmentPayments");
            DropTable("dbo.PaymentTypes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PaymentTypes",
                c => new
                    {
                        Id = c.Short(nullable: false, identity: true),
                        Name = c.String(maxLength: 20),
                        CreatedBy = c.String(maxLength: 50),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ShipmentPayments",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ShipmentId = c.Long(nullable: false),
                        PaymentId = c.String(),
                        Status = c.Short(nullable: false),
                        StatusCode = c.String(),
                        CreatedBy = c.String(maxLength: 50),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.Payments");
            CreateIndex("dbo.ShipmentPayments", "ShipmentId");
            AddForeignKey("dbo.ShipmentPayments", "ShipmentId", "dbo.Shipments", "Id");
        }
    }
}
