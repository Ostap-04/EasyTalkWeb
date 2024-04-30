using EasyTalkWeb.Controllers;
using EasyTalkWeb.Identity.EmailHost;
using EasyTalkWeb.Models.Repositories;
using EasyTalkWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using EasyTalkWeb.Persistance;
using EasyTalkWeb.Models.ViewModels;
using EasyTalkWeb.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace EasyTalk.Tests.Controllers
{
    public class RegisterControllerTests
    {
        private readonly AppDbContext _dbContextMock;
        private readonly Mock<UserManager<Person>> _userManagerMock;
        private readonly Mock<IMailService> _mailServiceMock;
        private readonly Mock<FreelancerRepository> _freelancerRepositoryMock;
        private readonly Mock<ClientRepository> _clientRepositoryMock;
        private readonly Mock<JobPostRepository> _jobPostRepositoryMock;

        public RegisterControllerTests()
        {
            _dbContextMock = Mock.Of<AppDbContext>();
            _userManagerMock = new Mock<UserManager<Person>>(Mock.Of<IUserStore<Person>>(), null!, null!, null!, null!, null!, null!, null!, null!);
            _mailServiceMock = new Mock<IMailService>();
            _freelancerRepositoryMock = new Mock<FreelancerRepository>(_dbContextMock);
            _clientRepositoryMock = new Mock<ClientRepository>(_dbContextMock);
            _jobPostRepositoryMock = new Mock<JobPostRepository>(_dbContextMock);

        }

        [Fact]
        public void Register_Returns_ViewResult()
        {
            var controller = new RegisterController(_userManagerMock.Object, _mailServiceMock.Object, _freelancerRepositoryMock.Object, _clientRepositoryMock.Object);

            var result = controller.Register();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Register_ValidModel_Returns_RedirectToAction()
        {
            var services = new ServiceCollection();
            services.AddMvc();
            services.AddHttpContextAccessor();
            var serviceProvider = services.BuildServiceProvider();

            var controller = new RegisterController(_userManagerMock.Object, _mailServiceMock.Object, _freelancerRepositoryMock.Object, _clientRepositoryMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        RequestServices = serviceProvider
                    }
                }
            };

            var model = new RegisterViewModel
            {
                Gender = Gender.Male,
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Location = "New York",
                Email = "john@example.com",
                Password = "Password123",
                Role = "Client"
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<Person>(), It.IsAny<string>()))
               .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<Person>(), It.IsAny<string>()))
                           .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<Person>()))
                           .ReturnsAsync("fakeToken");

            _mailServiceMock.Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
               .Returns(true);
            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns("http://example.com");

            controller.Url = urlHelper.Object;

            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = new ServiceCollection().BuildServiceProvider();
            httpContext.Request.Host = new HostString("https://localhost:7049/");
            httpContext.Request.Scheme = "http";
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.Register(model);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ConfirmEmail", redirectResult.ActionName);
            Assert.Equal("Email", redirectResult.ControllerName);
        }

        [Fact]
        public async Task Register_InvalidModel_Returns_ViewResult_With_ErrorMessages()
        {
            var controller = new RegisterController(_userManagerMock.Object, _mailServiceMock.Object, _freelancerRepositoryMock.Object, _clientRepositoryMock.Object);
            controller.ModelState.AddModelError("Email", "Email is required");

            var model = new RegisterViewModel();

            var result = await controller.Register(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Register", viewResult.ViewName);

            Assert.True(controller.ModelState.TryGetValue("Email", out var modelStateEntry));
            var errorMessage = Assert.Single(modelStateEntry.Errors)?.ErrorMessage;
            Assert.Equal("Email is required", errorMessage);
        }

        [Fact]
        public async Task Register_CreateAsyncFails_Returns_ViewResult_With_ErrorMessages()
        {
            var controller = new RegisterController(_userManagerMock.Object, _mailServiceMock.Object, _freelancerRepositoryMock.Object, _clientRepositoryMock.Object);
            var model = new RegisterViewModel();
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<Person>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error creating user" }));

            var result = await controller.Register(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Register", viewResult.ViewName);

            var modelStateErrors = controller.ModelState[string.Empty]?.Errors;
            Assert.Single(modelStateErrors!);
            Assert.Equal("Error creating user", modelStateErrors?[0].ErrorMessage);
        }

        [Fact]
        public async Task Register_EmailResponseFalse_Returns_ViewResult_With_ErrorMessages()
        {
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddAuthorization();
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddMvc();
            var app = builder.Build();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllers();

            var controller = new RegisterController(_userManagerMock.Object, _mailServiceMock.Object, _freelancerRepositoryMock.Object, _clientRepositoryMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        RequestServices = app.Services
                    }
                }
            };

            var model = new RegisterViewModel
            {
                Gender = Gender.Male,
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Location = "New York",
                Email = "john@example.com",
                Password = "Password123",
                Role = "Freelancer"
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<Person>(), It.IsAny<string>()))
               .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<Person>(), It.IsAny<string>()))
                           .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<Person>()))
                           .ReturnsAsync("fakeToken");

            _mailServiceMock.Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
               .Returns(false);
            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns("http://example.com");

            controller.Url = urlHelper.Object;

            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = new ServiceCollection().BuildServiceProvider();
            httpContext.Request.Host = new HostString("https://localhost:7049/");
            httpContext.Request.Scheme = "http";
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            var result = await controller.Register(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Register", viewResult.ViewName); // Assert that the action returns the Register view

            var modelStateErrors = controller.ModelState[string.Empty]?.Errors;
            Assert.Single(modelStateErrors!);
            Assert.Equal("Problem with email confirmation", modelStateErrors?[0].ErrorMessage);
        }
    }
}
