namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateShipmentPaymententityfields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShipmentPayments", "PaymentId", c => c.String());
            AlterColumn("dbo.ShipmentPayments", "Status", c => c.Short(nullable: false));
            DropColumn("dbo.ShipmentPayments", "SaleId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ShipmentPayments", "SaleId", c => c.Long(nullable: false));
            AlterColumn("dbo.ShipmentPayments", "Status", c => c.String(maxLength: 20));
            DropColumn("dbo.ShipmentPayments", "PaymentId");
        }
    }
}
