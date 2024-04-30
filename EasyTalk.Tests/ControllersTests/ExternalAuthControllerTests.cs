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
        // ====================================================================



        [Fact]
        public async Task ExternalAuthCallback_NewUserCreatedAndSignedIn_ReturnsChooseRoleView()
        {
            // Arrange
            var controller = new ExternalAuthController(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _freelancerRepositoryMock.Object,
                _clientRepositoryMock.Object);
            var info = new ExternalLoginInfo(new ClaimsPrincipal(), "Google", "provider", "key");
            _signInManagerMock.Setup(m => m.GetExternalLoginInfoAsync(It.IsAny<string>()))
                .ReturnsAsync(info);

            _signInManagerMock.Setup(m => m.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed); // Simulate new user scenario

            var user = new Person { Email = "test@example.com" };
            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<Person>()))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(m => m.AddLoginAsync(user, info))
                .ReturnsAsync(IdentityResult.Success);
            _signInManagerMock.Setup(m => m.SignInAsync(It.IsAny<Person>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await controller.ExternalAuthCallback();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("ChooseRole", viewResult.ViewName);
        }

        [Fact]
        public async Task ExternalAuthCallback_UserCreationFails_RedirectsToErrorAction()
        {
            // Arrange
            var controller = new ExternalAuthController(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _freelancerRepositoryMock.Object,
                _clientRepositoryMock.Object);
            var info = new ExternalLoginInfo(new ClaimsPrincipal(), "Google", "provider", "key");
            //_signInManagerMock.Setup(m => m.GetExternalLoginInfoAsync())
            //    .ReturnsAsync(info);

            _signInManagerMock.Setup(m => m.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed); // Simulate new user scenario

            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<Person>()))
                .ReturnsAsync(IdentityResult.Failed()); // Simulate user creation failure

            // Act
            var result = await controller.ExternalAuthCallback();

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectToActionResult.ActionName);
            Assert.Equal("ExternalAuth", redirectToActionResult.ControllerName);
        }

        //[Fact]
        //public async Task ExternalAuthCallback_UserIsLockedOut_RedirectsToLockout()
        //{
        //    // Arrange
        //    var controller = new ExternalAuthController(
        //        _userManagerMock.Object,
        //        _signInManagerMock.Object,
        //        _freelancerRepositoryMock.Object,
        //        _clientRepositoryMock.Object);
        //    var info = new ExternalLoginInfo(new ClaimsPrincipal(), "Google", "provider", "key");
        //    _signInManagerMock.Setup(m => m.GetExternalLoginInfoAsync())
        //        .ReturnsAsync(info);

        //    var signInResult = Microsoft.AspNetCore.Identity.SignInResult.Failed; // Simulate locked out scenario
        //    signInResult.IsLockedOut = true;
        //    _signInManagerMock.Setup(m => m.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, false))
        //        .ReturnsAsync(signInResult.IsLockedOut);

        //    // Act
        //    var result = await controller.ExternalAuthCallback();

        //    // Assert
        //    var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        //    Assert.Equal("Lockout", redirectToActionResult.ActionName);
        //}

        //[Fact]
        //public async Task ExternalAuthCallback_NoConditionsMet_ReturnsExternalAuthViewWithViewData()
        //{
        //    // Arrange
        //    var controller = CreateExternalAuthController();
        //    var info = new ExternalLoginInfo(new ClaimsPrincipal(), "Google", "provider", "key");
        //    _signInManagerMock.Setup(m => m.GetExternalLoginInfoAsync())
        //        .ReturnsAsync(info);

        //    var signInResult = SignInResult.Failed; // Simulate scenario where neither sign in succeeds nor user is created
        //    _signInManagerMock.Setup(m => m.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, false))
        //        .ReturnsAsync(signInResult);

        //    // Act
        //    var result = await controller.ExternalAuthCallback();

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    Assert.Equal("ExternalAuth", viewResult.ViewName);
        //    Assert.Equal(info.LoginProvider, viewResult.ViewData["LoginProvider"]);
        //    Assert.Equal(info.Principal.FindFirstValue(ClaimTypes.Email), viewResult.ViewData["Email"]);
        //    Assert.Null(viewResult.ViewData["ReturnUrl"]);
        //}
    }
}
