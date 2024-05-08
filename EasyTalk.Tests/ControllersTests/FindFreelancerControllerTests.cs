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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace EasyTalk.Tests.ControllersTests
{
    public class FindFreelancerControllerTests
    {
        private readonly AppDbContext _dbContextMock;
        private readonly Mock<UserManager<Person>> _userManagerMock;
        private readonly Mock<IMailService> _mailServiceMock;
        private readonly Mock<FreelancerRepository> _freelancerRepositoryMock;
        private readonly Mock<ClientRepository> _clientRepositoryMock;
        private readonly Mock<TechRepository> _techRepositoryMock;
        private readonly Mock<JobPostRepository> _jobPostRepositoryMock;
        private readonly Mock<ProposalRepository> _proposalRepositoryMock;
        public FindFreelancerControllerTests()
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
        public async Task List_ReturnsViewResult_WithFreelancers()
        {
            // Arrange
            var freelancers = new List<Freelancer>
            {
                new Freelancer { FreelancerId = Guid.NewGuid(), Specialization = "Software Development" },
                new Freelancer { FreelancerId = Guid.NewGuid(), Specialization = "Graphic Design" },
                new Freelancer { FreelancerId = Guid.NewGuid(), Specialization = "Digital Marketing" }
            };

            _freelancerRepositoryMock.Setup(repo => repo.GetAllAsyncWithPerson()).ReturnsAsync(freelancers);

            var controller = new FindFreelancerController(_freelancerRepositoryMock.Object, _userManagerMock.Object, _clientRepositoryMock.Object);

            // Act
            var result = await controller.List();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Freelancer>>(viewResult.Model);
            Assert.Equal(freelancers.Count, model.Count);
        }


        [Fact]
        public async Task FindFreelancer_WithInputData_ReturnsViewResultWithSearchResults()
        {
            // Arrange
            var inputData = "Software Development";
            var expectedFreelancers = new List<Freelancer>
            {
                new Freelancer { FreelancerId = Guid.NewGuid(), Specialization = "Software Development" },
                new Freelancer { FreelancerId = Guid.NewGuid(), Specialization = "Web Development" }
            };

            _freelancerRepositoryMock.Setup(repo => repo.GetFreelancersBySearch(inputData)).ReturnsAsync(expectedFreelancers);

            var controller = new FindFreelancerController(_freelancerRepositoryMock.Object, _userManagerMock.Object, _clientRepositoryMock.Object);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            // Act
            var result = await controller.FindFreelancer(inputData);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Freelancer>>(viewResult.Model);
            Assert.Equal(expectedFreelancers.Count, model.Count());
        }

        [Fact]
        public async Task FindFreelancer_WithNullInputData_ReturnsViewResultWithAllFreelancers()
        {
            // Arrange
            string inputData = null; // Set inputData to null
            var expectedFreelancers = new List<Freelancer>
            {
                new Freelancer { FreelancerId = Guid.NewGuid(), Specialization = "Software Development" },
                new Freelancer { FreelancerId = Guid.NewGuid(), Specialization = "Web Development" }
            };

            _freelancerRepositoryMock.Setup(repo => repo.GetAllAsyncWithPerson()).ReturnsAsync(expectedFreelancers); // Mock repository to return all freelancers

            var controller = new FindFreelancerController(_freelancerRepositoryMock.Object, _userManagerMock.Object, _clientRepositoryMock.Object);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            // Act
            var result = await controller.FindFreelancer(inputData);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Freelancer>>(viewResult.Model);
            Assert.Equal(expectedFreelancers.Count, model.Count());
        }


    }
}
