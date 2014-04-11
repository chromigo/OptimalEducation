using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Logic.MulticriterialAnalysis.Models
{
    public class PreferenceRelation
    {
        public PreferenceRelation(string name)
        {
            ImportantClusterName = name;
            Tetas = new Dictionary<string, double>();
        }
        public string ImportantClusterName { get; private set; }
        /// <summary>
        /// Словарь "Название неважного кластера, Тета - коэффциент"
        /// </summary>
        public Dictionary<string, double> Tetas { get; set; }
    }
}
