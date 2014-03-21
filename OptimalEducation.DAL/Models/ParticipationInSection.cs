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
        [Display(Name = "Общее время занятий")]
        public double YearPeriod { get; set; }



        public virtual Entrant Entrants { get; set; }

        public virtual Section Section { get; set; }

    }

}
