using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OptimalEducation.DAL.Models
{
    public class ExamDiscipline
    {
        public ExamDiscipline()
        {
            UnitedStateExams = new HashSet<UnitedStateExam>();

            EducationLineRequirements = new HashSet<EducationLineRequirement>();

            Weights = new HashSet<Weight>();
        }

        public int Id { get; set; }

        [Display(Name = "Дисциплина")]
        public string Name { get; set; }

        public virtual ICollection<UnitedStateExam> UnitedStateExams { get; set; }

        /// <summary>
        ///     Тип экзамена
        /// </summary>
        public ExamType ExamType { get; set; }

        public virtual ICollection<EducationLineRequirement> EducationLineRequirements { get; set; }
        public virtual ICollection<Weight> Weights { get; set; }
    }

    /// <summary>
    ///     Типы экзаменов
    /// </summary>
    public enum ExamType
    {
        UnitedStateExam,
        CustomWrittenExam,
        CustomOralExam,
        CustomExamOther
    }
}