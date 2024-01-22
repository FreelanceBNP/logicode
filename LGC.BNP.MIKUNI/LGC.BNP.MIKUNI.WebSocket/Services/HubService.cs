using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LGC.BNP.MIKUNI.WebSocket.Models;

namespace LGC.BNP.MIKUNI.WebSocket.Services
{
    public class HubService : Hub
    {
        private static Dictionary<string, string> Players = new Dictionary<string, string>();

        private readonly IHubContext<HubService> _hubContext;
        public HubService(IHubContext<HubService> hubContext)
        {
            _hubContext = hubContext;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("onConnected", "connected");
                await base.OnConnectedAsync();
               
            }
            catch
            {

            }
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            try
            {
                var connectionId = Context.ConnectionId;

                if (Players.ContainsKey(connectionId))
                {
                    await Groups.RemoveFromGroupAsync(connectionId, Players[connectionId]);
                    //Console.WriteLine(connectionId + " Leave group " + Players[connectionId]);
                }
                await _hubContext.Clients.All.SendAsync("onDisConnected", "connected");
                await base.OnDisconnectedAsync(ex);
            }
            catch
            {

            }

        }

        public async Task<ReturnObject<JoinGroupResponse>> JoinGroup(JoinGroupRequest request)
        {
            var result = new ReturnObject<JoinGroupResponse>();
            try
            {
                var connectionId = Context.ConnectionId;

                if (!Players.ContainsKey(connectionId))
                {
                    Players.Add(connectionId, request.Channel);
                }
                else
                {
                    await Groups.RemoveFromGroupAsync(connectionId, Players[connectionId]);
                    //Console.WriteLine(connectionId + " Leave group " + Players[connectionId]);

                    Players[Context.ConnectionId] = request.Channel;
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, request.Channel);

                result.iscompleted = true;
                result.data = new JoinGroupResponse() { ConnectionID = Context.ConnectionId };
                result.message.Add("OK");

                //Console.WriteLine(Context.ConnectionId + " Has Join " + groupName);
            }
            catch (Exception ex)
            {
                result.iscompleted = false;
                result.message.Add(ex.Message);
            }

            return result;
        }

        public async Task<ReturnMessage> LeaveGroup(string groupName)
        {
            var result = new ReturnMessage();
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
                result.iscompleted = true;
                result.message.Add("OK");

                //Console.WriteLine(Context.ConnectionId + " Leave group " + groupName);
            }
            catch (Exception ex)
            {
                result.iscompleted = false;
                result.message.Add(ex.Message);
            }

            return result;
        }

        public async Task SetAlarm(string message)
        {
            await _hubContext.Clients.All.SendAsync("onReceiveAlarm", message);
        }

    }
}