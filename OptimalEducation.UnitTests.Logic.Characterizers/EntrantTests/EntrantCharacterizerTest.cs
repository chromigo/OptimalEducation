using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using OptimalEducation.DAL.Models;
using OptimalEducation.Implementation.Logic.Characterizers;
using OptimalEducation.Interfaces.Logic.Characterizers;

namespace OptimalEducation.UnitTests.Logic.Characterizers.EntrantTests
{
    [TestFixture]
    public class EntrantCharacterizerTest
    {
        [Test]
        public void Calculate_DefaultComplicated_Norm_Result()
        {
            //Arrange
            var namesHelper = Substitute.For<IEducationCharacteristicNamesHelper>();
            var entrantSummator = Substitute.For<ISummator<Entrant>>();
            var idealResult = Substitute.For<IIdealResult>();

            var entrantCharacterizer = new EntrantCharacterizer(namesHelper, entrantSummator, idealResult);
            var entrant = new Entrant();

            var characteristicNames = new List<string>
            {
                "Русский",
                "Математика",
                "Информатика"
            };
            namesHelper.Names.Returns(characteristicNames);

            var complicatedSum = new Dictionary<string, double>();
            complicatedSum.Add("Русский", 0.4);
            complicatedSum.Add("Математика", 0.6);
            complicatedSum.Add("Информатика", 0.8);
            entrantSummator.CalculateComplicatedSum(entrant).Returns(complicatedSum);

            var complicatedIdealSum = new Dictionary<string, double>();
            complicatedIdealSum.Add("Русский", 0.4);
            complicatedIdealSum.Add("Математика", 0.6);
            complicatedIdealSum.Add("Информатика", 0.8);
            idealResult.GetComplicatedResult().Returns(Task.FromResult(complicatedIdealSum));

            //Act
            var resultTask = entrantCharacterizer.Calculate(entrant);
            resultTask.Wait();

            //Assert
            var correctAnswer = new Dictionary<string, double>();
            correctAnswer.Add("Русский", 1);
            correctAnswer.Add("Математика", 1);
            correctAnswer.Add("Информатика", 1);
            foreach (var item in resultTask.Result)
            {
                Assert.AreEqual(correctAnswer[item.Key], item.Value);
            }
        }

        [Test]
        public void Calculate_Simple_Norm_Result()
        {
            //Arrange
            var namesHelper = Substitute.For<IEducationCharacteristicNamesHelper>();
            var entrantSummator = Substitute.For<ISummator<Entrant>>();
            var idealResult = Substitute.For<IIdealResult>();

            var entrantCharacterizer = new EntrantCharacterizer(namesHelper, entrantSummator, idealResult);
            var entrant = new Entrant();

            var characteristicNames = new List<string>
            {
                "Русский",
                "Математика",
                "Информатика"
            };
            namesHelper.Names.Returns(characteristicNames);

            var complicatedSum = new Dictionary<string, double>();
            complicatedSum.Add("Русский", 0.4);
            complicatedSum.Add("Математика", 0.6);
            complicatedSum.Add("Информатика", 0.8);
            entrantSummator.CalculateSimpleSum(entrant).Returns(complicatedSum);

            var complicatedIdealSum = new Dictionary<string, double>();
            complicatedIdealSum.Add("Русский", 0.4);
            complicatedIdealSum.Add("Математика", 0.6);
            complicatedIdealSum.Add("Информатика", 0.8);
            idealResult.GetSimpleResult().Returns(Task.FromResult(complicatedIdealSum));

            //Act
            var resultTask = entrantCharacterizer.Calculate(entrant, false);
            resultTask.Wait();

            //Assert
            var correctAnswer = new Dictionary<string, double>();
            correctAnswer.Add("Русский", 1);
            correctAnswer.Add("Математика", 1);
            correctAnswer.Add("Информатика", 1);
            foreach (var item in resultTask.Result)
            {
                Assert.AreEqual(correctAnswer[item.Key], item.Value);
            }
        }
    }
}