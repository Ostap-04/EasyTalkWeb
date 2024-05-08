using EasyTalkWeb.Identity.EmailHost;
using EasyTalkWeb.Models;
using EasyTalkWeb.Persistance;
using Microsoft.AspNetCore.Identity;
using Moq;
using Microsoft.AspNetCore.Http;
using EasyTalkWeb.Controllers;
using Microsoft.AspNetCore.Mvc;
using EasyTalkWeb.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace EasyTalk.Tests.ControllersTests
{
    public class LoginControllerTests
    {
        private readonly Mock<UserManager<Person>> _userManagerMock;
        private readonly Mock<IMailService> _mailServiceMock;
        private readonly Mock<SignInManager<Person>> _signInManagerMock;
        private readonly Mock<IUrlHelper> _urlHelperMock;

        public LoginControllerTests()
        {
            _mailServiceMock = new Mock<IMailService>();
            _userManagerMock = new Mock<UserManager<Person>>(
                /* IUserStore<TUser> store */Mock.Of<IUserStore<Person>>(),
                /* IOptions<IdentityOptions> optionsAccessor */null!,
                /* IPasswordHasher<TUser> passwordHasher */null!,
                /* IEnumerable<IUserValidator<TUser>> userValidators */null!,
                /* IEnumerable<IPasswordValidator<TUser>> passwordValidators */null!,
                /* ILookupNormalizer keyNormalizer */null!,
                /* IdentityErrorDescriber errors */null!,
                /* IServiceProvider services */null!,
                /* ILogger<UserManager<TUser>> logger */null!);
            _signInManagerMock = new Mock<SignInManager<Person>>(
                _userManagerMock.Object,
                /* IHttpContextAccessor contextAccessor */Mock.Of<IHttpContextAccessor>(),
                /* IUserClaimsPrincipalFactory<TUser> claimsFactory */Mock.Of<IUserClaimsPrincipalFactory<Person>>(),
                /* IOptions<IdentityOptions> optionsAccessor */null!,
                /* ILogger<SignInManager<TUser>> logger */null!,
                /* IAuthenticationSchemeProvider schemes */null!,
                /* IUserConfirmation<TUser> confirmation */null!);
            _urlHelperMock = new Mock<IUrlHelper>();
        }

        [Fact]
        public void Login_Returns_ViewResult()
        {
            var controller = new LoginController(_signInManagerMock.Object, _userManagerMock.Object, _mailServiceMock.Object);

            var result = controller.Login();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Login_Returns_ViewResult_When_ModelState_IsNotValid()
        {
            var controller = new LoginController(_signInManagerMock.Object, _userManagerMock.Object, _mailServiceMock.Object);
            controller.ModelState.AddModelError("Email", "Email is required");

            var loginViewModel = new LoginViewModel
            {
                Email = null,
                Password = "Password1@",
                RememberMe = false
            };
            
            var result = await controller.Login(loginViewModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Login", viewResult.ViewName);
        }

        [Fact]
        public async Task Login_Returns_RedirectToActionResult_When_Succeeded()
        {
            var controller = new LoginController(_signInManagerMock.Object, _userManagerMock.Object, _mailServiceMock.Object);
            var loginViewModel = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "password",
                RememberMe = false
            };

            _userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new Person());
            _signInManagerMock.Setup(m => m.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var result = await controller.Login(loginViewModel);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Profile", redirectToActionResult.ControllerName);
        }

        [Fact]
        public async Task Login_Returns_LoginView_When_Failed()
        {
            var controller = new LoginController(_signInManagerMock.Object, _userManagerMock.Object, _mailServiceMock.Object);
            var loginViewModel = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "password",
                RememberMe = false
            };

            _userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new Person());
            _signInManagerMock.Setup(m => m.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var result = await controller.Login(loginViewModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.True(controller.ModelState.TryGetValue("Login", out var modelStateEntry));
            var errorMessage = Assert.Single(modelStateEntry.Errors)?.ErrorMessage;
            Assert.Equal("Failed to login", errorMessage);

            Assert.True(controller.ModelState.TryGetValue("Email", out modelStateEntry));
            errorMessage = Assert.Single(modelStateEntry.Errors)?.ErrorMessage;
            Assert.Equal("Email is unconfirmed, please confirm it first", errorMessage);

        }


        [Fact]
        public async Task Login_Returns_LoginView_When_LockedOut()
        {
            var controller = new LoginController(_signInManagerMock.Object, _userManagerMock.Object, _mailServiceMock.Object);
            var loginViewModel = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "password",
                RememberMe = false
            };

            _userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new Person());
            _signInManagerMock.Setup(m => m.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);

            var result = await controller.Login(loginViewModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.True(controller.ModelState.TryGetValue("Login", out var modelStateEntry));
            var errorMessage = Assert.Single(modelStateEntry.Errors)?.ErrorMessage;
            Assert.Equal("You are locked out", errorMessage);

            Assert.True(controller.ModelState.TryGetValue("Email", out modelStateEntry));
            errorMessage = Assert.Single(modelStateEntry.Errors)?.ErrorMessage;
            Assert.Equal("Email is unconfirmed, please confirm it first", errorMessage);
        }

        [Fact]
        public async Task Login_SignOut_Returns_RedirectToActionResult()
        {
            var controller = new LoginController(_signInManagerMock.Object, _userManagerMock.Object, _mailServiceMock.Object);
                
            _userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new Person());
            _signInManagerMock.Setup(m => m.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);

            var result = await controller.SignOut();

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
        }

        [Fact]
        public void LoginExternally_Returns_ChallengeResult()
        {
            var controller = new LoginController(_signInManagerMock.Object, _userManagerMock.Object, _mailServiceMock.Object);

            var provider = "Google";
            var properties = new AuthenticationProperties();

            _signInManagerMock.Setup(m => m.ConfigureExternalAuthenticationProperties(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(properties);

            var fakeRedirectUri = "http://example.com/Account/ExternalLoginCallback";

            _urlHelperMock.Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns(fakeRedirectUri);

            controller.Url = _urlHelperMock.Object;

            var result = controller.LoginExternally(provider);

            var challengeResult = Assert.IsType<ChallengeResult>(result);
            Assert.Equal(provider, challengeResult.AuthenticationSchemes.FirstOrDefault());
            Assert.Equal(fakeRedirectUri, properties.RedirectUri);
        }

        [Fact]
        public void ForgotPassword_Returns_ViewResult()
        {
            var controller = new LoginController(_signInManagerMock.Object, _userManagerMock.Object, _mailServiceMock.Object);

            var result = controller.ForgotPassword();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task ForgotPassword_InvalidModel_Returns_ViewResult()
        {
            var invalidModel = new ForgotPasswordViewModel
            {
                Email = null!
            };

            var controller = new LoginController(_signInManagerMock.Object, _userManagerMock.Object, _mailServiceMock.Object);

            controller.ModelState.AddModelError("Email", "Email is required");

            var result = await controller.ForgotPassword(invalidModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(invalidModel, viewResult.Model);
        }

        [Fact]
        public async Task ForgotPassword_ValidModel_Returns_RedirectToAction()
        {
            var model = new ForgotPasswordViewModel
            {
                Email = "test@example.com"
            };

            _userManagerMock.Setup(m => m.FindByEmailAsync(model.Email))
                .ReturnsAsync(new Person { Email = model.Email });

            _userManagerMock.Setup(m => m.IsEmailConfirmedAsync(It.IsAny<Person>()))
                .ReturnsAsync(true);

            _userManagerMock.Setup(m => m.GeneratePasswordResetTokenAsync(It.IsAny<Person>()))
                .ReturnsAsync("fakeToken");

            var callbackUrl = "http://example.com/Login/ResetPassword?token=fakeToken&email=test@example.com";
            _urlHelperMock.Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns(callbackUrl);

            var controller = new LoginController(_signInManagerMock.Object, _userManagerMock.Object, _mailServiceMock.Object);
            controller.Url = _urlHelperMock.Object;
            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = new ServiceCollection().BuildServiceProvider();
            httpContext.Request.Host = new HostString("https://localhost:7049/");
            httpContext.Request.Scheme = "http";
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            
            var result = await controller.ForgotPassword(model);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ForgotPasswordConfirmation", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task ForgotPassword_User_Null_Returns_RedirectToAction()
        {
            var model = new ForgotPasswordViewModel
            {
                Email = "test@example.com"
            };

            _userManagerMock.Setup(m => m.FindByEmailAsync(model.Email))
                .ReturnsAsync((Person)null!);

            _userManagerMock.Setup(m => m.IsEmailConfirmedAsync(It.IsAny<Person>()))
                .ReturnsAsync(true);

            var controller = new LoginController(_signInManagerMock.Object, _userManagerMock.Object, _mailServiceMock.Object);
           
            var result = await controller.ForgotPassword(model);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ForgotPasswordConfirmation", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task ForgotPassword_EmailIsNotConfirmed_Returns_RedirectToAction()
        {
            var model = new ForgotPasswordViewModel
            {
                Email = "test@example.com"
            };

            _userManagerMock.Setup(m => m.FindByEmailAsync(model.Email))
                .ReturnsAsync(new Person { Email = model.Email });

            _userManagerMock.Setup(m => m.IsEmailConfirmedAsync(It.IsAny<Person>()))
                .ReturnsAsync(false);

            var controller = new LoginController(_signInManagerMock.Object, _userManagerMock.Object, _mailServiceMock.Object);

            var result = await controller.ForgotPassword(model);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ForgotPasswordConfirmation", redirectToActionResult.ActionName);
        }

        [Fact]
        public void ForgotPasswordConfirmation_Returns_ViewResult()
        {
            var controller = new LoginController(_signInManagerMock.Object, _userManagerMock.Object, _mailServiceMock.Object);

            var result = controller.ForgotPasswordConfirmation();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void ResetPassword_Returns_ViewResult_With_Model()
        {
            var token = "fakeToken";
            var email = "test@example.com";

            var controller = new LoginController(_signInManagerMock.Object, _userManagerMock.Object, _mailServiceMock.Object);

            var result = controller.ResetPassword(token, email);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);

            var model = Assert.IsType<ResetPasswordViewModel>(viewResult.Model);
            Assert.Equal(token, model.Token);
            Assert.Equal(email, model.Email);
        }

        [Fact]
        public async Task ResetPassword_ValidModel_Successful_PasswordReset_RedirectsToProfileIndex()
        {
            var model = new ResetPasswordViewModel
            {
                Email = "test@example.com",
                Token = "fakeToken",
                Password = "newPassword"
            };

            var user = new Person { Email = model.Email };
            _userManagerMock.Setup(m => m.FindByEmailAsync(model.Email))
                .ReturnsAsync(user);

            var result = IdentityResult.Success;
            _userManagerMock.Setup(m => m.ResetPasswordAsync(user, model.Token, model.Password))
                .ReturnsAsync(result);

            var controller = new LoginController(_signInManagerMock.Object, _userManagerMock.Object, _mailServiceMock.Object);

            var actionResult = await controller.ResetPassword(model);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(actionResult);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Profile", redirectToActionResult.ControllerName);
        }

        [Fact]
        public async Task ResetPassword_InvalidModel_Returns_ViewResult()
        {
            var invalidModel = new ResetPasswordViewModel();
            var controller = new LoginController(_signInManagerMock.Object, _userManagerMock.Object, _mailServiceMock.Object);
            controller.ModelState.AddModelError("Password", "Password is required");

            var actionResult = await controller.ResetPassword(invalidModel);

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task ResetPassword_UserNotFound_RedirectsToResetPasswordConfirmation()
        {
            var model = new ResetPasswordViewModel
            {
                Email = "nonExistingUser@example.com",
                Token = "fakeToken",
                Password = "newPassword"
            };

            _userManagerMock.Setup(m => m.FindByEmailAsync(model.Email))
                .ReturnsAsync((Person)null!);

            var controller = new LoginController(_signInManagerMock.Object, _userManagerMock.Object, _mailServiceMock.Object);

            var actionResult = await controller.ResetPassword(model);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(actionResult);
            Assert.Equal("ResetPasswordConfirmation", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task ResetPassword_PasswordResetFailed_Returns_ViewResultWithErrors()
        {
            var model = new ResetPasswordViewModel
            {
                Email = "test@example.com",
                Token = "fakeToken",
                Password = "newPassword"
            };

            var user = new Person { Email = model.Email };
            _userManagerMock.Setup(m => m.FindByEmailAsync(model.Email))
                .ReturnsAsync(user);

            var errors = new List<IdentityError> { new() { Description = "Reset failed" } };
            var failedResult = IdentityResult.Failed(errors.ToArray());
            _userManagerMock.Setup(m => m.ResetPasswordAsync(user, model.Token, model.Password))
                .ReturnsAsync(failedResult);

            var controller = new LoginController(_signInManagerMock.Object, _userManagerMock.Object, _mailServiceMock.Object);

            var actionResult = await controller.ResetPassword(model);

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.Null(viewResult.ViewName);

            Assert.True(controller.ModelState.ErrorCount > 0);
        }

        [Fact]
        public void ResetPasswordConfirmation_Returns_ViewResult()
        {
            var controller = new LoginController(_signInManagerMock.Object, _userManagerMock.Object, _mailServiceMock.Object);

            var result = controller.ResetPasswordConfirmation();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }
    }
}
