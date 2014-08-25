using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Helpers
{
    public interface IInfoExtractor
    {
        Task<int> ExtractEntrantId(string userId);
    }
}
