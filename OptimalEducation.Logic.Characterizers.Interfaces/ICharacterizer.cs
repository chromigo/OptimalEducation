using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OptimalEducation.Interfaces.Logic.Characterizers
{
    public interface ICharacterizer<T>
    {
        Task<Dictionary<string, double>> Calculate(T subject, bool isComplicatedMode = true);
    }
}
