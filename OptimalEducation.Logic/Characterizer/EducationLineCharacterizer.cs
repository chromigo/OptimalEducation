using OptimalEducation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OptimalEducation.Logic.Characterizer
{
    /// <summary>
    /// Результаты характеристикиизации для учебного направления по методу уменьш коэф-в:
    /// Вычисляются значения для каждой характеристики(например, потенциал в математике, информатике, русском)
    /// Эти значения не суммируются сразу, а складываются в списки, соотносящиеся с данной характеристикой
    /// Позже эти значения сортируются и домножаются на убывающий коэффициент
    /// </summary>
    public class EducationLineCharacterizer
    {
        EducationLine _educationLine;
        List<string> educationCharacterisiticNames;

        Dictionary<string, double> result;
        public Dictionary<string, double> Result
        {
            get
            {
                if (result == null)
                {
                    //Здесь выбираем метод которым формируем результат
                    result = CalculateSimpleNormSum();
                    //Result = CalculateComplicatedNormSum();
                }
                return result;
            }
        }
        
        public EducationLineCharacterizer(EducationLine educationLine)
        {
            _educationLine = educationLine;
            InitCharacterisitcs();
        }
        void InitCharacterisitcs()
        {
            //Заполняем словарь всеми ключами по возможным весам
            OptimalEducationDbContext context = new OptimalEducationDbContext();
            educationCharacterisiticNames = context.Characteristics
                .Where(p => p.Type == CharacteristicType.Education)
                .Select(p => p.Name)
                .ToList();
        }
        #region Добавляем в списки слагаемые для сложения(для каждой характеристики)
        Dictionary<string, double> Characterising(Action<Dictionary<string, List<double>>> partSumsMethod)
        {
            var characteristicAddItems = new Dictionary<string, List<double>>();
            var resultCharacteristics = new Dictionary<string, double>();
            foreach (var name in educationCharacterisiticNames)
            {
                characteristicAddItems.Add(name, new List<double>());
                resultCharacteristics.Add(name, 0);
            }

            //Т.к. олимпиад может быть очень много, результаты будет складывать в отдельный список(по хар-кам).
            partSumsMethod(characteristicAddItems);

            //Логика суммирования
            foreach (var item in characteristicAddItems)
            {
                item.Value.Sort();
                var itemList = item.Value;
                //Используем идею геометрической прогрессии
                //Наибольший вклад вносит первое(наибольшее) значение. 
                //чем больше результатов, тем меньше их вклад
                //Пример: 4,3,3,2: 4*1+3/2 + 3/4 +2/8
                double b = 1;
                double sum = 0;
                for (int i = 0; i < itemList.Count; i++)
                {
                    sum += itemList[i] * b;
                    b = b / 2;
                }

                resultCharacteristics[item.Key] = sum;
            }
            return resultCharacteristics;
        }

        void CreateUnatedStateExamPartSums(Dictionary<string, List<double>> unatedStateExamCharactericAddItems)
        {
            foreach (var requirement in _educationLine.EducationLinesRequirements)
            {
                var result = requirement.Requirement / 100.0;
                var discipline = requirement.ExamDiscipline;
                foreach (var weight in discipline.Weights)
                {
                    var coeff = weight.Coefficient;
                    var characteristicName = weight.Characterisic.Name;

                    var characteristicResult = result * coeff;
                    unatedStateExamCharactericAddItems[characteristicName].Add(characteristicResult);
                }
            }
        }

        #endregion
        #region 2 Метода + вспомогаетльные для вычисления результата
        /// <summary>
        /// Вычесленный результат по правилу: простое сложение частичных результатов
        /// (по идее будет плохой результат, если, к примеру, не указаны олимпиады. Возможно такое поведение и нужно.)
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, double> CalculateSimpleSum()
        {
            //Вычисляем частичные характеристики
            //в результатер работы каждой функции получается новая таблица характеристик
            var unatedStateExamCharacteristics = Characterising(CreateUnatedStateExamPartSums);
            //TODO: Остальные методы(по остальным характеристикам)


            var totalCharacteristics = new Dictionary<string, double>();
            foreach (var name in educationCharacterisiticNames)
            {
                totalCharacteristics.Add(name, 0);
            }
            //Просто складываем и делим на норм. число
            //(Или складываем по аналогии с геом. прогрессией и делим на норм число?)
            foreach (var item in unatedStateExamCharacteristics)
            {
                totalCharacteristics[item.Key] += item.Value;
            }
            //TODO: Сложение по остальным характеристикам

            return totalCharacteristics;
        }
        Dictionary<string, double> CalculateSimpleNormSum()
        {
            var sum = CalculateSimpleSum();
            var idealResult = IdealEducationLineResult.SimpleResult;

            var totalCharacteristics = new Dictionary<string, double>();
            foreach (var name in educationCharacterisiticNames)
            {
                totalCharacteristics.Add(name, 0);
            }

            //Нормируем
            foreach (var item in sum)
            {
                totalCharacteristics[item.Key] = item.Value / idealResult[item.Key];
            }
            return totalCharacteristics;
        }

        public Dictionary<string, double> CalculateComplicatedSum()
        {
            //Вычисляем частичные характеристики
            //в результатер работы каждой функции получается новая таблица характеристик
            var unatedStateExamCharacteristics = Characterising(CreateUnatedStateExamPartSums);
            //TODO: Остальные методы(по остальным характеристикам)

            //Cкладываем по аналогии с геом. прогрессией и делим на норм число
            var characteristicAddItems = new Dictionary<string, List<double>>();
            var resultCharacteristics = new Dictionary<string, double>();
            foreach (var name in educationCharacterisiticNames)
            {
                characteristicAddItems.Add(name, new List<double>()
                    {
                        unatedStateExamCharacteristics[name],
                        //etc1[name],
                        //etc2[name]
                    });
                resultCharacteristics.Add(name, 0);
            }
            foreach (var item in characteristicAddItems)
            {
                item.Value.Sort();
                var itemList = item.Value;
                double b = 1;
                double sum = 0;
                for (int i = 0; i < itemList.Count; i++)
                {
                    sum += itemList[i] * b;
                    b = b / 2;
                }

                resultCharacteristics[item.Key] = sum;
            }

            return resultCharacteristics;
        }
        Dictionary<string, double> CalculateComplicatedNormSum()
        {
            var sum = CalculateComplicatedSum();
            var idealResult = IdealEducationLineResult.ComplicatedResult;

            var totalCharacteristics = new Dictionary<string, double>();
            foreach (var name in educationCharacterisiticNames)
            {
                totalCharacteristics.Add(name, 0);
            }

            //Нормируем
            foreach (var item in sum)
            {
                totalCharacteristics[item.Key] = item.Value / idealResult[item.Key];
            }
            return totalCharacteristics;
        } 
        #endregion
    }
    /// <summary>
    /// Статичный класс для вычислений 1 раз и получения в дальнейшем идеального результата(для абитуриента).
    /// Используется при нормировании результата.
    /// </summary>
    public static class IdealEducationLineResult
    {
        //Для 1-го предположения(простое сложение+ нормир)
        static Dictionary<string, double> simpleResult;
        public static Dictionary<string, double> SimpleResult
        {
            get
            {
                if (simpleResult == null)
                {
                    var context = new OptimalEducationDbContext();
                    var idealEducationLine = context.EducationLines.Where(p=>p.Name=="IDEAL").Single();
                    var characterizer = new EducationLineCharacterizer(idealEducationLine);
                    simpleResult = characterizer.CalculateSimpleSum();
                }
                return simpleResult;
            }
        }
        //Для 2-го предположения(геом сложение+ нормир)
        static Dictionary<string, double> complicatedResult;
        public static Dictionary<string, double> ComplicatedResult
        {
            get
            {
                if (complicatedResult == null)
                {
                    var context = new OptimalEducationDbContext();
                    var idealEducationLine = context.EducationLines.Where(p => p.Name == "IDEAL").Single();
                    var characterizer = new EducationLineCharacterizer(idealEducationLine);
                    complicatedResult = characterizer.CalculateComplicatedSum();
                }
                return complicatedResult;
            }
        }
    }
}