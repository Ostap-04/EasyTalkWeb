using EasyTalkWeb.Models;
using EasyTalkWeb.Models.Repositories;
using EasyTalkWeb.Models.ViewModels;
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
            return View(freelancers as List<Freelancer>);
        }
        //public async Task<IActionResult> SignContract(Guid id)
        //{
        //    var chosen = await freelancerrepository.GetByIdAsync(id);
        //    ProjectRequest project = new ProjectRequest() { FreelancerId = chosen.FreelancerId };

        //    return View(project);
        //}
        //public async Task<IActionResult> SignContact(ProjectRequest  request )
        //{
        //    var curuser = await userManager.GetUserAsync(User);
        //    var client = clientRepository.GetClientByPersonId(curuser.Id);
        //    Project project = new Project()
        //    {
        //        Client = client, ClientId = client.ClientId, CreatedDate = DateTime.UtcNow,
        //        FreelancerId = request.FreelancerId, Description = request.Description, Name = request.Name,
        //        Price = request.Price, ModifiedDate = DateTime.UtcNow, Id = Guid.NewGuid()
        //    };

        //    return RedirectToAction("", "Project");
        //}
    }
}