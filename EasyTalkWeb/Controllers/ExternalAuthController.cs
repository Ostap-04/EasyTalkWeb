using EasyTalkWeb.Models;
using EasyTalkWeb.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc; 
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Security.Claims;

namespace EasyTalkWeb.Controllers
{
    public class ExternalAuthController : Controller
    {
        private readonly SignInManager<Person> _signInManager;
        private readonly UserManager<Person> _userManager;


        public ExternalAuthController(UserManager<Person> userManager, SignInManager<Person> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> ExternalAuth(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalAuthCallback), "ExternalAuth", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalAuthCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                return RedirectToAction(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: false);
            if (result.Succeeded)
            {
                return RedirectToLocal("/Profile/Index");
            }
            else
            {
                Person user = new Person
                {
                    Email = info.Principal.FindFirst(ClaimTypes.Email).Value,
                    UserName = info.Principal.FindFirst(ClaimTypes.Email).Value,
                    EmailConfirmed = true
                };

                IdentityResult identResult = await _userManager.CreateAsync(user);
                if (identResult.Succeeded)
                {
                    identResult = await _userManager.AddLoginAsync(user, info);
                    if (identResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, false);
                        return View("ChooseRole");

                    }
                }
                else
                {
                    return RedirectToAction("Error", "ExternalAuth");

                }
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                ViewData["Email"] = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalAuth");
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        public IActionResult Error()
        {
            return View();
        }

        public async Task<IActionResult> ChooseRole()
        {
            return View();
        }

        public async Task<IActionResult> GetRole(RegisterViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            user.Role = model.Role;
            var res = await _userManager.AddToRoleAsync(user, model.Role);

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: false);
            if (result.Succeeded)
            {
                return RedirectToLocal("/Profile/Index");
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                ViewData["LoginProvider"] = info.LoginProvider;
                ViewData["Email"] = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalAuth");
            }
        }
    }
}
