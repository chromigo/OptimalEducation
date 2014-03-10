namespace OptimalEducation.Models
{

    using System;
    using System.Collections.Generic;

    public enum OlypmpiadResult
    {
        FirstPlace=100,
        SecondPlace=70,
        ThirdPlace=50
    }

    public partial class ParticipationInOlympiad
    {

        public int Id { get; set; }

        public OlypmpiadResult Result { get; set; }

        public int EntrantId { get; set; }

        public int OlympiadId { get; set; }



        public virtual Entrant Entrant { get; set; }

        public virtual Olympiad Olympiad { get; set; }

    }

}
