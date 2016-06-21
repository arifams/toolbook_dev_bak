namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateInvoiceLineItemwithHSCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InvoiceItemLines", "HSCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.InvoiceItemLines", "HSCode");
        }
    }
}
