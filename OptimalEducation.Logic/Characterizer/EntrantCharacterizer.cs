using OptimalEducation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Logic.Characterizer
{
    public class EntrantCharacterizer
    {
        EntrantSummator entrantSummator;
        IdealEntrantResult idealEntrantResult;
        List<string> educationCharacterisiticNames;

        public EntrantCharacterizer(Entrant entrant, EntrantCalculationOptions options)
        {
            //характеристики для нашего направления
            entrantSummator = new EntrantSummator(entrant, options);
            idealEntrantResult = new IdealEntrantResult(options);

            OptimalEducationDbContext context = new OptimalEducationDbContext();
            educationCharacterisiticNames = context.Characteristics
                .Where(p => p.Type == CharacteristicType.Education)
                .Select(p => p.Name)
                .ToList();
        }

        public Dictionary<string, double> CalculateNormSum(bool isComlicatedMode = false)
        {
            Dictionary<string, double> sum;
            Dictionary<string, double> idealResult;

            //Здесь выбираем метод которым формируем результат
            if (isComlicatedMode)
            {
                sum = entrantSummator.CalculateComplicatedSum();
                idealResult = idealEntrantResult.ComplicatedResult;
            }
            else
            {
                sum = entrantSummator.CalculateSimpleSum();
                idealResult = idealEntrantResult.SimpleResult;
            }

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
    }

    public class EntrantSummator
    {
        Entrant _entrant;
        EntrantCalculationOptions _options;
        List<string> educationCharacterisiticNames;

        public EntrantSummator(Entrant entrant, EntrantCalculationOptions options)
        {
            _entrant = entrant;
            _options = options;
            InitCharacterisitcs();
        }
        private void InitCharacterisitcs()
        {
            //Заполняем словарь всеми ключами по возможным весам
            OptimalEducationDbContext context = new OptimalEducationDbContext();
            educationCharacterisiticNames = context.Characteristics
                .Where(p => p.Type == CharacteristicType.Education)
                .Select(p => p.Name)
                .ToList();
        }

        #region Построение словарей с характеристиками
        private Dictionary<string, double> Characterising(Action<Dictionary<string, List<double>>> partSumsMethod)
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

        private void CreateUnatedStateExamPartSums(Dictionary<string, List<double>> unatedStateExamCharactericAddItems)
        {
            //Вычисляем пары "название кластера"+"спискок частичных сумм"
            foreach (var exam in _entrant.UnitedStateExams)
            {
                if (exam.Result.HasValue)
                {
                    double result = exam.Result.Value / 100.0;//нормализованный результат(100б=1.00, 70,=0.7)
                    var discipline = exam.Discipline;
                    foreach (var weight in discipline.Weights)
                    {
                        var coeff = weight.Coefficient;
                        var characteristicName = weight.Characterisic.Name;

                        var characteristicResult = result * coeff;
                        unatedStateExamCharactericAddItems[characteristicName].Add(characteristicResult);
                    }
                }
            }
        }

        private void CreateSchoolMarkPartSums(Dictionary<string, List<double>> schoolMarkCharactericAddItems)
        {
            foreach (var shoolMark in _entrant.SchoolMarks)
            {
                if (shoolMark.Result.HasValue)
                {
                    //нормализованный результат(5=1.00)
                    double result = shoolMark.Result.Value / 5.0;
                    var discipline = shoolMark.SchoolDiscipline;
                    foreach (var weight in discipline.Weights)
                    {
                        var coeff = weight.Coefficient;
                        var characteristicName = weight.Characterisic.Name;

                        var characteristicResult = result * coeff;
                        schoolMarkCharactericAddItems[characteristicName].Add(characteristicResult);
                    }
                }
            }
        }

        private void CreateOlympiadPartSums(Dictionary<string, List<double>> olympiadCharacteristicAddItems)
        {
            foreach (var olympResult in _entrant.ParticipationInOlympiads)
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
                        case OlypmpiadResult.FirstPlace: characteristicResult = ((double)result / 100) * coeff;
                            break;
                        case OlypmpiadResult.SecondPlace: characteristicResult = ((double)result / 100) * coeff;
                            break;
                        case OlypmpiadResult.ThirdPlace: characteristicResult = ((double)result / 100) * coeff;
                            break;
                    }

                    olympiadCharacteristicAddItems[characteristicName].Add(characteristicResult);
                }
            }
        }

        private void CreateSectionPartSums(Dictionary<string, List<double>> sectionCharacteristicAddItems)
        {
            foreach (var sectionResult in _entrant.ParticipationInSections)
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
                    double characteristicResult = result * coeff;

                    sectionCharacteristicAddItems[characteristicName].Add(characteristicResult);
                }
            }
        }

        private void CreateHobbiePartSums(Dictionary<string, List<double>> hobbieCharacteristicAddItems)
        {
            foreach (var hobbieResult in _entrant.Hobbies)
            {
                foreach (var weight in hobbieResult.Weights)
                {
                    var coeff = weight.Coefficient;
                    var characteristicName = weight.Characterisic.Name;

                    //TODO: Реализовать особую логику учета данных?
                    //пока просто учитывается наличие хобби как факт
                    double someValue = 1;
                    double characteristicResult = someValue * coeff;

                    hobbieCharacteristicAddItems[characteristicName].Add(characteristicResult);
                }
            }
        }

        private void CreateSchoolTypePartSums(Dictionary<string, List<double>> schoolTypeCharacteristicAddItems)
        {
            //для простоты будем брать последнюю школу, где абитуриент учился
            //(возможно стоит рассмотреть более сложный вариант в будущем)
            var lastParticipationInSchool = _entrant.ParticipationInSchools.LastOrDefault();
            if (lastParticipationInSchool != null)
            {
                //Или еще учитывать кол-во лет обучения?
                var quality = lastParticipationInSchool.School.EducationQuality / 100.0;
                var schoolWeights = lastParticipationInSchool.School.Weights;
                foreach (var weight in schoolWeights)
                {
                    var coeff = weight.Coefficient;
                    var characteristicName = weight.Characterisic.Name;

                    //TODO: Реализовать особую логику учета данных?
                    double characteristicResult = quality * coeff;

                    schoolTypeCharacteristicAddItems[characteristicName].Add(characteristicResult);
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
            var totalCharacteristics = new Dictionary<string, double>();
            foreach (var name in educationCharacterisiticNames)
            {
                totalCharacteristics.Add(name, 0);
            }

            //Вычисляем частичные характеристики
            //в результатер работы каждой функции получается новая таблица характеристик
            Dictionary<string, double> unatedStateExamCharacteristics;
            Dictionary<string, double> schoolMarkCharacteristics;
            Dictionary<string, double> olympiadCharacteristics;
            Dictionary<string, double> sectionCharacteristics;
            Dictionary<string, double> hobbieCharacteristics;
            Dictionary<string, double> schoolTypeCharacteristics;

            if (_options.IsCalculateUnateStateExam)
            {
                unatedStateExamCharacteristics = Characterising(CreateUnatedStateExamPartSums);
                foreach (var item in unatedStateExamCharacteristics)
                {
                    totalCharacteristics[item.Key] += item.Value;
                }
            }
            if (_options.IsCalculateSchoolMark)
            {
                schoolMarkCharacteristics = Characterising(CreateSchoolMarkPartSums);
                foreach (var item in schoolMarkCharacteristics)
                {
                    totalCharacteristics[item.Key] += item.Value;
                }
            }
            if (_options.IsCalculateOlympiad)
            {
                olympiadCharacteristics = Characterising(CreateOlympiadPartSums);
                foreach (var item in olympiadCharacteristics)
                {
                    totalCharacteristics[item.Key] += item.Value;
                }
            }
            //if (_options.IsCalculateSection)
            //{
            //    sectionCharacteristics = Characterising(CreateOlympiadPartSums);
            //    foreach (var item in sectionCharacteristics)
            //    {
            //        totalCharacteristics[item.Key] += item.Value;
            //    }
            //}
            //if (_options.IsCalculateHobbie)
            //{
            //    hobbieCharacteristics = Characterising(CreateOlympiadPartSums);
            //    foreach (var item in hobbieCharacteristics)
            //    {
            //        totalCharacteristics[item.Key] += item.Value;
            //    }
            //}
            //if (_options.IsCalculateSchoolMark)
            //{
            //    schoolTypeCharacteristics = Characterising(CreateOlympiadPartSums);
            //    foreach (var item in schoolTypeCharacteristics)
            //    {
            //        totalCharacteristics[item.Key] += item.Value;
            //    }
            //}

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
            Dictionary<string, double> schoolMarkCharacteristics;
            Dictionary<string, double> olympiadCharacteristics;
            Dictionary<string, double> sectionCharacteristics;
            Dictionary<string, double> hobbieCharacteristics;
            Dictionary<string, double> schoolTypeCharacteristics;
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
            if (_options.IsCalculateSchoolMark)
            {
                schoolMarkCharacteristics = Characterising(CreateSchoolMarkPartSums);
                foreach (var name in educationCharacterisiticNames)
                {
                    characteristicAddItems[name].Add(schoolMarkCharacteristics[name]);
                }
            }
            if (_options.IsCalculateOlympiad)
            {
                olympiadCharacteristics = Characterising(CreateOlympiadPartSums);
                foreach (var name in educationCharacterisiticNames)
                {
                    characteristicAddItems[name].Add(olympiadCharacteristics[name]);
                }
            }
            //if (_options.IsCalculateSection)
            //{
            //    sectionCharacteristics = Characterising(CreateOlympiadPartSums);
            //    foreach (var name in educationCharacterisiticNames)
            //    {
            //        characteristicAddItems[name].Add(sectionCharacteristics[name]);
            //    }
            //}
            //if (_options.IsCalculateHobbie)
            //{
            //    hobbieCharacteristics = Characterising(CreateOlympiadPartSums);
            //    foreach (var name in educationCharacterisiticNames)
            //    {
            //        characteristicAddItems[name].Add(hobbieCharacteristics[name]);
            //    }
            //}
            //if (_options.IsCalculateSchoolType)
            //{
            //    schoolTypeCharacteristics = Characterising(CreateOlympiadPartSums);
            //    foreach (var name in educationCharacterisiticNames)
            //    {
            //        characteristicAddItems[name].Add(schoolTypeCharacteristics[name]);
            //    }
            //}

            //Cкладываем по аналогии с геом. прогрессией и делим на норм число
            foreach (var item in characteristicAddItems)
            {
                var itemList = (from elem in  item.Value
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
    public class IdealEntrantResult
    {
        EntrantCalculationOptions _options;

        public IdealEntrantResult(EntrantCalculationOptions options)
        {
            _options = options;
        }

        //Для 1-го предположения(простое сложение+ нормир)
        Dictionary<string, double> simpleResult;
        public  Dictionary<string, double> SimpleResult
        {
            get 
            {
                if (simpleResult == null)
                {
                    var context = new OptimalEducationDbContext();
                    var idealEntrant = context.Entrants.Find(2);
                    var characterizer = new EntrantSummator(idealEntrant, _options);
                    simpleResult = characterizer.CalculateSimpleSum();
                }
                return simpleResult;
            }
        }
        //Для 2-го предположения(геом сложение+ нормир)
        Dictionary<string, double> complicatedResult;
        public  Dictionary<string, double> ComplicatedResult
        {
            get
            {
                if (complicatedResult == null)
                {
                    var context = new OptimalEducationDbContext();
                    var idealEntrant = context.Entrants.Find(2);
                    var characterizer = new EntrantSummator(idealEntrant, _options);
                    complicatedResult = characterizer.CalculateComplicatedSum();
                }
                return complicatedResult;
            }
        }
    }

    /// <summary>
    /// Насктройки, в которых указывается, какие данные пользователя учитывать.
    /// Стандартный конструкор без параметров - вычислять все.
    /// </summary>
    public class EntrantCalculationOptions
    {
        public bool IsCalculateUnateStateExam { get; private set; }
        public bool IsCalculateSchoolMark { get; private set; }
        public bool IsCalculateOlympiad { get; private set; }
        public bool IsCalculateSection { get; private set; }
        public bool IsCalculateHobbie { get; private set; }
        public bool IsCalculateSchoolType { get; private set; }

        public EntrantCalculationOptions(
            bool IsCalculateUnateStateExam,
            bool IsCalculateSchoolMark,
            bool IsCalculateOlympiad,
            bool IsCalculateSection,
            bool IsCalculateHobbie,
            bool IsCalculateSchoolType)
        {
            this.IsCalculateUnateStateExam = IsCalculateUnateStateExam;
            this.IsCalculateSchoolMark = IsCalculateSchoolMark;
            this.IsCalculateOlympiad = IsCalculateOlympiad;
            this.IsCalculateSection = IsCalculateSection;
            this.IsCalculateHobbie = IsCalculateHobbie;
            this.IsCalculateSchoolType = IsCalculateSchoolType;
        }

        public EntrantCalculationOptions()
        {
            this.IsCalculateUnateStateExam = true;
            this.IsCalculateSchoolMark = true;
            this.IsCalculateOlympiad = true;
            this.IsCalculateSection = true;
            this.IsCalculateHobbie = true;
            this.IsCalculateSchoolType = true;
        }
    }
}
