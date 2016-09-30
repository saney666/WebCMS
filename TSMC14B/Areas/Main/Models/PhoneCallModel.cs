using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using TSMC14B.Models;

namespace TSMC14B.Areas.Main.Models
{
    public class PhoneCallModel
    {
        #region 組織成員屬性
        [Display(Name = "department_name", ResourceType = typeof(Menu))]
        public string department_name { get; set; }

        [Display(Name = "FullTagName")]
        public string FullTagName { get; set; }

        [Display(Name = "data_Tag")]
        public string data_Tag { get; set; }

        [Display(Name = "CallOut")]
        public bool CallOut { get; set; }

        [Display(Name = "plc_id")]
        public int plc_id { get; set; }

        [Display(Name = "ChamberName")]
        public string sensorID { get; set; }

        [Display(Name = "LastEditUser", ResourceType = typeof(Menu))]
        public string login_name { get; set; }

        [Display(Name = "EditTime")]
        public string builtdate { get; set; }

        [Display(Name = "AlarmID")]
        public string AlarmID { get; set; }
        #endregion

        public static DataTable GetPhoneCallListdt(string Tool, string PhoneCallType)
        {
            string sqlStr = "SELECT department_name,FullTagName,data_Tag,plc_id,sensorID,login_name,CallOut,Alarmid  FROM vw_PhoneCallSetting where fulltagName like '%[_]" + Tool + "[_]%' ";

            if (PhoneCallType == "Alarm" || PhoneCallType == "PreAlarm")
            {
                sqlStr += " AND data_Tag like '%[_]" + PhoneCallType + "'";
            }
            DataSet DeptDS = DBConnector.executeQuery("Intouch", sqlStr);

            return DeptDS.Tables[0];
        }

        public static IEnumerable<PhoneCallModel> GetPhoneCallList(string Tool, string PhoneCallType)
        {
            List<PhoneCallModel> PhoneCallList = new List<PhoneCallModel>();
            DataTable dt = GetPhoneCallListdt(Tool, PhoneCallType);

            PhoneCallList = (from dept in dt.AsEnumerable()
                             orderby dept.Field<string>("Alarmid")
                             select new PhoneCallModel
                              {
                                  department_name = dept.Field<string>("department_name").ToString(),
                                  FullTagName = dept.Field<string>("FullTagName").ToString(),
                                  data_Tag = dept.Field<string>("data_Tag"),
                                  CallOut = dept.Field<bool>("CallOut"),
                                  plc_id = dept.Field<int>("plc_id"),
                                  sensorID = dept.Field<string>("sensorID"),
                                  login_name = dept.Field<string>("login_name"),
                                  AlarmID = dept.Field<string>("Alarmid")
                              }).ToList();

            return PhoneCallList;
        }

        public static PhoneCallModel GetPhoneCall(string tagname)
        {
            PhoneCallModel PhoneCall = new PhoneCallModel();
            //DataTable dt = GetAlarmListdt(Tool);

            string sqlStr = "SELECT department_name,FullTagName,data_Tag,plc_id,sensorID,login_name,CallOut,Alarmid  FROM vw_PhoneCallSetting where fulltagName = '" + tagname + "'";

            DataSet DeptDS = DBConnector.executeQuery("Intouch", sqlStr);

            PhoneCall = (from dept in DeptDS.Tables[0].AsEnumerable()
                         select new PhoneCallModel
                          {
                              department_name = dept.Field<string>("department_name"),
                              FullTagName = dept.Field<string>("FullTagName"),
                              data_Tag = dept.Field<string>("data_Tag"),
                              CallOut = dept.Field<bool>("CallOut"),
                              plc_id = dept.Field<int>("plc_id"),
                              sensorID = dept.Field<string>("sensorID"),
                              login_name = dept.Field<string>("login_name"),
                              AlarmID = dept.Field<string>("Alarmid")
                          }).FirstOrDefault();

            return PhoneCall;
        }

        public string Update(string Usr)
        {
            try
            {

                DBConnector.executeSQL("Intouch", "EXEC [dbo].[uSP_Change_PhoneCallSetting] @FullTagName='" + FullTagName + "',@data_Tag='" + data_Tag + "',@plc_id=" + plc_id + ",@sensorID='" + sensorID + "',@CallOut=" + CallOut + ",@login_name='" + Usr + "'");
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return null;
        }

        internal static string SetAllSwitch(string tool, bool Switch, string Usr)
        {
            string Str = "";
            string rString = "";

            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                var rr = from row in db.vw_PhoneCallSetting where row.FullTagName.Contains(tool) select row;

                try
                {
                    foreach (vw_PhoneCallSetting item in rr)
                    {
                        DBConnector.executeSQL("Intouch", "EXEC [dbo].[uSP_Change_PhoneCallSetting] @FullTagName='" + item.FullTagName + "',@data_Tag='" + item.data_Tag + "',@plc_id=" + item.plc_id + ",@sensorID='" + item.sensorID + "',@CallOut=" + Switch + ",@login_name='" + Usr + "'");
                    }

                    rString = "Success";
                }
                catch (Exception ex)
                {
                    rString = ex.Message;
                }
            }
            return rString;
        }

        internal static string SetPhoneCallSetting(string[] data, string Usr)
        {
            string rString = "";

            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                foreach (string item in data)
                {
                    string fulltagname = item.Split(':')[0];
                    bool setting = bool.Parse(item.Split(':')[1]);
                    var rr = (from row in db.vw_PhoneCallSetting where row.FullTagName == fulltagname select row).SingleOrDefault();

                    try
                    {
                        if (rr!=null)
                        {
                            DBConnector.executeSQL("Intouch", "EXEC [dbo].[uSP_Change_PhoneCallSetting] @FullTagName='" + rr.FullTagName + "',@data_Tag='" + rr.data_Tag + "',@plc_id=" + rr.plc_id + ",@sensorID='" + rr.sensorID + "',@CallOut=" + setting + ",@login_name='" + Usr + "'");
                            rString = "Setting OK";
                        }
                        else
                        {
                            rString = "Setting Fail";
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        rString = ex.Message;
                    }
                }
            }
            return rString;
        }

        internal static IEnumerable<PhoneCallModel> GetPhoneCallHistory(string tagname)
        {
            IEnumerable<PhoneCallModel> PhoneCallHistory;
            //DataTable dt = GetAlarmListdt(Tool);

            string sqlStr = "SELECT FullTagName,data_Tag,Callout,login_name,builtdate FROM PhoneCallSrttingHistory where FullTagName='"+tagname+"' order by builtdate desc";

            DataSet DeptDS = DBConnector.executeQuery("Intouch", sqlStr);

            PhoneCallHistory = (from dept in DeptDS.Tables[0].AsEnumerable()
                         select new PhoneCallModel
                         {
                             //department_name = dept.Field<string>("department_name"),
                             FullTagName = dept.Field<string>("FullTagName"),
                             data_Tag = dept.Field<string>("data_Tag"),
                             builtdate = dept.Field<DateTime>("builtdate").ToString("yyyy-MM-dd HH:mm:ss"),
                             CallOut = dept.Field<bool>("CallOut"),
                             //plc_id = dept.Field<int>("plc_id"),
                             //sensorID = dept.Field<string>("sensorID"),
                             login_name = dept.Field<string>("login_name")
                         }).ToList();

            return PhoneCallHistory;
        }

        internal static byte[] GetPhoneCallFile(string tool, string phoneCallType)
        {
            IEnumerable<PhoneCallModel> al = null;
            al = PhoneCallModel.GetPhoneCallList(tool, phoneCallType);

            ExcelPackage ep = new ExcelPackage();
            ep.Workbook.Worksheets.Add("MySheet");
            ExcelWorksheet sheet1 = ep.Workbook.Worksheets["MySheet"];

            sheet1.Cells["A1"].Value = "FullTagName";
            sheet1.Cells["B1"].Value = "部門";
            sheet1.Cells["C1"].Value = "AlarmID";
            sheet1.Cells["D1"].Value = "DataTag";
            sheet1.Cells["E1"].Value = "Setting";
            sheet1.Cells["F1"].Value = "plc_id";
            sheet1.Cells["G1"].Value = "sensorID";
            sheet1.Cells["H1"].Value = "最後修改者";
            
            if (al.Count() > 0)
            {
                //sheet1.Cells["A2"].LoadFromDataTable(dt, false);
                int i = 2;
                foreach (var item in al)
                {
                    sheet1.Cells["A" + i].Value = item.FullTagName;//"AlarmId"
                    sheet1.Cells["B" + i].Value = item.department_name;//"時間"
                    sheet1.Cells["C" + i].Value = item.AlarmID;//"Alarm 訊息"
                    sheet1.Cells["D" + i].Value = item.data_Tag;//"ToolID"
                    sheet1.Cells["E" + i].Value = item.CallOut;//"數值"
                    sheet1.Cells["F" + i].Value = item.plc_id;//"Alarm 訊息"
                    sheet1.Cells["G" + i].Value = item.sensorID;//"Alarm 訊息"
                    sheet1.Cells["H" + i].Value = item.login_name;//"Alarm 訊息"
                    
                    i++;
                }
                sheet1.Cells.AutoFitColumns();
                sheet1.Column(1).Width = 0;
                sheet1.Column(5).Width = 0;
                sheet1.Column(6).Width = 0;
            }
            return ep.GetAsByteArray();
        }
    }
}