using System.Threading.Tasks;
using Interfaces.CQRS;

namespace OptimalEducation.DAL.Commands.ParticipationInSection
{
    public class UpdateParticipationInSectionCommand : EfBaseCommand,
        ICommand<UpdateParticipationInSectionResultContext>
    {
        public UpdateParticipationInSectionCommand(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task ExecuteAsync(UpdateParticipationInSectionResultContext commandContext)
        {
            var dbPartOlymp =
                await DbContext.ParticipationInSections.FindAsync(commandContext.ParticipationInSection.Id);
            dbPartOlymp.YearPeriod = commandContext.ParticipationInSection.YearPeriod;
            await DbContext.SaveChangesAsync();
        }
    }

    public class UpdateParticipationInSectionResultContext : ICommandContext
    {
        public Models.ParticipationInSection ParticipationInSection { get; set; }
    }
}