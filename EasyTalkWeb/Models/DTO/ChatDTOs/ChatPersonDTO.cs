namespace EasyTalkWeb.Models.DTO.ChatDTOs
{
    public class ChatPersonDTO
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Location { get; set; }

        public ChatPersonDTO(Person person)
        {
            Id = person.Id;
            Email = person.Email;
            FirstName = person.FirstName;
            LastName = person.LastName;
            Location = person.Location;
        }
    }
}
