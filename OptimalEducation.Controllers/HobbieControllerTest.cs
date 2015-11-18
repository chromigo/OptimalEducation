using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Interfaces.CQRS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using OptimalEducation.Areas.EntrantUser.Controllers;
using OptimalEducation.DAL.Commands;
using OptimalEducation.DAL.Queries;
using OptimalEducation.DAL.ViewModels;
using OptimalEducation.Helpers;

namespace OptimalEducation.UnitTest.Controllers
{
    [TestClass]
    public class HobbieControllerTest
    {
        private const int EntrantId = 123;
        private readonly ICommandBuilder _commandBuilder = Substitute.For<ICommandBuilder>();
        private readonly IInfoExtractor _infoExtractor = Substitute.For<IInfoExtractor>();
        private readonly IQueryBuilder _queryBuilder = Substitute.For<IQueryBuilder>();
        private readonly RequestContext _requestContext;

        public HobbieControllerTest()
        {
            _requestContext = SubstituteRequerstContext();

            _infoExtractor.ExtractEntrantId("").ReturnsForAnyArgs(Task.FromResult(EntrantId));
        }

        private RequestContext SubstituteRequerstContext()
        {
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
            return reqContext;
        }

        [TestMethod]
        public void Index_get_correct_assignedHobbie_list()
        {
            //Arrange            
            IEnumerable<AssignedHobbie> assignedHobbie = new List<AssignedHobbie>
            {
                new AssignedHobbie(),
                new AssignedHobbie(),
                new AssignedHobbie()
            };
            _queryBuilder
                .For<Task<IEnumerable<AssignedHobbie>>>()
                .With(Arg.Is<GetAssignedHobbiesCriterion>(p => p.EntrantId == EntrantId))
                .Returns(Task.FromResult(assignedHobbie));

            //Act
            var controller = new HobbieController(_queryBuilder, _commandBuilder, _infoExtractor);
            controller.ControllerContext =
                new ControllerContext(_requestContext, controller);
            var task = controller.Index();
            task.Wait();
            var result = (ViewResult) task.Result;
            //Assert
            Assert.AreEqual(assignedHobbie, result.Model);
        }

        [TestMethod]
        public void Index_post_assignedHobbieList_sucsess_and_redirect_to_index()
        {
            var selectedHobbies = new string[10];

            _commandBuilder
                .ExecuteAsync(new UpdateEntrantHobbieContext {EntrantId = EntrantId, SelectedHobbies = selectedHobbies})
                .Returns(Task.Delay(1));

            //Act
            var controller = new HobbieController(_queryBuilder, _commandBuilder, _infoExtractor);
            controller.ControllerContext =
                new ControllerContext(_requestContext, controller);
            var task = controller.Index(selectedHobbies);
            task.Wait();
            var result = ((RedirectToRouteResult) task.Result).RouteValues.Single();

            //Assert

            Assert.IsTrue(result.Key == "action" && (string) result.Value == "Index");
        }
    }
}