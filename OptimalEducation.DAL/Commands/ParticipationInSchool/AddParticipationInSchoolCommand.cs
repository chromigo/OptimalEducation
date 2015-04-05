using System.Threading.Tasks;
using Interfaces.CQRS;

namespace OptimalEducation.DAL.Commands.ParticipationInSchool
{
    public class AddParticipationInSchoolCommand : EfBaseCommand, ICommand<AddParticipationInSchoolContext>
    {
        public AddParticipationInSchoolCommand(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task ExecuteAsync(AddParticipationInSchoolContext commandContext)
        {
            DbContext.ParticipationInSchools.Add(commandContext.ParticipationInSchool);
            await DbContext.SaveChangesAsync();
        }
    }

    public class AddParticipationInSchoolContext : ICommandContext
    {
        public Models.ParticipationInSchool ParticipationInSchool { get; set; }
    }
}