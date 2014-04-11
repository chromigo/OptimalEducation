using OptimalEducation.Logic.MulticriterialAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Logic.MulticriterialAnalysis
{
    public class MulticriterialAnalysis
    {
        PreferenceRelationCalculator preferenceRelationCalculator;
        VectorCriteriaRecalculator vectorCriteriaRecalculator;
        ParretoCalculator parretoCalculator;

        List<EducationLineAndClustersRow> educationLineRequrements;

        public MulticriterialAnalysis(Dictionary<string, double> userCluster, List<EducationLineAndClustersRow> educationLineRequrements)
        {
            preferenceRelationCalculator = new PreferenceRelationCalculator(userCluster);
            vectorCriteriaRecalculator = new VectorCriteriaRecalculator();
            parretoCalculator = new ParretoCalculator();

            this.educationLineRequrements = educationLineRequrements;
        }

        public List<EducationLineAndClustersRow> Calculate()
        {
            var userPref = preferenceRelationCalculator.GetPreferenceRelations();//Получаем предпочтения пользователя
            var recalculatedCluster = vectorCriteriaRecalculator.RecalculateEducationLineClusters(educationLineRequrements, userPref);//Пересчитываем кластеры университетов с учетом предпочтений пользователя
            var parretoEducationLineClusters = parretoCalculator.ParretoSetCreate(recalculatedCluster);//Строим множество паррето-оптимальных веткоров

            return parretoEducationLineClusters;
        }
    }
}
