using System.Data.Entity;

namespace OptimalEducation.DAL.Models
{
    public class OptimalEducationDbContext : DbContext
    {
        public OptimalEducationDbContext()
            : base("OptimalEducationDataBase")
        {
        }

        public virtual DbSet<Section> Sections { get; set; }

        public virtual DbSet<Entrant> Entrants { get; set; }

        public virtual DbSet<School> Schools { get; set; }

        public virtual DbSet<Hobbie> Hobbies { get; set; }

        public virtual DbSet<ParticipationInOlympiad> ParticipationInOlympiads { get; set; }
        public virtual DbSet<ParticipationInSection> ParticipationInSections { get; set; }
        public virtual DbSet<ParticipationInSchool> ParticipationInSchools { get; set; }
        public virtual DbSet<Olympiad> Olympiads { get; set; }

        public virtual DbSet<UnitedStateExam> UnitedStateExams { get; set; }

        public virtual DbSet<SchoolMark> SchoolMarks { get; set; }

        public virtual DbSet<ExamDiscipline> ExamDisciplines { get; set; }

        public virtual DbSet<Characteristic> Characteristics { get; set; }

        public virtual DbSet<HigherEducationInstitution> HigherEducationInstitutions { get; set; }

        public virtual DbSet<City> Cities { get; set; }

        public virtual DbSet<Faculty> Faculties { get; set; }

        public virtual DbSet<EducationLine> EducationLines { get; set; }

        public virtual DbSet<GeneralEducationLine> GeneralEducationLines { get; set; }

        public virtual DbSet<EducationLineRequirement> EducationLineRequirements { get; set; }

        public virtual DbSet<SchoolDiscipline> SchoolDisciplines { get; set; }

        public virtual DbSet<Weight> Weights { get; set; }

    }
}