namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateShipmentPaymentasacollection : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.ShipmentPayments");
            AddColumn("dbo.ShipmentPayments", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.ShipmentPayments", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.ShipmentPayments");
            DropColumn("dbo.ShipmentPayments", "Id");
            AddPrimaryKey("dbo.ShipmentPayments", "ShipmentId");
        }
    }
}
