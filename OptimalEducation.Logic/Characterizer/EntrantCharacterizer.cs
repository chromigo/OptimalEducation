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
                var result = exam.Result;
                var discipline = exam.Discipline;
                foreach (var weight in discipline.Weights)
                {
                    var coeff = weight.Coefficient;
                    var characteristicName = weight.Cluster.Name;

                    var characteristicResult = result * coeff;
                    FillPartialCharacteristics(unatedStatedExamCharacterisics, characteristicName, characteristicResult);
                }
            }
        }

        private void SchoolMarkCharacterising()
        {
            foreach (var shoolMark in _entrant.SchoolMarks)
            {
                var result = shoolMark.Result;
                var discipline = shoolMark.SchoolDiscipline;
                foreach (var weight in discipline.Weights)
                {
                    var coeff = weight.Coefficient;
                    var characteristicName = weight.Cluster.Name;

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
                    var characteristicName = weight.Cluster.Name;

                    double characteristicResult = 0;
                    //TODO: Реализовать особую логику учета данных?
                    switch (result)
                    {
                        case OlypmpiadResult.FirstPlace: characteristicResult = (int)result * coeff;
                            break;
                        case OlypmpiadResult.SecondPlace: characteristicResult = (int)result * coeff;
                            break;
                        case OlypmpiadResult.ThirdPlace: characteristicResult = (int)result * coeff;
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
                var result = sectionResult.YearPeriod;
                var section = sectionResult.Section;
                foreach (var weight in section.Weights)
                {
                    var coeff = weight.Coefficient;
                    var characteristicName = weight.Cluster.Name;

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
                    var characteristicName = weight.Cluster.Name;

                    //TODO: Реализовать особую логику учета данных?
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
                //Или еще учитывать кол-во лет обучения?(нет в модели)
                var quality = lastParticipationInSchool.School.EducationQuality;
                var schoolWeights = lastParticipationInSchool.School.Weights;
                foreach (var weight in schoolWeights)
                {
                    var coeff = weight.Coefficient;
                    var characteristicName = weight.Cluster.Name;

                    //TODO: Реализовать особую логику учета данных?
                    double characteristicResult = (int)quality * coeff;

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
            UnatedStateExamCharacterising();
            SchoolMarkCharacterising();
            OlympiadCharacterising();
            SectionCharacterising();
            HobbieCharacterising();
            SchoolTypeCharacterising();

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
