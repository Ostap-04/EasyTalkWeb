using System.ComponentModel.DataAnnotations;

namespace EasyTalkWeb.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
