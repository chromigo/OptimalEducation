namespace OptimalEducation.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SchoolMarkCanBeNull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SchoolMarks", "Result", c => c.Short());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SchoolMarks", "Result", c => c.Short(nullable: false));
        }
    }
}
