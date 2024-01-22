using System;
using System.Collections.Generic;
using System.Text;

namespace LGC.BNP.MIKUNI.WebSocket.Models
{
    public class ReturnMessage
    {
        private List<string> _message = new List<string>();
        public List<string> message
        {
            get
            {
                return _message;
            }
            set
            {
                if (value == null)
                    _message = new List<string>();
                else
                    _message = value;
            }
        }

        public bool iscompleted { get; set; }
    }
    public class ReturnObject<T> : ReturnMessage
    {
        public T data { get; set; }
    }
    public class ReturnList<T> : ReturnObject<List<T>>
    {
        //public Int32 totalitem { get; set; }
        //public Int32 TotalPage { get; set; }
    }
}
