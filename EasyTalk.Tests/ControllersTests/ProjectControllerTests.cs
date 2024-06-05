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
using EasyTalkWeb.Controllers;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using EasyTalkWeb.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace EasyTalk.Tests.ControllersTests
{
    public class ProjectControllerTests
    {
        private readonly AppDbContext _dbContextMock;
        private readonly Mock<UserManager<Person>> _userManagerMock;
        private readonly Mock<ClientRepository> _clientRepositoryMock;
        private readonly Mock<FreelancerRepository> _freelancerRepositoryMock;
        private readonly Mock<ProjectRepository> _projectRepositoryMock;
        private readonly Mock<ChatRepository> _chatRepositoryMock;


        public ProjectControllerTests()
        {
            _dbContextMock = Mock.Of<AppDbContext>();
            _userManagerMock = new Mock<UserManager<Person>>(Mock.Of<IUserStore<Person>>(), null!, null!, null!, null!, null!, null!, null!, null!);
            _clientRepositoryMock = new Mock<ClientRepository>(_dbContextMock);
            _freelancerRepositoryMock = new Mock<FreelancerRepository>(_dbContextMock);
            _projectRepositoryMock = new Mock<ProjectRepository>(_dbContextMock);
            _chatRepositoryMock = new Mock<ChatRepository>(_dbContextMock);
        }

        [Fact]
        public async Task StartProject_WithValidId_ReturnsView()
        {
            var controller = new ProjectController(_userManagerMock.Object, _clientRepositoryMock.Object, _freelancerRepositoryMock.Object, _projectRepositoryMock.Object, _chatRepositoryMock.Object);
            var userId = Guid.NewGuid().ToString();
            var freelancerId = Guid.NewGuid();
            var chosenFreelancer = new Freelancer { FreelancerId = freelancerId };

            _freelancerRepositoryMock.Setup(m => m.GetByIdAsync(freelancerId)).ReturnsAsync(chosenFreelancer);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, "Client")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var result = await controller.StartProject(freelancerId) as ViewResult;

            Assert.NotNull(result);
            Assert.Null(result.ViewName);
            var model = result.Model as ProjectRequest;
            Assert.NotNull(model);
            Assert.Equal(chosenFreelancer.FreelancerId, model.FreelancerId);
        }

        [Fact]
        public async Task SaveProject_WithValidRequest_ReturnsRedirectToActionResult()
        {
            var controller = new ProjectController(_userManagerMock.Object, _clientRepositoryMock.Object, _freelancerRepositoryMock.Object, _projectRepositoryMock.Object, _chatRepositoryMock.Object);
            var userId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var freelancerId = Guid.NewGuid();
            var request = new ProjectRequest
            {
                FreelancerId = freelancerId,
                Name = "Test Project",
                Description = "Test Description",
                Price = 100
            };
            var client = new Client { ClientId = clientId };

            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new Person { Id = userId });
            _clientRepositoryMock.Setup(m => m.GetClientByPersonId(It.IsAny<Guid>())).Returns(client);

            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()), new Claim(ClaimTypes.Role, "Client") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var result = await controller.SaveProject(request) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("List", result.ActionName);
        }

        [Fact]
        public async Task Edit_WithValidId_ReturnsViewWithProjectRequestModel()
        {
            var controller = new ProjectController(null, null, null, _projectRepositoryMock.Object, _chatRepositoryMock.Object);
            var projectId = Guid.NewGuid();
            var project = new Project
            {
                Id = projectId,
                Name = "Test Project",
                Description = "Test Description",
                Price = 100
            };

            _projectRepositoryMock.Setup(m => m.GetByIdAsync(projectId)).ReturnsAsync(project);

            var result = await controller.Edit(projectId) as ViewResult;

            Assert.NotNull(result);
            Assert.IsType<ProjectRequest>(result.Model);
            var model = result.Model as ProjectRequest;
            Assert.Equal(projectId, model.Id);
            Assert.Equal(project.Name, model.Name);
            Assert.Equal(project.Description, model.Description);
            Assert.Equal(project.Price, model.Price);
        }

        [Fact]
        public async Task List_ReturnsViewResultWithProjects()
        {
            var controller = new ProjectController(null, null, null, _projectRepositoryMock.Object, _chatRepositoryMock.Object);
            var projects = new List<Project>
            {
                new Project { Id = Guid.NewGuid(), Name = "Project 1", Description = "Description 1", Price = 100 },
                new Project { Id = Guid.NewGuid(), Name = "Project 2", Description = "Description 2", Price = 200 },
                new Project { Id = Guid.NewGuid(), Name = "Project 3", Description = "Description 3", Price = 300 }
            };

            _projectRepositoryMock.Setup(m => m.GetAllProjects()).ReturnsAsync(projects);

            var result = await controller.List() as ViewResult;

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<Project>>(result.Model);
            var model = result.Model as List<Project>;
            Assert.Equal(projects.Count, model.Count);
        }

        [Fact]
        public async Task Edit_WithInvalidId_ReturnsViewWithNullModel()
        {
            var controller = new ProjectController(null, null, null, _projectRepositoryMock.Object, _chatRepositoryMock.Object);
            var projectId = Guid.NewGuid();

            _projectRepositoryMock.Setup(m => m.GetByIdAsync(projectId)).ReturnsAsync((Project)null);

            var result = await controller.Edit(projectId) as ViewResult;

            Assert.NotNull(result);
            Assert.Null(result.Model);
        }

        [Fact]
        public async Task Edit_WithValidProjectRequest_ReturnsRedirectToActionResult()
        {
            var controller = new ProjectController(null, null, null, _projectRepositoryMock.Object, _chatRepositoryMock.Object);
            var projectId = Guid.NewGuid();
            var projectRequest = new ProjectRequest
            {
                Id = projectId,
                Name = "Updated Project Name",
                Description = "Updated Description",
                Price = 200
            };
            var project = new Project
            {
                Id = projectId,
                Name = "Initial Project Name",
                Description = "Initial Description",
                Price = 100
            };

            _projectRepositoryMock.Setup(m => m.GetByIdAsync(projectRequest.Id)).ReturnsAsync(project);

            var result = await controller.Edit(projectRequest) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("List", result.ActionName);
        }

        [Fact]
        public async Task Delete_WithValidProjectRequest_ReturnsRedirectToActionResult()
        {
            var controller = new ProjectController(null, null, null, _projectRepositoryMock.Object, _chatRepositoryMock.Object);
            var projectId = Guid.NewGuid();
            var projectRequest = new ProjectRequest
            {
                Id = projectId
            };

            var result = await controller.Delete(projectRequest) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("List", result.ActionName);
        }
    }
}
