using System.ComponentModel.DataAnnotations;

namespace OptimalEducation.DAL.Models
{
    public class SchoolMark
    {
        public int Id { get; set; }

        [Range(3, 5)]
        [Display(Name = "ќценка")]
        public short? Result { get; set; }

        public int? Respect { get; set; }
        public int EntrantId { get; set; }
        public int SchoolDisciplineId { get; set; }
        public virtual Entrant Entrant { get; set; }
        public virtual SchoolDiscipline SchoolDiscipline { get; set; }
    }
}