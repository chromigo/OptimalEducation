using OptimalEducation.Logic.MulticriterialAnalysis.Interfaces.Models;
using System;
using System.Collections.Generic;

namespace OptimalEducation.Logic.MulticriterialAnalysis.Interfaces
{
    public interface IVectorCriteriaRecalculator
    {
        List<EducationLineWithCharacterisics> RecalculateEducationLineCharacterisics(List<EducationLineWithCharacterisics> educationLineWithCharacterisics, List<PreferenceRelation> userPrefer);
    }
}
