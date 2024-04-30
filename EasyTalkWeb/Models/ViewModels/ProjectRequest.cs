namespace EasyTalkWeb.Models.ViewModels
{
    public class ProjectRequest
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public Guid FreelancerId { get; set; }
        public Guid ClientId { get; set; }
        public Guid ChatId { get; set; }

    }
}
