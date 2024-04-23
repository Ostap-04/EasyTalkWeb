using EasyTalkWeb.Models;
using EasyTalkWeb.Models.Repositories;
using EasyTalkWeb.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace EasyTalkWeb.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ITechRepository _techRepository;
        private readonly UserManager<Person> _userManager;
        private readonly PersonRepository _personRepo;

        public ProfileController(UserManager<Person> userManager, ITechRepository techRepository, PersonRepository personRepo)
        {
            this._userManager = userManager;
            this._techRepository = techRepository;
            this._personRepo = personRepo;
        }

        [Authorize (Roles = "Client, Freelancer")]
        public async Task<IActionResult> Index()
        {
            Person user = await _userManager.GetUserAsync(User);
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
                var p = await _personRepo.GetFreelancer(user.Id);
                if (p != null)
                {
                    model.Technologies = p.Technologies;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit()
        {
            var allTechnologies = await _techRepository.GetAllAsync();
            Person user = await _userManager.GetUserAsync(User);
            ProfileViewModel model = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Location = user.Location,
                Email = user.Email,
                Technologies = (ICollection<Technology>)allTechnologies
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userManager.DeleteAsync(user);
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
            Person user = await _userManager.GetUserAsync(User);
            await _userManager.UpdateAsync(user);
            if (User.IsInRole("Freelancer"))
            {
                var curuser = await _personRepo.GetPersonWithTechnologiesById(user.Id);
                curuser.FirstName = editProfile.FirstName;
                curuser.LastName = editProfile.LastName;
                curuser.Email = editProfile.Email;
                curuser.Location = editProfile.Location;
                curuser.DateOfBirth = editProfile.DateOfBirth;
                var selectedTech = new List<Technology>();
                foreach (var selectedTId in editProfile.SelectedTechnologies)
                {
                    var selectedTagIdAsGuid = Guid.Parse(selectedTId);
                    var existingTag = await _techRepository.GetAsync(selectedTagIdAsGuid);
                    if (existingTag != null)
                    {
                        selectedTech.Add(existingTag);
                    }
                }
                curuser.Freelancer.Technologies = selectedTech;
                curuser.ModifiedDate = DateTime.UtcNow;
                await _personRepo.Update(curuser);
            }
            else if(User.IsInRole("Client"))
            {
                user.FirstName = editProfile.FirstName;
                user.LastName = editProfile.LastName;
                user.Email = editProfile.Email;
                user.Location = editProfile.Location;
                user.DateOfBirth = editProfile.DateOfBirth;
                user.ModifiedDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction("Index", "Profile");
        }
    }
}
