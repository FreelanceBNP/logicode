namespace LGC.BNP.MIKUNI.Web.Models
{
    public class RequestSave
    {
        public string data { get; set; }
        public string type { get; set; }
        public string id { get; set; }
        public string action_by { get; set; }
    }
    public class ResSaveCSV
    {
        public List<DataCSV> list_duppplicate { get; set; }
    }
    public class DataCSV
    {
        public string data_csv { get; set; }
        public string action_by { get; set; }
        public string type { get; set; }
    }
    public class ComputerType
    {
        public long computer_type_id { get; set; }
        public string computer_type { get; set; }
        public bool is_active { get; set; }
        public bool is_deleted { get; set; }
        public DateTime update_date { get; set; }
        public DateTime created_date { get; set; }
        public string update_by { get; set; }

    }
    public class ComputerBrand
    {
        public long computer_brand_id { get; set; }
        public string computer_brand { get; set; }
        public bool is_active { get; set; }
        public bool is_deleted { get; set; }
        public DateTime update_date { get; set; }
        public DateTime created_date { get; set; }
        public string update_by { get; set; }
    }

    public class MasterItem
    {
        public long mas_item_id { get; set; }
        public string title { get; set; }
        public string fix_asset { get; set; }
        public string top_no { get; set; }
        public string status { get; set; }
        public string details { get; set; }
        public string computer_type { get; set; }
        public string computer_brand { get; set; }
        public string serial_number { get; set; }
        public string computer_model { get; set; }
        public string mit_ad_user { get; set; }
        public DateTime start_date { get; set; }
        public DateTime return_date { get; set; }
        public string location { get; set; }
        public string location_no { get; set; }
        public string location_sub { get; set; }
        public string mit_name { get; set; }
        public string emp_name { get; set; }
        public string section_code { get; set; }
        public string section_code_2 { get; set; }
        public string description { get; set; }
        public string created_by { get; set; }
        public DateTime created_date { get; set; }
        public string sections_code { get; set; }
        public List<ComputerBrand> computer_brand_list { get; set; }
        public List<ComputerType> computer_type_list { get; set; }
        public bool is_deleted { get; set; }
        public string deleted_by { get; set; }
        public DateTime update_date { get; set; }
        public string update_by { get; set; }
        public bool is_allow { get; set; }
        public DateTime aging_date { get; set; }
        public string type_allow { get; set; }
        public bool is_vip { get; set; }
        public bool is_del_vip { get; set; }
        public string emp_email { get; set; }

        public string allow_day { get; set; }

    }
    public class MasLogRequest
    {
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string title { get; set; }
        public string fix_asset { get; set; }
        public string emp_name { get; set; }
        public string serial_number { get; set; }
    }
    public class TagLogRequest
    {
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string title { get; set; }
        public string fix_asset { get; set; }
        public string emp_name { get; set; }
        public string serial_number { get; set; }
    }
    public class TagLog
    {
        public string? title { get; set; } = "";
        public string? fix_asset { get; set; } = "";
        public string? computer_type { get; set; } = "";
        public string? computer_brand { get; set; } = "";
        public string? computer_model { get; set; } = "";
        public string? emp_name { get; set; } = "";
        public string? mit_name { get; set; } = "";
        public string? mit_ad_user { get; set; } = "";
        public string? serial_number { get; set; } = "";
        public DateTime? tag_location_date { get; set; } = null;
        public bool? is_alert { get; set; } = false;
        public string? type_allow { get; set; } = "";
    }
    public class MasterItemLog
    {
        public long? mas_item_id { get; set; } = 0;
        public string? title { get; set; } = "";
        public string? fix_asset { get; set; } = "";
        public string? top_no { get; set; } = "";
        public string? status { get; set; } = "";
        public string? details { get; set; } = "";
        public string? computer_type { get; set; } = "";
        public string? computer_brand { get; set; } = "";
        public string? serial_number { get; set; } = "";
        public string? computer_model { get; set; } = "";
        public string? mit_ad_user { get; set; } = "";
        public DateTime? start_date { get; set; } = null;
        public DateTime? return_date { get; set; } = null;
        public string? location { get; set; } = "";
        public string? location_no { get; set; } = "";
        public string? location_sub { get; set; } = "";
        public string? mit_name { get; set; } = "";
        public string? emp_name { get; set; } = "";
        public string? section_code { get; set; } = "";
        public string? section_code_2 { get; set; } = "";
        public string? description { get; set; } = "";
        public string? created_by { get; set; } = "";
        public DateTime? created_date { get; set; } = null;
        public string? sections_code { get; set; } = "";
        public List<ComputerBrand> computer_brand_list { get; set; } = null;
        public List<ComputerType> computer_type_list { get; set; } = null;
        public bool? is_deleted { get; set; } = false;
        public string? deleted_by { get; set; } = "";
        public DateTime? update_date { get; set; } = null;
        public string? update_by { get; set; } = "";
        public bool? is_allow { get; set; } = false;
        public DateTime? aging_date { get; set; } = null;
        public DateTime? log_date { get; set; } = null;
        public string? type_allow { get; set; } = "";
        public bool? is_vip { get; set; } = false;
        public bool? is_del_vip { get; set; } = false;



    }
    public class MasterItemCSV
    {
        public string mas_item_id { get; set; }
        public string title { get; set; }
        public string fix_asset { get; set; }
        public string top_no { get; set; }
        public string status { get; set; }
        public string details { get; set; }
        public string computer_type { get; set; }
        public string computer_brand { get; set; }
        public string serial_number { get; set; }
        public string computer_model { get; set; }
        public string mit_ad_user { get; set; }
        public string start_date { get; set; }
        public string return_date { get; set; }
        public string location { get; set; }
        public string location_no { get; set; }
        public string location_sub { get; set; }
        public string mit_name { get; set; }
        public string emp_name { get; set; }
        public string section_code { get; set; }
        public string section_code_2 { get; set; }
        public string description { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public string sections_code { get; set; }
        public string computer_brand_list { get; set; }
        public string computer_type_list { get; set; }
        public string is_deleted { get; set; }
        public string deleted_by { get; set; }
        public string update_date { get; set; }
        public string update_by { get; set; }
        public string emp_email { get; set; }


    }

    public class MasTagMapping : MappingItemDetail
    {
        public long tag_id { get; set; }
        public string tag_serial { get; set; }
        public DateTime created_date { get; set; }
        public string created_by { get; set; }
        public string mas_item_id { get; set; }
        public string status { get; set; }
        public DateTime update_date { get; set; }
        public string update_by { get; set; }
        public bool is_deleted { get; set; }
        public string deleted_by { get; set; }
    }

    public class MappingItemDetail
    {
        public string title { get; set; }
        public string fix_asset { get; set; }
        public string top_no { get; set; }
        public string status_item { get; set; }
        public string details { get; set; }
        public string computer_type { get; set; }
        public string computer_brand { get; set; }
        public string serial_number { get; set; }
        public string computer_model { get; set; }
        public string mit_ad_user { get; set; }
    }
    public class TagCode
    {
        public string tag_code { get; set; }
    }

    public class MappingRequest
    {
        public string tag_id { get; set; }
        public string mas_item_id { get; set; }
        public string update_by { get; set; }
    }

    public class DeleteTag
    {
        public long tag_id { get; set; }
        public string update_by { get; set; }
        public bool is_deleted { get; set; }
        public string deleted_by { get; set; }

    }
    public class DeleteItemMasItemReq
    {
        public long mas_item_id { get; set; }
        public string action_by { get; set; }

    }
    public class requset_allow_masterItem
    {
        public long mas_item_id { get; set; }
        public bool is_allow { get; set; }
        public string allow_day { get; set; }
        public string aging_date { get; set; }
        public string update_by { get; set; }
        public string type_allow { get; set; }


    }

    public class ReportAllowData
    {
        public string title { get; set; }
        public string fix_asset { get; set; }
        public string computer_type { get; set; }
        public string computer_brand { get; set; }
        public string serial_number { get; set; }
        public string computer_model { get; set; }
        public string mit_name { get; set; }
        public string emp_name { get; set; }
        public string description { get; set; }
        public bool is_allow { get; set; }
        public string type_allow { get; set; }
        public string aging_date { get; set; }
    }

    public class SetVipDataRequest
    {
        public string employee_id { get; set; }
        public string name { get; set; }
        public string desciption { get; set; }
        public bool is_active { get; set; }
        public string type_request { get; set; }
        public string action_by { get; set; }
        public bool is_delete { get; set; }
    }

    public class SetActiveRequest
    {
        public string action_by { get; set; }
        public string status { get; set; }
        public string employee_id { get; set; }
    }
    public class event_tag_model
    {
        public string id { get; set; }
        public bool is_alert { get; set; }
        public string type_allow { get; set; }
        public DateTime aging_date { get; set; }
        public DateTime created_date { get; set; }
        public string title { get; set; }
        public string emp_name { get; set; }
        public string computer_brand { get; set; }
        public string computer_model { get; set; }
        public string serial_number { get; set; }
        public string computer_type { get; set; }
        public bool is_allow { get; set; }
        public bool is_del_vip { get; set; }
        public bool is_vip { get; set; }

    }
    public class request_save_event_tag
    {
        public string id { get; set; }
        public bool is_alert { get; set; }
        public string aging_date { get; set; }
        public string title { get; set; }
        public string emp_name { get; set; }
        public bool is_allow { get; set; }
        public string type_allow { get; set; }


    }
    public class save_event_tag_list
    {
        public List<request_save_event_tag> eventList { get; set; }
    }

    public class CheckNotice {
        public string mas_item_id  { get; set; }

        public string title  { get; set; }
        public string fix_asset { get; set; }
        public string emp_name { get; set; }
        public string details { get; set; }
        public string computer_type { get; set; }
        public string computer_brand { get; set; }
        public string computer_model { get; set; }
        public string serial_number { get; set; }

        public DateTime aging_date  { get; set; }

        public DateTime notice_date  { get; set; }

        public DateTime now_date  { get; set; }
        public bool is_send { get; set; } = false;

    }
    public class SendMailLog {
        public string title { get; set; }
        public bool is_send { get; set; }
        public DateTime create_date { get; set; }

    }
}
