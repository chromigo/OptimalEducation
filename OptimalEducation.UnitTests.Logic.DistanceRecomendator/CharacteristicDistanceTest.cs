using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimalEducation.Implementation.Logic.DistanceRecomendator;
using System.Collections.Generic;

namespace OptimalEducation.UnitTests.Logic.DistanceRecomendator
{
    [TestClass]
    public class CharacteristicDistanceTest
    {
        [TestMethod]
        public void GetDistance_with_CityDistance_return_correct_answer()
        {
            //Arrange
            var entrantCharacteristics =new Dictionary<string,double>();
            var educationLineCharacteristics = new Dictionary<string, double>();

            entrantCharacteristics.Add("Русский", 0.6);
            entrantCharacteristics.Add("Математика", 0.7);
            entrantCharacteristics.Add("Информатика", 0.7);

            educationLineCharacteristics.Add("Русский", 0.5);
            educationLineCharacteristics.Add("Математика", 0.7);
            educationLineCharacteristics.Add("Информатика", 0.8);

            var correctAnswer=Math.Abs(0.6-0.5)+Math.Abs(0.7-0.7)+Math.Abs(0.7-0.8);

            //Act
            var result = CharacteristicDistance.GetDistance(entrantCharacteristics, educationLineCharacteristics);

            //Assert
            Assert.AreEqual(correctAnswer, result);
        }

        [TestMethod]
        public void GetDistance_return_nullResult_if_in_Entrant_and_EducationLine_not_equal_key_elements()
        {
            //Arrange
            var entrantCharacteristics = new Dictionary<string, double>();
            var educationLineCharacteristics = new Dictionary<string, double>();

            entrantCharacteristics.Add("Литература", 0.6);
            entrantCharacteristics.Add("Математика", 0.7);
            entrantCharacteristics.Add("Информатика", 0.7);

            educationLineCharacteristics.Add("Русский", 0.5);
            educationLineCharacteristics.Add("Математика", 0.7);
            educationLineCharacteristics.Add("Информатика", 0.8);

            //Act
            var result = CharacteristicDistance.GetDistance(entrantCharacteristics, educationLineCharacteristics);

            //Assert
            Assert.IsTrue(!result.HasValue);
        }
    }
}
