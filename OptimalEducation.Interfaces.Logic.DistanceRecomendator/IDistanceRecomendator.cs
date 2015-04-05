using System.Collections.Generic;
using System.Threading.Tasks;

namespace OptimalEducation.Interfaces.Logic.DistanceRecomendator
{
    public interface IDistanceRecomendator<in TSubject, TObjects>
    {
        Task<Dictionary<TObjects, double>> GetRecomendation(TSubject subject, IEnumerable<TObjects> objects);
    }
}
