using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.CQRS;
using OptimalEducation.DAL.Models;

namespace OptimalEducation.DAL.Commands
{
    public class UpdateSchoolMarkOfEntrantCommand : EfBaseCommand, ICommand<UpdateSchoolMarkOfEntrantContext>
    {
        public UpdateSchoolMarkOfEntrantCommand(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task ExecuteAsync(UpdateSchoolMarkOfEntrantContext commandContext)
        {
            var oldEnrantSchoolMarks = await DbContext.SchoolMarks
                .Include(u => u.SchoolDiscipline)
                .Where(p => p.EntrantId == commandContext.EntrantId)
                .ToListAsync();

            var schoolMark = commandContext.SchoolMark.ToDictionary(p => p.SchoolDisciplineId);
            foreach (var oldMark in oldEnrantSchoolMarks)
            {
                oldMark.Result = schoolMark[oldMark.SchoolDisciplineId].Result;
            }

            await DbContext.SaveChangesAsync();
        }
    }

    public class UpdateSchoolMarkOfEntrantContext : ICommandContext
    {
        public int EntrantId { get; set; }
        public IEnumerable<SchoolMark> SchoolMark { get; set; }
    }
}