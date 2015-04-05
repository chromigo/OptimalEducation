using System.Collections.Generic;
using System.Data.Entity.Spatial;

namespace OptimalEducation.DAL.Models
{
    public class City
    {
        public City()
        {
            HigherEducationInstitutions = new HashSet<HigherEducationInstitution>();

            Schools = new HashSet<School>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Prestige { get; set; }
        public DbGeography Location { get; set; }
        public virtual ICollection<HigherEducationInstitution> HigherEducationInstitutions { get; set; }
        public virtual ICollection<School> Schools { get; set; }
    }
}