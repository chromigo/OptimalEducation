using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Logic.MulticriterialAnalysis
{
    public class MostValueCoefficientFinder
    {
        Dictionary<string, double> _userCluster;

        Dictionary<string, double> _importantClusters;
        Dictionary<string, double> _unImportantClusters;

        public MostValueCoefficientFinder(Dictionary<string, double> userCluster)
        {
            _userCluster = userCluster;
            SeparateClustersToImprotantAnd_UnImportant();
        }
        void SeparateClustersToImprotantAnd_UnImportant()
        {
            //Пусть пока работает по такому правилу:
            //Находим максимальный критерий
            var maxCluster = _userCluster.Max().Value;
            //При предположении что все значения у нас от 0 до 1
            //находим близкие по значению критерии - с разницей до -0,1
            _importantClusters = (from cluster in _userCluster
                                    where (cluster.Value >= (maxCluster-0.1))
                                    select cluster).ToDictionary(p=>p.Key,el=>el.Value);

            _unImportantClusters = (from cluster in _userCluster
                                  where (cluster.Value < (maxCluster - 0.1))
                                  select cluster).ToDictionary(p => p.Key, el => el.Value);
        }
        /// <summary>
        /// Получаем отношение предпочтения
        /// </summary>
        /// <returns>Список из Важных кластеров и их отношений с неважными(teta)</returns>
        public List<PreferenceRelation> GetPreferenceRelations()
        {
            var preferenceRelations = new List<PreferenceRelation>();
            //Определяем коэффициенты отн важности. Здесь настраиваем правило, по которому будем строить эти коэффициенты
            //На данный момент: задаем фиксир значения вручную(СМ TODO)
            foreach (var impCluster in _importantClusters)
            {
                var preferenceRelation = new PreferenceRelation(impCluster.Key);
                foreach (var unImpCluster in _unImportantClusters)
                {
                    //TODO: Определеить более клевую логику по выбору коэффициентов относительной важности

                    //1. Через формулу относительной важности
                    //var teta = TetaMethod();

                    //2. Другой вариант - тупо попробовать использовать не формулу, а (1-значение значимости неважного коэффициента)
                    var teta = 1 - unImpCluster.Value;

                    //3. Etc
                    //var teta = Wizzard.SuperMathFormula_Magic();

                    preferenceRelation.Tetas.Add(unImpCluster.Key, teta);
                }
                preferenceRelations.Add(preferenceRelation);
            }
            return preferenceRelations;
        }

        private static double TetaMethod()
        {
            //На данный момент строятся одинаковые предпочтения для всех важных/неважных критериев
            //готовы потерять 0.2 но получить 0.1
            var w_j = 0.2;//сколько готовы пожертвовать(по незначимому коэф-ту)
            var w_i = 0.1;//сколько хотим получить(по значимому коэф-ту)
            var teta = w_j / (w_j + w_i);//Коэффициент относительной важности
            //По идее все что <0.5 - означает что нам на самом деле ценне противоположный коэффициент
            return teta;
        }
    }
    public class PreferenceRelation
    {
        public PreferenceRelation(string name)
        {
            ImportantClusterName=name;
        }
        public string ImportantClusterName { get; private set; }
        /// <summary>
        /// Словарь "Название неважного кластера, Тета - коэффциент"
        /// </summary>
        public Dictionary<string, double> Tetas { get; set; }
    }
}
