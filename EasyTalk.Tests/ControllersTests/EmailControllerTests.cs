using EasyTalkWeb.Controllers;
using EasyTalkWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EasyTalk.Tests.ControllersTests
{
    public class EmailControllerTests
    {
        private readonly Mock<UserManager<Person>> _userManagerMock;

        public EmailControllerTests()
        {
            _userManagerMock = new Mock<UserManager<Person>>(
                Mock.Of<IUserStore<Person>>(), null!, null!, null!, null!, null!, null!, null!, null!);
        }

        [Fact]
        public async Task EmailConfirmed_UserFoundAndConfirmed_Returns_EmailConfirmedView()
        {
            var token = "fakeToken";
            var email = "test@example.com";
            var user = new Person { Email = email };

            _userManagerMock.Setup(m => m.FindByEmailAsync(email))
                .ReturnsAsync(user);

            _userManagerMock.Setup(m => m.ConfirmEmailAsync(user, token))
                .ReturnsAsync(IdentityResult.Success);

            var controller = new EmailController(_userManagerMock.Object);

            var result = await controller.EmailConfirmed(token, email);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("EmailConfirmed", viewResult.ViewName);
        }

        [Fact]
        public async Task EmailConfirmed_UserNotFound_Returns_ErrorView()
        {
            var token = "fakeToken";
            var email = "nonExistingUser@example.com";

            _userManagerMock.Setup(m => m.FindByEmailAsync(email))
                .ReturnsAsync((Person)null);

            var controller = new EmailController(_userManagerMock.Object);

            var result = await controller.EmailConfirmed(token, email);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task EmailConfirmed_EmailConfirmationFailed_Returns_ErrorView()
        {
            var token = "fakeToken";
            var email = "test@example.com";
            var user = new Person { Email = email };

            _userManagerMock.Setup(m => m.FindByEmailAsync(email))
                .ReturnsAsync(user);

            var errors = new List<IdentityError> { new IdentityError { Description = "Confirmation failed" } };
            var failedResult = IdentityResult.Failed(errors.ToArray());
            _userManagerMock.Setup(m => m.ConfirmEmailAsync(user, token))
                .ReturnsAsync(failedResult);

            var controller = new EmailController(_userManagerMock.Object);

            var result = await controller.EmailConfirmed(token, email);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public void ConfirmEmail_Returns_ConfirmEmailView()
        {
            var controller = new EmailController(_userManagerMock.Object);

            var result = controller.ConfirmEmail();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }
    }
}
