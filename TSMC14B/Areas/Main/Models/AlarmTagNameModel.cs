using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using TSMC14B.Models;

namespace TSMC14B.Areas.Main.Models
{
    public class AlarmTagNameModel
    {
        #region 組織成員屬性
        [Display(Name = "SeqNO")]
        public int SeqNO { get; set; }

        [Display(Name = "Alarm訊息")]
        public String Comment { get; set; }

        [Display(Name = "Type ID")]
        public Int16 Type_id { get; set; }

        [Display(Name = "Tag 編號")]
        public String Tag_Name { get; set; }

        [Display(Name = "Tag 名稱")]
        public String Cht_Comment { get; set; }

        [Display(Name = "Type")]
        public string Typeid { get; set; }
        public string TName { get; set; }

        [Display(Name = "Tag")]
        public string TagValue { get; set; }
        public string TagText { get; set; }

        [Display(Name = "Alarm")]
        public string AlarmValue { get; set; }
        public string AlarmText { get; set; }

        [Display(Name = "修改 Tag 名稱")]
        public String TagFullName { get; set; }

        [Display(Name = "Alarm 名稱")]
        public String AlarmFullName { get; set; }

        [Display(Name = "Type")]
        public String TypeName { get; set; }

        #endregion

        public static IEnumerable<AlarmTagNameModel> EQAlarmTagName(string vName)
        {
            //DataSet ds = DBConnector.executeQuery("Intouch", " SELECT A.*,AC.Cht_Comment FROM AlarmTagName A LEFT JOIN analog_comment AC ON A.Type_id = AC.Type_id AND A.Tag_Name = AC.Tag_Name WHERE A.Type_id = " + vid + " ORDER BY Comment");
            DataSet ds = DBConnector.executeQuery("Intouch", " SELECT A.*,AC.Cht_Comment FROM AlarmTagName A LEFT JOIN analog_comment AC ON A.Type_id = AC.Type_id AND A.Tag_Name = AC.Tag_Name WHERE A.Type_id IN (SELECT distinct(typeid) FROM vw_eq_status where vName = '" + vName + "')");
            return from dept in ds.Tables[0].AsEnumerable()
                   select new AlarmTagNameModel
                   {
                       SeqNO = dept.Field<int>("SeqNO"),
                       Comment = dept.IsNull("Comment") ? string.Empty : dept.Field<String>("Comment").ToString(),
                       Type_id = dept.Field<Int16>("Type_id"),
                       Tag_Name = dept.IsNull("Tag_Name") ? string.Empty : dept.Field<String>("Tag_Name").ToString(),
                       Cht_Comment = dept.IsNull("Cht_Comment") ? string.Empty : dept.Field<String>("Cht_Comment").ToString(),
                       TagFullName = dept.Field<Int16>("Type_id").ToString() + "," + dept.Field<String>("Tag_Name").ToString(),
                   };
        }

        //public static IEnumerable<LoginModel> SessionLevel()
        //{
        //    DataSet ds = DBConnector.executeQuery("Intouch", "SELECT * FROM analog_comment");

        //    return from dept in ds.Tables[0].AsEnumerable()
        //           orderby dept.Field<Byte>("level_id")
        //           select new LoginModel
        //           {
        //               Level = dept.Field<string>("level_name").ToString(),
        //               Levelid = dept.Field<Byte>("level_id"),
        //           };
        //}

        public static AlarmTagNameModel Get(int SeqNO)
        {
            DataSet ds = DBConnector.executeQuery("Intouch", "SELECT A.*,AC.Cht_Comment FROM AlarmTagName A LEFT JOIN analog_comment AC ON A.Type_id = AC.Type_id AND A.Tag_Name = AC.Tag_Name WHERE SeqNO = " + SeqNO.ToString());

            if (ds.Tables[0].Rows.Count == 0)
                return null;

            return new AlarmTagNameModel
            {
                SeqNO = ds.Tables[0].Rows[0].Field<int>("SeqNO"),
                Comment = ds.Tables[0].Rows[0].Field<string>("Comment").ToString(),
                Type_id = ds.Tables[0].Rows[0].Field<Int16>("Type_id"),
                Tag_Name = ds.Tables[0].Rows[0].Field<string>("Tag_Name").ToString(),
                Cht_Comment = ds.Tables[0].Rows[0].Field<string>("Cht_Comment").ToString(),
                TagFullName = ds.Tables[0].Rows[0].Field<Int16>("Type_id") + "," + ds.Tables[0].Rows[0].Field<string>("Tag_Name").ToString(),
            };
        }
        

        public static IEnumerable<AlarmTagNameModel> TypeList(string vName)
        {
            //DataSet ds = DBConnector.executeQuery("Intouch", "SELECT CAST(type_id AS varchar(20)) + ',' + Tag_Name TValue, Cht_Comment + '-' + Tag_Name TText FROM analog_comment WHERE type_id = "+ vid +" ORDER BY Cht_Comment");
            //DataSet ds = DBConnector.executeQuery("Intouch", "SELECT DISTINCT(CAST(type_id AS varchar(20)) + ',' + Tag_Name) TValue, Cht_Comment + '-' + Tag_Name TText, Cht_Comment FROM analog_comment ac LEFT JOIN vw_eq_status E ON ac.type_id= E.typeid WHERE E.vName = '" + vName + "' ORDER BY Cht_Comment");
            DataSet ds = DBConnector.executeQuery("Intouch", "SELECT CONVERT(varchar,typeid) typeid,tName FROM vw_eq_status  WHERE vName = '" + vName + "' GROUP BY typeid,tName ORDER BY typeid");

            return from dept in ds.Tables[0].AsEnumerable()
                   select new AlarmTagNameModel
                   {
                       Typeid = dept.Field<string>("typeid").ToString(),
                       TName = dept.Field<string>("tName").ToString(),
                   };
        }

        public static IEnumerable<AlarmTagNameModel> AlarmList(string vName, string typeid)
        {
            //DataSet ds = DBConnector.executeQuery("Intouch", "SELECT DISTINCT(Comment) FROM [tsmc14_Alarm].[dbo].[Comment] ORDER BY Comment");
            DataSet ds = DBConnector.executeQuery("Intouch", "EXEC uSP_Select_Comment '" + vName + "','" + typeid + "'");

            return from dept in ds.Tables[0].AsEnumerable()
                   select new AlarmTagNameModel
                   {
                       AlarmValue = dept.IsNull("Comment") ? string.Empty : dept.Field<string>("Comment").ToString(),
                       AlarmText = dept.IsNull("Comment") ? string.Empty : dept.Field<string>("Comment").ToString(),
                   };
        }

        public static IEnumerable<AlarmTagNameModel> TagList(string vName, string typeid)
        {
            //DataSet ds = DBConnector.executeQuery("Intouch", "SELECT CAST(type_id AS varchar(20)) + ',' + Tag_Name TValue, Cht_Comment + '-' + Tag_Name TText FROM analog_comment WHERE type_id = "+ vid +" ORDER BY Cht_Comment");
            //DataSet ds = DBConnector.executeQuery("Intouch", "SELECT DISTINCT(CAST(type_id AS varchar(20)) + ',' + Tag_Name) TValue, Cht_Comment + '-' + Tag_Name TText, Cht_Comment FROM analog_comment ac LEFT JOIN vw_eq_status E ON ac.type_id= E.typeid WHERE E.vName = '" + vName + "' ORDER BY Cht_Comment");
            DataSet ds = DBConnector.executeQuery("Intouch", "EXEC uSP_Select_AlarmTagName '" + vName + "','" + typeid + "'" );

            return from dept in ds.Tables[0].AsEnumerable()
                   select new AlarmTagNameModel
                       {
                           TagValue = dept.Field<string>("TValue").ToString(),
                           TagText = dept.Field<string>("TText").ToString(),
                       };
        }

        //新增
        public string Insert(string Comment, string type_id, string Tag_Name)
        {
            string DBMsg = null;
            try
            {
                DBConnector.executeSQL("Intouch", "EXEC uSP_Change_AlarmTagName @action = 1, @Comment = '" + Comment + "', @type_id = '" + @type_id + "', @Tag_Name = '" + Tag_Name + "'");
                DBMsg = "Tag: " + Tag_Name + " 新增成功";
            }
            catch (Exception ex)
            {
                DBMsg += ex.Message;
            }
            return DBMsg;
        }

        //刪除
        public string Delete(int SeqNO)
        {
            string DBMsg = null;
            try
            {
                DBConnector.executeSQL("Intouch", "EXEC uSP_Change_AlarmTagName @action = 0, @SeqNO = " + SeqNO );
                DBMsg = "刪除成功";
            }
            catch (Exception ex)
            {
                DBMsg += ex.Message;
            }
            return DBMsg;
        }

        public string Update()
        {
            string DBMsg = null;
            try
            {
                DBConnector.executeSQL("Intouch", "EXEC uSP_Change_AlarmTagName @action = 2, @SeqNO = " + SeqNO + ", @Comment = '" + Comment + "', @Type_id = " + TagFullName.Split(',')[0]  + ", @Tag_Name = '" + TagFullName.Split(',')[1] + "'");
                DBMsg = "修改成功";
            }
            catch (Exception ex)
            {
                DBMsg += ex.Message;
            }
            return DBMsg;
        }
    }
}