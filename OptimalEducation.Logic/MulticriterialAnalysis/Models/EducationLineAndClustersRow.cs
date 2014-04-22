using OptimalEducation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Logic.MulticriterialAnalysis.Models
{
    public class EducationLineWithCharacterisics
    {
        public EducationLine EducationLine { get; set; }
        public Dictionary<string, double> Characterisics { get; set; }

        public EducationLineWithCharacterisics(EducationLine educationLine)
        {
            EducationLine = educationLine;
            Characterisics = new Dictionary<string, double>();
        }
    }
}
