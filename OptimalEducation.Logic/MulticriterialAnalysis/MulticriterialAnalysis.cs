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
    public interface IMulticriterialAnalysisRecomendator
    {
        List<EducationLine> Calculate();
    }

    public class MulticriterialAnalysis : IMulticriterialAnalysisRecomendator
    {
        PreferenceRelationCalculator preferenceRelationCalculator;
        VectorCriteriaRecalculator vectorCriteriaRecalculator;
        ParretoCalculator parretoCalculator;

        List<EducationLineWithCharacterisics> educationLineRequrements;

        ICharacterizer<Entrant> _entrantCharacterizer;
        ICharacterizer<EducationLine> _educationLineCharacterizer;

        public MulticriterialAnalysis(Entrant entrant, IEnumerable<EducationLine> educationLines, 
            ICharacterizer<Entrant> entrantCharacterizer, ICharacterizer<EducationLine> educationLineCharacterizer)
        {
            _entrantCharacterizer = entrantCharacterizer;
            _educationLineCharacterizer = educationLineCharacterizer;

            //Вычисляем характеристики пользователя
            var userCharacterisics = _entrantCharacterizer.Calculate(entrant);

            //Вычисляем характеристики учебных направлений
            educationLineRequrements = new List<EducationLineWithCharacterisics>();
            foreach (var item in educationLines)
            {
                if(item.EducationLinesRequirements.Count>0)
                {
                    var characteristics = _educationLineCharacterizer.Calculate(item);
                    var educationLineWithCharacteristics = new EducationLineWithCharacterisics(item)
                    {
                        Characterisics = characteristics
                    };
                    educationLineRequrements.Add(educationLineWithCharacteristics);
                }
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
