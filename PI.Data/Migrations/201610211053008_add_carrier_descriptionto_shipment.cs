namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_carrier_descriptionto_shipment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shipments", "CarrierDescription", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shipments", "CarrierDescription");
        }
    }
}
