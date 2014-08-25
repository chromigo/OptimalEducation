using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using OptimalEducation.DAL.Models;
using OptimalEducation.Implementation.Logic.DistanceRecomendator;
using OptimalEducation.Interfaces.Logic.Characterizers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OptimalEducation.UnitTests.Logic.DistanceRecomendator
{
    [TestClass]
    public class EntrantDistanceRecomendatorTest
    {
        [TestMethod]
        public void GetRecomedtationMethod_get_ordered_and_correct_recomedations()
        {
            //Arrange
            var entrantCharacterizer = Substitute.For<ICharacterizer<Entrant>>();
            var educationLineCharacterizer = Substitute.For<ICharacterizer<EducationLine>>();

            var entrant = new Entrant();
            var educationLineList=new List<EducationLine>()
            {
                new EducationLine(){Id=1},
                new EducationLine(){Id=2},
            };

            var entrantCharcteristic = new Dictionary<string, double>();
            entrantCharcteristic.Add("Русский", 0.5);
            entrantCharcteristic.Add("Математика", 0.6);
            entrantCharcteristic.Add("Информатика", 0.7);
            var educationLineCharacteristic1 = new Dictionary<string, double>();
            educationLineCharacteristic1.Add("Русский", 0.8);
            educationLineCharacteristic1.Add("Математика", 0.6);
            educationLineCharacteristic1.Add("Информатика", 0.5);
            var educationLineCharacteristic2 = new Dictionary<string, double>();
            educationLineCharacteristic2.Add("Русский", 0.6);
            educationLineCharacteristic2.Add("Математика", 0.7);
            educationLineCharacteristic2.Add("Информатика", 0.8);

            var entrantEdLineOneDistance = Math.Abs(0.5 - 0.8) + Math.Abs(0.6 - 0.6) + Math.Abs(0.7 - 0.5);//0.5
            var entrantEdLineTwoDistance = Math.Abs(0.5 - 0.6) + Math.Abs(0.6 - 0.7) + Math.Abs(0.7 - 0.8);//0.3
            
            entrantCharacterizer
                .Calculate(entrant)
                .Returns(Task.FromResult(entrantCharcteristic));
            educationLineCharacterizer
                .Calculate(educationLineList[0])
                .Returns(Task.FromResult(educationLineCharacteristic1));
            educationLineCharacterizer
                .Calculate(educationLineList[1])
                .Returns(Task.FromResult(educationLineCharacteristic2));

            //Act
            var entrantRecomendator = new EntrantDistanceRecomendator(entrantCharacterizer, educationLineCharacterizer);
            var taskResult = entrantRecomendator.GetRecomendation(entrant, educationLineList);
            taskResult.Wait();
            var result = taskResult.Result;

            //Assert
            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result[educationLineList[1]] == entrantEdLineTwoDistance);
            Assert.IsTrue(result[educationLineList[0]] == entrantEdLineOneDistance);
        }
    }
}
