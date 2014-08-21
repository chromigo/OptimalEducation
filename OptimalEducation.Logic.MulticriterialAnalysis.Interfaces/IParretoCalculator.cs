using OptimalEducation.Interfaces.Logic.MulticriterialAnalysis.Models;
using System;
using System.Collections.Generic;

namespace OptimalEducation.Interfaces.Logic.MulticriterialAnalysis
{
    public interface IParretoCalculator
    {
        List<EducationLineWithCharacterisics> ParretoSetCreate(List<EducationLineWithCharacterisics> educationLineCharacteristics);
    }
}
