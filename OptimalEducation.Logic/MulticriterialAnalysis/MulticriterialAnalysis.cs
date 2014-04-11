using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Logic.MulticriterialAnalysis
{
    public class MulticriterialAnalysis
    {
        Dictionary<string, double> _userCluster;

        Dictionary<string, double> _importantClusters;
        Dictionary<string, double> _unImportantClusters;

        List<EducationLineAndClustersRow> _educationLineClusters;

        public MulticriterialAnalysis(Dictionary<string, double> userCluster, List<EducationLineAndClustersRow> educationLineRequrements)
        {
            //_userCluster = userCluster;
            //_educationLineClusters = educationLineRequrements;
            //SeparateClustersToImprotantAnd_UnImportant();

            //var preferenceRelations = GetPreferenceRelations();//Получаем предпочтения пользователя
            //var recalculatedEducationLineClusters = RecalculateEducationLineClusters(preferenceRelations);//Пересчитываем кластеры университетов с учетом предпочтений пользователя
            //var parretoEducationLineClusters = ParretoSetCreate(recalculatedEducationLineClusters);//Строим множество паррето-оптимальных веткоров
        }

        #region PreferenceRelation
        void SeparateClustersToImprotantAnd_UnImportant()
        {
            //Пусть пока работает по такому правилу:
            //Находим максимальный критерий
            var maxCluster = _userCluster.Max().Value;
            //При предположении что все значения у нас от 0 до 1
            //находим близкие по значению критерии - с разницей до -0,1
            _importantClusters = (from cluster in _userCluster
                                    where (cluster.Value >= (maxCluster-0.1))
                                    select cluster).ToDictionary(p=>p.Key,el=>el.Value);

            _unImportantClusters = (from cluster in _userCluster
                                  where (cluster.Value < (maxCluster - 0.1))
                                  select cluster).ToDictionary(p => p.Key, el => el.Value);
        }
        /// <summary>
        /// Получаем отношение предпочтения
        /// </summary>
        /// <returns>Список из Важных кластеров и их отношений с неважными(teta)</returns>
        List<PreferenceRelation> GetPreferenceRelations()
        {
            var preferenceRelations = new List<PreferenceRelation>();
            //Определяем коэффициенты отн важности. Здесь настраиваем правило, по которому будем строить эти коэффициенты
            //На данный момент: задаем фиксир значения вручную(СМ TODO)
            foreach (var impCluster in _importantClusters)
            {
                var preferenceRelation = new PreferenceRelation(impCluster.Key);
                foreach (var unImpCluster in _unImportantClusters)
                {
                    //TODO: Определеить более клевую логику по выбору коэффициентов относительной важности

                    //1. Через формулу относительной важности
                    //var teta = TetaMethod();

                    //2. Другой вариант - тупо попробовать использовать не формулу, а (1-значение значимости неважного коэффициента)
                    var teta = 1 - unImpCluster.Value;

                    //3. Etc
                    //var teta = Wizzard.SuperMathFormula_Magic();

                    preferenceRelation.Tetas.Add(unImpCluster.Key, teta);
                }
                preferenceRelations.Add(preferenceRelation);
            }
            return preferenceRelations;
        }
        /// <summary>
        /// Вычисляем коэф. относительной важности
        /// </summary>
        /// <param name="w_i">сколько хотим получить(по значимому коэф-ту)</param>
        /// <param name="w_j">сколько готовы пожертвовать(по незначимому коэф-ту)</param>
        /// <returns></returns>
        double TetaMethod(double w_i = 0.2, double w_j = 0.1)
        {
            //На данный момент строятся одинаковые предпочтения для всех важных/неважных критериев
            var teta = w_j / (w_j + w_i);//Коэффициент относительной важности
            if (0 < teta && teta < 1)
                return teta;
            else
                throw new ArithmeticException("Значение коэффициента относительной важности не лежит в пределах 0<teta<1");
        }
        #endregion
        /// <summary>
        /// Перестраивает текущую "таблицу" учебноеНаправление/кластер c учетом предпочтений пользователя
        /// </summary>
        /// <param name="userPrefer">Отношения предпочтения пользователя</param>
        /// <returns>Перестроенная "таблица" учебноеНаправлени/кластер</returns>
        private List<EducationLineAndClustersRow> RecalculateEducationLineClusters(List<PreferenceRelation> userPrefer)
        {
            var recalculateEducationLineClusters = new List<EducationLineAndClustersRow>();
            foreach (var educationLine in _educationLineClusters)
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
                foreach (var importantCluster in importantClusters.Clusters)
                {
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

                        recalculatedClusters.Clusters.Add(unImportantCluster.Key, recalculatedCluster);//добавляем пересчитанный
                    }
                }

                recalculateEducationLineClusters.Add(recalculatedClusters);
            }
            return recalculateEducationLineClusters;
        }

        //private void CreateParettoSet(List<Dictionary<string, double>> recalculatedEducationLineClusters)
        //{
        //    var parettoSet = new List<Dictionary<string, double>>();

        //    var optimalElement = GetOptimalClustersAndRecalculateParretoSet(recalculatedEducationLineClusters);
        //    if (optimalElement != null)
        //        parettoSet.Add(optimalElement);
        //}
        /// <summary>
        /// Строит множество Парето из заданного множества
        /// </summary>
        /// <param name="educationLineClusters">Множество, из которого требуется строить мн-во Парето</param>
        /// <returns>Построенное множество Парето</returns>
        public List<EducationLineAndClustersRow> ParretoSetCreate(List<EducationLineAndClustersRow> educationLineClusters)
        {
            List<EducationLineAndClustersRow> parretoList = new List<EducationLineAndClustersRow>();

            for (int i = 0; i < educationLineClusters.Count; i++)
            {
                var comparedItemClusters = educationLineClusters[i].Clusters.ToArray();//Этот элемент(строку) сравниваем с остальными
                for (int j = i + 1; j < educationLineClusters.Count; j++)
                {
                    var itemClusters = educationLineClusters[j].Clusters.ToArray();//С этим элементом сравниваем
                    bool comparedIsMore = true;
                    for (int k = 0; k < comparedItemClusters.Count(); k++)
                    {
                        if (comparedItemClusters[k].Value < itemClusters[k].Value)
                        {
                            //переход к шагу 5 алгоритма, если оказывается несравним
                            comparedIsMore = false;
                            break;
                        }
                    }

                    if (comparedIsMore)
                    {
                        //1-й вектор оказался больше 2-го, поэтому удаляем 2-й из общего множества
                        educationLineClusters.Remove(educationLineClusters[j]);
                        //т.к. массив по которому мы проходим поменялся
                        --j;
                    }
                    else//проверяем на равенство эти же объекты, но в другом порядке(шаг 5)
                    {
                        bool itemIsMore = true;
                        for (int k = 0; k < comparedItemClusters.Count(); k++)
                        {
                            if (itemClusters[k].Value < comparedItemClusters[k].Value)
                            {
                                itemIsMore = false;
                                break;
                            }
                        }
                        if (itemIsMore)
                        {
                            //2-й вектор оказался больше 1-го, поэтому удаляем 1-й из общего множества
                            educationLineClusters.Remove(educationLineClusters[i]);
                            //т.к. массив по которому мы проходим поменялся
                            --i; break;
                        }
                    }
                }
                //После выполнения сравнения со всеми векторами, если вектор не удалился в процессе - удаляем его
                if (i != -1)
                {
                    parretoList.Add(educationLineClusters[i]);
                    educationLineClusters.Remove(educationLineClusters[0]);
                    --i;
                }
            }

            return parretoList;
        }
    }
    public class PreferenceRelation
    {
        public PreferenceRelation(string name)
        {
            ImportantClusterName=name;
        }
        public string ImportantClusterName { get; private set; }
        /// <summary>
        /// Словарь "Название неважного кластера, Тета - коэффциент"
        /// </summary>
        public Dictionary<string, double> Tetas { get; set; }
    }
    public class EducationLineAndClustersRow
    {
        public int Id { get; set; }
        public string Code { get; set; }

        public Dictionary<string, double> Clusters { get; set; }

        public EducationLineAndClustersRow(int id)
        {
            Id = id;
            Clusters = new Dictionary<string, double>();
        }
    }

#region заккоментированная версия алгоритма построения множества паретто
        //    /// <summary>
        ///// Выполняет 1 шаг из алгоритма построения множества паретто - Сравнивает первый со всеми остальными.
        ///// </summary>
        ///// <param name="recalculatedEducationLineClusters"></param>
        ///// <returns>Оптимальный вектор, если был вычислен. В out параметре - перестроенное множество</returns>
        //private Dictionary<string, double> GetOptimalClustersAndRecalculateParretoSet(List<EducationLineAndClustersRow> recalculatedEducationLineClusters)
        //{
        //    Dictionary<string,double> optimalCluster=null ;
        //    var clusters = recalculatedEducationLineClusters.FirstOrDefault();

        //    //Сравниваем текущий вектор со всеми остальными
        //    Dictionary<string, double> dominatedVector = null;
        //    var deletedClusters = new List<Dictionary<string, double>>();

        //    var otherClusters = recalculatedEducationLineClusters.Where(p => p != clusters);
        //    foreach (var item in otherClusters)
        //    {
        //        dominatedVector = GetDominatedVector(clusters, item);
        //        if (dominatedVector != null)
        //        {
        //            if (dominatedVector==clusters)
        //            {
        //                //Добавляем второй вектор(недоминируемый) в список удаляемых(позже удалим из множества)
        //                deletedClusters.Add(item);
        //            }
        //            else
        //            {
        //                deletedClusters.Add(clusters);
        //                break;
        //            }
        //        }
        //    }

        //    //не нашили доминирующих векторов
        //    //=>наш искомый вектор оптиммальный
        //    if (dominatedVector == null)
        //        optimalCluster=clusters;
        //    else 
        //    {
        //        if (dominatedVector==clusters)//находили доминирование, но оно принадлежит нашему вектору(например при сравнении с последним элементом)
        //            optimalCluster=clusters;
        //    }

        //    //Удаляем слабые элементы + текущий оптимальный из нашего множества
        //    foreach (var clusterForDelete in deletedClusters)
        //    {
        //        recalculatedEducationLineClusters.Remove(clusterForDelete);
        //    }
        //    if (optimalCluster != null) recalculatedEducationLineClusters.Remove(optimalCluster);

        //    return optimalCluster;
        //}
        ///// <summary>
        ///// Определяет какой из двух векторов доминируемый.
        ///// </summary>
        ///// <param name="firstClusters">Первый вектор(группа кластеров)</param>
        ///// <param name="secondClusters">Второй вектор(группа кластеров)</param>
        ///// <returns>Доминируемый вектор или null,если не сравнимы по отношению ">="</returns>
        //private Dictionary<string, double> GetDominatedVector(Dictionary<string, double> firstClusters, Dictionary<string, double> secondClusters)
        //{
        //    bool isFirstMoreOrEqual = IsFirstDominatiedOnSecond(firstClusters, secondClusters);

        //    if (isFirstMoreOrEqual)//Первый оказалься доминируемым
        //        return firstClusters;
        //    else//проверяем доминирует ли второй над первым
        //    {
        //        bool isSecondMoreOrEqual = IsFirstDominatiedOnSecond(secondClusters, firstClusters);
        //        if (isSecondMoreOrEqual) return secondClusters;
        //        else //никакой вектор не доминирует
        //            return null;
        //    }
            
        //}
        ///// <summary>
        ///// Проверяет, доминирует ли 1-й вектор над 2-м
        ///// </summary>
        ///// <param name="firstClusters">Первый вектор(группа кластеров)</param>
        ///// <param name="secondClusters">Второй вектор(группа кластеров)</param>
        ///// <returns>True- 1>=2</returns>
        //private bool IsFirstDominatiedOnSecond(Dictionary<string, double> firstClusters, Dictionary<string, double> secondClusters)
        //{
        //    foreach (var cluster in firstClusters)
        //    {
        //        if (cluster.Value < secondClusters[cluster.Key])
        //            return false;
        //    }
        //    return true;
        //}
#endregion
}
