namespace OptimalEducation.DAL.Commands
{
    public abstract class EfBaseCommand
    {
        protected readonly IOptimalEducationDbContext DbContext;

        public EfBaseCommand(IOptimalEducationDbContext dbContext)
        {
            DbContext = dbContext;
        }
    }
}
