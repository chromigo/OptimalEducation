﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.CQRS;
using OptimalEducation.DAL.Models;
using OptimalEducation.DAL.Queries;
using OptimalEducation.Interfaces.Logic.Characterizers;

namespace OptimalEducation.Implementation.Logic.Characterizers
{
    public class EducationLineCharacterizer : ICharacterizer<EducationLine>
    {
        private readonly ISummator<EducationLine> _educationLineSummator;
        private readonly IIdealResult _idealResult;
        private readonly IEducationCharacteristicNamesHelper _namesHelper;

        public EducationLineCharacterizer(IEducationCharacteristicNamesHelper namesHelper,
            ISummator<EducationLine> educationLineSummator, IIdealResult idealResult)
        {
            _namesHelper = namesHelper;

            _educationLineSummator = educationLineSummator;
            _idealResult = idealResult;
        }

        public async Task<Dictionary<string, double>> Calculate(EducationLine subject, bool isComlicatedMode = true)
        {
            Dictionary<string, double> sum;
            Dictionary<string, double> idealResult;

            var totalCharacteristics = _namesHelper.Names.ToDictionary<string, string, double>(name => name, name => 0);

            //Здесь выбираем метод которым формируем результат
            if (isComlicatedMode)
            {
                sum = _educationLineSummator.CalculateComplicatedSum(subject);
                idealResult = await _idealResult.GetComplicatedResult();
            }
            else
            {
                sum = _educationLineSummator.CalculateSimpleSum(subject);
                idealResult = await _idealResult.GetSimpleResult();
            }

            //Нормируем
            foreach (var item in sum)
            {
                totalCharacteristics[item.Key] = item.Value/idealResult[item.Key];
            }
            return totalCharacteristics;
        }
    }

    public class EducationLineSummator : ISummator<EducationLine>
    {
        private readonly IEducationCharacteristicNamesHelper _namesHelper;

        public EducationLineSummator(IEducationCharacteristicNamesHelper namesHelper)
        {
            _namesHelper = namesHelper;
        }

        #region Построение словарей с характеристиками

        private Dictionary<string, double> Characterising(
            Action<Dictionary<string, List<double>>, EducationLine> partSumsMethod, EducationLine educationLine)
        {
            var characteristicAddItems = new Dictionary<string, List<double>>();
            var resultCharacteristics = new Dictionary<string, double>();
            foreach (var name in _namesHelper.Names)
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
                for (var i = 0; i < itemList.Count; i++)
                {
                    sum += itemList[i]*b;
                    b = b/2;
                }

                resultCharacteristics[item.Key] = sum;
            }
            return resultCharacteristics;
        }

        private void CreateUnatedStateExamPartSums(Dictionary<string, List<double>> unatedStateExamCharactericAddItems,
            EducationLine educationLine)
        {
            //Временная заглушка: если встречаем требование не по егэ(доп исптыание, например письм. экзамен по математика), то пропускаем его
            var educationLineReq =
                educationLine.EducationLinesRequirements.Where(
                    p => p.ExamDiscipline.ExamType == ExamType.UnitedStateExam);

            foreach (var requirement in educationLineReq)
            {
                var result = requirement.Requirement/100.0;
                var discipline = requirement.ExamDiscipline;
                foreach (var weight in discipline.Weights)
                {
                    var coeff = weight.Coefficient;
                    var characteristicName = weight.Characterisic.Name;

                    var characteristicResult = result*coeff;
                    unatedStateExamCharactericAddItems[characteristicName].Add(characteristicResult);
                }
            }
        }

        #endregion

        #region 2 метода для вычисления сумм

        /// <summary>
        ///     Вычесленный результат по правилу: простое сложение частичных результатов
        ///     (по идее будет плохой результат, если, к примеру, не указаны олимпиады. Возможно такое поведение и нужно.)
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, double> CalculateSimpleSum(EducationLine educationLine)
        {
            var totalCharacteristics = new Dictionary<string, double>();
            foreach (var name in _namesHelper.Names)
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
            foreach (var name in _namesHelper.Names)
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
                foreach (var name in _namesHelper.Names)
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
                for (var i = 0; i < itemList.Count; i++)
                {
                    sum += itemList[i]*b;
                    b = b/2;
                }

                resultCharacteristics[item.Key] = sum;
            }

            return resultCharacteristics;
        }

        #endregion
    }

    /// <summary>
    ///     Класс для вычислений идеального результата.
    ///     Используется при нормировании результата.
    /// </summary>
    public class IdealEducationLineResult : IIdealResult
    {
        //Для 2-го предположения(геом сложение+ нормир)
        private Dictionary<string, double> _complicatedResult;
        //Для 1-го предположения(простое сложение+ нормир)
        private Dictionary<string, double> _simpleResult;
        private readonly IQueryBuilder _queryBuilder;
        private readonly ISummator<EducationLine> _summator;

        public IdealEducationLineResult(ISummator<EducationLine> summator, IQueryBuilder queryBuilder)
        {
            _summator = summator;
            _queryBuilder = queryBuilder;
        }

        public async Task<Dictionary<string, double>> GetSimpleResult()
        {
            if (_simpleResult == null)
            {
                var idealEducationLine = await _queryBuilder
                    .For<Task<EducationLine>>()
                    .With(new GetIdelaEducationLineForCharacterizerCriterion());

                _simpleResult = _summator.CalculateSimpleSum(idealEducationLine);
            }
            return _simpleResult;
        }

        public async Task<Dictionary<string, double>> GetComplicatedResult()
        {
            if (_complicatedResult == null)
            {
                var idealEducationLine = await _queryBuilder
                    .For<Task<EducationLine>>()
                    .With(new GetIdelaEducationLineForCharacterizerCriterion());

                _complicatedResult = _summator.CalculateComplicatedSum(idealEducationLine);
            }
            return _complicatedResult;
        }
    }
}