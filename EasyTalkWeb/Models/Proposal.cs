namespace EasyTalkWeb.Models
{
	public class Proposal : BaseEntity
	{
		public Guid Id {  get; set; }
		public string Title { get; set; }
		public string Text { get; set; }

		public Freelancer Freelancer { get; set; }
	}
}
