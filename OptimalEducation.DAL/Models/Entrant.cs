using System.Collections.Generic;

namespace OptimalEducation.DAL.Models
{
    public class Entrant
    {
        public Entrant()
        {
            ParticipationInSections = new HashSet<ParticipationInSection>();

            Hobbies = new HashSet<Hobbie>();

            ParticipationInSchools = new HashSet<ParticipationInSchool>();

            ParticipationInOlympiads = new HashSet<ParticipationInOlympiad>();

            UnitedStateExams = new HashSet<UnitedStateExam>();

            SchoolMarks = new HashSet<SchoolMark>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string SchoolEducation { get; set; }
        public string Medal { get; set; }
        public string Citizenship { get; set; }
        public virtual City City { get; set; }
        public double? AverageMark { get; set; }
        public virtual ICollection<ParticipationInSection> ParticipationInSections { get; set; }
        public virtual ICollection<Hobbie> Hobbies { get; set; }
        public virtual ICollection<ParticipationInSchool> ParticipationInSchools { get; set; }
        public virtual ICollection<ParticipationInOlympiad> ParticipationInOlympiads { get; set; }
        public virtual ICollection<UnitedStateExam> UnitedStateExams { get; set; }
        public virtual ICollection<SchoolMark> SchoolMarks { get; set; }
    }
}