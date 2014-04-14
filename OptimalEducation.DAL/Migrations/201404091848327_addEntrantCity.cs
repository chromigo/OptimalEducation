namespace OptimalEducation.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addEntrantCity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Entrants", "City_Id", c => c.Int());
            CreateIndex("dbo.Entrants", "City_Id");
            AddForeignKey("dbo.Entrants", "City_Id", "dbo.Cities", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Entrants", "City_Id", "dbo.Cities");
            DropIndex("dbo.Entrants", new[] { "City_Id" });
            DropColumn("dbo.Entrants", "City_Id");
        }
    }
}
