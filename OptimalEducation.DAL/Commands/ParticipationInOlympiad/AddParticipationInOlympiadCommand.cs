using System.Threading.Tasks;
using Interfaces.CQRS;
using OptimalEducation.DAL.Models;
using OptimalEducation.DAL.Queries;

namespace OptimalEducation.DAL.Commands
{
    public class AddParticipationInOlympiadCommand: EFBaseCommand, ICommand<AddParticipationInOlympiadContext>
    {
        public AddParticipationInOlympiadCommand(IOptimalEducationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task ExecuteAsync(AddParticipationInOlympiadContext commandContext)
        {
            _dbContext.ParticipationInOlympiads.Add(commandContext.ParticipationInOlympiad);
            await _dbContext.SaveChangesAsync();
        }
    }

    public class AddParticipationInOlympiadContext:ICommandContext
    {
        public ParticipationInOlympiad ParticipationInOlympiad { get; set; }      
    }
}
