using System.Threading.Tasks;
using Interfaces.CQRS;
using OptimalEducation.DAL.Models;
using OptimalEducation.DAL.Queries;

namespace OptimalEducation.DAL.Commands
{
    public class UpdateParticipationInOlympiadResultCommand : EFBaseCommand, ICommand<UpdateParticipationInOlympiadResultContext>
    {
        public UpdateParticipationInOlympiadResultCommand(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task ExecuteAsync(UpdateParticipationInOlympiadResultContext commandContext)
        {
            var dbPartOlymp = await _dbContext.ParticipationInOlympiads.FindAsync(commandContext.ParticipationInOlympiadId);
            dbPartOlymp.Result = commandContext.UpdateResult;
            await _dbContext.SaveChangesAsync();
        }
    }

    public class UpdateParticipationInOlympiadResultContext : ICommandContext
    {
        public int ParticipationInOlympiadId { get; set; }
        public OlypmpiadResult UpdateResult { get; set; }
    }
}
