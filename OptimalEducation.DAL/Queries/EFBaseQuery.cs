namespace OptimalEducation.DAL.Queries
{
    public abstract class EfBaseQuery
    {
        protected readonly IOptimalEducationDbContext DbContext;

        protected EfBaseQuery(IOptimalEducationDbContext dbContext)
        {
            DbContext = dbContext;
        }
    }
}
