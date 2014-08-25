using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimalEducation.Areas.EntrantUser.Controllers;
using NSubstitute;
using Interfaces.CQRS;
using System.Threading.Tasks;
using System.Collections.Generic;
using OptimalEducation.DAL.Queries;
using OptimalEducation.DAL.ViewModels;
using OptimalEducation.Models;
using OptimalEducation.Helpers;

namespace OptimalEducation.Controllers
{
    [TestClass]
    public class HobbieControllerTest
    {

        [TestMethod]
        public void TestMethod1()
        {
            //Arrange
            var userManager = Substitute.For<IApplicationUserManager>();
            var commandBuilder = Substitute.For<ICommandBuilder>();
            var queryBuilder = Substitute.For<IQueryBuilder>();
            var infoExtractor = Substitute.For<IInfoExtractor>();
            var appUser = Substitute.For<IdentityUser>();
            
            userManager
                .FindByIdAsync(User.Identity.GetUserId())
                .Returns(Task.FromResult(appUser));
            appUser.//FirstOrDefault(p => p.ClaimType == MyClaimTypes.EntityUserId);

            var entrantId=132;
            var assignedHobbie = new List<AssignedHobbie>()
            {
                new AssignedHobbie(),
                new AssignedHobbie(),
                new AssignedHobbie(),
            };
            queryBuilder
                .For<Task<IEnumerable<AssignedHobbie>>>()
                .With(Arg.Is<GetAssignedHobbiesCriterion>(p=>p.EntrantId==entrantId))
                .ReturnsForAnyArgs(Task.FromResult(assignedHobbie));

            //Act
            var controller = new HobbieController(queryBuilder, commandBuilder, userManager,infoExtractor);

            //Assert

        }
    }
}
