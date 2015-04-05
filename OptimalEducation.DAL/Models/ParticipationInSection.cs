using System.ComponentModel.DataAnnotations;

namespace OptimalEducation.DAL.Models
{
    public class ParticipationInSection
    {
        public int Id { get; set; }
        public int EntrantsId { get; set; }

        [Display(Name = "������")]
        public int SectionId { get; set; }

        [Display(Name = "���")]
        [Range(0.5, 30)]
        public double YearPeriod { get; set; }

        public virtual Entrant Entrants { get; set; }
        public virtual Section Section { get; set; }
    }
}