namespace OptimalEducation.DAL.Models
{

using System;
    using System.Collections.Generic;
    
public partial class SchoolType
{

    public SchoolType()
    {

        this.Schools = new HashSet<School>();

        this.Weights = new HashSet<Weight>();

    }


    public int Id { get; set; }

    public string Name { get; set; }



    public virtual ICollection<School> Schools { get; set; }

    public virtual ICollection<Weight> Weights { get; set; }

}

}
