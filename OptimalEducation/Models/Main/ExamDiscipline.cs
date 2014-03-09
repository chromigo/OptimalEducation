namespace OptimalEducation.Models
{

    using System;
    using System.Collections.Generic;
    
    public partial class ExamDiscipline
    {

        public ExamDiscipline()
        {

            this.UnitedStateExams = new HashSet<UnitedStateExam>();

            this.EducationLineRequirements = new HashSet<EducationLineRequirement>();

            this.Weights = new HashSet<Weight>();

        }


        public int Id { get; set; }

        public string Name { get; set; }



        public virtual ICollection<UnitedStateExam> UnitedStateExams { get; set; }

        public virtual ICollection<EducationLineRequirement> EducationLineRequirements { get; set; }

        public virtual ICollection<Weight> Weights { get; set; }

    }

}
