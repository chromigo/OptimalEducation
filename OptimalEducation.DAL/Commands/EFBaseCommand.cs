using OptimalEducation.DAL.Models;

namespace OptimalEducation.DAL.Queries
{
    public abstract class EFBaseCommand
    {
        protected readonly IOptimalEducationDbContext _dbContext;

        public EFBaseCommand(IOptimalEducationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
