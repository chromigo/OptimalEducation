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
    public class RemoveParticipationInOlympiadCommand : EFBaseCommand, ICommand<RemoveParticipationInOlympiadContext>
    {
        public RemoveParticipationInOlympiadCommand(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task ExecuteAsync(RemoveParticipationInOlympiadContext commandContext)
        {
            var participationinolympiad = await _dbContext.ParticipationInOlympiads
                .SingleOrDefaultAsync(
                    p => p.Id == commandContext.ParticipationInOlympiadId && p.EntrantId == commandContext.EntrantId);
            _dbContext.ParticipationInOlympiads.Remove(participationinolympiad);
            await _dbContext.SaveChangesAsync();
        }
    }

    public class RemoveParticipationInOlympiadContext : ICommandContext
    {
        public int EntrantId { get; set; }
        public int ParticipationInOlympiadId { get; set; }
    }
}
