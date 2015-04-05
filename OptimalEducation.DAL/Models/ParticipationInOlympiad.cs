using System.ComponentModel.DataAnnotations;

namespace OptimalEducation.DAL.Models
{
    public enum OlypmpiadResult
    {
        [Display(Name = "Первое место")] FirstPlace = 100,
        [Display(Name = "Второе место")] SecondPlace = 70,
        [Display(Name = "Третье место")] ThirdPlace = 50
    }

    public class ParticipationInOlympiad
    {
        public int Id { get; set; }

        [Display(Name = "Результат")]
        public OlypmpiadResult Result { get; set; }

        [Display(Name = "Абитуриент")]
        public int EntrantId { get; set; }

        [Display(Name = "Олимпиада")]
        public int OlympiadId { get; set; }

        public virtual Entrant Entrant { get; set; }
        public virtual Olympiad Olympiad { get; set; }
    }
}