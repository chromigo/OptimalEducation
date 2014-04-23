using OptimalEducation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Logic.Characterizer
{
    /// <summary>
    /// Результаты характеристикиизации для пользователя по методу уменьш коэф-в:
    /// Вычисляются значения для каждой характеристики(например, потенциал в математике, информатике, русском)
    /// Эти значения не суммируются сразу, а складываются в списки, соотносящиеся с данной характеристикой
    /// Позже эти значения сортируются и домножаются на убывающий коэффициент
    /// </summary>
    public class EntrantCharacterizer_DecreasingCoeff
    {
        Entrant _entrant;
        Dictionary<string, List<double>> characterisicAddItems = new Dictionary<string, List<double>>();

        Dictionary<string, double> _totalCharacteristics = new Dictionary<string, double>();
        public Dictionary<string, double> Characteristics { get { return _totalCharacteristics; } }

        public EntrantCharacterizer_DecreasingCoeff(Entrant entrant)
        {
            _entrant = entrant;
            InitCharacterisitcs();
            CalculateSum();
        }
        private void InitCharacterisitcs()
        {
            //Заполняем словарь всеми ключами по возможным весам
            OptimalEducationDbContext context = new OptimalEducationDbContext();
            var educationCharacterisitcs = context.Characteristics
                .Where(p=>p.Type==CharacteristicType.Education)
                .Select(p => p.Name)
                .ToList();
            foreach (var item in educationCharacterisitcs)
            {
                characterisicAddItems.Add(item, new List<double>());
            }
        }
        #region Добавляем в списки слагаемые для сложени(для каждой характеристики)
        private void UnatedStateExamCharacterising()
        {
            foreach (var exam in _entrant.UnitedStateExams)
            {
                if(exam.Result.HasValue)
                {
                    double result = exam.Result.Value / 100.0;//нормализованный результат(100б=1.00, 70,=0.7)
                    var discipline = exam.Discipline;
                    foreach (var weight in discipline.Weights)
                    {
                        var coeff = weight.Coefficient;
                        var characteristicName = weight.Characterisic.Name;

                        var characteristicResult = result * coeff;
                        characterisicAddItems[characteristicName].Add(characteristicResult);
                    }
                }
            }
        }

        private void SchoolMarkCharacterising()
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
                        characterisicAddItems[characteristicName].Add(characteristicResult);
                    }
                }
            }
        }

        private void OlympiadCharacterising()
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
                    //TODO: Реализовать особую логику учета данных?
                    switch (result)
                    {
                        case OlypmpiadResult.FirstPlace: characteristicResult = ((double)result/100) * coeff;
                            break;
                        case OlypmpiadResult.SecondPlace: characteristicResult = ((double)result / 100) * coeff;
                            break;
                        case OlypmpiadResult.ThirdPlace: characteristicResult = ((double)result / 100) * coeff;
                            break;
                        default:
                            break;
                    }

                    characterisicAddItems[characteristicName].Add(characteristicResult);
                }
            }
        }

        private void SectionCharacterising()
        {
            foreach (var sectionResult in _entrant.ParticipationInSections)
            {
                double result = 0;
                //По правилу 80/20?
                if (sectionResult.YearPeriod >= 10) result = 1.00;
                else if (sectionResult.YearPeriod>5) result = 0.90;
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

                    characterisicAddItems[characteristicName].Add(characteristicResult);
                }
            }
        }

        private void HobbieCharacterising()
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

                    characterisicAddItems[characteristicName].Add(characteristicResult);
                }
            }
        }

        private void SchoolTypeCharacterising()
        {
            //для простоты будем брать последнюю школу, где абитуриент учился
            //(возможно стоит рассмотреть более сложный вариант в будущем)
            var lastParticipationInSchool = _entrant.ParticipationInSchools.LastOrDefault();
            if(lastParticipationInSchool!=null)
            {
                //Или еще учитывать кол-во лет обучения?
                var quality = lastParticipationInSchool.School.EducationQuality/100.0;
                var schoolWeights = lastParticipationInSchool.School.Weights;
                foreach (var weight in schoolWeights)
                {
                    var coeff = weight.Coefficient;
                    var characteristicName = weight.Characterisic.Name;

                    //TODO: Реализовать особую логику учета данных?
                    double characteristicResult = quality * coeff;

                    characterisicAddItems[characteristicName].Add(characteristicResult);
                }
            }
        }
        #endregion

        /// <summary>
        /// Складывает результаты каждого частного характеристикиа по определенному правилу
        /// </summary>
        private void CalculateSum()
        {
            //Вычисляем частичные характеристики
            //в результатер работы каждой функции получается новая таблица характеристик
            UnatedStateExamCharacterising();
            SchoolMarkCharacterising();
            OlympiadCharacterising();
            //SectionCharacterising();
            //HobbieCharacterising();
            //SchoolTypeCharacterising();

            //TODO: Сортируем в нужном порядке и Складываем по определенному правилу

            foreach (var item in characterisicAddItems)
            {
                item.Value.Sort();
                _totalCharacteristics.Add(item.Key, 0);
                //Складываем
                double sum = 0;
                //TODO: сложить по правилу
                _totalCharacteristics[item.Key] = sum;
            }
        }
    }
}
