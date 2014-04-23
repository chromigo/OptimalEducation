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
    public class EducationLineCharacterizer_DecreasingCoeff
    {
        EducationLine _educationLine;
        Dictionary<string, List<double>> characteristicAddItems = new Dictionary<string, List<double>>();

        Dictionary<string, double> _totalCharacteristics = new Dictionary<string, double>();
        public Dictionary<string, double> Characteristics { get { return _totalCharacteristics; } }

        public EducationLineCharacterizer_DecreasingCoeff(EducationLine educationLine)
        {
            _educationLine = educationLine;
            InitCharacterisitcs();
            CalculateSum();
        }
        private void InitCharacterisitcs()
        {
            //Заполняем словарь всеми ключами по возможным весам, типа Education
            OptimalEducationDbContext context = new OptimalEducationDbContext();
            var educationCharacterisitcs = context.Characteristics
                .Where(p => p.Type == CharacteristicType.Education)
                .Select(p => p.Name)
                .ToList();
            foreach (var item in educationCharacterisitcs)
            {
                characteristicAddItems.Add(item, new List<double>());
            }
        }
        #region По заданным характеристикам строит частичные характеристикиы с результатами
        private void EducationLinesRequirementsCharacterising()
        {
            foreach (var requirement in _educationLine.EducationLinesRequirements)
            {
                var result = requirement.Requirement/100.0;
                var discipline = requirement.ExamDiscipline;
                foreach (var weight in discipline.Weights)
                {
                    var coeff = weight.Coefficient;
                    var characteristicName = weight.Characterisic.Name;

                    var characteristicResult = result * coeff;
                    characteristicAddItems[characteristicName].Add(characteristicResult);
                }
            }
        }


        #endregion
        /// <summary>
        /// Складывает результаты каждого частного характеристикиа по определенному правилу
        /// </summary>
        private void CalculateSum()
        {
            EducationLinesRequirementsCharacterising();

            //TODO: Сортируем в нужном порядке и Складываем по определенному правилу

            foreach (var item in characteristicAddItems)
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