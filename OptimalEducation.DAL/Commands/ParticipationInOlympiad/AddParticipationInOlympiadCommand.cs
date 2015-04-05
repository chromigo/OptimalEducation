using System.Threading.Tasks;
using Interfaces.CQRS;

namespace OptimalEducation.DAL.Commands.ParticipationInOlympiad
{
    public class AddParticipationInOlympiadCommand : EfBaseCommand, ICommand<AddParticipationInOlympiadContext>
    {
        public AddParticipationInOlympiadCommand(IOptimalEducationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task ExecuteAsync(AddParticipationInOlympiadContext commandContext)
        {
            DbContext.ParticipationInOlympiads.Add(commandContext.ParticipationInOlympiad);
            await DbContext.SaveChangesAsync();
        }
    }

    public class AddParticipationInOlympiadContext : ICommandContext
    {
        public Models.ParticipationInOlympiad ParticipationInOlympiad { get; set; }
    }
}