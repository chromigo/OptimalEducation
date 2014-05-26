using OptimalEducation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;

namespace OptimalEducation.Logic.Characterizer
{
    public class EducationLineCharacterizer
    {
        EducationLineSummator educationLineCharacterizer;
        static List<string> educationCharacterisiticNames;
        Dictionary<string, double> totalCharacteristics;

        public EducationLineCharacterizer(EducationLine educationLine, EducationLineCalculationOptions options)
        {            
            if(educationCharacterisiticNames==null)
            {
                OptimalEducationDbContext context = new OptimalEducationDbContext();
                educationCharacterisiticNames = context.Characteristics
                    .Where(p => p.Type == CharacteristicType.Education)
                    .Select(p => p.Name)
                    .ToList();
            }

            totalCharacteristics = new Dictionary<string, double>();
            foreach (var name in educationCharacterisiticNames)
            {
                totalCharacteristics.Add(name, 0);
            }

            //характеристики для нашего направления
            educationLineCharacterizer = new EducationLineSummator(educationLine, options, educationCharacterisiticNames);
            IdealEducationLineResult.SetUpSettings(options, educationCharacterisiticNames);
        }

        public Dictionary<string, double> CalculateNormSum(bool isComlicatedMode=false)
        {
            Dictionary<string, double> sum;
            Dictionary<string, double> idealResult;

            //Здесь выбираем метод которым формируем результат
            if(isComlicatedMode)
            {
                sum = educationLineCharacterizer.CalculateComplicatedSum();
                idealResult = IdealEducationLineResult.GetComplicatedResult();
            }
            else
            {
                sum = educationLineCharacterizer.CalculateSimpleSum();
                idealResult = IdealEducationLineResult.GetSimpleResult();
            }

            //Нормируем
            foreach (var item in sum)
            {
                totalCharacteristics[item.Key] = item.Value / idealResult[item.Key];
            }
            return totalCharacteristics;
        }
    }
    public class EducationLineSummator
    {
        EducationLine _educationLine;
        EducationLineCalculationOptions _options;
        List<string> educationCharacterisiticNames;

        public EducationLineSummator(EducationLine educationLine, EducationLineCalculationOptions options, List<string> educationCharacterisiticNames)
        {
            _educationLine = educationLine;
            _options = options;
            this.educationCharacterisiticNames = educationCharacterisiticNames;
        }

        #region Построение словарей с характеристиками
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
                var itemList = (from elem in item.Value
                                orderby elem descending
                                select elem).ToList();
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
            //Временная заглушка: если встречаем требование не по егэ(доп исптыание, например письм. экзамен по математика), то пропускаем его
            var educationLineReq = _educationLine.EducationLinesRequirements.Where(p => p.ExamDiscipline.ExamType == ExamType.UnitedStateExam);

            var test=_educationLine.EducationLinesRequirements.Where(p=>p.ExamDiscipline.ExamType!=ExamType.UnitedStateExam);
            var t = 1;
            if (test.Count() > 0)
                t = 2;
            foreach (var requirement in educationLineReq)
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
        #region 2 метода для вычисления сумм
        /// <summary>
        /// Вычесленный результат по правилу: простое сложение частичных результатов
        /// (по идее будет плохой результат, если, к примеру, не указаны олимпиады. Возможно такое поведение и нужно.)
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, double> CalculateSimpleSum()
        {
            var totalCharacteristics = new Dictionary<string, double>();
            foreach (var name in educationCharacterisiticNames)
            {
                totalCharacteristics.Add(name, 0);
            }
            //Вычисляем частичные характеристики
            //в результатер работы каждой функции получается новая таблица характеристик
            Dictionary<string, double> unatedStateExamCharacteristics;
            //Etc

            if (_options.IsCalculateUnateStateExam)
            {
                unatedStateExamCharacteristics = Characterising(CreateUnatedStateExamPartSums);
                foreach (var item in unatedStateExamCharacteristics)
                {
                    totalCharacteristics[item.Key] += item.Value;
                }
            }
            //etc

            return totalCharacteristics;
        }
        public Dictionary<string, double> CalculateComplicatedSum()
        {
            var characteristicAddItems = new Dictionary<string, List<double>>();
            var resultCharacteristics = new Dictionary<string, double>();
            foreach (var name in educationCharacterisiticNames)
            {
                characteristicAddItems.Add(name, new List<double>());
                resultCharacteristics.Add(name, 0);
            }

            Dictionary<string, double> unatedStateExamCharacteristics;
            //etc

            //Вычисляем частичные характеристики
            //в результатер работы каждой функции получается новая таблица характеристик, которую мы добавляем в общий список
            if (_options.IsCalculateUnateStateExam)
            {
                unatedStateExamCharacteristics = Characterising(CreateUnatedStateExamPartSums);
                foreach (var name in educationCharacterisiticNames)
                {
                    characteristicAddItems[name].Add(unatedStateExamCharacteristics[name]);
                }
            }
            //etc

            foreach (var item in characteristicAddItems)
            {
                var itemList = (from elem in item.Value
                                orderby elem descending
                                select elem).ToList();
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

        #endregion
    }
    /// <summary>
    /// Класс для вычислений идеального результата.
    /// Используется при нормировании результата.
    /// </summary>
    public static class IdealEducationLineResult
    {
        static EducationLineCalculationOptions _options;
        static List<string> _educationCharacterisiticNames;
        static bool isNewOptions;
        public static void SetUpSettings(EducationLineCalculationOptions options, List<string> educationCharacterisiticNames)
        {
            if (_options == null)
                _options = options;
            else if(_options.IsCalculateUnateStateExam!=options.IsCalculateUnateStateExam)
            {
                _options = options;
                isNewOptions = true;
            }

            _educationCharacterisiticNames = educationCharacterisiticNames;
        }

        //Для 1-го предположения(простое сложение+ нормир)
        static Dictionary<string, double> simpleResult;
        public static Dictionary<string, double> GetSimpleResult()
        {
            if (simpleResult == null||isNewOptions)
            {
                var context = new OptimalEducationDbContext();
                var idealEducationLine = context.EducationLines
                    .Include(edl => edl.EducationLinesRequirements.Select(edlReq => edlReq.ExamDiscipline.Weights.Select(w => w.Characterisic)))
                    .Where(p => p.Name == "IDEAL")
                    .Single();
                var characterizer = new EducationLineSummator(idealEducationLine, _options, _educationCharacterisiticNames);
                simpleResult = characterizer.CalculateSimpleSum();
                isNewOptions = false;
            }
            return simpleResult;
        }
        //Для 2-го предположения(геом сложение+ нормир)
        static Dictionary<string, double> complicatedResult;
        public static Dictionary<string, double> GetComplicatedResult()
        {
            if (complicatedResult == null|| isNewOptions)
            {
                var context = new OptimalEducationDbContext();
                var idealEducationLine = context.EducationLines
                    .Include(edl => edl.EducationLinesRequirements.Select(edlReq => edlReq.ExamDiscipline.Weights.Select(w => w.Characterisic)))
                    .Where(p => p.Name == "IDEAL")
                    .Single();
                var characterizer = new EducationLineSummator(idealEducationLine, _options, _educationCharacterisiticNames);
                complicatedResult = characterizer.CalculateComplicatedSum();
                isNewOptions = false;
            }
            return complicatedResult;
        } 
    }

    /// <summary>
    /// Насктройки, в которых указывается, какие данные учебного направления учитывать.
    /// Стандартный конструкор без параметров - вычислять все.
    /// </summary>
    public class EducationLineCalculationOptions
    {
        public bool IsCalculateUnateStateExam { get; private set; }
        //Etc

        public EducationLineCalculationOptions(
            bool IsCalculateUnateStateExam
            //etc
            )
        {
            this.IsCalculateUnateStateExam = IsCalculateUnateStateExam;
            //etc
        }

        public EducationLineCalculationOptions()
        {
            this.IsCalculateUnateStateExam = true;
            //etc
        }
    }
}