using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OptimalEducation.DAL.Models;
using OptimalEducation.Interfaces.Logic.Characterizers;
using OptimalEducation.Interfaces.Logic.DistanceRecomendator;

namespace OptimalEducation.Implementation.Logic.DistanceRecomendator
{
    public class EducationLineDistanceRecomendator : IDistanceRecomendator<EducationLine, Entrant>
    {
        private readonly ICharacterizer<EducationLine> _educationLineCharacterizer;
        private readonly ICharacterizer<Entrant> _entrantCharacterizer;

        public EducationLineDistanceRecomendator(ICharacterizer<Entrant> entrantCharacterizer,
            ICharacterizer<EducationLine> educationLineCharacterizer)
        {
            _entrantCharacterizer = entrantCharacterizer;
            _educationLineCharacterizer = educationLineCharacterizer;
        }

        /// <summary>
        ///     Вычисляет рекомендации по учебным направлениям для конкретного абитуриента
        /// </summary>
        /// <param name="subject">Абитуриент, для которого строятся рекомендации</param>
        /// <param name="objects">Список учебных направлений, отфильтрованных заранее по определенному криетрию</param>
        /// <returns>Словарь из подходящих учебных направлений и значений близости</returns>
        public async Task<Dictionary<Entrant, double>> GetRecomendation(EducationLine subject,
            IEnumerable<Entrant> objects)
        {
            //Вычисляем кластеры для направления и абитуриентов
            var educationLineCharacterisic = await _educationLineCharacterizer.Calculate(subject);
            var results = new Dictionary<Entrant, double>();
            foreach (var entrant in objects)
            {
                var entratnCharacterisic = await _entrantCharacterizer.Calculate(entrant);
                //Выполняем сравнение
                var compareResult = CharacteristicDistance.GetDistance(entratnCharacterisic, educationLineCharacterisic);
                if (compareResult.HasValue)
                {
                    results.Add(entrant, compareResult.Value);
                }
            }

            var sortedRes = new Dictionary<Entrant, double>();
            var orderdRes = results.OrderBy(p => p.Value);
            foreach (var item in orderdRes)
            {
                sortedRes.Add(item.Key, item.Value);
            }
            return sortedRes;
        }
    }
}