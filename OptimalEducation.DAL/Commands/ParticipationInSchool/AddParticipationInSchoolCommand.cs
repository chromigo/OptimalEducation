using System.Threading.Tasks;
using CQRS;
using OptimalEducation.DAL.Models;
using OptimalEducation.DAL.Queries;

namespace OptimalEducation.DAL.Commands
{
    public class AddParticipationInSchoolCommand : EFBaseCommand, ICommand<AddParticipationInSchoolContext>
    {
        public AddParticipationInSchoolCommand(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task ExecuteAsync(AddParticipationInSchoolContext commandContext)
        {
            _dbContext.ParticipationInSchools.Add(commandContext.ParticipationInSchool);
            await _dbContext.SaveChangesAsync();
        }
    }

    public class AddParticipationInSchoolContext : ICommandContext
    {
        public ParticipationInSchool ParticipationInSchool { get; set; }      
    }
}
