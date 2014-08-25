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
using System.Security.Principal;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;

namespace OptimalEducation.Controllers
{
    [TestClass]
    public class HobbieControllerTest
    {

        [TestMethod]
        public void Index_get_correct_assignedHobbie_list()
        {
            //Arrange
            var commandBuilder = Substitute.For<ICommandBuilder>();
            var queryBuilder = Substitute.For<IQueryBuilder>();
            var infoExtractor = Substitute.For<IInfoExtractor>();

            //how to test iprincipal,iidentity and asp etc http://stackoverflow.com/questions/1314370/how-to-setup-iprincipal-for-a-mockup
            var user = Substitute.For<IPrincipal>();
            var identity = Substitute.For<IIdentity>();
            identity.AuthenticationType.Returns("Type");
            identity.IsAuthenticated.Returns(true);
            identity.Name.Returns("Name");
            user.Identity.Returns(identity);

            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.User.Returns(user);
            var reqContext = new RequestContext(httpContext, new RouteData());

            var entrantId=123;

            infoExtractor.ExtractEntrantId("").ReturnsForAnyArgs(Task.FromResult(entrantId));
            
            IEnumerable<AssignedHobbie> assignedHobbie = new List<AssignedHobbie>()
            {
                new AssignedHobbie(),
                new AssignedHobbie(),
                new AssignedHobbie(),
            };
            queryBuilder
                .For<Task<IEnumerable<AssignedHobbie>>>()
                .With(Arg.Is<GetAssignedHobbiesCriterion>(p=>p.EntrantId==entrantId))
                .Returns(Task.FromResult(assignedHobbie));

            //Act
            var controller = new HobbieController(queryBuilder, commandBuilder,infoExtractor);
            controller.ControllerContext =
                    new ControllerContext(reqContext, controller);
            var task = controller.Index();
            task.Wait();
            var result = (ViewResult)task.Result;
            //Assert
            Assert.AreEqual(assignedHobbie, result.Model);
        }
    }
}
