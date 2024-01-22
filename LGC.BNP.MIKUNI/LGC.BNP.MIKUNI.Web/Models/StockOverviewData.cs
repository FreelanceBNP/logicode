namespace LGC.BNP.MIKUNI.Web.Models
{
	public class StockOverviewData
	{
		public long? id { get; set; }
		public string? id_encrypt { get; set; } = "";
		public int re_print_noo { get; set; }
		public string doc_status { get; set; }
		public string created_by { get; set; }
		public DateTime created_date { get; set; }
		public string modified_by { get; set; }
		public DateTime modified_date { get; set; }

		public long beginning_id { get; set; }
		public BeginningData project { get; set; } = new BeginningData();

		public DateTime? stamp_receiving { get; set; }
        public string? stamp_receiving_by { get; set; }
		public string? receiving_remark { get; set; }
		public DateTime? stamp_delivery { get; set; }
        public string? stamp_delivery_by { get; set; }
		public string? delivery_remark { get; set; }
		public DateTime? stamp_collection { get; set; }
        public string? stamp_collection_by { get; set; }
		public string? collection_remark { get; set; }
		public DateTime? stamp_aliquot { get; set; }
        public string? stamp_aliquot_by { get; set; }
		public string? aliquot_remark { get; set; }
		public DateTime? stamp_preparation { get; set; }
        public string? stamp_preparation_by { get; set; }
		public string? preparation_remark { get; set; }
		public DateTime? stamp_analysis { get; set; }
        public string? stamp_analysis_by { get; set; }
		public string? analysis_remark { get; set; }
		public DateTime? stamp_destroy { get; set; }
        public string? stamp_destroy_by { get; set; }
		public string? destroy_remark { get; set; }

        public string? location_name { get; set; }
        public DateTime? expire_date { get; set; }
        public string? expire_date_str { get; set; }
        public string? separate_tube_code { get; set; }
        public bool is_separate { get; set; } = false;
		public bool is_parent { get; set; } = false;
    }

    public class StockStatus
	{
		private StockStatus(string value) { Value = value; }

		public string Value { get; private set; }

		public static StockStatus STOCK { get { return new StockStatus("STOCK"); } }
        public static StockStatus DELIVERY { get { return new StockStatus("DELIVERY"); } }
        public static StockStatus SAMPLE_COLLECTION { get { return new StockStatus("SAMPLE_COLLECTION"); } }
		public static StockStatus ALIQUOT { get { return new StockStatus("ALIQUOT"); } }
		public static StockStatus SAMPLE_PREPARATION { get { return new StockStatus("SAMPLE_PREPARATION"); } }
		public static StockStatus ANALYSIS { get { return new StockStatus("ANALYSIS"); } }
		public static StockStatus DESTROY { get { return new StockStatus("DESTROY"); } }
		public override string ToString()
		{
			return Value;
		}
	}
}
