using OptimalEducation.Logic.MulticriterialAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Logic.MulticriterialAnalysis
{
    public class ParretoCalculator
    {
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
}
