using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OptimalEducation.Logic.Characterizers.Interfaces
{
    public interface ICharacterizer<T>
    {
        Task<Dictionary<string, double>> Calculate(T subject, bool isComplicatedMode = true);
    }
}
