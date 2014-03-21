namespace OptimalEducation.DAL.Models
{

using System;
    using System.Collections.Generic;
    
public partial class SchoolDiscipline
{

    public SchoolDiscipline()
    {

        this.SchoolMarks = new HashSet<SchoolMark>();

        this.Weights = new HashSet<Weight>();

    }


    public int Id { get; set; }

    public string Name { get; set; }



    public virtual ICollection<SchoolMark> SchoolMarks { get; set; }

    public virtual ICollection<Weight> Weights { get; set; }

}

}
