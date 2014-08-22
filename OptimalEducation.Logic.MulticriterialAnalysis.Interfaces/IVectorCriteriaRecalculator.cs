using OptimalEducation.Interfaces.Logic.MulticriterialAnalysis.Models;
using System;
using System.Collections.Generic;

namespace OptimalEducation.Interfaces.Logic.MulticriterialAnalysis
{
    public interface IVectorCriteriaRecalculator
    {
        List<EducationLineWithCharacterisics> RecalculateEducationLineCharacterisics(List<EducationLineWithCharacterisics> educationLineWithCharacterisics, List<PreferenceRelation> userPrefer);
    }
}
