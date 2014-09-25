using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace OptimalEducation.DAL.Models
{
    public interface IOptimalEducationDbContext
    {
        DbSet<Section> Sections { get; set; }
        DbSet<Entrant> Entrants { get; set; }
        DbSet<School> Schools { get; set; }
        DbSet<Hobbie> Hobbies { get; set; }
        DbSet<ParticipationInOlympiad> ParticipationInOlympiads { get; set; }
        DbSet<ParticipationInSection> ParticipationInSections { get; set; }
        DbSet<ParticipationInSchool> ParticipationInSchools { get; set; }
        DbSet<Olympiad> Olympiads { get; set; }
        DbSet<UnitedStateExam> UnitedStateExams { get; set; }
        DbSet<SchoolMark> SchoolMarks { get; set; }
        DbSet<ExamDiscipline> ExamDisciplines { get; set; }
        DbSet<Characteristic> Characteristics { get; set; }
        DbSet<HigherEducationInstitution> HigherEducationInstitutions { get; set; }
        DbSet<City> Cities { get; set; }
        DbSet<Faculty> Faculties { get; set; }
        DbSet<EducationLine> EducationLines { get; set; }
        DbSet<GeneralEducationLine> GeneralEducationLines { get; set; }
        DbSet<EducationLineRequirement> EducationLineRequirements { get; set; }
        DbSet<SchoolDiscipline> SchoolDisciplines { get; set; }
        DbSet<Weight> Weights { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        DbEntityEntry Entry(object entity);
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        DbSet Set(Type entityType);
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }

    public class OptimalEducationDbContext : DbContext, IOptimalEducationDbContext
    {
        public OptimalEducationDbContext()
            : base("OptimalEducationDB")
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