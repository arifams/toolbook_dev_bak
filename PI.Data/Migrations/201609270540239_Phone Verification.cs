namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PhoneVerification : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "MobileVerificationCode", c => c.String());
            AddColumn("dbo.AspNetUsers", "MobileVerificationExpiry", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "MobileVerificationExpiry");
            DropColumn("dbo.AspNetUsers", "MobileVerificationCode");
        }
    }
}
