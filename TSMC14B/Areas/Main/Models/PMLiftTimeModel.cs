using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using TSMC14B.Models;

namespace TSMC14B.Areas.Main.Models
{
    public class PMLiftTimeModel
    {
        #region 組織成員屬性

        [Display(Name = "toolId")]
        public string toolId { get; set; }

        [Display(Name = "typeId")]
        public string typeId { get; set; }

        [Display(Name = "時間")]
        public string tName { get; set; }

        [Display(Name = "PMLiftTime")]
        public string PMLiftTime { get; set; }

        #endregion

        public static IEnumerable<PMLiftTimeModel> PMLiftTimeList(string Option, string Toolid, string typeid)
        {
            string _Toolid = "NULL";
            string _typeid = "NULL";

            if (!string.IsNullOrEmpty(Toolid))
            {
                _Toolid = "'" + Toolid + "'";
            }

            if (!string.IsNullOrEmpty(typeid) )
            {
                _typeid = typeid;
            }

            if (Option=="1")
            {
                _Toolid = "NULL";
            }
            else
            {
                _typeid = "NULL";
            }

            DataSet ds = DBConnector.executeQuery("Intouch", "EXEC [dbo].[uSP_Select_PMLifeTime] @toolID=" + _Toolid + ",@tid=" + _typeid);

            return from dept in ds.Tables[0].AsEnumerable()
                   select new PMLiftTimeModel
                   {
                       toolId = dept.IsNull("toolId") ? string.Empty : dept.Field<string>("toolid"),
                       typeId = dept.IsNull("typeId") ? string.Empty : dept.Field<Int16>("typeId").ToString(),
                       tName = dept.IsNull("tName") ? string.Empty : dept.Field<string>("tName"),
                       PMLiftTime = dept.IsNull("PM_LifeTime") ? string.Empty : dept.Field<Int16>("PM_LifeTime").ToString()
                   };
        }

        public static string EditPMLiftTime(string ToolId, string Typeid, string PmLT)
        {
            string DBMsg = null;

            string _ToolId = "NULL";
            string _Typeid = "NULL";
            string _tName = "NULL";
            if (!string.IsNullOrEmpty(ToolId) && ToolId!="0")
            {
                _ToolId = "'" + ToolId + "'";
            }

            if (!string.IsNullOrEmpty(Typeid) && Typeid!="0")
            {
                _Typeid = Typeid;
                _tName = ListModel.GettName(_Typeid);
            }

            try
            {
                DBConnector.executeSQL("Intouch", "EXEC [dbo].[uSP_Change_PMLifeTime] @action=1,@toolID=" + _ToolId + ",@tid=" + _Typeid + ",@pmLT=" + PmLT );

                if (!string.IsNullOrEmpty(ToolId))
                {
                    DBMsg += "設定 " + ToolId + " LiftTime " + PmLT + " 成功";
                }
                else
                {
                    DBMsg += "設定 " + _tName + "  LiftTime " + PmLT + " 成功";
                }
            }
            catch (Exception)
            {
                if (!string.IsNullOrEmpty(ToolId))
                {
                    DBMsg += "設定 " + ToolId + " LiftTime 失敗";
                }
                else
                {
                    DBMsg += "設定 " + _tName + "  LiftTime 失敗";
                }
            }

            return DBMsg;
        }

        public static string ResetPMLiftTime(string ToolId, string Typeid)
        {
            string DBMsg = null;

            string _ToolId = "NULL";
            string _Typeid = "NULL";
            string _tName = "NULL";
            if (!string.IsNullOrEmpty(ToolId) && ToolId!="0")
            {
                _ToolId = "'" + ToolId + "'";
            }

            if (!string.IsNullOrEmpty(Typeid) && Typeid!="0")
            {
                _Typeid = Typeid;
                _tName = ListModel.GettName(_Typeid);
            }

            try
            {
                DBConnector.executeSQL("Intouch", "EXEC [dbo].[uSP_Change_PMLifeTime] @action=0,@toolID=" + _ToolId + ",@tid=" + _Typeid + ",@pmLT=NULL");

                if (!string.IsNullOrEmpty(ToolId))
                {
                    DBMsg += "Reset " + ToolId + " LiftTime 成功";
                }
                else
                {
                    DBMsg += "Reset " + ListModel.GettName(Typeid) + " LiftTime 成功";
                }
            }
            catch (Exception)
            {
                if (!string.IsNullOrEmpty(ToolId))
                {
                    DBMsg += "Reset " + ToolId + " LiftTime 失敗";
                }
                else
                {
                    DBMsg += "Reset " + ListModel.GettName(Typeid) + " LiftTime 失敗";
                }
            }

            return DBMsg;
        }
    }
}