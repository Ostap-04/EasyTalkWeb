using EasyTalkWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EasyTalkWeb.Controllers
{
    [Authorize(Roles = "Client, Freelancer")]
    public class ChatController : Controller
    {
        private readonly UserManager<Person> _userManager;

        public ChatController(UserManager<Person> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            TempData["CurrentUsername"] = currentUser.UserName;
            return View("Chat");
        }
    }
}
