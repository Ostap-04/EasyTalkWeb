namespace EasyTalkWeb.Models.ViewModels
{
    public class EditProposalRequest : BaseEntity
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public ICollection<Technology>? Technologies { get; set; }
        public string[] SelectedTech { get; set; } = Array.Empty<string>();
    }
}
