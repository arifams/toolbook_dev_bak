namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ph2Customerthemecolour : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "SelectedColour", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "SelectedColour");
        }
    }
}
