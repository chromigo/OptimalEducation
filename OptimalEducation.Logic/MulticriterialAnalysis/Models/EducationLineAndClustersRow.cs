using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Logic.MulticriterialAnalysis.Models
{
    public class EducationLineAndCharacterisicsRow
    {
        public int Id { get; set; }
        public string Code { get; set; }

        public Dictionary<string, double> Characterisics { get; set; }

        public EducationLineAndCharacterisicsRow(int id)
        {
            Id = id;
            Characterisics = new Dictionary<string, double>();
        }
    }
}
