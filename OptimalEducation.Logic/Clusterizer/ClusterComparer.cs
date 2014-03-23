using OptimalEducation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Logic.Clusterizer
{
	public static class ClusterComparer
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
			var entratnCluster = new EntrantClusterizer(entrant).Cluster;
			var results = new Dictionary<EducationLine, double>();
			foreach (var edLine in educationLines)
			{
				var educationLineCluster = new EducationLineClusterizer(edLine).Cluster;
				//Выполняем сравнение
				var compareResult = CompareClusters_Entrant_EducationLine(entratnCluster, educationLineCluster);
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
			var educationLineCluster = new EducationLineClusterizer(educationLine).Cluster;
			var results = new Dictionary<Entrant, double>();
			foreach (var entrant in entrants)
			{
				var entratnCluster = new EntrantClusterizer(entrant).Cluster;
				//Выполняем сравнение
				var compareResult = CompareClusters_Entrant_EducationLine(entratnCluster, educationLineCluster);
				if (compareResult.HasValue)
				{
					results.Add(entrant, compareResult.Value);
				}
			}
			return results;
		}
		/// <summary>
		/// Выполняет сравнение 2-х кластеров(ученик и учебное направление) и возвращает Евклидово расстояние межде ними
		/// </summary>
		/// <param name="entrantCluster">Значение вычисленного кластера для данного абитуриента</param>
		/// <param name="educationLineCluster">Значение вычисленного кластера для данного учебного направления</param>
		private static double? CompareClusters_Entrant_EducationLine(Dictionary<string, double> entrantCluster, Dictionary<string,double> educationLineCluster)
		{
			double result=0;
			foreach (var discipline in educationLineCluster)
			{
				//Если у ученика отсутсвую какие-либо направления, которые есть  в учебном направлении - исключаем из результатов?
				if (entrantCluster.ContainsKey(discipline.Key))
				{
					result += Math.Pow(entrantCluster[discipline.Key] - discipline.Value, 2);
				}
				else { return null; }
			}
			return Math.Sqrt(result);
		}
	}
}
