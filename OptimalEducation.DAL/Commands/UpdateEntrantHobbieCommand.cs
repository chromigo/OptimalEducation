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
    public class UpdateEntrantHobbieCommand : EFBaseCommand, ICommand<UpdateEntrantHobbieContext>
    {
        public UpdateEntrantHobbieCommand(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task ExecuteAsync(UpdateEntrantHobbieContext commandContext)
        {
            var currentEntrant = await _dbContext.Entrants.SingleAsync(p => p.Id == commandContext.EntrantId);
            var allHobbies = await _dbContext.Hobbies.ToListAsync<Hobbie>();

            AddOrRemoveEntrantHobbies(commandContext.SelectedHobbies, allHobbies, currentEntrant);
            _dbContext.Entry(currentEntrant).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
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
                var selectedHobbiesList = new List<int>();
                foreach (var hobbie in selectedHobbies)
                {
                    selectedHobbiesList.Add(int.Parse(hobbie));
                }

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
