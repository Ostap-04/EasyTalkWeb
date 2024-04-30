using EasyTalkWeb.Models.Repositories;
using EasyTalkWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EasyTalkWeb.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace EasyTalkWeb.Controllers
{
    [Authorize(Roles = "Client, Freelancer")]
    public class ProjectController: Controller
    {
        private readonly UserManager<Person> _userManager;
        private readonly ClientRepository _clientRepository;
        private readonly FreelancerRepository _freelancerRepository;
        private readonly ProjectRepository _projectRepository;

        public ProjectController(UserManager<Person> userManager, ClientRepository clientRepository, FreelancerRepository freelancerRepository, ProjectRepository projectRepository)
        {
            _userManager = userManager;
            _clientRepository = clientRepository;
            _freelancerRepository = freelancerRepository;
            _projectRepository = projectRepository;
        }

        [Authorize(Roles = "Client")]
        public async Task<IActionResult> StartProject(Guid id)
        {
            var chosen = await _freelancerRepository.GetByIdAsync(id);
            ProjectRequest project = new ProjectRequest() { FreelancerId = chosen.FreelancerId };

            return View(project);
        }

        [Authorize(Roles = "Client")]
        public async Task<IActionResult> SaveProject([FromBody]ProjectRequest request)
        {
            var freelancer = await _freelancerRepository.GetFreelancerByPersonId(request.FreelancerId);
            var client =  _clientRepository.GetClientByPersonId(request.ClientId);
            Project project = new Project()
            {
                ClientId = client.ClientId,
                CreatedDate = DateTime.UtcNow,
                FreelancerId = freelancer.FreelancerId,
                Description = request.Description,
                Name = request.Name,
                Price = request.Price,
                ModifiedDate = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                ChatId = request.ChatId

            };
            await _projectRepository.AddAsync(project);

            return RedirectToAction( "List");
        }


        public async Task<IActionResult> List()
        {
            var projects = await _projectRepository.GetAllProjects();
            return View(projects);
        }

        [HttpGet]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var project = await _projectRepository.GetByIdAsync(id);

            if (project != null)
            {
                var model = new ProjectRequest
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    Price = project.Price,
                };
                return View(model);
            }

            return View(null);
        }

        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Edit(ProjectRequest projectRequest)
        {
            Project project = await _projectRepository.GetByIdAsync(projectRequest.Id);
            project.Name = projectRequest.Name;
            project.Description = projectRequest.Description;
            project.Price = projectRequest.Price;
            project.ModifiedDate = DateTime.UtcNow;

            await _projectRepository.Update(project);
            return RedirectToAction("List");
        }

        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Delete(ProjectRequest projectRequest)
        {
            await _projectRepository.DeleteAsync(projectRequest.Id);

            return RedirectToAction("List");
        }
    }
}
