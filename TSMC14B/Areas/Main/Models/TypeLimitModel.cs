using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using System.Data;
using TSMC14B.Models;

namespace TSMC14B.Areas.Main.Models
{
    public class TypeLimitModel
    {
        [Display(Name = "type_id")]
        public short type_id { get; set; }

        [Display(Name = "Vendor Name")]
        public string vendor_name { get; set; }

        [Display(Name = "Type Name")]
        public string type_name { get; set; }

        [Display(Name = "Tag Name")]
        public string Tag_Name { get; set; }

        [Display(Name = "English Comment")]
        public string Eng_Comment { get; set; }

        [Display(Name = "Limit Max")]
        public double Limit_Max { get; set; }

        [Display(Name = "Limit Min")]
        public double Limit_Min { get; set; }

        [Display(Name = "DelayTime")]
        public int DelayTime { get; set; }

        [Display(Name = "LastEditUser", ResourceType = typeof(Menu))]
        public string login_name { get; set; }

        public static DataTable GetTypeLimitListDt(string type_id)
        {
            string sqlStr = "SELECT type_id,vendor_name,type_name,Tag_Name,Eng_Comment,Limit_Max,Limit_Min,DelayTime,login_name FROM vw_eq_typeLimit where type_id = '" + type_id + "'";

            DataSet DeptDS = DBConnector.executeQuery("Intouch", sqlStr);

            return DeptDS.Tables[0];
        }

        public static IEnumerable<TypeLimitModel> GetTypeLimitList(string type_id)
        {
            List<TypeLimitModel> TypeLimitList = new List<TypeLimitModel>();
            DataTable dt = GetTypeLimitListDt(type_id);

            TypeLimitList = (from dept in dt.AsEnumerable()
                              select new TypeLimitModel
                              {
                                  type_id = dept.Field<short>("type_id"),
                                  vendor_name = dept.Field<string>("vendor_name").ToString(),
                                  type_name = dept.Field<string>("type_name").ToString(),
                                  Tag_Name = dept.Field<string>("Tag_Name").ToString(),
                                  Eng_Comment = dept.Field<string>("Eng_Comment").ToString(),
                                  Limit_Max = dept.Field<double>("Limit_Max"),
                                  Limit_Min = dept.Field<double>("Limit_Min"),
                                  DelayTime = dept.Field<int>("DelayTime"),
                                  login_name = dept.Field<string>("login_name")
                              }).ToList();

            return TypeLimitList;
        }

        public static TypeLimitModel GetEditTypeLimit(string type_id, string Tag_Name)
        {
            TypeLimitModel EditTypeLimit = null;
            //DataTable dt = GetAlarmListdt(Tool);       

            string sqlStr = "SELECT type_id,vendor_name,type_name,Tag_Name,Eng_Comment,Limit_Max,Limit_Min,DelayTime,login_name FROM vw_eq_typeLimit where type_id = '" + type_id + "' AND Tag_Name = '" + Tag_Name + "'";

            DataSet DeptDS = DBConnector.executeQuery("Intouch", sqlStr);

            EditTypeLimit = (from dept in DeptDS.Tables[0].AsEnumerable()
                              select new TypeLimitModel
                          {
                              type_id = dept.Field<short>("type_id"),
                              vendor_name = dept.Field<string>("vendor_name").ToString(),
                              type_name = dept.Field<string>("type_name").ToString(),
                              Tag_Name = dept.Field<string>("Tag_Name").ToString(),
                              Eng_Comment = dept.Field<string>("Eng_Comment").ToString(),
                              Limit_Max = dept.Field<double>("Limit_Max"),
                              Limit_Min = dept.Field<double>("Limit_Min"),
                              DelayTime = dept.Field<int>("DelayTime"),
                              login_name = dept.Field<string>("login_name")
                          }).FirstOrDefault();

            return EditTypeLimit;
        }

        public string Update(string Usr)
        {
            try
            {
                DBConnector.executeSQL("Intouch", "EXEC [dbo].[uSP_Change_TypeLimit] @type_id='" + type_id + "',@Tag_Name=" + Tag_Name + ",@Limit_Max=" + Limit_Max + ",@Limit_Min=" + Limit_Min + ",@login_name='" + Usr + "',@DelayTime=" + DelayTime);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return null;
        }
    }
}