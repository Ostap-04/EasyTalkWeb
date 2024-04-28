using EasyTalkWeb.Enum;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace EasyTalkWeb.Models.ViewModels
{
    public class ProfileViewModel
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? Location { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Specialization {  get; set; }

        public ICollection<Technology>? Technologies {  get; set; }

        public ICollection<string> SelectedTechnologiesData { get; set; } = new List<string>();
    }
}
