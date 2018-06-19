using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Data.Linq;
using System.Web.Configuration;
using WebCMS.Models;
using OfficeOpenXml;
namespace WebCMS.Areas.Main.Models
{
    public class FDCPMModel
    {
        #region 組織成員屬性
        [Display(Name = "pid")]
        public int pid { get; set; }

        [Display(Name = "department_name", ResourceType = typeof(Menu))]
        public string department_name { get; set; }

        [Display(Name = "chamberName")]
        public string chamberName { get; set; }

        [Display(Name = "FDCPMTime")]
        public string FDCPMTime { get; set; }

        [Display(Name = "SetTime")]
        public string builtdate { get; set; }

        [Display(Name = "FDCPMStatus")]
        public string FDCPMStatus { get; set; }

        [Display(Name = "login_name")]
        public string login_name { get; set; }

        [Display(Name = "memo")]
        public string memo { get; set; }
        #endregion

        internal static byte[] GetFDCPMFile(bool IsHistory, string dept, string StartDate, string EndDate, string chamberName)
        {
            IEnumerable<FDCPMModel> dt = GetFDCPMList(IsHistory, dept, StartDate, EndDate, chamberName);
            ExcelPackage ep = new ExcelPackage();
            ep.Workbook.Worksheets.Add("Sheet1");
            ExcelWorksheet sheet1 = ep.Workbook.Worksheets["Sheet1"];

            sheet1.Cells["A1"].Value = "pid";
            sheet1.Cells["B1"].Value = "部門";
            sheet1.Cells["C1"].Value = "設定時間";
            sheet1.Cells["D1"].Value = "ToolID";
            sheet1.Cells["E1"].Value = "延後/提前解除時間";
            sheet1.Cells["F1"].Value = "狀態";
            sheet1.Cells["G1"].Value = "操作人員";
            sheet1.Cells["H1"].Value = "原因";
           

            if (dt.Count() > 0)
            {
                int i = 2;
                foreach (var item in dt)
                {
                    sheet1.Cells["A" + i].Value = item.pid;
                    sheet1.Cells["B" + i].Value = item.department_name;
                    sheet1.Cells["C" + i].Value = item.builtdate;
                    sheet1.Cells["D" + i].Value = item.chamberName;
                    sheet1.Cells["E" + i].Value = item.FDCPMTime;
                    sheet1.Cells["F" + i].Value = item.FDCPMStatus;
                    sheet1.Cells["G" + i].Value = item.login_name;
                    sheet1.Cells["H" + i].Value = item.memo;
                    i++;
                }

                sheet1.Cells.AutoFitColumns();
                sheet1.Column(1).Width = 0;
            }

            return ep.GetAsByteArray();
        }
        internal static DataTable GetFDCPMdt(bool IsHistory, string dept, string StartDate, string EndDate, string chamberName)
        {
            if (string.IsNullOrEmpty(StartDate))
            {
                StartDate = "NULL";
            }
            else
            {
                StartDate = "'" + StartDate + "'";
            }
            if (string.IsNullOrEmpty(EndDate))
            {
                EndDate = "NULL";
            }
            else
            {
                EndDate = "'" + EndDate + "'";
            }

            DataTable dt = new DataTable();

            if (string.IsNullOrEmpty(chamberName))
            {
                chamberName = "NULL";
            }
            else
            {
                chamberName = "'" + chamberName + "'";
            }
            if (string.IsNullOrEmpty(dept))
            {
                dept = "NULL";
            }
            else
            {
                dept = "'" + dept + "'";
            }

            using (SqlConnection conn = new SqlConnection())
            {
                IDbCommand DBCmd = null;
                DBCmd = new SqlCommand("EXEC	uSP_Select_FDCPM @IsHistory = " + IsHistory + ",@dept = " + dept + ",@toolid = " + chamberName + ",@startDate = " + StartDate + ",@endDate = " + EndDate + "", new SqlConnection(WebConfigurationManager.ConnectionStrings["Intouch"].ConnectionString));
                DBCmd.CommandTimeout = 30000;
                DBCmd.Connection.Open();
                dt.Load(DBCmd.ExecuteReader());
            }

            return dt;
        }

        internal static IEnumerable<FDCPMModel> GetFDCPMList(bool IsHistory, string dept, string StartDate, string EndDate, string chamberName)
        {
            DataTable dt = GetFDCPMdt(IsHistory, dept, StartDate, EndDate, chamberName);
            
            return from row in dt.AsEnumerable()
                   select new FDCPMModel
                   {
                       department_name = row.IsNull("department_name") ? string.Empty : row.Field<string>("department_name"),
                       chamberName = row.IsNull("chamberName") ? string.Empty : row.Field<string>("chamberName"),
                       FDCPMStatus = row.Field<bool>("IsFDCPM") ? "PM" : "解除PM",
                       memo = row.Field<string>("memo"),
                       FDCPMTime = row.IsNull("FDCPMTime") ? string.Empty : row.Field<DateTime>("FDCPMTime").ToString("yyyy/MM/dd HH:mm:ss"),
                       pid =  row.Field<int>("pid"),
                       builtdate = row.IsNull("builtdate") ? string.Empty : row.Field<DateTime>("builtdate").ToString("yyyy/MM/dd HH:mm:ss"),
                       login_name = row.IsNull("login_name") ? string.Empty : row.Field<string>("login_name"),
                  
                   };
        }

    }
}