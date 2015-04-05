using System.Collections.Generic;

namespace OptimalEducation.Interfaces.Logic.Characterizers
{
    public interface ISummator<in T>
    {
        Dictionary<string, double> CalculateComplicatedSum(T entrant);
        Dictionary<string, double> CalculateSimpleSum(T entrant);
    }
}
