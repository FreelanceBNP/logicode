using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.SignalR;
using LGC.BNP.MIKUNI.WebSocket.Services;
namespace LGC.BNP.MIKUNI.WebSocket.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IHubContext<HubService> _hubContext;


        public NotificationController(ILogger<NotificationController> logger, IHubContext<HubService> hubContext)
        {
            _hubContext = hubContext;
        }

        [Route("autobank/pushnoti")]
        [HttpPost]
        public async Task<ActionResult<string>> noti([FromForm] string tag_code , [FromForm] string type)
        {
            try
            {
                JObject returnModel = new JObject {
                    { "tag_code" , tag_code },
					 { "type" , type }
			};
                string jsonString = JsonConvert.SerializeObject(returnModel);
                await _hubContext.Clients.All.SendAsync("notification", jsonString);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "OK";
        }


    }
}
