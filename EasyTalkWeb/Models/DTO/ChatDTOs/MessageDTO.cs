namespace EasyTalkWeb.Models.DTO.ChatDTOs
{
    public class MessageDTO
    {
        public Guid Id { get; set; }
        
        public string Text { get; set; }
        
        public Guid SenderId { get; set; }

        public string SenderName { get; set; }
        
        public DateTime Date { get; set; }

        public MessageDTO(Message message)
        {
            Id = message.Id;
            Text = message.Text;
            SenderId = message.PersonId;
            SenderName = $"{message.Person.FirstName} {message.Person.LastName}";
            Date = message.CreatedDate;
        }
    }
}
