namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ph3Addcliententityforexternalauthentication : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.Clients",
            //    c => new
            //        {
            //            Id = c.String(nullable: false, maxLength: 128),
            //            Secret = c.String(nullable: false),
            //            Name = c.String(nullable: false, maxLength: 100),
            //            Active = c.Boolean(nullable: false),
            //            RefreshTokenLifeTime = c.Int(nullable: false),
            //            AllowedOrigin = c.String(maxLength: 100),
            //        })
            //    .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            //DropTable("dbo.Clients");
        }
    }
}
