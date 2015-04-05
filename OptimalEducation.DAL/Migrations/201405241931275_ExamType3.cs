using System.Data.Entity.Migrations;

namespace OptimalEducation.DAL.Migrations
{
    public partial class ExamType3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExamDisciplines", "ExamType", c => c.Int(nullable: false));
            DropColumn("dbo.ExamDisciplines", "EducationForm");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ExamDisciplines", "EducationForm", c => c.Int(nullable: false));
            DropColumn("dbo.ExamDisciplines", "ExamType");
        }
    }
}
