using EasyTalkWeb.Models;
using EasyTalkWeb.Models.ViewModels.ChatViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EasyTalkWeb.Components
{
    [ViewComponent(Name = "ChatPreviewComponent")]
    public class ChatPreviewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ChatPreviewViewModel model)
        {
            return View("Default", model);
        }
    }
}
