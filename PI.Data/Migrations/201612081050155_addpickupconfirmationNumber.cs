namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addpickupconfirmationNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shipments", "PickupConfirmationNumber", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shipments", "PickupConfirmationNumber");
        }
    }
}
