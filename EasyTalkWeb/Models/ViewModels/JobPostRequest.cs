namespace EasyTalkWeb.Models.ViewModels
{
    public class JobPostRequest : BaseEntity
    {
        public string? Title { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }

        public Guid ClientId { get; set; }
        public ICollection<Technology>? Technologies { get; set; }
        public string[] SelectedTech{ get; set; } = Array.Empty<string>();
    }
}

