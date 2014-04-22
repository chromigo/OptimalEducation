namespace OptimalEducation.DAL.Models
{

using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
public partial class SchoolMark
{

    public int Id { get; set; }
    [Range(3, 5)]
    [Display(Name = "ќценка")]
    public short? Result { get; set; }

    public Nullable<int> Respect { get; set; }

    public int EntrantId { get; set; }

    public int SchoolDisciplineId { get; set; }



    public virtual Entrant Entrant { get; set; }

    public virtual SchoolDiscipline SchoolDiscipline { get; set; }

}

}
