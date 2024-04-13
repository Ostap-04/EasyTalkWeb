using Microsoft.EntityFrameworkCore;
using Persistance.EntityConfiguration;

namespace EasyTalkWeb.Models
{
    [EntityTypeConfiguration(typeof(AttachmentConfiguration))]
    public class Attachment
    {
        public Guid AttachmentId { get; set; }

        public Guid MessageId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string? Name { get; set; }

        public string? StoragePath { get; set; }

        public Message? Message { get; set; }
    }
}
