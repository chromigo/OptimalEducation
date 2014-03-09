namespace OptimalEducation.Models
{

using System;
    using System.Collections.Generic;
    
public partial class Hobbie
{

    public Hobbie()
    {

        this.Entrants = new HashSet<Entrant>();

        this.Weights = new HashSet<Weight>();

    }


    public int Id { get; set; }

    public string Name { get; set; }



    public virtual ICollection<Entrant> Entrants { get; set; }

    public virtual ICollection<Weight> Weights { get; set; }

}

}
