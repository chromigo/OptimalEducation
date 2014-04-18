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
			var entratnCluster = new EntrantCharacterizer(entrant).Cluster;
			var results = new Dictionary<EducationLine, double>();
			foreach (var edLine in educationLines)
			{
				var educationLineCluster = new EducationLineCharacterizer(edLine).Cluster;
				//Выполняем сравнение
                var compareResult = CharacteristicDistance.GetEuclidDistance(entratnCluster, educationLineCluster);
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
			var educationLineCluster = new EducationLineCharacterizer(educationLine).Cluster;
			var results = new Dictionary<Entrant, double>();
			foreach (var entrant in entrants)
			{
				var entratnCluster = new EntrantCharacterizer(entrant).Cluster;
				//Выполняем сравнение
                var compareResult = CharacteristicDistance.GetEuclidDistance(entratnCluster, educationLineCluster);
				if (compareResult.HasValue)
				{
					results.Add(entrant, compareResult.Value);
				}
			}
			return results;
		}
	}
}
