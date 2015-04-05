using System.Data.Entity.Migrations;

namespace OptimalEducation.DAL.Migrations
{
    public partial class HeiFullName : DbMigration
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
