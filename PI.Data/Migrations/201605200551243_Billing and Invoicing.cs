namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BillingandInvoicing : DbMigration
    {
        public override void Up()
        {
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InvoiceDisputeHistories", "InvoiceId", "dbo.Invoices");
            DropForeignKey("dbo.Invoices", "ShipmentId", "dbo.Shipments");
            DropForeignKey("dbo.CreditNotes", "InvoiceId", "dbo.Invoices");
            DropIndex("dbo.InvoiceDisputeHistories", new[] { "InvoiceId" });
            DropIndex("dbo.Invoices", new[] { "ShipmentId" });
            DropIndex("dbo.CreditNotes", new[] { "InvoiceId" });
            DropTable("dbo.InvoiceDisputeHistories");
            DropTable("dbo.Invoices");
            DropTable("dbo.CreditNotes");
        }
    }
}
