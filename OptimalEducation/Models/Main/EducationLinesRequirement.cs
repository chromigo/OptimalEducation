namespace OptimalEducation.Models
{

    using System;
    using System.Collections.Generic;
    
    public partial class EducationLineRequirement
    {

        public int Id { get; set; }
    
        public int Requirement { get; set; }

        public int EducationLineId { get; set; }

        public int ExamDisciplineId { get; set; }



        public virtual EducationLine EducationLine { get; set; }

        public virtual ExamDiscipline ExamDiscipline { get; set; }

    }

}
