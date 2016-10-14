namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updated_Timezone_Entity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TimeZones", "TimeZoneId", c => c.String());
            AddColumn("dbo.TimeZones", "DisplayName", c => c.String());
            DropColumn("dbo.TimeZones", "TimeZoneCode");
            DropColumn("dbo.TimeZones", "CountryName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TimeZones", "CountryName", c => c.String());
            AddColumn("dbo.TimeZones", "TimeZoneCode", c => c.String());
            DropColumn("dbo.TimeZones", "DisplayName");
            DropColumn("dbo.TimeZones", "TimeZoneId");
        }
    }
}
