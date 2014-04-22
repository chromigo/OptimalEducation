namespace OptimalEducation.DAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    /// <summary>
    /// Результаты  ЕГЭ для конкретного предмета и пользователя
    /// </summary>
    public partial class UnitedStateExam
    {

        public int Id { get; set; }
        [Range(0,100)]
        [Display(Name = "Результат")]
        public int? Result { get; set; }

        public int ExamDisciplineId { get; set; }
        
        public int EntrantId { get; set; }


        [Display(Name = "Дисциплина")]
        public virtual ExamDiscipline Discipline { get; set; }
        [Display(Name = "Поступающий")]
        public virtual Entrant Entrant { get; set; }

    }

}
