using EasyTalkWeb.Models;
using EasyTalkWeb.Models.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EasyTalkWeb.Controllers
{
    public class FindJobController : Controller
    {
        private readonly JobPostRepository jobPostRepository;

        public FindJobController(JobPostRepository jobPostRepository)
        {
            this.jobPostRepository = jobPostRepository;
        }
        public async Task<IActionResult> Index(string searchString)
        {
            var jobPosts = await jobPostRepository.GetAllAsync();
            if(!String.IsNullOrEmpty(searchString))
            {
                jobPosts=jobPosts.Where(n=>n.Title.Contains(searchString)).ToList();
            }
            return View(jobPosts);
        }

        public async Task<IActionResult> WriteProposal(Guid id)
        { 
            var jobpost = await jobPostRepository.GetByIdAsync(id);

            return View(); 
        }
    }
}
