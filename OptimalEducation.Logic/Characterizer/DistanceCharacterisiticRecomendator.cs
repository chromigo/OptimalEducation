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
			var entratnCharacterisic = new EntrantCharacterizer(entrant).Characterisics;
			var results = new Dictionary<EducationLine, double>();
			foreach (var edLine in educationLines)
			{
				var educationLineCharacterisic = new EducationLineCharacterizer(edLine).Characteristics;
				//Выполняем сравнение
                var compareResult = CharacteristicDistance.GetEuclidDistance(entratnCharacterisic, educationLineCharacterisic);
				if(compareResult.HasValue)
				{
					results.Add(edLine, compareResult.Value);
				}
			}
			return results;
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
			var educationLineCharacterisic = new EducationLineCharacterizer(educationLine).Characteristics;
			var results = new Dictionary<Entrant, double>();
			foreach (var entrant in entrants)
			{
				var entratnCharacterisic = new EntrantCharacterizer(entrant).Characterisics;
				//Выполняем сравнение
                var compareResult = CharacteristicDistance.GetEuclidDistance(entratnCharacterisic, educationLineCharacterisic);
				if (compareResult.HasValue)
				{
					results.Add(entrant, compareResult.Value);
				}
			}
			return results;
		}
	}
}
