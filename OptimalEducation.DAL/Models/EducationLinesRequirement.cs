using System.ComponentModel.DataAnnotations;

namespace OptimalEducation.DAL.Models
{
    public class EducationLineRequirement
    {
        public int Id { get; set; }

        [Display(Name = "¡‡ÎÎ˚ ≈√›")]
        public int Requirement { get; set; }

        public int EducationLineId { get; set; }
        public int ExamDisciplineId { get; set; }
        public virtual EducationLine EducationLine { get; set; }
        public virtual ExamDiscipline ExamDiscipline { get; set; }
    }
}