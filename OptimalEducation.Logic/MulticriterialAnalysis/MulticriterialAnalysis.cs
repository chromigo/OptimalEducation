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
        List<EducationLine> Calculate(Entrant entrant, IEnumerable<EducationLine> educationLines);
    }

    public class MulticriterialAnalysis : IMulticriterialAnalysisRecomendator
    {
        readonly ICharacterizer<Entrant> _entrantCharacterizer;
        readonly ICharacterizer<EducationLine> _educationLineCharacterizer;

        readonly PreferenceRelationCalculator _preferenceRelationCalculator;
        readonly VectorCriteriaRecalculator _vectorCriteriaRecalculator;
        readonly ParretoCalculator _parretoCalculator;

        public MulticriterialAnalysis(ICharacterizer<Entrant> entrantCharacterizer, ICharacterizer<EducationLine> educationLineCharacterizer)
        {
            _entrantCharacterizer = entrantCharacterizer;
            _educationLineCharacterizer = educationLineCharacterizer;

            _preferenceRelationCalculator = new PreferenceRelationCalculator();
            _vectorCriteriaRecalculator = new VectorCriteriaRecalculator();
            _parretoCalculator = new ParretoCalculator();
        }

        public List<EducationLine> Calculate(Entrant entrant, IEnumerable<EducationLine> educationLines)
        {
            //Вычисляем характеристики пользователя
            var userCharacterisics = _entrantCharacterizer.Calculate(entrant);

            //Вычисляем характеристики учебных направлений
            var educationLineRequrements = new List<EducationLineWithCharacterisics>();
            foreach (var item in educationLines)
            {
                if (item.EducationLinesRequirements.Count > 0)
                {
                    var characteristics = _educationLineCharacterizer.Calculate(item);
                    var educationLineWithCharacteristics = new EducationLineWithCharacterisics(item, characteristics);
                    educationLineRequrements.Add(educationLineWithCharacteristics);
                }
            }
            
            //Получаем предпочтения пользователя
            var userPref = _preferenceRelationCalculator.GetPreferenceRelations(userCharacterisics);
            //Пересчитываем кластеры университетов с учетом предпочтений пользователя
            var recalculatedCharacterisics = _vectorCriteriaRecalculator.RecalculateEducationLineCharacterisics(educationLineRequrements, userPref);
            //Строим множество паррето-оптимальных веткоров
            var parretoEducationLineCharacterisics = _parretoCalculator.ParretoSetCreate(recalculatedCharacterisics);

            var recomendedEducationLines = new List<EducationLine>();
            foreach (var item in parretoEducationLineCharacterisics)
            {
                recomendedEducationLines.Add(item.EducationLine);
            }

            return recomendedEducationLines;
        }
    }
}
