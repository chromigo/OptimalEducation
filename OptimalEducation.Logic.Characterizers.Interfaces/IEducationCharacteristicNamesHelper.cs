using System;
using System.Collections.Generic;

namespace OptimalEducation.Interfaces.Logic.Characterizers
{
    public interface IEducationCharacteristicNamesHelper
    {
        IEnumerable<string> Names { get; }
    }
}
