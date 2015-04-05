using System.Threading.Tasks;

namespace OptimalEducation.Helpers
{
    public interface IInfoExtractor
    {
        Task<int> ExtractEntrantId(string userId);
    }
}
