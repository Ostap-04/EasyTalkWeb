using EasyTalkWeb.Models;
using EasyTalkWeb.Models.Repositories;
using EasyTalkWeb.Models.ViewModels;
using EasyTalkWeb.Models.ViewModels.ChatViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EasyTalkWeb.Controllers
{
    [Authorize(Roles = "Client")]
    public class JobPostController : Controller
    {
        private readonly JobPostRepository _jobPostRepository;
        private readonly TechRepository _techRepository;
        private readonly UserManager<Person> _userManager;
        private readonly ClientRepository _clientRepository;

        public JobPostController(JobPostRepository jobPostRepository, TechRepository techRepository, UserManager<Person> userManager , ClientRepository clientRepository )
        {
            _jobPostRepository = jobPostRepository;
            _techRepository = techRepository;
            _userManager = userManager;
            _clientRepository = clientRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var technologies = await _techRepository.GetAllAsync();
            var model = new JobPostRequest
            {
                Technologies = technologies.ToList(),
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add(JobPostRequest jobPostRequest)
        {
            var curuser = await _userManager.GetUserAsync(User);
            var client = _clientRepository.GetClientByPersonId(curuser.Id);
            var selectedTech = new List<Technology>();
            foreach (var selectedTId in jobPostRequest.SelectedTech)
            {
                var existingTag = await _techRepository.GetByIdAsync(Guid.Parse(selectedTId));
                if (existingTag != null)
                {
                    selectedTech.Add(existingTag);
                }
            }
            var jobpost = new JobPost
            {
                Id = Guid.NewGuid(),
                Title = jobPostRequest.Title,
                Price = jobPostRequest.Price,
                Description = jobPostRequest.Description,
                ModifiedDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                ClientId = client.ClientId,
            };

            await _jobPostRepository.AddAsync(jobpost);
            jobpost.Technologies = selectedTech;
            await _jobPostRepository.Update(jobpost);

            return RedirectToAction("List");

        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var jobPosts = await _jobPostRepository.GetAllAsync();
            return View(jobPosts);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var jobPost = await _jobPostRepository.GetByIdAsync(id);

            if (jobPost != null)
            {
                var model = new EditJobPostRequest
                {
                    Id = jobPost.Id,
                    Title = jobPost.Title,
                    Price = jobPost.Price,
                    Description = jobPost.Description,
                };
                return View(model);
            }

            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditJobPostRequest jobpostrequest)
        {
            var curuser = await _userManager.GetUserAsync(User);
            var client = _clientRepository.GetClientByPersonId(curuser.Id);
            JobPost jobpost = await _jobPostRepository.GetJobPostByIdForClient(client.ClientId, jobpostrequest.Id);
            jobpost.Title = jobpostrequest.Title;
            jobpost.Price = jobpostrequest.Price;
            jobpost.Description = jobpostrequest.Description;
            jobpost.ModifiedDate = DateTime.UtcNow;
            
            await _jobPostRepository.Update(jobpost);
            return RedirectToAction("List");

        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditJobPostRequest jobPostRequest)
        {
             await _jobPostRepository.DeleteAsync(jobPostRequest.Id);
          
            return RedirectToAction("List");
        }

        public async Task<IActionResult> ViewProposals(Guid id)
        {
            var jobpost = await _jobPostRepository.GetByIdAsyncWthProposals(id);
            CreateChatViewModel createChatViewModel = new CreateChatViewModel()
            {
                JobPost = jobpost
            };
            return View(createChatViewModel);
        }
    }
}
