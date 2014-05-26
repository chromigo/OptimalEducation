namespace OptimalEducation.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
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
