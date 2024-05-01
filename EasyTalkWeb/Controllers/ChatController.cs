using EasyTalkWeb.Models;
using EasyTalkWeb.Models.DTO.ChatDTOs;
using EasyTalkWeb.Models.Repositories;
using EasyTalkWeb.Models.ViewModels.ChatViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace EasyTalkWeb.Controllers
{
    [Authorize(Roles = "Client, Freelancer")]
    public class ChatController : Controller
    {
        private readonly UserManager<Person> _userManager;
        private readonly PersonRepository _personRepo;
        private readonly ChatRepository _chatRepo;
        private readonly FreelancerRepository _freelancerRepo;

        public ChatController(UserManager<Person> userManager, PersonRepository personRepo, ChatRepository chatRepo, FreelancerRepository freelancerRepo)
        {
            _userManager = userManager;
            _personRepo = personRepo;
            _chatRepo = chatRepo;
            _freelancerRepo = freelancerRepo;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var chats = await _chatRepo.GetChatsWithMsgsForPersonAsync(currentUser.Id);
            var chatListViewModel = new ChatsListViewModel() { Chats = GetChatPreviews(chats) };
            TempData["CurrentUsername"] = currentUser.UserName;

            return View("Chat", chatListViewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> StartChat(Guid id)
        {
            TempData["freelancerId"] = id;
            return View("CreateChat");
        }

        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> CreateChat(CreateChatViewModel model)
        {
            var clientIdentity = await _userManager.GetUserAsync(User);
            bool parsed = Guid.TryParse(TempData["freelancerId"]?.ToString(), out Guid freelancerId);
            if (!parsed)
            {
                return View("Error");
            }
            var personFreelancer = await _freelancerRepo.GetPersonByFreelancerId(freelancerId);
            var personClient = await _personRepo.GetByIdAsync(clientIdentity.Id);
            var chat = new Chat()
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
            };
            await _chatRepo.AddAsync(chat);
            chat.Persons = new List<Person>() { personClient, personFreelancer };
            await _chatRepo.Update(chat);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<JsonResult> LoadChatData(Guid id)
        {
            var chat = await _chatRepo.GetChatWithMsgsAsync(id);
            var transferableChat = new ChatDTO(chat);
            var chatData = JsonConvert.SerializeObject(transferableChat, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Json(new { chatData,  userId = _userManager.GetUserId(User) });
        }

        public ICollection<ChatPreviewViewModel> GetChatPreviews(ICollection<Chat> chats)
        {
            var previews = new List<ChatPreviewViewModel>();
            foreach (var chat in chats)
            {
                var lastMessage = chat.Messages.OrderBy(m => m.CreatedDate).ToList().LastOrDefault()?.Text;
                previews.Add(new ChatPreviewViewModel() { ChatId = chat.Id, Name = chat.Name, LastMessage = lastMessage });
            }

            return previews;
        }
    }
}
