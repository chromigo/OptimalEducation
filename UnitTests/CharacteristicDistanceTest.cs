using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using OptimalEducation.Logic.Characterizer;

namespace UnitTests
{
    [TestClass]
    public class CharacteristicDistanceTest
    {
        //Внимание! помнить про то что double не дает 100% точности при вычислениях
        //TODO: Добавить тесты, проверяющие результаты вычислений когда у пользователя и вуза отличается набор характеристик?
        readonly Dictionary<string, double> entrantCharacteristics=new Dictionary<string,double>();
        readonly Dictionary<string, double> educationLineCharacteristics = new Dictionary<string, double>();
        public CharacteristicDistanceTest()
        {
            entrantCharacteristics.Add("Русский", 0.6);
            entrantCharacteristics.Add("Математика", 0.8);
            entrantCharacteristics.Add("Информатика", 0.9);

            educationLineCharacteristics.Add("Русский", 0.4);
            educationLineCharacteristics.Add("Математика", 0.7);
            educationLineCharacteristics.Add("Информатика", 0.8);
        }
        [TestMethod]
        public void EuclidDistance_CorrectAnswer()
        {
            var result = CharacteristicDistance.GetEuclidDistance(entrantCharacteristics, educationLineCharacteristics);

            Assert.IsTrue(result.HasValue);
            Assert.IsTrue(result.Value==
                Math.Sqrt
                (
                    Math.Pow((0.6-0.4),2)+Math.Pow((0.8-0.7),2)+Math.Pow((0.9-0.8),2)
                ));
        }
        [TestMethod]
        public void SquareEuclidDistance_CorrectAnswer()
        {
            var result = CharacteristicDistance.GetSquareEuclidDistance(entrantCharacteristics, educationLineCharacteristics);

            Assert.IsTrue(result.HasValue);
            Assert.IsTrue(result.Value == Math.Pow((0.6 - 0.4), 2) + Math.Pow((0.8 - 0.7), 2) + Math.Pow((0.9 - 0.8), 2));
        }
        [TestMethod]
        public void CityBlockDistance_CorrectAnswer()
        {
            var result = CharacteristicDistance.GetCityBlockDistance(entrantCharacteristics, educationLineCharacteristics);

            Assert.IsTrue(result.HasValue);
            Assert.IsTrue(result.Value == Math.Abs(0.6 - 0.4) + Math.Abs(0.8 - 0.7) + Math.Abs(0.9 - 0.8));
        }
        [TestMethod]
        public void ChebishevDistance_CorrectAnswer()
        {
            var result = CharacteristicDistance.GetChebishevDistance(entrantCharacteristics, educationLineCharacteristics);

            Assert.IsTrue(result.HasValue);
            Assert.IsTrue(result.Value == (0.6 - 0.4));
        }
        [TestMethod]
        public void PowerDistance_CorrectAnswer()
        {
            double p = 4;
            double r = 4;
            var result = CharacteristicDistance.GetPowerDistance(entrantCharacteristics, educationLineCharacteristics,p,r);
            var correctResult = Math.Pow
                (
                    Math.Pow((0.6 - 0.4), p) + Math.Pow((0.8 - 0.7), p) + Math.Pow((0.9 - 0.8), p),
                    1 / r
                );
            Assert.IsTrue(result.HasValue);
            Assert.IsTrue(result.Value == correctResult);
        }
    }
}
