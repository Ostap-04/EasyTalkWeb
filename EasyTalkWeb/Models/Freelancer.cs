using Microsoft.EntityFrameworkCore;
using Persistance.EntityConfiguration;
using System;

namespace EasyTalkWeb.Models
{
    [EntityTypeConfiguration(typeof(FreelancerConfiguration))]
    public class Freelancer
    {
        public Guid FreelancerId { get; set; }

        public Guid? PersonId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? Specialization { get; set; }

        public int Rate { get; set; }

        public Person? Person { get; set; } = null!;

        public ICollection<Project>? Projects { get; set; }

        public ICollection<Technology>? Technologies { get; set; }

        public ICollection<Proposal>? Proposals { get; set; }
    }
}
