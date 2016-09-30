using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using TSMC14B.Models;

namespace TSMC14B.Areas.Main.Models
{
    public class LoginModel
    {
        [Display(Name = "UserName", ResourceType = typeof(TSMC14B.Menu))
            , Required(ErrorMessageResourceName = "UserNameMust", ErrorMessageResourceType = typeof(TSMC14B.Menu))
            //, StringLength(20, MinimumLength = 8, ErrorMessage = "帳號最長20個字…")
            , Remote("CheckUserName", "Validate",ErrorMessageResourceName="UserNameAlreadyExists", ErrorMessageResourceType = typeof(TSMC14B.Menu))
         
        ]
        public string UserName { get; set; }

        [Display(Name = "Password", ResourceType = typeof(TSMC14B.Menu))
            , Required
            //, StringLength(20, MinimumLength = 6, ErrorMessage = "密碼至少6個字")
            //, RegularExpression("^(?=.*\\d)(?=.*[a-z]).{6,20}$", ErrorMessage = "密碼最少6個字需包括英文大小寫、數字及特殊符號,如:Administr@t0r")
            //, RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[~!@#$%_^&*()=+[\\]{}''\";:/?.,><`|！·￥…—（）\\-、；：。，》《]).{6,20}$", ErrorMessage = "密碼最少6個字需包括英文大小寫、數字及特殊符號,如:Administr@t0r")
            ]
        public string Password { get; set; }

        [Display(Name = "NewPassword", ResourceType = typeof(TSMC14B.Menu))
            , Required
            , Compare("Password", ErrorMessageResourceName = "PasswordMustSame", ErrorMessageResourceType = typeof(TSMC14B.Menu))
            //, RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[~!@#$%_^&*()=+[\\]{}''\";:/?.,><`|！·￥…—（）\\-、；：。，》《]).{8,20}$", ErrorMessage = "密碼最少8個字需包括英文大小寫、數字及特殊符號,如:Administr@t0r")
            ]
        public string Password2 { get; set; }

        [Display(Name = "Level", ResourceType = typeof(TSMC14B.Menu))]
        public string Level { get; set; }
        public Int16 login_level { get; set; }
        public byte level_id { get; set; }

        [Display(Name = "title", ResourceType = typeof(TSMC14B.Menu))
            , StringLength(20, ErrorMessageResourceName="titleLongest",ErrorMessageResourceType=typeof(TSMC14B.Menu))]
        public string title { get; set; }

        [Display(Name = "UpdateDate")]
        public string UpdateDate { get; set; }
        public byte? department_id { get; set; }

        [Display(Name = "department_name", ResourceType = typeof(TSMC14B.Menu))]
        public string department_name { get; set; }

        public static bool Login(string UserName, string Password)
        {
            DataSet CodeDS = DBConnector.executeQuery("Intouch", "EXEC uSP_UserLogin @login_name = '" + UserName + "', @login_pwd = '" + Password + "'");
            if (CodeDS.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        //public static string GetLevel(string UserName)
        //{
        //    using (tsmc14BDataContext db = new tsmc14BDataContext())
        //    {
        //        DataSet CodeDS = DBConnector.executeQuery("Intouch", "select level_id from login_info where login_name = '" + UserName + "'");
        //        return (from dept in CodeDS.Tables[0].AsEnumerable() select dept.Field<Byte>("level_id")).FirstOrDefault().ToString();
        //    }
        //}

        public static LoginModel Get(string UserName)
        {
            //DataSet CodeDS = DBConnector.executeQuery("Intouch", "select * from login_info where login_name = '" + UserName + "'");
            DataSet CodeDS = DBConnector.executeQuery("Intouch", "EXEC uSP_Select_LoginInfo");
            DataTable dt =  new DataTable();
            if ((from code in CodeDS.Tables[0].AsEnumerable()
                 where code.Field<string>("login_name") == UserName.Trim()
                 select code).Any())
            {

                dt = (from code in CodeDS.Tables[0].AsEnumerable()
                                where code.Field<string>("login_name") == UserName.Trim()
                                select code).CopyToDataTable();

            } 

            if (dt.Rows.Count > 0)
            {
                return new LoginModel
                {
                    UserName = dt.Rows[0].Field<string>("login_name"),
                    Password = dt.Rows[0].Field<string>("login_pwd"),
                    login_level = dt.Rows[0].Field<Int16>("login_level"),
                    Level = dt.Rows[0].Field<string>("level_name"),
                    title = dt.Rows[0].Field<string>("username"),
                    UpdateDate = dt.Rows[0].Field<DateTime>("update_date").ToString("yyyy-MM-dd"),
                    department_id = dt.Rows[0].Field<byte?>("department_id"),
                    department_name = dt.Rows[0].Field<string>("department_name"),
                };
            }

            return null;
        }

        public static LoginModel Get(string UserName, string Pwd)
        {
            //DataSet CodeDS = DBConnector.executeQuery("Intouch", "select * from login_info where login_name = '" + UserName + "'");
            DataSet CodeDS = DBConnector.executeQuery("Intouch", "EXEC uSP_Select_LoginInfo");

            DataTable dt = (from code in CodeDS.Tables[0].AsEnumerable()
                            where code.Field<string>("login_name") == UserName.Trim() && code.Field<string>("login_pwd") == Pwd.Trim()
                            select code).CopyToDataTable();

            if (dt.Rows.Count > 0)
            {
                return new LoginModel
                {
                    UserName = dt.Rows[0].Field<string>("login_name"),
                    Password = dt.Rows[0].Field<string>("login_pwd"),
                    Level = dt.Rows[0].Field<string>("level_name"),
                    login_level = dt.Rows[0].Field<Int16>("login_level"),
                    title = dt.Rows[0].Field<string>("username"),
                    UpdateDate = dt.Rows[0].Field<DateTime>("update_date").ToString("yyyy-MM-dd"),
                    department_id = dt.Rows[0].Field<byte?>("department_id"),
                    department_name = dt.Rows[0].Field<string>("department_name"),
                };
            }
            return null;
        }

        public static IEnumerable<LoginModel> List(Int16 LoginLevel, byte? department_id)
        {
            DataSet ds = DBConnector.executeQuery("Intouch", "EXEC uSP_Select_LoginInfo");

            if (LoginLevel >= 998)
            {

                return from dept in ds.Tables[0].AsEnumerable()
                       where dept.Field<Int16>("login_level") < LoginLevel
                       orderby dept.Field<Int16>("login_level") descending
                       select new LoginModel
                       {
                           UserName = dept.Field<string>("login_name"),
                           Password = dept.Field<string>("login_pwd"),
                           Level = dept.Field<string>("level_name"),
                           login_level = dept.Field<Int16>("login_level"),
                           title = dept.Field<string>("username"),
                           UpdateDate = dept.IsNull("update_date") ? String.Empty : dept.Field<DateTime>("update_date").ToString("yyyy-MM-dd"),
                           department_id = dept.Field<byte?>("department_id"),
                           department_name = dept.Field<string>("department_name"),
                       };
            }
            else
            {
                return from dept in ds.Tables[0].AsEnumerable()
                       where dept.Field<Int16>("login_level") < LoginLevel && dept.Field<byte?>("department_id") == department_id
                       orderby dept.Field<Int16>("login_level") descending
                       select new LoginModel
                       {
                           UserName = dept.Field<string>("login_name"),
                           Password = dept.Field<string>("login_pwd"),
                           Level = dept.Field<string>("level_name"),
                           login_level = dept.Field<Int16>("login_level"),
                           title = dept.Field<string>("username"),
                           UpdateDate = dept.IsNull("update_date") ? String.Empty : dept.Field<DateTime>("update_date").ToString("yyyy-MM-dd"),
                           department_id = dept.Field<byte?>("department_id"),
                           department_name = dept.Field<string>("department_name"),
                       };
            }
            //if (LoginLevel == "系統管理者")
            //{
            //    return from dept in ds.Tables[0].AsEnumerable()
            //           where dept.Field<Int16>("login_level") < 999
            //           orderby dept.Field<Int16>("login_level") descending
            //           select new LoginModel
            //           {
            //               UserName = dept.Field<string>("login_name"),
            //               Password = dept.Field<string>("login_pwd"),
            //               Level = dept.Field<string>("level_name"),
            //               login_level = dept.Field<Int16>("login_level"),
            //               title = dept.Field<string>("username"),
            //               UpdateDate = dept.IsNull("update_date") ? String.Empty : dept.Field<DateTime>("update_date").ToString("yyyy-MM-dd"),
            //               department_id = dept.Field<byte>("department_id"),
            //               department_name = dept.Field<string>("department_name"),
            //           };
            //}
            //else if (LoginLevel == "帳號管理者")
            //{
            //    return from dept in ds.Tables[0].AsEnumerable()
            //           where dept.Field<Int16>("login_level") < 998
            //           orderby dept.Field<Int16>("login_level") descending
            //           select new LoginModel
            //           {
            //               UserName = dept.Field<string>("login_name"),
            //               Password = dept.Field<string>("login_pwd"),
            //               Level = dept.Field<string>("level_name"),
            //               login_level = dept.Field<Int16>("login_level"),
            //               title = dept.Field<string>("username"),
            //               UpdateDate = dept.IsNull("update_date") ? String.Empty : dept.Field<DateTime>("update_date").ToString("yyyy-MM-dd"),
            //               department_id = dept.Field<byte>("department_id"),
            //               department_name = dept.Field<string>("department_name"),
            //           };
            //}
            //else if (LoginLevel == "部門管理者")
            //{
            //    return from dept in ds.Tables[0].AsEnumerable()
            //           where (dept.Field<Int16>("login_level") < 998 & dept.Field<string>("level_name") == LevelStr) || dept.Field<string>("level_name") == "廠商"
            //           orderby dept.Field<Int16>("login_level") descending
            //           select new LoginModel
            //           {
            //               UserName = dept.Field<string>("login_name"),
            //               Password = dept.Field<string>("login_pwd"),
            //               Level = dept.Field<string>("level_name"),
            //               login_level = dept.Field<Int16>("login_level"),
            //               title = dept.Field<string>("username"),
            //               UpdateDate = dept.IsNull("update_date") ? String.Empty : dept.Field<DateTime>("update_date").ToString("yyyy-MM-dd"),
            //               department_id = dept.Field<byte>("department_id"),
            //               department_name = dept.Field<string>("department_name"),
            //           };
            //}
            //else
            //{
            //    return from dept in ds.Tables[0].AsEnumerable()
            //           where (dept.Field<Int16>("login_level") < 998 & dept.Field<string>("level_name") == LevelStr) || dept.Field<string>("level_name") == "廠商"
            //           orderby dept.Field<Int16>("login_level") descending
            //           select new LoginModel
            //           {
            //               UserName = dept.Field<string>("login_name"),
            //               Password = dept.Field<string>("login_pwd"),
            //               Level = dept.Field<string>("level_name"),
            //               login_level = dept.Field<Int16>("login_level"),
            //               title = dept.Field<string>("username"),
            //               UpdateDate = dept.IsNull("update_date") ? String.Empty : dept.Field<DateTime>("update_date").ToString("yyyy-MM-dd"),
            //               department_id = dept.Field<byte>("department_id"),
            //               department_name = dept.Field<string>("department_name"),
            //           };
            //}
        }

        public static IEnumerable<LoginModel> SessionLevel(Int16 LoginLevel)
        {
            DataSet ds = DBConnector.executeQuery("Intouch", "EXEC uSP_Select_LoginLevel");

            return from dept in ds.Tables[0].AsEnumerable()
                   where dept.Field<Int16>("login_level") < LoginLevel
                   orderby dept.Field<Int16>("login_level") descending
                   select new LoginModel
                   {
                       Level = dept.Field<string>("level_name").ToString(),
                       level_id = dept.Field<byte>("level_id"),
                   };

            //if (LevelStr == "系統管理者")
            //{
            //    return from dept in ds.Tables[0].AsEnumerable()
            //           where Convert.ToInt16(dept.Field<Byte>("level_id")) > 1
            //           orderby dept.Field<Int16>("login_level") descending
            //           select new LoginModel
            //           {
            //               Level = dept.Field<string>("level_name").ToString(),
            //               login_level = dept.Field<byte>("login_level"),
            //           };
            //}
            //else if (LevelStr == "帳號管理者")
            //{
            //    return from dept in ds.Tables[0].AsEnumerable()
            //           where Convert.ToInt16(dept.Field<Byte>("level_id")) > 2
            //           orderby dept.Field<Int16>("login_level") descending
            //           select new LoginModel
            //           {
            //               Level = dept.Field<string>("level_name").ToString(),
            //               login_level = dept.Field<Int16>("login_level"),
            //           };
            //}
            //else
            //{
            //    return from dept in ds.Tables[0].AsEnumerable()
            //           where (Convert.ToInt16(dept.Field<Byte>("level_id")) > 2 & dept.Field<string>("level_name") == LevelStr) || dept.Field<string>("level_name") == "廠商"
            //           orderby dept.Field<Int16>("login_level") descending
            //           select new LoginModel
            //           {
            //               Level = dept.Field<string>("level_name").ToString(),
            //               login_level = dept.Field<Int16>("login_level"),
            //           };
            //}
        }

        //新增
        public string Insert(string UserID, string Pwd, string UsrTitle, string Level, Int16? departmentId)
        {
            string DBMsg = null;
            try
            {
                //DBConnector.executeSQL("Intouch", "INSERT INTO login_info ([login_name],[login_pwd],[level_id],[username],[update_date],[built_date])VALUES('" + UserID + "','" + Pwd + "','" + Level + "','" + UsrTitle + "',GETDATE(),GETDATE())");
                DBConnector.executeSQL("Intouch", "EXEC uSP_Change_LoginInfo @action = 1, @login_name = '" + UserID + "', @login_pwd = '" + Pwd + "', @level_id = '" + Level + "', @username = '" + UsrTitle + "', @departmentId = " + departmentId);
                DBMsg = "新增成功";
            }
            catch (Exception ex)
            {
                DBMsg += ex.Message;
            }
            return DBMsg;
        }

        //刪除
        public string Delete(string UserID)
        {
            string DBMsg = null;
            try
            {
                //DBConnector.executeSQL("Intouch", "DELETE FROM login_info WHERE login_name ='" + UserID + "'");
                DBConnector.executeSQL("Intouch", "EXEC uSP_Change_LoginInfo @action = 0, @login_name = '" + UserID + "'");
                DBMsg = UserID + "刪除成功";
            }
            catch (Exception ex)
            {
                DBMsg += ex.Message;
            }
            return DBMsg;
        }

        //更新系統管理者密碼
        public string UpdateAdmin(string UserID, string Pwd)
        {
            string DBMsg = null;
            try
            {
                //DBConnector.executeSQL("Intouch", "update login_info SET login_pwd = '" + Pwd + "' WHERE level_id = 1");
                DBConnector.executeSQL("Intouch", "EXEC uSP_Change_LoginInfo @action = 2, @login_name = '" + UserID + "', @login_pwd = '" + Pwd + "'");
                DBMsg = "密碼更新成功";
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
                //DBConnector.executeSQL("Intouch", "update login_info SET login_pwd = '" + Password2 + "', username = '" + title + "', update_date = GETDATE() WHERE login_name = '" + UserName + "'");
                DBConnector.executeSQL("Intouch", "EXEC uSP_Change_LoginInfo @action = 2, @login_name = '" + UserName + "', @login_pwd = '" + Password + "', @username = '" + title + "', @level_id = " + LoginModel.Getlid(Level));
                DBMsg = "修改成功";
            }
            catch (Exception ex)
            {
                DBMsg += ex.Message;
            }
            return DBMsg;
        }
        
        public static short Getlid(string lvl)
        {
            DataSet ds = DBConnector.executeQuery("Intouch", "EXEC uSP_Select_LoginLevel");

            return (from dept in ds.Tables[0].AsEnumerable()
                    where dept.Field<string>("level_name") == lvl
                    select dept.Field<byte>("level_id")).FirstOrDefault();
        }
    }
}