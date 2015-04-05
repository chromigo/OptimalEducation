using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces.CQRS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using OptimalEducation.DAL.Models;
using OptimalEducation.DAL.Queries;
using OptimalEducation.Implementation.Logic.Characterizers;
using OptimalEducation.Interfaces.Logic.Characterizers;

namespace OptimalEducation.UnitTests.Logic.Characterizers.EntrantTests
{
    [TestClass]
    public class IdealEntrantResultTest
    {
        [TestMethod]
        public void GetCorrectIdealEntrantSimpleResult_and_result_is_cached()
        {
            //Arrange
            var entrant = new Entrant {Id = 2};
            var idealResult = new Dictionary<string, double>();

            var queryBuilder = Substitute.For<IQueryBuilder>();
            var entrantSummator = Substitute.For<ISummator<Entrant>>();
            var idealEntrantResult = new IdealEntrantResult(entrantSummator, queryBuilder);

            queryBuilder
                .For<Task<Entrant>>()
                .With(Arg.Any<GetEntrantForCharacterizerCriterion>())
                .ReturnsForAnyArgs(Task.FromResult(entrant));
            queryBuilder.ClearReceivedCalls();

            entrantSummator.CalculateSimpleSum(entrant).Returns(idealResult);
            entrantSummator.ClearReceivedCalls();

            //Act
            var currentResultTask = idealEntrantResult.GetSimpleResult();
            currentResultTask.Wait();
            //последующие вызовы
            idealEntrantResult.GetSimpleResult().Wait();
            idealEntrantResult.GetSimpleResult().Wait();

            //Assert
            Assert.AreEqual(idealResult, currentResultTask.Result);

            //cache test
            //Проверяем что методы вызывались только при первом обращении
            queryBuilder.Received(1).For<Task<Entrant>>();
            entrantSummator.Received(1).CalculateSimpleSum(entrant);
        }

        [TestMethod]
        public void GetCorrectIdealEntrantComplicatedResult_and_result_is_cached()
        {
            //Arrange
            var entrant = new Entrant {Id = 2};
            var idealResult = new Dictionary<string, double>();

            var queryBuilder = Substitute.For<IQueryBuilder>();
            var entrantSummator = Substitute.For<ISummator<Entrant>>();
            var idealEntrantResult = new IdealEntrantResult(entrantSummator, queryBuilder);

            queryBuilder
                .For<Task<Entrant>>()
                .With(Arg.Any<GetEntrantForCharacterizerCriterion>())
                .ReturnsForAnyArgs(Task.FromResult(entrant));
            queryBuilder.ClearReceivedCalls();

            entrantSummator.CalculateComplicatedSum(entrant).Returns(idealResult);
            entrantSummator.ClearReceivedCalls();

            //Act
            var currentResultTask = idealEntrantResult.GetComplicatedResult();
            currentResultTask.Wait();
            //последующие вызовы
            idealEntrantResult.GetComplicatedResult().Wait();
            idealEntrantResult.GetComplicatedResult().Wait();

            //Assert
            Assert.AreEqual(idealResult, currentResultTask.Result);

            //cache test
            //Проверяем что методы вызывались только при первом обращении
            queryBuilder.Received(1).For<Task<Entrant>>();
            entrantSummator.Received(1).CalculateComplicatedSum(entrant);
        }
    }
}