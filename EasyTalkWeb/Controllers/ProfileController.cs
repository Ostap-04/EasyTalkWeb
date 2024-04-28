using EasyTalkWeb.Models;
using EasyTalkWeb.Models.Repositories;
using EasyTalkWeb.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Linq;

namespace EasyTalkWeb.Controllers
{
    public class ProfileController : Controller
    {
        private readonly TechRepository _techRepository;
        private readonly UserManager<Person> _userManager;
        private readonly PersonRepository _personRepo;
        private readonly SignInManager<Person> _signInManager;
        private readonly FreelancerRepository _freelancerRepository;


        public ProfileController(UserManager<Person> userManager, TechRepository techRepository, PersonRepository personRepo, SignInManager<Person> signInManager, FreelancerRepository freelancerRepository)
        {
            _userManager = userManager;
            _techRepository = techRepository;
            _personRepo = personRepo;
            _signInManager = signInManager;
            _freelancerRepository = freelancerRepository;
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
                    model.Specialization = p.Specialization;
                    model.Technologies = p.Technologies;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit()
        {
            var allTechnologies = await _techRepository.GetAllAsync();
            Person user = await _userManager.GetUserAsync(User);
            Person curPerson = await _personRepo.GetPersonWithTechnologiesById(user.Id);
            ProfileViewModel model = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Location = user.Location,
                Email = user.Email
            };
            if (user != null && User.IsInRole("Freelancer"))
            {
                var freelancer = await _personRepo.GetFreelancer(user.Id);
                user.Freelancer = freelancer;
                model.Specialization = user.Freelancer.Specialization;
                model.Technologies = (ICollection<Technology>)allTechnologies;
                foreach (var technology in curPerson.Freelancer.Technologies)
                {
                    model.SelectedTechnologiesData.Add(technology.Name);
                }
                
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            await _signInManager.SignOutAsync();
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Deleted", "Profile");
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

            if (User.IsInRole("Freelancer"))
            {

                var curuser = await _personRepo.GetPersonWithTechnologiesById(user.Id);
                var formData = HttpContext.Request.Form;
                curuser.FirstName = formData["firstName"];
                curuser.LastName = formData["lastName"];
                curuser.Email = formData["email"];
                curuser.Location = formData["location"];
                curuser.DateOfBirth = string.IsNullOrEmpty(formData["dateOfBirth"]) ? null : DateOnly.Parse(formData["dateOfBirth"]);
                curuser.Freelancer.Specialization = formData["specialization"];
                string selectedTechnologies = formData["selectedTechnologies"];
                List<string> selectedTechsNames = selectedTechnologies.Split(",").ToList();

                foreach (var technology in curuser.Freelancer.Technologies.ToList())
                {
                    if (selectedTechsNames.Contains(technology.Name))
                    {
                        selectedTechsNames.Remove(technology.Name);
                    }
                    else
                    {
                        curuser.Freelancer.Technologies.Remove(technology);
                    }
                }

                foreach (var techName in selectedTechsNames)
                {
                    var technology = await _techRepository.GetTechnologyWithFreelancerByNameAsync(techName);
                    if (technology != null)
                    {
                        curuser.Freelancer.Technologies.Add(technology);
                    }
                }

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

        public async Task<IActionResult> Deleted()
        {
            return View();
        }
    }
}
