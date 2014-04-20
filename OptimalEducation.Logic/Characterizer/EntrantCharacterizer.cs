using OptimalEducation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Logic.Characterizer
{
    /// <summary>
    /// Результаты характеристикиизации для пользователя. 
    /// Класс инкапсулирует логику построения частичных характеристикиов(по каждой характеристике пользователя)
    /// и ихнему суммированию в целостный характеристики.
    /// </summary>
    public class EntrantCharacterizer
    {
        Entrant _entrant;
        Dictionary<string, double> unatedStatedExamCharacterisics = new Dictionary<string, double>();
        Dictionary<string, double> schoolMarkCharacterisics = new Dictionary<string, double>();
        Dictionary<string, double> olympiadCharacterisics = new Dictionary<string, double>();
        Dictionary<string, double> sectionCharacterisics = new Dictionary<string, double>();
        Dictionary<string, double> hobbieCharacterisics = new Dictionary<string, double>();
        Dictionary<string, double> schoolTypeCharacterisics = new Dictionary<string, double>();

        Dictionary<string, double> _totalCharacterisics = new Dictionary<string, double>();
        public Dictionary<string, double> Characterisics { get { return _totalCharacterisics; } }
        public EntrantCharacterizer(Entrant entrant)
        {
            _entrant = entrant;
            CalculateSum();
        }

        #region По заданным частным данным абитуриента(егэ, оценки, хобби и пр) строит частичные таблицы с характеристиками, которые позже просуммируются по заданному правилу
        private void UnatedStateExamCharacterising()
        {
            foreach (var exam in _entrant.UnitedStateExams)
            {
                double result = exam.Result/100.0;//нормализованный результат(100б=1.00, 70,=0.7)
                var discipline = exam.Discipline;
                foreach (var weight in discipline.Weights)
                {
                    var coeff = weight.Coefficient;
                    var characteristicName = weight.Characterisic.Name;

                    var characteristicResult = result * coeff;
                    FillPartialCharacteristics(unatedStatedExamCharacterisics, characteristicName, characteristicResult);
                }
            }
        }

        private void SchoolMarkCharacterising()
        {
            foreach (var shoolMark in _entrant.SchoolMarks)
            {
                //нормализованный результат(5=1.00)
                double result = shoolMark.Result/5.0;
                var discipline = shoolMark.SchoolDiscipline;
                foreach (var weight in discipline.Weights)
                {
                    var coeff = weight.Coefficient;
                    var characteristicName = weight.Characterisic.Name;

                    var characteristicResult = result * coeff;
                    FillPartialCharacteristics(schoolMarkCharacterisics, characteristicName, characteristicResult);
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

                    FillPartialCharacteristics(olympiadCharacterisics, characteristicName, characteristicResult);
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

                    FillPartialCharacteristics(sectionCharacterisics, characteristicName, characteristicResult);
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

                    FillPartialCharacteristics(hobbieCharacterisics, characteristicName, characteristicResult);
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

                    FillPartialCharacteristics(schoolTypeCharacterisics, characteristicName, characteristicResult);
                }
            }
        }
        /// <summary>
        /// Заполняет выбранный характеристики заданными значениями(добавляет или суммирует)
        /// </summary>
        /// <param name="characteristicToFill">характеристики, который необходимо заполнить/обновить</param>
        /// <param name="characteristicName">Элемент характеристики, который добавляют/обновляют</param>
        /// <param name="characteristicResult"></param>
        private void FillPartialCharacteristics(Dictionary<string,double> characteristicToFill ,string characteristicName, double characteristicResult)
        {
            if (!characteristicToFill.ContainsKey(characteristicName))
                characteristicToFill.Add(characteristicName, characteristicResult);
            else
                characteristicToFill[characteristicName] += characteristicResult;
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
            SectionCharacterising();
            HobbieCharacterising();
            SchoolTypeCharacterising();

            //Скаладываем по правилу:
            //T1*K1 + T2*K2 + T3*K3 +...
            // где T1 наши частичные таблицы с характеристиками
            //K - коэффцииенты( сумма которых=1)
            //Вообще 2 стратегии:
            //1. Равного вклада -  не важно сколько данных ты заполнил (у васи только егэ, у пети все, результат одинаковый)
            //2. Неравного вклада - у пети, который указал все, больше очков
            //В первом случае для вузов(у которых ток егэ): S = (1.0W1+1.0*W2...)1.0
            //Т.е. хорошо тем, что доп данные не улучшают, а как бы уточняют результат.

            //Требуется так же учитывать то, что в наборе характеристик может не быть какой-то частной характеристики
            //Так же прикинуть ситуацию, может ли у нас получиться значение вычисленных хар-к >1 
            //(напр 100 по русскому, 100 по матема, 100 по информатике=>1*0.6+1*0.5>1 (илии просто правильно подбирать веса)
            foreach (var item in unatedStatedExamCharacterisics)
            {
                FillTotalCharacteristics(item);
            }
            foreach (var item in schoolMarkCharacterisics)
            {
                FillTotalCharacteristics(item);
            }
            foreach (var item in olympiadCharacterisics)
            {
                FillTotalCharacteristics(item);
            }
            foreach (var item in sectionCharacterisics)
            {
                FillTotalCharacteristics(item);
            }
            foreach (var item in hobbieCharacterisics)
            {
                FillTotalCharacteristics(item);
            }
            foreach (var item in schoolTypeCharacterisics)
            {
                FillTotalCharacteristics(item);
            }
        }

        private void FillTotalCharacteristics(KeyValuePair<string, double> item)
        {
            if (!_totalCharacterisics.ContainsKey(item.Key))
                _totalCharacterisics.Add(item.Key, item.Value);
            else
                _totalCharacterisics[item.Key] += item.Value;
        }
    }
}
