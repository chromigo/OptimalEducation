using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimalEducation.Implementation.Logic.MulticriterialAnalysis;

namespace OptimalEducation.UnitTests.Logic.MulticriterialAnalysis
{
    [TestClass]
    public class PreferenceRelationCalculatorTest
    {
        [TestMethod]
        public void GetCorrectPreferenceRelation_For_first_characteristic_set()
        {
            //Arrange
            var userCharacteristics = new Dictionary<string, double>
            {
                {"Русский", 0.5},
                {"Математика", 0.6},
                {"Информатика", 0.7}
            };

            //Act
            var prefRelationCalculator = new PreferenceRelationCalculator();
            var prefRelation = prefRelationCalculator.GetPreferenceRelations(userCharacteristics);

            //Assert
            Assert.IsTrue(true);
            Assert.IsTrue(prefRelation.Count == 1);
            Assert.IsTrue(prefRelation[0].ImportantCharacterisicName == "Информатика");
            var t1 = Math.Round(prefRelation[0].Tetas["Русский"], 2);
            var t2 = Math.Round(prefRelation[0].Tetas["Математика"], 2);
            Assert.IsTrue(t1 == 0.67);
            Assert.IsTrue(t2 == 0.5);
        }

        [TestMethod]
        public void GetCorrectPreferenceRelation_For_second_characteristic_set()
        {
            //Arrange
            var userCharacteristics = new Dictionary<string, double>();
            userCharacteristics.Add("Русский", 0.5);
            userCharacteristics.Add("Математика", 0.696);
            userCharacteristics.Add("Информатика", 0.7);

            //Act
            var prefRelationCalculator = new PreferenceRelationCalculator();
            var prefRelation = prefRelationCalculator.GetPreferenceRelations(userCharacteristics);

            //Assert
            Assert.IsTrue(true);
            Assert.IsTrue(prefRelation.Count == 2);
            Assert.IsTrue(prefRelation[0].ImportantCharacterisicName == "Математика");
            Assert.IsTrue(prefRelation[1].ImportantCharacterisicName == "Информатика");
            var t1 = Math.Round(prefRelation[0].Tetas["Русский"], 3);
            var t2 = Math.Round(prefRelation[1].Tetas["Русский"], 3);
            Assert.IsTrue(t1 == 0.662);
            Assert.IsTrue(t2 == 0.667);
        }
    }
}