using EasyTalkWeb.Models.Repositories;
using EasyTalkWeb.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using EasyTalkWeb.Identity.EmailHost;
using EasyTalkWeb.Persistance;
using EasyTalkWeb.Models.ViewModels.ChatViewModels;
using EasyTalkWeb.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using EasyTalkWeb.Models.DTO.ChatDTOs;
using NuGet.Versioning;
using Message = EasyTalkWeb.Models.Message;
using System.Text.Json;
using Newtonsoft.Json;

namespace EasyTalk.Tests.ControllersTests
{
    public class ChatControllerTests
    {
        private readonly AppDbContext _dbContextMock;
        private readonly Mock<UserManager<Person>> _userManagerMock;
        private readonly Mock<PersonRepository> _personRepoMock;
        private readonly Mock<ChatRepository> _chatRepoMock;
        private readonly Mock<FreelancerRepository> _freelancerRepoMock;
        private readonly Mock<JobPostRepository> _jobPostRepoMock;
        private readonly Mock<ProposalRepository> _proposalRepoMock;

        public ChatControllerTests()
        {
            _dbContextMock = Mock.Of<AppDbContext>();
            _userManagerMock = new Mock<UserManager<Person>>(Mock.Of<IUserStore<Person>>(), null!, null!, null!, null!, null!, null!, null!, null!);
            _personRepoMock = new Mock<PersonRepository>(_dbContextMock);
            _chatRepoMock = new Mock<ChatRepository>(_dbContextMock);
            _freelancerRepoMock = new Mock<FreelancerRepository>(_dbContextMock);
            _jobPostRepoMock = new Mock<JobPostRepository>(_dbContextMock);
            _proposalRepoMock = new Mock<ProposalRepository>(_dbContextMock);
        }

        [Fact]
        public void GetChatPreviews_ReturnsCorrectChatPreviews()
        {
            var controller = new ChatController(_userManagerMock.Object, _personRepoMock.Object, _chatRepoMock.Object, _freelancerRepoMock.Object, _jobPostRepoMock.Object, _proposalRepoMock.Object);
            
            var chats = new List<Chat>
            {
                new Chat { Id = Guid.NewGuid(), Name = "Chat 1", Messages = new List<Message> { new Message { Text = "Message 1", CreatedDate = DateTime.UtcNow } } },
                new Chat { Id = Guid.NewGuid(), Name = "Chat 2", Messages = new List<Message> { new Message { Text = "Message 2", CreatedDate = DateTime.UtcNow.AddDays(-1) } } }
            };

            var expectedPreviews = new List<ChatPreviewViewModel>
            {
                new ChatPreviewViewModel { ChatId = chats[0].Id, Name = "Chat 1", LastMessage = "Message 1" },
                new ChatPreviewViewModel { ChatId = chats[1].Id, Name = "Chat 2", LastMessage = "Message 2" }
            };

            var actualPreviews = controller.GetChatPreviews(chats).ToList();

            Assert.Equal(expectedPreviews.Count, actualPreviews.Count);
            for (int i = 0; i < expectedPreviews.Count; i++)
            {
                Assert.Equal(expectedPreviews[i].ChatId, actualPreviews[i].ChatId);
                Assert.Equal(expectedPreviews[i].Name, actualPreviews[i].Name);
                Assert.Equal(expectedPreviews[i].LastMessage, actualPreviews[i].LastMessage);
            }
        }

        [Fact]
        public async Task Index_ReturnsChatViewWithCorrectViewModel()
        {
            var currentUser = new Person { Id = Guid.NewGuid(), UserName = "TestUser" };

            var chats = new List<Chat>
            {
                new Chat { Id = Guid.NewGuid(), Name = "Chat 1", Messages = new List<Message> { new Message { Text = "Message 1", CreatedDate = DateTime.UtcNow } } },
                new Chat { Id = Guid.NewGuid(), Name = "Chat 2", Messages = new List<Message> { new Message { Text = "Message 2", CreatedDate = DateTime.UtcNow.AddDays(-1) } } }
            };

            var expectedPreviews = new List<ChatPreviewViewModel>
            {
                new ChatPreviewViewModel { ChatId = chats[0].Id, Name = "Chat 1", LastMessage = "Message 1" },
                new ChatPreviewViewModel { ChatId = chats[1].Id, Name = "Chat 2", LastMessage = "Message 2" }
            };

            var expectedViewModel = new ChatsListViewModel
            {
                Chats = expectedPreviews
            };

            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
            _chatRepoMock.Setup(m => m.GetChatsWithMsgsForPersonAsync(It.IsAny<Guid>())).ReturnsAsync(chats);

            var controller = new ChatController(_userManagerMock.Object, _personRepoMock.Object, _chatRepoMock.Object, _freelancerRepoMock.Object, _jobPostRepoMock.Object, _proposalRepoMock.Object);
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, currentUser.Id.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var result = await controller.Index() as ViewResult;
            var viewModel = result?.Model as ChatsListViewModel;
            var expectedViewModelChats = expectedViewModel.Chats.ToList();
            var viewModelChats = viewModel?.Chats.ToList();

            Assert.NotNull(result);
            Assert.NotNull(viewModel);
            Assert.Equal("Chat", result.ViewName);
            Assert.Equal(expectedViewModel.Chats.Count, viewModelChats?.Count);
            for (int i = 0; i < expectedViewModel.Chats.Count; i++)
            {
                Assert.Equal(expectedViewModelChats[i].ChatId, viewModelChats[i].ChatId);
                Assert.Equal(expectedViewModelChats[i].Name, viewModelChats[i].Name);
                Assert.Equal(expectedViewModelChats[i].LastMessage, viewModelChats[i].LastMessage);
            }
            Assert.Equal(currentUser.UserName, controller.TempData["CurrentUsername"]);
        }

        [Fact]
        public async Task StartChat_ReturnsCreateChatViewWithFreelancerIdInTempData()
        {
            var controller = new ChatController(_userManagerMock.Object, _personRepoMock.Object, _chatRepoMock.Object, _freelancerRepoMock.Object, _jobPostRepoMock.Object, _proposalRepoMock.Object);

            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            var jobPostId = Guid.NewGuid();
            var proposalId = Guid.NewGuid();
            var freelancerId = Guid.NewGuid();

            var result = await controller.StartChat(jobPostId, proposalId, freelancerId) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal("CreateChat", result.ViewName);

            Assert.True(controller.TempData.ContainsKey("freelancerId"));
            Assert.Equal(freelancerId, controller.TempData["freelancerId"]);
            Assert.True(controller.TempData.ContainsKey("jobPostId"));
            Assert.Equal(jobPostId, controller.TempData["jobPostId"]);
            Assert.True(controller.TempData.ContainsKey("proposalId"));
            Assert.Equal(proposalId, controller.TempData["proposalId"]);
        }

        [Fact]
        public async Task LoadChatData_ReturnsJsonResultWithChatDataAndUserId()
        {
            var controller = new ChatController(_userManagerMock.Object, _personRepoMock.Object, _chatRepoMock.Object, _freelancerRepoMock.Object, _jobPostRepoMock.Object, _proposalRepoMock.Object);

            var chatId = Guid.NewGuid();
            var chat = new Chat { Id = chatId, Persons = new List<Person>(), Messages = new List<Message>() };
            var chatData = new ChatDTO(chat);
            var expectedUserId = Guid.NewGuid().ToString();
            var expectedChatData = JsonConvert.SerializeObject(chatData, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            var newObj = new { chatData = expectedChatData, userId = expectedUserId};
            _userManagerMock.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(expectedUserId);

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, expectedUserId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _chatRepoMock.Setup(m => m.GetChatWithMsgsAsync(chatId)).ReturnsAsync(chat);

            var result = await controller.LoadChatData(chatId) as JsonResult;

            Assert.NotNull(result);
            Assert.Equal(newObj.ToString(), result.Value.ToString());
        }

        [Fact]
        public async Task CreateChat_ReturnsRedirectToAction_Index_When_SuccessfullyCreated()
        {
            var controller = new ChatController(_userManagerMock.Object, _personRepoMock.Object, _chatRepoMock.Object, _freelancerRepoMock.Object, _jobPostRepoMock.Object, _proposalRepoMock.Object);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var model = new CreateChatViewModel();
            var clientIdentity = new Person { Id = Guid.NewGuid() };
            var freelancerId = Guid.NewGuid();
            var freelancer = new Person();
            var client = new Person();
            var chatId = Guid.NewGuid();

            controller.TempData["freelancerId"] = freelancerId;
            
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(clientIdentity);
            _freelancerRepoMock.Setup(repo => repo.GetPersonByFreelancerId(freelancerId)).ReturnsAsync(freelancer);
            _personRepoMock.Setup(repo => repo.GetByIdAsync(clientIdentity.Id)).ReturnsAsync(client);
            _chatRepoMock.Setup(repo => repo.AddAsync(It.IsAny<Chat>())).Callback((Chat newChat) => newChat.Id = chatId);

            var result = await controller.CreateChat(model);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task CreateChat_ReturnsViewResult_Error_When_TempDataParseFailed()
        {
            var controller = new ChatController(_userManagerMock.Object, _personRepoMock.Object, _chatRepoMock.Object, _freelancerRepoMock.Object, _jobPostRepoMock.Object, _proposalRepoMock.Object);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var clientIdentity = new Person { Id = Guid.NewGuid() };

            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(clientIdentity);

            var result = await controller.CreateChat(new CreateChatViewModel());

            var ViewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", ViewResult.ViewName);
        }
    }
}
