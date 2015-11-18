using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using OptimalEducation.DAL.Models;
using OptimalEducation.Implementation.Logic.DistanceRecomendator;
using OptimalEducation.Interfaces.Logic.Characterizers;

namespace OptimalEducation.UnitTests.Logic.DistanceRecomendator
{
    [TestFixture]
    public class EducationLineDistanceRecomendatorTest
    {
        [Test]
        public void GetRecomedtationMethod_get_ordered_and_correct_recomedations()
        {
            //Arrange
            var entrantCharacterizer = Substitute.For<ICharacterizer<Entrant>>();
            var educationLineCharacterizer = Substitute.For<ICharacterizer<EducationLine>>();

            var educationLine = new EducationLine();
            var entrantList = new List<Entrant>
            {
                new Entrant {Id = 1},
                new Entrant {Id = 2}
            };

            var educationLineCharcteristic = new Dictionary<string, double>
            {
                {"Русский", 0.5},
                {"Математика", 0.6},
                {"Информатика", 0.7}
            };
            var entrantCharacteristic1 = new Dictionary<string, double>
            {
                {"Русский", 0.8},
                {"Математика", 0.6},
                {"Информатика", 0.5}
            };
            var entrantCharacteristic2 = new Dictionary<string, double>
            {
                {"Русский", 0.6},
                {"Математика", 0.7},
                {"Информатика", 0.8}
            };

            var entrantEdLineOneDistance = Math.Abs(0.5 - 0.8) + Math.Abs(0.6 - 0.6) + Math.Abs(0.7 - 0.5); //0.5
            var entrantEdLineTwoDistance = Math.Abs(0.5 - 0.6) + Math.Abs(0.6 - 0.7) + Math.Abs(0.7 - 0.8); //0.3

            educationLineCharacterizer
                .Calculate(educationLine)
                .Returns(Task.FromResult(educationLineCharcteristic));
            entrantCharacterizer
                .Calculate(entrantList[0])
                .Returns(Task.FromResult(entrantCharacteristic1));
            entrantCharacterizer
                .Calculate(entrantList[1])
                .Returns(Task.FromResult(entrantCharacteristic2));

            //Act
            var entrantRecomendator = new EducationLineDistanceRecomendator(entrantCharacterizer,
                educationLineCharacterizer);
            var taskResult = entrantRecomendator.GetRecomendation(educationLine, entrantList);
            taskResult.Wait();
            var result = taskResult.Result;

            //Assert
            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result[entrantList[1]] == entrantEdLineTwoDistance);
            Assert.IsTrue(result[entrantList[0]] == entrantEdLineOneDistance);
        }
    }
}