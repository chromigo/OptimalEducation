using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OptimalEducation.DAL.Models
{
    public class Olympiad
    {
        public Olympiad()
        {
            ParticipationInOlympiads = new HashSet<ParticipationInOlympiad>();

            Weights = new HashSet<Weight>();
        }

        public int Id { get; set; }

        [Display(Name = "Название олимпиады")]
        public string Name { get; set; }

        public virtual ICollection<ParticipationInOlympiad> ParticipationInOlympiads { get; set; }
        public virtual ICollection<Weight> Weights { get; set; }
    }
}