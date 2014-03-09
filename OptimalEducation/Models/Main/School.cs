namespace OptimalEducation.Models
{

using System;
    using System.Collections.Generic;
    
public partial class School
{

    public School()
    {

        this.Entrants = new HashSet<Entrant>();

    }


    public int Id { get; set; }

    public string Name { get; set; }

    public int EducationQuality { get; set; }

    public int SchoolTypeId { get; set; }



    public virtual ICollection<Entrant> Entrants { get; set; }

    public virtual SchoolType SchoolType { get; set; }

    public virtual City City { get; set; }

}

}
