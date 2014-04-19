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

        List<EducationLineAndCharacterisicsRow> educationLineRequrements;

        public MulticriterialAnalysis(Dictionary<string, double> userCharacterisics, List<EducationLineAndCharacterisicsRow> educationLineRequrements)
        {
            preferenceRelationCalculator = new PreferenceRelationCalculator(userCharacterisics);
            vectorCriteriaRecalculator = new VectorCriteriaRecalculator();
            parretoCalculator = new ParretoCalculator();

            this.educationLineRequrements = educationLineRequrements;
        }

        public List<EducationLineAndCharacterisicsRow> Calculate()
        {
            var userPref = preferenceRelationCalculator.GetPreferenceRelations();//Получаем предпочтения пользователя
            var recalculatedCharacterisics = vectorCriteriaRecalculator.RecalculateEducationLineCharacterisics(educationLineRequrements, userPref);//Пересчитываем кластеры университетов с учетом предпочтений пользователя
            var parretoEducationLineCharacterisics = parretoCalculator.ParretoSetCreate(recalculatedCharacterisics);//Строим множество паррето-оптимальных веткоров

            return parretoEducationLineCharacterisics;
        }
    }
}
