namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ph2AddTarifTextentity : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.TarrifTextCodes",
            //    c => new
            //        {
            //            Id = c.Long(nullable: false, identity: true),
            //            TarrifText = c.String(),
            //            CountryCode = c.String(),
            //            CreatedBy = c.String(),
            //            CreatedDate = c.DateTime(nullable: false),
            //            IsActive = c.Boolean(nullable: false),
            //            IsDelete = c.Boolean(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            //DropTable("dbo.TarrifTextCodes");
        }
    }
}
