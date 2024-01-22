using LGC.BNP.MIKUNI.Web.Models;
using LGC.BNP.MIKUNI.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using System.Web;

namespace LGC.BNP.MIKUNI.Web.Controllers
{
    [Authorize]
    public class TransactionController : BaseController
    {
        private IConfiguration _config;
        private readonly AccountService _account;
        private readonly UtilityService _utility;
		private readonly MasterService _master;
		private readonly TransactionService _transaction;
		public TransactionController(IConfiguration config, AccountService account, UtilityService util, TransactionService transaction, MasterService master) : base(account, util)
		{
			_config = config;
			_account = account;
			_utility = util;
			_transaction = transaction;
			_master = master;
		}
        public IActionResult TagRegistration()
        {
			var model = _master.GetTagList().Result;

			return View(model.data);
		}
		[AllowAnonymous]
		public IActionResult Monitor()
        {
			ViewBag.url = _config.GetSection("SignalR").GetSection("HubUrl").Value;
			var model = _master.GetTagList().Result;

			return View(model.data);
		}
		[HttpPost]
		public IActionResult MappingTag(MappingRequest model)
		{
			model.update_by = User.Identity.Name;

			var result = _master.MappingTagMaster(model).Result;
			if (result.isCompleted)
			{
				return Json(new { status = "success" });
			}
			else
			{
				return Json(new { status = "error", message = result.message[0] });
			}
		}

		[HttpPost]
		public IActionResult CancelMappingTag(MappingRequest model)
		{
			model.update_by = User.Identity.Name;
			var result = _master.CancelMappingTag(model).Result;
			if (result.isCompleted)
			{
				return Json(new { status = "success" });
			}
			else
			{
				return Json(new { status = "error", message = result.message[0] });
			}
		}
		[HttpPost]
		public IActionResult SaveTag(string list_tag)
		{
			var ListTag = JsonConvert.DeserializeObject<List<TagCode>>(list_tag);
			var modelIns = new List<MasTagMapping>();
			if (ListTag.Count() > 0)
			{
				foreach (var item in ListTag)
				{
					var model_tag = new MasTagMapping()
					{
						created_by = User.Identity.Name,
						created_date = DateTime.Now,
						tag_serial = item.tag_code,
						status = "active",
						is_deleted = false,
						deleted_by = ""
					};
					modelIns.Add(model_tag);
				}
			}
			var result = _master.SaveTag(modelIns).Result;
			if (result.isCompleted)
			{
				return Json(new { status = "success", message = "Save Tag Completed" });
			}
			else
			{
				return Json(new { status = "error", message = result.message[0] });
			}
		}
		[HttpPost]
		public IActionResult DeleteTagId(DeleteTag model)
		{
			model.update_by = User.Identity.Name;
			model.is_deleted = true;
			model.deleted_by = User.Identity.Name;

			var result = _master.DeleteTagId(model).Result;
			if (result.isCompleted)
			{
				return Json(new { status = "success" });
			}
			else
			{
				return Json(new { status = "error", message = result.message });
			}
		}
		public IActionResult Loan()
        {
			var model = _transaction.GetAllBeginningList().Result;
			foreach (var item in model)
			{
				item.id_encrypt = _utility.EncryptString(item.id.ToString()).ToString();
			}
			return View(model);
		}
		public IActionResult FormLoan(string? id_encrypt)
		{
			ViewBag.ItemTubeData = _master.GetItemTubeList().Result;
			ViewBag.TemplateData = _master.GetTemplateList().Result;
			if (string.IsNullOrWhiteSpace(id_encrypt))
			{
				//add
				var new_model = new BeginningData();
				new_model.is_active = true; new_model.is_deleted = false;
				new_model.doc_status = "DRAFT";
				return View(new_model);
			}
			else
			{
				//edit
				long _id = 0;
				long.TryParse(_utility.DecryptString(HttpUtility.HtmlDecode(id_encrypt)), out _id);
				var model = _transaction.GetBeginningById(_id).Result;
				return View(model);
			}
		}

	}
}
