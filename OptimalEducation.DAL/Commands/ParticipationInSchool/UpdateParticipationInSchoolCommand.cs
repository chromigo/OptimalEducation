using System.Threading.Tasks;
using Interfaces.CQRS;
using OptimalEducation.DAL.Models;
using OptimalEducation.DAL.Queries;
using System.Data.Entity.Core;

namespace OptimalEducation.DAL.Commands
{
    public class UpdateParticipationInSchoolCommand : EFBaseCommand, ICommand<UpdateParticipationInSchoolContext>
    {
        public UpdateParticipationInSchoolCommand(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task ExecuteAsync(UpdateParticipationInSchoolContext commandContext)
        {
            var dbPartOlymp = await _dbContext.ParticipationInSchools.FindAsync(commandContext.ParticipationInSchool.Id);
            dbPartOlymp.YearPeriod = commandContext.ParticipationInSchool.YearPeriod;
            await _dbContext.SaveChangesAsync();
        }
    }

    public class UpdateParticipationInSchoolContext : ICommandContext
    {
        public ParticipationInSchool ParticipationInSchool { get; set; }      
    }
}
