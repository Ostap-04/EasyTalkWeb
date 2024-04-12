using EasyTalkWeb.Enum;
using System.ComponentModel.DataAnnotations;

namespace EasyTalkWeb.Models.ViewModels
{
    public class RegisterViewModel
    {
        public Gender? Gender { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? Location { get; set; }

        [Required]
        [EmailAddress(ErrorMessage ="Invalid email address")]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        public Role? Role { get; set; }
    }
}
