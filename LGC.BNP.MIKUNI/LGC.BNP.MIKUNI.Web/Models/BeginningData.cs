namespace LGC.BNP.MIKUNI.Web.Models
{
	public class BeginningData
	{
        public long? id { get; set; }
		public string? id_encrypt { get; set; } = "";
		public string tran_code { get; set; }
		
		public long? template_id { get; set; }
		public string template_code { get; set; }
		public string template_name { get; set; }

		public string remark { get; set; }
		
		public long? itemtube_id { get; set; }
		public string itemtube_code { get; set; }
		public string itemtube_description { get; set; }

		public string created_by { get; set; }
		public DateTime created_date { get; set; }
		public string modified_by { get; set; }
		public DateTime modified_date { get; set; }
		public bool is_active { get; set; }
		public bool is_deleted { get; set; }

		public List<BeginningDetailData> details { get; set; } = new List<BeginningDetailData>();
        public string? details_str { get; set; }
        public string doc_status { get; set; }

        public TemplateData templete { get; set; }
        //Template Build
        public List<TemplateBuildData> tempBuildDetails { get; set; }

		//all about status
		public bool is_print { get; set; } = false;
		public bool is_entry { get; set; } = false;
	}
	public class InputValueData
	{
		public string input { get; set; }
        public string value { get; set; }
    }

	public class ProjectStatus
	{
		private ProjectStatus(string value) { Value = value; }

		public string Value { get; private set; }

		public static ProjectStatus PRINTED { get { return new ProjectStatus("PRINTED"); } }
		public static ProjectStatus DRAFT { get { return new ProjectStatus("DRAFT"); } }
		public static ProjectStatus NEW { get { return new ProjectStatus("NEW"); } }
		public static ProjectStatus LAUNCH { get { return new ProjectStatus("LAUNCH"); } }
		public static ProjectStatus RELEASED { get { return new ProjectStatus("RELEASED"); } }
		public static ProjectStatus COMPLETED { get { return new ProjectStatus("COMPLETED"); } }

		public override string ToString()
		{
			return Value;
		}
	}

}
