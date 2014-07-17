using OptimalEducation.DAL.Models;

namespace OptimalEducation.DAL.Queries
{
    public abstract class EFBaseQuery
    {
        protected readonly IOptimalEducationDbContext _dbContext;

        public EFBaseQuery(IOptimalEducationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
