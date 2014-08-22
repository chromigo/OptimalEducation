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
    public class RemoveParticipationInSectionCommand : EFBaseCommand, ICommand<RemoveParticipationInSectionContext>
    {
        public RemoveParticipationInSectionCommand(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task ExecuteAsync(RemoveParticipationInSectionContext commandContext)
        {
            var participationinSection = await _dbContext.ParticipationInSections
                .Where(p => p.Id == commandContext.ParticipationInSectionId && p.EntrantsId == commandContext.EntrantId)
                .SingleOrDefaultAsync();
            _dbContext.ParticipationInSections.Remove(participationinSection);
            await _dbContext.SaveChangesAsync();
        }
    }

    public class RemoveParticipationInSectionContext : ICommandContext
    {
        public int EntrantId { get; set; }
        public int ParticipationInSectionId { get; set; }
    }
}
