using OptimalEducation.Logic.MulticriterialAnalysis.Models;
using System;
using System.Collections.Generic;

namespace OptimalEducation.Logic.MulticriterialAnalysis
{
    public interface IParretoCalculator
    {
        List<EducationLineWithCharacterisics> ParretoSetCreate(List<EducationLineWithCharacterisics> educationLineCharacteristics);
    }
}
