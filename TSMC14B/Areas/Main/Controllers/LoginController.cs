using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TSMC14B.Areas.Main.Models;

namespace TSMC14B.Areas.Main.Controllers
{
    public class LoginController : Controller
    {
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)] // will disable caching for Index only
        public ActionResult LoginPage()
        {
            ViewBag.bar = "Account";
            Session["UserID"] = null;
            Session["UserName"] = null;
            Session["UserLevel"] = null;
            Session["UserLevelName"] = null;
            return View();
        }

        [HttpPost]
        public ActionResult LoginPage(string UserName, string Password, string reUrl)
        {
            if (UserName.Trim().Length > 0)
            {
                if (LoginModel.Login(UserName.Trim(), Password.Trim()))
                {
                    LoginModel LoginUser1 = LoginModel.Get(UserName);
                    Session["UserID"] = LoginUser1.UserName.Trim();
                    Session["UserName"] = LoginUser1.title.Trim();
                    Session["UserLevel"] = LoginUser1.login_level;
                    Session["UserLevelName"] = LoginUser1.Level.Trim();
                    Session["UserdepartmentId"] = LoginUser1.department_id;
                    Session["UserdepartmentName"] = LoginUser1.department_name;

                    //if ((DateTime.Now - Convert.ToDateTime(LoginModel.Get(UserName).UpdateDate)).TotalDays < 90 && (DateTime.Now - Convert.ToDateTime(LoginModel.Get(UserName).UpdateDate)).TotalDays > 76)
                    //{
                    //    Session["UserName"] = null;
                    //    Session["UserLevel"] = null;
                    //    ViewBag.MessageType = "警告";
                    //    ViewBag.Message = "您的密碼已到期，如有疑問請洽管理員，謝謝…";
                    //    return View("Message");
                    //}
                    //else if ((DateTime.Now - Convert.ToDateTime(LoginModel.Get(UserName).UpdateDate)).TotalDays < 76)
                    //{
                    //    ViewBag.Message = "您的密碼於14天後到期，請變更密碼，謝謝…";
                    //}

                    try
                    {
                        if (string.IsNullOrEmpty(reUrl))
                        {
                            string DefvName = string.Empty;

                            if (LoginUser1.login_level <= 800)
                            {
                                using (tsmc14BDataContext db = new tsmc14BDataContext())
                                {
                                    DefvName = (from row in db.department_info where row.department_id == LoginUser1.department_id orderby row.display_order select row.department_name).FirstOrDefault();
                                }
                            }

                            if (DefvName.Length == 0)
                            {
                                using (tsmc14BDataContext db = new tsmc14BDataContext())
                                {
                                    DefvName = (from row in db.department_info orderby row.display_order select row.department_name).FirstOrDefault();
                                }
                            }

                            return RedirectToAction("PLCCheck2", "MachineGroup", new { vName = DefvName, type = "PUMP" });
                        }
                        else
                        {
                            return Redirect(reUrl);
                        }
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("PLCCheck2", "MachineGroup", new { vName = "TF1", type = "PUMP" });
                    }
                }
                else
                {
                    ViewBag.Message = "帳密錯誤";
                }
            }
            else
            {
                ViewBag.Message = "請輸入帳號密碼";
            }
            ViewBag.bar = "Account";

            return View();
        }

        public ActionResult Logout()
        {
            Session["UserName"] = null;
            return Redirect("/Main/Login/LoginPage");
        }

        public ActionResult LoginManage()
        {
            IEnumerable<LoginModel> codes2 = LoginModel.List((Int16)Session["UserLevel"], (byte?)Session["UserdepartmentId"]);

            ViewBag.AccountList = from c in codes2
                                  select c;

            ViewBag.sessionType = from c in LoginModel.SessionLevel((Int16)Session["UserLevel"])
                                  select new SelectListItem
                                  {
                                      Value = c.level_id.ToString(),
                                      Text = c.Level
                                  };

            if ((Int16)Session["UserLevel"] >= 998)
            {
                ViewBag.department_info = from d in ListModel.GetDepartmentList()
                                          select new SelectListItem
                                          {
                                              Value = d.Value,
                                              Text = d.Text
                                          };
            }
            else
            {
                ViewBag.department_info = from d in ListModel.GetDepartmentList()
                                          where d.Value == Session["UserdepartmentId"].ToString()
                                          select new SelectListItem
                                          {
                                              Value = d.Value,
                                              Text = d.Text
                                          };
            }

            ViewBag.bar = "Account";
            return View();
        }

        public JsonResult JsonActQuery(int page, int rp)
        {
            IEnumerable<LoginModel> ls = LoginModel.List((Int16)Session["UserLevel"], (byte?)Session["UserdepartmentId"]);
            return Json(new
            {
                page = page,
                total = ls.Count(),
                rows = from c in ls.Skip((page - 1) * rp).Take(rp)
                       select c
            });
        }

        [HttpPost]
        public JsonResult JsonExecute(string UserID, string Pwd, string UsrTitle, string Level, string Type, Int16? departmentId)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                string UsrID = (string)Session["UserName"];
                bool IsSuccess = false;
                string Msg = "失敗…";
                LoginModel lm = new LoginModel();
                try
                {
                    string DBMsg = null;
                    if (Type == "Insert")
                    {
                        DBMsg = lm.Insert(UserID, Pwd, UsrTitle, Level, departmentId);
                    }
                    else if (Type == "delete")
                    {
                        DBMsg = lm.Delete(UserID);
                    }
                    else if (Type == "UpdateAdmin")
                    {
                        DBMsg = lm.UpdateAdmin(UserID, Pwd);
                    }

                    if (!string.IsNullOrEmpty(DBMsg) && DBMsg.Contains("成功"))
                    {
                        ViewBag.Message = DBMsg;
                        IsSuccess = true;
                        Msg = DBMsg;
                    }
                    else
                    {
                        ViewBag.Message = DBMsg;
                        Msg = DBMsg;
                    }
                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    Msg = ex.Message;
                }

                return Json(new { IsSuccess = IsSuccess, Msg = Msg });
            }
            else
                return null;
        }

        [HttpGet]
        public ActionResult EditUser(string UsrName)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                ViewBag.sessionType = from c in LoginModel.SessionLevel((Int16)Session["UserLevel"])
                                      select new SelectListItem
                                      {
                                          Value = c.Level,
                                          Text = c.Level
                                      };
                LoginModel lm = LoginModel.Get(UsrName);
                return View(lm);
            }
            else
            {
                ViewBag.MessageType = "警告";
                ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                return View("Message");
            }
        }

        [HttpPost]
        public ActionResult EditUser(LoginModel lm)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                //if (ModelState.IsValid)
                //{
                string DBMsg = lm.Update();
                if (string.IsNullOrEmpty(DBMsg))
                    ViewBag.Message = "修改完成…";
                else
                    ViewBag.Message = DBMsg;
                //}
                //else
                //    ViewBag.Message = "資料不正確…";

                ViewBag.sessionType = from c in LoginModel.SessionLevel((Int16)Session["UserLevel"])
                                      select new SelectListItem
                                      {
                                          Value = c.Level,
                                          Text = c.Level
                                      };
                return View();
            }
            else
            {
                ViewBag.MessageType = "警告";
                ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                return View("Message");
            }
        }

        public JsonResult JsonMobileLogin(string username, string pwd)
        {
            bool IsSuccess = false;
            string Msg = "失敗…";
            try
            {
       
                if (LoginModel.Login(username.Trim(), pwd.Trim()))
                {
                    using (tsmc14BDataContext db = new tsmc14BDataContext())
                    {
                        var login = (from row in db.MobileLogin where row.login_name == username select row).FirstOrDefault();

                        if (login!=null)
                        {
                            login.login_Time = DateTime.Now;
                            
                            db.SubmitChanges();
                        }
                        else
                        {
                            MobileLogin ML = new MobileLogin();
                            ML.login_name = username;
                            ML.login_Time = DateTime.Now;

                            db.MobileLogin.InsertOnSubmit(ML);
                            db.SubmitChanges();
                        }

                        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                            username,//你想要存放在 User.Identy.Name 的值，通常是使用者帳號
                            DateTime.Now,
                            DateTime.Now.AddMinutes(30),
                            false,//將管理者登入的 Cookie 設定成 Session Cookie
                            "",//userdata看你想存放啥
                            FormsAuthentication.FormsCookiePath);

                        string encTicket = FormsAuthentication.Encrypt(ticket);

                        Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

                        IsSuccess = true;
                        Msg = "登入成功";
                    }
                }
                else
                {
                    IsSuccess = false;
                    Msg = "登入失敗";
                }
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Msg = ex.Message;
            }

            return Json(new { IsSuccess = IsSuccess, Msg = Msg },JsonRequestBehavior.AllowGet);
        }

        public JsonResult JsonMobileLogout(string username)
        {
            bool IsSuccess = false;
            string Msg = "失敗…";
            try
            {
                using (tsmc14BDataContext db = new tsmc14BDataContext())
                {
                    var ML = from row in db.MobileLogin where row.login_name == username select row;

                    db.MobileLogin.DeleteAllOnSubmit(ML.ToList());
                    db.SubmitChanges();

                    FormsAuthentication.SignOut();
                }

                IsSuccess = true;
                Msg = "登出成功";

            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Msg = ex.Message;
            }

            return Json(new { IsSuccess = IsSuccess, Msg = Msg }, JsonRequestBehavior.AllowGet);
        }
    }
}