using Microsoft.EntityFrameworkCore;
using Persistance.EntityConfiguration;
using System;

namespace EasyTalkWeb.Models
{
    [EntityTypeConfiguration(typeof(MessageConfiguration))]
    public class Message : BaseEntity
    {
        public string? Text { get; set; }

        public Guid PersonId { get; set; }

        public Guid ChatId { get; set; }

        public Person? Person { get; set; }

        public Chat? Chat { get; set; }

        public ICollection<Attachment>? Attachments { get; set; }
    }
}
