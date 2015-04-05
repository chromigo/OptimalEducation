using System.Data.Entity.Migrations;

namespace OptimalEducation.DAL.Migrations
{
    public partial class EducationFormEnum : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EducationLines", "EducationForm", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EducationLines", "EducationForm", c => c.String());
        }
    }
}
