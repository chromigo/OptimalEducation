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
        /// <param name="educationLineCharacteristics">Множество, из которого требуется строить мн-во Парето</param>
        /// <returns>Построенное множество Парето</returns>
        public List<EducationLineAndCharacterisicsRow> ParretoSetCreate(List<EducationLineAndCharacterisicsRow> educationLineCharacteristics)
        {
            List<EducationLineAndCharacterisicsRow> parretoList = new List<EducationLineAndCharacterisicsRow>();

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
    }
}
