using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.CQRS;

namespace OptimalEducation.DAL.Commands.ParticipationInSchool
{
    public class RemoveParticipationInSchoolCommand : EfBaseCommand, ICommand<RemoveParticipationInShoolContext>
    {
        public RemoveParticipationInSchoolCommand(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task ExecuteAsync(RemoveParticipationInShoolContext commandContext)
        {
            var participationInSchool = await DbContext.ParticipationInSchools
                .Where(p => p.EntrantsId == commandContext.EntrantId && p.Id == commandContext.Id)
                .SingleAsync();
            DbContext.ParticipationInSchools.Remove(participationInSchool);
            await DbContext.SaveChangesAsync();
        }
    }

    public class RemoveParticipationInShoolContext : ICommandContext
    {
        public int EntrantId { get; set; }
        public int Id { get; set; }
    }
}