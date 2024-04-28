namespace EasyTalkWeb.Models.ViewModels.ChatViewModels
{
    public class CreateChatViewModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public ICollection<Person>? Persons { get; set; }
    }
}
