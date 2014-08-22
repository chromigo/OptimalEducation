using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimalEducation.DAL.Models;
using System.Collections.Generic;
using OptimalEducation.Implementation.Logic.Characterizers;
using NSubstitute;
using Interfaces.CQRS;
using System.Threading.Tasks;
using OptimalEducation.DAL.Queries;
using OptimalEducation.Interfaces.Logic.Characterizers;
namespace OptimalEducation.UnitTests.Logic.Characterizers
{
    [TestClass]
    public class IdealEntrantResultTest
    {
        [TestMethod]
        public void GetCorrectIdealEntrantSimpleResult_and_result_is_cached()
        {
            //Arrange
            var entrant = new Entrant() { Id = 2 };
            var idealResult = new Dictionary<string,double>();

            var queryBuilder = Substitute.For<IQueryBuilder>();
            var entrantSummator = Substitute.For<ISummator<Entrant>>();
            var idealEntrantResult = new IdealEntrantResult(entrantSummator, queryBuilder);

            queryBuilder
                .For<Task<Entrant>>()
                .With(Arg.Any<GetEntrantForCharacterizerCriterion>())
                .ReturnsForAnyArgs(Task.FromResult(entrant));

            entrantSummator.CalculateSimpleSum(entrant).Returns(idealResult);

            //Act
            var currentResultTask = idealEntrantResult.GetSimpleResult();
            currentResultTask.Wait();

            //Assert
            Assert.AreEqual(idealResult, currentResultTask.Result);

            //2.
            //queryBuilder.DidNotReceive();
            
            //idealEntrantResult.n.GetSimpleResult();

        }
    }
}
