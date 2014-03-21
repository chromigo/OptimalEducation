namespace OptimalEducation.DAL.Models
{

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public partial class School
    {

        public School()
        {

            this.Entrants = new HashSet<Entrant>();

        }


        public int Id { get; set; }
        [Display(Name = "Ўкола")]
        public string Name { get; set; }
        [Display(AutoGenerateField=false)]
        public int EducationQuality { get; set; }
        [Display(Name = "“ип школы")]
        public int SchoolTypeId { get; set; }



        public virtual ICollection<Entrant> Entrants { get; set; }

        public virtual SchoolType SchoolType { get; set; }

        public virtual City City { get; set; }

    }

}
