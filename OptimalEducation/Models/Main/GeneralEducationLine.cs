namespace OptimalEducation.Models
{

using System;
    using System.Collections.Generic;
    
public partial class GeneralEducationLine
{

    public GeneralEducationLine()
    {

        this.EducationLines = new HashSet<EducationLine>();

    }


    public int Id { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }



    public virtual ICollection<EducationLine> EducationLines { get; set; }

}

}
