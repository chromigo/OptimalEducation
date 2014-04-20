namespace OptimalEducation.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initialize : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Characteristics",
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
                        CharacterisicId = c.Int(nullable: false),
                        SchoolId = c.Int(),
                        SchoolDisciplineId = c.Int(),
                        SectionId = c.Int(),
                        HobbieId = c.Int(),
                        OlympiadId = c.Int(),
                        ExamDisciplineId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Characteristics", t => t.CharacterisicId, cascadeDelete: true)
                .ForeignKey("dbo.Hobbies", t => t.HobbieId)
                .ForeignKey("dbo.Olympiads", t => t.OlympiadId)
                .ForeignKey("dbo.Sections", t => t.SectionId)
                .ForeignKey("dbo.SchoolDisciplines", t => t.SchoolDisciplineId)
                .ForeignKey("dbo.Schools", t => t.SchoolId)
                .ForeignKey("dbo.ExamDisciplines", t => t.ExamDisciplineId)
                .Index(t => t.CharacterisicId)
                .Index(t => t.SchoolId)
                .Index(t => t.SchoolDisciplineId)
                .Index(t => t.SectionId)
                .Index(t => t.HobbieId)
                .Index(t => t.OlympiadId)
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
                "dbo.Cities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Prestige = c.Int(nullable: false),
                        Location = c.Geography(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Schools",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        EducationQuality = c.Int(nullable: false),
                        City_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cities", t => t.City_Id)
                .Index(t => t.City_Id);
            
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
                        City_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cities", t => t.City_Id)
                .Index(t => t.City_Id);
            
            CreateTable(
                "dbo.Hobbies",
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
                "dbo.Olympiads",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
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
                "dbo.Sections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ActivityType = c.String(),
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
                "dbo.SchoolDisciplines",
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Weights", "ExamDisciplineId", "dbo.ExamDisciplines");
            DropForeignKey("dbo.EducationLineRequirements", "ExamDisciplineId", "dbo.ExamDisciplines");
            DropForeignKey("dbo.EducationLines", "GeneralEducationLineId", "dbo.GeneralEducationLines");
            DropForeignKey("dbo.Faculties", "HigherEducationInstitutionId", "dbo.HigherEducationInstitutions");
            DropForeignKey("dbo.Weights", "SchoolId", "dbo.Schools");
            DropForeignKey("dbo.ParticipationInSchools", "SchoolId", "dbo.Schools");
            DropForeignKey("dbo.UnitedStateExams", "EntrantId", "dbo.Entrants");
            DropForeignKey("dbo.UnitedStateExams", "ExamDisciplineId", "dbo.ExamDisciplines");
            DropForeignKey("dbo.Weights", "SchoolDisciplineId", "dbo.SchoolDisciplines");
            DropForeignKey("dbo.SchoolMarks", "SchoolDisciplineId", "dbo.SchoolDisciplines");
            DropForeignKey("dbo.SchoolMarks", "EntrantId", "dbo.Entrants");
            DropForeignKey("dbo.Weights", "SectionId", "dbo.Sections");
            DropForeignKey("dbo.ParticipationInSections", "SectionId", "dbo.Sections");
            DropForeignKey("dbo.ParticipationInSections", "EntrantsId", "dbo.Entrants");
            DropForeignKey("dbo.ParticipationInSchools", "EntrantsId", "dbo.Entrants");
            DropForeignKey("dbo.Weights", "OlympiadId", "dbo.Olympiads");
            DropForeignKey("dbo.ParticipationInOlympiads", "OlympiadId", "dbo.Olympiads");
            DropForeignKey("dbo.ParticipationInOlympiads", "EntrantId", "dbo.Entrants");
            DropForeignKey("dbo.Weights", "HobbieId", "dbo.Hobbies");
            DropForeignKey("dbo.HobbieEntrants", "Entrant_Id", "dbo.Entrants");
            DropForeignKey("dbo.HobbieEntrants", "Hobbie_Id", "dbo.Hobbies");
            DropForeignKey("dbo.Entrants", "City_Id", "dbo.Cities");
            DropForeignKey("dbo.Schools", "City_Id", "dbo.Cities");
            DropForeignKey("dbo.HigherEducationInstitutions", "CityId", "dbo.Cities");
            DropForeignKey("dbo.EducationLines", "FacultyId", "dbo.Faculties");
            DropForeignKey("dbo.EducationLineRequirements", "EducationLineId", "dbo.EducationLines");
            DropForeignKey("dbo.Weights", "CharacterisicId", "dbo.Characteristics");
            DropIndex("dbo.HobbieEntrants", new[] { "Entrant_Id" });
            DropIndex("dbo.HobbieEntrants", new[] { "Hobbie_Id" });
            DropIndex("dbo.UnitedStateExams", new[] { "EntrantId" });
            DropIndex("dbo.UnitedStateExams", new[] { "ExamDisciplineId" });
            DropIndex("dbo.SchoolMarks", new[] { "SchoolDisciplineId" });
            DropIndex("dbo.SchoolMarks", new[] { "EntrantId" });
            DropIndex("dbo.ParticipationInSections", new[] { "SectionId" });
            DropIndex("dbo.ParticipationInSections", new[] { "EntrantsId" });
            DropIndex("dbo.ParticipationInOlympiads", new[] { "OlympiadId" });
            DropIndex("dbo.ParticipationInOlympiads", new[] { "EntrantId" });
            DropIndex("dbo.Entrants", new[] { "City_Id" });
            DropIndex("dbo.ParticipationInSchools", new[] { "SchoolId" });
            DropIndex("dbo.ParticipationInSchools", new[] { "EntrantsId" });
            DropIndex("dbo.Schools", new[] { "City_Id" });
            DropIndex("dbo.HigherEducationInstitutions", new[] { "CityId" });
            DropIndex("dbo.Faculties", new[] { "HigherEducationInstitutionId" });
            DropIndex("dbo.EducationLines", new[] { "FacultyId" });
            DropIndex("dbo.EducationLines", new[] { "GeneralEducationLineId" });
            DropIndex("dbo.EducationLineRequirements", new[] { "ExamDisciplineId" });
            DropIndex("dbo.EducationLineRequirements", new[] { "EducationLineId" });
            DropIndex("dbo.Weights", new[] { "ExamDisciplineId" });
            DropIndex("dbo.Weights", new[] { "OlympiadId" });
            DropIndex("dbo.Weights", new[] { "HobbieId" });
            DropIndex("dbo.Weights", new[] { "SectionId" });
            DropIndex("dbo.Weights", new[] { "SchoolDisciplineId" });
            DropIndex("dbo.Weights", new[] { "SchoolId" });
            DropIndex("dbo.Weights", new[] { "CharacterisicId" });
            DropTable("dbo.HobbieEntrants");
            DropTable("dbo.GeneralEducationLines");
            DropTable("dbo.UnitedStateExams");
            DropTable("dbo.SchoolDisciplines");
            DropTable("dbo.SchoolMarks");
            DropTable("dbo.Sections");
            DropTable("dbo.ParticipationInSections");
            DropTable("dbo.Olympiads");
            DropTable("dbo.ParticipationInOlympiads");
            DropTable("dbo.Hobbies");
            DropTable("dbo.Entrants");
            DropTable("dbo.ParticipationInSchools");
            DropTable("dbo.Schools");
            DropTable("dbo.Cities");
            DropTable("dbo.HigherEducationInstitutions");
            DropTable("dbo.Faculties");
            DropTable("dbo.EducationLines");
            DropTable("dbo.EducationLineRequirements");
            DropTable("dbo.ExamDisciplines");
            DropTable("dbo.Weights");
            DropTable("dbo.Characteristics");
        }
    }
}
