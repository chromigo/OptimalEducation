using OptimalEducation.DAL.Models;
using OptimalEducation.Logic.Characterizer;
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

        List<EducationLineWithCharacterisics> educationLineRequrements=new List<EducationLineWithCharacterisics>();

        public MulticriterialAnalysis(Entrant entrant, IEnumerable<EducationLine> educationLines)
        {
            //Вычисляем характеристики пользователя
            var entrantCharacterizer = new EntrantCharacterizer(entrant);
            var userCharacterisics = entrantCharacterizer.Characterisics;

            //Вычисляем характеристики учебных направлений
            educationLineRequrements = new List<EducationLineWithCharacterisics>();
            foreach (var item in educationLines)
            {
                var educationLineCharacterizer = new EducationLineCharacterizer(item);
                var characteristics = educationLineCharacterizer.Characteristics;
                var educationLineWithCharacteristics = new EducationLineWithCharacterisics(item)
                {
                    Characterisics = characteristics
                };
                educationLineRequrements.Add(educationLineWithCharacteristics);
            }


            preferenceRelationCalculator = new PreferenceRelationCalculator(userCharacterisics);
            vectorCriteriaRecalculator = new VectorCriteriaRecalculator();
            parretoCalculator = new ParretoCalculator();
        }

        public List<EducationLine> Calculate()
        {
            //Получаем предпочтения пользователя
            var userPref = preferenceRelationCalculator.GetPreferenceRelations();
            //Пересчитываем кластеры университетов с учетом предпочтений пользователя
            var recalculatedCharacterisics = vectorCriteriaRecalculator.RecalculateEducationLineCharacterisics(educationLineRequrements, userPref);
            //Строим множество паррето-оптимальных веткоров
            var parretoEducationLineCharacterisics = parretoCalculator.ParretoSetCreate(recalculatedCharacterisics);

            var recomendedEducationLines = new List<EducationLine>();
            foreach (var item in parretoEducationLineCharacterisics)
            {
                recomendedEducationLines.Add(item.EducationLine);
            }

            return recomendedEducationLines;
        }
    }
}
