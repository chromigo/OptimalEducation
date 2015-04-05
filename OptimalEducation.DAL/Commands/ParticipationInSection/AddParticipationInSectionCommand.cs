using System.Threading.Tasks;
using Interfaces.CQRS;

namespace OptimalEducation.DAL.Commands.ParticipationInSection
{
    public class AddParticipationInSectionCommand : EfBaseCommand, ICommand<AddParticipationInSectionContext>
    {
        public AddParticipationInSectionCommand(IOptimalEducationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task ExecuteAsync(AddParticipationInSectionContext commandContext)
        {
            DbContext.ParticipationInSections.Add(commandContext.ParticipationInSection);
            await DbContext.SaveChangesAsync();
        }
    }

    public class AddParticipationInSectionContext : ICommandContext
    {
        public Models.ParticipationInSection ParticipationInSection { get; set; }
    }
}