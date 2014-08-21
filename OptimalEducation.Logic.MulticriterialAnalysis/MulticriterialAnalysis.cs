﻿using OptimalEducation.DAL.Models;
using OptimalEducation.Logic.Characterizers;
using OptimalEducation.Logic.Characterizers.Interfaces;
using OptimalEducation.Logic.MulticriterialAnalysis.Interfaces;
using OptimalEducation.Logic.MulticriterialAnalysis.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Logic.MulticriterialAnalysis
{
    public class MulticriterialAnalysis : IMulticriterialAnalysisRecomendator
    {
        readonly ICharacterizer<Entrant> _entrantCharacterizer;
        readonly ICharacterizer<EducationLine> _educationLineCharacterizer;

        readonly IPreferenceRelationCalculator _preferenceRelationCalculator;
        readonly IVectorCriteriaRecalculator _vectorCriteriaRecalculator;
        readonly IParretoCalculator _parretoCalculator;

        public MulticriterialAnalysis(ICharacterizer<Entrant> entrantCharacterizer, ICharacterizer<EducationLine> educationLineCharacterizer,
            IPreferenceRelationCalculator preferenceRelationCalculator,
            IVectorCriteriaRecalculator vectorCriteriaRecalculator,
            IParretoCalculator parretoCalculator)
        {
            _entrantCharacterizer = entrantCharacterizer;
            _educationLineCharacterizer = educationLineCharacterizer;

            _preferenceRelationCalculator = preferenceRelationCalculator;
            _vectorCriteriaRecalculator = vectorCriteriaRecalculator;
            _parretoCalculator = parretoCalculator;
        }

        public async Task<List<EducationLine>> Calculate(Entrant entrant, IEnumerable<EducationLine> educationLines)
        {
            //Вычисляем характеристики пользователя
            var userCharacterisics = await _entrantCharacterizer.Calculate(entrant);

            //Вычисляем характеристики учебных направлений
            var educationLineRequrements = new List<EducationLineWithCharacterisics>();
            foreach (var item in educationLines)
            {
                if (item.EducationLinesRequirements.Count > 0)
                {
                    var characteristics = await _educationLineCharacterizer.Calculate(item);
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
