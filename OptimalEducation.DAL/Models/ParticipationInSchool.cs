namespace OptimalEducation.DAL.Models
{

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public partial class ParticipationInSchool
    {

        public int Id { get; set; }
        public int EntrantsId { get; set; }
        [Display(Name = "Школа")]
        public int SchoolId { get; set; }
        [Display(Name = "Лет обучения")]
        [Range(0.5,30)]
        public double YearPeriod { get; set; }

        public virtual Entrant Entrants { get; set; }

        public virtual School School { get; set; }

    }

}
