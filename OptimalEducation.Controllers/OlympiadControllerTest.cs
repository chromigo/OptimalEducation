using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Interfaces.CQRS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using OptimalEducation.Areas.EntrantUser.Controllers;
using OptimalEducation.DAL.Models;
using OptimalEducation.DAL.Queries;
using OptimalEducation.DAL.Queries.ParticipationInOlympiad;
using OptimalEducation.Helpers;

namespace OptimalEducation.UnitTest.Controllers
{
    [TestClass]
    public class OlympiadControllerTest
    {
        private const int EntrantId = 123;
        private readonly ICommandBuilder _commandBuilder = Substitute.For<ICommandBuilder>();
        private readonly IInfoExtractor _infoExtractor = Substitute.For<IInfoExtractor>();
        private readonly IQueryBuilder _queryBuilder = Substitute.For<IQueryBuilder>();
        private readonly RequestContext _requestContext;

        public OlympiadControllerTest()
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
        public void Index_get_correct_ParticipationInOlympiads_list()
        {
            //Arrange            
            IEnumerable<ParticipationInOlympiad> participationInOlympiad = new List<ParticipationInOlympiad>
            {
                new ParticipationInOlympiad(),
                new ParticipationInOlympiad(),
                new ParticipationInOlympiad()
            };
            _queryBuilder
                .For<Task<IEnumerable<ParticipationInOlympiad>>>()
                .With(Arg.Is<GetAllParticipationInOlympiadCriterion>(p => p.EntrantId == EntrantId))
                .Returns(Task.FromResult(participationInOlympiad));

            //Act
            var controller = new OlympiadController(_queryBuilder, _commandBuilder, _infoExtractor);
            controller.ControllerContext =
                new ControllerContext(_requestContext, controller);
            var task = controller.Index();
            task.Wait();
            var result = (ViewResult) task.Result;
            //Assert
            Assert.AreEqual(participationInOlympiad, result.Model);
        }

        [TestMethod]
        public void Details_return_HttpStatusCodeBadRequest_if_id_is_null()
        {
            //Act
            var controller = new OlympiadController(_queryBuilder, _commandBuilder, _infoExtractor);
            controller.ControllerContext =
                new ControllerContext(_requestContext, controller);
            var task = controller.Details(null);
            task.Wait();
            var result = ((HttpStatusCodeResult) task.Result);

            //Assert
            Assert.IsTrue(result.StatusCode == (int) HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void Details_return_correct_ParticipationInOlympiads_if_id_is_correct()
        {
            const int partInOlympId = 22;
            var partInOlympResult = new ParticipationInOlympiad();
            //Arrange
            _queryBuilder
                .For<Task<ParticipationInOlympiad>>()
                .With(
                    Arg.Is<GetCurrentParticipationInOlympiadCriterion>(
                        p => p.EntrantId == EntrantId && p.ParticipationInOlympiadId == partInOlympId))
                .Returns(Task.FromResult(partInOlympResult));

            //Act
            var controller = new OlympiadController(_queryBuilder, _commandBuilder, _infoExtractor);
            controller.ControllerContext =
                new ControllerContext(_requestContext, controller);
            var task = controller.Details(partInOlympId);
            task.Wait();
            var result = ((ViewResult) task.Result).Model;

            //Assert
            Assert.IsTrue(result == partInOlympResult);
        }

        [TestMethod]
        public void Details_return_HttpNotFound_if_not_found_element_in_out_collections()
        {
            const int partInOlympId = 23;
            //Arrange
            _queryBuilder
                .For<Task<ParticipationInOlympiad>>()
                .With(
                    Arg.Is<GetCurrentParticipationInOlympiadCriterion>(
                        p => p.EntrantId == EntrantId && p.ParticipationInOlympiadId == partInOlympId))
                .Returns(Task.FromResult<ParticipationInOlympiad>(null));

            //Act
            var controller = new OlympiadController(_queryBuilder, _commandBuilder, _infoExtractor);
            controller.ControllerContext =
                new ControllerContext(_requestContext, controller);
            var task = controller.Details(partInOlympId);
            task.Wait();

            //Assert
            Assert.IsTrue(task.Result is HttpNotFoundResult);
        }

        [TestMethod]
        public void Create_get_method_return_all_olympiads()
        {
            IEnumerable<Olympiad> olympiads = new List<Olympiad>
            {
                new Olympiad {Id = 1, Name = "o1"},
                new Olympiad {Id = 2, Name = "o2"},
                new Olympiad {Id = 3, Name = "o3"},
                new Olympiad {Id = 4, Name = "o4"}
            };
            //Arrange
            _queryBuilder
                .For<Task<IEnumerable<Olympiad>>>()
                .With(Arg.Any<GetAllOlympiadsCriterion>())
                .Returns(Task.FromResult(olympiads));

            //Act
            var controller = new OlympiadController(_queryBuilder, _commandBuilder, _infoExtractor);
            controller.ControllerContext = new ControllerContext(_requestContext, controller);
            var task = controller.Create();
            task.Wait();
            var result = ((ViewResult) task.Result).ViewBag.OlympiadId as SelectList;

            //Assert
            Assert.AreEqual(olympiads, result.Items);
        }
    }
}