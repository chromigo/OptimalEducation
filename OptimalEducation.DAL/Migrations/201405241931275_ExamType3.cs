namespace OptimalEducation.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
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
