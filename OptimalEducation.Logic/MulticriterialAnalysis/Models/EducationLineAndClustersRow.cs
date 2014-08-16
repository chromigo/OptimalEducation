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
        public EducationLine EducationLine { get;private set; }
        public Dictionary<string, double> Characterisics { get; private set; }

        public EducationLineWithCharacterisics(EducationLine educationLine, Dictionary<string, double> characterisics)
        {
            EducationLine = educationLine;
            Characterisics = characterisics;
        }
    }
}
