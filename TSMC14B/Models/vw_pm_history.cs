//------------------------------------------------------------------------------
// <auto-generated>
//    這個程式碼是由範本產生。
//
//    對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//    如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace TSMC14B.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class vw_pm_history
    {
        public int pmHistoryID { get; set; }
        public short plc_id { get; set; }
        public string tool_id { get; set; }
        public System.DateTime pm_set { get; set; }
        public Nullable<System.DateTime> pm_unset { get; set; }
        public string memo { get; set; }
        public string @operator { get; set; }
        public Nullable<bool> pm_flag { get; set; }
        public Nullable<bool> ts_flag { get; set; }
        public Nullable<bool> backup_flag { get; set; }
        public Nullable<bool> cmsError_flag { get; set; }
        public Nullable<bool> other_flag { get; set; }
        public System.DateTime update_date { get; set; }
        public string gname { get; set; }
        public string lid { get; set; }
        public string loc { get; set; }
        public string locSide { get; set; }
        public string tName { get; set; }
        public Nullable<short> vid { get; set; }
        public string vName { get; set; }
        public string vendor_Intouch { get; set; }
    }
}