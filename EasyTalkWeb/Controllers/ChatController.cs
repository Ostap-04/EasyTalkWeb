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
            this._userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await this._userManager.GetUserAsync(User);
            ViewBag.CurrentUsername = currentUser.UserName;
            return View("Chat");
        }
    }
}
