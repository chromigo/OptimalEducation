using OptimalEducation.DAL.Models;
using OptimalEducation.Interfaces.Logic.Characterizers;
using OptimalEducation.Interfaces.Logic.MulticriterialAnalysis;
using OptimalEducation.Interfaces.Logic.MulticriterialAnalysis.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace OptimalEducation.Implementation.Logic.MulticriterialAnalysis
{
    public class MulticriterialAnalysisRecomendator : IMulticriterialAnalysisRecomendator
    {
        readonly ICharacterizer<Entrant> _entrantCharacterizer;
        readonly ICharacterizer<EducationLine> _educationLineCharacterizer;

        readonly IPreferenceRelationCalculator _preferenceRelationCalculator;

        public MulticriterialAnalysisRecomendator(ICharacterizer<Entrant> entrantCharacterizer, ICharacterizer<EducationLine> educationLineCharacterizer,
            IPreferenceRelationCalculator preferenceRelationCalculator)
        {
            _entrantCharacterizer = entrantCharacterizer;
            _educationLineCharacterizer = educationLineCharacterizer;

            _preferenceRelationCalculator = preferenceRelationCalculator;
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
            var recalculatedCharacterisics = RecalculateEducationLineCharacterisics(educationLineRequrements, userPref);
            //Строим множество паррето-оптимальных веткоров
            var parretoEducationLineCharacterisics = ParretoSetCreate(recalculatedCharacterisics);

            var recomendedEducationLines = new List<EducationLine>();
            foreach (var item in parretoEducationLineCharacterisics)
            {
                recomendedEducationLines.Add(item.EducationLine);
            }

            return recomendedEducationLines;
        }

        #region Вспомогательные методы
        /// <summary>
        /// Перестраивает текущую "таблицу" учебноеНаправление/кластер c учетом предпочтений пользователя
        /// </summary>
        /// <param name="userPrefer">Отношения предпочтения пользователя</param>
        /// <returns>Перестроенная "таблица" учебноеНаправлени/кластер</returns>
        private List<EducationLineWithCharacterisics> RecalculateEducationLineCharacterisics(List<EducationLineWithCharacterisics> educationLineWithCharacterisics, List<PreferenceRelation> userPrefer)
        {
            var recalculateEducationLineCharacterisics = new List<EducationLineWithCharacterisics>();
            foreach (var item in educationLineWithCharacterisics)
            {
                //Разбиваем все кластеры текущего направления на важные и неважные
                var importantCharacterisics = new EducationLineWithCharacterisics(item.EducationLine, new Dictionary<string, double>());
                var unImportantCharacterisics = new EducationLineWithCharacterisics(item.EducationLine, new Dictionary<string, double>());
                foreach (var characterisics in item.Characterisics)
                {
                    if (userPrefer.Any(p => p.ImportantCharacterisicName == characterisics.Key))
                        importantCharacterisics.Characterisics.Add(characterisics.Key, characterisics.Value);
                    else
                        unImportantCharacterisics.Characterisics.Add(characterisics.Key, characterisics.Value);
                }

                //Пересчитываем значения неважных кластеров
                var recalculatedCharacterisics = new EducationLineWithCharacterisics(item.EducationLine, new Dictionary<string, double>());
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

                        recalculatedCharacterisics.Characterisics.Add(unImportantCharacterisic.Key + "(" + i + ")", recalculatedCharacterisic);//добавляем пересчитанный
                    }
                }

                recalculateEducationLineCharacterisics.Add(recalculatedCharacterisics);
            }
            return recalculateEducationLineCharacterisics;
        }

        /// <summary>
        /// Строит множество Парето из заданного множества
        /// </summary>
        /// <param name="educationLineCharacteristics">Множество, из которого требуется строить мн-во Парето</param>
        /// <returns>Построенное множество Парето</returns>
        private List<EducationLineWithCharacterisics> ParretoSetCreate(List<EducationLineWithCharacterisics> educationLineCharacteristics)
        {
            List<EducationLineWithCharacterisics> parretoList = new List<EducationLineWithCharacterisics>();

            for (int i = 0; i < educationLineCharacteristics.Count; i++)
            {
                var comparedItemCharacteristics = educationLineCharacteristics[i].Characterisics.ToArray();//Этот элемент(строку) сравниваем с остальными
                for (int j = i + 1; j < educationLineCharacteristics.Count; j++)
                {
                    var itemCharacteristics = educationLineCharacteristics[j].Characterisics.ToArray();//С этим элементом сравниваем
                    bool comparedIsMore = true;
                    for (int k = 0; k < comparedItemCharacteristics.Count(); k++)
                    {
                        if (comparedItemCharacteristics[k].Value < itemCharacteristics[k].Value)
                        {
                            //переход к шагу 5 алгоритма, если оказывается несравним
                            comparedIsMore = false;
                            break;
                        }
                    }

                    if (comparedIsMore)
                    {
                        //1-й вектор оказался больше 2-го, поэтому удаляем 2-й из общего множества
                        educationLineCharacteristics.Remove(educationLineCharacteristics[j]);
                        //т.к. массив по которому мы проходим поменялся
                        --j;
                    }
                    else//проверяем на равенство эти же объекты, но в другом порядке(шаг 5)
                    {
                        bool itemIsMore = true;
                        for (int k = 0; k < comparedItemCharacteristics.Count(); k++)
                        {
                            if (itemCharacteristics[k].Value < comparedItemCharacteristics[k].Value)
                            {
                                itemIsMore = false;
                                break;
                            }
                        }
                        if (itemIsMore)
                        {
                            //2-й вектор оказался больше 1-го, поэтому удаляем 1-й из общего множества
                            educationLineCharacteristics.Remove(educationLineCharacteristics[i]);
                            //т.к. массив по которому мы проходим поменялся
                            --i; break;
                        }
                    }
                }
                //После выполнения сравнения со всеми векторами, если вектор не удалился в процессе - удаляем его
                if (i != -1)
                {
                    parretoList.Add(educationLineCharacteristics[i]);
                    educationLineCharacteristics.Remove(educationLineCharacteristics[0]);
                    --i;
                }
            }

            return parretoList;
        }
        #endregion
    }
}
