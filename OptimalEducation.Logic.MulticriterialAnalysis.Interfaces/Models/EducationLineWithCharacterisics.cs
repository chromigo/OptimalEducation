using System.Collections.Generic;
using OptimalEducation.DAL.Models;

namespace OptimalEducation.Interfaces.Logic.MulticriterialAnalysis.Models
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
