using EasyTalkWeb.Models;
using EasyTalkWeb.Models.Repositories;
using EasyTalkWeb.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EasyTalkWeb.Controllers
{
    public class FindJobController : Controller
    {
        private readonly JobPostRepository jobPostRepository;
        private readonly ProposalRepository proposalRepository;
        private readonly TechRepository techRepository;
        private readonly UserManager<Person> userManager;
        private readonly FreelancerRepository freelancerRepository;

        public FindJobController(JobPostRepository jobPostRepository, ProposalRepository proposalRepository, TechRepository techRepository, UserManager<Person> userManager, FreelancerRepository freelancerRepository)
        {
            this.jobPostRepository = jobPostRepository;
            this.proposalRepository = proposalRepository;
            this.techRepository = techRepository;
            this.userManager = userManager;
            this.freelancerRepository = freelancerRepository;
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
            var jobpost = await jobPostRepository.GetByIdAsyncWthProposals(id);
            ProposalRequest proposalrequest = new ProposalRequest() {JobPost = jobpost, JobPostId = id};

            return View(proposalrequest); 
        }
        public async Task<IActionResult> SaveProposal(ProposalRequest proposalRequest)
        {
            var curuser = await userManager.GetUserAsync(User);
            var freelancer = await freelancerRepository.GetFreelancerByPersonId(curuser.Id);
            var jobpost = await jobPostRepository.GetByIdAsyncWthProposals(proposalRequest.JobPostId);
            //var selectedTech = new List<Technology>();
            //foreach (var selectedTId in proposalRequest.SelectedTech)
            //{
            //    var existingTag = await techRepository.GetByIdAsync(Guid.Parse(selectedTId));
            //    if (existingTag != null)
            //    {
            //        selectedTech.Add(existingTag);
            //    }
            //}
            var proposal = new Proposal
            {
                Id = Guid.NewGuid(),
                Title = proposalRequest.Title,
                Text = proposalRequest.Text,
                ModifiedDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                FreelancerId = freelancer.FreelancerId,
                JobPostId=proposalRequest.JobPostId
            };

            await proposalRepository.AddAsync(proposal);
            //proposal.Technologies = selectedTech;
            await proposalRepository.Update(proposal);
            return View("Index");
        }
    }
}
