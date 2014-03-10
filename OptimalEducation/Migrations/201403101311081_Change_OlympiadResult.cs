namespace OptimalEducation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change_OlympiadResult : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ParticipationInOlympiads", "Result", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ParticipationInOlympiads", "Result", c => c.String());
        }
    }
}
