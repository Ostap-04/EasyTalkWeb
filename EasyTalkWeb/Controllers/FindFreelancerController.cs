using EasyTalkWeb.Models;
using EasyTalkWeb.Models.Repositories;
using EasyTalkWeb.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EasyTalkWeb.Controllers
{
    [Authorize(Roles = "Client")]
    public class FindFreelancerController : Controller
    {
        private readonly FreelancerRepository freelancerrepository;
        private readonly UserManager<Person> userManager;
        private readonly ClientRepository clientRepository;

        public FindFreelancerController(FreelancerRepository freelancerrepository, UserManager<Person> userManager, ClientRepository clientRepository)
        {
            this.freelancerrepository = freelancerrepository;
            this.userManager = userManager;
            this.clientRepository = clientRepository;
        }
        public async Task<IActionResult> List()
        {
            var freelancers = await freelancerrepository.GetAllAsyncWithPerson();
            return View(freelancers as List<Freelancer>);
        }
        
        public  async Task<IActionResult> FindFreelancer(string inputData)
        {
            IEnumerable<Freelancer> freelancers;
            if (inputData == null)
            {
                freelancers = await freelancerrepository.GetAllAsyncWithPerson();
                return View("List", freelancers);
            }

            TempData["searchTerm"] = inputData;
            freelancers = await freelancerrepository.GetFreelancersBySearch(inputData);
            return View("List", freelancers);

            //var freelancers = freelancerrepository.GetFreelancersBySearch(inputData);
            //return View(freelancers);
        }
    }
}