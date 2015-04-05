using System.ComponentModel.DataAnnotations;

namespace OptimalEducation.DAL.Models
{
    public enum OlypmpiadResult
    {
        [Display(Name = "������ �����")] FirstPlace = 100,
        [Display(Name = "������ �����")] SecondPlace = 70,
        [Display(Name = "������ �����")] ThirdPlace = 50
    }

    public class ParticipationInOlympiad
    {
        public int Id { get; set; }

        [Display(Name = "���������")]
        public OlypmpiadResult Result { get; set; }

        [Display(Name = "����������")]
        public int EntrantId { get; set; }

        [Display(Name = "���������")]
        public int OlympiadId { get; set; }

        public virtual Entrant Entrant { get; set; }
        public virtual Olympiad Olympiad { get; set; }
    }
}