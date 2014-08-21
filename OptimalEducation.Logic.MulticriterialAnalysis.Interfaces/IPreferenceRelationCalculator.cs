using OptimalEducation.Interfaces.Logic.MulticriterialAnalysis.Models;
using System;
using System.Collections.Generic;

namespace OptimalEducation.Interfaces.Logic.MulticriterialAnalysis
{
    public interface IPreferenceRelationCalculator
    {
        List<PreferenceRelation> GetPreferenceRelations(Dictionary<string, double> userCharacteristics);
    }
}
