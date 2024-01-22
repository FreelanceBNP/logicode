using LGC.BNP.MIKUNI.Web.Models;
using LGC.BNP.MIKUNI.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;

namespace LGC.BNP.MIKUNI.Web.Controllers
{
	public class StockController : BaseController
	{
		private AccountService _account;
		private IConfiguration _config;
		private readonly UtilityService _utility;
		private readonly MasterService _master;
		private readonly MailService _mail;
		public StockController(MailService mail, IConfiguration config, AccountService account, UtilityService util, MasterService master) : base(account, util)
		{
			_config = config;
			_account = account;
			_utility = util;
			_master = master;
			_mail = mail;
		}
		public IActionResult Overview()
		{
			ViewBag.title = "Stock Overview";
				var model = _account.GetList().Result;
			foreach (var item in model)
			{
				item.id_encrypt = _utility.EncryptString(item.id.ToString()).ToString();
			}
			return View(model);
		}
	}
}
