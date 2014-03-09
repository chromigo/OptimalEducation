using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace OptimalEducation.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        //Указатели на таблицу пользоваетля или на таблицу кафедры (в зависимости от типа аккаунта/роли)
        public int? EntrantId { get; set; }
        public int? FacultyId { get; set; }

        public virtual Entrant Entrant { get; set; }
        public virtual Faculty Faculty { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public virtual DbSet<Section> Sections { get; set; }

        public virtual DbSet<Entrant> Entrants { get; set; }

        public virtual DbSet<SchoolType> SchoolTypes { get; set; }

        public virtual DbSet<School> Schools { get; set; }

        public virtual DbSet<Preference> Preferences { get; set; }

        public virtual DbSet<Hobbie> Hobbies { get; set; }

        public virtual DbSet<ParticipationInOlympiad> ParticipationInOlympiads { get; set; }

        public virtual DbSet<Olympiad> Olympiads { get; set; }

        public virtual DbSet<UnitedStateExam> UnitedStateExams { get; set; }

        public virtual DbSet<SchoolMark> SchoolMarks { get; set; }

        public virtual DbSet<ExamDiscipline> ExamDisciplines { get; set; }

        public virtual DbSet<Cluster> Clusters { get; set; }

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