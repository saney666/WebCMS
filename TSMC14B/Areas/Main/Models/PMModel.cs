using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Configuration;
using TSMC14B.Models;

namespace TSMC14B.Areas.Main.Models
{
    public class PMModel
    {

        #region 組織成員屬性

        [Display(Name = "pmCycle", ResourceType = typeof(Menu))]
        public string pmCycle { get; set; }

        [Display(Name = "pmNextTime", ResourceType = typeof(Menu))]
        public string pmNextTime { get; set; }

        [Display(Name = "LifeTime(day)")]
        public string PMLifeTime { get; set; }

        [Display(Name = "_DateTime", ResourceType = typeof(Menu))]
        public string _DateTime { get; set; }

        [Display(Name = "課別名稱")]
        public string GroupName { get; set; }

        [Display(Name = "Tool ID")]
        public string ToolID { get; set; }

        [Display(Name = "Location ID")]
        public string LocationID { get; set; }

        [Display(Name = "Location", ResourceType = typeof(Menu))]
        public string Location { get; set; }

        [Display(Name = "Vendor", ResourceType = typeof(Menu))]
        public Int16 VendorID { get; set; }
        public string Vendor { get; set; }

        [Display(Name = "department_name", ResourceType = typeof(Menu))]
        public Int16 department_id { get; set; }
        public string department_name { get; set; }

        [Display(Name = "status", ResourceType = typeof(Menu))]
        public string Status { get; set; }

        [Display(Name = "pmHistoryID")]
        public string pmHistoryID { get; set; }

        [Display(Name = "plcID")]
        public string plcID { get; set; }

        [Display(Name = "pmSet",ResourceType = typeof(Menu))]
        public string pmSet { get; set; }

        [Display(Name = "pmUnset",ResourceType = typeof(Menu))]
        public string pmUnset { get; set; }

        [Display(Name = "memo", ResourceType = typeof(Menu))]
        public string memo { get; set; }

        [Display(Name = "Operator", ResourceType = typeof(Menu))]
        public string _operator { get; set; }

        [Display(Name = "PM")]
        public string pmFlag { get; set; }

        [Display(Name = "Trouble Shooting")]
        public string tsFlag { get; set; }

        [Display(Name = "backupFlag")]
        public string backupFlag { get; set; }

        [Display(Name = "cmsErrorFlag")]
        public string cmsErrorFlag { get; set; }

        [Display(Name = "Pumping PM")]
        public string PumpingDone { get; set; }

        [Display(Name = "SEX PM")]
        public string SEXDone { get; set; }

        [Display(Name = "other", ResourceType = typeof(Menu))]
        public string otherFlag { get; set; }

        [Display(Name = "TemporaryPFlag", ResourceType = typeof(Menu))]
        public string TemporaryPFlag { get; set; }

        [Display(Name = "ptDone")]
        public string ptDone { get; set; }

        [Display(Name = "UpdateDate")]
        public string UpdateDate { get; set; }

        [Display(Name = "gName")]
        public string gName { get; set; }

        [Display(Name = "locSide")]
        public string locSide { get; set; }

        [Display(Name = "tName")]
        public string tName { get; set; }

        [Display(Name = "vendorIntouch")]
        public string vendorIntouch { get; set; }

        [Display(Name = "Bellow")]
        public string Bellow { get; set; }

        [Display(Name = "Turning", ResourceType = typeof(Menu))]
        public string Turning { get; set; }

        [Display(Name = "ThreeWay", ResourceType = typeof(Menu))]
        public string ThreeWay { get; set; }

        [Display(Name = "HandValve", ResourceType = typeof(Menu))]
        public string HandValve { get; set; }

        [Display(Name = "bsHead", ResourceType = typeof(Menu))]
        public string bsHead { get; set; }

        [Display(Name = "GasValve", ResourceType = typeof(Menu))]
        public string GasValve { get; set; }

        [Display(Name = "LPipe", ResourceType = typeof(Menu))]
        public string LPipe { get; set; }

        [Display(Name = "Entry")]
        public string Entry { get; set; }

        [Display(Name = "Inlet")]
        public string Inlet { get; set; }

        [Display(Name = "MixingBox")]
        public string MixingBox { get; set; }

        [Display(Name = "Reactor")]
        public string Reactor { get; set; }

        [Display(Name = "Tank")]
        public string Tank { get; set; }

        [Display(Name = "Outlet")]
        public string Outlet { get; set; }

        [Display(Name = "Tpipe", ResourceType = typeof(Menu))]
        public string Tpipe { get; set; }

        [Display(Name = "LPipe", ResourceType = typeof(Menu))]
        public string Pmlpipe { get; set; }

        [Display(Name = "Upturning", ResourceType = typeof(Menu))]
        public string Upturning { get; set; }

        [Display(Name = "ms", ResourceType = typeof(Menu))]
        public string ms { get; set; }

        [Display(Name = "Inlet")]
        public string Inlet_TP { get; set; }

        [Display(Name = "MixingBox")]
        public string MixingBox_TP { get; set; }

        [Display(Name = "Reactor")]
        public string Reactor_TP { get; set; }

        [Display(Name = "Tank")]
        public string Tank_TP { get; set; }

        [Display(Name = "Outlet")]
        public string Outlet_TP { get; set; }

        [Display(Name = "Tpipe", ResourceType = typeof(Menu))]
        public string Tpipe_TP { get; set; }

        [Display(Name = "LPipe", ResourceType = typeof(Menu))]
        public string Pmlpipe_TP { get; set; }

        [Display(Name = "Upturning", ResourceType = typeof(Menu))]
        public string Upturning_TP { get; set; }

        [Display(Name = "ms", ResourceType = typeof(Menu))]
        public string ms_TP { get; set; }
        #endregion

        //設定PM/解離PM
        public string EditPM(string toolId, string selectType, string pmFlag, string memo, string usrID, string bellow, string turning, string threeway, string handvalve, string bshead, string gasvalve, string lpipe, string entry, string inlet, string mixingbox, string reactor, string tank, string outlet, string tpipe, string pmlpipe, string upturning, string ms, string inlet_TP, string mixingbox_TP, string reactor_TP, string tank_TP, string outlet_TP, string tpipe_TP, string pmlpipe_TP, string upturning_TP, string ms_TP)
        {
            string DBMsg = null;

            string[] arrToolid = null;
            arrToolid = toolId.Split(',');

            string flag = string.Empty;
            if (!string.IsNullOrEmpty(selectType))
            {
                string[] arrType = null;

                arrType = selectType.Split(',');
                foreach (string i in arrType)
                {
                    flag += "@" + i + "=1,";
                }
                flag = "," + flag.Substring(0, flag.Length - 1);
            }

            foreach (string i in arrToolid)
            {
                // 更新資料
                //string tempStr = i + "/" + pmFlag;
                try
                {
                    using (tsmc14BDataContext db = new tsmc14BDataContext())
                    {
                        var pid = (from row in db.vw_eq_status where row.chamberName == i select row.pid).FirstOrDefault();

                        DBConnector.executeSQL("Intouch", "EXEC [dbo].[uSP_Change_PMTrigger] 1,'" + i + "'," + pmFlag);
                        DBConnector.executeSQL("Intouch", "EXEC [dbo].[uSP_Change_PMHistory] @pid = " + pid + ",@tid ='" + i
                            + "',@pm =" + pmFlag + ",@memo=" + (string.IsNullOrEmpty(memo) ? "NULL" : "'" + memo + "'")
                            + ",@op='" + usrID + "'" + flag + ",@pumping_bellow='" + bellow + "',@pumping_turning='"
                            + turning + "',@pumping_threeway='" + threeway + "',@pumping_handvalve='"
                            + handvalve + "',@pumping_bshead='" + bshead + "',@pumping_gasvalve='"
                            + gasvalve + "',@pumping_lpipe='" + lpipe + "',@pumping_entry='" + entry + "',@pm_inlet='"
                            + inlet + "',@pm_mixingbox='" + mixingbox + "',@pm_reactor='" + reactor + "',@pm_tank='"
                            + tank + "',@pm_outlet='" + outlet + "',@pm_tpipe='" + tpipe + "',@pm_lpipe='"
                            + pmlpipe + "',@pm_upturning='" + upturning + "',@pm_ms='" + ms + "',@pm_inlet_TP='"
                            + inlet_TP + "',@pm_mixingbox_TP='" + mixingbox_TP + "',@pm_reactor_TP='"
                            + reactor_TP + "',@pm_tank_TP='" + tank_TP + "',@pm_outlet_TP='"
                            + outlet_TP + "',@pm_tpipe_TP='" + tpipe_TP + "',@pm_lpipe_TP='"
                            + pmlpipe_TP + "',@pm_upturning_TP='" + upturning_TP + "',@pm_ms_TP='" + ms_TP + "'");
                        //DBConnector.executeSQL("Intouch", "EXEC	[uSP_Change_PMHistory]		@pid = 104,		@tid = N'MIHIT4',		@pm = 1,		@memo = N'1123',		@op = N'web',		@pm_flag = null,		@ts_flag = null,		@backup_flag = null,		@cmsError_flag = null,		@other_flag = null ");

                        if (pmFlag == "1")
                            DBMsg += "Tool ID : " + i + " PM設定完成\n";
                        else
                            DBMsg += "Tool ID : " + i + " PM解離完成\n";
                    }
                }
                catch (Exception ex)
                {
                    if (pmFlag == "1")
                        DBMsg += "Tool ID : " + i + " PM設定失敗，原因 : " + ex.Message + "\n";
                    else
                        DBMsg += "Tool ID : " + i + " PM隔離失敗，原因 : " + ex.Message + "\n";
                }
            }
            return DBMsg;
        }

        //PM中List
        public static IEnumerable<PMModel> PMingList()
        {
            DataSet ds = DBConnector.executeQuery("Intouch", "EXEC [dbo].[uSP_Select_PMTrigger] 0,0");

            return from dept in ds.Tables[0].AsEnumerable()
                   select new PMModel
                   {
                       _DateTime = dept.IsNull("statusdate") ? string.Empty : dept.Field<DateTime>("statusdate").ToString(),
                       ToolID = dept.Field<string>("toolid"),
                       LocationID = dept.Field<string>("lid"),
                       Location = dept.Field<string>("loc"),
                       VendorID = dept.IsNull("vid") ? (short)1 : dept.Field<Int16>("vid"),
                       Vendor = dept.Field<string>("vName"),
                       Status = dept.Field<string>("status"),
                       GroupName = dept.Field<string>("gname"),

                   };
        }

        //待PM清單
        public static IEnumerable<PMModel> NotyetPMList()
        {
            DataSet ds = DBConnector.executeQuery("Intouch", "EXEC [dbo].[uSP_Select_PMTrigger]");

            return from dept in ds.Tables[0].AsEnumerable()
                   select new PMModel
                   {
                       ToolID = dept.Field<string>("tool_id"),
                       Status = dept.Field<bool>("pmFlag") ? "待隔離" : "待解隔",
                       Vendor = dept.Field<string>("vendor_Intouch"),
                   };
        }

        //PM History List
        public static IEnumerable<PMModel> PMHistoryList(string vName, string tooolId, string _StartDate, string _EndDate, string selectType, string gname)
        {
            if (string.IsNullOrEmpty(_StartDate))
            {
                _StartDate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm");
            }

            if (string.IsNullOrEmpty(_EndDate))
            {
                _EndDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            }
            else if (!string.IsNullOrEmpty(_EndDate))
            {
                DateTime tempDate = Convert.ToDateTime(_EndDate);
                if (tempDate.Hour == 0)
                {
                    tempDate = tempDate.AddDays(1);
                }
                _EndDate = tempDate.ToString("yyyy-MM-dd HH:mm");
            }

            string sqlStr = "EXEC [uSP_Select_PMHistory] @fromDate = N'" + _StartDate + "',@toDate = N'" + _EndDate + "'";
            if (!string.IsNullOrEmpty(vName))
            {
                //DataSet vds = DBConnector.executeQuery("Intouch", "SELECT department_id FROM department_info where department_name = '" + vName + "'");

                sqlStr += ",@vid = " + ListModel.GetDepartmentId(vName);
            }
            else
                sqlStr += ",@vid = NULL";

            if (!string.IsNullOrEmpty(tooolId))
                sqlStr += ",@toolID = N'" + tooolId + "'";
            else
                sqlStr += ",@toolID = NULL";

            string flag = string.Empty;
            if (!string.IsNullOrEmpty(selectType))
            {
                string[] arrType = null;

                arrType = selectType.Split(',');
                foreach (string i in arrType)
                {
                    flag += "@" + i + "=1,";
                }
                flag = "," + flag.Substring(0, flag.Length - 1);
            }
            //if (!string.IsNullOrEmpty(gname))
            //    sqlStr += ",@gname = N'" + gname + "'";
            //else
            //    sqlStr += ",@gname = NULL";
            sqlStr += flag;
            DataSet ds = DBConnector.executeQuery("Intouch", sqlStr);

            if (ds.Tables.Count > 0)
            {

                return from dept in ds.Tables[0].AsEnumerable()
                       select new PMModel
                       {
                           pmCycle = dept.IsNull("PM_Cycle") ? string.Empty : dept.Field<Int32>("PM_Cycle").ToString(),
                           //pmNextTime = dept.IsNull("PM_Next_Time") ? string.Empty : dept.Field<DateTime>("PM_Next_Time").ToString("yyyy-MM-dd HH:mm:ss"),
                           //PMLifeTime = dept.IsNull("PM_LifeTime") ? string.Empty : dept.Field<Int16>("PM_LifeTime").ToString(),
                           pmHistoryID = dept.IsNull("pmHistoryID") ? string.Empty : dept.Field<Int32>("pmHistoryID").ToString(),
                           plcID = dept.Field<Int16>("plc_id").ToString(),
                           ToolID = dept.Field<string>("tool_id"),
                           pmSet = dept.IsNull("pm_set") ? string.Empty : dept.Field<DateTime>("pm_set").ToString("yyyy-MM-dd HH:mm:ss"),
                           pmUnset = dept.IsNull("pm_unset") ? string.Empty : dept.Field<DateTime>("pm_unset").ToString("yyyy-MM-dd HH:mm:ss"),
                           memo = dept.IsNull("memo") ? string.Empty : dept.Field<string>("memo"),
                           _operator = dept.Field<string>("operator"),
                           pmFlag = dept.IsNull("pm_flag") ? "Ｘ" : dept.Field<bool>("pm_flag") ? "Ｏ" : "Ｘ",
                           tsFlag = dept.IsNull("ts_flag") ? "Ｘ" : dept.Field<bool>("ts_flag") ? "Ｏ" : "Ｘ",
                           backupFlag = dept.IsNull("backup_flag") ? "Ｘ" : dept.Field<bool>("backup_flag") ? "Ｏ" : "Ｘ",
                           cmsErrorFlag = dept.IsNull("cmsError_flag") ? "Ｘ" : dept.Field<bool>("cmsError_flag") ? "Ｏ" : "Ｘ",
                           PumpingDone = dept.IsNull("pumping_done") ? "Ｘ" : dept.Field<bool>("pumping_done") ? "Ｏ" : "Ｘ",
                           SEXDone = dept.IsNull("sex_done") ? "Ｘ" : dept.Field<bool>("sex_done") ? "Ｏ" : "Ｘ",
                           otherFlag = dept.IsNull("other_flag") ? "Ｘ" : dept.Field<bool>("other_flag") ? "Ｏ" : "Ｘ",
                           TemporaryPFlag = dept.IsNull("TemporaryP_flag") ? "Ｘ" : dept.Field<bool>("TemporaryP_flag") ? "Ｏ" : "Ｘ",
                           ptDone = dept.IsNull("pt_done") ? "Ｘ" : dept.Field<bool>("pt_done") ? "Ｏ" : "Ｘ",
                           UpdateDate = dept.IsNull("update_date") ? string.Empty : dept.Field<DateTime>("update_date").ToString("yyyy-MM-dd HH:mm:ss"),
                           //gName = dept.IsNull("gname") ? string.Empty : dept.Field<string>("gname"),
                           //LocationID = dept.Field<string>("lid"),
                           Location = dept.Field<string>("loc"),
                           //locSide = dept.IsNull("locSide") ? string.Empty : dept.Field<string>("locSide"),
                           tName = dept.Field<string>("tname"),
                           department_id = dept.IsNull("department_id") ? (short)1 : dept.Field<Int16>("vid"),
                           department_name = dept.Field<string>("department_name"),
                           //vendorIntouch = dept.Field<string>("vendor_Intouch"),

                           //Bellow = dept.IsNull("pumping_bellow") ? string.Empty : dept.Field<string>("pumping_bellow"),
                           //Turning = dept.IsNull("pumping_turning") ? string.Empty : dept.Field<string>("pumping_turning"),
                           //ThreeWay = dept.IsNull("pumping_threeway") ? string.Empty : dept.Field<string>("pumping_threeway"),
                           //HandValve = dept.IsNull("pumping_handvalve") ? string.Empty : dept.Field<string>("pumping_handvalve"),
                           //bsHead = dept.IsNull("pumping_bshead") ? string.Empty : dept.Field<string>("pumping_bshead"),
                           //GasValve = dept.IsNull("pumping_gasvalve") ? string.Empty : dept.Field<string>("pumping_gasvalve"),
                           //LPipe = dept.IsNull("pumping_lpipe") ? string.Empty : dept.Field<string>("pumping_lpipe"),
                           //Entry = dept.IsNull("pumping_entry") ? string.Empty : dept.Field<string>("pumping_entry"),

                           //Inlet = dept.IsNull("pm_inlet") ? string.Empty : dept.Field<string>("pm_inlet"),
                           //MixingBox = dept.IsNull("pm_mixingbox") ? string.Empty : dept.Field<string>("pm_mixingbox"),
                           //Reactor = dept.IsNull("pm_reactor") ? string.Empty : dept.Field<string>("pm_reactor"),
                           //Tank = dept.IsNull("pm_tank") ? string.Empty : dept.Field<string>("pm_tank"),
                           //Outlet = dept.IsNull("pm_outlet") ? string.Empty : dept.Field<string>("pm_outlet"),
                           //Tpipe = dept.IsNull("pm_tpipe") ? string.Empty : dept.Field<string>("pm_tpipe"),
                           //Pmlpipe = dept.IsNull("pm_lpipe") ? string.Empty : dept.Field<string>("pm_lpipe"),
                           //Upturning = dept.IsNull("pm_upturning") ? string.Empty : dept.Field<string>("pm_upturning"),
                           //ms = dept.IsNull("pm_ms") ? string.Empty : dept.Field<string>("pm_ms"),

                           //Inlet_TP = dept.IsNull("pm_inlet_TP") ? string.Empty : dept.Field<string>("pm_inlet_TP"),
                           //MixingBox_TP = dept.IsNull("pm_mixingbox_TP") ? string.Empty : dept.Field<string>("pm_mixingbox_TP"),
                           //Reactor_TP = dept.IsNull("pm_reactor_TP") ? string.Empty : dept.Field<string>("pm_reactor_TP"),
                           //Tank_TP = dept.IsNull("pm_tank_TP") ? string.Empty : dept.Field<string>("pm_tank_TP"),
                           //Outlet_TP = dept.IsNull("pm_outlet_TP") ? string.Empty : dept.Field<string>("pm_outlet_TP"),
                           //Tpipe_TP = dept.IsNull("pm_tpipe_TP") ? string.Empty : dept.Field<string>("pm_tpipe_TP"),
                           //Pmlpipe_TP = dept.IsNull("pm_lpipe_TP") ? string.Empty : dept.Field<string>("pm_lpipe_TP"),
                           //Upturning_TP = dept.IsNull("pm_upturning_TP") ? string.Empty : dept.Field<string>("pm_upturning_TP"),
                           //ms_TP = dept.IsNull("pm_ms_TP") ? string.Empty : dept.Field<string>("pm_ms_TP")
                       };
            }
            else
            {
                return new List<PMModel>();
            }
        }

        public static PMModel GetPMHistroy(int pmHistoryID)
        {
            DataSet ds = DBConnector.executeQuery("Intouch", " SELECT P.*,I.location ,I.location_id  FROM pm_setting_history P JOIN Plc_Info I ON P.Plc_id = I.Plc_id WHERE pmHistoryID = '" + pmHistoryID + "'");

            DataTable dt = ds.Tables[0];
            // 將查詢到的資料回傳
            if (dt.Rows.Count == 0)
                return null;

            return new PMModel
            {
                //pmHistoryID = dept.IsNull("pmHistoryID") ? string.Empty : dept.Field<Int32>("pmHistoryID").ToString(),
                ToolID = dt.Rows[0].Field<string>("tool_id"),
                LocationID = dt.Rows[0].Field<string>("location_id"),
                Location = dt.Rows[0].Field<string>("location"),
                pmSet = dt.Rows[0].IsNull("pm_set") ? string.Empty : dt.Rows[0].Field<DateTime>("pm_set").ToString("yyyy-MM-dd HH:mm:ss"),
                pmUnset = dt.Rows[0].IsNull("pm_unset") ? string.Empty : dt.Rows[0].Field<DateTime>("pm_unset").ToString("yyyy-MM-dd HH:mm:ss"),
                memo = dt.Rows[0].Field<string>("memo"),
                pmFlag = dt.Rows[0].IsNull("pm_flag") ? "N" : dt.Rows[0].Field<bool>("pm_flag") ? "Y" : "N",
                tsFlag = dt.Rows[0].IsNull("ts_flag") ? "N" : dt.Rows[0].Field<bool>("ts_flag") ? "Y" : "N",
                //backupFlag = dt.Rows[0].IsNull("backup_flag") ? string.Empty : dt.Rows[0].Field<bool>("backup_flag") ? "Y" : "N",
                //cmsErrorFlag = dt.Rows[0].IsNull("cmsError_flag") ? string.Empty : dt.Rows[0].Field<bool>("cmsError_flag") ? "Y" : "N",
                PumpingDone = dt.Rows[0].IsNull("pumping_done") ? "N" : dt.Rows[0].Field<bool>("pumping_done") ? "Y" : "N",
                SEXDone = dt.Rows[0].IsNull("sex_done") ? "N" : dt.Rows[0].Field<bool>("sex_done") ? "Y" : "N",
                otherFlag = dt.Rows[0].IsNull("other_flag") ? "N" : dt.Rows[0].Field<bool>("other_flag") ? "Y" : "N",
                TemporaryPFlag = dt.Rows[0].IsNull("TemporaryP_flag") ? "N" : dt.Rows[0].Field<bool>("TemporaryP_flag") ? "Y" : "N",
                ptDone = dt.Rows[0].IsNull("pt_done") ? "N" : dt.Rows[0].Field<bool>("pt_done") ? "Y" : "N",
                //locSide = dept.IsNull("locSide") ? string.Empty : dept.Field<string>("locSide"),
                //tName = dept.Field<string>("tname"),
                //VendorID = dept.IsNull("vid") ? (short)1 : dept.Field<Int16>("vid"),
                //Vendor = dt.Rows[0].Field<string>("vName"),
                //vendorIntouch = dept.Field<string>("vendor_Intouch"),
                //Bellow = dt.Rows[0].IsNull("pumping_bellow") ? string.Empty : dt.Rows[0].Field<string>("pumping_bellow"),
                //Turning = dt.Rows[0].IsNull("pumping_turning") ? string.Empty : dt.Rows[0].Field<string>("pumping_turning"),
                //ThreeWay = dt.Rows[0].IsNull("pumping_threeway") ? string.Empty : dt.Rows[0].Field<string>("pumping_threeway"),
                //HandValve = dt.Rows[0].IsNull("pumping_handvalve") ? string.Empty : dt.Rows[0].Field<string>("pumping_handvalve"),
                //bsHead = dt.Rows[0].IsNull("pumping_bshead") ? string.Empty : dt.Rows[0].Field<string>("pumping_bshead"),
                //GasValve = dt.Rows[0].IsNull("pumping_gasvalve") ? string.Empty : dt.Rows[0].Field<string>("pumping_gasvalve"),
                //LPipe = dt.Rows[0].IsNull("pumping_lpipe") ? string.Empty : dt.Rows[0].Field<string>("pumping_lpipe"),
                //Entry = dt.Rows[0].IsNull("pumping_entry") ? string.Empty : dt.Rows[0].Field<string>("pumping_entry"),

                //Inlet = dt.Rows[0].IsNull("pm_inlet") ? string.Empty : dt.Rows[0].Field<string>("pm_inlet"),
                //MixingBox = dt.Rows[0].IsNull("pm_mixingbox") ? string.Empty : dt.Rows[0].Field<string>("pm_mixingbox"),
                //Reactor = dt.Rows[0].IsNull("pm_reactor") ? string.Empty : dt.Rows[0].Field<string>("pm_reactor"),
                //Tank = dt.Rows[0].IsNull("pm_tank") ? string.Empty : dt.Rows[0].Field<string>("pm_tank"),
                //Outlet = dt.Rows[0].IsNull("pm_outlet") ? string.Empty : dt.Rows[0].Field<string>("pm_outlet"),
                //Tpipe = dt.Rows[0].IsNull("pm_tpipe") ? string.Empty : dt.Rows[0].Field<string>("pm_tpipe"),
                //Pmlpipe = dt.Rows[0].IsNull("pm_lpipe") ? string.Empty : dt.Rows[0].Field<string>("pm_lpipe"),
                //Upturning = dt.Rows[0].IsNull("pm_upturning") ? string.Empty : dt.Rows[0].Field<string>("pm_upturning"),
                //ms = dt.Rows[0].IsNull("pm_ms") ? string.Empty : dt.Rows[0].Field<string>("pm_ms"),

                //Inlet_TP = dt.Rows[0].IsNull("pm_inlet_TP") ? string.Empty : dt.Rows[0].Field<string>("pm_inlet_TP"),
                //MixingBox_TP = dt.Rows[0].IsNull("pm_mixingbox_TP") ? string.Empty : dt.Rows[0].Field<string>("pm_mixingbox_TP"),
                //Reactor_TP = dt.Rows[0].IsNull("pm_reactor_TP") ? string.Empty : dt.Rows[0].Field<string>("pm_reactor_TP"),
                //Tank_TP = dt.Rows[0].IsNull("pm_tank_TP") ? string.Empty : dt.Rows[0].Field<string>("pm_tank_TP"),
                //Outlet_TP = dt.Rows[0].IsNull("pm_outlet_TP") ? string.Empty : dt.Rows[0].Field<string>("pm_outlet_TP"),
                //Tpipe_TP = dt.Rows[0].IsNull("pm_tpipe_TP") ? string.Empty : dt.Rows[0].Field<string>("pm_tpipe_TP"),
                //Pmlpipe_TP = dt.Rows[0].IsNull("pm_lpipe_TP") ? string.Empty : dt.Rows[0].Field<string>("pm_lpipe_TP"),
                //Upturning_TP = dt.Rows[0].IsNull("pm_upturning_TP") ? string.Empty : dt.Rows[0].Field<string>("pm_upturning_TP"),
                //ms_TP = dt.Rows[0].IsNull("pm_ms") ? string.Empty : dt.Rows[0].Field<string>("pm_ms_TP")
            };
        }

        //PM History Export
        public static DataTable dtReportList(string vName, string tooolId, string _StartDate, string _EndDate, string selectType)
        {
            DataTable dt = new DataTable();

            vName = vName.Replace("\\", string.Empty).Replace("\"", string.Empty);
            if (string.IsNullOrEmpty(_StartDate))
                _StartDate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm");

            if (string.IsNullOrEmpty(_EndDate))
                _EndDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            else if (!string.IsNullOrEmpty(_EndDate))
            {
                DateTime tempDate = Convert.ToDateTime(_EndDate);
                if (tempDate.Hour == 0)
                    tempDate = tempDate.AddDays(1);
                _EndDate = tempDate.ToString("yyyy-MM-dd HH:mm");
            }

            string sqlStr = "EXEC [uSP_Select_PMHistory] @fromDate = N'" + _StartDate + "',@toDate = N'" + _EndDate + "'";
            if (!string.IsNullOrEmpty(vName))
            {
                DataSet vds = DBConnector.executeQuery("Intouch", "SELECT department_id FROM department_info where department_name = '" + vName + "'");
                if (vds.Tables.Count > 0)
                {
                    sqlStr += ",@vid = " + vds.Tables[0].Rows[0][0].ToString();
                }
            }
            else
                sqlStr += ",@vid = NULL";

            if (!string.IsNullOrEmpty(tooolId))
                sqlStr += ",@toolID = N'" + tooolId + "'";
            else
                sqlStr += ",@toolID = NULL";

            string flag = string.Empty;
            if (!string.IsNullOrEmpty(selectType))
            {
                string[] arrType = null;

                arrType = selectType.Split(',');
                foreach (string i in arrType)
                {
                    flag += "@" + i + "=1,";
                }
                flag = "," + flag.Substring(0, flag.Length - 1);
            }
            sqlStr += flag;
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = WebConfigurationManager.ConnectionStrings["Intouch"].ConnectionString;
                conn.Open();

                dt.Load(new System.Data.SqlClient.SqlCommand(sqlStr, conn).ExecuteReader());
            }

            return dt;
        }

        public static byte[] GetPMHistoryFile(string Vendor, string toolId, string StartDate, string EndDate, string selectType)
        {
            DataTable dt = PMModel.dtReportList(Vendor, toolId, StartDate == null ? DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm") : StartDate, EndDate == null ? DateTime.Now.ToString("yyyy-MM-dd HH:mm") : EndDate, selectType);
            ExcelPackage ep = new ExcelPackage();
            ep.Workbook.Worksheets.Add("MySheet");
            ExcelWorksheet sheet1 = ep.Workbook.Worksheets["MySheet"];

            sheet1.Cells[1, 1].Value = "更新時間";
            //sheet1.Cells[1, 2].Value = "課別名稱";
            sheet1.Cells[1, 2].Value = "Chamber";
            //sheet1.Cells[1, 4].Value = "Location ID";
            sheet1.Cells[1, 3].Value = "柱位";
            //sheet1.Cells[1, 6].Value = "Vendor ID";
            sheet1.Cells[1, 4].Value = "部門";
            //sheet1.Cells[1, 8].Value = "課別";
            sheet1.Cells[1, 5].Value = "設定pm時間";
            sheet1.Cells[1, 6].Value = "解除pm時間";
            //sheet1.Cells[1, 10].Value = "pm週期";
            //sheet1.Cells[1, 11].Value = "下次隔離時間";
            //sheet1.Cells[1, 12].Value = "PM_LifeTime";
            sheet1.Cells[1, 7].Value = "隔離事由";
            sheet1.Cells[1, 8].Value = "pm";
            sheet1.Cells[1, 9].Value = "Trouble Shooting";
            //sheet1.Cells[1, 15].Value = "備機";
            //sheet1.Cells[1, 16].Value = "CMS 錯誤";
            sheet1.Cells[1, 10].Value = "Pumping PM";
            sheet1.Cells[1, 11].Value = "SEX PM";
            sheet1.Cells[1, 12].Value = "其他";
            //sheet1.Cells[1, 17].Value = "臨時拉P";
            //sheet1.Cells[1, 20].Value = "是否PT校正";
            sheet1.Cells[1, 13].Value = "操作者";
            //sheet1.Cells[1, 19].Value = "Bellow";
            //sheet1.Cells[1, 20].Value = "轉彎處";
            //sheet1.Cells[1, 21].Value = "三通";
            //sheet1.Cells[1, 22].Value = "手閥";
            //sheet1.Cells[1, 26].Value = "大小頭";
            //sheet1.Cells[1, 23].Value = "氣動閥";
            //sheet1.Cells[1, 24].Value = "L管";
            //sheet1.Cells[1, 25].Value = "Entry";

            if (dt.Rows.Count > 0)
            {
                //sheet1.Cells["A2"].LoadFromDataTable(dt, false);

                for (int i = 1; i <= dt.Rows.Count; i++)
                {
                    sheet1.Cells[i + 1, 1].Value = dt.Rows[i - 1]["update_date"];
                    //sheet1.Cells[i + 1, 2].Value = dt.Rows[i - 1]["gname"];
                    sheet1.Cells[i + 1, 2].Value = dt.Rows[i - 1]["tool_id"];
                    //sheet1.Cells[i + 1, 4].Value = dt.Rows[i - 1]["lid"];
                    sheet1.Cells[i + 1, 3].Value = dt.Rows[i - 1]["loc"];
                    //sheet1.Cells[i + 1, 6].Value = dt.Rows[i - 1]["vid"];
                    sheet1.Cells[i + 1, 4].Value = dt.Rows[i - 1]["department_name"];
                    //sheet1.Cells[i + 1, 8].Value = dt.Rows[i - 1]["gName"];
                    sheet1.Cells[i + 1, 5].Value = dt.Rows[i - 1]["pm_set"];
                    sheet1.Cells[i + 1, 6].Value = dt.Rows[i - 1]["pm_unset"];
                    //sheet1.Cells[i + 1, 10].Value = dt.Rows[i - 1]["PM_Cycle"];
                    //sheet1.Cells[i + 1, 11].Value = dt.Rows[i - 1]["PM_Next_Time"];
                    //sheet1.Cells[i + 1, 12].Value = dt.Rows[i - 1]["PM_LifeTime"];
                    sheet1.Cells[i + 1, 7].Value = dt.Rows[i - 1]["memo"];
                    sheet1.Cells[i + 1, 8].Value = dt.Rows[i - 1].Field<bool?>("pm_flag").HasValue && dt.Rows[i - 1].Field<bool?>("pm_flag").Value ? "Ｏ" : string.Empty;
                    sheet1.Cells[i + 1, 9].Value = dt.Rows[i - 1].Field<bool?>("ts_flag").HasValue && dt.Rows[i - 1].Field<bool?>("ts_flag").Value ? "Ｏ" : string.Empty;
                    //sheet1.Cells[i + 1, 15].Value = dt.Rows[i - 1].Field<bool?>("backup_flag").HasValue && dt.Rows[i - 1].Field<bool?>("backup_flag").Value ? "Ｏ" : string.Empty;
                    //sheet1.Cells[i + 1, 16].Value = dt.Rows[i - 1].Field<bool?>("cmsError_flag").HasValue && dt.Rows[i - 1].Field<bool?>("cmsError_flag").Value ? "Ｏ" : string.Empty;
                    sheet1.Cells[i + 1, 10].Value = dt.Rows[i - 1].Field<bool?>("pumping_done").HasValue && dt.Rows[i - 1].Field<bool?>("pumping_done").Value ? "Ｏ" : string.Empty;
                    sheet1.Cells[i + 1, 11].Value = dt.Rows[i - 1].Field<bool?>("sex_done").HasValue && dt.Rows[i - 1].Field<bool?>("sex_done").Value ? "Ｏ" : string.Empty;
                    sheet1.Cells[i + 1, 12].Value = dt.Rows[i - 1].Field<bool?>("other_flag").HasValue && dt.Rows[i - 1].Field<bool?>("other_flag").Value ? "Ｏ" : string.Empty;
                    //sheet1.Cells[i + 1, 17].Value = dt.Rows[i - 1].Field<bool?>("TemporaryP_flag").HasValue && dt.Rows[i - 1].Field<bool?>("TemporaryP_flag").Value ? "Ｏ" : string.Empty;
                    //sheet1.Cells[i + 1, 20].Value = dt.Rows[i - 1].Field<bool?>("pt_done").HasValue && dt.Rows[i - 1].Field<bool?>("pt_done").Value ? "Ｏ" : string.Empty;

                    sheet1.Cells[i + 1, 13].Value = dt.Rows[i - 1]["operator"];
                    //sheet1.Cells[i + 1, 19].Value = dt.Rows[i - 1]["pumping_bellow"];
                    //sheet1.Cells[i + 1, 20].Value = dt.Rows[i - 1]["pumping_turning"];
                    //sheet1.Cells[i + 1, 21].Value = dt.Rows[i - 1]["pumping_threeway"];
                    //sheet1.Cells[i + 1, 22].Value = dt.Rows[i - 1]["pumping_handvalve"];
                    //sheet1.Cells[i + 1, 26].Value = dt.Rows[i - 1]["pumping_bshead"];
                    //sheet1.Cells[i + 1, 23].Value = dt.Rows[i - 1]["pumping_gasvalve"];
                    //sheet1.Cells[i + 1, 24].Value = dt.Rows[i - 1]["pumping_lpipe"];
                    //sheet1.Cells[i + 1, 25].Value = dt.Rows[i - 1]["pumping_entry"];

                    sheet1.Cells[i + 1, 1].Style.Numberformat.Format = "yyyy-mm-dd HH:mm:ss";
                    sheet1.Cells[i + 1, 5].Style.Numberformat.Format = "yyyy-mm-dd HH:mm:ss";
                    sheet1.Cells[i + 1, 6].Style.Numberformat.Format = "yyyy-mm-dd HH:mm:ss";
                    //sheet1.Cells[i + 1, 11].Style.Numberformat.Format = "yyyy-mm-dd HH:mm:ss";
                }

                sheet1.Cells.AutoFitColumns();
                //sheet1.Column(2).Width = 0;
                //sheet1.Column(6).Width = 0;
                //if (!selectType.Contains("pumping_done"))
                //{
                //    sheet1.Column(19).Width = 0;
                //    sheet1.Column(20).Width = 0;
                //    sheet1.Column(21).Width = 0;
                //    sheet1.Column(22).Width = 0;
                //    sheet1.Column(23).Width = 0;
                //    sheet1.Column(24).Width = 0;
                //    sheet1.Column(25).Width = 0;
                //}
            }
            return ep.GetAsByteArray();
        }

        public string Update(string pmHistoryID,string uName)
        {
            try
            {
                pmFlag = (pmFlag == "Y" ? "true" : "false");
                tsFlag = (tsFlag == "Y" ? "true" : "false");
                PumpingDone = (PumpingDone == "Y" ? "true" : "false");
                SEXDone = (SEXDone == "Y" ? "true" : "false");
                otherFlag = (otherFlag == "Y" ? "true" : "false");
                TemporaryPFlag = (TemporaryPFlag == "Y" ? "true" : "false");
                //// 更新資料
                DBConnector.executeSQL("Intouch", "EXEC [dbo].[uSP_Edit_pm_setting_history] '" + pmHistoryID + "','" + ToolID + "','" + pmSet + "','" + pmUnset + "','" + memo + "'," + pmFlag + "," + tsFlag + "," + PumpingDone + "," + SEXDone + "," + otherFlag + "," + TemporaryPFlag + ",'" + Bellow + "','" + Turning + "','" + ThreeWay + "','" + HandValve + "','" + bsHead + "','" + GasValve + "','" + LPipe + "','" + Entry + "','" + Inlet + "','" + MixingBox + "','" + Reactor + "','" + Tank + "','" + Outlet + "','" + Tpipe + "','" + Pmlpipe + "','" + Upturning + "','" + ms + "','" + Inlet_TP + "','" + MixingBox_TP + "','" + Reactor_TP + "','" + Tank_TP + "','" + Outlet_TP + "','" + Tpipe_TP + "','" + Pmlpipe_TP + "','" + Upturning_TP + "','" + ms_TP + "','" + uName + "'");
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return null;
        }
    }
}