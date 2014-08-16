using OptimalEducation.DAL.Models;
using System;
using System.Collections.Generic;

namespace OptimalEducation.Logic.Characterizer
{
    public interface ICharacterizer<T>
    {
        Dictionary<string, double> Calculate(T subject, bool isComplicatedMode = true);
    }
}
