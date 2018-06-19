using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using WebCMS.Models;

namespace WebCMS.Models
{
    public class MasterPageModel
    {
        #region 組織成員屬性
        [Display(Name = "廠商名稱：")]
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

        #endregion

        //首頁所有vendor的機台狀態數
        public static IEnumerable<MasterPageModel> IndexAllStatusList()
        {
            DataSet DeptDS = DBConnector.executeQuery("Intouch", "EXEC [dbo].[uSP_Select_EQ_StatusCount]");

            return from dept in DeptDS.Tables[0].AsEnumerable()
                   select new MasterPageModel
                   {
                       vName = dept.Field<string>("vName"),
                       eqCount = dept.Field<Int32>("eqCount"),
                       normalCount = dept.Field<Int32>("normalCount"),
                       troubleCount = dept.Field<Int32>("troubleCount"),
                       pmCount = dept.Field<Int32>("pmCount"),
                       otherCount = dept.Field<Int32>("otherCount")
                   };
        }

        //個別廠商其他狀態數資訊
        public static IEnumerable<MasterPageModel> IndexOtherStatusList()
        {


            return null;
        }
    }
}