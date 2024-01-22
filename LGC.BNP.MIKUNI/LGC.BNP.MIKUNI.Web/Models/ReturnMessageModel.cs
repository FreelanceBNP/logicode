namespace LGC.BNP.MIKUNI.Web.Models
{
    public class ReturnMessageModel
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

        public bool isCompleted { get; set; }
    }
    public class ReturnObject<T> : ReturnMessageModel
    {
        public T data { get; set; }
    }
    public class ReturnList<T> : ReturnObject<List<T>>
    {

    }
}
