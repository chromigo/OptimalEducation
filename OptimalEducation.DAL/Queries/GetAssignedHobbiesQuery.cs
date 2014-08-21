using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces.CQRS;
using OptimalEducation.DAL.Models;
using OptimalEducation.DAL.ViewModels;

namespace OptimalEducation.DAL.Queries
{
    public class GetAssignedHobbiesQuery : EFBaseQuery, IQuery<GetAssignedHobbiesCriterion, Task<IEnumerable<AssignedHobbie>>>
    {
        public GetAssignedHobbiesQuery(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<IEnumerable<AssignedHobbie>> Ask(GetAssignedHobbiesCriterion criterion)
        {
            var userHobbieIds = await _dbContext.Entrants
                .Include(p => p.Hobbies)
                .Where(e => e.Id == criterion.EntrantId)
                .SelectMany(en=>en.Hobbies)
                .AsNoTracking()
                .Select(h=>h.Id)
                .ToListAsync();

            var allHobbies = await _dbContext.Hobbies
                .AsNoTracking()
                .ToListAsync();

            var viewModel = new List<AssignedHobbie>();
            foreach (var hobbie in allHobbies)
            {
                viewModel.Add(new AssignedHobbie
                {
                    Id = hobbie.Id,
                    Name = hobbie.Name,
                    IsAssigned = userHobbieIds.Contains(hobbie.Id)
                });
            }
            return viewModel;
        }

    }
    public class GetAssignedHobbiesCriterion : ICriterion
    {
        public int EntrantId { get; set; }
    }
}
