using OptimalEducation.Logic.MulticriterialAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Logic.MulticriterialAnalysis
{
    public class VectorCriteriaRecalculator
    {
        /// <summary>
        /// Перестраивает текущую "таблицу" учебноеНаправление/кластер c учетом предпочтений пользователя
        /// </summary>
        /// <param name="userPrefer">Отношения предпочтения пользователя</param>
        /// <returns>Перестроенная "таблица" учебноеНаправлени/кластер</returns>
        public List<EducationLineAndClustersRow> RecalculateEducationLineClusters(List<EducationLineAndClustersRow> educationLineClusters, List<PreferenceRelation> userPrefer)
        {
            var recalculateEducationLineClusters = new List<EducationLineAndClustersRow>();
            foreach (var educationLine in educationLineClusters)
            {
                //Разбиваем все кластеры текущего направления на важные и неважные
                var importantClusters = new EducationLineAndClustersRow(educationLine.Id);
                var unImportantClusters = new EducationLineAndClustersRow(educationLine.Id);
                foreach (var clusters in educationLine.Clusters)
                {
                    if (userPrefer.Any(p => p.ImportantClusterName == clusters.Key))
                        importantClusters.Clusters.Add(clusters.Key, clusters.Value);
                    else
                        unImportantClusters.Clusters.Add(clusters.Key, clusters.Value);
                }

                //Пересчитываем значения неважных кластеров
                var recalculatedClusters = new EducationLineAndClustersRow(educationLine.Id);
                int i = 0;//используется для поментки пересозданных кластеров
                foreach (var importantCluster in importantClusters.Clusters)
                {
                    i++;
                    //Добавляем текущий значимый
                    recalculatedClusters.Clusters.Add(importantCluster.Key, importantCluster.Value);
                    //Добавляем пересчитанные неважные
                    foreach (var unImportantCluster in unImportantClusters.Clusters)
                    {
                        //формула пересчета
                        var f_i = importantCluster.Value;
                        var f_j = unImportantCluster.Value;

                        var preference = userPrefer.Single(p => p.ImportantClusterName == importantCluster.Key);//находим значимый кластер в списке с предпочтениями
                        var teta = preference.Tetas.Single(p => p.Key == unImportantCluster.Key).Value;//находим в этом кластере текущее отношение предпочтения

                        var recalculatedCluster = teta * f_i + (1 - teta) * f_j;

                        recalculatedClusters.Clusters.Add(unImportantCluster.Key+"("+i+")", recalculatedCluster);//добавляем пересчитанный
                    }
                }

                recalculateEducationLineClusters.Add(recalculatedClusters);
            }
            return recalculateEducationLineClusters;
        }
    }
}
