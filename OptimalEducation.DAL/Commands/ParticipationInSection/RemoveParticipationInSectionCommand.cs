using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.CQRS;

namespace OptimalEducation.DAL.Commands.ParticipationInSection
{
    public class RemoveParticipationInSectionCommand : EfBaseCommand, ICommand<RemoveParticipationInSectionContext>
    {
        public RemoveParticipationInSectionCommand(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task ExecuteAsync(RemoveParticipationInSectionContext commandContext)
        {
            var participationinSection = await DbContext.ParticipationInSections
                .Where(p => p.Id == commandContext.ParticipationInSectionId && p.EntrantsId == commandContext.EntrantId)
                .SingleOrDefaultAsync();
            DbContext.ParticipationInSections.Remove(participationinSection);
            await DbContext.SaveChangesAsync();
        }
    }

    public class RemoveParticipationInSectionContext : ICommandContext
    {
        public int EntrantId { get; set; }
        public int ParticipationInSectionId { get; set; }
    }
}