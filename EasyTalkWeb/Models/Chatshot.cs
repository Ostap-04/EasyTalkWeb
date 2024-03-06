namespace EasyTalkWeb.Models
{
    public class Chatshot
    {
        public Guid Id { get; set; }

        public DateOnly CreatedDate { get; set; }

        public Guid TopicId { get; set; }

        public Topic? Topic { get; set; }
    }
}
