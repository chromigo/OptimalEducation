using System.Collections.Generic;
using System.Threading.Tasks;
using OptimalEducation.DAL.Models;

namespace OptimalEducation.Interfaces.Logic.MulticriterialAnalysis
{
    public interface IMulticriterialAnalysisRecomendator
    {
        Task<List<EducationLine>> Calculate(Entrant entrant, IEnumerable<EducationLine> educationLines);
    }
}
