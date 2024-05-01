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

namespace EasyTalk.Tests.ControllersTests
{
    public class FindJobControllerTests
    {
        private readonly AppDbContext _dbContextMock;
        private readonly Mock<UserManager<Person>> _userManagerMock;
        private readonly Mock<IMailService> _mailServiceMock;
        private readonly Mock<FreelancerRepository> _freelancerRepositoryMock;
        private readonly Mock<ClientRepository> _clientRepositoryMock;
        private readonly Mock<TechRepository> _techRepositoryMock;
        private readonly Mock<JobPostRepository> _jobPostRepositoryMock;
        private readonly Mock<ProposalRepository> _proposalRepositoryMock;
        public FindJobControllerTests()
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
        public async Task Index_ReturnsViewResult_WithJobPosts()
        {
            // Arrange
            var mockDbContext = new Mock<AppDbContext>();
            var mockJobPostRepository = new Mock<JobPostRepository>(mockDbContext.Object);


            var jobPosts = new List<JobPost>
            {
                new JobPost { Id = Guid.NewGuid(), Title = "Software Engineer" },
                new JobPost { Id = Guid.NewGuid(), Title = "Data Analyst" },
                new JobPost { Id = Guid.NewGuid(), Title = "Web Developer" }
            };
            mockJobPostRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(jobPosts);

            var controller = new FindJobController(mockJobPostRepository.Object);

            // Act
            var result = await controller.Index(null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<JobPost>>(viewResult.Model);
            Assert.Equal(3, model.Count()); // Ensure all job posts are returned

            // Additional assertions can be added to verify specific job posts or other conditions based on the searchString
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithFilteredJobPosts()
        {
            // Arrange
            var mockDbContext = new Mock<AppDbContext>();
            var mockJobPostRepository = new Mock<JobPostRepository>(mockDbContext.Object);

            var jobPosts = new List<JobPost>
            {
                new JobPost { Id = Guid.NewGuid(), Title = "Software Engineer" },
                new JobPost { Id = Guid.NewGuid(), Title = "Data Analyst" },
                new JobPost { Id = Guid.NewGuid(), Title = "Web Developer" }
            };
            mockJobPostRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(jobPosts);

            var controller = new FindJobController(mockJobPostRepository.Object);

            // Act
            var result = await controller.Index("Software");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<JobPost>>(viewResult.Model);
            Assert.Single(model); // Ensure only the job post containing "Software" in the title is returned
            Assert.Contains(model, j => j.Title.Contains("Software"));
        }

    }
}
