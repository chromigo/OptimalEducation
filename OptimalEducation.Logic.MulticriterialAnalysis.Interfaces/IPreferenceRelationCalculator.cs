using System.Collections.Generic;
using OptimalEducation.Interfaces.Logic.MulticriterialAnalysis.Models;

namespace OptimalEducation.Interfaces.Logic.MulticriterialAnalysis
{
    public interface IPreferenceRelationCalculator
    {
        List<PreferenceRelation> GetPreferenceRelations(Dictionary<string, double> userCharacteristics);
    }
}
