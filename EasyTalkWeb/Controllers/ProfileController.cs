using EasyTalkWeb.Models;
using EasyTalkWeb.Models.Repositories;
using EasyTalkWeb.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EasyTalkWeb.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ITechRepository techRepository;
        private readonly UserManager<Person> userManager;
        private readonly PersonRepository _personRepo;

        public ProfileController(UserManager<Person> userManager, ITechRepository techRepository, PersonRepository personRepo)
        {
            this.userManager = userManager;
            this.techRepository = techRepository;
            _personRepo = personRepo;
        }

        [Authorize (Roles = "Client, Freelancer")]
        public async Task<IActionResult> Index()
        {
            Person user = await userManager.GetUserAsync(User);
            
            ProfileViewModel model = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Location = user.Location,
                Email = user.Email,
            };

            if (User.IsInRole("Freelancer"))
            {
                var p = await _personRepo.GetPersonWithTechnologies(user.Id);
                if (p != null)
                {
                    model.Technologies = p.Technologies;
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Edit()
        {
            Person user = await userManager.GetUserAsync(User);

            ProfileViewModel model = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Location = user.Location,
                Email = user.Email,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProfileViewModel editProfile)
        {
            var curuser = await userManager.GetUserAsync(User);
            var freelancer = _personRepo.GetFreelancerByPersonId(curuser.Id);
            curuser.FirstName = editProfile.FirstName;
            curuser.LastName = editProfile.LastName;
            curuser.Email = editProfile.Email;
            curuser.Location = editProfile.Location;
            curuser.DateOfBirth = editProfile.DateOfBirth;

            curuser.ModifiedDate = DateTime.UtcNow;

            await userManager.UpdateAsync(curuser);
            return RedirectToAction("Index", "Profile");

        }
    }


}
