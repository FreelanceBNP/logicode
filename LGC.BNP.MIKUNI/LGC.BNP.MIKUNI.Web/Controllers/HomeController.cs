using LGC.BNP.MIKUNI.Web.Models;
using LGC.BNP.MIKUNI.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LGC.BNP.MIKUNI.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
		private AccountService _account;
        private readonly UtilityService _utilityService;
		public HomeController(ILogger<HomeController> logger, AccountService account, UtilityService utilityService) : base(account, utilityService)
        {
            _logger = logger;
            _account = account;
            _utilityService = utilityService;
        }

        public IActionResult Index()
        {
			var isLogin = User.Identity is { IsAuthenticated: true };
			if (isLogin)
			{
                return View();
			}
			return RedirectToAction("Signin", "Account");
        }
	
		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}