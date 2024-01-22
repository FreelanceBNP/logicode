using LGC.BNP.MIKUNI.Web.Models;
using LGC.BNP.MIKUNI.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;

namespace LGC.BNP.MIKUNI.Web.Controllers
{
	public class SecurityController : Controller
	{
		private AccountService _account;
		public SecurityController( AccountService account)
		{
			_account = account;
		}
		public IActionResult Signin()
		{
			return View(new UserData());
		}
		[HttpPost]
		public IActionResult Signin(UserData model)
		{
			var _chkUser = _account.PostLogin(model).Result;
			if (!String.IsNullOrWhiteSpace(_chkUser.error_message))
			{
				return View(_chkUser);
			}
			else
			{
				var identity = new ClaimsIdentity(new[] {
					new Claim(ClaimTypes.Name, model.username),
					new Claim(ClaimTypes.Role, _account.GetRoleById(_chkUser.id.Value).Result),
					new Claim("user_id", _chkUser.id.Value.ToString())
				
								}, CookieAuthenticationDefaults.AuthenticationScheme);
				var principal = new ClaimsPrincipal(identity);
				var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
				{
					IsPersistent = true,
					ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
				});
				return RedirectToAction("Index", "Home");
			}
		}
		[HttpGet]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync();
			return RedirectToAction("Signin");
		}
	}
}
