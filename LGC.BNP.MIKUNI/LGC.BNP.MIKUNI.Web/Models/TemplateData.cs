namespace LGC.BNP.MIKUNI.Web.Models
{
	public class TemplateData
	{
		public long? id { get; set; }
		public string? id_encrypt { get; set; } = "";
		public string template_code { get; set; }
		public string template_name { get; set; }
		public string template_description { get; set; }
		public string created_by { get; set; }
		public DateTime created_date { get; set; }
		public string modified_by { get; set; }
		public DateTime modified_date { get; set; }
		public bool is_active { get; set; }
		public bool is_deleted { get; set; }
        public string? template_build_str { get; set; }
        public long[]? template_build_id { get; set; }
    }
	public class TemplateDetailData
	{
        public long? id { get; set; }
        public long templete_id { get; set; }
        public long template_build_id { get; set; }
    }

}
