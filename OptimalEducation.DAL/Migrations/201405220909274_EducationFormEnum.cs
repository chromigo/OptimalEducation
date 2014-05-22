namespace OptimalEducation.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
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
