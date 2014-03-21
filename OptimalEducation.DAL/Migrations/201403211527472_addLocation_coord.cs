namespace OptimalEducation.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Spatial;
    
    public partial class addLocation_coord : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cities", "Location", c => c.Geography());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cities", "Location");
        }
    }
}
