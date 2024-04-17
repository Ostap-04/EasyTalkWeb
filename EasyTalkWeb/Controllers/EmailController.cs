using EasyTalkWeb.Identity.EmailHost;
using EasyTalkWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EasyTalkWeb.Controllers
{
    public class EmailController : Controller
    {
        private UserManager<Person> _userManager;

        public EmailController(UserManager<Person> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> EmailConfirmed(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return View("Error");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return View(result.Succeeded ? "EmailConfirmed" : "Error");
        }

        public IActionResult ConfirmEmail()
        {
            return View();
        }

    }
}
