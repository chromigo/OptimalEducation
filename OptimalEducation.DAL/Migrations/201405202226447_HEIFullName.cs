namespace OptimalEducation.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HEIFullName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HigherEducationInstitutions", "FullName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.HigherEducationInstitutions", "FullName");
        }
    }
}
