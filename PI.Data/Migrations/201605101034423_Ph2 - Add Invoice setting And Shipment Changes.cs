namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ph2AddInvoicesettingAndShipmentChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "IsInvoiceEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.Shipments", "ManualStatusUpdatedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shipments", "ManualStatusUpdatedDate");
            DropColumn("dbo.Companies", "IsInvoiceEnabled");
        }
    }
}
