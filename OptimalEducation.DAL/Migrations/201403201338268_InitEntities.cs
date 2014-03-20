namespace OptimalEducation.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitEntities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Prestige = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.HigherEducationInstitutions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Prestige = c.Int(nullable: false),
                        CityId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cities", t => t.CityId, cascadeDelete: true)
                .Index(t => t.CityId);
            
            CreateTable(
                "dbo.Faculties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Prestige = c.Int(nullable: false),
                        HigherEducationInstitutionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.HigherEducationInstitutions", t => t.HigherEducationInstitutionId, cascadeDelete: true)
                .Index(t => t.HigherEducationInstitutionId);
            
            CreateTable(
                "dbo.EducationLines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GeneralEducationLineId = c.Int(),
                        FacultyId = c.Int(nullable: false),
                        Code = c.String(),
                        EducationForm = c.String(),
                        Name = c.String(),
                        RequiredSum = c.Int(),
                        Actual = c.Boolean(nullable: false),
                        Price = c.Int(nullable: false),
                        PaidPlacesNumber = c.Int(nullable: false),
                        FreePlacesNumber = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Faculties", t => t.FacultyId, cascadeDelete: true)
                .ForeignKey("dbo.GeneralEducationLines", t => t.GeneralEducationLineId)
                .Index(t => t.GeneralEducationLineId)
                .Index(t => t.FacultyId);
            
            CreateTable(
                "dbo.EducationLineRequirements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Requirement = c.Int(nullable: false),
                        EducationLineId = c.Int(nullable: false),
                        ExamDisciplineId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EducationLines", t => t.EducationLineId, cascadeDelete: true)
                .ForeignKey("dbo.ExamDisciplines", t => t.ExamDisciplineId, cascadeDelete: true)
                .Index(t => t.EducationLineId)
                .Index(t => t.ExamDisciplineId);
            
            CreateTable(
                "dbo.ExamDisciplines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UnitedStateExams",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Result = c.Int(nullable: false),
                        ExamDisciplineId = c.Int(nullable: false),
                        EntrantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ExamDisciplines", t => t.ExamDisciplineId, cascadeDelete: true)
                .ForeignKey("dbo.Entrants", t => t.EntrantId, cascadeDelete: true)
                .Index(t => t.ExamDisciplineId)
                .Index(t => t.EntrantId);
            
            CreateTable(
                "dbo.Entrants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Gender = c.String(),
                        SchoolEducation = c.String(),
                        Medal = c.String(),
                        Citizenship = c.String(),
                        AverageMark = c.Double(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Hobbies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Weights",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Coefficient = c.Double(nullable: false),
                        ClusterId = c.Int(nullable: false),
                        SchoolTypeId = c.Int(),
                        SchoolDisciplineId = c.Int(),
                        SectionId = c.Int(),
                        HobbieId = c.Int(),
                        OlympiadId = c.Int(),
                        ExamDisciplineId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clusters", t => t.ClusterId, cascadeDelete: true)
                .ForeignKey("dbo.ExamDisciplines", t => t.ExamDisciplineId)
                .ForeignKey("dbo.Hobbies", t => t.HobbieId)
                .ForeignKey("dbo.Olympiads", t => t.OlympiadId)
                .ForeignKey("dbo.SchoolDisciplines", t => t.SchoolDisciplineId)
                .ForeignKey("dbo.SchoolTypes", t => t.SchoolTypeId)
                .ForeignKey("dbo.Sections", t => t.SectionId)
                .Index(t => t.ClusterId)
                .Index(t => t.SchoolTypeId)
                .Index(t => t.SchoolDisciplineId)
                .Index(t => t.SectionId)
                .Index(t => t.HobbieId)
                .Index(t => t.OlympiadId)
                .Index(t => t.ExamDisciplineId);
            
            CreateTable(
                "dbo.Clusters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Olympiads",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ParticipationInOlympiads",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Result = c.Int(nullable: false),
                        EntrantId = c.Int(nullable: false),
                        OlympiadId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Entrants", t => t.EntrantId, cascadeDelete: true)
                .ForeignKey("dbo.Olympiads", t => t.OlympiadId, cascadeDelete: true)
                .Index(t => t.EntrantId)
                .Index(t => t.OlympiadId);
            
            CreateTable(
                "dbo.SchoolDisciplines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SchoolMarks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Result = c.Short(nullable: false),
                        Respect = c.Int(),
                        EntrantId = c.Int(nullable: false),
                        SchoolDisciplineId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Entrants", t => t.EntrantId, cascadeDelete: true)
                .ForeignKey("dbo.SchoolDisciplines", t => t.SchoolDisciplineId, cascadeDelete: true)
                .Index(t => t.EntrantId)
                .Index(t => t.SchoolDisciplineId);
            
            CreateTable(
                "dbo.SchoolTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Schools",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        EducationQuality = c.Int(nullable: false),
                        SchoolTypeId = c.Int(nullable: false),
                        City_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cities", t => t.City_Id)
                .ForeignKey("dbo.SchoolTypes", t => t.SchoolTypeId, cascadeDelete: true)
                .Index(t => t.SchoolTypeId)
                .Index(t => t.City_Id);
            
            CreateTable(
                "dbo.Sections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ActivityType = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ParticipationInSections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntrantsId = c.Int(nullable: false),
                        SectionId = c.Int(nullable: false),
                        YearPeriod = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Entrants", t => t.EntrantsId, cascadeDelete: true)
                .ForeignKey("dbo.Sections", t => t.SectionId, cascadeDelete: true)
                .Index(t => t.EntrantsId)
                .Index(t => t.SectionId);
            
            CreateTable(
                "dbo.Preferences",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Presige = c.Int(nullable: false),
                        EducationFrom = c.String(),
                        EntrantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Entrants", t => t.EntrantId, cascadeDelete: true)
                .Index(t => t.EntrantId);
            
            CreateTable(
                "dbo.GeneralEducationLines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.HobbieEntrants",
                c => new
                    {
                        Hobbie_Id = c.Int(nullable: false),
                        Entrant_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Hobbie_Id, t.Entrant_Id })
                .ForeignKey("dbo.Hobbies", t => t.Hobbie_Id, cascadeDelete: true)
                .ForeignKey("dbo.Entrants", t => t.Entrant_Id, cascadeDelete: true)
                .Index(t => t.Hobbie_Id)
                .Index(t => t.Entrant_Id);
            
            CreateTable(
                "dbo.SchoolEntrants",
                c => new
                    {
                        School_Id = c.Int(nullable: false),
                        Entrant_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.School_Id, t.Entrant_Id })
                .ForeignKey("dbo.Schools", t => t.School_Id, cascadeDelete: true)
                .ForeignKey("dbo.Entrants", t => t.Entrant_Id, cascadeDelete: true)
                .Index(t => t.School_Id)
                .Index(t => t.Entrant_Id);
            
            CreateTable(
                "dbo.PreferenceCities",
                c => new
                    {
                        Preference_Id = c.Int(nullable: false),
                        City_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Preference_Id, t.City_Id })
                .ForeignKey("dbo.Preferences", t => t.Preference_Id, cascadeDelete: true)
                .ForeignKey("dbo.Cities", t => t.City_Id, cascadeDelete: true)
                .Index(t => t.Preference_Id)
                .Index(t => t.City_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Faculties", "HigherEducationInstitutionId", "dbo.HigherEducationInstitutions");
            DropForeignKey("dbo.EducationLines", "GeneralEducationLineId", "dbo.GeneralEducationLines");
            DropForeignKey("dbo.EducationLines", "FacultyId", "dbo.Faculties");
            DropForeignKey("dbo.UnitedStateExams", "EntrantId", "dbo.Entrants");
            DropForeignKey("dbo.Preferences", "EntrantId", "dbo.Entrants");
            DropForeignKey("dbo.PreferenceCities", "City_Id", "dbo.Cities");
            DropForeignKey("dbo.PreferenceCities", "Preference_Id", "dbo.Preferences");
            DropForeignKey("dbo.Weights", "SectionId", "dbo.Sections");
            DropForeignKey("dbo.ParticipationInSections", "SectionId", "dbo.Sections");
            DropForeignKey("dbo.ParticipationInSections", "EntrantsId", "dbo.Entrants");
            DropForeignKey("dbo.Weights", "SchoolTypeId", "dbo.SchoolTypes");
            DropForeignKey("dbo.Schools", "SchoolTypeId", "dbo.SchoolTypes");
            DropForeignKey("dbo.SchoolEntrants", "Entrant_Id", "dbo.Entrants");
            DropForeignKey("dbo.SchoolEntrants", "School_Id", "dbo.Schools");
            DropForeignKey("dbo.Schools", "City_Id", "dbo.Cities");
            DropForeignKey("dbo.Weights", "SchoolDisciplineId", "dbo.SchoolDisciplines");
            DropForeignKey("dbo.SchoolMarks", "SchoolDisciplineId", "dbo.SchoolDisciplines");
            DropForeignKey("dbo.SchoolMarks", "EntrantId", "dbo.Entrants");
            DropForeignKey("dbo.Weights", "OlympiadId", "dbo.Olympiads");
            DropForeignKey("dbo.ParticipationInOlympiads", "OlympiadId", "dbo.Olympiads");
            DropForeignKey("dbo.ParticipationInOlympiads", "EntrantId", "dbo.Entrants");
            DropForeignKey("dbo.Weights", "HobbieId", "dbo.Hobbies");
            DropForeignKey("dbo.Weights", "ExamDisciplineId", "dbo.ExamDisciplines");
            DropForeignKey("dbo.Weights", "ClusterId", "dbo.Clusters");
            DropForeignKey("dbo.HobbieEntrants", "Entrant_Id", "dbo.Entrants");
            DropForeignKey("dbo.HobbieEntrants", "Hobbie_Id", "dbo.Hobbies");
            DropForeignKey("dbo.UnitedStateExams", "ExamDisciplineId", "dbo.ExamDisciplines");
            DropForeignKey("dbo.EducationLineRequirements", "ExamDisciplineId", "dbo.ExamDisciplines");
            DropForeignKey("dbo.EducationLineRequirements", "EducationLineId", "dbo.EducationLines");
            DropForeignKey("dbo.HigherEducationInstitutions", "CityId", "dbo.Cities");
            DropIndex("dbo.PreferenceCities", new[] { "City_Id" });
            DropIndex("dbo.PreferenceCities", new[] { "Preference_Id" });
            DropIndex("dbo.SchoolEntrants", new[] { "Entrant_Id" });
            DropIndex("dbo.SchoolEntrants", new[] { "School_Id" });
            DropIndex("dbo.HobbieEntrants", new[] { "Entrant_Id" });
            DropIndex("dbo.HobbieEntrants", new[] { "Hobbie_Id" });
            DropIndex("dbo.Preferences", new[] { "EntrantId" });
            DropIndex("dbo.ParticipationInSections", new[] { "SectionId" });
            DropIndex("dbo.ParticipationInSections", new[] { "EntrantsId" });
            DropIndex("dbo.Schools", new[] { "City_Id" });
            DropIndex("dbo.Schools", new[] { "SchoolTypeId" });
            DropIndex("dbo.SchoolMarks", new[] { "SchoolDisciplineId" });
            DropIndex("dbo.SchoolMarks", new[] { "EntrantId" });
            DropIndex("dbo.ParticipationInOlympiads", new[] { "OlympiadId" });
            DropIndex("dbo.ParticipationInOlympiads", new[] { "EntrantId" });
            DropIndex("dbo.Weights", new[] { "ExamDisciplineId" });
            DropIndex("dbo.Weights", new[] { "OlympiadId" });
            DropIndex("dbo.Weights", new[] { "HobbieId" });
            DropIndex("dbo.Weights", new[] { "SectionId" });
            DropIndex("dbo.Weights", new[] { "SchoolDisciplineId" });
            DropIndex("dbo.Weights", new[] { "SchoolTypeId" });
            DropIndex("dbo.Weights", new[] { "ClusterId" });
            DropIndex("dbo.UnitedStateExams", new[] { "EntrantId" });
            DropIndex("dbo.UnitedStateExams", new[] { "ExamDisciplineId" });
            DropIndex("dbo.EducationLineRequirements", new[] { "ExamDisciplineId" });
            DropIndex("dbo.EducationLineRequirements", new[] { "EducationLineId" });
            DropIndex("dbo.EducationLines", new[] { "FacultyId" });
            DropIndex("dbo.EducationLines", new[] { "GeneralEducationLineId" });
            DropIndex("dbo.Faculties", new[] { "HigherEducationInstitutionId" });
            DropIndex("dbo.HigherEducationInstitutions", new[] { "CityId" });
            DropTable("dbo.PreferenceCities");
            DropTable("dbo.SchoolEntrants");
            DropTable("dbo.HobbieEntrants");
            DropTable("dbo.GeneralEducationLines");
            DropTable("dbo.Preferences");
            DropTable("dbo.ParticipationInSections");
            DropTable("dbo.Sections");
            DropTable("dbo.Schools");
            DropTable("dbo.SchoolTypes");
            DropTable("dbo.SchoolMarks");
            DropTable("dbo.SchoolDisciplines");
            DropTable("dbo.ParticipationInOlympiads");
            DropTable("dbo.Olympiads");
            DropTable("dbo.Clusters");
            DropTable("dbo.Weights");
            DropTable("dbo.Hobbies");
            DropTable("dbo.Entrants");
            DropTable("dbo.UnitedStateExams");
            DropTable("dbo.ExamDisciplines");
            DropTable("dbo.EducationLineRequirements");
            DropTable("dbo.EducationLines");
            DropTable("dbo.Faculties");
            DropTable("dbo.HigherEducationInstitutions");
            DropTable("dbo.Cities");
        }
    }
}
