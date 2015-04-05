using System.Data.Entity.Migrations;

namespace OptimalEducation.DAL.Migrations
{
    public partial class ResultCanBeEmpty : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UnitedStateExams", "Result", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UnitedStateExams", "Result", c => c.Int(nullable: false));
        }
    }
}
