namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updatepaymentwithamountandcurrency : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payments", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Payments", "CurrencyType", c => c.Short(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Payments", "CurrencyType");
            DropColumn("dbo.Payments", "Amount");
        }
    }
}
