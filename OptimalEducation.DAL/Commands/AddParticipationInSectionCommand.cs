using System.Threading.Tasks;
using CQRS;
using OptimalEducation.DAL.Models;
using OptimalEducation.DAL.Queries;

namespace OptimalEducation.DAL.Commands
{
    public class AddParticipationInSectionCommand: EFBaseCommand, ICommand<AddParticipationInSectionContext>
    {
        public AddParticipationInSectionCommand(IOptimalEducationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task ExecuteAsync(AddParticipationInSectionContext commandContext)
        {
            _dbContext.ParticipationInSections.Add(commandContext.ParticipationInSection);
            await _dbContext.SaveChangesAsync();
        }
    }

    public class AddParticipationInSectionContext:ICommandContext
    {
        public ParticipationInSection ParticipationInSection { get; set; }      
    }
}
