using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using WebCMS.Models;

namespace WebCMS.Areas.Main.Models
{
    public class AlarmLimitModel
    {
        #region 組織成員屬性
        [Display(Name = "department_name", ResourceType = typeof(Menu))]
        public string department_name { get; set; }

        [Display(Name = "FullTagName")]
        public string FullTagName { get; set; }

        [Display(Name = "FDC_Tag")]
        public string FDC_Tag { get; set; }

        [Display(Name = "Type_Name")]
        public string type_name { get; set; }

        [Display(Name = "Toolid")]
        public string Toolid { get; set; }

        [Display(Name = "ChamberName")]
        public string ChamberName { get; set; }

        [Display(Name = "Limit_Max_Setting", ResourceType = typeof(Menu)), Range(-32767, 32766)]
        public double? Limit_Max_Setting { get; set; }

        [Display(Name = "Limit_Max", ResourceType = typeof(Menu)), Range(-32767, 32766)]
        public double? Limit_Max { get; set; }

        [Display(Name = "IsHiEnable", ResourceType = typeof(Menu))]
        public bool? IsHiEnable { get; set; }

        [Display(Name = "Limit_Min_Setting", ResourceType = typeof(Menu)), Range(-32767, 32766)]
        public double? Limit_Min_Setting { get; set; }

        [Display(Name = "Limit_Min", ResourceType = typeof(Menu)), Range(-32767, 32766)]
        public double? Limit_Min { get; set; }

        [Display(Name = "IsLoEnable", ResourceType = typeof(Menu))]
        public bool? IsLoEnable { get; set; }

        [Display(Name = "DelayTime(sec)")]
        [Range(0, 60), Required()]
        public int DelayTime { get; set; }

        [Display(Name = "LastEditUser", ResourceType = typeof(Menu))]
        public string login_name { get; set; }

        [Display(Name = "Limit_Max_Type")]
        public double? Limit_Max_Type { get; set; }

        [Display(Name = "Limit_Min_Type")]
        public double? Limit_Min_Type { get; set; }

        [Display(Name = "builddate")]
        public string builddate { get; set; }
        #endregion

        public static DataTable GetToolAlarmListdt(string Tool)
        {
            string sqlStr = "SELECT department_name,FullTagName,FDC_Tag,Limit_Max_Setting,Limit_Max_Type,Limit_Max,IsHiEnable,Limit_Min_Setting,Limit_Min_Type,Limit_Min,IsLoEnable,delayTime,login_name,type_name,SUBSTRING( FDC_Tag,0,CHARINDEX('_', FDC_Tag,0)) as Toolid,SUBSTRING(FullTagName, CHARINDEX('_', FullTagName,0)+1,CHARINDEX('_', FullTagName,5)-4) as AccessoryName FROM vw_eq_limit ";

            if (!string.IsNullOrEmpty(Tool))
            {
                sqlStr = sqlStr + " where fulltagName like '%[_]" + Tool + "[_]%'";
            }

            DataSet DeptDS = DBConnector.executeQuery("Intouch", sqlStr);

            return DeptDS.Tables[0];
        }

        public static DataTable GetParameterAlarmListdt(string vendor, string parameter)
        {
            string sqlStr = "SELECT department_name,FullTagName,FDC_Tag,Limit_Max_Setting,Limit_Max_Type,Limit_Max,IsHiEnable,Limit_Min_Setting,Limit_Min_Type,Limit_Min,IsLoEnable,delayTime,login_name FROM vw_eq_limit where fulltagName like '%" + parameter + "' ";

            if (!string.IsNullOrEmpty(vendor))
            {
                sqlStr = sqlStr + " and department_name='" + vendor + "' ";
            }

            DataSet DeptDS = DBConnector.executeQuery("Intouch", sqlStr);

            return DeptDS.Tables[0];
        }

        public static DataTable GetParameterAlarmListdt(string vendor, string Tool, string parameter)
        {
            string sqlStr = "SELECT department_name,FullTagName,FDC_Tag,Limit_Max_Setting,Limit_Max_Type,Limit_Max,IsHiEnable,Limit_Min_Setting,Limit_Min_Type,Limit_Min,IsLoEnable,delayTime,login_name,type_name FROM vw_eq_limit where  1=1 ";


            if (!string.IsNullOrEmpty(parameter))
            {
                sqlStr = sqlStr + " and fulltagName like '%" + parameter + "' ";
            }

            if (!string.IsNullOrEmpty(Tool))
            {
                sqlStr = sqlStr + " and fulltagName like '%[_]" + Tool + "[_]%' ";
            }

            if (!string.IsNullOrEmpty(vendor))
            {
                sqlStr = sqlStr + " and department_name='" + vendor + "' ";
            }

            DataSet DeptDS = DBConnector.executeQuery("Intouch", sqlStr);

            return DeptDS.Tables[0];
        }

        public static IEnumerable<AlarmLimitModel> GetToolLimitList(string Tool)
        {
            List<AlarmLimitModel> AlarmLimitList = new List<AlarmLimitModel>();
            DataTable dt = GetToolAlarmListdt(Tool);

            AlarmLimitList = (from dept in dt.AsEnumerable()
                              select new AlarmLimitModel
                              {
                                  department_name = dept.Field<string>("department_name").ToString(),
                                  FullTagName = dept.Field<string>("FullTagName").ToString(),
                                  FDC_Tag = dept.Field<string>("FDC_Tag").ToString(),
                                  type_name = dept.Field<string>("type_name").ToString(),
                                  Toolid = dept.Field<string>("Toolid").ToString(),
                                  ChamberName = dept.Field<string>("AccessoryName").ToString(),
                                  Limit_Max_Setting = dept.Field<double?>("Limit_Max_Setting"),
                                  Limit_Max_Type = dept.Field<double?>("Limit_Max_Type"),
                                  Limit_Max = dept.Field<double?>("Limit_Max"),
                                  IsHiEnable = dept.Field<bool?>("IsHiEnable"),
                                  Limit_Min_Setting = dept.Field<double?>("Limit_Min_Setting"),
                                  Limit_Min_Type = dept.Field<double?>("Limit_Min_Type"),
                                  Limit_Min = dept.Field<double?>("Limit_Min"),
                                  IsLoEnable = dept.Field<bool?>("IsLoEnable"),
                                  DelayTime = dept.Field<int>("delayTime"),
                                  login_name = dept.Field<string>("login_name")
                              }).ToList();

            return AlarmLimitList;
        }

        public static IEnumerable<AlarmLimitModel> GetParameterLimitList(string vendor, string tool, string parameter)
        {
            List<AlarmLimitModel> AlarmLimitList = new List<AlarmLimitModel>();
            DataTable dt = GetParameterAlarmListdt(vendor, tool, parameter);

            AlarmLimitList = (from dept in dt.AsEnumerable()
                              select new AlarmLimitModel
                              {
                                  department_name = dept.Field<string>("department_name").ToString(),
                                  FullTagName = dept.Field<string>("FullTagName").ToString(),
                                  FDC_Tag = dept.Field<string>("FDC_Tag").ToString(),
                                  type_name = dept.Field<string>("type_name").ToString(),
                                  Limit_Max_Setting = dept.Field<double?>("Limit_Max_Setting"),
                                  Limit_Max_Type = dept.Field<double?>("Limit_Max_Type"),
                                  Limit_Max = dept.Field<double?>("Limit_Max"),
                                  IsHiEnable = dept.Field<bool?>("IsHiEnable"),
                                  Limit_Min_Setting = dept.Field<double?>("Limit_Min_Setting"),
                                  Limit_Min_Type = dept.Field<double?>("Limit_Min_Type"),
                                  Limit_Min = dept.Field<double?>("Limit_Min"),
                                  IsLoEnable = dept.Field<bool?>("IsLoEnable"),
                                  DelayTime = dept.Field<int>("delayTime"),
                                  login_name = dept.Field<string>("login_name")
                              }).ToList();

            return AlarmLimitList;
        }

        public static AlarmLimitModel GetLimit(string tagname)
        {
            AlarmLimitModel AlarmLimit = new AlarmLimitModel();

            string sqlStr = "SELECT department_name,FullTagName,FDC_Tag,Limit_Max,Limit_Max_Type,IsHiEnable,Limit_Min,Limit_Min_Type,IsLoEnable,delayTime FROM vw_eq_limit where fulltagName = '" + tagname + "'";

            DataSet DeptDS = DBConnector.executeQuery("Intouch", sqlStr);

            AlarmLimit = (from dept in DeptDS.Tables[0].AsEnumerable()
                          select new AlarmLimitModel
                          {
                              department_name = dept.Field<string>("department_name"),
                              FullTagName = dept.Field<string>("FullTagName"),
                              FDC_Tag = dept.Field<string>("FDC_Tag"),
                              Limit_Max = dept.Field<double?>("Limit_Max"),
                              Limit_Max_Type = dept.Field<double?>("Limit_Max_Type"),
                              IsHiEnable = dept.Field<bool?>("IsHiEnable"),
                              Limit_Min = dept.Field<double?>("Limit_Min"),
                              Limit_Min_Type = dept.Field<double?>("Limit_Min_Type"),
                              IsLoEnable = dept.Field<bool?>("IsLoEnable"),
                              DelayTime = dept.Field<int>("delayTime")
                          }).FirstOrDefault();

            return AlarmLimit;
        }

        public static AlarmLimitModel GetAllLimit(string tagname)
        {
            AlarmLimitModel AlarmLimit = new AlarmLimitModel();

            //string sqlStr = "SELECT department_name,FullTagName,FDC_Tag,Limit_Max,Limit_Max_Type,IsHiEnable,Limit_Min,Limit_Min_Type,IsLoEnable,delayTime FROM vw_eq_limit where fulltagName = '" + tagname + "'";
            //DataSet DeptDS = DBConnector.executeQuery("Intouch", sqlStr);
            //AlarmLimit = (from dept in DeptDS.Tables[0].AsEnumerable()
            //              select new AlarmLimitModel
            //              {
            //                  department_name = dept.Field<string>("department_name"),
            //                  FullTagName = dept.Field<string>("FullTagName"),
            //                  FDC_Tag = dept.Field<string>("FDC_Tag"),
            //                  Limit_Max = dept.Field<double?>("Limit_Max"),
            //                  Limit_Max_Type = dept.Field<double?>("Limit_Max_Type"),
            //                  IsHiEnable = dept.Field<bool?>("IsHiEnable"),
            //                  Limit_Min = dept.Field<double?>("Limit_Min"),
            //                  Limit_Min_Type = dept.Field<double?>("Limit_Min_Type"),
            //                  IsLoEnable = dept.Field<bool?>("IsLoEnable"),
            //                  DelayTime = dept.Field<int>("delayTime")
            //              }).FirstOrDefault();

            AlarmLimit.FullTagName = tagname;
            AlarmLimit.FDC_Tag = tagname;
            AlarmLimit.Limit_Max = 32766;
            AlarmLimit.Limit_Min = -32767;
            AlarmLimit.DelayTime = 0;
            AlarmLimit.IsHiEnable = true;
            AlarmLimit.IsLoEnable = true;

            return AlarmLimit;
        }

        public string Update(string Usr)
        {
            try
            {
                if (!Limit_Min.HasValue)
                {
                    Limit_Min = -32767;
                }
                if (!Limit_Max.HasValue)
                {
                    Limit_Max = 32766;
                }
                DBConnector.executeSQL("Intouch", "EXEC [dbo].[uSP_Change_Limit] @FullTagName='" + FullTagName + "',@Limit_Max=" + Limit_Max + ",@IsHiEnable=" + IsHiEnable + ",@Limit_Min=" + Limit_Min + ",@IsLoEnable=" + IsLoEnable + ",@login_name='" + Usr + "',@delayTime=" + DelayTime + ",@delayTimeEnable=1,@FDC_Tag='" + FDC_Tag + "'");
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return null;
        }

        internal static IEnumerable<AlarmLimitModel> GetLimitHistory(string tagname)
        {
            IEnumerable<AlarmLimitModel> AlarmLimitHistory;
            //DataTable dt = GetAlarmListdt(Tool);

            string sqlStr = "SELECT TagName,builddate,FDC_Tag,login_name,Limit_Max_Setting,Limit_Max,Limit_Min_Setting,Limit_Min,IsHiHiEnable,IsHiEnable,IsLoEnable,IsLoLoEnable,delayTime FROM Limit_history where TagName='" + tagname + "' order by builddate desc";

            DataSet DeptDS = DBConnector.executeQuery("Intouch", sqlStr);

            AlarmLimitHistory = (from dept in DeptDS.Tables[0].AsEnumerable()
                                 select new AlarmLimitModel
                                 {
                                     //department_name = dept.Field<string>("department_name"),
                                     FullTagName = dept.Field<string>("TagName"),
                                     FDC_Tag = dept.Field<string>("FDC_Tag"),
                                     Limit_Max_Setting = dept.Field<double?>("Limit_Max_Setting"),
                                     Limit_Max = dept.Field<double?>("Limit_Max"),
                                     //Limit_Max_Type = dept.Field<double?>("Limit_Max_Type"),
                                     IsHiEnable = dept.Field<bool?>("IsHiEnable"),
                                     Limit_Min_Setting = dept.Field<double?>("Limit_Min_Setting"),
                                     Limit_Min = dept.Field<double?>("Limit_Min"),
                                     //Limit_Min_Type = dept.Field<double?>("Limit_Min_Type"),
                                     IsLoEnable = dept.Field<bool?>("IsLoEnable"),
                                     DelayTime = dept.Field<int?>("delayTime").HasValue ? dept.Field<int?>("delayTime").Value : 0,
                                     login_name = dept.Field<string>("login_name"),
                                     builddate = dept.Field<DateTime>("builddate").ToString("yyyy/MM/dd HH:mm:ss")
                                 }).ToList();

            return AlarmLimitHistory;
        }

        internal static byte[] GetAlarmLimitFile(string tool)
        {
            IEnumerable<AlarmLimitModel> al = GetToolLimitList(tool);

            Dictionary<string, int> dicDepCount = new Dictionary<string, int>();

            ExcelPackage ep = new ExcelPackage();

            //ep.Workbook.Worksheets.Add("AlarmLimit");
            ExcelWorksheet sheet1 ;

            if (al.Count() > 0)
            {
               
                foreach (var item in al)
                {

                    if (!ep.Workbook.Worksheets.Any(x => x.Name == item.department_name))
                    {
                        dicDepCount.Add(item.department_name, 2);
                        ep.Workbook.Worksheets.Add(item.department_name);
                        sheet1 = ep.Workbook.Worksheets[item.department_name];

                        sheet1.Cells["A1"].Value = "FullTagName";
                        sheet1.Cells["B1"].Value = WebCMS.Menu.department_name;
                        sheet1.Cells["C1"].Value = "TagName";
                        sheet1.Cells["D1"].Value = "Toolid";
                        sheet1.Cells["E1"].Value = "AccessoryName";
                        sheet1.Cells["F1"].Value = WebCMS.Menu.typeName;
                        sheet1.Cells["G1"].Value = WebCMS.Menu.Limit_Max_Setting;
                        sheet1.Cells["H1"].Value = WebCMS.Menu.Limit_Min_Setting;
                        sheet1.Cells["I1"].Value = WebCMS.Menu.LastEditUser;
                    }

                    sheet1 = ep.Workbook.Worksheets[item.department_name];

                    sheet1.Cells["A" + dicDepCount[item.department_name]].Value = item.FullTagName;
                    sheet1.Cells["B" + dicDepCount[item.department_name]].Value = item.department_name;
                    sheet1.Cells["C" + dicDepCount[item.department_name]].Value = item.FDC_Tag;
                    sheet1.Cells["D" + dicDepCount[item.department_name]].Value = item.Toolid;
                    sheet1.Cells["E" + dicDepCount[item.department_name]].Value = item.ChamberName;
                    sheet1.Cells["F" + dicDepCount[item.department_name]].Value = item.type_name;
                    sheet1.Cells["G" + dicDepCount[item.department_name]].Value = item.Limit_Max_Setting;
                    sheet1.Cells["H" + dicDepCount[item.department_name]].Value = item.Limit_Min_Setting;
                    sheet1.Cells["I" + dicDepCount[item.department_name]].Value = item.login_name;
                    dicDepCount[item.department_name]++;
                }

                foreach (var item in ep.Workbook.Worksheets)
                {
                    item.Cells.AutoFitColumns();
                    item.Column(1).Width = 0;
                }
            }

            return ep.GetAsByteArray();
        }

        internal string UpdateAll(string Usr,string tagname)
        {
            foreach (string item in tagname.Split(','))
            {
                try
                {
                    if (!Limit_Min.HasValue)
                    {
                        Limit_Min = -32767;
                    }
                    if (!Limit_Max.HasValue)
                    {
                        Limit_Max = 32766;
                    }
                    DBConnector.executeSQL("Intouch", "EXEC [dbo].[uSP_Change_Limit] @FullTagName='" + item + "',@Limit_Max=" + Limit_Max + ",@IsHiEnable=" + IsHiEnable + ",@Limit_Min=" + Limit_Min + ",@IsLoEnable=" + IsLoEnable + ",@login_name='" + Usr + "',@delayTime=" + DelayTime + ",@delayTimeEnable=1,@FDC_Tag='" + FDC_Tag + "'");
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            
            return null;
        }
    }
}