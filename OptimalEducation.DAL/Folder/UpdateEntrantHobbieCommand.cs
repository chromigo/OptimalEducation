using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptimalEducation.DAL.Models;

namespace OptimalEducation.DAL.Folder
{
    public class UpdateEntrantHobbieCommand
    {
        private readonly OptimalEducationDbContext _dbContext;

        public UpdateEntrantHobbieCommand(OptimalEducationDbContext context)
        {
            _dbContext = context;
        }

        public async Task Execute(int entrantId, string[] selectedHobbies)
        {
            var currentEntrant = await _dbContext.Entrants.SingleAsync(p => p.Id == entrantId);
            var allHobbies = await _dbContext.Hobbies.ToListAsync<Hobbie>();

            AddOrRemoveEntrantHobbies(selectedHobbies, allHobbies, currentEntrant);
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
}
