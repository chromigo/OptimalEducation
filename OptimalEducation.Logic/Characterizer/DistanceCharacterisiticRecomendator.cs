using OptimalEducation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Logic.Characterizer
{
	public static class DistanceCharacterisiticRecomendator
	{
		/// <summary>
		/// Вычисляет рекомендации по учебным направлениям для конкретного абитуриента
		/// </summary>
		/// <param name="entrant">Абитуриент, для которого строятся рекомендации</param>
		/// <param name="educationLines">Список учебных направлений, отфильтрованных заранее по определенному криетрию</param>
		/// <returns>Словарь из подходящих учебных направлений и значений близости</returns>
		public static Dictionary<EducationLine, double> GetRecomendationForEntrant(Entrant entrant, List<EducationLine> educationLines)
		{
			//Вычисляем кластеры для абитуриента и направлений
            var entratnCharacteristic = new EntrantCharacterizer(entrant, new EntrantCalculationOptions()).CalculateNormSum();
			var results = new Dictionary<EducationLine, double>();
			foreach (var edLine in educationLines)
			{
                var educationLineCharacterisic = new EducationLineCharacterizer(edLine, new EducationLineCalculationOptions()).CalculateNormSum();
				//Выполняем сравнение
                var compareResult = CharacteristicDistance.GetDistance(entratnCharacteristic, educationLineCharacterisic);
				if(compareResult.HasValue)
				{
					results.Add(edLine, compareResult.Value);
				}
			}

            var sortedRes = new Dictionary<EducationLine, double>();
            var orderdRes = results.OrderBy(p=>p.Value);
            foreach (var item in orderdRes)
            {
                sortedRes.Add(item.Key, item.Value);
            }
            return sortedRes;
		}
		/// <summary>
		/// Вычисляет рекомендации по абитуриентам для конкретного учебного направления
		/// </summary>
		/// <param name="educationLine">Учебное направление, для которого строятся рекомендации</param>
		/// <param name="entrants">Список абитуриентов, отфильтрованных заранее по определенному криетрию</param>
		/// <returns>Словарь из подходящих абитуриентов и значений близости</returns>
		public static Dictionary<Entrant, double> GetRecomendationForEducationLine(EducationLine educationLine, List<Entrant> entrants)
		{
			//Вычисляем кластеры для направления и абитуриентов
			var educationLineCharacterisic = new EducationLineCharacterizer(educationLine, new EducationLineCalculationOptions()).CalculateNormSum();
			var results = new Dictionary<Entrant, double>();
			foreach (var entrant in entrants)
			{
				var entratnCharacterisic = new EntrantCharacterizer(entrant,new EntrantCalculationOptions()).CalculateNormSum();
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
