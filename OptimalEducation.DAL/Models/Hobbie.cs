using System.Collections.Generic;

namespace OptimalEducation.DAL.Models
{
    public class Hobbie
    {
        public Hobbie()
        {
            Entrants = new HashSet<Entrant>();

            Weights = new HashSet<Weight>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Entrant> Entrants { get; set; }
        public virtual ICollection<Weight> Weights { get; set; }
    }
}