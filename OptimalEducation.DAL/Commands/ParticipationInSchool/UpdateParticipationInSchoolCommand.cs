using System.Threading.Tasks;
using Interfaces.CQRS;

namespace OptimalEducation.DAL.Commands.ParticipationInSchool
{
    public class UpdateParticipationInSchoolCommand : EfBaseCommand, ICommand<UpdateParticipationInSchoolContext>
    {
        public UpdateParticipationInSchoolCommand(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task ExecuteAsync(UpdateParticipationInSchoolContext commandContext)
        {
            var dbPartOlymp = await DbContext.ParticipationInSchools.FindAsync(commandContext.ParticipationInSchool.Id);
            dbPartOlymp.YearPeriod = commandContext.ParticipationInSchool.YearPeriod;
            await DbContext.SaveChangesAsync();
        }
    }

    public class UpdateParticipationInSchoolContext : ICommandContext
    {
        public Models.ParticipationInSchool ParticipationInSchool { get; set; }
    }
}