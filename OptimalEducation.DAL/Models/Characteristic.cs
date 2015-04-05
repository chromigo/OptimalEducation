using System.Collections.Generic;

namespace OptimalEducation.DAL.Models
{
    /// <summary>
    ///     Характеристика для весов (данный вес вносит свой вклад в данную харатеристику)
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
    ///     Обозначаем к какому типу относится заданная характеристика: образовательная, спортивная, хобби
    /// </summary>
    public enum CharacteristicType
    {
        Education,
        Physical,
        Etc
    }
}