using OptimalEducation.Logic.MulticriterialAnalysis.Interfaces.Models;
using System;
using System.Collections.Generic;

namespace OptimalEducation.Logic.MulticriterialAnalysis.Interfaces
{
    public interface IPreferenceRelationCalculator
    {
        List<PreferenceRelation> GetPreferenceRelations(Dictionary<string, double> userCharacteristics);
    }
}
