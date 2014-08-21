using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Logic.Characterizers.Interfaces
{
    public interface IDistanceRecomendator<TSubject, TObjects>
    {
        Task<Dictionary<TObjects, double>> GetRecomendation(TSubject subject, IEnumerable<TObjects> objects);
    }
}
