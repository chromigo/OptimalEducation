using System.ComponentModel.DataAnnotations;

namespace OptimalEducation.DAL.Models
{
    /// <summary>
    ///     ����������  ��� ��� ����������� �������� � ������������
    /// </summary>
    public class UnitedStateExam
    {
        public int Id { get; set; }

        [Range(0, 100)]
        [Display(Name = "���������")]
        public int? Result { get; set; }

        public int ExamDisciplineId { get; set; }
        public int EntrantId { get; set; }

        [Display(Name = "����������")]
        public virtual ExamDiscipline Discipline { get; set; }

        [Display(Name = "�����������")]
        public virtual Entrant Entrant { get; set; }
    }
}