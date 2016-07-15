namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ph2Addcompanynametoshipmentaddress : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.ShipmentAddresses", "CompanyName", c => c.String());
        }
        
        public override void Down()
        {
            //DropColumn("dbo.ShipmentAddresses", "CompanyName");
        }
    }
}
