namespace OptimalEducation.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SchoolAndSchoolType : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SchoolEntrants", "School_Id", "dbo.Schools");
            DropForeignKey("dbo.SchoolEntrants", "Entrant_Id", "dbo.Entrants");
            DropForeignKey("dbo.Schools", "SchoolTypeId", "dbo.SchoolTypes");
            DropForeignKey("dbo.Weights", "SchoolTypeId", "dbo.SchoolTypes");
            DropIndex("dbo.Weights", new[] { "SchoolTypeId" });
            DropIndex("dbo.Schools", new[] { "SchoolTypeId" });
            DropIndex("dbo.SchoolEntrants", new[] { "School_Id" });
            DropIndex("dbo.SchoolEntrants", new[] { "Entrant_Id" });
            CreateTable(
                "dbo.ParticipationInSchools",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntrantsId = c.Int(nullable: false),
                        SchoolId = c.Int(nullable: false),
                        YearPeriod = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Entrants", t => t.EntrantsId, cascadeDelete: true)
                .ForeignKey("dbo.Schools", t => t.SchoolId, cascadeDelete: true)
                .Index(t => t.EntrantsId)
                .Index(t => t.SchoolId);
            
            AddColumn("dbo.Weights", "SchoolId", c => c.Int());
            CreateIndex("dbo.Weights", "SchoolId");
            AddForeignKey("dbo.Weights", "SchoolId", "dbo.Schools", "Id");
            DropColumn("dbo.Weights", "SchoolTypeId");
            DropColumn("dbo.Schools", "SchoolTypeId");
            DropTable("dbo.SchoolTypes");
            DropTable("dbo.SchoolEntrants");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SchoolEntrants",
                c => new
                    {
                        School_Id = c.Int(nullable: false),
                        Entrant_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.School_Id, t.Entrant_Id });
            
            CreateTable(
                "dbo.SchoolTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Schools", "SchoolTypeId", c => c.Int(nullable: false));
            AddColumn("dbo.Weights", "SchoolTypeId", c => c.Int());
            DropForeignKey("dbo.Weights", "SchoolId", "dbo.Schools");
            DropForeignKey("dbo.ParticipationInSchools", "SchoolId", "dbo.Schools");
            DropForeignKey("dbo.ParticipationInSchools", "EntrantsId", "dbo.Entrants");
            DropIndex("dbo.ParticipationInSchools", new[] { "SchoolId" });
            DropIndex("dbo.ParticipationInSchools", new[] { "EntrantsId" });
            DropIndex("dbo.Weights", new[] { "SchoolId" });
            DropColumn("dbo.Weights", "SchoolId");
            DropTable("dbo.ParticipationInSchools");
            CreateIndex("dbo.SchoolEntrants", "Entrant_Id");
            CreateIndex("dbo.SchoolEntrants", "School_Id");
            CreateIndex("dbo.Schools", "SchoolTypeId");
            CreateIndex("dbo.Weights", "SchoolTypeId");
            AddForeignKey("dbo.Weights", "SchoolTypeId", "dbo.SchoolTypes", "Id");
            AddForeignKey("dbo.Schools", "SchoolTypeId", "dbo.SchoolTypes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SchoolEntrants", "Entrant_Id", "dbo.Entrants", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SchoolEntrants", "School_Id", "dbo.Schools", "Id", cascadeDelete: true);
        }
    }
}
