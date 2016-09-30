namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_newfields_to_shpmentStatus_history : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShipmentLocationHistories", "State", c => c.String(maxLength: 100));
            AddColumn("dbo.ShipmentLocationHistories", "Zip", c => c.String(maxLength: 100));
            AddColumn("dbo.ShipmentLocationHistories", "Message", c => c.String(maxLength: 100));
            AddColumn("dbo.ShipmentLocationHistories", "DateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.ShipmentLocationHistories", "Status", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShipmentLocationHistories", "Status");
            DropColumn("dbo.ShipmentLocationHistories", "DateTime");
            DropColumn("dbo.ShipmentLocationHistories", "Message");
            DropColumn("dbo.ShipmentLocationHistories", "Zip");
            DropColumn("dbo.ShipmentLocationHistories", "State");
        }
    }
}
