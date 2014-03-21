namespace OptimalEducation.DAL.Models
{

using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
public partial class Section
{

    public Section()
    {

        this.ParticipationInSections = new HashSet<ParticipationInSection>();

        this.Weights = new HashSet<Weight>();

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
