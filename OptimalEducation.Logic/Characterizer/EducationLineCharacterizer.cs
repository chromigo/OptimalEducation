using OptimalEducation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OptimalEducation.Logic.Characterizer
{
    /// <summary>
    /// Результаты кластеризации для учебного направления. 
    /// </summary>
    public class EducationLineCharacterizer
    {
        EducationLine _educationLine;
        Dictionary<string, double> _educationLineCluster = new Dictionary<string, double>();

        Dictionary<string, double> _totalCluster = new Dictionary<string, double>();
        public Dictionary<string, double> Cluster { get { return _totalCluster; } }

        public EducationLineCharacterizer(EducationLine educationLine)
        {
            _educationLine = educationLine;
            CalculateSum();
        }

        #region По заданным характеристикам строит частичные кластеры с результатами
        private void EducationLinesRequirementsClustering()
        {
            foreach (var requirement in _educationLine.EducationLinesRequirements)
            {
                var result = requirement.Requirement;
                var discipline = requirement.ExamDiscipline;
                foreach (var weight in discipline.Weights)
                {
                    var coeff = weight.Coefficient;
                    var clusterName = weight.Cluster.Name;

                    var clusterResult = result * coeff;
                    FillPartialCluster(_educationLineCluster, clusterName, clusterResult);
                }
            }
        }
        /// <summary>
        /// Заполняет выбранный кластер заданными значениями(добавляет или суммирует)
        /// </summary>
        /// <param name="clusterToFill">Кластер, который необходимо заполнить/обновить</param>
        /// <param name="clusterName">Элемент кластера, который добавляют/обновляют</param>
        /// <param name="clusterResult"></param>
        private void FillPartialCluster(Dictionary<string,double> clusterToFill ,string clusterName, double clusterResult)
        {
            if (!clusterToFill.ContainsKey(clusterName))
                clusterToFill.Add(clusterName, clusterResult);
            else
                clusterToFill[clusterName] += clusterResult;
        }
        #endregion
        /// <summary>
        /// Складывает результаты каждого частного кластера по определенному правилу
        /// </summary>
        private void CalculateSum()
        {
            EducationLinesRequirementsClustering();

            _totalCluster = _educationLineCluster;
            //Добавить если будут дополнительные характеристики
            //foreach (var item in educationLineCluster)
            //{
            //    FillTotalCluster(item);
            //}
        }

        private void FillTotalCluster(KeyValuePair<string, double> item)
        {
            if (!_totalCluster.ContainsKey(item.Key))
                _totalCluster.Add(item.Key, item.Value);
            else
                _totalCluster[item.Key] += item.Value;
        }
    }
}