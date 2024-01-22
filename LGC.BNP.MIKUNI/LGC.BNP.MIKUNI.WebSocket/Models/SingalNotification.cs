using System;
using System.Collections.Generic;
using System.Text;

namespace LGC.BNP.MIKUNI.WebSocket.Models
{
    public class JoinGroupRequest
    {
        public string Username { get; set; }
        public string Channel { get; set; }
    }

    public class JoinGroupResponse
    {
        public string ConnectionID { get; set; }
    }

    public class UserOnline
    {
        public int CurrentConnection { get; set; }
    }
    public class NoticeModel
    {
        public string username { get; set; }


    }
}


