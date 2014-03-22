namespace OptimalEducation.DAL.Models
{

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public partial class ParticipationInSection
    {

        public int Id { get; set; }
        public int EntrantsId { get; set; }
        [Display(Name = "Секция")]
        public int SectionId { get; set; }
        [Display(Name = "Лет")]
        [Range(0.5,30)]
        public double YearPeriod { get; set; }


        public virtual Entrant Entrants { get; set; }

        public virtual Section Section { get; set; }

    }

}
