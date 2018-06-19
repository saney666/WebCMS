using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using WebCMS.Models;
using OfficeOpenXml;
using System.Text;

namespace WebCMS.Areas.Main.Models
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


        internal static byte[] GetFDCExportFile(string type)
        {
            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                var al = (from row in db.vw_FDC_KEP_Tag select row).ToList();

                if (type=="HJ")
                {
                    al = (from row in al where row.tagname.Substring(0, 4) == "1201" select row).ToList();
                }
                else if (type=="Edward")
                {
                    al = (from row in al where row.tagname.Substring(0, 4) == "1200" select row).ToList();
                }
                else if (type == "HN2")
                {
                    al = (from row in al where row.tagname.Substring(0, 4) == "1202" select row).ToList();
                }
                else if (type == "ExtraSensor")
                {
                    al = (from row in al where row.tagname.Substring(0, 4) == "1203" select row).ToList();
                }
                else if (type == "PFEIFFER")
                {
                    al = (from row in al where row.tagname.Substring(0, 4) == "1204" select row).ToList();
                }

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

        internal static byte[] GetFDCDeviceExportFile(string channel_name)
        {
            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                var r = from row in db.vw_FDC_KEP_Tag_load where row.channel_name == channel_name orderby row.toolid, row.ChamberID select row;

                StringBuilder sb = new StringBuilder();

                string csvTitle = "Tag Name,Address,Data Type,Respect Data Type,Client Access,Scan Rate,Scaling,Raw Low,Raw High,Scaled Low,Scaled High,Scaled Data Type,Clamp Low,Clamp High,Eng Units,Description,Negate Value";

                sb.AppendLine(csvTitle);

                foreach (vw_FDC_KEP_Tag_load item in r)
                {
                    string csvItem = "\"" + item.toolid + "." + item.ChamberID + "." + item.tagname + "\",\"" + item.KEP_address + "\"," + item.KepType + ",1," + item.KepAccess + ",100," + (item.Raw_High.HasValue ? "Linear" : "") + "," + (item.Raw_High.HasValue ? item.Raw_Low.Value.ToString() : "") + "," + (item.Raw_High.HasValue ? item.Raw_High.Value.ToString() : "") + "," + (item.Raw_High.HasValue ? item.Scaled_Low.Value.ToString() : "") + "," + (item.Raw_High.HasValue ? item.Scaled_High.Value.ToString() : "") + "," + (item.Raw_High.HasValue ? "Double" : "") + "," + (item.Raw_High.HasValue ? "0" : "") + "," + (item.Raw_High.HasValue ? "0" : "") + ",\"\",\"" + item.Eng_Comment + "\"," + (item.Raw_High.HasValue ? "0" : "");
                    sb.AppendLine(csvItem);
                }
                
                return Encoding.ASCII.GetBytes(sb.ToString());
            }
        }

        internal static List<string> GetAllDevice()
        {
            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                return (from row in db.vw_FDC_KEP_Tag_load orderby row.built_date descending select row.channel_name).Distinct().ToList();
            }
        }
    }
}