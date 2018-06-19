using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using WebCMS.Models;

namespace WebCMS.Areas.Main.Models
{
    public class AlarmCheckModel
    {
        #region 組織成員屬性
        [Display(Name = "AlarmId")]
        public string AlarmId { get; set; }

        [Display(Name = "週別")]
        public string AlarmWeek { get; set; }

        [Display(Name = "AlarmTime_Begin")]
        public string AlarmTime_Begin { get; set; }

        [Display(Name = "Group")]
        public string Group { get; set; }

        [Display(Name = "ToolID")]
        public string ToolID { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Location_id")]
        public string Location_id { get; set; }

        [Display(Name = "廠商")]
        public string Vendor { get; set; }

        [Display(Name = "Alarm訊息")]
        public string AlarmMessage { get; set; }

        [Display(Name = "AlarmLevel")]
        public Int16 AlarmLevel { get; set; }

        //[Display(Name = "解除警報音")]
        //public string AlarmAck { get; set; }

        #endregion

        public static DataTable GetAlarmListdt(string vName, string fromDate, string toDate, string Toolid, string Device, string onceadayFlag, string alarmleve)
        {

            DateTime _fromdate = Convert.ToDateTime(fromDate);
            DateTime _toDate = Convert.ToDateTime(toDate);
            string _toolid = "NULL";
            string sqlStr = "";
            if (vName == "DPM")
            {
                sqlStr = "EXEC [dbo].[uSP_Select_AlarmHistory_DPM] N'" + _fromdate.ToString("yyyy-MM-dd HH:mm") + "',N'" + _toDate.ToString("yyyy-MM-dd HH:mm") + "'";
            }
            else
            {
                sqlStr = "EXEC [dbo].[uSP_Select_AlarmHistory] N'" + _fromdate.ToString("yyyy-MM-dd HH:mm") + "',N'" + _toDate.ToString("yyyy-MM-dd HH:mm") + "'";
            }
            
            if (!string.IsNullOrEmpty(Toolid) && Toolid.ToUpper() != "NULL" && Toolid.Length > 0)
            {
                _toolid = "'" + Toolid + "'";
                sqlStr = sqlStr + "," + _toolid + "," + ListModel.GetVid(vName);
            }
            else
            {
                sqlStr = sqlStr + ",NULL," + ListModel.GetVid(vName);

                //if (!string.IsNullOrEmpty(Device) && Device.ToUpper() != "NULL" && Device.Length > 0)
                //{
                //    sqlStr = sqlStr + ",'" + Device + "'";
                //}

            }

            if  (!string.IsNullOrEmpty(Device) && Device.ToUpper() != "NULL" && Device.Length > 0)
            {
                sqlStr = sqlStr + ",'" + Device + "','" + alarmleve + "'";
            }
            else{
                sqlStr = sqlStr + ",NULL,'" + alarmleve + "'";
            }
            
            //bool.TryParse(onceadayFlag, out _onceadayFlag);

            //DataSet DeptDS = DBConnector.executeQuery("Intouch", "EXEC [dbo].[uSP_Select_AlarmHistory] '" + _fromdate + "','" + _toDate + "','" + Toolid + "'," + _onceadayFlag);
            //DataSet DeptDS = DBConnector.executeQuery("Intouch", "EXEC [dbo].[uSP_Select_AlarmHistory] N'" + fromDate + "',N'" + toDate + "',NULL,0");
            DataSet DeptDS = DBConnector.executeQuery("Intouch", sqlStr);

            //var dtt = (from row in DeptDS.Tables[0].AsEnumerable() select new { row["AlarmId"],row["AlarmWeek"],row["AlarmTime_Begin"],row["Group_name"],row["toolID"],row["location"],row["Location_id"],row["vendor_name"],row["AlarmMsg"],row["AlarmLevel"]}).CopyToDataTable();

            //DataTable dt = (from row in DeptDS.Tables[0].AsEnumerable() where row.Field<string>("vendor_name") == vName select row).CopyToDataTable<DataRow>();

            //DataTable newdt = new DataTable();

            //newdt.Columns.Add("AlarmId", Type.GetType("System.Int32"));
            //newdt.Columns.Add("AlarmWeek", Type.GetType("System.String"));
            //newdt.Columns.Add("AlarmTime_Begin", Type.GetType("System.DateTime"));
            //newdt.Columns.Add("Group_name", Type.GetType("System.String"));
            //newdt.Columns.Add("toolID", Type.GetType("System.String"));
            //newdt.Columns.Add("location", Type.GetType("System.String"));
            //newdt.Columns.Add("Location_id", Type.GetType("System.String"));
            //newdt.Columns.Add("vendor_name", Type.GetType("System.String"));
            //newdt.Columns.Add("AlarmMsg", Type.GetType("System.String"));
            //newdt.Columns.Add("AlarmLevel", Type.GetType("System.Int16"));


            //foreach (DataRow row in dt.Rows)
            //{
            //    DataRow nrow = newdt.NewRow();

            //    nrow["AlarmId"] = row["AlarmId"];
            //    nrow["AlarmWeek"] = row["AlarmWeek"];
            //    nrow["AlarmTime_Begin"] = row["AlarmTime_Begin"];
            //    nrow["Group_name"] = row["Group_name"];
            //    nrow["toolID"] = row["toolID"];
            //    nrow["location"] = row["location"];
            //    nrow["Location_id"] = row["Location_id"];
            //    nrow["vendor_name"] = row["vendor_name"];
            //    nrow["AlarmMsg"] = row["AlarmMsg"];
            //    nrow["AlarmLevel"] = row["AlarmLevel"];

            //    newdt.Rows.Add(nrow);
            //}

            return DeptDS.Tables[0];
        }

        public static IEnumerable<AlarmNowModel> GetAlarmList(string vName, string fromDate, string toDate, string Toolid, string Device, string onceadayFlag, string alarmlevel)
        {
            List<AlarmNowModel> AlarmCheckList = new List<AlarmNowModel>();
            DataTable dt = GetAlarmListdt(vName, fromDate, toDate, Toolid, Device, onceadayFlag, alarmlevel);

            AlarmCheckList = (from dept in dt.AsEnumerable()
                              select new AlarmNowModel
                              {
                                  //AlarmId = dept.IsNull("AlarmId") ? string.Empty : dept.Field<Int32>("AlarmId").ToString(),
                                  //AlarmWeek = "W" + dept.Field<string>("AlarmWeek"),
                                  _DateTime = dept.IsNull("AlarmTime_Begin") ? string.Empty : dept.Field<DateTime>("AlarmTime_Begin").ToString("yyyy-MM-dd HH:mm:ss"),
                                  //Group = dept.IsNull("Group_name") ? string.Empty : dept.Field<string>("Group_name"),
                                  ToolID = dept.Field<string>("toolID"),
                                  Location = dept.Field<string>("location"),
                                  //Location_id = dept.Field<string>("Location_id"),
                                  department_name = dept.Field<string>("department_name"),
                                  AlarmValue = dept.Field<string>("ValueString"),
                                  AlarmType = dept.Field<string>("AlarmType"),
                                  AlarmMessage = dept.Field<string>("AlarmMsg"),
                                  AlarmLevel = dept.IsNull("AlarmLevel") ? (short)500 : dept.Field<Int16>("AlarmLevel")
                              }).ToList();

            return AlarmCheckList;
            //if (Toolid.Length == 0 && Device.Length > 0)
            //{
            //    AlarmCheckList = (from dept in dt.AsEnumerable()
            //                      where dept.Field<string>("vendor_name") == vName && dept.Field<string>("Group_name") == Device
            //                      select new AlarmCheckModel
            //                      {
            //                          AlarmId = dept.Field<Int32>("AlarmId").ToString(),
            //                          AlarmWeek = "W"+dept.Field<string>("AlarmWeek"),
            //                          AlarmTime_Begin = dept.Field<DateTime>("AlarmTime_Begin").ToString("yyyy-MM-dd HH:mm:ss"),
            //                          Group = dept.IsNull("Group_name")? string.Empty : dept.Field<string>("Group_name"),
            //                          ToolID = dept.Field<string>("toolID"),
            //                          Location = dept.Field<string>("location"),
            //                          Location_id = dept.Field<string>("Location_id"),
            //                          Vendor = dept.Field<string>("vendor_name"),
            //                          AlarmMessage = dept.Field<string>("AlarmMsg"),
            //                          AlarmLevel = dept.Field<Int16>("AlarmLevel")
            //                      }).ToList();
            //}
            //else
            //{
            //    AlarmCheckList = (from dept in dt.AsEnumerable()
            //                      where dept.Field<string>("vendor_name") == vName
            //                      select new AlarmCheckModel
            //                      {
            //                          AlarmId = dept.Field<Int32>("AlarmId").ToString(),
            //                          AlarmWeek = "W"+dept.Field<string>("AlarmWeek"),
            //                          AlarmTime_Begin = dept.Field<DateTime>("AlarmTime_Begin").ToString("yyyy-MM-dd HH:mm:ss"),
            //                          Group = dept.IsNull("Group_name")? string.Empty : dept.Field<string>("Group_name"),
            //                          ToolID = dept.Field<string>("toolID"),
            //                          Location = dept.Field<string>("location"),
            //                          Location_id = dept.Field<string>("Location_id"),
            //                          Vendor = dept.Field<string>("vendor_name"),
            //                          AlarmMessage = dept.Field<string>("AlarmMsg"),
            //                          AlarmLevel = dept.Field<Int16>("AlarmLevel")
            //                      }).ToList();
            //}


        }

        public static byte[] GetAlarmExportFile(string vName, string fromDate, string toDate, string Toolid, string Device, string onceadayFlag, string alarmlevel)
        {
            ExcelPackage ep = new ExcelPackage();
            ep.Workbook.Worksheets.Add("Sheet1");
            ExcelWorksheet sheet1 = ep.Workbook.Worksheets["Sheet1"];
            //DataTable dt = AlarmCheckModel.GetAlarmListdt(vName, fromDate, toDate, ToolID, Device, onceadayFlag);

            IEnumerable<AlarmNowModel> Al = AlarmCheckModel.GetAlarmList(vName, fromDate, toDate, Toolid, Device, onceadayFlag, alarmlevel);

            sheet1.Cells["A1"].Value = "時間";
            sheet1.Cells["B1"].Value = "ToolId";
            sheet1.Cells["C1"].Value = "柱位";
            sheet1.Cells["D1"].Value = "部門";
            sheet1.Cells["E1"].Value = "數值";
            sheet1.Cells["F1"].Value = "狀態";
            //sheet1.Cells["G1"].Value = "LocationID";
            sheet1.Cells["G1"].Value = "Alarm 訊息";
            sheet1.Cells["H1"].Value = "Alarm Level";
            
            int Alcount = Al.Count();
            if (Alcount > 0)
            {
                //sheet1.Cells["A2"].LoadFromDataTable(dt, false);
                sheet1.Cells["A2"].LoadFromCollection<AlarmNowModel>(Al);
                for (int i = 1; i <= Alcount; i++)
                {
                    sheet1.Cells[i, 2].Style.Numberformat.Format = "yyyy-mm-dd HH:mm:ss";
                }
            }

            sheet1.Cells.AutoFitColumns();
            //sheet1.Column(1).Width = 0;

            return ep.GetAsByteArray();
        }

        public static IEnumerable<AlarmNowModel> GetPLCAlarmList(string fromDate, string toDate, string Toolid, string alarmlevel)
        {
            List<AlarmNowModel> AlarmCheckList = new List<AlarmNowModel>();
            DataTable dt = GetAlarmListdt("NULL", fromDate, toDate, Toolid, "NULL", "NULL", alarmlevel);

            AlarmCheckList = (from dept in dt.AsEnumerable()
                              select new AlarmNowModel
                              {
                                  //AlarmId = dept.IsNull("AlarmId") ? string.Empty : dept.Field<Int32>("AlarmId").ToString(),
                                  //AlarmWeek = "W" + dept.Field<string>("AlarmWeek"),
                                  _DateTime = dept.IsNull("AlarmTime_Begin") ? string.Empty : dept.Field<DateTime>("AlarmTime_Begin").ToString("yyyy-MM-dd HH:mm:ss"),
                                  //Group = dept.IsNull("Group_name") ? string.Empty : dept.Field<string>("Group_name"),
                                  ToolID = dept.Field<string>("toolID"),
                                  Location = dept.Field<string>("location"),
                                  //Location_id = dept.Field<string>("Location_id"),
                                  AlarmValue = dept.Field<string>("ValueString"),
                                  AlarmType = dept.Field<string>("AlarmType"),
                                  department_name = dept.Field<string>("department_name"),
                                  AlarmMessage = dept.Field<string>("AlarmMsg"),
                                  AlarmLevel = dept.IsNull("AlarmLevel") ? (short)500 : dept.Field<Int16>("AlarmLevel")
                              }).ToList();

            return AlarmCheckList;
        
        }
    }
}