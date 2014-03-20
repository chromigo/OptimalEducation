namespace OptimalEducation.DAL.Models
{

using System;
    using System.Collections.Generic;
    
public partial class Weight
{

    public int Id { get; set; }

    public double Coefficient { get; set; }

    public int ClusterId { get; set; }

    public Nullable<int> SchoolTypeId { get; set; }

    public Nullable<int> SchoolDisciplineId { get; set; }

    public Nullable<int> SectionId { get; set; }

    public Nullable<int> HobbieId { get; set; }

    public Nullable<int> OlympiadId { get; set; }

    public Nullable<int> ExamDisciplineId { get; set; }



    public virtual Cluster Cluster { get; set; }

    public virtual SchoolType SchoolType { get; set; }

    public virtual SchoolDiscipline SchoolDiscipline { get; set; }

    public virtual Section Section { get; set; }

    public virtual Hobbie Hobbie { get; set; }

    public virtual Olympiad Olympiad { get; set; }

    public virtual ExamDiscipline ExamDiscipline { get; set; }

}

}
