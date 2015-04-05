using System.Collections.Generic;

namespace OptimalEducation.DAL.Models
{
    public class SchoolDiscipline
    {
        public SchoolDiscipline()
        {
            SchoolMarks = new HashSet<SchoolMark>();

            Weights = new HashSet<Weight>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<SchoolMark> SchoolMarks { get; set; }
        public virtual ICollection<Weight> Weights { get; set; }
    }
}