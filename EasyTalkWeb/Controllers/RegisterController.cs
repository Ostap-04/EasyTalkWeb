using EasyTalkWeb.Identity.EmailHost;
using EasyTalkWeb.Models;
using EasyTalkWeb.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EasyTalkWeb.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<Person> _userManager;
        private readonly IMailService _mailService;


        public RegisterController(UserManager<Person> userManager, IMailService mailService)
        {
            _userManager = userManager;
            _mailService = mailService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Person user = new Person {
                    UserName = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Location = model.Location,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    Role = model.Role,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, user.Role);
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("EmailConfirmed", "Email", new { token, email = user.Email }, Request.Scheme);
                    bool emailResponse = _mailService.SendEmail(user.Email, confirmationLink);

                    if (emailResponse)
                        return RedirectToAction("ConfirmEmail", "Email");
                    else
                    {
                        ModelState.AddModelError(nameof(model.Email), "Problem with email confirmation");
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View("Register", model);
        }
    }
}
