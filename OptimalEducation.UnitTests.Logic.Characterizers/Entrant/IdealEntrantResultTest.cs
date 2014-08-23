﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimalEducation.DAL.Models;
using System.Collections.Generic;
using OptimalEducation.Implementation.Logic.Characterizers;
using NSubstitute;
using Interfaces.CQRS;
using System.Threading.Tasks;
using OptimalEducation.DAL.Queries;
using OptimalEducation.Interfaces.Logic.Characterizers;
namespace OptimalEducation.UnitTests.Logic.Characterizers.Entrant
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
            var entrant = new Entrant() { Id = 2 };
            var idealResult = new Dictionary<string, double>();

            var queryBuilder = Substitute.For<IQueryBuilder>();
            var entrantSummator = Substitute.For<ISummator<Entrant>>();
            var idealEntrantResult = new IdealEntrantResult(entrantSummator, queryBuilder);

            queryBuilder
                .For<Task<Entrant>>()
                .With(Arg.Any<GetEntrantForCharacterizerCriterion>())
                .ReturnsForAnyArgs(Task.FromResult(entrant));
            queryBuilder.ClearReceivedCalls();

            entrantSummator.GetComplicatedResult(entrant).Returns(idealResult);
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
            entrantSummator.Received(1).GetComplicatedResult(entrant);
        }
    }
}
