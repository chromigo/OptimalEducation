using OptimalEducation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Logic.Characterizers
{
    public class EntrantCharacterizer : ICharacterizer<Entrant>
    {
        readonly EntrantSummator _entrantSummator;
        readonly List<string> _educationCharacterisiticNames;
        readonly IdealEntrantResult _idealResult;

        public EntrantCharacterizer()
        {
            var context = new OptimalEducationDbContext();
            _educationCharacterisiticNames = context.Characteristics
                .Where(p => p.Type == CharacteristicType.Education)
                .Select(p => p.Name)
                .AsNoTracking()
                .ToList();

            //характеристики для нашего направления
            _entrantSummator = new EntrantSummator(_educationCharacterisiticNames);
            _idealResult=new IdealEntrantResult(_educationCharacterisiticNames);
        }

        public Dictionary<string, double> Calculate(Entrant subject, bool isComplicatedMode = true)
        {
            Dictionary<string, double> sum;
            Dictionary<string, double> idealResult;

            var totalCharacteristics = new Dictionary<string, double>();
            foreach (var name in _educationCharacterisiticNames)
            {
                totalCharacteristics.Add(name, 0);
            }

            //Здесь выбираем метод которым формируем результат
            if (isComplicatedMode)
            {
                sum = _entrantSummator.CalculateComplicatedSum(subject);
                idealResult = _idealResult.GetComplicatedResult();
            }
            else
            {
                sum = _entrantSummator.CalculateSimpleSum(subject);
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

    public class EntrantSummator
    {
        readonly List<string> _educationCharacterisiticNames;

        public EntrantSummator(List<string> educationCharacterisiticNames)
        {
            _educationCharacterisiticNames = educationCharacterisiticNames;
        }

        #region Построение словарей с характеристиками
        private Dictionary<string, double> Characterising(Action<Dictionary<string, List<double>>, Entrant> partSumsMethod, Entrant entrant)
        {
            var characteristicAddItems = new Dictionary<string, List<double>>();
            var resultCharacteristics = new Dictionary<string, double>();
            foreach (var name in _educationCharacterisiticNames)
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
                for (int i = 0; i < itemList.Count; i++)
                {
                    sum += itemList[i] * b;
                    b = b / 4;
                }

                resultCharacteristics[item.Key] = sum;
            }
            return resultCharacteristics;
        }

        private void CreateUnatedStateExamPartSums(Dictionary<string, List<double>> unatedStateExamCharactericAddItems, Entrant entrant)
        {
            //Вычисляем пары "название кластера"+"спискок частичных сумм"
            foreach (var exam in entrant.UnitedStateExams)
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

        private void CreateSchoolMarkPartSums(Dictionary<string, List<double>> schoolMarkCharactericAddItems, Entrant entrant)
        {
            foreach (var shoolMark in entrant.SchoolMarks)
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

        private void CreateOlympiadPartSums(Dictionary<string, List<double>> olympiadCharacteristicAddItems, Entrant entrant)
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

        private void CreateSectionPartSums(Dictionary<string, List<double>> sectionCharacteristicAddItems, Entrant entrant)
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
                    double characteristicResult = result * coeff;

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
                    double characteristicResult = someValue * coeff;

                    hobbieCharacteristicAddItems[characteristicName].Add(characteristicResult);
                }
            }
        }

        private void CreateSchoolTypePartSums(Dictionary<string, List<double>> schoolTypeCharacteristicAddItems, Entrant entrant)
        {
            //для простоты будем брать последнюю школу, где абитуриент учился
            //(возможно стоит рассмотреть более сложный вариант в будущем)
            var lastParticipationInSchool = entrant.ParticipationInSchools.LastOrDefault();
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
        public Dictionary<string, double> CalculateSimpleSum(Entrant entrant)
        {
            var totalCharacteristics = new Dictionary<string, double>();
            foreach (var name in _educationCharacterisiticNames)
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
            foreach (var name in _educationCharacterisiticNames)
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
                foreach (var name in _educationCharacterisiticNames)
                {
                    characteristicAddItems[name].Add(unatedStateExamCharacteristics[name]);
                }
            }
            if (true)
            {
                schoolMarkCharacteristics = Characterising(CreateSchoolMarkPartSums, entrant);
                foreach (var name in _educationCharacterisiticNames)
                {
                    characteristicAddItems[name].Add(schoolMarkCharacteristics[name]);
                }
            }
            if (true)
            {
                olympiadCharacteristics = Characterising(CreateOlympiadPartSums, entrant);
                foreach (var name in _educationCharacterisiticNames)
                {
                    characteristicAddItems[name].Add(olympiadCharacteristics[name]);
                }
            }
            //Etc characterisitcs

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
                    b = b / 4;
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
        readonly List<string> _educationCharacterisiticNames;
        //Задает/обновляет настройки статического класса.

        public IdealEntrantResult(List<string> educationCharacterisiticNames)
        {
            _educationCharacterisiticNames = educationCharacterisiticNames;
        }

        //Для 1-го предположения(простое сложение+ нормир)
        public Dictionary<string, double> GetSimpleResult()
        {
            var db = new OptimalEducationDbContext();
            var idealEntrant = db.Entrants
                .Include(e => e.ParticipationInSchools.Select(h => h.School.Weights))
                .Include(e => e.ParticipationInSections.Select(pse => pse.Section.Weights))
                .Include(e => e.ParticipationInOlympiads.Select(po => po.Olympiad.Weights))
                .Include(e => e.Hobbies.Select(h => h.Weights))
                .Include(e => e.SchoolMarks.Select(sm => sm.SchoolDiscipline.Weights))
                .Include(e => e.UnitedStateExams.Select(use => use.Discipline.Weights))
                .AsNoTracking()
                .Where(e => e.Id == 2).Single();
            var characterizer = new EntrantSummator(_educationCharacterisiticNames);

            return characterizer.CalculateSimpleSum(idealEntrant);
        }
        //Для 2-го предположения(геом сложение+ нормир)
        public Dictionary<string, double> GetComplicatedResult()
        {
            var db = new OptimalEducationDbContext();
            var idealEntrant =db.Entrants
                .Include(e => e.ParticipationInSchools.Select(h => h.School.Weights))
                .Include(e => e.ParticipationInSections.Select(pse=>pse.Section.Weights))
                .Include(e => e.ParticipationInOlympiads.Select(po => po.Olympiad.Weights))
                .Include(e => e.Hobbies.Select(h => h.Weights))
                .Include(e => e.SchoolMarks.Select(sm => sm.SchoolDiscipline.Weights))
                .Include(e => e.UnitedStateExams.Select(use => use.Discipline.Weights))
                .Where(e => e.Id == 2)
                .AsNoTracking()
                .Single();
            var characterizer = new EntrantSummator(_educationCharacterisiticNames);

            return characterizer.CalculateComplicatedSum(idealEntrant);
        }
    }
}
