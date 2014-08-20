using OptimalEducation.Logic.MulticriterialAnalysis.Models;
using System;
using System.Collections.Generic;

namespace OptimalEducation.Logic.MulticriterialAnalysis
{
    public interface IPreferenceRelationCalculator
    {
        List<PreferenceRelation> GetPreferenceRelations(Dictionary<string, double> userCharacteristics);
    }
}
