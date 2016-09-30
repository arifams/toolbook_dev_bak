namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDueDates_TermsforInvoice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoices", "DueDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Invoices", "Terms", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Invoices", "Terms");
            DropColumn("dbo.Invoices", "DueDate");
        }
    }
}
