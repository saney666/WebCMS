using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Linq;
using System.Web.Configuration;
using TSMC14B.Models;

namespace TSMC14B.Areas.Main.Models
{
    public class AlarmReportModel
    {
        #region 組織成員屬性

        [Display(Name = "Closed", ResourceType = typeof(Menu))]
        public string Type { get; set; }

        [Display(Name = "StartDate", ResourceType = typeof(Menu))]
        public string StartDate { get; set; }

        [Display(Name = "EndDate", ResourceType = typeof(Menu))]
        public string EndDate { get; set; }

        [Display(Name = "AlarmMessage", ResourceType = typeof(Menu))]
        public string AlarmMsg { get; set; }

        [Display(Name = "AlarmValue", ResourceType = typeof(Menu))]
        public string AlarmValue { get; set; }

        [Display(Name = "TriggerLimit")]
        public string TriggerLimit { get; set; }

        [Display(Name = "TagName")]
        public string TagName { get; set; }

        public Int32 AlarmID { get; set; }

        [Display(Name = "Chamber ID")]
        public string ToolID { get; set; }

        [Display(Name = "department_name", ResourceType = typeof(Menu))]
        public string Vendor { get; set; }

        [Display(Name = "課別")]
        public string GroupName { get; set; }

        [Display(Name = "Weekly", ResourceType = typeof(Menu))]
        public string Weekly { get; set; }

        [Display(Name = "_DateTime",ResourceType=typeof(Menu))]
        public string _DateTime { get; set; }

        [Display(Name = "Location", ResourceType = typeof(Menu))]
        public string Location { get; set; }

        [Display(Name = "Location ID")]
        public string LocationID { get; set; }

        [Display(Name = "Alarm Type")]
        public string AlarmType { get; set; }

        [Display(Name = "Alarm Level")]
        public Int16 AlarmLevel { get; set; }

        [Display(Name = "effectOutPut", ResourceType = typeof(Menu))]
        public string effectOutPut { get; set; }

        [Display(Name = "effectDateTime", ResourceType = typeof(Menu))]
        public string effectDateTime { get; set; }

        [Display(Name = "DealWith", ResourceType = typeof(Menu))]
        public string Status { get; set; }

        [Display(Name = "Alarm Cause")]
        public string AlarmCause { get; set; }

        [Display(Name = "ChangeParts", ResourceType = typeof(Menu))]
        public string ChangeParts { get; set; }

        [Display(Name = "PM週期(天)")]
        public string pmCycle { get; set; }

        [Display(Name = "上次PM時間")]
        public string pmDate { get; set; }

        [Display(Name = "LifeTime(天)")]
        public string PMLifeTime { get; set; }

        [Display(Name = "Closed", ResourceType = typeof(Menu))]
        public string Closed { get; set; }

        [Display(Name = "ReceptionName", ResourceType = typeof(Menu))]
        public string ReceptionName { get; set; }

        [Display(Name = "System")]
        public string System { get; set; }

        [Display(Name = "AlarmID")]
        public string PhoneCallAlarmID { get; set; }

        #endregion

        public static AlarmReportModel GetAlarm(int AlarmID, string vName, string _System, string UsrName)
        {
            DataSet CodeDS = DBConnector.executeQuery("Intouch", "SELECT AlarmId, AlarmWeek, AlarmTime_Begin AS AlarmTime, toolID, location, vendor_name,department_name, AlarmMsg, AlarmLevel,system, affect, affTime, dealWith, almCause, parts, caseClose, dealUser, pmCycle, pmDate FROM vw_AlarmDealInfo WHERE AlarmId = '" + AlarmID + "' AND department_name = '" + vName + "'");

            DataTable dt = CodeDS.Tables[0];
            // 將查詢到的資料回傳
            if (dt.Rows.Count == 0)
                return null;

            return new AlarmReportModel
            {
                AlarmID = dt.Rows[0].IsNull("AlarmId") ? (Int32)0 : dt.Rows[0].Field<Int32>("AlarmId"),
                Weekly = "W" + (dt.Rows[0].IsNull("AlarmWeek") ? "000" : dt.Rows[0].Field<string>("AlarmWeek")),
                _DateTime = dt.Rows[0].IsNull("AlarmTime") ? string.Empty : dt.Rows[0].Field<DateTime>("AlarmTime").ToString("yyyy-MM-dd HH:mm:ss"),
                ToolID = dt.Rows[0].IsNull("toolID") ? string.Empty : dt.Rows[0].Field<string>("toolID"),
                Location = dt.Rows[0].IsNull("location") ? string.Empty : dt.Rows[0].Field<string>("location"),
                //LocationID = dt.Rows[0].IsNull("location_id") ? string.Empty : dt.Rows[0].Field<string>("location_id"),
                Vendor = dt.Rows[0].IsNull("department_name") ? string.Empty : dt.Rows[0].Field<string>("department_name"),
                //GroupName = dt.Rows[0].IsNull("group_name") ? string.Empty : dt.Rows[0].Field<string>("group_name"),
                AlarmMsg = dt.Rows[0].IsNull("AlarmMsg") ? string.Empty : dt.Rows[0].Field<string>("AlarmMsg"),
                AlarmLevel = dt.Rows[0].IsNull("AlarmLevel") ? (short)500 : dt.Rows[0].Field<Int16>("AlarmLevel"),
                effectOutPut = dt.Rows[0].IsNull("affect") ? string.Empty : dt.Rows[0].Field<bool>("affect") ? "Y" : "N",
                effectDateTime = dt.Rows[0].IsNull("affTime") ? string.Empty : dt.Rows[0].Field<string>("affTime"),
                Status = dt.Rows[0].IsNull("dealWith") ? string.Empty : dt.Rows[0].Field<string>("dealWith"),
                AlarmCause = dt.Rows[0].IsNull("almCause") ? string.Empty : dt.Rows[0].Field<string>("almCause"),
                ChangeParts = dt.Rows[0].IsNull("parts") ? string.Empty : dt.Rows[0].Field<string>("parts"),
                //pmCycle = dt.Rows[0].IsNull("pmCycle") ? string.Empty : dt.Rows[0].Field<string>("pmCycle"),
                //pmDate = dt.Rows[0].IsNull("pmDate") ? string.Empty : dt.Rows[0].Field<string>("pmDate") == string.Empty ? string.Empty : dt.Rows[0].Field<string>("pmDate").Substring(0, 19),
                //PMNextTime = dt.Rows[0].IsNull("PM_Next_Time") ? string.Empty : dt.Rows[0].Field<string>("PM_Next_Time"),
                //PMLifeTime = dt.Rows[0].IsNull("PM_LifeTime") ? string.Empty : dt.Rows[0].Field<string>("PM_LifeTime"),
                Closed = dt.Rows[0].IsNull("CaseClose") ? string.Empty : dt.Rows[0].Field<bool>("CaseClose") ? "Y" : "N",
                ReceptionName = dt.Rows[0].IsNull("dealUser") || dt.Rows[0].Field<string>("dealUser").Length == 0 ? UsrName : dt.Rows[0].Field<string>("dealUser"),
                System = _System,
            };
        }

        public static IEnumerable<AlarmReportModel> ReportList(string _StartDate, string _EndDate, string ToolID, string AlarmMsg, string _Closed, string Vendor, string gname, string alarmlevel)
        {
            _StartDate = _StartDate.Replace("\\", string.Empty).Replace("\"", string.Empty);
            if (string.IsNullOrEmpty(_StartDate))
            {
                _StartDate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm");
            }
            else if (!string.IsNullOrEmpty(_StartDate))
            {
                //DateTime tempDate = Convert.ToDateTime(_StartDate);
                //_StartDate = tempDate.ToString("yyyy-MM-dd HH:mm");
            }

            if (string.IsNullOrEmpty(_EndDate))
            {
                _EndDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            }
            else if (!string.IsNullOrEmpty(_EndDate))
            {
                //DateTime tempDate = Convert.ToDateTime(_EndDate);
                //if (tempDate.Hour == 0)
                //{
                //    tempDate = tempDate.AddDays(1);
                //}
                //_EndDate = tempDate.ToString("yyyy-MM-dd HH:mm:ss");
            }

            DataTable gdt = new DataTable();
            string sqlStr = null;
            if (string.IsNullOrEmpty(ToolID))
            {
                ToolID = "*";
            }
            else
            {
                ToolID = "*" + ToolID + "*";
            }
            if (string.IsNullOrEmpty(AlarmMsg))
            {
                AlarmMsg = "*";
            }
            //else
            //{
            //    AlarmMsg = "*" + AlarmMsg + "*";
            //}
            if (string.IsNullOrEmpty(_Closed))
            {
                _Closed = "NULL";
            }
            //if (string.IsNullOrEmpty(gname))
            //{
            //    gname = "NULL";
            //}
            if (string.IsNullOrEmpty(Vendor))
            {
                sqlStr = "SELECT department_id FROM department_info --where department_name in (" + Vendor + ")";
            }
            else
            {
                sqlStr = "SELECT department_id FROM department_info where department_name in (" + Vendor + ")";
            }
            DataSet ds = DBConnector.executeQuery("Intouch", sqlStr);
            string VendorID = null;
            for (int i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
            {
                VendorID += ds.Tables[0].Rows[i][0].ToString() + ",";
            }
            if (VendorID != null) {
                VendorID = VendorID.Substring(0, VendorID.Length - 1);
            }
            
            //string sqlStr = null;
            //    sqlStr = "EXEC [dbo].[uSP_Select_AlarmDeal] '" + VendorID + "','" + ToolID + "','" + AlarmMsg + "'," + _Closed + ",'" + _StartDate + "','" + _EndDate + "'";
            DataSet DeptDS = DBConnector.executeQuery("Intouch", "EXEC [dbo].[uSP_Select_AlarmDeal] '" + VendorID + "','" + ToolID + "','" + AlarmMsg + "'," + _Closed + ",'" + _StartDate + "','" + _EndDate + "',NULL,'" + alarmlevel + "'");
            //fncARList( _StartDate,  _EndDate,  ToolID,  AlarmMsg,  _Closed,  Vendor);

            return from dept in DeptDS.Tables[0].AsEnumerable()
                   select new AlarmReportModel
                   {
                       AlarmID = dept.IsNull("AlarmId") ? (Int32)0 : dept.Field<Int32>("AlarmId"),
                       Weekly = "W" + (dept.IsNull("AlarmWeek") ? "000" : dept.Field<string>("AlarmWeek")),
                       _DateTime = dept.IsNull("AlarmTime") ? string.Empty : dept.Field<DateTime>("AlarmTime").ToString("yyyy/MM/dd HH:mm:ss"),
                       ToolID = dept.IsNull("toolID") ? string.Empty : dept.Field<string>("toolID"),
                       Location = dept.IsNull("location") ? string.Empty : dept.Field<string>("location"),
                       //LocationID = dept.IsNull("location_id") ? string.Empty : dept.Field<string>("location_id"),
                       Vendor = dept.IsNull("department_name") ? string.Empty : dept.Field<string>("department_name"),
                       //GroupName = dept.IsNull("group_name") ? string.Empty : dept.Field<string>("group_name"),
                       TagName = dept.IsNull("TagName") ? string.Empty : dept.Field<string>("TagName"),
                       AlarmValue = dept.IsNull("Value") ? string.Empty : dept.Field<string>("Value"),
                       TriggerLimit = dept.IsNull("TriggerLimit") ? string.Empty : dept.Field<string>("TriggerLimit"),
                       AlarmMsg = dept.IsNull("AlarmMsg") ? string.Empty : dept.Field<string>("AlarmMsg"),
                       AlarmLevel = dept.IsNull("AlarmLevel") ? (short)500 : dept.Field<Int16>("AlarmLevel"),
                       effectOutPut = dept.IsNull("affect") ? string.Empty : dept.Field<bool>("affect") ? "Y" : "N",
                       effectDateTime = dept.IsNull("affTime") ? string.Empty : dept.Field<string>("affTime"),
                       Status = dept.IsNull("dealWith") ? string.Empty : dept.Field<string>("dealWith"),
                       AlarmCause = dept.IsNull("almCause") ? string.Empty : dept.Field<string>("almCause"),
                       ChangeParts = dept.IsNull("parts") ? string.Empty : dept.Field<string>("parts"),
                       //pmCycle = dept.IsNull("pmCycle") ? string.Empty : dept.Field<string>("pmCycle"),
                       //pmDate = dept.IsNull("pmDate") ? string.Empty : dept.Field<string>("pmDate") == string.Empty ? string.Empty : dept.Field<string>("pmDate").Substring(0, 19),
                       //PMLifeTime = dept.IsNull("PM_LifeTime") ? string.Empty : dept.Field<Int16>("PM_LifeTime").ToString(),
                       Closed = dept.IsNull("CaseClose") ? string.Empty : dept.Field<bool>("CaseClose") ? "Y" : "N",
                       ReceptionName = dept.IsNull("dealUser") ? string.Empty : dept.Field<string>("dealUser"),
                       System = dept.IsNull("system") ? string.Empty : dept.Field<string>("system"),
                       PhoneCallAlarmID = dept.IsNull("PhoneCallAlarmID") ? string.Empty : dept.Field<string>("PhoneCallAlarmID"),
                   };
        }

        public static DataTable dtReportList(string _StartDate, string _EndDate, string ToolID, string AlarmMsg, string _Closed, string Vendor, string gname, string alarmlevel)
        {
            DataTable dt = new DataTable();

            _StartDate = _StartDate.Replace("\\", string.Empty).Replace("\"", string.Empty);
            if (string.IsNullOrEmpty(_StartDate))
            {
                _StartDate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm");
            }
            else if (!string.IsNullOrEmpty(_StartDate))
            {
                //DateTime tempDate = Convert.ToDateTime(_StartDate);
                //_StartDate = tempDate.ToString("yyyy-MM-dd HH:mm");
            }

            if (string.IsNullOrEmpty(_EndDate))
            {
                _EndDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            }
            else if (!string.IsNullOrEmpty(_EndDate))
            {
                //DateTime tempDate = Convert.ToDateTime(_EndDate);
                //if (tempDate.Hour == 0)
                //{
                //    tempDate = tempDate.AddDays(1);
                //}
                //_EndDate = tempDate.ToString("yyyy-MM-dd HH:mm");
            }

            DataTable gdt = new DataTable();

            if (string.IsNullOrEmpty(ToolID))
            {
                ToolID = "*" + ToolID + "*";
            }
            if (string.IsNullOrEmpty(AlarmMsg))
            {
                AlarmMsg = "*";
            }
            if (string.IsNullOrEmpty(_Closed))
            {
                _Closed = "NULL";
            }
            if (string.IsNullOrEmpty(gname))
            {
                gname = "NULL";
            }

            DataSet ds = DBConnector.executeQuery("Intouch", "SELECT department_id FROM department_info where department_name in (" + Vendor + ")");
            string VendorID = null;
            for (int i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
            {
                VendorID += ds.Tables[0].Rows[i][0].ToString() + ",";
            }

            VendorID = VendorID.Substring(0, VendorID.Length - 1);

            //using (SqlConnection conn = new SqlConnection())
            //{
            //    conn.ConnectionString = WebConfigurationManager.ConnectionStrings["Intouch"].ConnectionString;
            //    conn.Open();

            //    dt.Load(new System.Data.SqlClient.SqlCommand("EXEC [dbo].[uSP_Select_AlarmDeal] '" + VendorID + "','" + ToolID + "','" + AlarmMsg + "'," + _Closed + ",'" + _StartDate + "','" + _EndDate + "',NULL,'" + alarmlevel + "'", conn).ExecuteReader());
            //}

            using (SqlConnection conn = new SqlConnection())
            {
                IDbCommand DBCmd = null;
                DBCmd = new SqlCommand("EXEC [dbo].[uSP_Select_AlarmDeal] '" + VendorID + "','" + ToolID + "','" + AlarmMsg + "'," + _Closed + ",'" + _StartDate + "','" + _EndDate + "',NULL,'" + alarmlevel + "'," + gname, new SqlConnection(WebConfigurationManager.ConnectionStrings["Intouch"].ConnectionString));
                DBCmd.CommandTimeout = 30000;
                DBCmd.Connection.Open();
                dt.Load(DBCmd.ExecuteReader());
            }

            return dt;
        }

        public string Update(string UsrName)
        {
            try
            {
                // 更新資料
                bool _effectOutPut, _Closed;
                if (effectOutPut == "Y")
                    _effectOutPut = true;
                else
                    _effectOutPut = false;

                if (Closed == "Y")
                    _Closed = true;
                else
                    _Closed = false;

                DBConnector.executeSQL("Intouch", "EXEC [dbo].[uSP_Change_AlarmDeal] '" + AlarmID + "','" + System + "','" + ToolID + "'," + _effectOutPut + ",'" + effectDateTime + "','" + Status + "','" + AlarmCause + "','" + ChangeParts + "','" + pmDate + "','" + pmCycle + "'," + _Closed + ",'" + UsrName + "'");
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return null;
        }

        public static byte[] GetAlarmDealFile(string StartDate, string EndDate, string ToolID, string AlarmMsg, string Closed, string Vendor, string gname, string alarmlevel)
        {
            //DataTable dt = AlarmReportModel.dtReportList(StartDate, EndDate, ToolID, AlarmMsg, Closed, Vendor, gname, alarmlevel);
            IEnumerable<AlarmReportModel> dt = AlarmReportModel.ReportList(StartDate, EndDate, ToolID, AlarmMsg, Closed, Vendor, gname, alarmlevel);
            ExcelPackage ep = new ExcelPackage();
            ep.Workbook.Worksheets.Add("MySheet");
            ExcelWorksheet sheet1 = ep.Workbook.Worksheets["MySheet"];

            sheet1.Cells["A1"].Value = "AlarmId";
            //sheet1.Cells["B1"].Value = "週期";
            sheet1.Cells["B1"].Value = "時間";
            sheet1.Cells["C1"].Value = "ToolID";
            sheet1.Cells["D1"].Value = "數值";
            sheet1.Cells["E1"].Value = "Limit Setting";
            //sheet1.Cells["E1"].Value = "柱位";
            //sheet1.Cells["F1"].Value = "Alarm Type";
            //sheet1.Cells["F1"].Value = "LocationID";
            //sheet1.Cells["F1"].Value = "部門名稱";
            //sheet1.Cells["H1"].Value = "課別名稱";
            sheet1.Cells["F1"].Value = "Alarm 訊息";
            sheet1.Cells["G1"].Value = "AlarmID";
            //sheet1.Cells["H1"].Value = "Link";
            //sheet1.Cells["K1"].Value = "廠牌";
            //sheet1.Cells["I1"].Value = "影響生產";
            //sheet1.Cells["J1"].Value = "影響時段";
            //sheet1.Cells["K1"].Value = "處理狀況";
            //sheet1.Cells["L1"].Value = "Alarm Cause";
            //sheet1.Cells["M1"].Value = "零件更換紀錄";
            //sheet1.Cells["Q1"].Value = "上次PM時間";
            //sheet1.Cells["R1"].Value = "PM週期(天)";
            //sheet1.Cells["S1"].Value = "下次PM時間";
            //sheet1.Cells["T1"].Value = "LifeTime(天)";
            //sheet1.Cells["N1"].Value = "結案";
            //sheet1.Cells["O1"].Value = "處理人員";

            if (dt.Count() > 0)
            {
                //sheet1.Cells["A2"].LoadFromDataTable(dt, false);
                int i = 2;
                foreach (var item in dt)
                {
                    //sheet1.Cells[i + 1, 3].Style.Numberformat.Format = "yyyy-mm-dd HH:mm:ss"
                    sheet1.Cells["A" + i ].Value = item.AlarmID;//"AlarmId"
                    //sheet1.Cells["B" + i ].Value = item.Weekly;//"週期"
                    sheet1.Cells["B" + i ].Value = item._DateTime;//"時間"
                    sheet1.Cells["C" + i ].Value = item.ToolID;//"ToolID"
                    sheet1.Cells["D" + i].Value = item.AlarmValue;//"數值"
                    sheet1.Cells["E" + i].Value = item.TriggerLimit;//"Limit Setting"
                    //sheet1.Cells["E" + i ].Value = item.Location;//"柱位"
                    //sheet1.Cells["F" + (i + 2)].Value = item.LocationID;//"LocationID"
                    //sheet1.Cells["F" + i ].Value = item.Vendor;//"部門名稱"
                    sheet1.Cells["F" + i].Value = item.AlarmMsg;//"Alarm 訊息"
                    sheet1.Cells["G" + i].Value = item.PhoneCallAlarmID;//"Alarm Level"
                    //sheet1.Cells["H" + i].Value = "TrendChart";
                    //sheet1.Cells["H" + i].Hyperlink = new Uri("http://10.12.9.213/Main/MachineGroup/MyChart3?toolId=" + item.ToolID + "&TagName=" + item.TagName + "&StartDate=" + DateTime.Parse(item._DateTime).AddHours(-12).ToString("yyyy-MM-dd HH:mm:ss") + "&EndDate=" + DateTime.Parse(item._DateTime).AddHours(12).ToString("yyyy-MM-dd HH:mm:ss") + "&flag=3&Y1Y2=Y1&_=1472028498994");//"Alarm 訊息"
                    //sheet1.Cells["H" + i].Value = item.AlarmLevel;//"Alarm Level"
                    //sheet1.Cells["I" + i].Value = item.effectOutPut;//"影響生產"
                    //sheet1.Cells["K" + (i + 2)].Value = item.System;//"廠牌"
                    //sheet1.Cells["J" + i].Value = item.effectDateTime;//"影響時段"
                    //sheet1.Cells["K" + i].Value = item.Status;//"處理狀況"
                    //sheet1.Cells["L" + i].Value = item.AlarmCause;//"Alarm Cause"
                    //sheet1.Cells["M" + i].Value = item.ChangeParts;//"零件更換紀錄"
                    //sheet1.Cells["N" + i].Value = item.Closed;//"結案"
                    //sheet1.Cells["Q" + (i + 2)].Value = item.pmDate;//"上次PM時間"
                    //sheet1.Cells["R1"].Value = dt[i].pmCycle;//"PM週期(天)"
                    //sheet1.Cells["S1"].Value = dt[i].;//"下次PM時間"
                    //sheet1.Cells["T1"].Value = dt[i].PMLifeTime;//"LifeTime(天)"
                    //sheet1.Cells["O" + i].Value = item.ReceptionName;//"處理人員"
                    i++;
                }

                sheet1.Cells.AutoFitColumns();
                sheet1.Column(1).Width = 0;
            }

            return ep.GetAsByteArray();
        }
    }
}