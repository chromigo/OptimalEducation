using System.Collections.Generic;

namespace OptimalEducation.DAL.Models
{
    public class GeneralEducationLine
    {
        public GeneralEducationLine()
        {
            EducationLines = new HashSet<EducationLine>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public virtual ICollection<EducationLine> EducationLines { get; set; }
    }
}