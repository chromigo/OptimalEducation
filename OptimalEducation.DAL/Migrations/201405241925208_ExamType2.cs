namespace OptimalEducation.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
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
