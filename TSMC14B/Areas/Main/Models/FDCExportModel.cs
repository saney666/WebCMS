using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using TSMC14B.Models;
using OfficeOpenXml;

namespace TSMC14B.Areas.Main.Models
{
    public class FDCExportModel
    {
        #region 組織成員屬性
        [Display(Name = "Channel")]
        public string Channel { get; set; }

        [Display(Name = "Device")]
        public string Device { get; set; }

        [Display(Name = "ToolID")]
        public string ToolID { get; set; }

        [Display(Name = "Chamber")]
        public string Chamber { get; set; }

        [Display(Name = "SVID")]
        public string SVID { get; set; }
        #endregion

        internal static byte[] GetFDCExportFile()
        {
            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                var al = (from row in db.vw_FDC_KEP_Tag select row).ToList();

                ExcelPackage ep = new ExcelPackage();
                ep.Workbook.Worksheets.Add("MySheet");
                ExcelWorksheet sheet1 = ep.Workbook.Worksheets["MySheet"];

                sheet1.Cells["A1"].Value = "Channel";
                sheet1.Cells["B1"].Value = "Device";
                sheet1.Cells["C1"].Value = "ToolID";
                sheet1.Cells["D1"].Value = "Chamber";
                sheet1.Cells["E1"].Value = "SVID";

                if (al.Count() > 0)
                {
                    int i = 2;
                    foreach (var item in al)
                    {
                        sheet1.Cells["A" + i].Value = item.channel_name.Split('.')[0];
                        sheet1.Cells["B" + i].Value = item.channel_name.Split('.')[1];
                        sheet1.Cells["C" + i].Value = item.toolid;
                        sheet1.Cells["D" + i].Value = item.ChamberID;
                        sheet1.Cells["E" + i].Value = item.tagname;
                        i++;
                    }
                    sheet1.Cells.AutoFitColumns();
                }
                return ep.GetAsByteArray();
            }
        }
    }
}