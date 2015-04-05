using System.ComponentModel.DataAnnotations;

namespace OptimalEducation.DAL.Models
{
    public class ParticipationInSchool
    {
        public int Id { get; set; }
        public int EntrantsId { get; set; }

        [Display(Name = "�����")]
        public int SchoolId { get; set; }

        [Display(Name = "��� ��������")]
        [Range(0.5, 30)]
        public double YearPeriod { get; set; }

        public virtual Entrant Entrants { get; set; }
        public virtual School School { get; set; }
    }
}