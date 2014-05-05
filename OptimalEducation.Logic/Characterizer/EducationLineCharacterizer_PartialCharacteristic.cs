﻿using OptimalEducation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OptimalEducation.Logic.Characterizer
{
    /// <summary>
    /// Результаты характеристикиизации для учебного направления по методу частичных сумм:
    /// Для каждой группы криетриев(балы егэ, оценки в школе и тп) строится отдельная таблица характеристик
    /// Позже эти отдельные таблицы определенным(пока не определенным) образом складываются и получается итоговый ответ.
    /// </summary>
    public class EducationLineCharacterizer_PartialCharacteristic
    {
        EducationLine _educationLine;
        Dictionary<string, double> _educationLineCharacteristics = new Dictionary<string, double>();

        Dictionary<string, double> _totalCharacteristics = new Dictionary<string, double>();
        public Dictionary<string, double> Characteristics { get { return _totalCharacteristics; } }

        public EducationLineCharacterizer_PartialCharacteristic(EducationLine educationLine)
        {
            _educationLine = educationLine;
            InitCharacterisitcs();
            CalculateSum();
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
                    _educationLineCharacteristics[characteristicName] += characteristicResult;
                }
            }
        }

        private void InitCharacterisitcs()
        {
            //Заполняем словарь всеми ключами по возможным весам, типа Education
            OptimalEducationDbContext context = new OptimalEducationDbContext();
            var characterisitcs = context.Characteristics
                .Where(p=>p.Type==CharacteristicType.Education)
                .Select(p => p.Name)
                .ToList();
            foreach (var item in characterisitcs)
            {
                _educationLineCharacteristics.Add(item, 0);
            }
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