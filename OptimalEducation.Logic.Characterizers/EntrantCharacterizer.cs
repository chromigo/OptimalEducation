using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.CQRS;
using OptimalEducation.DAL.Models;
using OptimalEducation.DAL.Queries;
using OptimalEducation.Interfaces.Logic.Characterizers;

namespace OptimalEducation.Implementation.Logic.Characterizers
{
    public class EntrantCharacterizer : ICharacterizer<Entrant>
    {
        private readonly ISummator<Entrant> _entrantSummator;
        private readonly IIdealResult _idealResult;
        private readonly IEducationCharacteristicNamesHelper _namesHelper;

        public EntrantCharacterizer(IEducationCharacteristicNamesHelper namesHelper, ISummator<Entrant> entrantSummator,
            IIdealResult idealResult)
        {
            _namesHelper = namesHelper;

            _entrantSummator = entrantSummator;
            _idealResult = idealResult;
        }

        public async Task<Dictionary<string, double>> Calculate(Entrant subject, bool isComplicatedMode = true)
        {
            Dictionary<string, double> sum;
            Dictionary<string, double> idealResult;

            var totalCharacteristics = new Dictionary<string, double>();
            foreach (var name in _namesHelper.Names)
            {
                totalCharacteristics.Add(name, 0);
            }

            //Здесь выбираем метод которым формируем результат
            if (isComplicatedMode)
            {
                sum = _entrantSummator.CalculateComplicatedSum(subject);
                idealResult = await _idealResult.GetComplicatedResult();
            }
            else
            {
                sum = _entrantSummator.CalculateSimpleSum(subject);
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

    public class EntrantSummator : ISummator<Entrant>
    {
        private readonly IEducationCharacteristicNamesHelper _namesHelper;

        public EntrantSummator(IEducationCharacteristicNamesHelper namesHelper)
        {
            _namesHelper = namesHelper;
        }

        #region Построение словарей с характеристиками

        private Dictionary<string, double> Characterising(
            Action<Dictionary<string, List<double>>, Entrant> partSumsMethod, Entrant entrant)
        {
            var characteristicAddItems = new Dictionary<string, List<double>>();
            var resultCharacteristics = new Dictionary<string, double>();
            foreach (var name in _namesHelper.Names)
            {
                characteristicAddItems.Add(name, new List<double>());
                resultCharacteristics.Add(name, 0);
            }

            //Т.к. олимпиад может быть очень много, результаты будет складывать в отдельный список(по хар-кам).
            partSumsMethod(characteristicAddItems, entrant);

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
                    b = b/4;
                }

                resultCharacteristics[item.Key] = sum;
            }
            return resultCharacteristics;
        }

        private void CreateUnatedStateExamPartSums(Dictionary<string, List<double>> unatedStateExamCharactericAddItems,
            Entrant entrant)
        {
            //Вычисляем пары "название кластера"+"спискок частичных сумм"
            foreach (var exam in entrant.UnitedStateExams)
            {
                if (exam.Result.HasValue)
                {
                    var result = exam.Result.Value/100.0; //нормализованный результат(100б=1.00, 70,=0.7)
                    var discipline = exam.Discipline;
                    foreach (var weight in discipline.Weights)
                    {
                        var coeff = weight.Coefficient;
                        var characteristicName = weight.Characterisic.Name;

                        var characteristicResult = result*coeff;
                        unatedStateExamCharactericAddItems[characteristicName].Add(characteristicResult);
                    }
                }
            }
        }

        private void CreateSchoolMarkPartSums(Dictionary<string, List<double>> schoolMarkCharactericAddItems,
            Entrant entrant)
        {
            foreach (var shoolMark in entrant.SchoolMarks)
            {
                if (shoolMark.Result.HasValue)
                {
                    //нормализованный результат(5=1.00)
                    var result = shoolMark.Result.Value/5.0;
                    var discipline = shoolMark.SchoolDiscipline;
                    foreach (var weight in discipline.Weights)
                    {
                        var coeff = weight.Coefficient;
                        var characteristicName = weight.Characterisic.Name;

                        var characteristicResult = result*coeff;
                        schoolMarkCharactericAddItems[characteristicName].Add(characteristicResult);
                    }
                }
            }
        }

        private void CreateOlympiadPartSums(Dictionary<string, List<double>> olympiadCharacteristicAddItems,
            Entrant entrant)
        {
            foreach (var olympResult in entrant.ParticipationInOlympiads)
            {
                var result = olympResult.Result;
                var olympiad = olympResult.Olympiad;
                foreach (var weight in olympiad.Weights)
                {
                    var coeff = weight.Coefficient;
                    var characteristicName = weight.Characterisic.Name;

                    double characteristicResult = 0;
                    switch (result)
                    {
                        case OlypmpiadResult.FirstPlace:
                            characteristicResult = ((double) result/100)*coeff;
                            break;
                        case OlypmpiadResult.SecondPlace:
                            characteristicResult = ((double) result/100)*coeff;
                            break;
                        case OlypmpiadResult.ThirdPlace:
                            characteristicResult = ((double) result/100)*coeff;
                            break;
                    }

                    olympiadCharacteristicAddItems[characteristicName].Add(characteristicResult);
                }
            }
        }

        private void CreateSectionPartSums(Dictionary<string, List<double>> sectionCharacteristicAddItems,
            Entrant entrant)
        {
            foreach (var sectionResult in entrant.ParticipationInSections)
            {
                double result = 0;
                //По правилу 80/20?
                if (sectionResult.YearPeriod >= 10) result = 1.00;
                else if (sectionResult.YearPeriod > 5) result = 0.90;
                else if (sectionResult.YearPeriod > 2) result = 0.80;
                else if (sectionResult.YearPeriod > 1) result = 0.40;
                else if (sectionResult.YearPeriod > 0.5) result = 0.20;

                var section = sectionResult.Section;
                foreach (var weight in section.Weights)
                {
                    var coeff = weight.Coefficient;
                    var characteristicName = weight.Characterisic.Name;

                    //TODO: Реализовать особую логику учета данных?
                    var characteristicResult = result*coeff;

                    sectionCharacteristicAddItems[characteristicName].Add(characteristicResult);
                }
            }
        }

        private void CreateHobbiePartSums(Dictionary<string, List<double>> hobbieCharacteristicAddItems, Entrant entrant)
        {
            foreach (var hobbieResult in entrant.Hobbies)
            {
                foreach (var weight in hobbieResult.Weights)
                {
                    var coeff = weight.Coefficient;
                    var characteristicName = weight.Characterisic.Name;

                    //TODO: Реализовать особую логику учета данных?
                    //пока просто учитывается наличие хобби как факт
                    double someValue = 1;
                    var characteristicResult = someValue*coeff;

                    hobbieCharacteristicAddItems[characteristicName].Add(characteristicResult);
                }
            }
        }

        private void CreateSchoolTypePartSums(Dictionary<string, List<double>> schoolTypeCharacteristicAddItems,
            Entrant entrant)
        {
            //для простоты будем брать последнюю школу, где абитуриент учился
            //(возможно стоит рассмотреть более сложный вариант в будущем)
            var lastParticipationInSchool = entrant.ParticipationInSchools.LastOrDefault();
            if (lastParticipationInSchool != null)
            {
                //Или еще учитывать кол-во лет обучения?
                var quality = lastParticipationInSchool.School.EducationQuality/100.0;
                var schoolWeights = lastParticipationInSchool.School.Weights;
                foreach (var weight in schoolWeights)
                {
                    var coeff = weight.Coefficient;
                    var characteristicName = weight.Characterisic.Name;

                    //TODO: Реализовать особую логику учета данных?
                    var characteristicResult = quality*coeff;

                    schoolTypeCharacteristicAddItems[characteristicName].Add(characteristicResult);
                }
            }
        }

        #endregion

        #region 2 Метода + вспомогаетльные для вычисления результата

        /// <summary>
        ///     Вычесленный результат по правилу: простое сложение частичных результатов
        ///     (по идее будет плохой результат, если, к примеру, не указаны олимпиады. Возможно такое поведение и нужно.)
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, double> CalculateSimpleSum(Entrant entrant)
        {
            var totalCharacteristics = new Dictionary<string, double>();
            foreach (var name in _namesHelper.Names)
            {
                totalCharacteristics.Add(name, 0);
            }

            //Вычисляем частичные характеристики
            //в результатер работы каждой функции получается новая таблица характеристик
            Dictionary<string, double> unatedStateExamCharacteristics;
            Dictionary<string, double> schoolMarkCharacteristics;
            Dictionary<string, double> olympiadCharacteristics;
            //etc dictionary


            if (true)
            {
                unatedStateExamCharacteristics = Characterising(CreateUnatedStateExamPartSums, entrant);
                foreach (var item in unatedStateExamCharacteristics)
                {
                    totalCharacteristics[item.Key] += item.Value;
                }
            }
            if (true)
            {
                schoolMarkCharacteristics = Characterising(CreateSchoolMarkPartSums, entrant);
                foreach (var item in schoolMarkCharacteristics)
                {
                    totalCharacteristics[item.Key] += item.Value;
                }
            }
            if (true)
            {
                olympiadCharacteristics = Characterising(CreateOlympiadPartSums, entrant);
                foreach (var item in olympiadCharacteristics)
                {
                    totalCharacteristics[item.Key] += item.Value;
                }
            }
            //Etc characterisitcs

            return totalCharacteristics;
        }

        public Dictionary<string, double> CalculateComplicatedSum(Entrant entrant)
        {
            var characteristicAddItems = new Dictionary<string, List<double>>();
            var resultCharacteristics = new Dictionary<string, double>();
            foreach (var name in _namesHelper.Names)
            {
                characteristicAddItems.Add(name, new List<double>());
                resultCharacteristics.Add(name, 0);
            }

            Dictionary<string, double> unatedStateExamCharacteristics;
            Dictionary<string, double> schoolMarkCharacteristics;
            Dictionary<string, double> olympiadCharacteristics;
            //etc dictionary

            //Вычисляем частичные характеристики
            //в результатер работы каждой функции получается новая таблица характеристик, которую мы добавляем в общий список
            if (true)
            {
                unatedStateExamCharacteristics = Characterising(CreateUnatedStateExamPartSums, entrant);
                foreach (var name in _namesHelper.Names)
                {
                    characteristicAddItems[name].Add(unatedStateExamCharacteristics[name]);
                }
            }
            if (true)
            {
                schoolMarkCharacteristics = Characterising(CreateSchoolMarkPartSums, entrant);
                foreach (var name in _namesHelper.Names)
                {
                    characteristicAddItems[name].Add(schoolMarkCharacteristics[name]);
                }
            }
            if (true)
            {
                olympiadCharacteristics = Characterising(CreateOlympiadPartSums, entrant);
                foreach (var name in _namesHelper.Names)
                {
                    characteristicAddItems[name].Add(olympiadCharacteristics[name]);
                }
            }
            //Etc characterisitcs

            //Cкладываем по аналогии с геом. прогрессией и делим на норм число
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
                    b = b/4;
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
    public class IdealEntrantResult : IIdealResult
    {
        //Для 2-го предположения(геом сложение+ нормир)
        private Dictionary<string, double> _complicatedResult;
        //Для 1-го предположения(простое сложение+ нормир)
        private Dictionary<string, double> _simpleResult;
        private readonly ISummator<Entrant> _entrantSummator;
        private readonly IQueryBuilder _queryBuilder;

        public IdealEntrantResult(ISummator<Entrant> entrantSummator, IQueryBuilder queryBuilder)
        {
            _entrantSummator = entrantSummator;
            _queryBuilder = queryBuilder;
        }

        public async Task<Dictionary<string, double>> GetSimpleResult()
        {
            if (_simpleResult == null)
            {
                var idealEntrant = await _queryBuilder
                    .For<Task<Entrant>>()
                    .With(new GetEntrantForCharacterizerCriterion {EntrantId = 2});

                _simpleResult = _entrantSummator.CalculateSimpleSum(idealEntrant);
            }

            return _simpleResult;
        }

        public async Task<Dictionary<string, double>> GetComplicatedResult()
        {
            if (_complicatedResult == null)
            {
                var idealEntrant = await _queryBuilder
                    .For<Task<Entrant>>()
                    .With(new GetEntrantForCharacterizerCriterion {EntrantId = 2});

                _complicatedResult = _entrantSummator.CalculateComplicatedSum(idealEntrant);
            }

            return _complicatedResult;
        }
    }
}