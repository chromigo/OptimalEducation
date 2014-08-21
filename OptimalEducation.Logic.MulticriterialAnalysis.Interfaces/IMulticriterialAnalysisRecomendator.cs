using OptimalEducation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Logic.MulticriterialAnalysis.Interfaces
{
    public interface IMulticriterialAnalysisRecomendator
    {
        Task<List<EducationLine>> Calculate(Entrant entrant, IEnumerable<EducationLine> educationLines);
    }
}
