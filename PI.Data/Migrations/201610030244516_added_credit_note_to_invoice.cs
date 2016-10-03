namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_credit_note_to_invoice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoices", "CreditAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Invoices", "CreditAmount");
        }
    }
}
