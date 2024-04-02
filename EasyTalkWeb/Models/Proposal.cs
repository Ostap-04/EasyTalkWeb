namespace EasyTalkWeb.Models
{
	public class Proposal : BaseEntity
	{
		public string Title { get; set; }
		public string Text { get; set; }
		public Guid FreelancerId { get; set; }
		public Freelancer Freelancer { get; set; }
	}
}
