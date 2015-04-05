using System.Collections.Generic;

namespace OptimalEducation.DAL.Models
{
    /// <summary>
    ///     �������������� ��� ����� (������ ��� ������ ���� ����� � ������ �������������)
    /// </summary>
    public class Characteristic
    {
        public Characteristic()
        {
            Weights = new HashSet<Weight>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public CharacteristicType Type { get; set; }
        public virtual ICollection<Weight> Weights { get; set; }
    }

    /// <summary>
    ///     ���������� � ������ ���� ��������� �������� ��������������: ���������������, ����������, �����
    /// </summary>
    public enum CharacteristicType
    {
        Education,
        Physical,
        Etc
    }
}