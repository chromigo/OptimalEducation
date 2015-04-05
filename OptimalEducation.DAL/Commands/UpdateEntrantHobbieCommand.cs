using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.CQRS;
using OptimalEducation.DAL.Models;

namespace OptimalEducation.DAL.Commands
{
    public class UpdateEntrantHobbieCommand : EfBaseCommand, ICommand<UpdateEntrantHobbieContext>
    {
        public UpdateEntrantHobbieCommand(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task ExecuteAsync(UpdateEntrantHobbieContext commandContext)
        {
            var currentEntrant = await DbContext.Entrants.SingleAsync(p => p.Id == commandContext.EntrantId);
            var allHobbies = await DbContext.Hobbies.ToListAsync();

            AddOrRemoveEntrantHobbies(commandContext.SelectedHobbies, allHobbies, currentEntrant);
            DbContext.Entry(currentEntrant).State = EntityState.Modified;
            await DbContext.SaveChangesAsync();
        }

        private void AddOrRemoveEntrantHobbies(string[] selectedHobbies, List<Hobbie> allHobbies, Entrant currentEntrant)
        {
            if (selectedHobbies == null)
            {
                foreach (var hobbie in allHobbies)
                {
                    currentEntrant.Hobbies.Remove(hobbie);
                }
            }
            else
            {
                var selectedHobbiesList = selectedHobbies.Select(int.Parse).ToList();

                var lastUserHobbieIds = currentEntrant.Hobbies.Select(h => h.Id);
                foreach (var hobbie in allHobbies)
                {
                    if (selectedHobbiesList.Contains(hobbie.Id))
                    {
                        //Если не было - добавляем
                        if (!lastUserHobbieIds.Contains(hobbie.Id))
                            currentEntrant.Hobbies.Add(hobbie);
                    }
                    else //не выбранное хобби
                    {
                        //Если было - удаляем
                        if (lastUserHobbieIds.Contains(hobbie.Id))
                            currentEntrant.Hobbies.Remove(hobbie);
                    }
                }
            }
        }
    }

    public class UpdateEntrantHobbieContext : ICommandContext
    {
        public int EntrantId { get; set; }
        public string[] SelectedHobbies { get; set; }
    }
}