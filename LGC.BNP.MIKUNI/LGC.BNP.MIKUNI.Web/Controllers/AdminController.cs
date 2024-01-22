using LGC.BNP.MIKUNI.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Dynamic;
using System.Reflection;
using LGC.BNP.MIKUNI.Web.Models;
namespace LGC.BNP.MIKUNI.Web.Controllers
{
	[Authorize]
	public class AdminController : BaseController
	{
		private IConfiguration _config;
		private readonly AccountService _account;
		private readonly UtilityService _utility;
		public AdminController(IConfiguration config, AccountService account, UtilityService util) : base(account, util)
		{
			_config = config;
			_account = account;
			_utility = util;
		}
		public IActionResult Setting()
		{
			var _allow = _config.GetValue<string>("AllowInfo:allow_day");
			var FromAddress = _config.GetValue<string>("MailSetting:FromAddress");
			var FromName = _config.GetValue<string>("MailSetting:FromName");
			var FromPassword = _config.GetValue<string>("MailSetting:FromPassword");
			var Host = _config.GetValue<string>("MailSetting:Host");
			var Port = _config.GetValue<string>("MailSetting:Port");
			var TimeSaveTag = _config.GetValue<string>("TimeSaveTag");
			ViewBag.AllowDays = _allow;
			ViewBag.FromAddress = FromAddress;
			ViewBag.FromName = FromName;
			ViewBag.FromPassword = FromPassword;
			ViewBag.Host = Host;
			ViewBag.Port = Port;
			ViewBag.TimeSaveTag = TimeSaveTag;
			var txtstring = "teewaffle";
			var encrypt = _utility.ConvertStringToHex(txtstring);
			ViewBag.txtstring = encrypt;
			var decryptor = _utility.ConvertHexToString(encrypt);
			ViewBag.decryptor = decryptor;
			return View();
		}
		[HttpPost]
		public virtual ActionResult SaveAllowDays(ConfigSetting model)
		{
			try
			{
				var FromAddress = model.FromAddress;
				var FromName = model.FromName;
				var FromPassword = model.FromPassword;
				var Host = model.Host;
				var Port = model.Port;
				var text = model.text;
				var TimeSaveTag = model.TimeSaveTag;

				var appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
				var json = System.IO.File.ReadAllText(appSettingsPath);

				var jsonSettings = new JsonSerializerSettings();
				dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(json, jsonSettings);
				config.AllowInfo.allow_day = text;
				config.MailSetting.FromAddress = FromAddress;
				config.MailSetting.FromName = FromName;
				config.MailSetting.FromPassword = FromPassword;
				config.MailSetting.Host = Host;
				config.MailSetting.Port = Port;
				config.TimeSaveTag = TimeSaveTag;
				var newJson = JsonConvert.SerializeObject(config, Formatting.Indented, jsonSettings);
				System.IO.File.WriteAllText(appSettingsPath, newJson);

				return Json(new { status = "success" });
			}
			catch (Exception ex)
			{
				return Json(new { status = "error", message = ex.Message });
			}

		}
	}
}
