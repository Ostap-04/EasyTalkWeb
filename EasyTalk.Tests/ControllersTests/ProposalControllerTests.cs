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
using EasyTalkWeb.Controllers;
using Microsoft.AspNetCore.Mvc;
using EasyTalkWeb.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace EasyTalk.Tests.ControllersTests
{
    public class ProposalControllerTests
    {
        private readonly AppDbContext _dbContextMock;
        private readonly Mock<UserManager<Person>> _userManagerMock;
        private readonly Mock<IMailService> _mailServiceMock;
        private readonly Mock<FreelancerRepository> _freelancerRepositoryMock;
        private readonly Mock<ClientRepository> _clientRepositoryMock;
        private readonly Mock<TechRepository> _techRepositoryMock;
        private readonly Mock<JobPostRepository> _jobPostRepositoryMock;
        private readonly Mock<ProposalRepository> _proposalRepositoryMock;
        public ProposalControllerTests()
        {
            _dbContextMock = Mock.Of<AppDbContext>();
            var store = new Mock<IUserStore<Person>>();
            _userManagerMock = new Mock<UserManager<Person>>(store.Object, null, null, null, null, null, null, null, null);
            _mailServiceMock = new Mock<IMailService>();
            _freelancerRepositoryMock = new Mock<FreelancerRepository>(_dbContextMock);
            _clientRepositoryMock = new Mock<ClientRepository>(_dbContextMock);
            _techRepositoryMock = new Mock<TechRepository>(_dbContextMock);
            _jobPostRepositoryMock = new Mock<JobPostRepository>(_dbContextMock);
            _proposalRepositoryMock = new Mock<ProposalRepository>(_dbContextMock);

        }


        [Fact]
        public async void AddProposal_Returns_ViewResult()
        {
            var controller = new ProposalController(_proposalRepositoryMock.Object, _techRepositoryMock.Object, _userManagerMock.Object, _freelancerRepositoryMock.Object);
            var result = await controller.Add();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task Add_ValidProposal_ReturnsRedirectToAction()
        {
            var controller = new ProposalController(_proposalRepositoryMock.Object, _techRepositoryMock.Object, _userManagerMock.Object, _freelancerRepositoryMock.Object);

            var user = new Person { Id = Guid.NewGuid() };
            var freelancer = new Freelancer() { FreelancerId = Guid.NewGuid(), PersonId = user.Id };

            var proposalRequest = new ProposalRequest()
            {
                Title = "Test Job",
                Text = "Test job description",
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
            _freelancerRepositoryMock.Setup(m => m.GetFreelancerByPersonId(It.IsAny<Guid>()))
                .Returns(freelancer);
            _techRepositoryMock.Setup(m => m.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => new Technology { Id = id }); // Simulate fetching tech by ID

            // Act
            var result = await controller.Add(proposalRequest);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectToActionResult.ActionName);
        }

        [Fact]
        public async void ListProposal_Returns_ViewResult()
        {
            var controller = new ProposalController(_proposalRepositoryMock.Object, _techRepositoryMock.Object, _userManagerMock.Object, _freelancerRepositoryMock.Object);
            var result = await controller.List();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }


        [Fact]
        public async Task Edit_ValidProposal_ReturnsRedirectToAction()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var freelancer = new Freelancer { FreelancerId = Guid.NewGuid() };
            var proposalId = Guid.NewGuid();
            var proposalRequest = new EditProposalRequest()
            {
                Id = proposalId,
                Title = "Updated Title",
                Text = "Updated Description"
            };
            var proposal = new Proposal { Id = proposalId /* Initialize other properties */ };



            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(new Person { Id = userId });
            _freelancerRepositoryMock.Setup(cr => cr.GetFreelancerByPersonId(It.IsAny<Guid>())).Returns(freelancer);
            _proposalRepositoryMock.Setup(jpr => jpr.GetProposaltByIdForFreelancer(It.IsAny<Guid>(), proposalId)).ReturnsAsync(proposal);

            var controller = new ProposalController(_proposalRepositoryMock.Object, _techRepositoryMock.Object, _userManagerMock.Object, _freelancerRepositoryMock.Object);

            // Act
            var result = await controller.Edit(proposalRequest);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectToActionResult.ActionName);
        }
        [Fact]
        public async Task Edit_ValidId_ReturnsViewWithModel()
        {
            // Arrange
            var proposalId = Guid.NewGuid();
            var proposal = new Proposal
            {
                Id = proposalId,
                Title = "Test Job",
                Text = "Test Description"
            };


            _proposalRepositoryMock.Setup(repo => repo.GetByIdAsync(proposalId)).ReturnsAsync(proposal);

            var controller = new ProposalController(_proposalRepositoryMock.Object, _techRepositoryMock.Object, _userManagerMock.Object, _freelancerRepositoryMock.Object);

            // Act
            var result = await controller.Edit(proposalId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<EditProposalRequest>(viewResult.ViewData.Model);
            Assert.Equal(proposalId, model.Id);
            Assert.Equal("Test Job", model.Title);
            Assert.Equal("Test Description", model.Text);
        }
        [Fact]
        public async Task Edit_ValidId_ReturnsNull()
        {
            // Arrange
            var proposalId = Guid.NewGuid();
            var proposal = new Proposal
            {
                Id = proposalId,
                Title = "Test Job",
                Text = "Test Description"
            };

            _proposalRepositoryMock.Setup(repo => repo.GetByIdAsync(proposalId)).ReturnsAsync((Proposal)null);

            var controller = new ProposalController(_proposalRepositoryMock.Object, _techRepositoryMock.Object, _userManagerMock.Object, _freelancerRepositoryMock.Object);

            // Act
            var result = await controller.Edit(proposalId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewData.Model);
        }
        [Fact]
        public async Task Delete_ValidProposal_ReturnsRedirectToAction()
        {
            // Arrange
            var propId = Guid.NewGuid();
            var proposalRequest = new EditProposalRequest() { Id = propId };
            var proposalRepositoryMock = new Mock<ProposalRepository>();
            _proposalRepositoryMock.Setup(repo => repo.DeleteAsync(propId)).Returns(Task.CompletedTask);
            var controller = new ProposalController(_proposalRepositoryMock.Object, _techRepositoryMock.Object, _userManagerMock.Object, _freelancerRepositoryMock.Object);


            // Act
            var result = await controller.Delete(proposalRequest);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectToActionResult.ActionName);
        }

    }
}
