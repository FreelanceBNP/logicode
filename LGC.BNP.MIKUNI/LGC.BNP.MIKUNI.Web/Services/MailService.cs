using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Net.Mail;
using LGC.BNP.MIKUNI.Web.Models;
using System.Net;
     
namespace LGC.BNP.MIKUNI.Web.Services
{
        public interface IViewRenderService
        {
            Task<string> RenderToStringAsync(string viewName, object model);
        }
     
        public class MailService : IViewRenderService
        {
            private IConfiguration _config;
            private readonly IRazorViewEngine _razorViewEngine;
            private readonly ITempDataProvider _tempDataProvider;
            private readonly IServiceProvider _serviceProvider;
     
            public MailService(IRazorViewEngine razorViewEngine,
                ITempDataProvider tempDataProvider,
                IServiceProvider serviceProvider,
                IConfiguration config)
            {
                _config = config;
                _razorViewEngine = razorViewEngine;
                _tempDataProvider = tempDataProvider;
                _serviceProvider = serviceProvider;
            }
     
            public async Task<string> RenderToStringAsync(string viewName, object model)
            {
                var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
                var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
     
                using (var sw = new StringWriter())
                {
                    var viewResult = _razorViewEngine.FindView(actionContext, viewName, false);
     
                    if (viewResult.View == null)
                    {
                        throw new ArgumentNullException($"{viewName} does not match any available view");
                    }
     
                    var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    {
                        Model = model
                    };
     
                    var viewContext = new ViewContext(
                        actionContext,
                        viewResult.View,
                        viewDictionary,
                        new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                        sw,
                        new HtmlHelperOptions()
                    );
     
                    await viewResult.View.RenderAsync(viewContext);
                    return sw.ToString();
                }
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
        }
}