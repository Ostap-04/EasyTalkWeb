using System;

namespace EasyTalkWeb.Models
{
    public class Message : BaseEntity
    {
        public string? Text { get; set; }

        public Guid PersonId { get; set; }

        public Guid ChatId { get; set; }

        public Person? Person { get; set; }

        public Chat? Chat { get; set; }

        public Attachment? Attachment { get; set; }
    }
}
