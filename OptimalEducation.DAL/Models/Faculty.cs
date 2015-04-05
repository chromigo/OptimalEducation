using System.Collections.Generic;

namespace OptimalEducation.DAL.Models
{
    public class Faculty
    {
        public Faculty()
        {
            FacultyEducationLines = new HashSet<EducationLine>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Prestige { get; set; }
        public int HigherEducationInstitutionId { get; set; }
        public virtual HigherEducationInstitution HigherEducationInstitution { get; set; }
        public virtual ICollection<EducationLine> FacultyEducationLines { get; set; }
    }
}