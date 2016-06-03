namespace PI.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ph2Addedcolumnforfavouriteshipments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shipments", "IsFavourite", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shipments", "IsFavourite");
        }
    }
}
