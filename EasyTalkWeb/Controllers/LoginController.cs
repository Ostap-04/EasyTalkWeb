using EasyTalkWeb.Models;
using EasyTalkWeb.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EasyTalkWeb.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserManager<Person> _userManager;
        private readonly SignInManager<Person> _signInManager;

        public LoginController(SignInManager<Person> signInManager, UserManager<Person> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View("Login");
            Person appUser = await _userManager.FindByEmailAsync(model.Email);
            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);
              

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Profile");
            }
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Login", "You are locked out");
                }
                else
                {
                    ModelState.AddModelError("Login", "Failed to login");
                }
            }
            bool emailStatus = await _userManager.IsEmailConfirmedAsync(appUser);
            if (emailStatus == false)
            {
                ModelState.AddModelError(nameof(model.Email), "Email is unconfirmed, please confirm it first");
            }
            
            if (emailStatus == false)
            {
                ModelState.AddModelError(nameof(model.Email), "Email is unconfirmed, please confirm it first");
            }

            return View("Login", model);
        }

        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
