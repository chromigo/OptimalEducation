using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimalEducation.Implementation.Logic.Characterizers;
using NSubstitute;
using OptimalEducation.Interfaces.Logic.Characterizers;
using OptimalEducation.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OptimalEducation.UnitTests.Logic.Characterizers.EducationLineTests
{
    [TestClass]
    public class EducationLinesCharacterizerTest
    {
        [TestMethod]
        public void Calculate_DefaultComplicated_Norm_Result()
        {
            //Arrange
            var namesHelper = Substitute.For<IEducationCharacteristicNamesHelper>();
            var educationLineSummator = Substitute.For<ISummator<EducationLine>>();
            var idealResult = Substitute.For<IIdealResult<EducationLine>>();

            var educationLineCharacterizer = new EducationLineCharacterizer(namesHelper, educationLineSummator, idealResult);
            var educationLine = new EducationLine(){};

            var characteristicNames=new List<string>()
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
            educationLineSummator.CalculateComplicatedSum(educationLine).Returns(complicatedSum);
            
            var complicatedIdealSum=new Dictionary<string,double>();
            complicatedIdealSum.Add("Русский", 0.4);
            complicatedIdealSum.Add("Математика", 0.6);
            complicatedIdealSum.Add("Информатика", 0.8);
            idealResult.GetComplicatedResult().Returns(Task.FromResult(complicatedIdealSum));
            
            //Act
            var resultTask = educationLineCharacterizer.Calculate(educationLine);
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

        [TestMethod]
        public void Calculate_Simple_Norm_Result()
        {
            //Arrange
            var namesHelper = Substitute.For<IEducationCharacteristicNamesHelper>();
            var educationLineSummator = Substitute.For<ISummator<EducationLine>>();
            var idealResult = Substitute.For<IIdealResult<EducationLine>>();

            var educationLineCharacterizer = new EducationLineCharacterizer(namesHelper, educationLineSummator, idealResult);
            var educationLine = new EducationLine() { };

            var characteristicNames = new List<string>()
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
            educationLineSummator.CalculateSimpleSum(educationLine).Returns(complicatedSum);

            var complicatedIdealSum = new Dictionary<string, double>();
            complicatedIdealSum.Add("Русский", 0.4);
            complicatedIdealSum.Add("Математика", 0.6);
            complicatedIdealSum.Add("Информатика", 0.8);
            idealResult.GetSimpleResult().Returns(Task.FromResult(complicatedIdealSum));

            //Act
            var resultTask = educationLineCharacterizer.Calculate(educationLine,false);
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
