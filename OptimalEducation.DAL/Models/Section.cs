using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OptimalEducation.DAL.Models
{
    public class Section
    {
        public Section()
        {
            ParticipationInSections = new HashSet<ParticipationInSection>();

            Weights = new HashSet<Weight>();
        }

        public int Id { get; set; }

        [Display(Name = "Секция")]
        public string Name { get; set; }

        [Display(Name = "Тип активности")]
        public string ActivityType { get; set; }

        public virtual ICollection<ParticipationInSection> ParticipationInSections { get; set; }
        public virtual ICollection<Weight> Weights { get; set; }
    }
}