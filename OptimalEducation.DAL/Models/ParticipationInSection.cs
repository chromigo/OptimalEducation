namespace OptimalEducation.DAL.Models
{

    using System;
    using System.Collections.Generic;
    
    public partial class ParticipationInSection
    {

        public int Id { get; set; }
        public int EntrantsId { get; set; }
        public int SectionId { get; set; }

        public double YearPeriod { get; set; }



        public virtual Entrant Entrants { get; set; }

        public virtual Section Section { get; set; }

    }

}
