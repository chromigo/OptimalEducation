using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces.CQRS;
using OptimalEducation.DAL.Models;
using OptimalEducation.DAL.Queries;

namespace OptimalEducation.DAL.Commands
{
    public class RemoveParticipationInSchoolCommand : EFBaseCommand, ICommand<RemoveParticipationInShoolContext>
    {
        public RemoveParticipationInSchoolCommand(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task ExecuteAsync(RemoveParticipationInShoolContext commandContext)
        {
            var participationInSchool = await _dbContext.ParticipationInSchools
                .Where(p => p.EntrantsId == commandContext.EntrantId && p.Id == commandContext.Id)
                .SingleAsync();
            _dbContext.ParticipationInSchools.Remove(participationInSchool);
            await _dbContext.SaveChangesAsync();
        }
    }

    public class RemoveParticipationInShoolContext : ICommandContext
    {
        public int EntrantId { get; set; }
        public int Id { get; set; }
    }
}
