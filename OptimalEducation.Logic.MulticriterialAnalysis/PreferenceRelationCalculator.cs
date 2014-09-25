using OptimalEducation.Interfaces.Logic.MulticriterialAnalysis;
using OptimalEducation.Interfaces.Logic.MulticriterialAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OptimalEducation.Implementation.Logic.MulticriterialAnalysis
{
    public class PreferenceRelationCalculator : IPreferenceRelationCalculator
    {
        const double diff = 0.05;
        const double w_i = 0.1;//на данный момент задается просто константой
        /// <summary>
        /// Получаем отношение предпочтения
        /// </summary>
        /// <returns>Список из Важных кластеров и их отношений с неважными(teta)</returns>
        public List<PreferenceRelation> GetPreferenceRelations(Dictionary<string, double> userCharacteristics)
        {
            var separatedCharacteristics = SeparateCharacterisicsToImprotantAnd_Unimportant(userCharacteristics);
            var importantCharactristics = separatedCharacteristics.Item1;
            var unImportantCharacteristics = separatedCharacteristics.Item2;

            var preferenceRelations = new List<PreferenceRelation>();
            //Определяем коэффициенты отн важности. Здесь настраиваем правило, по которому будем строить эти коэффициенты
            foreach (var impCharacteristic in importantCharactristics)
            {
                var preferenceRelation = new PreferenceRelation(impCharacteristic.Key);
                foreach (var unImpCharacteristic in unImportantCharacteristics)
                {
                    //1. Через формулу относительной важности с небольшими модификациями
                    var delta = impCharacteristic.Value - unImpCharacteristic.Value;
                    var teta = TetaMethod(w_i, delta);

                    preferenceRelation.Tetas.Add(unImpCharacteristic.Key, teta);
                }
                preferenceRelations.Add(preferenceRelation);
            }
            return preferenceRelations;
        }

        #region Helpers
        Tuple<Dictionary<string,double>,Dictionary<string,double>> SeparateCharacterisicsToImprotantAnd_Unimportant(Dictionary<string, double> userCharacteristics)
        {
            //Пусть пока работает по такому правилу:
            //Находим максимальный критерий
            var maxCharacteristic = GetMaxValue(userCharacteristics);
            //При предположении что все значения у нас от 0 до 1
            //находим близкие по значению критерии
            var importantCharacterisics = (from characteristic in userCharacteristics
                                        where (characteristic.Value >= (maxCharacteristic - diff))
                                  select characteristic).ToDictionary(p => p.Key, el => el.Value);

            var unImportantCharacterisics = (from characteristic in userCharacteristics
                                          where (characteristic.Value < (maxCharacteristic - diff))
                                    select characteristic).ToDictionary(p => p.Key, el => el.Value);

            return new Tuple<Dictionary<string, double>, Dictionary<string, double>>(importantCharacterisics, unImportantCharacterisics);
        }
        /// <summary>
        /// Вычисляем коэф. относительной важности
        /// </summary>
        /// <param name="w_i">сколько хотим получить(по значимому коэф-ту)</param>
        /// <param name="w_j">сколько готовы пожертвовать(по незначимому коэф-ту)</param>
        /// <returns></returns>
        double TetaMethod(double w_i, double w_j)
        {
            //На данный момент строятся одинаковые предпочтения для всех важных/неважных критериев
            var teta = w_j / (w_j + w_i);//Коэффициент относительной важности
            if (0 < teta && teta < 1)
                return teta;
            else
                throw new ArithmeticException("Значение коэффициента относительной важности не лежит в пределах 0<teta<1");
        }
        double GetMaxValue(Dictionary<string,double> userCharacteristics)
        {
            double maxValue = userCharacteristics.First().Value;
            foreach (var item in userCharacteristics)
            {
                if (item.Value > maxValue) maxValue = item.Value;
            }
            return maxValue;
        }
        #endregion
    }
}
