namespace LGC.BNP.MIKUNI.Web.Models
{
    public class TemplateBuildData
    {
        public long? id { get; set; }
        public string? id_encrypt { get; set; } = "";
        public string column_code { get; set; }
        public string column_display_text { get; set; }
        public string column_type { get; set; }
        public string? column_ddl_value { get; set; }
        public bool column_is_required { get; set; }
        public decimal column_seq { get; set; } = 0;
        public string created_by { get; set; }
        public DateTime created_date { get; set; }
        public string modified_by { get; set; }
        public DateTime modified_date { get; set; }
        public bool is_active { get; set; }
    }
}