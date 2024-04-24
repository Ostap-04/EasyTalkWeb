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
        public void Add_Returns_ViewResult()
        {
            var controller = new JobPostController(_jobPostRepositoryMock.Object,_techRepositoryMock.Object,_userManagerMock.Object ,_clientRepositoryMock.Object);

            var result = controller.Add();

            Assert.IsType<>(result);
        }
    }
    
}
