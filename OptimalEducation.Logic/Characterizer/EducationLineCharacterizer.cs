using OptimalEducation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OptimalEducation.Logic.Characterizer
{
    /// <summary>
    /// Результаты характеристикиизации для учебного направления. 
    /// </summary>
    public class EducationLineCharacterizer
    {
        EducationLine _educationLine;
        Dictionary<string, double> _educationLineCharacteristics = new Dictionary<string, double>();

        Dictionary<string, double> _totalCharacteristics = new Dictionary<string, double>();
        public Dictionary<string, double> Characteristics { get { return _totalCharacteristics; } }

        public EducationLineCharacterizer(EducationLine educationLine)
        {
            _educationLine = educationLine;
            CalculateSum();
        }

        #region По заданным характеристикам строит частичные характеристикиы с результатами
        private void EducationLinesRequirementsCharacterising()
        {
            foreach (var requirement in _educationLine.EducationLinesRequirements)
            {
                var result = requirement.Requirement;
                var discipline = requirement.ExamDiscipline;
                foreach (var weight in discipline.Weights)
                {
                    var coeff = weight.Coefficient;
                    var characteristicName = weight.Characterisic.Name;

                    var characterisitcResult = result * coeff;
                    FillPartialCharacteristics(_educationLineCharacteristics, characteristicName, characterisitcResult);
                }
            }
        }
        /// <summary>
        /// Заполняет выбранный характеристики заданными значениями(добавляет или суммирует)
        /// </summary>
        /// <param name="characteristicsToFill">характеристики, которые необходимо заполнить/обновить</param>
        /// <param name="characteristicsName">Элемент характеристикиа, который добавляют/обновляют</param>
        /// <param name="characterisicResult"></param>
        private void FillPartialCharacteristics(Dictionary<string,double> characteristicsToFill ,string characteristicsName, double characterisicResult)
        {
            if (!characteristicsToFill.ContainsKey(characteristicsName))
                characteristicsToFill.Add(characteristicsName, characterisicResult);
            else
                characteristicsToFill[characteristicsName] += characterisicResult;
        }
        #endregion
        /// <summary>
        /// Складывает результаты каждого частного характеристикиа по определенному правилу
        /// </summary>
        private void CalculateSum()
        {
            EducationLinesRequirementsCharacterising();

            _totalCharacteristics = _educationLineCharacteristics;
            //Добавить если будут дополнительные характеристики
            //foreach (var item in educationLineCharacteristics)
            //{
            //    FillTotalCharacteristics(item);
            //}
        }

        private void FillTotalCharacteristics(KeyValuePair<string, double> item)
        {
            if (!_totalCharacteristics.ContainsKey(item.Key))
                _totalCharacteristics.Add(item.Key, item.Value);
            else
                _totalCharacteristics[item.Key] += item.Value;
        }
    }
}