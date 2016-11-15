namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Payment_Entity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payments", "LocationId", c => c.String());
            AddColumn("dbo.Payments", "TransactionId", c => c.String());
            AddColumn("dbo.Payments", "TenderId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Payments", "TenderId");
            DropColumn("dbo.Payments", "TransactionId");
            DropColumn("dbo.Payments", "LocationId");
        }
    }
}
