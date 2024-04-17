using EasyTalkWeb.Models;
using EasyTalkWeb.Models.Repositories;
using EasyTalkWeb.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EasyTalkWeb.Controllers
{
    public class JobPostController : Controller
    {
        private readonly JobPostRepository jobPostRepository;
        private readonly ITechRepository techRepository;
        private readonly UserManager<Person> userManager;
        private readonly ClientRepository clientRepository;

        public JobPostController(JobPostRepository jobPostRepository,ITechRepository techRepository, UserManager<Person> userManager , ClientRepository clientRepository )
        {
            this.jobPostRepository = jobPostRepository;
            this.techRepository = techRepository;
            this.userManager = userManager;
            this.clientRepository = clientRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var technologies = await techRepository.GetAllAsync();
            var model = new JobPostRequest
            {
                Technologies = technologies.ToList(),
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add(JobPostRequest jobPostRequest)
        {
            var curuser = await userManager.GetUserAsync(User);
            var client = clientRepository.GetClientByPersonId(curuser.Id);
            var selectedTech = new List<Technology>();
            foreach (var selectedTId in jobPostRequest.SelectedTech)
            {
                var selectedTagIdAsGuid = Guid.Parse(selectedTId);
                var existingTag = await techRepository.GetAsync(selectedTagIdAsGuid);
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

            await jobPostRepository.AddAsync(jobpost);
            jobpost.Technologies = selectedTech;
            await jobPostRepository.Update(jobpost);

            return RedirectToAction("List");

        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var jobPosts = await jobPostRepository.GetAllAsync();


            return View(jobPosts);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var jobPost = await jobPostRepository.GetByIdAsync(id);

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
            var curuser = await userManager.GetUserAsync(User);
            var client = clientRepository.GetClientByPersonId(curuser.Id);
            JobPost jobpost = await jobPostRepository.GetJobPostByIdForClient(client.ClientId, jobpostrequest.Id);
            jobpost.Title = jobpostrequest.Title;
            jobpost.Price = jobpostrequest.Price;
            jobpost.Description = jobpostrequest.Description;
            jobpost.ModifiedDate = DateTime.UtcNow;
            
            await jobPostRepository.Update(jobpost);
            return RedirectToAction("List");

        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditJobPostRequest jobPostRequest)
        {
             await jobPostRepository.DeleteAsync(jobPostRequest.Id);
          
            return RedirectToAction("List");
        }
    }
}
