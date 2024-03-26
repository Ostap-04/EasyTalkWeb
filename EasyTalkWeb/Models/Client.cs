using System;

namespace EasyTalkWeb.Models
{
    public class Client
    {
        public Guid? PersonId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public Person? Person { get; set; }

        public ICollection<Project>? Projects { get; set; }

        public ICollection<JobPost> JobPosts { get; set; }
    }
}
