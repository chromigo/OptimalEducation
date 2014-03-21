namespace OptimalEducation.DAL.Models
{
	using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public partial class Olympiad
    {

        public Olympiad()
        {

            this.ParticipationInOlympiads = new HashSet<ParticipationInOlympiad>();

            this.Weights = new HashSet<Weight>();

        }


        public int Id { get; set; }
        [Display(Name = "Название олимпиады")]
        public string Name { get; set; }

        public virtual ICollection<ParticipationInOlympiad> ParticipationInOlympiads { get; set; }

        public virtual ICollection<Weight> Weights { get; set; }

    }

}
