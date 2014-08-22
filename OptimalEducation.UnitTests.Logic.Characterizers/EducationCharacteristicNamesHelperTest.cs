using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Interfaces.CQRS;
using System.Collections.Generic;
using OptimalEducation.DAL.Queries;
using OptimalEducation.Implementation.Logic.Characterizers;

namespace OptimalEducation.UnitTests.Logic.Characterizers
{
    [TestClass]
    public class EducationCharacteristicNamesHelperTest
    {
        [TestMethod]
        public void GetCorrectListOfNames()
        {
            //Arrange
            var answer=new List<string>() {"Русский" ,"Математика" ,"Информатика"};

            var queryBuilder = Substitute.For<IQueryBuilder>();
            var queryFor = Substitute.For<IQueryFor<IEnumerable<string>>>();

            queryBuilder
                .For<IEnumerable<string>>()
                .Returns(queryFor);

            queryFor.With(Arg.Any<GetEducationCharacterisitcNamesCriterion>()).ReturnsForAnyArgs(answer);

            //Act
            var helper = new EducationCharacteristicNamesHelper(queryBuilder);
            var result = helper.Names;

            //Assert
            Assert.AreEqual(answer, result);
        }
    }
}
