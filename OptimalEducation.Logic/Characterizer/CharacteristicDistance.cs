using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Logic.Characterizer
{
    public static class CharacteristicDistance
    {
        //Выбор метрики полностью лежит на исследователе, 
        //поскольку результаты кластеризации могут существенно отличаться при использовании разных мер.

        /// <summary>
        /// Евклидово расстояние м/д абитуриентом и учебным направлением
        /// </summary>
        /// <param name="entrantCharacteristics">Значение вычисленного кластера для данного абитуриента</param>
        /// <param name="educationLineCharacteristics">Значение вычисленного кластера для данного учебного направления</param>
        public static double? GetEuclidDistance(Dictionary<string, double> entrantCharacteristics, Dictionary<string, double> educationLineCharacteristics)
        {
            //Наиболее распространенная функция расстояния. Представляет собой геометрическим расстоянием в многомерном пространстве
            double result = 0;
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
        public static double? GetSquareEuclidDistance(Dictionary<string, double> entrantCharacteristics, Dictionary<string, double> educationLineCharacteristics)
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
        /// <param name="educationLineCharacteristics">Значение вычисленного кластера для данного учебного направления</param>
        public static double? GetCityBlockDistance(Dictionary<string, double> entrantCharacteristics, Dictionary<string, double> educationLineCharacteristics)
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
        /// <param name="educationLineCharacteristics">Значение вычисленного кластера для данного учебного направления</param>
        public static double? GetChebishevDistance(Dictionary<string, double> entrantCharacteristics, Dictionary<string, double> educationLineCharacteristics)
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
        /// <param name="educationLineCharacteristics">Значение вычисленного кластера для данного учебного направления</param>
        /// <param name="p">Ответственен за постепенное взвешивание разностей по отдельным координатам</param>
        /// <param name="r">Ответственен за прогрессивное взвешивание больших расстояний между объектами</param>
        public static double? GetPowerDistance(Dictionary<string, double> entrantCharacteristics, Dictionary<string, double> educationLineCharacteristics, double p, double r)
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
    }
}
