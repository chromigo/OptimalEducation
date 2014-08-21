using System.Threading.Tasks;
using Interfaces.CQRS;
using OptimalEducation.DAL.Models;
using OptimalEducation.DAL.Queries;

namespace OptimalEducation.DAL.Commands
{
    public class UpdateParticipationInSectionCommand : EFBaseCommand, ICommand<UpdateParticipationInSectionResultContext>
    {
        public UpdateParticipationInSectionCommand(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task ExecuteAsync(UpdateParticipationInSectionResultContext commandContext)
        {
            var dbPartOlymp = await _dbContext.ParticipationInSections.FindAsync(commandContext.ParticipationInSection.Id);
            dbPartOlymp.YearPeriod = commandContext.ParticipationInSection.YearPeriod;
            await _dbContext.SaveChangesAsync();
        }
    }

    public class UpdateParticipationInSectionResultContext : ICommandContext
    {
        public ParticipationInSection ParticipationInSection { get; set; }
    }
}
