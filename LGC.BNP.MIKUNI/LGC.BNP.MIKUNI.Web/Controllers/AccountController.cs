using LGC.BNP.MIKUNI.Web.Models;
using LGC.BNP.MIKUNI.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Web;

namespace LGC.BNP.MIKUNI.Web.Controllers
{
	[Authorize]
	public class AccountController : BaseController
	{
		private IConfiguration _config;
		private AccountService _account;
		private UtilityService _utility;
		public AccountController(IConfiguration config, AccountService account, UtilityService util) : base(account, util)
		{
			_config = config;
			_account = account;
			_utility = util;
		}
		public IActionResult Overview()
		{
			var model = _account.GetList().Result;
			foreach (var item in model)
			{
				item.id_encrypt = _utility.EncryptString(item.id.ToString()).ToString();
			}
			return View(model);
		}
		public IActionResult Edit(string? id_encrypt)
		{
			if (string.IsNullOrWhiteSpace(id_encrypt))
			{
				//add
				var new_model = new UserData();
				new_model.is_admin = true; new_model.is_active = true;
				return View(new_model);
			}
			else
			{
				//edit
				long _id = 0;
				long.TryParse(_utility.DecryptString(HttpUtility.HtmlDecode(id_encrypt)), out _id);
				var model = _account.GetById(_id).Result;
				return View(model);
			}
		}
		[HttpPost]
		public IActionResult Edit(UserData model)
		{
			model.is_admin = false; model.is_staff = false; model.is_reporter = false;
			//role
			switch (model.role_string)
			{
				case "0":
					model.is_admin = true;
					break;
				case "1":
					model.is_staff = true;
					break;
				case "2":
					model.is_reporter = true;
					break;
				default:
					break;
			}
			if (!String.IsNullOrWhiteSpace(model.id_encrypt))
			{
				long.TryParse(_utility.DecryptString(HttpUtility.HtmlDecode(model.id_encrypt)), out long _id);
				model.id = _id;
			}
			model.created_by = User.Identity.Name; model.created_date = DateTime.Now;
			model.modified_by = User.Identity.Name; model.modified_date = DateTime.Now;

			//profile // default of blank
			if (String.IsNullOrWhiteSpace(model.profile_picture))
			{
				model.profile_picture = new UserData().profile_picture;
			}

			//encrypt pass
			if (model.change_pass)
			{
				byte[] encodedPassword = new UTF8Encoding().GetBytes(model.password);
				byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
				string encodedPass = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
				model.password = encodedPass;
			}

			var result = _account.Upsert(model).Result;
			if (result.isCompleted)
			{
				TempData["SuccessMessage"] = (string.IsNullOrWhiteSpace(model.id_encrypt) ? "Insert User Completed"  : "Update User Completed");
				return RedirectToAction("Overview", "Account");
			}
			else
			{
				TempData["ErrorMessage"] = result.message[0];
				return View(model);
			}
		}
		[HttpPost]
		public IActionResult Delete(string id_encrypt)
		{
			long.TryParse(_utility.DecryptString(HttpUtility.HtmlDecode(id_encrypt)), out long _id);
			var result = _account.DeleteById(_id).Result;
			if (result.isCompleted)
			{
				TempData["SuccessMessage"] = "Delete User Completed";
				return Json(new { status = "success" });
			}
			else
			{	
				return Json(new { status = "error",  result.message });
			}
		}
	}
}
