using System.Data.Entity.Migrations;

namespace OptimalEducation.DAL.Migrations
{
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
