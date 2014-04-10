using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Logic.MulticriterialAnalysis
{
    public class MulticriterialAnalysis
    {
        Dictionary<string, double> _userCluster;

        Dictionary<string, double> _importantClusters;
        Dictionary<string, double> _unImportantClusters;

        List<Dictionary<string, double>> _universityClusters;

        public MulticriterialAnalysis(Dictionary<string, double> userCluster, List<Dictionary<string, double>> universityClusters)
        {
            _userCluster = userCluster;
            _universityClusters = universityClusters;
            SeparateClustersToImprotantAnd_UnImportant();

            var preferenceRelations = GetPreferenceRelations();//Получаем предпочтения пользователя
            var recalculatedUniversityClusters = RecalculateUniversityClusters(preferenceRelations);//Пересчитываем кластеры университетов с учетом предпочтений пользователя
            var parretoUniversityClusters = CreateParretoOptimalClusters(recalculatedUniversityClusters);//Строим множество паррето-оптимальных веткоров
        }

        #region PreferenceRelation
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
        List<PreferenceRelation> GetPreferenceRelations()
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
        static double TetaMethod()
        {
            //На данный момент строятся одинаковые предпочтения для всех важных/неважных критериев
            //готовы потерять 0.2 но получить 0.1
            var w_j = 0.2;//сколько готовы пожертвовать(по незначимому коэф-ту)
            var w_i = 0.1;//сколько хотим получить(по значимому коэф-ту)
            var teta = w_j / (w_j + w_i);//Коэффициент относительной важности
            //По идее все что <0.5 - означает что нам на самом деле ценне противоположный коэффициент
            return teta;
        }
        #endregion

        private List<Dictionary<string, double>> RecalculateUniversityClusters(List<PreferenceRelation> userPrefer)
        {
            var recalculateUniversityClusters = new List<Dictionary<string, double>>();
            foreach (var universityClusters in _universityClusters)
            {
                //Разбиваем все кластеры теккущего вуза на важные и неважные
                var importantClusters = new Dictionary<string, double>();
                var unImportantClusters = new Dictionary<string, double>();
                foreach (var cluster in universityClusters)
                {
                    if (userPrefer.Any(p => p.ImportantClusterName == cluster.Key))
                        importantClusters.Add(cluster.Key, cluster.Value);
                    else
                        unImportantClusters.Add(cluster.Key, cluster.Value);
                }

                //Пересчитываем значения неважных кластеров
                var recalculatedClusters = new Dictionary<string, double>();
                foreach (var importantCluster in importantClusters)
                {
                    recalculatedClusters.Add(importantCluster.Key, importantCluster.Value);//Добавляем текущий значимый
                    foreach (var unImportantCluster in unImportantClusters)
                    {
                        //формула пересчета
                        var f_i = importantCluster.Value;
                        var f_j = unImportantCluster.Value;

                        var preference = userPrefer.Single(p => p.ImportantClusterName == importantCluster.Key);//находим значимый кластер в списке с предпочтениями
                        var teta = preference.Tetas.Single(p => p.Key == unImportantCluster.Key).Value;//находим в этом кластере текущее отношение предпочтения

                        var recalculatedCluster = teta * f_i + (1 - teta) * f_j;

                        recalculatedClusters.Add(unImportantCluster.Key, recalculatedCluster);//добавляем пересчитанный
                    }
                }
                recalculateUniversityClusters.Add(recalculatedClusters);
            }
            return recalculateUniversityClusters;
        }

        private object CreateParretoOptimalClusters(List<Dictionary<string, double>> recalculatedUniversityClusters)
        {
            foreach (var clusters in recalculatedUniversityClusters)
	        {
                //Сравниваем текущий вектор со всеми остальными
                var otherClusters = recalculatedUniversityClusters.Where(p => p != clusters);
                foreach (var item in otherClusters)
                {
                    //Короче тут лучше массивами  
                }
	        }
            throw new NotImplementedException();
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
