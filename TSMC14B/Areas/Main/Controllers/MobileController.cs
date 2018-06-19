using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebCMS.Areas.Main.Models;
using System.Configuration;

namespace WebCMS.Areas.Main.Controllers
{
    public class MobileController : Controller
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
            if (UserName !=null && UserName.Trim().Length > 0)
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

                    try
                    {
                        if (string.IsNullOrEmpty(reUrl))
                        {
                            string DefvName = LoginUser1.department_name;

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
                            
                            return RedirectToAction("ToolCheck", new { vName = DefvName, type = "PUMP" });
                        }
                        else
                        {
                            return Redirect(reUrl);
                        }
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("ToolCheck", new { vName = "TF1", type = "PUMP" });
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

        public ActionResult ToolCheck(string vName, string type)
        {
            if (Session["UserdepartmentName"]!=null && string.IsNullOrEmpty( vName))
            {
                vName = Session["UserdepartmentName"] as string;
            }
            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                List<vw_eq_status> codes2 = (from row in db.vw_eq_status where row.department_name == vName orderby row.process_name, row.EQID, row.chamber, row.WEB_code descending select row).ToList();
                ViewData["Device"] = codes2;
                ViewData["bar"] = vName;
                ViewData["title"] = vName;
                ViewBag.dept = vName;
                ViewData["process_name"] = (from row in codes2 select row.process_name).Distinct();
            }

            return View();
        }

        public ActionResult Logout()
        {
            Session["UserName"] = null;
            return Redirect("/Main/mobile/LoginPage");
        }

        public ActionResult Accessory(int pid) {

            vw_eq_status codes2; //PUMP

            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                codes2 = (from row in db.vw_eq_status where row.pid == pid select row).SingleOrDefault();

                //PM_Set PS = (from row in db.PM_Set where row.tool_id == codes2.chamberName select row).SingleOrDefault();

                //if (PS != null)
                //{
                //    ViewData["IsPM"] = PS.pmFlag ? 2 : 3;
                //}
                //else
                //{
                //    ViewData["IsPM"] = codes2.sid == 8 ? 0 : 1;
                //}
                
                ViewBag.WEB_code = codes2.WEB_code;
                ViewBag.title = codes2.chamberName;
                
                ViewBag.pid = codes2.pid;
                ViewBag.AccessoryType = codes2.WEB_code;
                ViewBag.ToolID = codes2.chamberName;
                ViewBag.plc_name = codes2.plc_name;
                ViewBag.vName = codes2.department_name;
                ViewBag.bar = codes2.department_name;
                //ViewBag.tImageUrl = "/images/" + codes2.tName + ".jpg";
                //ViewBag.IsValueNow = db.vw_eq_valuenow.Where(x => x.pid == pid).Any();
                //ViewBag.IsValueNow = (from row in db.vw_eq_valuenow.AsParallel() where row.pid == pid select row).Any();
                //ViewBag.IsAuth = Session["UserdepartmentName"] != null && ((string)Session["UserdepartmentName"] == codes2.department_name && int.Parse(Session["UserLevel"].ToString()) >= 600) || (Session["UserLevel"] != null && int.Parse(Session["UserLevel"].ToString()) >= 900);
                ViewBag.tName = codes2.tName;
                //ViewBag.IOGNO = codes2.channel_name;
                ViewBag.chamber = codes2.chamber;

                //ViewBag.LastFDCUploadTime = codes2.LastCaptureTime.ToString("yyyy-MM-dd HH:mm");
                //ViewBag.nextFDCUploadTime = codes2.LastCaptureTime > DateTime.Now.AddMinutes(2) ? "下次資料收集時間延至:" + codes2.LastCaptureTime.ToString("yyyy-MM-dd HH:mm") : "";
                //List<vw_eq_status> OtherAccessory = (from row in db.vw_eq_status where row.department_name == codes2.department_name && row.chamber == codes2.chamber && row.toolid == codes2.toolid && row.pid != pid select row).ToList();
                //ViewData["OtherAccessory"] = OtherAccessory;
                //List<vw_eq_alarmnow> Alarmnow = (from row in db.vw_eq_alarmnow where row.pid==pid select row).ToList();
                //ViewData["AlarmNow"] = Alarmnow;
            }

            return View();
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

                        if (login != null)
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

            return Json(new { IsSuccess = IsSuccess, Msg = Msg }, JsonRequestBehavior.AllowGet);
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

        public JsonResult JsonMobileLoginPlase()
        {
            bool IsSuccess = true;
            string Msg = "PlaseLogin";

            return Json(new { IsSuccess = IsSuccess, Msg = Msg }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult JsonMobileGetMenuItem(string username)
        {
            IEnumerable<ListModel> items = null;
            string Msg = null;
            bool IsSuccess = false;

            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                try
                {
                    var temp = from row in db.MobileLogin where row.login_name == username select row;
                    if (temp.Any())
                    {
                        items = ListModel.GetDepartmentTitleList();

                        if (items.Count() > 0)
                        {
                            IsSuccess = true;
                        }
                    }
                    else
                    {
                        IsSuccess = false;
                    }
                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    Msg = ex.Message;
                }
            }
            return this.Json(new { IsSuccess = IsSuccess, Msg = Msg, data = items }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult JsonMobileGetToolList(string dep_name, short vid)
        {
            IEnumerable<vw_eq_status> items = null;
            string Msg = null;
            bool IsSuccess = false;

            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                try
                {
                    var ToolList = from row in db.vw_eq_status where row.department_name == dep_name && row.vid == vid select row;
                    items = ToolList.ToList();
                    IsSuccess = true;
                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    Msg = ex.Message;
                }
            }
            return this.Json(new { IsSuccess = IsSuccess, Msg = Msg, data = items }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult JsonMobileGetTypeList()
        {
            string Msg = null;
            bool IsSuccess = false;

            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                try
                {
                    var TypeList = from row in db.type_info join v in db.vendor_info on row.vendor_id equals v.vendor_id select new { row.type_id, row.type_name, row.WEB_code, row.type_code, row.vendor_id, v.vendor_name, v.vendor_Code };
                    IsSuccess = true;

                    return this.Json(new { IsSuccess = IsSuccess, Msg = Msg, data = TypeList.ToList() }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    Msg = ex.Message;
                }
            }
            return this.Json(new { IsSuccess = IsSuccess, Msg = Msg, data = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult JsonMobileGetChartValue(string chambers, string TagNames, string StartDate, string EndDate)
        {
            string Msg = null;
            bool IsSuccess = false;
            System.Data.DataTable dt = null;
            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                try
                {
                    dt = PLCCheckModel.GetChartValuedt("aaa", chambers, TagNames, StartDate, EndDate, "3");
                    IsSuccess = true;
                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    Msg = ex.Message;
                }
            }
            return this.Json(new { IsSuccess = IsSuccess, Msg = Msg, data = dt.Rows }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult JsonMobileGetAccessory(int pid)
        {
            string Msg = null;
            bool IsSuccess = false;
            int IsPM = 0;

            string AccessoryType = null;
            string ToolID = null;
            string plc_name = null;
            string vName = null;
            string bar = null;
            string tImageUrl = null;
            bool IsValueNow = false;
            bool IsAuth = false;
            string tName = null;
            int statusid = 0;

            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                try
                {
                    vw_eq_status codes2 = (from row in db.vw_eq_status where row.pid == pid select row).FirstOrDefault();

                    PM_Set PS = (from row in db.PM_Set where row.tool_id == codes2.chamberName select row).SingleOrDefault();

                    if (PS != null)
                    {
                        IsPM = PS.pmFlag ? 2 : 3;
                    }
                    else
                    {
                        IsPM = codes2.sid == 8 ? 0 : 1;
                    }

                    AccessoryType = codes2.WEB_code;
                    ToolID = codes2.chamberName;
                    plc_name = codes2.plc_name;
                    vName = codes2.department_name;
                    bar = codes2.department_name;
                    tImageUrl = "/images/" + codes2.tName + ".jpg";
                    IsValueNow = (from row in db.vw_eq_valuenow where row.pid == pid select row).Any();
                    statusid = codes2.sid;
                    IsAuth = (string)Session["UserLevelName"] == codes2.department_name;
                    tName = codes2.tName;
                    IsSuccess = true;
                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    Msg = ex.Message;
                }
            }

            return this.Json(new
            {
                IsSuccess = IsSuccess,
                Msg = Msg,
                IsPM = IsPM,
                AccessoryType = AccessoryType,
                ToolID = ToolID,
                plc_name = plc_name,
                vName = vName,
                statusid = statusid,
                department_name = bar,
                tImageUrl = tImageUrl,
                IsValueNow = IsValueNow,
                tName = tName
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult JsonMobileGetMenuLevel()
        {
            string Msg = null;
            bool IsSuccess = false;

            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                try
                {
                    IsSuccess = true;
                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    Msg = ex.Message;
                }
            }
            return this.Json(new { IsSuccess = IsSuccess, Msg = Msg, data = System.Web.Configuration.WebConfigurationManager.AppSettings["MobileMenuLevel"] }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult JsonMobileGetMenuLevelItem(int Level, string parameter1, string parameter2, string parameter3)
        {
            string Msg = null;
            bool IsSuccess = false;

            JsonResult Result = null;
            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                try
                {
                    switch (Level)
                    {
                        case 1:
                            
                            var items = (from row in db.vw_eq_status select new { department_id = row.department_id, department_name = row.department_name }).ToList();

                            IsSuccess = true;
                            Result= this.Json(new { IsSuccess = IsSuccess, Msg = Msg, data = items }, JsonRequestBehavior.AllowGet);

                            break;
                        case 2:

                            if (parameter1.Length > 0)
                            {
                                var items2 = (from row in db.vw_eq_status where row.department_id == Byte.Parse(parameter1) select new { AccessoryType = row.WEB_code }).ToList();

                                IsSuccess = true;
                                Result= this.Json(new { IsSuccess = IsSuccess, Msg = Msg, data = items2 }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                var items2 = (from row in db.vw_eq_status select new { AccessoryType = row.WEB_code }).ToList();

                                IsSuccess = true;
                                Result = this.Json(new { IsSuccess = IsSuccess, Msg = Msg, data = items2 }, JsonRequestBehavior.AllowGet);
                            }

                            break;
                        case 3:
                            if (parameter1.Length > 0 &&　parameter2.Length >0 )
                            {
                                var items3 = (from row in db.vw_eq_status where row.WEB_code == parameter1 && row.department_id == Byte.Parse(parameter2) select new { process_id = row.process_id, process_name = row.process_name }).ToList();

                                IsSuccess = true;
                                Result = this.Json(new { IsSuccess = IsSuccess, Msg = Msg, data = items3 }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                var items3 = (from row in db.vw_eq_status select new { process_id = row.process_id, process_name = row.process_name }).ToList();

                                IsSuccess = true;
                                Result = this.Json(new { IsSuccess = IsSuccess, Msg = Msg, data = items3 }, JsonRequestBehavior.AllowGet);
                            }

                            break;
                        case 4:
                            if (parameter1.Length > 0 && parameter2.Length >0)
                            {
                                var items4 = (from row in db.vw_eq_status where row.WEB_code == parameter1 && row.process_id == short.Parse(parameter2) select new { EQID = row.EQID }).Distinct().ToList();

                                IsSuccess = true;
                                Result = this.Json(new { IsSuccess = IsSuccess, Msg = Msg, data = items4 }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                var items4 = (from row in db.vw_eq_status select new { EQID = row.EQID }).Distinct().ToList();

                                IsSuccess = true;
                                Result = this.Json(new { IsSuccess = IsSuccess, Msg = Msg, data = items4 }, JsonRequestBehavior.AllowGet);
                            }

                            break;
                        case 5:
                            if (parameter1.Length > 0 && parameter2.Length > 0 && parameter3.Length > 0)
                            {
                                var items5 = (from row in db.vw_eq_status where row.WEB_code == parameter1 && row.process_id == short.Parse(parameter2) && row.EQID == parameter3 select new { pid = row.pid, chamberName = row.chamberName }).ToList();

                                IsSuccess = true;
                                Result = this.Json(new { IsSuccess = IsSuccess, Msg = Msg, data = items5 }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                var items5 = (from row in db.vw_eq_status select new { pid = row.pid, chamberName = row.chamberName }).ToList();

                                IsSuccess = true;
                                Result = this.Json(new { IsSuccess = IsSuccess, Msg = Msg, data = items5 }, JsonRequestBehavior.AllowGet);
                            }

                            break;
                        default:
                            IsSuccess = false;
                            Msg = "no have this level";
                            break;
                    }
                    return Result;
                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                    Msg = ex.Message;
                }
            }
            return this.Json(new { IsSuccess = IsSuccess, Msg = Msg }, JsonRequestBehavior.AllowGet);
        }
    }
}