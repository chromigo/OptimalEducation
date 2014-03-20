namespace OptimalEducation.DAL.Models
{

using System;
    using System.Collections.Generic;
    
public partial class Preference
{

    public Preference()
    {

        this.Cities = new HashSet<City>();

    }


    public int Id { get; set; }

    public int Presige { get; set; }

    public string EducationFrom { get; set; }

    public int EntrantId { get; set; }



    public virtual Entrant Entrant { get; set; }

    public virtual ICollection<City> Cities { get; set; }

}

}
