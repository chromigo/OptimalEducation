using System.Data.Entity;
using System.Threading.Tasks;
using Interfaces.CQRS;

namespace OptimalEducation.DAL.Commands.ParticipationInOlympiad
{
    public class RemoveParticipationInOlympiadCommand : EfBaseCommand, ICommand<RemoveParticipationInOlympiadContext>
    {
        public RemoveParticipationInOlympiadCommand(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task ExecuteAsync(RemoveParticipationInOlympiadContext commandContext)
        {
            var participationinolympiad = await DbContext.ParticipationInOlympiads
                .SingleOrDefaultAsync(
                    p => p.Id == commandContext.ParticipationInOlympiadId && p.EntrantId == commandContext.EntrantId);
            DbContext.ParticipationInOlympiads.Remove(participationinolympiad);
            await DbContext.SaveChangesAsync();
        }
    }

    public class RemoveParticipationInOlympiadContext : ICommandContext
    {
        public int EntrantId { get; set; }
        public int ParticipationInOlympiadId { get; set; }
    }
}