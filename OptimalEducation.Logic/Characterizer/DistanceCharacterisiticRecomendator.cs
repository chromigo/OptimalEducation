﻿using OptimalEducation.DAL.Models;
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
        ICharacterizer<Entrant> _entrantCharacterizer;
        ICharacterizer<EducationLine> _educationLineCharacterizer;

        public EntrantDistanceRecomendator(ICharacterizer<Entrant> entrantCharacterizer, ICharacterizer<EducationLine> educationLineCharacterizer)
        {
            _entrantCharacterizer = entrantCharacterizer;
            _educationLineCharacterizer = educationLineCharacterizer;
        }
        /// <summary>
        /// Вычисляет рекомендации по учебным направлениям для конкретного абитуриента
        /// </summary>
        /// <param name="subject">Абитуриент, для которого строятся рекомендации</param>
        /// <param name="objects">Список учебных направлений, отфильтрованных заранее по определенному криетрию</param>
        /// <returns>Словарь из подходящих учебных направлений и значений близости</returns>
        public Dictionary<EducationLine, double> GetRecomendation(Entrant subject, IEnumerable<EducationLine> objects)
        {
            //Вычисляем кластеры для абитуриента и направлений
            var entrantCharacteristic = _entrantCharacterizer.Calculate(subject);
            var results = new Dictionary<EducationLine, double>();
            foreach (var edLine in objects)
            {
                var educationLineCharacterisic = _educationLineCharacterizer.Calculate(edLine);

                //Выполняем сравнение
                var compareResult = CharacteristicDistance.GetDistance(entrantCharacteristic, educationLineCharacterisic);
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
        EntrantCharacterizer _entrantCharacterizer;
        EducationLineCharacterizer _educationLineCharacterizer;
        public EducationLineDistanceRecomendator()
        {
            _entrantCharacterizer = new EntrantCharacterizer(new EntrantCalculationOptions());
            _educationLineCharacterizer = new EducationLineCharacterizer(new EducationLineCalculationOptions());
        }
        /// <summary>
        /// Вычисляет рекомендации по учебным направлениям для конкретного абитуриента
        /// </summary>
        /// <param name="subject">Абитуриент, для которого строятся рекомендации</param>
        /// <param name="objects">Список учебных направлений, отфильтрованных заранее по определенному криетрию</param>
        /// <returns>Словарь из подходящих учебных направлений и значений близости</returns>
        public Dictionary<Entrant, double> GetRecomendation(EducationLine subject, IEnumerable<Entrant> objects)
        {
            //Вычисляем кластеры для направления и абитуриентов
            var educationLineCharacterisic = _educationLineCharacterizer.Calculate(subject);
            var results = new Dictionary<Entrant, double>();
            foreach (var entrant in objects)
            {
                var entratnCharacterisic = _entrantCharacterizer.Calculate(entrant);
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
