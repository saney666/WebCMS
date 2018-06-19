using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using WebCMS.Models;

namespace WebCMS.Areas.Main.Models
{
    public class StatusCountModel
    {
        #region 組織成員屬性
        [Display(Name = "部門名稱：")]
        public string vName { get; set; }

        [Display(Name = "總共設備數：")]
        public Int32 eqCount { get; set; }

        [Display(Name = "正常設備數：")]
        public Int32 normalCount { get; set; }

        [Display(Name = "警告/異常數：")]
        public Int32 troubleCount { get; set; }

        [Display(Name = "PM隔離：")]
        public Int32 pmCount { get; set; }

        [Display(Name = "其他狀態數：")]
        public Int32 otherCount { get; set; }

        [Display(Name = "Service數：")]
        public Int32 serviceCount { get; set; }
        #endregion

        //首頁所有vendor的機台狀態數
        public static IEnumerable<StatusCountModel> IndexAllStatusList()
        {
            DataSet DeptDS = DBConnector.executeQuery("Intouch", "EXEC [dbo].[uSP_Select_EQ_StatusCount]");

            return from dept in DeptDS.Tables[0].AsEnumerable()
                   select new StatusCountModel
                   {
                       vName = dept.Field<string>("vName"),
                       eqCount = dept.IsNull("eqCount") ? (Int32)0 : dept.Field<Int32>("eqCount"),
                       normalCount = dept.IsNull("normalCount") ? (Int32)0 : dept.Field<Int32>("normalCount"),
                       troubleCount = dept.IsNull("troubleCount") ? (Int32)0 : dept.Field<Int32>("troubleCount"),
                       pmCount = dept.IsNull("pmCount") ? (Int32)0 : dept.Field<Int32>("pmCount"),
                       otherCount = dept.IsNull("otherCount") ? (Int32)0 : dept.Field<Int32>("otherCount") + (dept.IsNull("DCCount") ? (Int32)0 : dept.Field<Int32>("DCCount")),
                       serviceCount = dept.IsNull("serviceCount") ? (Int32)0 : dept.Field<Int32>("serviceCount")
                   };
        }

        //個別廠商其他狀態數資訊
        public static IEnumerable<StatusCountModel> toolIDList(string vName)
        {
            return null;
        }
    }
}