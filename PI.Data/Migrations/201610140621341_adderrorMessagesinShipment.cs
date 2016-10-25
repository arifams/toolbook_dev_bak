namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adderrorMessagesinShipment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shipments", "Provider", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shipments", "Provider");
        }
    }
}
