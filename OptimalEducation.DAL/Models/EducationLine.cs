using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OptimalEducation.DAL.Models
{
    /// <summary>
    ///     ����������� �������� �� ����������
    /// </summary>
    public class EducationLine
    {
        public EducationLine()
        {
            EducationLinesRequirements = new HashSet<EducationLineRequirement>();
        }

        public int Id { get; set; }
        public int? GeneralEducationLineId { get; set; }
        public int FacultyId { get; set; }

        /// <summary>
        ///     ��� ����������� (����. 0105001)
        /// </summary>
        [Display(Name = "��� �����������")]
        public string Code { get; set; }

        /// <summary>
        ///     ����� �������� (�������, ��������, �������)
        /// </summary>
        [Display(Name = "����� ��������")]
        public EducationFormType? EducationForm { get; set; }

        /// <summary>
        ///     �������� �����������
        /// </summary>
        [Display(Name = "�������� �����������")]
        public string Name { get; set; }

        /// <summary>
        ///     ����������� ����� �� ������ ���
        /// </summary>
        [Display(Name = "���. ����� ���")]
        public int? RequiredSum { get; set; }

        /// <summary>
        ///     ��������� �� �����������(��� �������� � ������ �� ������������)
        /// </summary>
        [Display(Name = "������������")]
        public bool Actual { get; set; }

        /// <summary>
        ///     ��������� ��������
        /// </summary>
        [Display(Name = "��������� ��������"), DataType(DataType.Currency)]
        public int Price { get; set; }

        /// <summary>
        ///     ���������� ������� ����
        /// </summary>
        [Display(Name = "���������� ������� ����")]
        public int PaidPlacesNumber { get; set; }

        /// <summary>
        ///     ���������� ��������� ����
        /// </summary>
        [Display(Name = "���������� ��������� ����")]
        public int FreePlacesNumber { get; set; }

        public virtual GeneralEducationLine GeneralEducationLine { get; set; }
        public virtual Faculty Faculty { get; set; }
        public virtual ICollection<EducationLineRequirement> EducationLinesRequirements { get; set; }
    }


    /// <summary>
    ///     ���� �������� �� �����������
    /// </summary>
    public enum EducationFormType
    {
        InternalBachelor,
        InternalSpecialist,
        ExtramuralBachelor,
        ExtramuralSpecialist,
        Etc
    }
}