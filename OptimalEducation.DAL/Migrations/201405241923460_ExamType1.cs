using System.Data.Entity.Migrations;

namespace OptimalEducation.DAL.Migrations
{
    public partial class ExamType1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExamDisciplines", "EducationForm", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExamDisciplines", "EducationForm");
        }
    }
}
