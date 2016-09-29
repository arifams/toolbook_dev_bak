namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedStatusCodeforShipmentPayment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShipmentPayments", "StatusCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShipmentPayments", "StatusCode");
        }
    }
}
