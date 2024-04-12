using EasyTalkWeb.Models;
using EasyTalkWeb.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EasyTalkWeb.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<Person> _userManager;

        public RegisterController(UserManager<Person> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OnPost(RegisterViewModel model)
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
                    //Role
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //return Redirect(Url.PageLink(pageName: "/Account/ConfirmEmail",
                    //    values: new {userId= user.Id, token= confirmationToken}));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View("Register", model);
        }

        //public async Task<IActionResult> ConfirmEmail(Guid id, string confirmationToken)
        //{

        //}

    }
}
