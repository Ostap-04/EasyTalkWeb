using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyTalkWeb.Controllers
{
    public class ProfileController : Controller
    {
        [Authorize (Roles = "Client, Freelancer")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
