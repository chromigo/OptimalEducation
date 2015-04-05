using System.Collections.Generic;

namespace OptimalEducation.Interfaces.Logic.MulticriterialAnalysis.Models
{
    public class PreferenceRelation
    {
        public PreferenceRelation(string name)
        {
            ImportantCharacterisicName = name;
            Tetas = new Dictionary<string, double>();
        }
        public string ImportantCharacterisicName { get; private set; }
        /// <summary>
        /// Словарь "Название неважного кластера, Тета - коэффциент"
        /// </summary>
        public Dictionary<string, double> Tetas { get; set; }
    }
}
