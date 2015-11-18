using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces.CQRS;
using NSubstitute;
using NUnit.Framework;
using OptimalEducation.DAL.Models;
using OptimalEducation.DAL.Queries;
using OptimalEducation.Implementation.Logic.Characterizers;
using OptimalEducation.Interfaces.Logic.Characterizers;

namespace OptimalEducation.UnitTests.Logic.Characterizers.EducationLineTests
{
    [TestFixture]
    public class IdealEducationLineResultTest
    {
        [Test]
        public void GetCorrectIdealEducationLineSimpleResult_and_result_is_cached()
        {
            //Arrange
            var educationLine = new EducationLine {Id = 2};
            var idealResult = new Dictionary<string, double>();

            var queryBuilder = Substitute.For<IQueryBuilder>();
            var educationLineSummator = Substitute.For<ISummator<EducationLine>>();
            var idealEducationLineResult = new IdealEducationLineResult(educationLineSummator, queryBuilder);

            queryBuilder
                .For<Task<EducationLine>>()
                .With(Arg.Any<GetIdelaEducationLineForCharacterizerCriterion>())
                .ReturnsForAnyArgs(Task.FromResult(educationLine));
            queryBuilder.ClearReceivedCalls();

            educationLineSummator.CalculateSimpleSum(educationLine).Returns(idealResult);
            educationLineSummator.ClearReceivedCalls();

            //Act
            var currentResultTask = idealEducationLineResult.GetSimpleResult();
            currentResultTask.Wait();
            //последующие вызовы
            idealEducationLineResult.GetSimpleResult().Wait();
            idealEducationLineResult.GetSimpleResult().Wait();

            //Assert
            Assert.AreEqual(idealResult, currentResultTask.Result);

            //cache test
            //Проверяем что методы вызывались только при первом обращении
            queryBuilder.Received(1).For<Task<EducationLine>>();
            educationLineSummator.Received(1).CalculateSimpleSum(educationLine);
        }

        [Test]
        public void GetCorrectIdealEducationLineComplicatedResult_and_result_is_cached()
        {
            //Arrange
            var educationLine = new EducationLine {Id = 2};
            var idealResult = new Dictionary<string, double>();

            var queryBuilder = Substitute.For<IQueryBuilder>();
            var educationLineSummator = Substitute.For<ISummator<EducationLine>>();
            var idealEducationLineResult = new IdealEducationLineResult(educationLineSummator, queryBuilder);

            queryBuilder
                .For<Task<EducationLine>>()
                .With(Arg.Any<GetIdelaEducationLineForCharacterizerCriterion>())
                .ReturnsForAnyArgs(Task.FromResult(educationLine));
            queryBuilder.ClearReceivedCalls();

            educationLineSummator.CalculateComplicatedSum(educationLine).Returns(idealResult);
            educationLineSummator.ClearReceivedCalls();

            //Act
            var currentResultTask = idealEducationLineResult.GetComplicatedResult();
            currentResultTask.Wait();
            //последующие вызовы
            idealEducationLineResult.GetComplicatedResult().Wait();
            idealEducationLineResult.GetComplicatedResult().Wait();

            //Assert
            Assert.AreEqual(idealResult, currentResultTask.Result);

            //cache test
            //Проверяем что методы вызывались только при первом обращении
            queryBuilder.Received(1).For<Task<EducationLine>>();
            educationLineSummator.Received(1).CalculateComplicatedSum(educationLine);
        }
    }
}