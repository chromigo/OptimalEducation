using System.Data.Entity.Migrations;

namespace OptimalEducation.DAL.Migrations
{
    public partial class ExamType2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ExamDisciplines", "EducationForm", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ExamDisciplines", "EducationForm", c => c.Int());
        }
    }
}
