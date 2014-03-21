namespace OptimalEducation.DAL.Models
{

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;


    public partial class School
    {

        public School()
        {
            ParticipationInSchools = new HashSet<ParticipationInSchool>();
            Weights = new HashSet<Weight>();
        }

        public int Id { get; set; }
        [Display(Name = "Ўкола")]
        public string Name { get; set; }
        [Display(AutoGenerateField=false)]
        public int EducationQuality { get; set; }

        public virtual ICollection<ParticipationInSchool> ParticipationInSchools { get; set; }

        public virtual City City { get; set; }

        public virtual ICollection<Weight> Weights { get; set; }

    }

}
