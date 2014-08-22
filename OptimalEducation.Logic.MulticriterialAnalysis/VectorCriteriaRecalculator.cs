using OptimalEducation.Interfaces.Logic.MulticriterialAnalysis;
using OptimalEducation.Interfaces.Logic.MulticriterialAnalysis.Models;
using System.Collections.Generic;
using System.Linq;

namespace OptimalEducation.Implementation.Logic.MulticriterialAnalysis
{
    public class VectorCriteriaRecalculator :IVectorCriteriaRecalculator
    {
        /// <summary>
        /// Перестраивает текущую "таблицу" учебноеНаправление/кластер c учетом предпочтений пользователя
        /// </summary>
        /// <param name="userPrefer">Отношения предпочтения пользователя</param>
        /// <returns>Перестроенная "таблица" учебноеНаправлени/кластер</returns>
        public List<EducationLineWithCharacterisics> RecalculateEducationLineCharacterisics(List<EducationLineWithCharacterisics> educationLineWithCharacterisics, List<PreferenceRelation> userPrefer)
        {
            var recalculateEducationLineCharacterisics = new List<EducationLineWithCharacterisics>();
            foreach (var item in educationLineWithCharacterisics)
            {
                //Разбиваем все кластеры текущего направления на важные и неважные
                var importantCharacterisics = new EducationLineWithCharacterisics(item.EducationLine, new Dictionary<string,double>());
                var unImportantCharacterisics = new EducationLineWithCharacterisics(item.EducationLine, new Dictionary<string,double>());
                foreach (var characterisics in item.Characterisics)
                {
                    if (userPrefer.Any(p => p.ImportantCharacterisicName == characterisics.Key))
                        importantCharacterisics.Characterisics.Add(characterisics.Key, characterisics.Value);
                    else
                        unImportantCharacterisics.Characterisics.Add(characterisics.Key, characterisics.Value);
                }

                //Пересчитываем значения неважных кластеров
                var recalculatedCharacterisics = new EducationLineWithCharacterisics(item.EducationLine, new Dictionary<string,double>());
                int i = 0;//используется для поментки пересозданных кластеров
                foreach (var importantCharacterisic in importantCharacterisics.Characterisics)
                {
                    i++;
                    //Добавляем текущий значимый
                    recalculatedCharacterisics.Characterisics.Add(importantCharacterisic.Key, importantCharacterisic.Value);
                    //Добавляем пересчитанные неважные
                    foreach (var unImportantCharacterisic in unImportantCharacterisics.Characterisics)
                    {
                        //формула пересчета
                        var f_i = importantCharacterisic.Value;
                        var f_j = unImportantCharacterisic.Value;

                        var preference = userPrefer.Single(p => p.ImportantCharacterisicName == importantCharacterisic.Key);//находим значимый кластер в списке с предпочтениями
                        var teta = preference.Tetas.Single(p => p.Key == unImportantCharacterisic.Key).Value;//находим в этом кластере текущее отношение предпочтения

                        var recalculatedCharacterisic = teta * f_i + (1 - teta) * f_j;

                        recalculatedCharacterisics.Characterisics.Add(unImportantCharacterisic.Key+"("+i+")", recalculatedCharacterisic);//добавляем пересчитанный
                    }
                }

                recalculateEducationLineCharacterisics.Add(recalculatedCharacterisics);
            }
            return recalculateEducationLineCharacterisics;
        }
    }
}
