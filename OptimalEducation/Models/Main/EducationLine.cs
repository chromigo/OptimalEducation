namespace OptimalEducation.Models
{

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    /// <summary>
    /// Направление обучения на факультете
    /// </summary>
    public partial class EducationLine
    {

        public EducationLine()
        {

            this.EducationLinesRequirements = new HashSet<EducationLineRequirement>();

        }


        public int Id { get; set; }
        public Nullable<int> GeneralEducationLineId { get; set; }
        public int FacultyId { get; set; }
        /// <summary>
        /// Код направления (напр. 0105001)
        /// </summary>
        [Display(Name = "Код направления")]
        public string Code { get; set; }
        /// <summary>
        /// Форма обучения (дневная, вечерная, заочная)
        /// </summary>
        [Display(Name = "Форма обучения")]
        public string EducationForm { get; set; }
        /// <summary>
        /// Название направления
        /// </summary>
        [Display(Name = "Название направления")]
        public string Name { get; set; }
        /// <summary>
        /// Минимальная сумма по баллам егэ
        /// </summary>
        [Display(Name = "Мин. сумма ЕГЭ")]
        public Nullable<int> RequiredSum { get; set; }
        /// <summary>
        /// Актуально ли направление(или устарело и больше не используется)
        /// </summary>
        [Display(Name = "Актуальность")]
        public bool Actual { get; set; }
        /// <summary>
        /// Стоимость обучения
        /// </summary>
        [Display(Name = "Стоимость обучения")]
        public bool Price { get; set; }
        /// <summary>
        /// Количество платных мест
        /// </summary>
        [Display(Name = "Количество платных мест")]
        public int PaidPlacesNumber { get; set; }
        /// <summary>
        /// Количество бюджетных мест
        /// </summary>
        [Display(Name = "Количество бюджетных мест")]
        public int FreePlacesNumber { get; set; }

        public virtual GeneralEducationLine GeneralEducationLine { get; set; }

        public virtual Faculty Faculty { get; set; }

        public virtual ICollection<EducationLineRequirement> EducationLinesRequirements { get; set; }

    }

}
