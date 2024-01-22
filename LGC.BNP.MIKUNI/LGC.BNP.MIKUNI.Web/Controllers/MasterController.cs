using LGC.BNP.MIKUNI.Web.Models;
using LGC.BNP.MIKUNI.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using Newtonsoft.Json;
using System.Web;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
namespace LGC.BNP.MIKUNI.Web.Controllers
{
	[Authorize]
	public class MasterController : BaseController
	{
		private IConfiguration _config;
		private readonly AccountService _account;
		private readonly UtilityService _utility;
		private readonly MasterService _master;
		private readonly MailService _mail;
		public MasterController(MailService mail, IConfiguration config, AccountService account, UtilityService util, MasterService master) : base(account, util)
		{
			_config = config;
			_account = account;
			_utility = util;
			_master = master;
			_mail = mail;
		}
		public IActionResult ItemMaster()
		{
			var allow_day = _config.GetSection("AllowInfo").GetSection("allow_day").Value;
			ViewBag.allow_day = allow_day;
			// var model = _master.GetItemTubeList().Result;
			var model = _master.GetItemMaster().Result;
			return View(model.data);
		}
		public IActionResult ConfigMember()
		{
			ViewBag.title = "Config Member";
			return View();
		}

		public IActionResult getItemMaster()
		{
			var model = _master.GetItemMaster().Result;
			var dataTag = _master.GetTagList().Result;
			if (model.isCompleted)
			{
				return Json(new { status = "success", data = model.data, dataTag = dataTag.data });
			}
			else
			{
				return Json(new { status = model.message[0], data = new List<MasterItem>() });
			}
		}
		public IActionResult EditItemMaster(string? mas_item_id)
		{
			var optionComType = _master.GetComputerType().Result.data;
			var optionComBrand = _master.GetComputerBrand().Result.data;

			if (string.IsNullOrWhiteSpace(mas_item_id))
			{
				//add
				var new_model = new MasterItem();
				// new_model.is_active = true; new_model.is_deleted = false;
				new_model.computer_brand_list = optionComBrand;
				new_model.computer_type_list = optionComType;
				return View(new_model);
			}
			else
			{
				//edit
				// long _id = 0;
				// long.TryParse(_utility.DecryptString(HttpUtility.HtmlDecode(id_encrypt)), out _id);
				// var model = _master.GetItemTubeById(mas_item_id).Result;
				var model = _master.GetItemMasterById(long.Parse(mas_item_id)).Result.data;
				model.computer_brand_list = optionComBrand;
				model.computer_type_list = optionComType;
				return View(model);
			}
		}
		[HttpPost]
		// public IActionResult EditItemTube(ItemTubeData model)
		public IActionResult EditItemMaster(MasterItem model)
		{

			model.created_by = User.Identity.Name; model.created_date = DateTime.Now;
			model.update_by = User.Identity.Name; model.update_date = DateTime.Now;

			var result = _master.UpsertItemMaster(model).Result;
			if (result.isCompleted)
			{
				TempData["SuccessMessage"] = (string.IsNullOrWhiteSpace(model.mas_item_id.ToString()) ? "Insert Item Tube Completed" : "Update Item Tube	 Completed");
				return RedirectToAction("ItemMaster", "Master");
			}
			else
			{
				TempData["ErrorMessage"] = result.message[0];
				return View(model);
			}
		}

		[HttpPost]
		public IActionResult MappingTag(MappingRequest model)
		{

			// model.created_by = User.Identity.Name; model.created_date = DateTime.Now;
			// model.update_by = User.Identity.Name; model.update_date = DateTime.Now;
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
		// public IActionResult EditItemTube(ItemTubeData model)
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
				// return
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
		[HttpPost]
		public IActionResult DeleteItemMasItem(DeleteItemMasItemReq model)
		{
			var result = _master.DeleteItemMasItem(model).Result;
			if (result.isCompleted)
			{
				TempData["SuccessMessage"] = "Delete Item Tube Completed";
				return Json(new { status = "success" });
			}
			else
			{
				return Json(new { status = "error", result.message });
			}
		}
		public IActionResult Template()
		{
			// var model = _master.GetTemplateList().Result;
			var model = _master.GetTagList().Result;

			return View(model.data);
		}
		public IActionResult EditTemplate(string? id_encrypt)
		{
			ViewBag.TemplateBuildData = _master.GetTemplateBuildList().Result.OrderBy(o => o.column_seq).ToList();
			if (string.IsNullOrWhiteSpace(id_encrypt))
			{
				//add
				var new_model = new TemplateData();
				new_model.is_active = true; new_model.is_deleted = false;
				return View(new_model);
			}
			else
			{
				//edit
				long _id = 0;
				long.TryParse(_utility.DecryptString(HttpUtility.HtmlDecode(id_encrypt)), out _id);
				var model = _master.GetTemplateById(_id).Result;
				return View(model);
			}
		}
		[HttpPost]
		public IActionResult EditTemplate(TemplateData model)
		{
			if (!String.IsNullOrWhiteSpace(model.id_encrypt))
			{
				long.TryParse(_utility.DecryptString(HttpUtility.HtmlDecode(model.id_encrypt)), out long _id);
				model.id = _id;
			}
			model.created_by = User.Identity.Name; model.created_date = DateTime.Now;
			model.modified_by = User.Identity.Name; model.modified_date = DateTime.Now;

			var result = _master.UpsertTemplate(model).Result;
			if (result.isCompleted)
			{
				TempData["SuccessMessage"] = (string.IsNullOrWhiteSpace(model.id_encrypt) ? "Insert Template Completed" : "Update	 Template Completed");
				return RedirectToAction("Template", "Master");
			}
			else
			{
				TempData["ErrorMessage"] = result.message[0];
				return View(model);
			}
		}
		[HttpPost]
		public IActionResult DeleteTemplate(string id_encrypt)
		{
			long.TryParse(_utility.DecryptString(HttpUtility.HtmlDecode(id_encrypt)), out long _id);
			var result = _master.DeleteTemplateById(_id).Result;
			if (result.isCompleted)
			{
				TempData["SuccessMessage"] = "Delete Template Completed";
				return Json(new { status = "success" });
			}
			else
			{
				return Json(new { status = "error", result.message });
			}
		}

		//Template Build
		public IActionResult TemplateBuild()
		{
			var model = _master.GetTemplateBuildList().Result;
			foreach (var item in model)
			{
				item.id_encrypt = _utility.EncryptString(item.id.ToString()).ToString();
			}
			return View(model);
		}
		public IActionResult EditTemplateBuild(string? id_encrypt)
		{
			if (string.IsNullOrWhiteSpace(id_encrypt))
			{
				//add
				var new_model = new TemplateBuildData();
				new_model.is_active = true;
				return View(new_model);
			}
			else
			{
				//edit
				long _id = 0;
				long.TryParse(_utility.DecryptString(HttpUtility.HtmlDecode(id_encrypt)), out _id);
				var model = _master.GetTemplateBuildById(_id).Result;
				return View(model);
			}
		}
		[HttpPost]
		public IActionResult EditTemplateBuild(TemplateBuildData model)
		{
			if (!String.IsNullOrWhiteSpace(model.id_encrypt))
			{
				long.TryParse(_utility.DecryptString(HttpUtility.HtmlDecode(model.id_encrypt)), out long _id);
				model.id = _id;
			}
			model.created_by = User.Identity.Name; model.created_date = DateTime.Now;
			model.modified_by = User.Identity.Name; model.modified_date = DateTime.Now;

			var result = _master.UpsertTemplateBuild(model).Result;
			if (result.isCompleted)
			{
				TempData["SuccessMessage"] = (string.IsNullOrWhiteSpace(model.id_encrypt) ? "Insert Template Build Completed" : "Update Template Build Completed");
				return RedirectToAction("TemplateBuild", "Master");
			}
			else
			{
				TempData["ErrorMessage"] = result.message[0];
				return View(model);
			}
		}
		[HttpPost]
		public IActionResult DeleteTemplateBuild(string id_encrypt)
		{
			long.TryParse(_utility.DecryptString(HttpUtility.HtmlDecode(id_encrypt)), out long _id);
			var result = _master.DeleteTemplateById(_id).Result;
			if (result.isCompleted)
			{
				TempData["SuccessMessage"] = "Delete Template Build Completed";
				return Json(new { status = "success" });
			}
			else
			{
				return Json(new { status = "error", result.message });
			}
		}
		[HttpPost]
		public IActionResult GetTypeDataList()
		{
			var model = _master.GetComputerType().Result;
			if (model.isCompleted)
			{
				return Json(new { status = "success", data = model.data });
			}
			else
			{
				return Json(new { status = "error", message = model.message[0] });
			}
		}
		[HttpPost]
		public IActionResult GetBrandDataList()
		{
			var model = _master.GetComputerBrand().Result;
			if (model.isCompleted)
			{
				return Json(new { status = "success", data = model.data });
			}
			else
			{
				return Json(new { status = "error", message = model.message[0] });
			}
		}
		[HttpPost]
		public IActionResult SaveType(RequestSave model)
		{
			model.action_by = User.Identity.Name;
			var res = _master.SaveDataType(model).Result;
			if (res.isCompleted)
			{
				return Json(new { status = "success" });
			}
			else
			{
				return Json(new { status = "error", message = res.message[0] });
			}
		}
		[HttpPost]
		public IActionResult SaveBrand(RequestSave model)
		{
			model.action_by = User.Identity.Name;
			var res = _master.SaveDataBrand(model).Result;
			if (res.isCompleted)
			{
				return Json(new { status = "success" });
			}
			else
			{
				return Json(new { status = "error", message = res.message[0] });
			}
		}
		[HttpPost]
		public IActionResult CheckDupplicate(DataCSV model)
		{
			model.action_by = User.Identity.Name;
			var res = _master.CheckDupplicate(model).Result;
			if (res.isCompleted)
			{
				return Json(new { status = "success" });
			}
			else
			{
				return Json(new { status = "error", message = res.message[0], data = res.data });
			}
		}
		[HttpPost]
		public IActionResult SaveDataCSV()
		{
			var action_by = User.Identity.Name;
			var res = _master.SaveDataCSV(action_by).Result;
			if (res.isCompleted)
			{
				return Json(new { status = "success" });
			}
			else
			{
				return Json(new { status = "error", message = res.message[0] });
			}
		}
		public IActionResult ManageType()
		{
			var model = _master.GetComputerType().Result;
			return View(model.data);
		}
		public IActionResult ManageBrand()
		{
			var model = _master.GetComputerBrand().Result;
			return View(model.data);
		}

		[HttpPost]
		public IActionResult getInfo()
		{
			try
			{
				var cat = new List<string>();
				var dep = new List<string>();
				var _allow_date = _config.GetSection("AllowInfo").GetSection("allow_date").Value;
				return Json(new { status = true, data = new { allow_date = _allow_date } });

			}
			catch (Exception ex)
			{
				return Json(new { status = false, message = ex.Message });
			}

		}
		[HttpPost]
		public IActionResult updateAllow(requset_allow_masterItem model)
		{
			model.update_by = User.Identity.Name;
			var res = _master.UpdateAllow(model).Result;
			if (res.isCompleted)
			{
				return Json(new { status = "success" });
			}
			else
			{
				return Json(new { status = "error", message = res.message[0] });
			}
		}
		[HttpPost]
		public IActionResult GetreportAllowData()
		{
			var res = _master.GetreportAllowData().Result;
			if (res.isCompleted)
			{
				return Json(new { status = "success", data = res.data });
			}
			else
			{
				return Json(new { status = "error", message = res.message[0] });
			}
		}
		[HttpPost]
		public IActionResult SetVIPData(SetVipDataRequest model)
		{
			model.action_by = User.Identity.Name;
			var res = _master.SetVIPData(model).Result;
			if (res.isCompleted)
			{
				return Json(new { status = "success" });
			}
			else
			{
				return Json(new { status = "error", message = res.message[0] });
			}
		}
		[HttpPost]
		public IActionResult SetActiveDataMember(SetActiveRequest model)
		{
			model.action_by = User.Identity.Name;
			var res = _master.SetActiveDataMember(model).Result;
			if (res.isCompleted)
			{
				return Json(new { status = "success" });
			}
			else
			{
				return Json(new { status = "error", message = res.message[0] });
			}
		}
		[HttpPost]
		public IActionResult GetVIPData()
		{
			var res = _master.GetVIPData().Result;
			if (res.isCompleted)
			{
				return Json(new { status = "success", data = res.data });
			}
			else
			{
				return Json(new { status = "error", message = res.message[0] });
			}
		}
		[AllowAnonymous]
		[HttpPost]
		public IActionResult GetDataItem()
		{
			var res = _master.GetDataItem().Result;
			if (res.isCompleted)
			{
				return Json(new { status = "success", data = res.data });
			}
			else
			{
				return Json(new { status = "error", message = res.message[0] });
			}
		}
		[AllowAnonymous]
		[HttpPost]
		public IActionResult GetEventTag()
		{
			var res = _master.GetEventTag().Result;
			if (res.isCompleted)
			{
				return Json(new { status = "success", data = res.data });
			}
			else
			{
				return Json(new { status = "error", message = res.message[0] });
			}
		}
		[AllowAnonymous]
		[HttpPost]
		public IActionResult SaveTagEvent(save_event_tag_list model)
		{
			var res = _master.SaveTagEvent(model).Result;
			if (res.isCompleted)
			{
				return Json(new { status = "success" });
			}
			else
			{
				return Json(new { status = "error", message = res.message[0] });
			}
		}
		[HttpPost]
		public IActionResult GetReportLog(MasLogRequest model)
		{
			var res = _master.GetReportLog(model).Result;
			if (res.isCompleted)
			{
				return Json(new { status = "success", data = res.data });
			}
			else
			{
				return Json(new { status = "error", message = res.message[0] });
			}
		}
		[AllowAnonymous]
		[HttpPost]
		public IActionResult GetReportTagLog(TagLogRequest model)
		{
			var res = _master.GetReportTagLog(model).Result;
			if (res.isCompleted)
			{
				return Json(new { status = "success", data = res.data });
			}
			else
			{
				return Json(new { status = "error", message = res.message[0] });
			}
		}

		public async Task<ReturnMessageModel> sendDataToUser()
		{
			var res = new ReturnMessageModel();
			try
			{
				var title = "DMIT30299";
				var getItemMaster = _master.GetItemMasterDetail(title).Result;
				if (getItemMaster.isCompleted)
				{
					if (!string.IsNullOrEmpty(getItemMaster.data.emp_email))
					{
						var body = await _mail.RenderToStringAsync("Master/EmailTemplate", getItemMaster.data);
						var modelSend = new MailSender()
						{
							send_to = getItemMaster.data.emp_email,
							send_name = getItemMaster.data.emp_name,
							subject = "แจ้งเตือนการยืมอุปกรณ์ " + title,
							body = body
						};
						var sendMail = await SendMail(modelSend);
						if (sendMail.isCompleted)
						{
							res.isCompleted = true;
							res.message.Add("success");
						}
						else
						{
							res.isCompleted = false;
							res.message.Add(sendMail.message[0]);
						}
					} else {
						res.isCompleted = false;
						res.message.Add("ไม่พบ Email ของผู้ใช้งาน");
					}
				}
				else
				{
					res.isCompleted = false;
					res.message.Add(getItemMaster.message[0]);
				}
			}
			catch (Exception ex)
			{
				res.isCompleted = false;
				res.message.Add(ex.Message);
			}
			return res;
		}

		public async Task<ReturnMessageModel> SendMail(MailSender model)
		{
			var res = new ReturnMessageModel();
			try
			{

				var FromAddress = _config.GetSection("MailSetting").GetSection("FromAddress").Value;
				var FromName = _config.GetSection("MailSetting").GetSection("FromName").Value;
				var FromPassword = _config.GetSection("MailSetting").GetSection("FromPassword").Value;
				var Host = _config.GetSection("MailSetting").GetSection("Host").Value;
				var Port = _config.GetSection("MailSetting").GetSection("Port").Value;


				var fromAddress = new MailAddress(FromAddress, FromName);
				var toAddress = new MailAddress(model.send_to, model.send_name);
				string fromPassword = FromPassword;
				string subject = model.subject;
				var newModal = new MailSender();
				var body = model.body;

				var smtp = new SmtpClient
				{
					Host = Host,
					Port = Convert.ToInt32(Port),
					EnableSsl = true,
					DeliveryMethod = SmtpDeliveryMethod.Network,
					UseDefaultCredentials = false,
					Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
				};
				using (var message = new MailMessage(fromAddress, toAddress)
				{
					Subject = subject,
					Body = body,
					IsBodyHtml = true
				})
				{
					smtp.Send(message);
				}
				res.isCompleted = true;
				res.message.Add("success");
			}
			catch (Exception ex)
			{
				res.isCompleted = false;
				res.message.Add(ex.Message);
			}
			return res;

		}

		[HttpPost]
		public IActionResult CheckAllowPerson()
		{
			var res = _master.CheckAllowPerson().Result;
			if (res.isCompleted)
			{
				return Json(new { status = "success" });
			}
			else
			{
				return Json(new { status = "error", message = res.message[0] });
			}
		}

	}
}
