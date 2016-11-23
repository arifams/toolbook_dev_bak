namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class introducenewParentShipmentColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shipments", "MainShipment", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shipments", "MainShipment");
        }
    }
}
