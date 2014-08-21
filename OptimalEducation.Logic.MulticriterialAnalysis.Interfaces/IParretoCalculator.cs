using OptimalEducation.Logic.MulticriterialAnalysis.Interfaces.Models;
using System;
using System.Collections.Generic;

namespace OptimalEducation.Logic.MulticriterialAnalysis.Interfaces
{
    public interface IParretoCalculator
    {
        List<EducationLineWithCharacterisics> ParretoSetCreate(List<EducationLineWithCharacterisics> educationLineCharacteristics);
    }
}
