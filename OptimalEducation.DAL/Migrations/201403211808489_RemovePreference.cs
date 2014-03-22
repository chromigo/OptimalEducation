namespace OptimalEducation.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovePreference : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PreferenceCities", "Preference_Id", "dbo.Preferences");
            DropForeignKey("dbo.PreferenceCities", "City_Id", "dbo.Cities");
            DropForeignKey("dbo.Preferences", "EntrantId", "dbo.Entrants");
            DropIndex("dbo.Preferences", new[] { "EntrantId" });
            DropIndex("dbo.PreferenceCities", new[] { "Preference_Id" });
            DropIndex("dbo.PreferenceCities", new[] { "City_Id" });
            DropTable("dbo.Preferences");
            DropTable("dbo.PreferenceCities");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PreferenceCities",
                c => new
                    {
                        Preference_Id = c.Int(nullable: false),
                        City_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Preference_Id, t.City_Id });
            
            CreateTable(
                "dbo.Preferences",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Presige = c.Int(nullable: false),
                        EducationFrom = c.String(),
                        EntrantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.PreferenceCities", "City_Id");
            CreateIndex("dbo.PreferenceCities", "Preference_Id");
            CreateIndex("dbo.Preferences", "EntrantId");
            AddForeignKey("dbo.Preferences", "EntrantId", "dbo.Entrants", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PreferenceCities", "City_Id", "dbo.Cities", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PreferenceCities", "Preference_Id", "dbo.Preferences", "Id", cascadeDelete: true);
        }
    }
}
