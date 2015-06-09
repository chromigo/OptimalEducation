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
                    var teta = TetaMethod(impCharacteristic.Value, unImpCharacteristic.Value);
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
        /// <returns></returns>
        private double TetaMethod(double imp, double unImp)
        {
            var teta = imp / (imp + unImp); //Коэффициент относительной важности. Не соотвествует с оригинальным - поправили после обсуждения с Ногиным
            if (0 < teta && teta < 1)
                return teta;
            throw new ArithmeticException("Значение коэффициента относительной важности не лежит в пределах 0<teta<1");
        }

        private double GetMaxValue(Dictionary<string, double> userCharacteristics)
        {
            var maxValue = userCharacteristics.First().Value;
            return userCharacteristics
                .Select(item => item.Value)
                .Concat(new[] {maxValue})
                .Max();
        }

        #endregion
    }
}