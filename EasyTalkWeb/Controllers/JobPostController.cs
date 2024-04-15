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
        private readonly IJobPostRepository jobPostRepository;
        private readonly ITechRepository techRepository;
        private readonly UserManager<Person> userManager;

        public JobPostController(IJobPostRepository jobPostRepository,ITechRepository techRepository, UserManager<Person> userManager)
        {
            this.jobPostRepository = jobPostRepository;
            this.techRepository = techRepository;
            this.userManager = userManager;
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
            var jobpost = new JobPost
            {
                Id = Guid.NewGuid(),
                Title = jobPostRequest.Title,
                Price = jobPostRequest.Price,
                Description = jobPostRequest.Description,
                ModifiedDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                ClientId = userManager.Users.First().Id,
                Client = new Client { ClientId = userManager.Users.First().Id, CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow }

            };
            var selectedTech = new List<Technology>();
            //map tags from selected tags 
            foreach (var selectedTId in jobPostRequest.SelectedTech)
            {
                var selectedTagIdAsGuid = Guid.Parse(selectedTId);
                var existingTag = await techRepository.GetAsync(selectedTagIdAsGuid);
                if (existingTag != null)
                {
                    selectedTech.Add(existingTag);
                }
            }
            //mapping tags back to domain model
            jobpost.Technologies = selectedTech;
            await jobPostRepository.AddAsync(jobpost);
            return RedirectToAction("Add");

        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            //call the repo
            var jobPosts = await jobPostRepository.GetAllAsync();


            return View(jobPosts);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            //Retrieve the result from the repo
            var jobPost = await jobPostRepository.GetAsync(id);


            if (jobPost != null)
            {
                var model = new EditJobPostRequest
                {
                    Title = jobPost.Title,
                    Price = jobPost.Price,
                    Description = jobPost.Description,
                    ModifiedDate = DateTime.UtcNow,
                    CreatedDate = jobPost.CreatedDate,
                    ClientId = jobPost.ClientId,

                };
                return View(model);
            }

            //pass data to view
            return View(null);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditJobPostRequest jobpostrequest)
        {
            var jobpost = new JobPost
            {
                Id = jobpostrequest.Id,
                Title = jobpostrequest.Title,
                Price = jobpostrequest.Price,
                Description = jobpostrequest.Description,
                ModifiedDate = DateTime.UtcNow,
                CreatedDate = jobpostrequest.CreatedDate,
                ClientId = jobpostrequest.ClientId,
            };
            
            var updatedPost= await jobPostRepository.UpdateAsync(jobpost);
            if (updatedPost!= null)
            {
                //show success 
                return RedirectToAction("Edit");
            }
            return RedirectToAction("Edit");

        }
        [HttpPost]
        public async Task<IActionResult> Delete(EditJobPostRequest jobPostRequest)
        {
            var deletedPost = await jobPostRepository.DeleteAsync(jobPostRequest.Id);
            if (deletedPost != null)
            {
                return RedirectToAction("List");
            }
            //show error
            return RedirectToAction("Edit", new { id = jobPostRequest.Id });
        }

    }
}
