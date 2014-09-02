using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Routing;
using OptimalEducation.Helpers;
using Interfaces.CQRS;
using NSubstitute;
using System.Web;
using System.Security.Principal;
using OptimalEducation.DAL.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;
using OptimalEducation.DAL.Models;
using OptimalEducation.DAL.ViewModels;
using OptimalEducation.Areas.EntrantUser.Controllers;
using System.Web.Mvc;
using System.Net;

namespace OptimalEducation.Controllers
{
    [TestClass]
    public class OlympiadControllerTest
    {
        readonly ICommandBuilder commandBuilder = Substitute.For<ICommandBuilder>();
        readonly IQueryBuilder queryBuilder = Substitute.For<IQueryBuilder>();
        readonly IInfoExtractor infoExtractor = Substitute.For<IInfoExtractor>();
        readonly RequestContext requestContext;

        const int entrantId = 123;

        public OlympiadControllerTest()
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
        public void Index_get_correct_ParticipationInOlympiads_list()
        {
            //Arrange            
            IEnumerable<ParticipationInOlympiad> participationInOlympiad = new List<ParticipationInOlympiad>()
            {
                new ParticipationInOlympiad(),
                new ParticipationInOlympiad(),
                new ParticipationInOlympiad(),
            };
            queryBuilder
                .For<Task<IEnumerable<ParticipationInOlympiad>>>()
                .With(Arg.Is<GetAllParticipationInOlympiadCriterion>(p => p.EntrantId == entrantId))
                .Returns(Task.FromResult(participationInOlympiad));

            //Act
            var controller = new OlympiadController(queryBuilder, commandBuilder, infoExtractor);
            controller.ControllerContext =
                    new ControllerContext(requestContext, controller);
            var task = controller.Index();
            task.Wait();
            var result = (ViewResult)task.Result;
            //Assert
            Assert.AreEqual(participationInOlympiad, result.Model);
        }

        [TestMethod]
        public void Details_return_HttpStatusCodeBadRequest_if_id_is_null()
        {
            //Act
            var controller = new OlympiadController(queryBuilder, commandBuilder, infoExtractor);
            controller.ControllerContext =
                    new ControllerContext(requestContext, controller);
            var task = controller.Details(null);
            task.Wait();
            var result = ((HttpStatusCodeResult)task.Result);

            //Assert
            Assert.IsTrue(result.StatusCode== (int)HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void Details_return_correct_ParticipationInOlympiads_if_id_is_correct()
        {
            const int partInOlympId= 22;
            var partInOlympResult = new ParticipationInOlympiad();
            //Arrange
            queryBuilder
                .For<Task<ParticipationInOlympiad>>()
                .With(Arg.Is<GetCurrentParticipationInOlympiadCriterion>(p=>p.EntrantId==entrantId && p.ParticipationInOlympiadId == partInOlympId))
                .Returns(Task.FromResult(partInOlympResult));

            //Act
            var controller = new OlympiadController(queryBuilder, commandBuilder, infoExtractor);
            controller.ControllerContext =
                    new ControllerContext(requestContext, controller);
            var task = controller.Details(partInOlympId);
            task.Wait();
            var result = ((ViewResult)task.Result).Model;

            //Assert
            Assert.IsTrue(result == partInOlympResult);
        }

        [TestMethod]
        public void Details_return_HttpNotFound_if_not_found_element_in_out_collections()
        {
            const int partInOlympId = 23;
            ParticipationInOlympiad partInOlympResult=null;
            //Arrange
            queryBuilder
                .For<Task<ParticipationInOlympiad>>()
                .With(Arg.Is<GetCurrentParticipationInOlympiadCriterion>(p => p.EntrantId == entrantId && p.ParticipationInOlympiadId == partInOlympId))
                .Returns(Task.FromResult(partInOlympResult));

            //Act
            var controller = new OlympiadController(queryBuilder, commandBuilder, infoExtractor);
            controller.ControllerContext =
                    new ControllerContext(requestContext, controller);
            var task = controller.Details(partInOlympId);
            task.Wait();

            //Assert
            Assert.IsTrue(task.Result is HttpNotFoundResult);
        }
    }
}
