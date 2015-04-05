using System.Collections.Generic;
using Interfaces.CQRS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using OptimalEducation.DAL.Queries;
using OptimalEducation.Implementation.Logic.Characterizers;

namespace OptimalEducation.UnitTests.Logic.Characterizers
{
    [TestClass]
    public class EducationCharacteristicNamesHelperTest
    {
        [TestMethod]
        public void GetCorrectListOfNames_and_result_is_cached()
        {
            //Arrange
            var answer = new List<string> {"Русский", "Математика", "Информатика"};

            var queryBuilder = Substitute.For<IQueryBuilder>();

            queryBuilder
                .For<IEnumerable<string>>()
                .With(Arg.Any<GetEducationCharacterisitcNamesCriterion>()).ReturnsForAnyArgs(answer);

            queryBuilder.ClearReceivedCalls();

            //Act
            var helper = new EducationCharacteristicNamesHelper(queryBuilder);
            var result = helper.Names;

            //Assert
            Assert.AreEqual(answer, result);
            //cache test
            queryBuilder.Received(1).For<IEnumerable<string>>();
        }
    }
}