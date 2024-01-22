namespace LGC.BNP.MIKUNI.Web.Models
{
	public class ItemTubeData
	{
        public long? id { get; set; }
		public string? id_encrypt { get; set; } = "";
		public string itemtube_code { get; set; }
        public string itemtube_description { get; set; }
		public string created_by { get; set; }
		public DateTime created_date { get; set; }
		public string modified_by { get; set; }
		public DateTime modified_date { get; set; }
		public bool is_active { get; set; }
        public bool is_deleted { get; set; }


    }
	public class MailSender {
		public string send_to { get; set; }
		public string send_name { get; set; }
		public string subject { get; set; }
		public string body { get; set; }
	}

	public class MailUser {
		public string name { get; set; }
		public string email { get; set; }
	}

	public class ConfigSetting {
		public string FromAddress { get; set; }
		public string FromName { get; set; }
		public string FromPassword { get; set; }
		public string Host { get; set; }
		public string Port { get; set; }
		public string text { get; set; }
		public string TimeSaveTag { get; set; }
	}
}
