namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ph2Addweighvolumematrix : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccountSettings", "WeightMetricId", c => c.Short(nullable: false));
            AddColumn("dbo.AccountSettings", "VolumeMetricId", c => c.Short(nullable: false));
            CreateIndex("dbo.AccountSettings", "WeightMetricId");
            CreateIndex("dbo.AccountSettings", "VolumeMetricId");
            AddForeignKey("dbo.AccountSettings", "VolumeMetricId", "dbo.VolumeMetrics", "Id");
            AddForeignKey("dbo.AccountSettings", "WeightMetricId", "dbo.WeightMetrics", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountSettings", "WeightMetricId", "dbo.WeightMetrics");
            DropForeignKey("dbo.AccountSettings", "VolumeMetricId", "dbo.VolumeMetrics");
            DropIndex("dbo.AccountSettings", new[] { "VolumeMetricId" });
            DropIndex("dbo.AccountSettings", new[] { "WeightMetricId" });
            DropColumn("dbo.AccountSettings", "VolumeMetricId");
            DropColumn("dbo.AccountSettings", "WeightMetricId");
        }
    }
}
