using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using TSMC14B.Models;

namespace TSMC14B.Areas.Main.Models
{
    public class AlarmLimitModel
    {
        #region 組織成員屬性
        [Display(Name = "department_name",ResourceType=typeof(Menu))]
        public string department_name { get; set; }

        [Display(Name = "FullTagName")]
        public string FullTagName { get; set; }

        [Display(Name = "FDC_Tag")]
        public string FDC_Tag { get; set; }

        [Display(Name = "Limit_Max_Setting" , ResourceType = typeof(Menu)),Range(-32767,32766)]
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
        [Range(0,100)]
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

        public static DataTable GetAlarmListdt(string Tool)
        {
            string sqlStr = "SELECT department_name,FullTagName,FDC_Tag,Limit_Max_Setting,Limit_Max_Type,Limit_Max,IsHiEnable,Limit_Min_Setting,Limit_Min_Type,Limit_Min,IsLoEnable,delayTime,login_name FROM vw_eq_limit where fulltagName like '%[_]" + Tool + "[_]%'";

            DataSet DeptDS = DBConnector.executeQuery("Intouch", sqlStr);

            return DeptDS.Tables[0];
        }

        public static IEnumerable<AlarmLimitModel> GetLimitList(string Tool)
        {
            List<AlarmLimitModel> AlarmLimitList = new List<AlarmLimitModel>();
            DataTable dt = GetAlarmListdt(Tool);

            AlarmLimitList = (from dept in dt.AsEnumerable()
                              select new AlarmLimitModel
                              {
                                  department_name = dept.Field<string>("department_name").ToString(),
                                  FullTagName = dept.Field<string>("FullTagName").ToString(),
                                  FDC_Tag = dept.Field<string>("FDC_Tag").ToString(),
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
            //DataTable dt = GetAlarmListdt(Tool);

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
            IEnumerable<AlarmLimitModel> AlarmLimitHistory ;
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
                              DelayTime = dept.Field<int?>("delayTime").HasValue? dept.Field<int?>("delayTime").Value:0,
                              login_name = dept.Field<string>("login_name"),
                              builddate = dept.Field<DateTime>("builddate").ToString("yyyy/MM/dd HH:mm:ss")
                          }).ToList();

            return AlarmLimitHistory;
        }

        internal static byte[] GetAlarmLimitFile(string tool)
        {
            IEnumerable<AlarmLimitModel> al = GetLimitList(tool);

            ExcelPackage ep = new ExcelPackage();
            ep.Workbook.Worksheets.Add("MySheet");
            ExcelWorksheet sheet1 = ep.Workbook.Worksheets["MySheet"];

            sheet1.Cells["A1"].Value = "FullTagName";
            sheet1.Cells["B1"].Value = "部門";
            sheet1.Cells["C1"].Value = "TagName";
            sheet1.Cells["D1"].Value = "目前上限設定";
            sheet1.Cells["E1"].Value = "手動上限值";
            sheet1.Cells["F1"].Value = "手動上限";
            sheet1.Cells["G1"].Value = "目前下限設定";
            sheet1.Cells["H1"].Value = "手動下限值";
            sheet1.Cells["I1"].Value = "手動下限";
            sheet1.Cells["J1"].Value = "DelayTime(sec)";
            sheet1.Cells["K1"].Value = "最後修改者";

            if (al.Count() > 0)
            {
                int i = 2;
                foreach (var item in al)
                {
                    sheet1.Cells["A" + i].Value = item.FullTagName;
                    sheet1.Cells["B" + i].Value = item.department_name;
                    sheet1.Cells["C" + i].Value = item.FDC_Tag;
                    sheet1.Cells["D" + i].Value = item.Limit_Max_Setting;
                    sheet1.Cells["E" + i].Value = item.Limit_Max;
                    sheet1.Cells["F" + i].Value = item.IsHiEnable;
                    sheet1.Cells["G" + i].Value = item.Limit_Min_Setting;
                    sheet1.Cells["H" + i].Value = item.Limit_Min;
                    sheet1.Cells["I" + i].Value = item.IsLoEnable;
                    sheet1.Cells["J" + i].Value = item.DelayTime;
                    sheet1.Cells["K" + i].Value = item.login_name;
                    i++;
                }

                sheet1.Cells.AutoFitColumns();
                sheet1.Column(1).Width = 0;
            }

            return ep.GetAsByteArray();
        }
    }
}