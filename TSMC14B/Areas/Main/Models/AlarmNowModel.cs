using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using TSMC14B.Models;

namespace TSMC14B.Areas.Main.Models
{
    public class AlarmNowModel
    {
        #region 組織成員屬性
        [Display(Name = "日期/時間")]
        public string _DateTime { get; set; }

        [Display(Name = "Tool ID")]
        public string ToolID { get; set; }

        [Display(Name = "柱位")]
        public string Location { get; set; }

        [Display(Name = "部門")]
        public string department_name { get; set; }

        [Display(Name = "AlarmValue")]
        public string AlarmValue { get; set; }

        [Display(Name = "Alarm訊息")]
        public string AlarmMessage { get; set; }

        [Display(Name = "AlarmLevel")]
        public Int16 AlarmLevel { get; set; }

        [Display(Name = "AlarmType")]
        public string AlarmType { get; set; }


        //[Display(Name = "解除警報音")]
        //public string AlarmAck { get; set; }

        #endregion

        public static IEnumerable<AlarmNowModel> alarmNowList(string alarmlevel,int preAlarm, int vendor)
        {
            DataTable gdt = new DataTable();
            string vendorStr = "null";
            if (vendor > 0) {
                vendorStr = Convert.ToString(vendor);
            }

            DataSet DeptDS = null;
            if (alarmlevel != "810")
            {
                DeptDS = DBConnector.executeQuery("Intouch", "EXEC [dbo].[uSP_Select_AlarmNow] NULL,'" + alarmlevel + "'," + preAlarm + "," + vendorStr);
            }
            else
            {
                DeptDS = DBConnector.executeQuery("Intouch", "EXEC [dbo].[uSP_Select_AlarmNow_DPM] NULL,'" + alarmlevel + "'," + preAlarm + "," + vendorStr);
            }

            return from dept in DeptDS.Tables[0].AsEnumerable()
                   select new AlarmNowModel
                   {
                       _DateTime = dept.IsNull("AlarmTime") ? string.Empty : dept.Field<DateTime>("AlarmTime").ToString("yyyy-MM-dd HH:mm:ss"),
                       ToolID = dept.Field<string>("toolID"),
                       Location = dept.Field<string>("location"),
                       //LocationID = dept.Field<string>("location_id"),
                       department_name = dept.Field<string>("department_name"),
                       AlarmValue = dept.IsNull("AlarmValue") ? string.Empty : String.Format("{0:F}", dept.Field<Double>("AlarmValue")),
                       AlarmType = dept.IsNull("AlarmValue") ? string.Empty : dept.Field<string>("AlarmType").Trim() == "LO" || dept.Field<string>("AlarmType").Trim() == "LOLO" || dept.Field<string>("AlarmType").Trim() == "HI" || dept.Field<string>("AlarmType").Trim() == "HIHI" ? dept.Field<string>("AlarmType") : string.Empty,
                       AlarmMessage = string.IsNullOrEmpty(dept.Field<string>("AlarmMsg")) ? string.Empty : dept.Field<string>("AlarmMsg"),
                       AlarmLevel = dept.IsNull("AlarmLevel") ? (short)500 : dept.Field<Int16>("AlarmLevel"),
                   };
        }

        public static IEnumerable<AlarmNowModel> EQAlarmNowList(string alarmlevel, int preAlarm, int vendor,string toolid)
        {
            DataTable gdt = new DataTable();
            string vendorStr = "null";
            if (vendor > 0)
            {
                vendorStr = Convert.ToString(vendor);
            }

            DataSet DeptDS = null;
            if (alarmlevel != "810")
            {
                DeptDS = DBConnector.executeQuery("Intouch", "EXEC [dbo].[uSP_Select_AlarmNow] NULL,'" + alarmlevel + "'," + preAlarm + "," + vendorStr + ",'" + toolid+"'");
            }
            else
            {
                DeptDS = DBConnector.executeQuery("Intouch", "EXEC [dbo].[uSP_Select_AlarmNow_DPM] NULL,'" + alarmlevel + "'," + preAlarm + "," + vendorStr + ",'" + toolid + "'");
            }

            return from dept in DeptDS.Tables[0].AsEnumerable()
                   select new AlarmNowModel
                   {
                       _DateTime = dept.IsNull("AlarmTime") ? string.Empty : dept.Field<DateTime>("AlarmTime").ToString("yyyy-MM-dd HH:mm:ss"),
                       ToolID = dept.Field<string>("toolID"),
                       Location = dept.Field<string>("location"),
                       //LocationID = dept.Field<string>("location_id"),
                       department_name = dept.Field<string>("department_name"),
                       AlarmValue = dept.IsNull("AlarmValue") ? string.Empty : String.Format("{0:F}", dept.Field<Double>("AlarmValue")),
                       AlarmType = dept.IsNull("AlarmValue") ? string.Empty : dept.Field<string>("AlarmType").Trim() == "LO" || dept.Field<string>("AlarmType").Trim() == "LOLO" || dept.Field<string>("AlarmType").Trim() == "HI" || dept.Field<string>("AlarmType").Trim() == "HIHI" ? dept.Field<string>("AlarmType") : string.Empty,
                       AlarmMessage = string.IsNullOrEmpty(dept.Field<string>("AlarmMsg")) ? string.Empty : dept.Field<string>("AlarmMsg"),
                       AlarmLevel = dept.IsNull("AlarmLevel") ? (short)500 : dept.Field<Int16>("AlarmLevel"),
                       //AlarmAck = dept.Field<string>("ack")
                   };
        }

        public static bool GetPCstatusLight()
        {
            DataTable gdt = new DataTable();

            DataSet DeptDS = DBConnector.executeQuery("Intouch", "EXEC [dbo].[uSP_Select_PhoneCallStatus]");

            return (from dept in DeptDS.Tables[0].AsEnumerable() select dept.Field<bool>("status_now")).FirstOrDefault();
        }
    }
}