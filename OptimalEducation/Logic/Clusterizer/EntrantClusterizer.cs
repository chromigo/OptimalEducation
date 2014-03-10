using OptimalEducation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Logic.Clusterizer
{
    /// <summary>
    /// Результаты кластеризации для пользователя. 
    /// Класс инкапсулирует логику построения частичных кластеров(по каждой характеристике пользователя)
    /// и ихнему суммированию в целостный кластер.
    /// </summary>
    public class EntrantClusterizer
    {
        Entrant _entrant;
        Dictionary<string, double> unatedStatedExamCluster = new Dictionary<string, double>();
        Dictionary<string, double> schoolMarkCluster = new Dictionary<string, double>();
        Dictionary<string, double> olympiadCluster = new Dictionary<string, double>();

        Dictionary<string, double> _totalCluster;
        public Dictionary<string, double> Cluster { get { return _totalCluster; } }
        public EntrantClusterizer(Entrant entrant)
        {
            _entrant = entrant;
            CalculateSum();
        }
        //По баллам егэ строит частичный кластер с результатами
        private void UnatedStateExamClustering()
        {
            foreach (var exam in _entrant.UnitedStateExams)
            {
                var result = exam.Result;
                var discipline = exam.Discipline;
                foreach (var weight in discipline.Weights)
                {
                    var coeff = weight.Coefficient;
                    var cluster = weight.Cluster;

                    var clusterResult = result * coeff;
                    if (!unatedStatedExamCluster.ContainsKey(cluster.Name))
                        unatedStatedExamCluster.Add(cluster.Name, clusterResult);
                    else
                        unatedStatedExamCluster[cluster.Name] += clusterResult;
                }
            }
        }
        //По школьным оценкам строит частичный кластер с результатами
        private void SchoolMarkClustering()
        {
            foreach (var shoolMark in _entrant.SchoolMarks)
            {
                var result = shoolMark.Result;
                var discipline = shoolMark.SchoolDiscipline;
                foreach (var weight in discipline.Weights)
                {
                    var coeff = weight.Coefficient;
                    var cluster = weight.Cluster;

                    var clusterResult = result * coeff;
                    if (!schoolMarkCluster.ContainsKey(cluster.Name))
                        schoolMarkCluster.Add(cluster.Name, clusterResult);
                    else
                        schoolMarkCluster[cluster.Name] += clusterResult;
                }
            }
        }
        //По олимпиадам строит частичный кластер с результатами
        private void OlympiadClustering()
        {
            //foreach (var olympResult in _entrant.ParticipationInOlympiads)
            //{
            //    var result = olympResult.Result;
            //    var olympiad = olympResult.Olympiad;
            //    foreach (var weight in olympiad.Weights)
            //    {
            //        var coeff = weight.Coefficient;
            //        var cluster = weight.Cluster;

            //        var clusterResult = result * coeff;
            //        if (!olympiadCluster.ContainsKey(cluster.Name))
            //            olympiadCluster.Add(cluster.Name, clusterResult);
            //        else
            //            olympiadCluster[cluster.Name] += clusterResult;
            //    }
            //}
        }
        /// <summary>
        /// Складывает результаты каждого частного кластера по определенному правилу
        /// </summary>
        private void CalculateSum()
        {
            UnatedStateExamClustering();
            SchoolMarkClustering();
            //TODO: Добавить остальные методы

            _totalCluster = new Dictionary<string, double>();
            foreach (var useCluster in unatedStatedExamCluster)
            {
                if (!_totalCluster.ContainsKey(useCluster.Key))
                    _totalCluster.Add(useCluster.Key, useCluster.Value);
                else
                    _totalCluster[useCluster.Key] += useCluster.Value;
            }
            foreach (var shoolMarkCluster in schoolMarkCluster)
            {
                if (!_totalCluster.ContainsKey(shoolMarkCluster.Key))
                    _totalCluster.Add(shoolMarkCluster.Key, shoolMarkCluster.Value);
                else
                    _totalCluster[shoolMarkCluster.Key] += shoolMarkCluster.Value;
            }
            //TODO: Добавить остальные методы
        }
    }
}
