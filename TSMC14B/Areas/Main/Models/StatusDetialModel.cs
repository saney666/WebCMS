using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WebCMS.Areas.Main.Models
{
    public class StatusDetialModel
    {
        #region 組織成員屬性
        [Display(Name = "設備狀態")]
        public Byte Statusid { get; set; }

        [Display(Name = "課別名稱")]
        public string gName { get; set; }

        [Display(Name = "ToolID")]
        public string ToolID { get; set; }

        [Display(Name = "柱位")]
        public string location { get; set; }

        [Display(Name = "LocationID")]
        public string LocationID { get; set; }

        [Display(Name = "左右邊")]
        public string LocationSide { get; set; }

        [Display(Name = "設備型號")]
        public string TypeName { get; set; }

        [Display(Name = "設定日期")]
        public string UpdateDate { get; set; }

        [Display(Name = "操作者")]
        public string Op { get; set; }

        #endregion

        public static IEnumerable<StatusDetialModel> GetStatusDetial(string vName, string GroupName)
        {
            List<StatusDetialModel> StatusDetialList = new List<StatusDetialModel>();
            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                
                if (GroupName.Length > 0)
                {
                    
                    StatusDetialList = (from row in db.vw_eq_status
                                        where row.vName == vName && row.groupName == GroupName
                                        select new StatusDetialModel
                                        {
                                            Statusid = row.sid,
                                            gName = row.groupName == null ? string.Empty : row.groupName,
                                            ToolID = row.toolid,
                                            location = row.loc == null ? string.Empty : row.loc,
                                            //LocationID = row.lid,
                                            //LocationSide = row.locSide == null ? string.Empty : row.locSide,
                                            TypeName = row.tName
                                        }).ToList();
                }
                else
                {
                    StatusDetialList = (from row in db.vw_eq_status
                                        where row.vName == vName && row.groupName == null
                                        select new StatusDetialModel
                                        {
                                            Statusid = row.sid,
                                            gName = row.groupName == null ? string.Empty : row.groupName,
                                            ToolID = row.toolid,
                                            location = row.loc == null ? string.Empty : row.loc,
                                            //LocationID = row.lid,
                                            //LocationSide = row.locSide == null ? string.Empty : row.locSide,
                                            TypeName = row.tName
                                        }).ToList();
                }
            }
            return StatusDetialList;
        }

        public static IEnumerable<StatusDetialModel> GetStatusDetial(string vName, string GroupName, string sid, string toolId)
        {
            List<StatusDetialModel> StatusDetialList = new List<StatusDetialModel>();
            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                if (GroupName.Length > 0)
                {
                    StatusDetialList = (from row in db.vw_eq_status
                                        where row.vName == vName && row.groupName == GroupName
                                        select new StatusDetialModel
                                        {
                                            Statusid = row.sid,
                                            gName = row.groupName == null ? string.Empty : row.groupName,
                                            ToolID = row.toolid,
                                            location = row.loc == null ? string.Empty : row.loc,
                                            //LocationID = row.lid,
                                            //LocationSide = row.locSide == null ? string.Empty : row.locSide,
                                            TypeName = row.tName
                                        }).ToList();
                }
                else
                {
                    StatusDetialList = (from row in db.vw_eq_status
                                        where row.vName == vName && row.groupName == null
                                        select new StatusDetialModel
                                        {
                                            Statusid = row.sid,
                                            gName = row.groupName == null ? string.Empty : row.groupName,
                                            ToolID = row.toolid,
                                            location = row.loc == null ? string.Empty : row.loc,
                                            //LocationID = row.lid,
                                            //LocationSide = row.locSide == null ? string.Empty : row.locSide,
                                            TypeName = row.tName
                                        }).ToList();
                }
                if (sid.Length > 0)
                {
                    StatusDetialList = (from row in StatusDetialList
                                        where row.Statusid.ToString() == sid
                                        select row).ToList();
                }
                if (toolId.Length > 0)
                {
                    StatusDetialList = (from row in StatusDetialList
                                        where row.ToolID.ToUpper().Contains(toolId.ToUpper())
                                        select row).ToList();
                }
            }
            return StatusDetialList;
        }

        //for Index pass VendorName & Status
        public static IEnumerable<StatusDetialModel> StatusDetial(string vName, string status)
        {
            List<StatusDetialModel> StatusDetialList = new List<StatusDetialModel>();
            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                if (status.Equals("others"))
                {
                    StatusDetialList = (from row in db.vw_eq_status
                                        where row.vName == vName && !(new int?[] { 0, 1, 2, 8, 20 }).Contains(row.sid)
                                        select new StatusDetialModel
                                        {
                                            Statusid = row.sid,
                                            gName = row.groupName == null ? string.Empty : row.groupName,
                                            ToolID = row.toolid,
                                            location = row.loc == null ? string.Empty : row.loc,
                                            //LocationID = row.lid,
                                            //LocationSide = row.locSide == null ? string.Empty : row.locSide,
                                            TypeName = row.tName
                                        }).ToList();
                }
                //service數
                else if (status.Equals("Service"))
                {
                    StatusDetialList = (from row in db.vw_eq_status
                                        where row.vName == vName && row.sid == 20
                                        select new StatusDetialModel
                                        {
                                            Statusid = row.sid,
                                            gName = row.groupName == null ? string.Empty : row.groupName,
                                            ToolID = row.toolid,
                                            location = row.loc == null ? string.Empty : row.loc,
                                            //LocationID = row.lid,
                                            //LocationSide = row.locSide == null ? string.Empty : row.locSide,
                                            TypeName = row.tName
                                        }).ToList();
                }
                //PM數
                else if (status.Equals("PM"))
                {
                    StatusDetialList = (from row in db.vw_eq_status
                                        join roww in db.vw_pm_history on row.pid equals roww.plc_id
                                        where row.vName == vName && row.sid == 8 && roww.pm_set != null && roww.pm_unset == null
                                        select new StatusDetialModel
                                        {
                                            Statusid = row.sid,
                                            gName = row.groupName == null ? string.Empty : row.groupName,
                                            ToolID = row.toolid,
                                            location = row.loc == null ? string.Empty : row.loc,
                                            //LocationID = row.lid ,
                                            //LocationSide = row.locSide == null ? string.Empty : row.locSide,
                                            TypeName = row.tName,
                                            UpdateDate = string.Format("{0:yyyy-MM-dd HH:mm:ss}", roww.pm_set),
                                            Op = roww.@operator
                                        }).ToList();
                }
            }
            return StatusDetialList;
        }
    }
}