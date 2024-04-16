using EasyTalkWeb.Models;
using EasyTalkWeb.Models.Repositories;
using EasyTalkWeb.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EasyTalkWeb.Controllers
{
    public class ProposalController : Controller
    {
        private readonly ProposalRepository proposalRepository;
        private readonly ITechRepository techRepository;
        private readonly UserManager<Person> userManager;
        private readonly FreelancerRepository freelancerRepository;

        public ProposalController(ProposalRepository proposalRepository, ITechRepository techRepository, UserManager<Person> userManager, FreelancerRepository freelancerRepository)
        {
            this.proposalRepository = proposalRepository;
            this.techRepository = techRepository;
            this.userManager = userManager;
            this.freelancerRepository = freelancerRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var technologies = await techRepository.GetAllAsync();
            var model = new ProposalRequest
            {
                Technologies = technologies.ToList(),
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add(ProposalRequest proposalRequest)
        {
            var curuser = await userManager.GetUserAsync(User);
            var freelancer = freelancerRepository.GetFreelancerByPersonId(curuser.Id);
            var selectedTech = new List<Technology>();
            foreach (var selectedTId in proposalRequest.SelectedTech)
            {
                var selectedTagIdAsGuid = Guid.Parse(selectedTId);
                var existingTag = await techRepository.GetAsync(selectedTagIdAsGuid);
                if (existingTag != null)
                {
                    selectedTech.Add(existingTag);
                }
            }
            var proposal = new Proposal
            {
                Id = Guid.NewGuid(),
                Title = proposalRequest.Title,
                Text = proposalRequest.Text,
                ModifiedDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                FreelancerId = freelancer.FreelancerId,
            };

            await proposalRepository.AddAsync(proposal);
            proposal.Technologies = selectedTech;
            await proposalRepository.Update(proposal);

            return RedirectToAction("List");

        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var proposals = await proposalRepository.GetAllAsync();


            return View(proposals);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var proposal = await proposalRepository.GetByIdAsync(id);

            if (proposal != null)
            {
                var model = new EditProposalRequest
                {
                    Id = proposal.Id,
                    Title = proposal.Title,
                    Text = proposal.Text,
                };
                return View(model);
            }

            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditProposalRequest proposalrequest)
        {
            var curuser = await userManager.GetUserAsync(User);
            var freelancer = freelancerRepository.GetFreelancerByPersonId(curuser.Id);
            Proposal proposal = await proposalRepository.GetProposaltByIdForFreelancer(freelancer.FreelancerId, proposalrequest.Id);
            proposal.Title = proposalrequest.Title;
            proposal.Text = proposalrequest.Text;
            proposal.ModifiedDate = DateTime.UtcNow;

            await proposalRepository.Update(proposal);
            return RedirectToAction("List");

        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditProposalRequest proposalrequest)
        {
            await proposalRepository.DeleteAsync(proposalrequest.Id);

            return RedirectToAction("List");
        }
    }
}
