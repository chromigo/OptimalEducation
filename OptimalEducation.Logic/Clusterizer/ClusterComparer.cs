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
				var compareResult = GetEuclidDistance(entratnCluster, educationLineCluster);
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
				var compareResult = GetEuclidDistance(entratnCluster, educationLineCluster);
				if (compareResult.HasValue)
				{
					results.Add(entrant, compareResult.Value);
				}
			}
			return results;
		}

		#region DistanceCalculators
        //Выбор метрики полностью лежит на исследователе, 
        //поскольку результаты кластеризации могут существенно отличаться при использовании разных мер.

		/// <summary>
		/// Евклидово расстояние м/д абитуриентом и учебным направлением
		/// </summary>
		/// <param name="entrantCharacteristics">Значение вычисленного кластера для данного абитуриента</param>
		/// <param name="educationLineCharacteristics">Значение вычисленного кластера для данного учебного направления</param>
		private static double? GetEuclidDistance(Dictionary<string, double> entrantCharacteristics, Dictionary<string,double> educationLineCharacteristics)
		{
			//Наиболее распространенная функция расстояния. Представляет собой геометрическим расстоянием в многомерном пространстве
			double result=0;
			foreach (var edLineCharacteristic in educationLineCharacteristics)
			{
				//Если у ученика отсутсвую какие-либо характеристики, которые есть  в учебном направлении - исключаем из результатов?
				if (entrantCharacteristics.ContainsKey(edLineCharacteristic.Key))
				{
					result += Math.Pow(entrantCharacteristics[edLineCharacteristic.Key] - edLineCharacteristic.Value, 2);
				}
				else return null;
			}
			return Math.Sqrt(result);
		}

		/// <summary>
		/// Квадрат евклидова расстояния м/д абитуриентом и учебным направлением
		/// </summary>
		/// <param name="entrantCharacteristics">Значение вычисленного кластера для данного абитуриента</param>
		/// <param name="educationLineCharacteristics">Значение вычисленного кластера для данного учебного направления</param>
		private static double? GetSquareEuclidDistance(Dictionary<string, double> entrantCharacteristics, Dictionary<string, double> educationLineCharacteristics)
		{
			//Применяется для придания большего веса более отдаленным друг от друга объектам.
			double result = 0;
			foreach (var edLineCharacteristic in educationLineCharacteristics)
			{
				//Если у ученика отсутсвую какие-либо направления, которые есть  в учебном направлении - исключаем из результатов?
				if (entrantCharacteristics.ContainsKey(edLineCharacteristic.Key))
				{
					result += Math.Pow(entrantCharacteristics[edLineCharacteristic.Key] - edLineCharacteristic.Value, 2);
				}
				else return null;
			}
			return result;
		}

		/// <summary>
		/// Расстояние городских кварталов (манхэттенское расстояние) м/д абитуриентом и учебным направлением
		/// </summary>
		/// <param name="entrantCharacteristics">Значение вычисленного кластера для данного абитуриента</param>
		/// <param name="educationLineCluster">Значение вычисленного кластера для данного учебного направления</param>
		private static double? GetCityBlockDistance(Dictionary<string, double> entrantCharacteristics, Dictionary<string, double> educationLineCharacteristics)
		{
			//Влияние отдельных больших разностей (выбросов) уменьшается (т.к. они не возводятся в квадрат).
			double result = 0;
			foreach (var edLineCharacteristic in educationLineCharacteristics)
			{
				//Если у ученика отсутсвую какие-либо направления, которые есть  в учебном направлении - исключаем из результатов?
				if (entrantCharacteristics.ContainsKey(edLineCharacteristic.Key))
				{
					result += Math.Abs(entrantCharacteristics[edLineCharacteristic.Key] - edLineCharacteristic.Value);
				}
				else return null;
			}
			return result;
		}

        /// <summary>
        /// Расстояние Чебышева м/д абитуриентом и учебным направлением
        /// </summary>
        /// <param name="entrantCharacteristics">Значение вычисленного кластера для данного абитуриента</param>
        /// <param name="educationLineCluster">Значение вычисленного кластера для данного учебного направления</param>
        private static double? GetChebishevDistance(Dictionary<string, double> entrantCharacteristics, Dictionary<string, double> educationLineCharacteristics)
        {
            //Это расстояние может оказаться полезным, когда нужно определить два объекта как «различные», если они различаются по какой-либо одной координате. 
            double result = 0;
            foreach (var edLineCharacteristic in educationLineCharacteristics)
            {
                //Если у ученика отсутсвую какие-либо направления, которые есть  в учебном направлении - исключаем из результатов?
                if (entrantCharacteristics.ContainsKey(edLineCharacteristic.Key))
                {
                    var module = Math.Abs(entrantCharacteristics[edLineCharacteristic.Key] - edLineCharacteristic.Value);
                    result = Math.Max(module, result);
                }
                else return null;
            }
            return result;
        }

        /// <summary>
        /// Степенное расстояние м/д абитуриентом и учебным направлением
        /// </summary>
        /// <param name="entrantCharacteristics">Значение вычисленного кластера для данного абитуриента</param>
        /// <param name="educationLineCluster">Значение вычисленного кластера для данного учебного направления</param>
        /// <param name="p">Ответственен за постепенное взвешивание разностей по отдельным координатам</param>
        /// <param name="r">Ответственен за прогрессивное взвешивание больших расстояний между объектами</param>
        private static double? GetPowerDistance(Dictionary<string, double> entrantCharacteristics, Dictionary<string, double> educationLineCharacteristics, double p, double r)
        {
            //Применяется в случае, когда необходимо увеличить или уменьшить вес,
            //относящийся к размерности, для которой соответствующие объекты сильно отличаются.
            double result = 0;
            foreach (var edLineCharacteristic in educationLineCharacteristics)
            {
                //Если у ученика отсутсвую какие-либо характеристики, которые есть  в учебном направлении - исключаем из результатов?
                if (entrantCharacteristics.ContainsKey(edLineCharacteristic.Key))
                {
                    result += Math.Pow(entrantCharacteristics[edLineCharacteristic.Key] - edLineCharacteristic.Value, p);
                }
                else return null;
            }
            return Math.Pow(result, 1 / r);
        }
		#endregion
	}
}
