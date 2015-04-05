using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.CQRS;
using OptimalEducation.DAL.ViewModels;

namespace OptimalEducation.DAL.Queries
{
    public class GetAssignedHobbiesQuery : EfBaseQuery,
        IQuery<GetAssignedHobbiesCriterion, Task<IEnumerable<AssignedHobbie>>>
    {
        public GetAssignedHobbiesQuery(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<IEnumerable<AssignedHobbie>> Ask(GetAssignedHobbiesCriterion criterion)
        {
            var userHobbieIds = await DbContext.Entrants
                .Include(p => p.Hobbies)
                .Where(e => e.Id == criterion.EntrantId)
                .SelectMany(en => en.Hobbies)
                .AsNoTracking()
                .Select(h => h.Id)
                .ToListAsync();

            var allHobbies = await DbContext.Hobbies
                .AsNoTracking()
                .ToListAsync();

            return allHobbies.Select(hobbie => new AssignedHobbie
            {
                Id = hobbie.Id,
                Name = hobbie.Name,
                IsAssigned = userHobbieIds.Contains(hobbie.Id)
            }).ToList();
        }
    }

    public class GetAssignedHobbiesCriterion : ICriterion
    {
        public int EntrantId { get; set; }
    }
}