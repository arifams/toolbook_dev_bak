namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedstatuslength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.LocationActivities", "Status", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.LocationActivities", "Status", c => c.String(maxLength: 20));
        }
    }
}
