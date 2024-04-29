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
using EasyTalkWeb.Controllers;
using Microsoft.AspNetCore.Mvc;
using EasyTalkWeb.Models.ViewModels;

namespace EasyTalk.Tests.ControllersTests
{
    public class LoginControllerTests
    {
        private readonly AppDbContext _dbContextMock;
        private readonly Mock<UserManager<Person>> _userManagerMock;
        private readonly Mock<IMailService> _mailServiceMock;
        private readonly Mock<SignInManager<Person>> _signInManagerMock;


        public LoginControllerTests()
        {
            _dbContextMock = Mock.Of<AppDbContext>();
            _mailServiceMock = new Mock<IMailService>();
            _userManagerMock = new Mock<UserManager<Person>>(
                /* IUserStore<TUser> store */Mock.Of<IUserStore<Person>>(),
                /* IOptions<IdentityOptions> optionsAccessor */null,
                /* IPasswordHasher<TUser> passwordHasher */null,
                /* IEnumerable<IUserValidator<TUser>> userValidators */null,
                /* IEnumerable<IPasswordValidator<TUser>> passwordValidators */null,
                /* ILookupNormalizer keyNormalizer */null,
                /* IdentityErrorDescriber errors */null,
                /* IServiceProvider services */null,
                /* ILogger<UserManager<TUser>> logger */null);
            _signInManagerMock = new Mock<SignInManager<Person>>(
                _userManagerMock.Object,
                /* IHttpContextAccessor contextAccessor */Mock.Of<IHttpContextAccessor>(),
                /* IUserClaimsPrincipalFactory<TUser> claimsFactory */Mock.Of<IUserClaimsPrincipalFactory<Person>>(),
                /* IOptions<IdentityOptions> optionsAccessor */null,
                /* ILogger<SignInManager<TUser>> logger */null,
                /* IAuthenticationSchemeProvider schemes */null,
                /* IUserConfirmation<TUser> confirmation */null);
        }

        [Fact]
        public void Login_Returns_ViewResult()
        {
            var controller = new LoginController(_signInManagerMock.Object, _userManagerMock.Object, _mailServiceMock.Object);

            var result = controller.Login();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Login_Returns_RedirectToActionResult_When_Succeeded()
        {
            // Arrange
            var controller = new LoginController(_signInManagerMock.Object, _userManagerMock.Object, _mailServiceMock.Object);
            //var loginViewModel = new LoginViewModel
            //{
            //    Email = "test@example.com",
            //    Password = "password",
            //    RememberMe = false
            //};

            var loginViewModel = new LoginViewModel();

            _userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new Person()); // Mock finding user by email
            _signInManagerMock.Setup(m => m.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success); // Mock successful sign-in

            // Act
            var result = await controller.Login(loginViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Profile", redirectToActionResult.ControllerName);
        }

        [Fact]
        public async Task Login_Returns_LoginView_When_Failed()
        {
            // Arrange
            var controller = new LoginController(_signInManagerMock.Object, _userManagerMock.Object, _mailServiceMock.Object);
            var loginViewModel = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "password",
                RememberMe = false
            };

            _userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new Person()); // Mock finding user by email
            _signInManagerMock.Setup(m => m.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed); // Mock failed sign-in

            // Act
            var result = await controller.Login(loginViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Login", viewResult.ViewName);
        }


    }
}
