using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using WebCMS.Models;

namespace WebCMS.Areas.Main.Models
{
    public class SingleAlarmModel
    {
        #region 組織成員屬性
        [Display(Name = "ToolID")]
        public string ToolID { get; set; }

        [Display(Name = "課別")]
        public string GroupName { get; set; }

        [Display(Name = "柱位")]
        public string Location { get; set; }

        [Display(Name = "LocationID")]
        public string LocationID { get; set; }

        [Display(Name = "錯誤代碼")]
        public string AlarmType { get; set; }

        [Display(Name = "錯誤描述")]
        public string AlarmMsg { get; set; }

        [Display(Name = "Alarmlevel")]
        public Int16 Alarmlevel { get; set; }

        [Display(Name = "發生時間")]
        public string AlarmTime_Begin { get; set; }
        #endregion

        public static IEnumerable<SingleAlarmModel> GetSingleAlarm(string Toolid)
        {

            DateTime _fromdate = Convert.ToDateTime("1999-01-01");
            DateTime _toDate = Convert.ToDateTime("2999-12-31");
            string _toolid = "NULL";

            if (!string.IsNullOrEmpty(Toolid) && Toolid.ToUpper() != "NULL" && Toolid.Length > 0)
            {
                _toolid = "'" + Toolid + "'";
            }

            DataSet DeptDS = DBConnector.executeQuery("Intouch", "EXEC [dbo].[uSP_Select_AlarmHistory] N'" + _fromdate.ToString("yyyy-MM-dd HH:mm") + "',N'" + _toDate.ToString("yyyy-MM-dd HH:mm") + "'," + _toolid + ",0");

            return (from dept in DeptDS.Tables[0].AsEnumerable()

                    select new SingleAlarmModel
                    {
                        ToolID = dept.IsNull("toolID") ? string.Empty : dept.Field<string>("toolID"),
                        GroupName = dept.IsNull("Group_name") ? string.Empty : dept.Field<string>("Group_name"),
                        Location = dept.IsNull("location") ? string.Empty : dept.Field<string>("location"),
                        LocationID = dept.IsNull("Location_id") ? string.Empty : dept.Field<string>("Location_id"),
                        AlarmType = dept.IsNull("AlarmType") ? string.Empty : dept.Field<string>("AlarmType"),
                        AlarmMsg = dept.IsNull("AlarmMsg") ? string.Empty : dept.Field<string>("AlarmMsg"),
                        Alarmlevel = dept.IsNull("AlarmLevel") ? (short)500 : dept.Field<Int16>("AlarmLevel"),
                        AlarmTime_Begin = dept.IsNull("AlarmTime_Begin") ? string.Empty : dept.Field<DateTime>("AlarmTime_Begin").ToString("yyyy-MM-dd HH:mm:ss")
                    }).ToList();
        }
    }
}