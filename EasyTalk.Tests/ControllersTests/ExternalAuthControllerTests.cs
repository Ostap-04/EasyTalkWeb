using EasyTalkWeb.Identity.EmailHost;
using EasyTalkWeb.Models.Repositories;
using EasyTalkWeb.Models;
using EasyTalkWeb.Persistance;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EasyTalkWeb.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Security.Claims;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using EasyTalkWeb.Models.ViewModels;

namespace EasyTalk.Tests.ControllersTests
{
    public class ExternalAuthControllerTests
    {
        private readonly AppDbContext _dbContextMock;
        private readonly Mock<UserManager<Person>> _userManagerMock;
        private readonly Mock<SignInManager<Person>> _signInManagerMock;
        private readonly Mock<FreelancerRepository> _freelancerRepositoryMock;
        private readonly Mock<ClientRepository> _clientRepositoryMock;
        private readonly Mock<IUrlHelper> _urlHelperMock;

        public ExternalAuthControllerTests()
        {
            _dbContextMock = Mock.Of<AppDbContext>();
            _userManagerMock = new Mock<UserManager<Person>>(Mock.Of<IUserStore<Person>>(), null!, null!, null!, null!, null!, null!, null!, null!);
            _freelancerRepositoryMock = new Mock<FreelancerRepository>(_dbContextMock);
            _clientRepositoryMock = new Mock<ClientRepository>(_dbContextMock);
            _signInManagerMock = new Mock<SignInManager<Person>>(
                _userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<Person>>(), null!, null!, null!, null!);
            _urlHelperMock = new Mock<IUrlHelper>();
        }

        [Fact]
        public void ExternalAuth_Returns_ChallengeResult_With_Properties()
        {
            var provider = "Google";
            var redirectUrl = "fakeRedirectUrl";
            _urlHelperMock.Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns(redirectUrl);

            var properties = new AuthenticationProperties();
            _signInManagerMock.Setup(m => m.ConfigureExternalAuthenticationProperties(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(properties);

            var controller = new ExternalAuthController(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _freelancerRepositoryMock.Object,
                _clientRepositoryMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { RequestServices = new ServiceCollection().AddSingleton(_urlHelperMock.Object).BuildServiceProvider() }
                }
            };
            controller.Url = _urlHelperMock.Object;

            var result = controller.ExternalAuth(provider);

            var challengeResult = Assert.IsType<ChallengeResult>(result);
            Assert.Equal(properties, challengeResult.Properties);
            Assert.Equal(provider, challengeResult.AuthenticationSchemes.FirstOrDefault());
        }

        [Fact]
        public async Task ExternalAuthCallback_RemoteErrorNotNull_RedirectsToLogin()
        {
            var controller = new ExternalAuthController(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _freelancerRepositoryMock.Object,
                _clientRepositoryMock.Object);
            var remoteError = "SomeError";

            var result = await controller.ExternalAuthCallback(remoteError: remoteError);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task ExternalAuthCallback_GetExternalLoginInfoReturnsNull_RedirectsToLogin()
        {
            var controller = new ExternalAuthController(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _freelancerRepositoryMock.Object,
                _clientRepositoryMock.Object);
            
            var result = await controller.ExternalAuthCallback();

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task ExternalAuthCallback_SignInSucceeds_ReturnUrlIsLocal_RedirectsToProfileIndex()
        {
            // Arrange
            var controller = new ExternalAuthController(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _freelancerRepositoryMock.Object,
                _clientRepositoryMock.Object);
            controller.Url = _urlHelperMock.Object;

            var info = new ExternalLoginInfo(new ClaimsPrincipal(), "Google", "provider", "key");
            _signInManagerMock.Setup(m => m.GetExternalLoginInfoAsync(It.IsAny<string>()))
                .ReturnsAsync(info);

            _signInManagerMock.Setup(m => m.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Mock the behavior of Url.IsLocalUrl(returnUrl)
            _urlHelperMock.Setup(x => x.IsLocalUrl(It.IsAny<string>())).Returns(true);

            // Act
            var result = await controller.ExternalAuthCallback();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Profile/Index", redirectResult.Url);
        }

        [Fact]
        public async Task ExternalAuthCallback_SignInSucceeds_ReturnUrlIsNotLocal_RedirectsToHomeIndex()
        {
            // Arrange
            var controller = new ExternalAuthController(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _freelancerRepositoryMock.Object,
                _clientRepositoryMock.Object);
            controller.Url = _urlHelperMock.Object;

            var info = new ExternalLoginInfo(new ClaimsPrincipal(), "Google", "provider", "key");
            _signInManagerMock.Setup(m => m.GetExternalLoginInfoAsync(It.IsAny<string>()))
                .ReturnsAsync(info);

            _signInManagerMock.Setup(m => m.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Mock the behavior of Url.IsLocalUrl(returnUrl)
            _urlHelperMock.Setup(x => x.IsLocalUrl(It.IsAny<string>())).Returns(false);

            // Act
            var result = await controller.ExternalAuthCallback();

            
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            
        }
        
        [Fact]
        public async Task ExternalAuthCallback_NewUserCreatedAndSignedIn_ReturnsChooseRoleView()
        {
            // Arrange
            var controller = new ExternalAuthController(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _freelancerRepositoryMock.Object,
                _clientRepositoryMock.Object);
            var email = "test@example.com";
            var info = new ExternalLoginInfo(new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, email)
            }, "TestAuthentication")), "Google", "provider", "key");
            _signInManagerMock.Setup(m => m.GetExternalLoginInfoAsync(It.IsAny<string>()))
                .ReturnsAsync(info);

            _signInManagerMock.Setup(m => m.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed); // Simulate new user scenario

            var user = new Person { Email = email, UserName = email, CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, EmailConfirmed = true };
            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<Person>()))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(m => m.AddLoginAsync(It.IsAny<Person>(), It.IsAny<UserLoginInfo>()))
                .ReturnsAsync(IdentityResult.Success);
            _signInManagerMock.Setup(m => m.SignInAsync(It.IsAny<Person>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var result = await controller.ExternalAuthCallback();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("ChooseRole", viewResult.ViewName);
        }


        [Fact]
        public async Task ExternalAuthCallback_NewUserCreatingFailed_ReturnsRedirectToAction()
        {
            // Arrange
            var controller = new ExternalAuthController(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _freelancerRepositoryMock.Object,
                _clientRepositoryMock.Object);
            var email = "test@example.com";
            var info = new ExternalLoginInfo(new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, email)
            }, "TestAuthentication")), "Google", "provider", "key");
            _signInManagerMock.Setup(m => m.GetExternalLoginInfoAsync(It.IsAny<string>()))
                .ReturnsAsync(info);

            _signInManagerMock.Setup(m => m.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed); // Simulate new user scenario

            var user = new Person { Email = email, UserName = email, CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, EmailConfirmed = true };
            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<Person>()))
                .ReturnsAsync(IdentityResult.Failed());
            _userManagerMock.Setup(m => m.AddLoginAsync(It.IsAny<Person>(), It.IsAny<UserLoginInfo>()))
                .ReturnsAsync(IdentityResult.Success);
            _signInManagerMock.Setup(m => m.SignInAsync(It.IsAny<Person>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var result = await controller.ExternalAuthCallback();

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectToActionResult.ActionName);
            Assert.Equal("ExternalAuth", redirectToActionResult.ControllerName);
        }

        [Fact]
        public async Task ExternalAuthCallback_UserIsLockedOut_ReturnsLockoutView()
        {
            var controller = new ExternalAuthController(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _freelancerRepositoryMock.Object,
                _clientRepositoryMock.Object);
            var email = "test@example.com";
            var info = new ExternalLoginInfo(new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, email)
            }, "TestAuthentication")), "Google", "provider", "key");
            _signInManagerMock.Setup(m => m.GetExternalLoginInfoAsync(It.IsAny<string>()))
                .ReturnsAsync(info);

            _signInManagerMock.Setup(m => m.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut); // Simulate new user scenario

            var user = new Person { Email = email, UserName = email, CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, EmailConfirmed = true };
            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<Person>()))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(m => m.AddLoginAsync(It.IsAny<Person>(), It.IsAny<UserLoginInfo>()))
                .ReturnsAsync(IdentityResult.Failed());
            
            var result = await controller.ExternalAuthCallback();

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(Lockout), redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task ExternalAuthCallback_NotLockedOut_ReturnsExternalAuthViewWithViewData()
        {
            var controller = new ExternalAuthController(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _freelancerRepositoryMock.Object,
                _clientRepositoryMock.Object);
            var email = "test@example.com";
            var returnUrl = "/some-return-url";
            var info = new ExternalLoginInfo(new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, email)
            }, "TestAuthentication")), "Google", "provider", "key");
            _signInManagerMock.Setup(m => m.GetExternalLoginInfoAsync(It.IsAny<string>()))
                .ReturnsAsync(info);

            _signInManagerMock.Setup(m => m.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed); // Simulate new user scenario

            var user = new Person { Email = email, UserName = email, CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow, EmailConfirmed = true };
            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<Person>()))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(m => m.AddLoginAsync(It.IsAny<Person>(), It.IsAny<UserLoginInfo>()))
                .ReturnsAsync(IdentityResult.Failed());

            var result = await controller.ExternalAuthCallback(returnUrl);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("ExternalAuth", viewResult.ViewName);
            Assert.Equal(returnUrl, viewResult.ViewData["ReturnUrl"]);
            Assert.Equal(info.LoginProvider, viewResult.ViewData["LoginProvider"]);
            Assert.Equal(info.Principal.FindFirstValue(ClaimTypes.Email), viewResult.ViewData["Email"]);
        }

        [Fact]
        public void Error_ReturnsErrorView()
        {
            // Arrange
            var controller = new ExternalAuthController(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _freelancerRepositoryMock.Object,
                _clientRepositoryMock.Object);

            // Act
            var result = controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task ChooseRole_ReturnsChooseRoleView()
        {
            // Arrange
            var controller = new ExternalAuthController(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _freelancerRepositoryMock.Object,
                _clientRepositoryMock.Object);

            // Act
            var result = await controller.ChooseRole();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task GetRole_UserRoleAssignedAsClient_AddsToClientRepositoryAndRedirectsToProfileIndex()
        {
            var controller = new ExternalAuthController(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _freelancerRepositoryMock.Object,
                _clientRepositoryMock.Object);
            controller.Url = _urlHelperMock.Object;
            _urlHelperMock.Setup(x => x.IsLocalUrl(It.IsAny<string>())).Returns(true);

            // Arrange
            var model = new RegisterViewModel { Role = "Client" };
            var user = new Person { Id = Guid.NewGuid() };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManagerMock.Setup(m => m.AddToRoleAsync(user, model.Role)).ReturnsAsync(IdentityResult.Success);
            var info = new ExternalLoginInfo(new ClaimsPrincipal(), "Google", "provider", "key");
            _signInManagerMock.Setup(m => m.GetExternalLoginInfoAsync(It.IsAny<string>())).ReturnsAsync(info);
            _signInManagerMock.Setup(m => m.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Act
            var result = await controller.GetRole(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Profile/Index", redirectResult.Url);

        }

        [Fact]
        public async Task GetRole_UserRoleAssignedAsFreelancer_AddsToFreelancerRepositoryAndRedirectsToProfileIndex()
        {
            var controller = new ExternalAuthController(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _freelancerRepositoryMock.Object,
                _clientRepositoryMock.Object);
            controller.Url = _urlHelperMock.Object;
            _urlHelperMock.Setup(x => x.IsLocalUrl(It.IsAny<string>())).Returns(true);

            // Arrange
            var model = new RegisterViewModel { Role = "Freelancer" };
            var user = new Person { Id = Guid.NewGuid() };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManagerMock.Setup(m => m.AddToRoleAsync(user, model.Role)).ReturnsAsync(IdentityResult.Success);
            var info = new ExternalLoginInfo(new ClaimsPrincipal(), "Google", "provider", "key");
            _signInManagerMock.Setup(m => m.GetExternalLoginInfoAsync(It.IsAny<string>())).ReturnsAsync(info);
            _signInManagerMock.Setup(m => m.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Act
            var result = await controller.GetRole(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Profile/Index", redirectResult.Url);
        }

        [Fact]
        public async Task GetRole_ExternalLoginInfoIsNull_RedirectsToLogin()
        {
            var controller = new ExternalAuthController(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _freelancerRepositoryMock.Object,
                _clientRepositoryMock.Object);

            var model = new RegisterViewModel { Role = "Freelancer" };
            var user = new Person { Id = Guid.NewGuid() };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManagerMock.Setup(m => m.AddToRoleAsync(user, model.Role)).ReturnsAsync(IdentityResult.Success);
            var info = new ExternalLoginInfo(new ClaimsPrincipal(), "Google", "provider", "key");
            _signInManagerMock.Setup(m => m.GetExternalLoginInfoAsync(It.IsAny<string>())).ReturnsAsync((ExternalLoginInfo)null);
            // Act
            var result = await controller.GetRole(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task GetRole_UserIsLockedOut_RedirectsToLockout()
        {
            var controller = new ExternalAuthController(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _freelancerRepositoryMock.Object,
                _clientRepositoryMock.Object);
            controller.Url = _urlHelperMock.Object;
            _urlHelperMock.Setup(x => x.IsLocalUrl(It.IsAny<string>())).Returns(true);

            // Arrange
            var model = new RegisterViewModel { Role = "Freelancer" };
            var user = new Person { Id = Guid.NewGuid() };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManagerMock.Setup(m => m.AddToRoleAsync(user, model.Role)).ReturnsAsync(IdentityResult.Success);
            var info = new ExternalLoginInfo(new ClaimsPrincipal(), "Google", "provider", "key");
            _signInManagerMock.Setup(m => m.GetExternalLoginInfoAsync(It.IsAny<string>())).ReturnsAsync(info);
            _signInManagerMock.Setup(m => m.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);

            // Act
            var result = await controller.GetRole(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Lockout", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task GetRole_DefaultCase_ReturnsExternalAuthViewWithViewData()
        {
            // Arrange
            var controller = new ExternalAuthController(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _freelancerRepositoryMock.Object,
                _clientRepositoryMock.Object);
            controller.Url = _urlHelperMock.Object;
            _urlHelperMock.Setup(x => x.IsLocalUrl(It.IsAny<string>())).Returns(true);

            var model = new RegisterViewModel { Role = "Freelancer" };
            var user = new Person { Id = Guid.NewGuid() };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManagerMock.Setup(m => m.AddToRoleAsync(user, model.Role)).ReturnsAsync(IdentityResult.Success);
            var expectedEmail = "test@example.com";

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, expectedEmail)
            }));
            var info = new ExternalLoginInfo(principal, "Google", "provider", "key");
            _signInManagerMock.Setup(m => m.GetExternalLoginInfoAsync(It.IsAny<string>())).ReturnsAsync(info); // Corrected setup
            _signInManagerMock.Setup(m => m.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = principal } };

            // Act
            var result = await controller.GetRole(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("ExternalAuth", viewResult.ViewName);
            Assert.Equal(info.LoginProvider, viewResult.ViewData["LoginProvider"]);
            Assert.Equal(expectedEmail, viewResult.ViewData["Email"]);
        }





    }
}
