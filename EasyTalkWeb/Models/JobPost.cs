namespace EasyTalkWeb.Models
{
	public class JobPost : BaseEntity
	{
		public Guid? Id { get; set; }	
		public string? Title {  get; set; }
		public decimal? Price { get; set; }
		public string? Description { get; set; }

		public Client Client { get; set; }
	}
}
