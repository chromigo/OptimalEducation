using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OptimalEducation.Interfaces.Logic.Characterizers
{
    public interface IIdealResult<T>
    {
        Task<Dictionary<string, double>> GetComplicatedResult();
        Task<Dictionary<string, double>> GetSimpleResult();
    }
}
