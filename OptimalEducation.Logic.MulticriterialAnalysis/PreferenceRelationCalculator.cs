using System;
using System.Collections.Generic;
using System.Linq;
using OptimalEducation.Interfaces.Logic.MulticriterialAnalysis;
using OptimalEducation.Interfaces.Logic.MulticriterialAnalysis.Models;

namespace OptimalEducation.Implementation.Logic.MulticriterialAnalysis
{
    public class PreferenceRelationCalculator : IPreferenceRelationCalculator
    {
        private const double Diff = 0.05;
        private const double WI = 0.1; //на данный момент задается просто константой

        /// <summary>
        ///     Получаем отношение предпочтения
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
                    var teta = TetaMethod(WI, delta);

                    preferenceRelation.Tetas.Add(unImpCharacteristic.Key, teta);
                }
                preferenceRelations.Add(preferenceRelation);
            }
            return preferenceRelations;
        }

        #region Helpers

        private Tuple<Dictionary<string, double>, Dictionary<string, double>>
            SeparateCharacterisicsToImprotantAnd_Unimportant(Dictionary<string, double> userCharacteristics)
        {
            //Пусть пока работает по такому правилу:
            //Находим максимальный критерий
            var maxCharacteristic = GetMaxValue(userCharacteristics);
            //При предположении что все значения у нас от 0 до 1
            //находим близкие по значению критерии
            var importantCharacterisics = (from characteristic in userCharacteristics
                where (characteristic.Value >= (maxCharacteristic - Diff))
                select characteristic).ToDictionary(p => p.Key, el => el.Value);

            var unImportantCharacterisics = (from characteristic in userCharacteristics
                where (characteristic.Value < (maxCharacteristic - Diff))
                select characteristic).ToDictionary(p => p.Key, el => el.Value);

            return new Tuple<Dictionary<string, double>, Dictionary<string, double>>(importantCharacterisics,
                unImportantCharacterisics);
        }

        /// <summary>
        ///     Вычисляем коэф. относительной важности
        /// </summary>
        /// <param name="wI">сколько хотим получить(по значимому коэф-ту)</param>
        /// <param name="wJ">сколько готовы пожертвовать(по незначимому коэф-ту)</param>
        /// <returns></returns>
        private double TetaMethod(double wI, double wJ)
        {
            //На данный момент строятся одинаковые предпочтения для всех важных/неважных критериев
            var teta = wJ/(wJ + wI); //Коэффициент относительной важности
            if (0 < teta && teta < 1)
                return teta;
            throw new ArithmeticException("Значение коэффициента относительной важности не лежит в пределах 0<teta<1");
        }

        private double GetMaxValue(Dictionary<string, double> userCharacteristics)
        {
            var maxValue = userCharacteristics.First().Value;
            foreach (var item in userCharacteristics)
            {
                if (item.Value > maxValue) maxValue = item.Value;
            }
            return maxValue;
        }

        #endregion
    }
}