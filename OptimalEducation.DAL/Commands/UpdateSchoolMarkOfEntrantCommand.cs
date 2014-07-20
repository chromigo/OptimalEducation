using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptimalEducation.DAL.Models;
using CQRS;
using OptimalEducation.DAL.Queries;

namespace OptimalEducation.DAL.Commands
{
    public class UpdateSchoolMarkOfEntrantCommand : EFBaseCommand, ICommand<UpdateSchoolMarkOfEntrantContext>
    {
        public UpdateSchoolMarkOfEntrantCommand(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task ExecuteAsync(UpdateSchoolMarkOfEntrantContext commandContext)
        {
            var oldEnrantSchoolMarks = await _dbContext.SchoolMarks
                .Include(u => u.SchoolDiscipline)
                .Where(p => p.EntrantId == commandContext.EntrantId)
                .ToListAsync();

            var schoolMark = commandContext.SchoolMark.ToDictionary(p=>p.SchoolDisciplineId);
            foreach (var oldMark in oldEnrantSchoolMarks)
            {
                oldMark.Result = schoolMark[oldMark.SchoolDisciplineId].Result;

            }

            await _dbContext.SaveChangesAsync();
        }
    }
    public class UpdateSchoolMarkOfEntrantContext : ICommandContext
    {
        public int EntrantId { get; set; }
        public IEnumerable<SchoolMark> SchoolMark { get; set; }
    }
}
