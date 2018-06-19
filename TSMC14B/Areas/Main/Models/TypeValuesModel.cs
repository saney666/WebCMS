using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using System.Data;
using WebCMS.Models;

namespace WebCMS.Areas.Main.Models
{
    public class TypeValuesModel
    {
       
        [Display(Name = "type_id")]
        public short? type_id { get; set; }

        [Display(Name = "Vendor Name")]
        public string vendor_name { get; set; }

        [Display(Name = "Type Name")]
        public string type_name { get; set; }

        [Display(Name = "Tag Name")]
        public string Tag_Name { get; set; }

        [Display(Name = "English Comment")]
        public string Eng_Comment { get; set; }

        [Display(Name = "Limit Max")]
        public double? Limit_Max { get; set; }

        [Display(Name = "Limit Min")]
        public double? Limit_Min { get; set; }


        public static DataTable GetTypeValuesListDt(string type_id)
        {        
            string sqlStr = "SELECT type_id,vendor_name,type_name,Tag_Name,Eng_Comment,Limit_Max,Limit_Min FROM vw_eq_typeValues where type_id = '" + type_id + "'";

            DataSet DeptDS = DBConnector.executeQuery("Intouch", sqlStr);

            return DeptDS.Tables[0];
        }

        public static IEnumerable<TypeValuesModel> GetTypeValuesList(string type_id)
        {
            List<TypeValuesModel> TypeValuesList = new List<TypeValuesModel>();
            DataTable dt = GetTypeValuesListDt(type_id);

            TypeValuesList = (from dept in dt.AsEnumerable()
                              select new TypeValuesModel
                              {
                                  type_id = dept.Field<short?>("type_id"),
                                  vendor_name = dept.Field<string>("vendor_name").ToString(),
                                  type_name = dept.Field<string>("type_name").ToString(),
                                  Tag_Name = dept.Field<string>("Tag_Name").ToString(),
                                  Eng_Comment = dept.Field<string>("Eng_Comment").ToString(),
                                  Limit_Max = dept.Field<double?>("Limit_Max"),
                                  Limit_Min = dept.Field<double?>("Limit_Min")           

                              }).ToList();

            return TypeValuesList;
        }

        public static TypeValuesModel GetEditTypeValues(string type_id, string Tag_Name)
        {
            TypeValuesModel EditTypeValues = new TypeValuesModel();
            //DataTable dt = GetAlarmListdt(Tool);       

            string sqlStr = "SELECT type_id,vendor_name,type_name,Tag_Name,Limit_Max,Limit_Min FROM vw_eq_typeValues where type_id = '" + type_id + "' AND Tag_Name = '" + Tag_Name + "'";

            DataSet DeptDS = DBConnector.executeQuery("Intouch", sqlStr);

            EditTypeValues = (from dept in DeptDS.Tables[0].AsEnumerable()
                          select new TypeValuesModel
                          {
                              type_id = dept.Field<short?>("type_id"),
                              vendor_name = dept.Field<string>("vendor_name").ToString(),
                              type_name = dept.Field<string>("type_name").ToString(),
                              Tag_Name = dept.Field<string>("Tag_Name").ToString(),
                              Limit_Max = dept.Field<double?>("Limit_Max"),
                              Limit_Min = dept.Field<double?>("Limit_Min")        
                          }).FirstOrDefault();

            return EditTypeValues;
        }

        public string Update(string Usr)
        {
            try
            {
                DBConnector.executeSQL("Intouch", "EXEC [dbo].[uSP_Change_TypeValues] '" + type_id + "'," + Tag_Name + "," + Limit_Max + "," + Limit_Min);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return null;
        }

    }
}