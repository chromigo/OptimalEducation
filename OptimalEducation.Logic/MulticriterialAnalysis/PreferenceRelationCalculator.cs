using OptimalEducation.Logic.MulticriterialAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Logic.MulticriterialAnalysis
{
    public class PreferenceRelationCalculator
    {
        //SeparateCharacterisicsToImprotantAnd_Unimportant - задается правило разбиения на группы важные/не важные. На данный момент по правилу: (max-0.1) -это важные
        const double diff = 0.05;
        //GetPreferenceRelations -задается логика определения отношения предпочтения(По важным/неважным критериям)

        Dictionary<string, double> _importantCharacterisics;
        Dictionary<string, double> _unImportantCharacterisics;

        /// <summary>
        /// Получаем отношение предпочтения
        /// </summary>
        /// <returns>Список из Важных кластеров и их отношений с неважными(teta)</returns>
        public List<PreferenceRelation> GetPreferenceRelations(Dictionary<string, double> userCharacteristics)
        {
            SeparateCharacterisicsToImprotantAnd_Unimportant(userCharacteristics);

            var preferenceRelations = new List<PreferenceRelation>();
            //Определяем коэффициенты отн важности. Здесь настраиваем правило, по которому будем строить эти коэффициенты
            foreach (var impCharacteristic in _importantCharacterisics)
            {
                var preferenceRelation = new PreferenceRelation(impCharacteristic.Key);
                foreach (var unImpCharacteristic in _unImportantCharacterisics)
                {
                    //TODO: Определеить более клевую логику по выбору коэффициентов относительной важности

                    //1. Через формулу относительной важности с небольшими модификациями
                    //значение важного - значение неважного
                    var diff = impCharacteristic.Value - unImpCharacteristic.Value;
                    var teta = TetaMethod(0.1, diff);//на данный момент задается просто константой

                    preferenceRelation.Tetas.Add(unImpCharacteristic.Key, teta);
                }
                preferenceRelations.Add(preferenceRelation);
            }
            return preferenceRelations;
        }

        #region Helpers
        void SeparateCharacterisicsToImprotantAnd_Unimportant(Dictionary<string, double> userCharacteristics)
        {
            //Пусть пока работает по такому правилу:
            //Находим максимальный критерий
            var maxCharacteristic = GetMaxValue(userCharacteristics);
            //При предположении что все значения у нас от 0 до 1
            //находим близкие по значению критерии - с разницей до -0,1
            _importantCharacterisics = (from characteristic in userCharacteristics
                                        where (characteristic.Value >= (maxCharacteristic - diff))
                                  select characteristic).ToDictionary(p => p.Key, el => el.Value);

            _unImportantCharacterisics = (from characteristic in userCharacteristics
                                          where (characteristic.Value < (maxCharacteristic - diff))
                                    select characteristic).ToDictionary(p => p.Key, el => el.Value);
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
