using Interfaces.CQRS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using OptimalEducation.Areas.EntrantUser.Controllers;
using OptimalEducation.DAL.Commands;
using OptimalEducation.DAL.Queries;
using OptimalEducation.DAL.ViewModels;
using OptimalEducation.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OptimalEducation.Controllers
{
    [TestClass]
    public class HobbieControllerTest
    {
        readonly ICommandBuilder commandBuilder = Substitute.For<ICommandBuilder>();
        readonly IQueryBuilder queryBuilder = Substitute.For<IQueryBuilder>();
        readonly IInfoExtractor infoExtractor = Substitute.For<IInfoExtractor>();
        readonly RequestContext requestContext;

        const int entrantId = 123;
        public HobbieControllerTest()
        {
            requestContext = SubstituteRequerstContext();

            infoExtractor.ExtractEntrantId("").ReturnsForAnyArgs(Task.FromResult(entrantId));
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
                    new ControllerContext(requestContext, controller);
            var task = controller.Index();
            task.Wait();
            var result = (ViewResult)task.Result;
            //Assert
            Assert.AreEqual(assignedHobbie, result.Model);
        }

        [TestMethod]
        public void Index_post_assignedHobbieList_sucsess_and_redirect_to_index()
        {
            var selectedHobbies=new string[10];

            commandBuilder
                .ExecuteAsync(new UpdateEntrantHobbieContext { EntrantId = entrantId, SelectedHobbies = selectedHobbies })
                .Returns(Task.Delay(1));
            
            //Act
            var controller = new HobbieController(queryBuilder, commandBuilder,infoExtractor);
            controller.ControllerContext =
                    new ControllerContext(requestContext, controller);
            var task = controller.Index(selectedHobbies);
            task.Wait();
            var result = ((RedirectToRouteResult)task.Result).RouteValues.Single();

            //Assert

            Assert.IsTrue(result.Key == "action" && result.Value=="Index");
        }
    }
}
