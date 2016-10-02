namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_Sum_in_Invoive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoices", "Sum", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Invoices", "Sum");
        }
    }
}
