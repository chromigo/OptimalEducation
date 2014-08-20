using OptimalEducation.Logic.MulticriterialAnalysis.Models;
using System;
using System.Collections.Generic;
namespace OptimalEducation.Logic.MulticriterialAnalysis
{
    public interface IVectorCriteriaRecalculator
    {
        List<EducationLineWithCharacterisics> RecalculateEducationLineCharacterisics(List<EducationLineWithCharacterisics> educationLineWithCharacterisics, List<PreferenceRelation> userPrefer);
    }
}
