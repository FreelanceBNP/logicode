using LGC.BNP.MIKUNI.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LGC.BNP.MIKUNI.Web.Controllers
{
	public class BaseController : Controller
	{
		private readonly AccountService _service;
		private readonly UtilityService _util;
		public BaseController(AccountService service, UtilityService util)
		{
			_service = service;
			_util = util;
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			string _id = User.Claims.Where(c => c.Type == "user_id").Select(c => c.Value).FirstOrDefault();
			if (!string.IsNullOrEmpty(_id)) {
				long.TryParse(_id, out long longId);
				var _currLogin = _service.GetById(longId).Result;
				_currLogin.id_encrypt = _util.EncryptString(_id.ToString());
				ViewData["CurrentUser"] = _currLogin;
				base.OnActionExecuting(filterContext);
			}
		}
	}
}
