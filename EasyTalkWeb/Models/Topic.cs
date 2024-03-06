namespace EasyTalkWeb.Models
{
    public class Topic : BaseEntity
    {
        public string? Name { get; set; }

        public Guid ProjectId { get; set; }

        public Project? Project { get; set; }

        public ICollection<Chatshot>? Chatshots { get; set; }

    }
}
