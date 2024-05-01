using EasyTalkWeb.Controllers;
using EasyTalkWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Diagnostics;

namespace EasyTalk.Tests.ControllersTests
{
    public class HomeControllerTests
    {
        [Fact]
        public void Index_ReturnsViewResult()
        {
            var controller = new HomeController();

            var result = controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void Privacy_ReturnsViewResult()
        {
            var controller = new HomeController();

            var result = controller.Privacy();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }
        [Fact]
        public void Error_ReturnsErrorView()
        {
            var controller = new HomeController();
            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = "test_trace_identifier";
            var activity = new Activity("test_activity").Start();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(httpContext);
            var mockTempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            var result = controller.Error();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.NotNull(model);
            Assert.Equal(activity.Id ?? httpContext.TraceIdentifier, model.RequestId);
            activity.Stop();
        }
    }
}
