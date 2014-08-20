using OptimalEducation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;

namespace OptimalEducation.Logic.Characterizers
{
    public class EducationLineCharacterizer:ICharacterizer<EducationLine>
    {
        readonly EducationLineSummator _educationLineSummator;
        readonly List<string> _educationCharacterisiticNames;
        readonly IdealEducationLineResult _idealResult;

        public EducationLineCharacterizer()
        {            
            var context = new OptimalEducationDbContext();
            _educationCharacterisiticNames = context.Characteristics
                .Where(p => p.Type == CharacteristicType.Education)
                .Select(p => p.Name)
                .AsNoTracking()
                .ToList();

            //характеристики для нашего направления
            _educationLineSummator = new EducationLineSummator(_educationCharacterisiticNames);
            _idealResult=new IdealEducationLineResult(_educationCharacterisiticNames);
        }

        public Dictionary<string, double> Calculate(EducationLine subject,bool isComlicatedMode = true)
        {
            Dictionary<string, double> sum;
            Dictionary<string, double> idealResult;
            
            var totalCharacteristics = new Dictionary<string, double>();
            foreach (var name in _educationCharacterisiticNames)
            {
                totalCharacteristics.Add(name, 0);
            }
            
            //Здесь выбираем метод которым формируем результат
            if(isComlicatedMode)
            {
                sum = _educationLineSummator.CalculateComplicatedSum(subject);
                idealResult = _idealResult.GetComplicatedResult();
            }
            else
            {
                sum = _educationLineSummator.CalculateSimpleSum(subject);
                idealResult = _idealResult.GetSimpleResult();
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
        readonly List<string> _educationCharacterisiticNames;

        public EducationLineSummator(List<string> educationCharacterisiticNames)
        {
            _educationCharacterisiticNames = educationCharacterisiticNames;
        }

        #region Построение словарей с характеристиками
        Dictionary<string, double> Characterising(Action<Dictionary<string, List<double>>, EducationLine> partSumsMethod, EducationLine educationLine)
        {
            var characteristicAddItems = new Dictionary<string, List<double>>();
            var resultCharacteristics = new Dictionary<string, double>();
            foreach (var name in _educationCharacterisiticNames)
            {
                characteristicAddItems.Add(name, new List<double>());
                resultCharacteristics.Add(name, 0);
            }

            //Т.к. олимпиад может быть очень много, результаты будет складывать в отдельный список(по хар-кам).
            partSumsMethod(characteristicAddItems, educationLine);

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
        void CreateUnatedStateExamPartSums(Dictionary<string, List<double>> unatedStateExamCharactericAddItems, EducationLine educationLine)
        {
            //Временная заглушка: если встречаем требование не по егэ(доп исптыание, например письм. экзамен по математика), то пропускаем его
            var educationLineReq = educationLine.EducationLinesRequirements.Where(p => p.ExamDiscipline.ExamType == ExamType.UnitedStateExam);

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
        public Dictionary<string, double> CalculateSimpleSum(EducationLine educationLine)
        {
            var totalCharacteristics = new Dictionary<string, double>();
            foreach (var name in _educationCharacterisiticNames)
            {
                totalCharacteristics.Add(name, 0);
            }
            //Вычисляем частичные характеристики
            //в результатер работы каждой функции получается новая таблица характеристик
            Dictionary<string, double> unatedStateExamCharacteristics;
            //Etc

            if (true)
            {
                unatedStateExamCharacteristics = Characterising(CreateUnatedStateExamPartSums, educationLine);
                foreach (var item in unatedStateExamCharacteristics)
                {
                    totalCharacteristics[item.Key] += item.Value;
                }
            }
            //etc

            return totalCharacteristics;
        }
        public Dictionary<string, double> CalculateComplicatedSum(EducationLine educationLine)
        {
            var characteristicAddItems = new Dictionary<string, List<double>>();
            var resultCharacteristics = new Dictionary<string, double>();
            foreach (var name in _educationCharacterisiticNames)
            {
                characteristicAddItems.Add(name, new List<double>());
                resultCharacteristics.Add(name, 0);
            }

            Dictionary<string, double> unatedStateExamCharacteristics;
            //etc

            //Вычисляем частичные характеристики
            //в результатер работы каждой функции получается новая таблица характеристик, которую мы добавляем в общий список
            if (true)
            {
                unatedStateExamCharacteristics = Characterising(CreateUnatedStateExamPartSums, educationLine);
                foreach (var name in _educationCharacterisiticNames)
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
    public class IdealEducationLineResult
    {
        readonly List<string> _educationCharacterisiticNames;

        public IdealEducationLineResult(List<string> educationCharacterisiticNames)
        {
            _educationCharacterisiticNames = educationCharacterisiticNames;
        }

        //Для 1-го предположения(простое сложение+ нормир)
        public Dictionary<string, double> GetSimpleResult()
        {
            var context = new OptimalEducationDbContext();
            var idealEducationLine = context.EducationLines
                .Include(edl => edl.EducationLinesRequirements.Select(edlReq => edlReq.ExamDiscipline.Weights.Select(w => w.Characterisic)))
                .Where(p => p.Name == "IDEAL")
                .AsNoTracking()
                .Single();
            var characterizer = new EducationLineSummator(_educationCharacterisiticNames);
            return characterizer.CalculateSimpleSum(idealEducationLine);
        }
        //Для 2-го предположения(геом сложение+ нормир)
        public Dictionary<string, double> GetComplicatedResult()
        {
            var context = new OptimalEducationDbContext();
            var idealEducationLine = context.EducationLines
                .Include(edl => edl.EducationLinesRequirements.Select(edlReq => edlReq.ExamDiscipline.Weights.Select(w => w.Characterisic)))
                .Where(p => p.Name == "IDEAL")
                .AsNoTracking()
                .Single();
            var characterizer = new EducationLineSummator(_educationCharacterisiticNames);

            return characterizer.CalculateComplicatedSum(idealEducationLine);
        } 
    }
}