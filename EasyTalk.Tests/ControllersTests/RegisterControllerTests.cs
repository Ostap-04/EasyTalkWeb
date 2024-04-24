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
            var store = new Mock<IUserStore<Person>>();
            _userManagerMock = new Mock<UserManager<Person>>(store.Object, null, null, null, null, null, null, null, null);
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
            // Arrange
            var controller = new RegisterController(_userManagerMock.Object, _mailServiceMock.Object, _freelancerRepositoryMock.Object, _clientRepositoryMock.Object);
            var model = new RegisterViewModel
            {
                Gender = Gender.Male, // Set gender
                FirstName = "John", // Set first name
                LastName = "Doe", // Set last name
                DateOfBirth = new DateOnly(1990, 1, 1), // Set date of birth
                Location = "New York", // Set location
                Email = "john@example.com", // Set email
                Password = "Password123", // Set password
                Role = "Client" // Set role
            };
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<Person>(), It.IsAny<string>()))
                            .ReturnsAsync(IdentityResult.Success);
            // Act
            var result = await controller.Register(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ConfirmEmail", redirectResult.ActionName);
            Assert.Equal("Email", redirectResult.ControllerName);
        }
    }
}
