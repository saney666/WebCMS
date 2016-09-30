using OfficeOpenXml;
using System.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using TSMC14B.Models;
using OfficeOpenXml.Style;

namespace TSMC14B.Areas.Main.Models
{
    public class PMfrequencySetModel
    {

        #region 組織成員屬性

        [Display(Name = "ID")]
        public int id { get; set; }

        [Display(Name = "PLC ID")]
        public int plc_id { get; set; }

        [Display(Name = "廠商")]
        public string Vendor { get; set; }

        [Display(Name = "Tool ID")]
        public string ToolID { get; set; }

        [Display(Name = "Location ID")]
        public string LocationID { get; set; }

        [Display(Name = "解PM時間")]
        public string PMDate { get; set; }

        [Display(Name = "Freq ID")]
        public Byte freq_id { get; set; }

        [Display(Name = "週期")]
        public int frequency { get; set; }

        [Display(Name = "容許值")]
        public int tolerance { get; set; }

        [Display(Name = "下次PM時間")]
        public string PlanPMDate { get; set; }

        [Display(Name = "容許起始日期")]
        public string ToleranceFrom { get; set; }

        [Display(Name = "容許到期日期")]
        public string ToleranceTo { get; set; }

        [Display(Name = "PM歷史記錄")]
        public string PMHistory { get; set; }

        [Display(Name = "PM歷史記錄(ALL)")]
        public string PMHistoryALL { get; set; }

        [Display(Name = "設定者")]
        public string userlogin { get; set; }

        [Display(Name = "時間")]
        public string date { get; set; }

        [Display(Name = "ON/OFF Status")]
        public string onoffStatus { get; set; }

        [Display(Name = "PowerOnDate")]
        public string PowerOnDate { get; set; }

        [Display(Name = "PM隔離上限小時")]
        public Decimal UnSetTooLate { get; set; }

        [Display(Name = "設定")]
        public int rbn { get; set; }

        #endregion

        //設定PM週期
        public string EditPM(string selectvendor, string selectplcid, string selecttoolid, string selectfrequency, int selectfreq_id, string UsrID)
        {
            string DBMsg = null;

            try
            {
                using (tsmc14BDataContext db = new tsmc14BDataContext())
                {
                    //DBMsg += "0";
                    var count = (from row in db.PM_Schedule where row.tool_id == selecttoolid select row.id).Count();
                    //DBMsg += "1";
                    var selecttolerance = (from row in db.PM_frequency_info where row.freq_id == selectfreq_id select row.PM_tolerance).FirstOrDefault();
                    if (count == 0)
                    {
                        DBConnector.executeSQL("Intouch", "INSERT INTO PM_Schedule (plc_id, tool_id, freq_id, frequency, tolerance, userlogin) VALUES (" + selectplcid + ", '" + selecttoolid + "', " + selectfreq_id + ", " + selectfrequency + ", " + selecttolerance + ", '" + UsrID + "')");
                        DBMsg = selecttoolid + "PM週期設定完成\n";
                    }
                    else if (count > 0)
                    {
                        DBConnector.executeSQL("Intouch", "UPDATE PM_Schedule SET freq_id = " + selectfreq_id + ", frequency = " + selectfrequency + ", tolerance = " + selecttolerance + ", userlogin = '" + UsrID + "' WHERE plc_id = " + selectplcid + " AND tool_id = '" + selecttoolid + "'");
                        DBMsg = selecttoolid + "PM週期已更新完成\n";
                    }
                }
            }
            catch (Exception ex)
            {
                DBMsg += "PM週期設定失敗 原因 : " + ex.Message + "\n";
            }

            return DBMsg;
        }

        //PMSchedule的list
        public static IEnumerable<PMfrequencySetModel> PMScheduleList()
        {
            DataSet ds = DBConnector.executeQuery("Intouch", "Select * from PM_Schedule");

            return from dept in ds.Tables[0].AsEnumerable()
                   select new PMfrequencySetModel
                   {
                       ToolID = dept.Field<string>("tool_id"),
                       frequency = dept.Field<int>("frequency"),
                       tolerance = dept.Field<int>("tolerance"),
                       userlogin = dept.Field<string>("userlogin"),
                       date = dept.IsNull("date") ? string.Empty : dept.Field<DateTime>("date").ToString(),

                   };
        }

        //PMFrequency的list
        public static IEnumerable<PMfrequencySetModel> PMFrequencyList()
        {
            DataSet ds = DBConnector.executeQuery("Intouch", "SELECT VES.[vName],VES.[tName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM vw_pm_schedule_control VPS right join  PM_Schedule PS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id");

            return from dept in ds.Tables[0].AsEnumerable()
                   select new PMfrequencySetModel
                   {
                       Vendor = dept.Field<string>("vName"),
                       ToolID = dept.Field<string>("tool_id"),
                       LocationID = dept.Field<string>("lid"),
                       PMDate = dept.IsNull("PMDate") ? string.Empty : dept.Field<DateTime>("PMDate").ToString("yyyy/MM/dd"),
                       frequency = dept.Field<int>("frequency"),
                       tolerance = dept.Field<int>("tolerance"),
                       //PlanPMDate = dept.Field<DateTime>("PMDate").AddDays(dept.Field<int>("frequency")).ToString("yyyy/MM/dd"),
                       //ToleranceFrom = dept.Field<DateTime>("PMDate").AddDays(dept.Field<int>("frequency") - dept.Field<int>("tolerance")).ToString("yyyy/MM/dd"),
                       //ToleranceTo = dept.Field<DateTime>("PMDate").AddDays(dept.Field<int>("frequency") + dept.Field<int>("tolerance")).ToString("yyyy/MM/dd"),
                       PlanPMDate = dept.IsNull("PlanPMDate") ? string.Empty : dept.Field<DateTime>("PlanPMDate").ToString("yyyy/MM/dd"),
                       ToleranceFrom = dept.IsNull("ToleranceFrom") ? string.Empty : dept.Field<DateTime>("ToleranceFrom").ToString("yyyy/MM/dd"),
                       ToleranceTo = dept.IsNull("ToleranceTo") ? string.Empty : dept.Field<DateTime>("ToleranceTo").ToString("yyyy/MM/dd"),
                       PMHistory = dept.IsNull("PMHistory") ? string.Empty : dept.Field<string>("PMHistory").Split(',').FirstOrDefault(),
                       PMHistoryALL = dept.Field<string>("PMHistory"),
                       userlogin = dept.Field<string>("userlogin"),
                       date = dept.IsNull("date") ? string.Empty : dept.Field<DateTime>("date").ToString("yyyy/MM/dd"),
                       onoffStatus = dept.IsNull("onoffStatus") ? "ON" : dept.Field<string>("onoffStatus") == "1" ? "ON" : "OFF",
                       PowerOnDate = dept.IsNull("PowerOnDate") ? "NULL" : dept.Field<DateTime>("PowerOnDate").ToString("yyyy/MM/dd"),
                       UnSetTooLate = dept.IsNull("UnSetTooLate") ? 0 : dept.Field<Decimal>("UnSetTooLate"),
                   };
        }

        //EditPMFrequency的list
        public static IEnumerable<PMfrequencySetModel> EditPMFrequencyList(string groupName, int rbn)
        {
            string sqlstr = string.Empty;
            if (rbn == 1)
            {
                if (groupName == "ALLGroup")
                {
                    sqlstr = "SELECT VES.[vName],VES.[tName],VES.[groupName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM PM_Schedule PS  right join vw_pm_schedule_control VPS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id where  frequency is not NULL and tolerance is not NULL";
                }
                else if (groupName != "ALLGroup")
                {
                    sqlstr = "SELECT VES.[vName],VES.[tName],VES.[groupName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM PM_Schedule PS  right join vw_pm_schedule_control VPS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id where  frequency is not NULL and tolerance is not NULL and groupName ='" + groupName + "'";
                }
            }
            else if (rbn == 0)
            {
                if (groupName == "ALLGroup")
                {
                    sqlstr = "SELECT VES.[vName],VES.[tName],VES.[groupName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM PM_Schedule PS  right join vw_pm_schedule_control VPS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id where  frequency is NULL and tolerance is NULL";
                }
                else if (groupName != "ALLGroup")
                {
                    sqlstr = "SELECT VES.[vName],VES.[tName],VES.[groupName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM PM_Schedule PS  right join vw_pm_schedule_control VPS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id where  frequency is NULL and tolerance is NULL and groupName ='" + groupName + "'";
                }
            }
            else if (rbn == 2)
            {
                if (groupName == "ALLGroup")
                {
                    sqlstr = "SELECT VES.[vName],VES.[tName],VES.[groupName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM PM_Schedule PS  right join vw_pm_schedule_control VPS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id ";
                }
                else if (groupName != "ALLGroup")
                {
                    sqlstr = "SELECT VES.[vName],VES.[tName],VES.[groupName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM PM_Schedule PS  right join vw_pm_schedule_control VPS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id and groupName ='" + groupName + "'";
                }

            }

            DataSet ds = DBConnector.executeQuery("Intouch", sqlstr);

            return from dept in ds.Tables[0].AsEnumerable()
                   select new PMfrequencySetModel
                   {
                       Vendor = dept.Field<string>("vName"),
                       ToolID = dept.Field<string>("tool_id"),
                       LocationID = dept.Field<string>("lid"),
                       PMDate = dept.IsNull("PMDate") ? string.Empty : dept.Field<DateTime>("PMDate").ToString("yyyy/MM/dd"),
                       frequency = dept.IsNull("frequency") ? 0 : dept.Field<int>("frequency"),
                       tolerance = dept.IsNull("tolerance") ? 0 : dept.Field<int>("tolerance"),
                       //PlanPMDate = dept.Field<DateTime>("PMDate").AddDays(dept.IsNull("frequency") ? 0 : dept.Field<int>("frequency")).ToString("yyyy/MM/dd"),
                       //ToleranceFrom = dept.Field<DateTime>("PMDate").AddDays((dept.IsNull("frequency") ? 0 : dept.Field<int>("frequency")) - (dept.IsNull("tolerance") ? 0 : dept.Field<int>("tolerance"))).ToString("yyyy/MM/dd"),
                       //ToleranceTo = dept.Field<DateTime>("PMDate").AddDays((dept.IsNull("frequency") ? 0 : dept.Field<int>("frequency")) + (dept.IsNull("tolerance") ? 0 : dept.Field<int>("tolerance"))).ToString("yyyy/MM/dd"),
                       PlanPMDate = dept.IsNull("PlanPMDate") ? string.Empty : dept.Field<DateTime>("PlanPMDate").ToString("yyyy/MM/dd"),
                       ToleranceFrom = dept.IsNull("ToleranceFrom") ? string.Empty : dept.Field<DateTime>("ToleranceFrom").ToString("yyyy/MM/dd"),
                       ToleranceTo = dept.IsNull("ToleranceTo") ? string.Empty : dept.Field<DateTime>("ToleranceTo").ToString("yyyy/MM/dd"),
                       PMHistory = dept.IsNull("PMHistory") ? string.Empty : dept.Field<string>("PMHistory").Split(',').FirstOrDefault(),
                       PMHistoryALL = dept.Field<string>("PMHistory"),
                       userlogin = dept.Field<string>("userlogin"),
                       date = dept.IsNull("date") ? string.Empty : dept.Field<DateTime>("date").ToString("yyyy/MM/dd"),
                       onoffStatus = dept.IsNull("onoffStatus") ? "ON" : dept.Field<string>("onoffStatus") == "1" ? "ON" : "OFF",
                       PowerOnDate = dept.IsNull("PowerOnDate") ? "NULL" : dept.Field<DateTime>("PowerOnDate").ToString("yyyy/MM/dd"),
                       UnSetTooLate = dept.IsNull("UnSetTooLate") ? 0 : dept.Field<Decimal>("UnSetTooLate"),

                   };
        }

        //各vName的機台週期秀出
        public static IEnumerable<PMfrequencySetModel> EditPMFrequencyvNameList(string vName, string groupName, int rbn)
        {
            string sqlstr = string.Empty;
            if (vName != null && rbn == 1)
            {
                if (groupName == "ALLGroup")
                {
                    sqlstr = "SELECT VES.[vName],VES.[tName],VES.[groupName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM PM_Schedule PS  right join vw_pm_schedule_control VPS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id where vName = '" + vName + "' and frequency is not NULL and tolerance is not NULL";
                }
                else if (groupName != "ALLGroup")
                {
                    sqlstr = "SELECT VES.[vName],VES.[tName],VES.[groupName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM PM_Schedule PS  right join vw_pm_schedule_control VPS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id where vName = '" + vName + "' and frequency is not NULL and tolerance is not NULL and groupName ='" + groupName + "'";
                }
            }
            else if (vName != null && rbn == 0)
            {
                if (groupName == "ALLGroup")
                {
                    sqlstr = "SELECT VES.[vName],VES.[tName],VES.[groupName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM PM_Schedule PS  right join vw_pm_schedule_control VPS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id where vName = '" + vName + "' and frequency is NULL and tolerance is NULL";
                }
                else if (groupName != "ALLGroup")
                {
                    sqlstr = "SELECT VES.[vName],VES.[tName],VES.[groupName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM PM_Schedule PS  right join vw_pm_schedule_control VPS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id where vName = '" + vName + "' and frequency is NULL and tolerance is NULL and groupName ='" + groupName + "'";
                }

            }
            else if (vName != null && rbn == 2)
            {
                if (groupName == "ALLGroup")
                {
                    sqlstr = "SELECT VES.[vName],VES.[tName],VES.[groupName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM PM_Schedule PS  right join vw_pm_schedule_control VPS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id where vName = '" + vName + "'";
                }
                else if (groupName != "ALLGroup")
                {
                    sqlstr = "SELECT VES.[vName],VES.[tName],VES.[groupName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM PM_Schedule PS  right join vw_pm_schedule_control VPS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id where vName = '" + vName + "' and groupName ='" + groupName + "'";
                }

            }

            DataSet ds = DBConnector.executeQuery("Intouch", sqlstr);

            return from dept in ds.Tables[0].AsEnumerable()
                   select new PMfrequencySetModel
                   {
                       Vendor = dept.Field<string>("vName"),
                       ToolID = dept.Field<string>("tool_id"),
                       LocationID = dept.Field<string>("lid"),
                       PMDate = dept.IsNull("PMDate") ? string.Empty : dept.Field<DateTime>("PMDate").ToString("yyyy/MM/dd"),
                       frequency = dept.IsNull("frequency") ? 0 : dept.Field<int>("frequency"),
                       tolerance = dept.IsNull("tolerance") ? 0 : dept.Field<int>("tolerance"),
                       //PlanPMDate = dept.Field<DateTime>("PMDate").AddDays(dept.IsNull("frequency") ? 0 : dept.Field<int>("frequency")).ToString("yyyy/MM/dd"),
                       //ToleranceFrom = dept.Field<DateTime>("PMDate").AddDays((dept.IsNull("frequency") ? 0 : dept.Field<int>("frequency")) - (dept.IsNull("tolerance") ? 0 : dept.Field<int>("tolerance"))).ToString("yyyy/MM/dd"),
                       //ToleranceTo = dept.Field<DateTime>("PMDate").AddDays((dept.IsNull("frequency") ? 0 : dept.Field<int>("frequency")) + (dept.IsNull("tolerance") ? 0 : dept.Field<int>("tolerance"))).ToString("yyyy/MM/dd"),
                       PlanPMDate = dept.IsNull("PlanPMDate") ? string.Empty : dept.Field<DateTime>("PlanPMDate").ToString("yyyy/MM/dd"),
                       ToleranceFrom = dept.IsNull("ToleranceFrom") ? string.Empty : dept.Field<DateTime>("ToleranceFrom").ToString("yyyy/MM/dd"),
                       ToleranceTo = dept.IsNull("ToleranceTo") ? string.Empty : dept.Field<DateTime>("ToleranceTo").ToString("yyyy/MM/dd"),
                       PMHistory = dept.IsNull("PMHistory") ? string.Empty : dept.Field<string>("PMHistory").Split(',').FirstOrDefault(),
                       PMHistoryALL = dept.Field<string>("PMHistory"),
                       userlogin = dept.Field<string>("userlogin"),
                       date = dept.IsNull("date") ? string.Empty : dept.Field<DateTime>("date").ToString("yyyy/MM/dd"),
                       onoffStatus = dept.IsNull("onoffStatus") ? "ON" : dept.Field<string>("onoffStatus") == "1" ? "ON" : "OFF",
                       PowerOnDate = dept.IsNull("PowerOnDate") ? "NULL" : dept.Field<DateTime>("PowerOnDate").ToString("yyyy/MM/dd"),
                       UnSetTooLate = dept.IsNull("UnSetTooLate") ? 0 : dept.Field<Decimal>("UnSetTooLate"),

                   };
        }

        //各Type的機台週期秀出
        public static IEnumerable<PMfrequencySetModel> PMFrequencyDetailList(String vName, String groupName)
        {
            String sqlstr = string.Empty;

            if (groupName == null)
            {
                if (vName == "7PMwarning")
                {
                    sqlstr = "SELECT VES.[vName],VES.[tName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM vw_pm_schedule_control VPS right join  PM_Schedule PS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id where dateadd(dd,-7,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]))<= GETDATE() and DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) >= GETDATE()";
                }
                else if (vName == "warningFrequency")
                {
                    sqlstr = "SELECT VES.[vName],VES.[tName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM vw_pm_schedule_control VPS right join  PM_Schedule PS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id where  DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) <= GETDATE() and DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) >= GETDATE()";
                }
                else if (vName == "AfterFrequency")
                {
                    sqlstr = "SELECT VES.[vName],VES.[tName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM vw_pm_schedule_control VPS right join  PM_Schedule PS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id where   DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) <= GETDATE()";
                }
                else
                {
                    sqlstr = "SELECT VES.[vName],VES.[tName],VES.[groupName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM PM_Schedule PS  right join vw_pm_schedule_control VPS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id where vName='" + vName + "' and frequency is not NULL";
                }

            }
            else if (groupName != null)
            {
                sqlstr = "SELECT VES.[vName],VES.[tName],VES.[groupName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM PM_Schedule PS  right join vw_pm_schedule_control VPS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id where vName='" + vName + "' and groupName='" + groupName + "' and frequency is not NULL";
            }

            //DataSet ds = DBConnector.executeQuery("Intouch", "SELECT VES.[vName],VES.[tName],VES.[groupName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM PM_Schedule PS  right join vw_pm_schedule_control VPS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id where tName='" + tName + "' and frequency is not NULL");
            DataSet ds = DBConnector.executeQuery("Intouch", sqlstr);

            return from dept in ds.Tables[0].AsEnumerable()
                   select new PMfrequencySetModel
                   {
                       Vendor = dept.Field<string>("vName"),
                       ToolID = dept.Field<string>("tool_id"),
                       LocationID = dept.Field<string>("lid"),
                       PMDate = dept.IsNull("PMDate") ? string.Empty : dept.Field<DateTime>("PMDate").ToString("yyyy/MM/dd"),
                       frequency = dept.IsNull("frequency") ? 0 : dept.Field<int>("frequency"),
                       tolerance = dept.IsNull("tolerance") ? 0 : dept.Field<int>("tolerance"),
                       //PlanPMDate = dept.Field<DateTime>("PMDate").AddDays(dept.Field<int>("frequency")).ToString("yyyy/MM/dd"),
                       //ToleranceFrom = dept.Field<DateTime>("PMDate").AddDays(dept.Field<int>("frequency") - dept.Field<int>("tolerance")).ToString("yyyy/MM/dd"),
                       //ToleranceTo = dept.Field<DateTime>("PMDate").AddDays(dept.Field<int>("frequency") + dept.Field<int>("tolerance")).ToString("yyyy/MM/dd"),
                       PlanPMDate = dept.IsNull("PlanPMDate") ? string.Empty : dept.Field<DateTime>("PlanPMDate").ToString("yyyy/MM/dd"),
                       ToleranceFrom = dept.IsNull("ToleranceFrom") ? string.Empty : dept.Field<DateTime>("ToleranceFrom").ToString("yyyy/MM/dd"),
                       ToleranceTo = dept.IsNull("ToleranceTo") ? string.Empty : dept.Field<DateTime>("ToleranceTo").ToString("yyyy/MM/dd"),
                       PMHistory = dept.IsNull("PMHistory") ? string.Empty : dept.Field<string>("PMHistory").Split(',').FirstOrDefault(),
                       PMHistoryALL = dept.Field<string>("PMHistory"),
                       userlogin = dept.Field<string>("userlogin"),
                       date = dept.IsNull("date") ? string.Empty : dept.Field<DateTime>("date").ToString("yyyy/MM/dd"),
                       onoffStatus = dept.IsNull("onoffStatus") ? "ON" : dept.Field<string>("onoffStatus") == "1" ? "ON" : "OFF",
                       PowerOnDate = dept.IsNull("PowerOnDate") ? "NULL" : dept.Field<DateTime>("PowerOnDate").ToString("yyyy/MM/dd"),
                       UnSetTooLate = dept.IsNull("UnSetTooLate") ? 0 : dept.Field<Decimal>("UnSetTooLate"),

                   };
        }

        //編輯PMSchedule的資料秀出
        public static PMfrequencySetModel GetPMSchedule(string toolid, int frequency, int tolerance, string UsrName)
        {
            //DataSet CodeDS = DBConnector.executeQuery("Intouch", "Select * from PM_Schedule where tool_id = '" + toolid + "'");
            DataSet CodeDS = DBConnector.executeQuery("Intouch", "SELECT VES.[pid], VPS.[tool_id],VPS.[PMDate],P.[UnSetTooLate],PS.[frequency],PS.[tolerance],PS.[userlogin],PS.[date],VPS.[onoffStatus],VPS.[PowerOnDate] FROM PM_Schedule PS right join vw_pm_schedule_control VPS  on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info P on VES.pid=P.plc_id where VPS.tool_id = '" + toolid + "'");

            DataTable dt = CodeDS.Tables[0];
            DateTime myDate = DateTime.Now;
            string myDateString = myDate.ToString("yyyy-MM-dd HH:mm:ss");
            // 將查詢到的資料回傳
            if (dt.Rows.Count == 0)
                return null;

            return new PMfrequencySetModel
            {
                //id = dt.Rows[0].IsNull("id") ? 0 : dt.Rows[0].Field<int>("id"),
                plc_id = dt.Rows[0].IsNull("pid") ? (int)0 : dt.Rows[0].Field<int>("pid"),
                ToolID = dt.Rows[0].IsNull("tool_id") ? toolid : dt.Rows[0].Field<string>("tool_id"),
                //freq_id = dt.Rows[0].IsNull("freq_id") ? (Byte)0 : dt.Rows[0].Field<Byte>("freq_id"),
                PMDate = dt.Rows[0].IsNull("PMDate") ? myDate.ToString("yyyy-MM-dd") : dt.Rows[0].Field<DateTime>("PMDate").ToString("yyyy-MM-dd"),
                frequency = dt.Rows[0].IsNull("frequency") ? 0 : dt.Rows[0].Field<int>("frequency"),
                tolerance = dt.Rows[0].IsNull("tolerance") ? 0 : dt.Rows[0].Field<int>("tolerance"),
                onoffStatus = dt.Rows[0].IsNull("onoffStatus") ? "ON" : dt.Rows[0].Field<string>("onoffStatus") == "1" ? "ON" : "OFF",
                PowerOnDate = dt.Rows[0].IsNull("PowerOnDate") ? myDate.ToString("yyyy-MM-dd") : dt.Rows[0].Field<DateTime>("PowerOnDate").ToString("yyyy-MM-dd"),
                userlogin = dt.Rows[0].IsNull("userlogin") ? UsrName : dt.Rows[0].Field<string>("userlogin"),
                UnSetTooLate = dt.Rows[0].IsNull("UnSetTooLate") ? 0 : dt.Rows[0].Field<Decimal>("UnSetTooLate"),
                date = dt.Rows[0].IsNull("date") ? myDateString : dt.Rows[0].Field<DateTime>("date").ToString(),
            };
        }

        //編輯PMSchedule的資料秀出
        public static PMfrequencySetModel GetPMSchedule1(string toolid, int frequency, int tolerance, string UsrName)
        {
            //string[] split = toolid.Split(',');
            //int i = 0;
            //foreach (string s in split)
            //{
            //    if (s != string.Empty)
            //    {
            //        i = i + 1;
            //    }

            //}
            //DataSet CodeDS = DBConnector.executeQuery("Intouch", "Select * from PM_Schedule where tool_id = '" + toolid + "'");
            //DataSet CodeDS = DBConnector.executeQuery("Intouch", "SELECT VES.[pid], VPS.[tool_id],VPS.[PMDate],PS.[frequency],PS.[tolerance],PS.[userlogin],PS.[date],VPS.[onoffStatus],VPS.[PowerOnDate] FROM PM_Schedule PS right join vw_pm_schedule_control VPS  on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid where VPS.tool_id = '" + toolid + "'");

            //DataTable dt = CodeDS.Tables[0];
            DateTime myDate = DateTime.Now;
            string myDateString = myDate.ToString("yyyy-MM-dd HH:mm:ss");
            // 將查詢到的資料回傳
            //if (dt.Rows.Count == 0)
            //{
            //    return null;
            //}
            return new PMfrequencySetModel
            {
                //id = dt.Rows[0].IsNull("id") ? 0 : dt.Rows[0].Field<int>("id"),
                //plc_id = dt.Rows[0].IsNull("pid") ? (int)i : dt.Rows[0].Field<int>("pid"),
                ToolID = toolid,
                //freq_id = dt.Rows[0].IsNull("freq_id") ? (Byte)0 : dt.Rows[0].Field<Byte>("freq_id"),
                //PMDate = dt.Rows[0].IsNull("PMDate") ? myDate.ToString("yyyy-MM-dd") : dt.Rows[0].Field<DateTime>("PMDate").ToString("yyyy-MM-dd"),
                frequency = 0,
                tolerance = 0,
                //onoffStatus = dt.Rows[0].IsNull("onoffStatus") ? "ON" : dt.Rows[0].Field<string>("onoffStatus") == "1" ? "ON" : "OFF",
                //PowerOnDate = dt.Rows[0].IsNull("PowerOnDate") ? myDate.ToString("yyyy-MM-dd") : dt.Rows[0].Field<DateTime>("PowerOnDate").ToString("yyyy-MM-dd"),
                userlogin = UsrName,
                date = myDateString,
            };
        }

        //PM frequency export
        public static DataTable dtReportList(string vName, string groupName)
        {
            DataTable dt = new DataTable();

            string sqlStr = string.Empty;
            if (vName == "Main")
            {
                sqlStr = "SELECT VES.[vName],VES.[tName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM vw_pm_schedule_control VPS right join  PM_Schedule PS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id";
            }
            else if (vName != "Main")
            {
                if (groupName == string.Empty)
                {
                    if (vName == "7PMwarning")
                    {
                        sqlStr = "SELECT VES.[vName],VES.[tName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM vw_pm_schedule_control VPS right join  PM_Schedule PS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id where dateadd(dd,-7,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]))<= GETDATE() and DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) >= GETDATE()";
                    }
                    else if (vName == "warningFrequency")
                    {
                        sqlStr = "SELECT VES.[vName],VES.[tName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM vw_pm_schedule_control VPS right join  PM_Schedule PS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id where  DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) <= GETDATE() and DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) >= GETDATE()";
                    }
                    else if (vName == "AfterFrequency")
                    {
                        sqlStr = "SELECT VES.[vName],VES.[tName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM vw_pm_schedule_control VPS right join  PM_Schedule PS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id where   DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) <= GETDATE()";
                    }
                    else
                    {
                        sqlStr = "SELECT VES.[vName],VES.[tName],VES.[groupName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM PM_Schedule PS  right join vw_pm_schedule_control VPS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id where vName='" + vName + "' and frequency is not NULL";
                    }
                    //sqlStr = "SELECT VES.[vName],VES.[tName],VES.[groupName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM PM_Schedule PS  right join vw_pm_schedule_control VPS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id where vName='" + vName + "' and frequency is not NULL";
                }
                else if (groupName != string.Empty)
                {
                    sqlStr = "SELECT VES.[vName],VES.[tName],VES.[groupName],VPS.[tool_id],VES.[lid],VPS.[PMDate],p.[UnSetTooLate],PS.[frequency],PS.[tolerance],DATEADD(dd,PS.[frequency],VPS.[PMDate]) as PlanPMDate,DATEADD(dd,PS.[frequency]-PS.[tolerance],VPS.[PMDate]) as ToleranceFrom,DATEADD(dd,PS.[frequency]+PS.[tolerance],VPS.[PMDate]) as ToleranceTo,VPS.[PMHistory],PS.[userlogin],PS.[date],p.[onoffStatus],p.[PowerOnDate] FROM PM_Schedule PS  right join vw_pm_schedule_control VPS on VPS.tool_id=PS.tool_id join vw_eq_status VES on VPS.tool_id = VES.toolid join plc_info p on VPS.tool_id = p.tool_id where vName='" + vName + "' and groupName='" + groupName + "' and frequency is not NULL";
                }

            }

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = WebConfigurationManager.ConnectionStrings["Intouch"].ConnectionString;
                conn.Open();

                dt.Load(new System.Data.SqlClient.SqlCommand(sqlStr, conn).ExecuteReader());
            }

            return dt;
        }

        public static string GetPMfrequencyexportFile(string vName, string groupName)
        {
            string FromFilePath = HttpContext.Current.Server.MapPath("../../Areas/Main/Views/Shared/PM History_New.xlsx");
            string ToFilePath = HttpContext.Current.Server.MapPath("../../Areas/Main/Views/Shared/PM History_Save.xlsx");
            DataTable dt = PMfrequencySetModel.dtReportList(vName, groupName);

            using (ExcelPackage p = new ExcelPackage(new FileInfo(FromFilePath)))
            {
                var sht = p.Workbook.Worksheets.FirstOrDefault();
                //sht.Cells[10, 1].Value = "已修改";
                if (dt.Rows.Count > 0)
                {
                    for (int i = 1; i <= dt.Rows.Count; i++)
                    {
                        sht.Cells[i + 3, 1].Value = dt.Rows[i - 1]["tool_id"];
                        sht.Cells[i + 3, 2].Value = dt.Rows[i - 1]["lid"];
                        sht.Cells[i + 3, 3].Value = dt.Rows[i - 1]["lid"];
                        sht.Cells[i + 3, 4].Value = dt.Rows[i - 1]["tName"];
                        sht.Cells[i + 3, 5].Value = dt.Rows[i - 1].IsNull("onoffStatus") ? "ON" : dt.Rows[i - 1].Field<string>("onoffStatus") == "1" ? "ON" : "OFF";
                        sht.Cells[i + 3, 6].Value = dt.Rows[i - 1].IsNull("PowerOnDate") ? string.Empty : dt.Rows[i - 1].Field<DateTime>("PowerOnDate").ToString("yyyy/MM/dd");
                        sht.Cells[i + 3, 7].Value = dt.Rows[i - 1].IsNull("PMDate") ? string.Empty : dt.Rows[i - 1].Field<DateTime>("PMDate").ToString("yyyy/MM/dd");
                        sht.Cells[i + 3, 8].Value = dt.Rows[i - 1].IsNull("frequency") ? 0 : dt.Rows[i - 1].Field<int>("frequency");
                        sht.Cells[i + 3, 9].Value = dt.Rows[i - 1].IsNull("tolerance") ? 0 : dt.Rows[i - 1].Field<int>("tolerance");
                        sht.Cells[i + 3, 10].Value = dt.Rows[i - 1].Field<DateTime>("PMDate").AddDays(dt.Rows[i - 1].Field<int>("frequency") - dt.Rows[i - 1].Field<int>("tolerance")).ToString("yyyy/MM/dd");
                        sht.Cells[i + 3, 11].Value = dt.Rows[i - 1].Field<DateTime>("PMDate").AddDays(dt.Rows[i - 1].Field<int>("frequency") + dt.Rows[i - 1].Field<int>("tolerance")).ToString("yyyy/MM/dd");
                        sht.Cells[i + 3, 12].Value = dt.Rows[i - 1].Field<DateTime>("PMDate").AddDays(dt.Rows[i - 1].Field<int>("frequency")).ToString("yyyy/MM/dd");
                        sht.Cells[i + 3, 13].Value = dt.Rows[i - 1]["tool_id"];
                        sht.Cells[i + 3, 14].Value = dt.Rows[i - 1].IsNull("PowerOnDate") ? string.Empty : dt.Rows[i - 1].Field<DateTime>("PowerOnDate").ToString("yyyy/MM/dd");
                        string split = dt.Rows[i - 1].IsNull("PMHistory") ? string.Empty : dt.Rows[i - 1].Field<string>("PMHistory");
                        if (split == string.Empty)
                        {
                            sht.Cells[i + 3, 15].Value = split;
                        }
                        else if (split != string.Empty)
                        {
                            string[] split1 = split.Split(',');
                            int j = 0;
                            foreach (string s in split1)
                            {
                                if (s.Trim() != string.Empty)
                                {
                                    sht.Cells[i + 3, 15 + j].Value = s;
                                    j = j + 1;
                                }
                            }
                        }

                        //sheet1.Cells[i + 1, 14].Value = dt.Rows[i - 1]["PMHistory"];
                        if (dt.Rows[i - 1].Field<DateTime>("PMDate").AddDays(dt.Rows[i - 1].Field<int>("frequency") - dt.Rows[i - 1].Field<int>("tolerance")) <= DateTime.Now && dt.Rows[i - 1].Field<DateTime>("PMDate").AddDays(dt.Rows[i - 1].Field<int>("frequency") + dt.Rows[i - 1].Field<int>("tolerance")) >= DateTime.Now)
                        {
                            sht.Cells[i + 3, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            sht.Cells[i + 3, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.HotPink);
                            sht.Cells[i + 3, 1].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            sht.Cells[i + 3, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            sht.Cells[i + 3, 7].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.HotPink);
                            sht.Cells[i + 3, 7].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        }
                        else if (dt.Rows[i - 1].Field<DateTime>("PMDate").AddDays(dt.Rows[i - 1].Field<int>("frequency") - dt.Rows[i - 1].Field<int>("tolerance") - 7) <= DateTime.Now && dt.Rows[i - 1].Field<DateTime>("PMDate").AddDays(dt.Rows[i - 1].Field<int>("frequency") - dt.Rows[i - 1].Field<int>("tolerance")) >= DateTime.Now)
                        {
                            sht.Cells[i + 3, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            sht.Cells[i + 3, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                            sht.Cells[i + 3, 1].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            sht.Cells[i + 3, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            sht.Cells[i + 3, 7].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                            sht.Cells[i + 3, 7].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        }
                        else if (dt.Rows[i - 1].Field<DateTime>("PMDate").AddDays(dt.Rows[i - 1].Field<int>("frequency") + dt.Rows[i - 1].Field<int>("tolerance")) <= DateTime.Now)
                        {
                            sht.Cells[i + 3, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            sht.Cells[i + 3, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                            sht.Cells[i + 3, 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
                            sht.Cells[i + 3, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            sht.Cells[i + 3, 7].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                            sht.Cells[i + 3, 7].Style.Font.Color.SetColor(System.Drawing.Color.White);
                        }

                    }

                    //sht.Cells.AutoFitColumns();

                }
                p.SaveAs(new FileInfo(ToFilePath));
                return ToFilePath;
            }
        }

        //編輯PMSchedule的Frequency及tolerance更新
        public string Update(string toolid, int frequency, int tolerance, string status, string powerondate, string UsrName, int UnSetTooLate)
        {
            string DBMsg = null;
            try
            {
                DBConnector.executeSQL("Intouch", "UPDATE PM_Schedule SET  frequency = " + frequency + ", tolerance = " + tolerance + ", userlogin = '" + UsrName + "' WHERE tool_id = '" + toolid + "'");
                DBConnector.executeSQL("Intouch", "UPDATE plc_info SET  onoffStatus = " + status + ", PowerOnDate = '" + powerondate + "', UnSetTooLate = " + UnSetTooLate + " WHERE tool_id = '" + toolid + "'");
                DBMsg = ToolID + "PM週期已更新完成\n";
            }
            catch (Exception ex)
            {
                DBMsg += "PM週期設定失敗 原因 : " + ex.Message + "\n";
            }

            return DBMsg;
        }

        //編輯PMSchedule的Frequency及tolerance新增
        public string addPM_Schedule(string plcid, string toolid, int freqid, int frequency, int tolerance, string status, string powerondate, string UsrName, int UnSetTooLate)
        {
            string DBMsg = null;
            try
            {
                if (freqid == 0)
                {
                    DBConnector.executeSQL("Intouch", "UPDATE plc_info SET  onoffStatus = " + status + ", PowerOnDate = '" + powerondate + "', UnSetTooLate = " + UnSetTooLate + " WHERE tool_id = '" + toolid + "'");
                }
                else if (freqid != 0)
                {
                    //DBConnector.executeSQL("Intouch", "UPDATE PM_Schedule SET  frequency = " + frequency + ", tolerance = " + tolerance + ", userlogin = '" + UsrName + "' WHERE tool_id = '" + toolid + "'");
                    DBConnector.executeSQL("Intouch", "INSERT INTO PM_Schedule (plc_id, tool_id, freq_id, frequency, tolerance, userlogin) VALUES (" + plcid + ", '" + toolid + "', " + freqid + ", " + frequency + ", " + tolerance + ", '" + UsrName + "')");
                    DBConnector.executeSQL("Intouch", "UPDATE plc_info SET  onoffStatus = " + status + ", PowerOnDate = '" + powerondate + "', UnSetTooLate = " + UnSetTooLate + " WHERE tool_id = '" + toolid + "'");
                }
                DBMsg = toolid + "PM週期已新增完成\n";
            }
            catch (Exception ex)
            {
                DBMsg += "PM週期設定失敗 原因 : " + ex.Message + "\n";
            }

            return DBMsg;
        }

        public string addPM_ScheduleALL(string toolid, int freqid, int frequency, int tolerance, string UsrName)
        {
            string DBMsg = null;
            try
            {
                string[] split = toolid.Split(',');
                //int i = 0;
                //string temp = string.Empty;
                foreach (string s in split)
                {
                    if (s.Trim() != string.Empty)
                    {
                        using (tsmc14BDataContext db = new tsmc14BDataContext())
                        {

                            var selectplcid = (from row in db.vw_eq_status where row.toolid == s select row.pid).FirstOrDefault();
                            //temp = temp + selectplcid;
                            DBConnector.executeSQL("Intouch", "INSERT INTO PM_Schedule (plc_id, tool_id, freq_id, frequency, tolerance, userlogin) VALUES (" + selectplcid + ", '" + s + "', " + freqid + ", " + frequency + ", " + tolerance + ", '" + UsrName + "')");
                        }
                    }
                }
                //DBConnector.executeSQL("Intouch", "UPDATE PM_Schedule SET  frequency = " + frequency + ", tolerance = " + tolerance + ", userlogin = '" + UsrName + "' WHERE tool_id = '" + toolid + "'");
                //DBConnector.executeSQL("Intouch", "INSERT INTO PM_Schedule (plc_id, tool_id, freq_id, frequency, tolerance, userlogin) VALUES (" + plcid + ", '" + toolid + "', " + freqid + ", " + frequency + ", " + tolerance + ", '" + UsrName + "')");
                //DBConnector.executeSQL("Intouch", "UPDATE plc_info SET  onoffStatus = " + status + ", PowerOnDate = '" + powerondate + "' WHERE tool_id = '" + toolid + "'");
                DBMsg = toolid + "PM週期已新增完成\n";
            }
            catch (Exception ex)
            {
                DBMsg += "PM週期設定失敗 原因 : " + ex.Message + "\n";
            }

            return DBMsg;
        }
    }
}