namespace LGC.BNP.MIKUNI.Web.Models
{
	public class BeginningDetailData
	{
		public long? id { get; set; }
		public string? id_encrypt { get; set; } = "";
		public long? template_build_id { get; set; }
		public long? beginning_id { get; set; }
		public string? tran_value_string { get; set; }
		public int? tran_value_number { get; set; }
		public string created_by { get; set; }
		public DateTime created_date { get; set; }

		//Addon 
		public string? template_build_code { get; set; }
	}
}
