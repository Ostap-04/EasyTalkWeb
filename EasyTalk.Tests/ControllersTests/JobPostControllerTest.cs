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
using System.Security.Claims;

namespace EasyTalk.Tests.Controllers
{
    
    public class JobPostControllerTest
    {
        private readonly AppDbContext _dbContextMock;
        private readonly Mock<UserManager<Person>> _userManagerMock;
        private readonly Mock<IMailService> _mailServiceMock;
        private readonly Mock<FreelancerRepository> _freelancerRepositoryMock;
        private readonly Mock<ClientRepository> _clientRepositoryMock;
        private readonly Mock<TechRepository> _techRepositoryMock;
        private readonly Mock<JobPostRepository> _jobPostRepositoryMock;
        public JobPostControllerTest()
        {
            _dbContextMock = Mock.Of<AppDbContext>();
            var store = new Mock<IUserStore<Person>>();
            _userManagerMock = new Mock<UserManager<Person>>(store.Object, null, null, null, null, null, null, null, null);
            _mailServiceMock = new Mock<IMailService>();
            _freelancerRepositoryMock = new Mock<FreelancerRepository>(_dbContextMock);
            _clientRepositoryMock = new Mock<ClientRepository>(_dbContextMock);
            _techRepositoryMock = new Mock<TechRepository>(_dbContextMock);
            _jobPostRepositoryMock = new Mock<JobPostRepository>(_dbContextMock);
        }

        [Fact]
        public async void AddJobPost_Returns_ViewResult()
        {
            var controller = new JobPostController(_jobPostRepositoryMock.Object,_techRepositoryMock.Object,_userManagerMock.Object,_clientRepositoryMock.Object);
            var result = await controller.Add();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task Add_ValidJobPost_ReturnsRedirectToAction()
        {
            // Arrange
            var controller = new JobPostController(
                _jobPostRepositoryMock.Object,
                _techRepositoryMock.Object,
                _userManagerMock.Object,
                _clientRepositoryMock.Object
            );

            var user = new Person { Id = Guid.NewGuid() };
            var client = new Client { ClientId = Guid.NewGuid(), PersonId = user.Id };

            var jobPostRequest = new JobPostRequest
            {
                Title = "Test Job",
                Price = 1000,
                Description = "Test job description",
                SelectedTech = new string[] { $"{Guid.NewGuid()}", $"{Guid.NewGuid()}" }
            };

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _userManagerMock.Setup(m => m.GetUserAsync(claimsPrincipal))
                .ReturnsAsync(user);
            _clientRepositoryMock.Setup(m => m.GetClientByPersonId(It.IsAny<Guid>()))
                .Returns(client);
            _techRepositoryMock.Setup(m => m.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => new Technology { Id = id }); // Simulate fetching tech by ID

            // Act
            var result = await controller.Add(jobPostRequest);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectToActionResult.ActionName);
        }
        [Fact]
        public async void ListJobPost_Returns_ViewResult()
        {
            var controller = new JobPostController(_jobPostRepositoryMock.Object, _techRepositoryMock.Object, _userManagerMock.Object, _clientRepositoryMock.Object);
            var result = await controller.List();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task Edit_ValidJobPost_ReturnsRedirectToAction()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var client = new Client { ClientId = Guid.NewGuid() };
            var jobPostId = Guid.NewGuid();
            var jobPostRequest = new EditJobPostRequest
            {
                Id = jobPostId,
                Title = "Updated Title",
                Price = 100,
                Description = "Updated Description"
            };
            var jobPost = new JobPost { Id = jobPostId /* Initialize other properties */ };

         

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(new Person { Id = userId });
            _clientRepositoryMock.Setup(cr => cr.GetClientByPersonId(It.IsAny<Guid>())).Returns(client);
            _jobPostRepositoryMock.Setup(jpr => jpr.GetJobPostByIdForClient(It.IsAny<Guid>(), jobPostId)).ReturnsAsync(jobPost);

            var controller = new JobPostController(_jobPostRepositoryMock.Object,_techRepositoryMock.Object, _userManagerMock.Object, _clientRepositoryMock.Object);

            // Act
            var result = await controller.Edit(jobPostRequest);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectToActionResult.ActionName);
        }
        [Fact]
        public async Task Edit_ValidId_ReturnsViewWithModel()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var jobPost = new JobPost
            {
                Id = jobId,
                Title = "Test Job",
                Price = 100,
                Description = "Test Description"
            };

          
            _jobPostRepositoryMock.Setup(repo => repo.GetByIdAsync(jobId)).ReturnsAsync(jobPost);

            var controller = new JobPostController(_jobPostRepositoryMock.Object, _techRepositoryMock.Object, _userManagerMock.Object, _clientRepositoryMock.Object);

            // Act
            var result = await controller.Edit(jobId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<EditJobPostRequest>(viewResult.ViewData.Model);
            Assert.Equal(jobId, model.Id);
            Assert.Equal("Test Job", model.Title);
            Assert.Equal(100, model.Price);
            Assert.Equal("Test Description", model.Description);
        }
        [Fact]
        public async Task Edit_ValidId_ReturnsNull()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var jobPost = new JobPost
            {
                Id = jobId,
                Title = "Test Job",
                Price = 100,
                Description = "Test Description"
            };


            _jobPostRepositoryMock.Setup(repo => repo.GetByIdAsync(jobId)).ReturnsAsync((JobPost)null);

            var controller = new JobPostController(_jobPostRepositoryMock.Object, _techRepositoryMock.Object, _userManagerMock.Object, _clientRepositoryMock.Object);

            // Act
            var result = await controller.Edit(jobId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Delete_ValidJobPost_ReturnsRedirectToAction()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var jobPostRequest = new EditJobPostRequest { Id = jobId };
            var jobPostRepositoryMock = new Mock<JobPostRepository>();
            _jobPostRepositoryMock.Setup(repo => repo.DeleteAsync(jobId)).Returns(Task.CompletedTask);
            var controller = new JobPostController(_jobPostRepositoryMock.Object, _techRepositoryMock.Object, _userManagerMock.Object, _clientRepositoryMock.Object);

            // Act
            var result = await controller.Delete(jobPostRequest);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectToActionResult.ActionName);
        }

    }

}
