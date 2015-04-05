using System.Threading.Tasks;
using Interfaces.CQRS;
using OptimalEducation.DAL.Models;

namespace OptimalEducation.DAL.Commands.ParticipationInOlympiad
{
    public class UpdateParticipationInOlympiadResultCommand : EfBaseCommand, ICommand<UpdateParticipationInOlympiadResultContext>
    {
        public UpdateParticipationInOlympiadResultCommand(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task ExecuteAsync(UpdateParticipationInOlympiadResultContext commandContext)
        {
            var dbPartOlymp = await DbContext.ParticipationInOlympiads.FindAsync(commandContext.ParticipationInOlympiadId);
            dbPartOlymp.Result = commandContext.UpdateResult;
            await DbContext.SaveChangesAsync();
        }
    }

    public class UpdateParticipationInOlympiadResultContext : ICommandContext
    {
        public int ParticipationInOlympiadId { get; set; }
        public OlypmpiadResult UpdateResult { get; set; }
    }
}
