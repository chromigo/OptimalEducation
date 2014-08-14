using OptimalEducation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Logic.Characterizer
{
    public interface IDistanceRecomendator<TSubject,TObjects>
    {
        Dictionary<TObjects, double> GetRecomendation(TSubject subject, IEnumerable<TObjects> objects);
    }

    public class EntrantDistanceRecomendator : IDistanceRecomendator<Entrant, EducationLine>
    {
        /// <summary>
        /// Вычисляет рекомендации по учебным направлениям для конкретного абитуриента
        /// </summary>
        /// <param name="subject">Абитуриент, для которого строятся рекомендации</param>
        /// <param name="objects">Список учебных направлений, отфильтрованных заранее по определенному криетрию</param>
        /// <returns>Словарь из подходящих учебных направлений и значений близости</returns>
        public Dictionary<EducationLine, double> GetRecomendation(Entrant subject, IEnumerable<EducationLine> objects)
        {
            //Вычисляем кластеры для абитуриента и направлений
            var entratnCharacteristic = new EntrantCharacterizer(subject, new EntrantCalculationOptions()).CalculateNormSum();
            var results = new Dictionary<EducationLine, double>();
            foreach (var edLine in objects)
            {
                var educationLineCharacterisic = new EducationLineCharacterizer(edLine, new EducationLineCalculationOptions()).CalculateNormSum();
                //Выполняем сравнение
                var compareResult = CharacteristicDistance.GetDistance(entratnCharacteristic, educationLineCharacterisic);
                if (compareResult.HasValue)
                {
                    results.Add(edLine, compareResult.Value);
                }
            }

            var sortedRes = new Dictionary<EducationLine, double>();
            var orderdRes = results.OrderBy(p => p.Value);
            foreach (var item in orderdRes)
            {
                sortedRes.Add(item.Key, item.Value);
            }
            return sortedRes;
        }
    }
    public class EducationLineDistanceRecomendator : IDistanceRecomendator<EducationLine, Entrant>
    {
        /// <summary>
        /// Вычисляет рекомендации по учебным направлениям для конкретного абитуриента
        /// </summary>
        /// <param name="subject">Абитуриент, для которого строятся рекомендации</param>
        /// <param name="objects">Список учебных направлений, отфильтрованных заранее по определенному криетрию</param>
        /// <returns>Словарь из подходящих учебных направлений и значений близости</returns>
        public Dictionary<Entrant, double> GetRecomendation(EducationLine subject, IEnumerable<Entrant> objects)
        {
            //Вычисляем кластеры для направления и абитуриентов
            var educationLineCharacterisic = new EducationLineCharacterizer(subject, new EducationLineCalculationOptions()).CalculateNormSum();
            var results = new Dictionary<Entrant, double>();
            foreach (var entrant in objects)
            {
                var entratnCharacterisic = new EntrantCharacterizer(entrant, new EntrantCalculationOptions()).CalculateNormSum();
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
