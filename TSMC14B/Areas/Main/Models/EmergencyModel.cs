using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using TSMC14B.Models;

namespace TSMC14B.Areas.Main.Models
{
    public class EmergencyModel
    {
        #region 組織成員屬性
        [Display(Name = "日期/時間")]
        public string AlarmTime { get; set; }

        [Display(Name = "ToolID")]
        public String toolID { get; set; }

        [Display(Name = "LocationID")]
        public String location_id { get; set; }

        [Display(Name = "柱位")]
        public String location { get; set; }

        [Display(Name = "廠牌")]
        public String vendor_name { get; set; }

        [Display(Name = "NGVMB")]
        public String VMB { get; set; }

        [Display(Name = "PowerSource")]
        public String PW_Source { get; set; }

        [Display(Name = "部門群組")]
        public String SendTo { get; set; }

        [Display(Name = "樓層")]
        public String eqFloor { get; set; }

        [Display(Name = "區域")]
        public String locArea { get; set; }

        [Display(Name = "製程")]
        public String Area { get; set; }

        [Display(Name = "Highlight")]
        public Int32 Highlight { get; set; }

        [Display(Name = "廠商編號")]
        public Int16 VendorId { get; set; }

        [Display(Name = "LocArea")]
        public String LocArea_l { get; set; }

        [Display(Name = "Area")]
        public String Area_l { get; set; }

        [Display(Name = "Alarm訊息")]
        public String AlarmMsg { get; set; }

        #endregion

        public static IEnumerable<EmergencyModel> EQDisconnectedList(string gName)
        {
            //DataSet ds = DBConnector.executeQuery("Intouch", "EXEC uSP_Select_EQDisconnected " + (gName == String.Empty ? null : "'" + gName + "'"));
            DataSet ds = DBConnector.executeQuery("Intouch", "EXEC uSP_Select_EQAlarmList " + (gName == String.Empty ? null : "'" + gName + "'"));

            return from dept in ds.Tables[0].AsEnumerable()
                   where dept.Field<Int16>("AlarmLevel") < 800
                       //orderby dept.Field<Byte>("level_id")
                   select new EmergencyModel
                       {
                           AlarmTime = dept.Field<DateTime>("AlarmTime").ToString("yyyy-MM-dd hh:mm:ss"),
                           toolID = dept.IsNull("toolID") ? String.Empty : dept.Field<String>("toolID").ToString(),
                           location_id = dept.IsNull("location_id") ? String.Empty : dept.Field<String>("location_id").ToString(),
                           location = dept.IsNull("location") ? String.Empty : dept.Field<String>("location").ToString(),
                           vendor_name = dept.IsNull("vendor_name") ? String.Empty : dept.Field<String>("vendor_name").ToString(),
                           VMB = dept.IsNull("VMB") ? String.Empty : dept.Field<String>("VMB").ToString(),
                           PW_Source = dept.IsNull("PW_Source") ? String.Empty : dept.Field<String>("PW_Source").ToString(),
                       };
            
        }

        public static IEnumerable<EmergencyModel> EQDisconnectedDeatil(string locArea,string Area)
        {
            DataSet ds = null;
            if (locArea == "A" && (Area == "ETCH" || Area == "EPI"))
            {
                Area = "ETCH";
                ds = DBConnector.executeQuery("Intouch", " EXEC [uSP_Select_EQAlarmList] @locArea = '" + locArea + "', @Area ='" + Area + "'");
                Area = "EPI";
                DataSet ds2 = DBConnector.executeQuery("Intouch", " EXEC [uSP_Select_EQAlarmList] @locArea = '" + locArea + "', @Area ='" + Area + "'");
                ds.Merge(ds2);
            }
            else
            {
                ds = DBConnector.executeQuery("Intouch", " EXEC [uSP_Select_EQAlarmList] @locArea = '" + locArea + "', @Area ='" + Area + "'");
            }

            return from dept in ds.Tables[0].AsEnumerable()
                   where dept.Field<Int16>("AlarmLevel") < 800
                   //where Convert.ToInt16(dept.Field<Byte>("level_id")) > 1
                   //orderby dept.Field<Byte>("level_id")
                   select new EmergencyModel
                   {
                       AlarmTime = dept.Field<DateTime>("AlarmTime").ToString("yyyy-MM-dd hh:mm:ss"),
                       toolID = dept.IsNull("toolID") ? String.Empty : dept.Field<String>("toolID").ToString(),
                       location_id = dept.IsNull("location_id") ? String.Empty : dept.Field<String>("location_id").ToString(),
                       location = dept.IsNull("location") ? String.Empty : dept.Field<String>("location").ToString(),
                       vendor_name = dept.IsNull("vendor_name") ? String.Empty : dept.Field<String>("vendor_name").ToString(),
                       VMB = dept.IsNull("VMB") ? String.Empty : dept.Field<String>("VMB").ToString(),
                       PW_Source = dept.IsNull("PW_Source") ? String.Empty : dept.Field<String>("PW_Source").ToString(),
                       AlarmMsg = dept.IsNull("AlarmMsg") ? String.Empty : dept.Field<String>("AlarmMsg").ToString(),
                   };

        }

        //PhoneCall Switch
        public static string ResetPhoneCallStatus(bool statusNow)
        {
            string DBMsg = null;

            try
            {
                DBConnector.executeSQL("Intouch", "EXEC [dbo].[uSP_Change_PhoneCallStatus] @statusNow=" + statusNow );

                if (statusNow)
                {
                    DBMsg += "PhoneCall 開啟完成";
                }
                else
                {
                    DBMsg += "PhoneCall 關閉完成";
                }
            }
            catch (Exception)
            {
                if (statusNow)
                {
                    DBMsg += "設定PhoneCall 啟動失敗!! 請洽系統管理員";
                }
                else
                {
                    DBMsg += "設定PhoneCall 關閉失敗!! 請洽系統管理員";
                }
            }

            return DBMsg;
        }

        public static IEnumerable<EmergencyModel> GetVendorgName()
        {
            DataSet ds = DBConnector.executeQuery("Intouch", " SELECT distinct(SendTo) FROM PhoneGroup ");

            return from dept in ds.Tables[0].AsEnumerable()
                   select new EmergencyModel
                   {
                       SendTo = dept.IsNull("SendTo") ? String.Empty : dept.Field<String>("SendTo").ToString(),
                   };

        }

        //Send Message
        public static string WriteToAlarm(string SendTo, string SendBody)
        {
            string DBMsg = String.Empty;
            string SuccMsg = "簡訊傳送完成";
            string FailgName = String.Empty;
            string SendType = "PHS";
            string Status = "NEW";
            string ServerName = "F14P6INSQL01";
            string Provider = "SQLNCLI10";
            string UserName = "P5";
            string Password = "P5";
            string AlarmTableName = "Alarm_Table";
            string SendContent = String.Empty;
            try
            {
                foreach (var gName in SendTo.Split(','))
                {
                    if (gName.Length > 0)
                    {
                        DBConnector.executeSQL("Intouch", "EXEC [dbo].[uSP_WriteToAlarmTable] @SendTo='" + gName + "', @SendBody='" + SendBody + "', @VoictText ='" + SendBody + "', @SendType ='" + SendType + "', @Status='" + Status + "', @ServerName = '" + ServerName + "', @Provider = '" + Provider + "', @UserName = '" + UserName + "', @Password = '" + Password + "', @AlarmTableName = '" + AlarmTableName + "'");
                        SendContent = gName + "|" + SendBody + "|" + SendBody + "|" + SendType + "|" + Status;

                        //延遲執行
                        System.Threading.Thread.Sleep(1500);

                        // 檢查是否寫入資料表
                        DataSet CodeDS = DBConnector.executeQuery("Intouch", "SELECT TOP 10 * FROM PhoneCallLog WHERE SendContent = '" + SendContent + "' AND CONVERT(varchar(10),LogDate,120) = CONVERT(varchar(10),GETDATE(),120) ");
                        DataTable dt = CodeDS.Tables[0];

                        if (dt.Rows.Count == 0)
                            FailgName += gName + ",";
                    }
                }
            } 
            catch (Exception)
            {
                FailgName = "All";
            }

            if (FailgName.Length == 0)
                DBMsg = SuccMsg;
            else
                DBMsg = "簡訊傳送 (" + FailgName.Substring (0,FailgName.Length-1) + ") 失敗!! 請洽系統管理員";

            return DBMsg;
        }

        public static IEnumerable<EmergencyModel> GetMircoLayout()
        {

            DataSet ds = DBConnector.executeQuery("Intouch", " SELECT * FROM vw_eq_areaStatus ");

            return from dept in ds.Tables[0].AsEnumerable()
                   select new EmergencyModel
                   {
                       eqFloor = dept.IsNull("eqFloor") ? String.Empty : dept.Field<String>("eqFloor").ToString(),
                       locArea = dept.IsNull("locArea") ? String.Empty : dept.Field<String>("locArea").ToString(),
                       Area = dept.IsNull("Area") ? String.Empty : dept.Field<String>("Area").ToString(),
                       Highlight = dept.IsNull("Highlight") ? (short)0 : dept.Field<Int32>("Highlight")
                   };
        }

        public static EmergencyModel getVendorid(string VendorName)
        {
            DataSet CodeDS = DBConnector.executeQuery("Intouch", "SELECT vendor_id FROM vendor_info WHERE vendor_name = '" + VendorName + "'");
            DataTable dt = (from code in CodeDS.Tables[0].AsEnumerable() select code).CopyToDataTable();
            if (dt.Rows.Count == 0)
                return null;
            return new EmergencyModel
            {
                VendorId = dt.Rows[0].Field<Int16>("vendor_id")
            };

        }

        public static IEnumerable<ListModel> AreaAlarmList()
        {
            DataSet ds = DBConnector.executeQuery("Intouch", "uSP_Select_EQAlarmList ");

            return from dept in ds.Tables[0].AsEnumerable()
                   where dept.Field<Int16>("AlarmLevel") < 1000 &&  !dept.IsNull("locArea") && !dept.IsNull("Area")
                   select new ListModel
                   {
                       Text = dept.Field<string>("locArea"),
                       Value = dept.Field<string>("Area"),
                   };
        }

    }
}