namespace OptimalEducation.DAL.Models
{

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public enum OlypmpiadResult
    {
        [Display(Name = "Первое место")]
        FirstPlace,
        [Display(Name = "Второе место")]
        SecondPlace,
        [Display(Name = "Третье место")]
        ThirdPlace
    }

    public partial class ParticipationInOlympiad
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
