namespace OptimalEducation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fix_type : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EducationLines", "Price", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EducationLines", "Price", c => c.Boolean(nullable: false));
        }
    }
}
