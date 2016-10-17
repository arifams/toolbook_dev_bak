namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addShpmentErrorTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShipmentErrors",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ShipmentId = c.Long(nullable: false),
                        ErrorMessage = c.String(),
                        CreatedBy = c.String(maxLength: 50),
                        CreatedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Shipments", t => t.ShipmentId)
                .Index(t => t.ShipmentId);
            
            DropColumn("dbo.Shipments", "ErrorUrl");
            DropColumn("dbo.Shipments", "ErrorMessage");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Shipments", "ErrorMessage", c => c.String());
            AddColumn("dbo.Shipments", "ErrorUrl", c => c.String());
            DropForeignKey("dbo.ShipmentErrors", "ShipmentId", "dbo.Shipments");
            DropIndex("dbo.ShipmentErrors", new[] { "ShipmentId" });
            DropTable("dbo.ShipmentErrors");
        }
    }
}
