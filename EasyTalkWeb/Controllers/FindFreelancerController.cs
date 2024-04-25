using EasyTalkWeb.Models;
using EasyTalkWeb.Models.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EasyTalkWeb.Controllers
{
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
            return View(freelancers);
        }
        public async Task<IActionResult> SignContract(Guid id)
        {
            var chosen = await freelancerrepository.GetByIdAsync(id);

            return View(chosen);
        }
        public async Task<IActionResult> SignContact(Freelancer freelancer)
        {
            var curuser = await userManager.GetUserAsync(User);
            var client = clientRepository.GetClientByPersonId(curuser.Id);
            Project project = new Project { FreelancerId = freelancer.FreelancerId, ClientId = client.ClientId, };
            return View();
        }
        public async Task<IActionResult> FindFreelancer(string inputData)
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