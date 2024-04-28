namespace EasyTalkWeb.Models.DTO.ChatDTOs
{
    public class ChatDTO
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public ICollection<ChatPersonDTO> Members { get; set; }
        
        public ICollection<MessageDTO> Messages { get; set; }
        
        public ChatDTO(Chat chat)
        {
            var members = new List<ChatPersonDTO>();
            var messages = new List<MessageDTO>();
            foreach (var member in chat.Persons)
                members.Add(new ChatPersonDTO(member));
            foreach (var message in chat.Messages)
                messages.Add(new MessageDTO(message));
            Id = chat.Id;
            Description = chat.Description;
            Name = chat.Name;
            Members = members;
            Messages = messages;
        }
    }
}
