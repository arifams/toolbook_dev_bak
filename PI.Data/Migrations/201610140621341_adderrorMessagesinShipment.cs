namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adderrorMessagesinShipment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shipments", "ErrorUrl", c => c.String());
            AddColumn("dbo.Shipments", "ErrorMessage", c => c.String());
            AddColumn("dbo.Shipments", "Provider", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shipments", "Provider");
            DropColumn("dbo.Shipments", "ErrorMessage");
            DropColumn("dbo.Shipments", "ErrorUrl");
        }
    }
}
