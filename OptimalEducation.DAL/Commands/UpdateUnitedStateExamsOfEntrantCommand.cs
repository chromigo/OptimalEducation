using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptimalEducation.DAL.Models;
using Interfaces.CQRS;
using OptimalEducation.DAL.Queries;

namespace OptimalEducation.DAL.Commands
{
    public class UpdateUnitedStateExamsOfEntrantCommand : EFBaseCommand, ICommand<UpdateUnitedStateExamOfEntrantContext>
    {
        public UpdateUnitedStateExamsOfEntrantCommand(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task ExecuteAsync(UpdateUnitedStateExamOfEntrantContext commandContext)
        {
            var oldEnrantUnitedStateExams = await _dbContext.UnitedStateExams
                .Include(u => u.Discipline)
                .Where(p => p.EntrantId == commandContext.EntrantId)
                .ToListAsync();

            var newUnitedStateExam = commandContext.UnitedStateExams.ToDictionary(p=>p.ExamDisciplineId);
            foreach (var oldExam in oldEnrantUnitedStateExams)
            {
                oldExam.Result = newUnitedStateExam[oldExam.ExamDisciplineId].Result;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
    public class UpdateUnitedStateExamOfEntrantContext : ICommandContext
    {
        public int EntrantId { get; set; }
        public IEnumerable<UnitedStateExam> UnitedStateExams { get; set; }
    }
}
