using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TSMC14B.Areas.Main.Models;
using System.Threading;

namespace TSMC14B.Areas.Main.Controllers
{
    public class HomeController : Controller
    {
        #region Index
        public ActionResult Index()
        {
            //ViewBag.phoneCallStatusLight = AlarmNowModel.GetPCstatusLight();
            //ViewBag.phoneCallStatus = ListModel.GetPCstatusList();

            ViewBag.eqCount = StatusCountModel.IndexAllStatusList();

            if (Session["UserName"] == null)
            {
                return RedirectToAction("LoginPage", "Login");
            }

            LoginModel LoginUser1 = LoginModel.Get(Session["UserName"].ToString());

            string DefvName = string.Empty;

            Session["IsCN"] = Thread.CurrentThread.CurrentCulture.ToString();
            //Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("zh-CN");
            //Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            if (LoginUser1 != null && LoginUser1.login_level <= 800)
            {
                using (tsmc14BDataContext db = new tsmc14BDataContext())
                {
                    DefvName = (from row in db.department_info where row.department_name == LoginUser1.department_name orderby row.display_order select row.department_name).FirstOrDefault();
                }
            }

            if (DefvName.Length == 0)
            {
                using (tsmc14BDataContext db = new tsmc14BDataContext())
                {
                    DefvName = (from row in db.department_info orderby row.display_order select row.department_name).FirstOrDefault();
                }
            }
            return RedirectToAction("PLCCheck2", "MachineGroup", new { vName = DefvName });

            //return View();
        }

        public JsonResult JsonIndex(int page, int rp)
        {
            IEnumerable<AlarmNowModel> ls = AlarmNowModel.alarmNowList(ConfigurationManager.AppSettings["AlarmLevel"], 0, 0);
            return Json(new
            {
                page = page,
                total = ls.Count(),
                rows = from c in ls.Skip((page - 1) * rp).Take(rp)
                       select c
            });
        }

        public JsonResult JsonVendorStatus()
        {
            return Json(StatusCountModel.IndexAllStatusList());
        }

        //public JsonResult JsonPhoneCall()
        //{
        //    //IEnumerable<ListModel> ls = ListModel.GetPCstatusList();
        //    //return Json(new
        //    //{
        //    //    page = page,
        //    //    total = ls.Count(),
        //    //    rows = from c in ls.Skip((page - 1) * rp).Take(rp)
        //    //           select c
        //    //});
        //    return Json(ListModel.GetPCstatusList(), JsonRequestBehavior.AllowGet);
        //}

        public JsonResult JsonPerWarning(int page, int rp)
        {
            IEnumerable<AlarmNowModel> ls = AlarmNowModel.alarmNowList("800", 1, 0);
            return Json(new
            {
                page = page,
                total = ls.Count(),
                rows = from c in ls.Skip((page - 1) * rp).Take(rp)
                       select c
            });
        }

        public JsonResult JsonIPIPerWarning(int vendor, int page, int rp)
        {
            IEnumerable<AlarmNowModel> ls = AlarmNowModel.alarmNowList("800", 1, vendor);
            return Json(new
            {
                page = page,
                total = ls.Count(),
                rows = from c in ls.Skip((page - 1) * rp).Take(rp)
                       select c
            });
        }

        public JsonResult JsonDryPumpWarning(int page, int rp)
        {
            IEnumerable<AlarmNowModel> ls = AlarmNowModel.alarmNowList("810", 1, 0);
            return Json(new
            {
                page = page,
                total = ls.Count(),
                rows = from c in ls.Skip((page - 1) * rp).Take(rp)
                       select c
            });
        }

        public JsonResult GetAlarmID(string toolid, string alarmTime)
        {
            return Json(ListModel.GetAlarmId(toolid, alarmTime), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AlarmTagList(string Comment)
        {
            return Json(ListModel.GetAlarmTagList(Comment), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 其他狀態數
        public ActionResult GroupElseStatus(string vName, string Status)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
                return View();
            else
                return RedirectToAction("LoginPage", "Login");
        }

        public JsonResult JsonGroupElseStatus(string vName, string Status)
        {
            IEnumerable<StatusDetialModel> ls = StatusDetialModel.StatusDetial(vName, Status);

            return Json(new
            {
                //page = page,
                total = ls.Count(),
                rows = from c in ls  //.Skip((page - 1) * rp).Take(rp)
                       select c
            });
        }

        #endregion

        #region PM History
        public ActionResult PMHistory(string Vendor, string toolId, string StartDate, string EndDate, string Device, string output, string selectType)
        {
            if (output != "export")
            {
                IEnumerable<ListModel> codes = ListModel.GetDepartmentList();
                ViewBag.Vendor = from c in codes
                                 select new SelectListItem
                                 {
                                     Value = c.Text,
                                     Text = c.Text
                                 };

                IEnumerable<ListModel> codes2 = ListModel.GetToolIDList(Vendor);
                ViewBag.ToolID = from c in codes2
                                 select new SelectListItem
                                 {
                                     Value = c.Value,
                                     Text = c.Text
                                 };
                //IEnumerable<PMModel> ls = PMModel.PMHistoryList(Vendor == null ? "CS250" : Vendor, toolId == null ? "*" : toolId, StartDate == null ? DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm") : StartDate, EndDate == null ? DateTime.Now.ToString("yyyy-MM-dd HH:mm") : EndDate).Take(1);
                //return View(ls);
                ViewBag.UserLevel = (string)Session["UserLevelName"];
                ViewData["bar"] = "Report";
                return View();
            }
            else
            {
                byte[] file = PMModel.GetPMHistoryFile(Vendor, toolId == "ALL" ? string.Empty : toolId, StartDate, EndDate, selectType);
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PM History_" + DateTime.Now.ToShortDateString() + ".xlsx");
            }
        }

        [HttpPost]
        public JsonResult PMHistorytid(string vName, string gname)
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            var tid = ListModel.GetToolIDList(vName, gname);
            if (tid.Count() > 0)
            {
                foreach (var product in tid)
                {
                    items.Add(new KeyValuePair<string, string>(
                        product.Text,
                        product.Text
                    ));
                }
            }

            return this.Json(items);
        }

        public JsonResult JsonPMHistory(string Vendor, string toolId, string StartDate, string EndDate, string selectType, string gname, int page, int rp)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                Vendor = Vendor.Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();
                IEnumerable<PMModel> ls = PMModel.PMHistoryList(Vendor == null ? "CS250" : Vendor, toolId == "ALL" ? "*" : toolId, StartDate == null ? DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm") : StartDate, EndDate == null ? DateTime.Now.ToString("yyyy-MM-dd HH:mm") : EndDate, selectType, gname);

                return Json(new
                {
                    page = page,
                    total = ls.Count(),
                    rows = from c in ls.Skip((page - 1) * rp).Take(rp)
                           select c
                });
            }
            else
                return null;
        }
        #endregion

        #region PM
        public ActionResult PMConfig(string Vendor, string Device, string toolId, string pmFlag, string output)
        {
            IEnumerable<ListModel> codes = ListModel.GetDepartmentList();
            ViewBag.Vendor = from c in codes
                             select new SelectListItem
                             {
                                 Value = c.Text,
                                 Text = c.Text
                             };
            IEnumerable<ListModel> codes2 = ListModel.GetGroupList(Vendor);
            ViewBag.Device = from c in codes2
                             select new SelectListItem
                             {
                                 Value = c.Value,
                                 Text = c.Text
                             };

            if (!string.IsNullOrEmpty(Vendor))
            {
                IEnumerable<PLCCheckModel> codes3 = PLCCheckModel.GetPMToolidList(Vendor, Device, toolId, pmFlag == null ? "1" : pmFlag);
                ViewBag.ToolID = from c in codes3
                                 select new SelectListItem
                                 {
                                     Value = c.Name,
                                     Text = c.Name
                                 };
            }
            else if (!string.IsNullOrEmpty(toolId))
            {
                IEnumerable<PLCCheckModel> codes3 = PLCCheckModel.GetPMToolidList((string)Session["UserLevelName"], toolId, pmFlag == null ? "1" : pmFlag);
                ViewBag.ToolID = from c in codes3
                                 select new SelectListItem
                                 {
                                     Value = c.Name,
                                     Text = c.Name
                                 };
            }

            IEnumerable<ListModel> codes4 = ListModel.GetPMVendorList((string)Session["UserLevelName"]);
            ViewBag.PMVendor = from c in codes4
                               select new SelectListItem
                               {
                                   Value = c.Text,
                                   Text = c.Text
                               };

            //待處理中清單
            ViewBag.List = from c in PMModel.NotyetPMList()
                           select c;

            ViewBag.UserLevel = (string)Session["UserLevelName"];

            return View();
        }

        [HttpPost]
        public JsonResult PMgName(string Vendor)
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            var tid = ListModel.GetGroupList(Vendor);

            if (tid.Count() > 0)
            {
                foreach (var product in tid)
                {
                    items.Add(new KeyValuePair<string, string>(
                        product.Text,
                        product.Text
                    ));
                }
            }
            return this.Json(items);
        }

        [HttpPost]
        public JsonResult PMtid(string Vendor, string Device, string toolId, string pmFlag)
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();
            IEnumerable<PLCCheckModel> tid;
            if (!string.IsNullOrEmpty(toolId) && string.IsNullOrEmpty(Vendor))
            {
                tid = PLCCheckModel.GetPMToolidList((string)Session["UserLevelName"], toolId, pmFlag == null ? "1" : pmFlag);
            }
            else if (!string.IsNullOrEmpty(Vendor))
            {
                tid = PLCCheckModel.GetPMToolidList(Vendor, Device, toolId, pmFlag == null ? "1" : pmFlag);
            }
            else
                return null;

            if (tid.Count() > 0)
            {
                foreach (var product in tid)
                {
                    items.Add(new KeyValuePair<string, string>(
                        product.Name,
                        product.Name
                    ));
                }
            }
            return this.Json(items);
        }

        public JsonResult JsonPMConfig(int page, int rp)
        {
            IEnumerable<PMModel> ls = PMModel.PMingList();

            return Json(new
            {
                page = page,
                total = ls.Count(),
                rows = from c in ls.Skip((page - 1) * rp).Take(rp)
                       select c
            });
        }

        public JsonResult JsonPMWait(int page, int rp)
        {
            IEnumerable<PMModel> ls = PMModel.NotyetPMList();

            return Json(new
            {
                page = page,
                total = ls.Count(),
                rows = from c in ls.Skip((page - 1) * rp).Take(rp)
                       select c
            });
        }

        [HttpPost]
        public JsonResult CancelPM(string toolId, string selectType, string pmFlag, string memo, string bellow, string turning, string threeway, string handvalve, string bshead, string gasvalve, string lpipe, string entry, string inlet, string mixingbox, string reactor, string tank, string outlet, string tpipe, string pmlpipe, string upturning, string ms, string inlet_TP, string mixingbox_TP, string reactor_TP, string tank_TP, string outlet_TP, string tpipe_TP, string pmlpipe_TP, string upturning_TP, string ms_TP)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                string UsrID = (string)Session["UserName"];
                bool IsSuccess = false;
                string Msg = "PM失敗…";
                PMModel pm = new PMModel();
                try
                {
                    string DBMsg = pm.EditPM(toolId, selectType, pmFlag, memo, UsrID, bellow, turning, threeway, handvalve, bshead, gasvalve, lpipe, entry, inlet, mixingbox, reactor, tank, outlet, tpipe, pmlpipe, upturning, ms, inlet_TP, mixingbox_TP, reactor_TP, tank_TP, outlet_TP, tpipe_TP, pmlpipe_TP, upturning_TP, ms_TP);

                    if (!string.IsNullOrEmpty(DBMsg) && DBMsg.Contains("完成"))
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

        public ActionResult EditPM()
        {
            return View();
        }
        #endregion

        #region Alarm Report

        public ActionResult AlarmDeal(string StartDate, string EndDate, string ToolID, string AlarmMsg, string Closed, string Vendor, string gname, string output)
        {
            if (output != "export")
            {
                ViewBag.CBL = from c in StatusCountModel.IndexAllStatusList()
                              select new SelectListItem
                              {
                                  Value = c.vName,
                                  Text = c.vName
                              };
                //ViewBag.List = from c in AlarmReportModel.ReportList(StartDate == null ? DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm") : StartDate, EndDate == null ? DateTime.Now.ToString("yyyy-MM-dd HH:mm") : Convert.ToDateTime(EndDate).AddDays(1).ToString("yyyy-MM-dd HH:mm"), ToolID == null ? "*" : ToolID, AlarmMsg == null ? "*" : AlarmMsg, Closed == string.Empty ? null : Closed, Vendor == null ? "'CS250'" : Vendor).Take(1)
                //               select c;
                ViewData["DepartmentList"] = ListModel.GetDepartmentList();
                ViewBag.bar = "Report";

                using (tsmc14BDataContext db = new tsmc14BDataContext())
                {
                    ViewData["AlarmCount"] = (from row in db.vw_AlarmReport_AlarmCount select row).ToList();
                }


                return View();
            }
            else
            {
                byte[] file = AlarmReportModel.GetAlarmDealFile(StartDate, EndDate, ToolID, AlarmMsg, Closed, Vendor, gname, System.Configuration.ConfigurationManager.AppSettings["AlarmLevel"]);
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AlarmReport_" + DateTime.Now.ToShortDateString() + ".xlsx");
            }
        }

        public JsonResult JsonAlarmDeal(string StartDate, string EndDate, string ToolID, string AlarmMsg, string Closed, string Vendor, string gname, int page, int rp)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                StartDate = StartDate.Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();
                IEnumerable<AlarmReportModel> ls = AlarmReportModel.ReportList(StartDate == null ? DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm") : StartDate, EndDate == null ? DateTime.Now.ToString("yyyy-MM-dd HH:mm") : EndDate, ToolID == null ? "*" : ToolID, AlarmMsg == null ? "*" : AlarmMsg, Closed == string.Empty ? null : Closed, Vendor == null ? "'CS250'" : Vendor, gname, System.Configuration.ConfigurationManager.AppSettings["AlarmLevel"]);

                return Json(new
                {
                    page = page,
                    total = ls.Count(),
                    rows = from c in ls.Skip((page - 1) * rp).Take(rp)
                           select c
                });
            }
            else
                return null;
        }

        [HttpGet]
        public ActionResult EditAlarmReport(int AlarmID, string vName, string Sys, string toolid, string AlarmTime)
        {

            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                int _AlarmID = AlarmID;
                if (_AlarmID == 0000)
                {
                    _AlarmID = Convert.ToInt32(ListModel.GetAlarmId(toolid, AlarmTime)[0]);
                }

                AlarmReportModel arm = AlarmReportModel.GetAlarm(_AlarmID, vName, Sys, (string)Session["UserName"]);

                if (arm.Vendor != Session["UserLevelName"].ToString() && Session["UserLevel"].ToString() == "500")
                {
                    ViewBag.MessageType = "警告";
                    ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                    return View("Message");
                }

                return View(arm);
            }
            else
            {
                ViewBag.MessageType = "警告";
                ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                return View("Message");
            }
        }

        [HttpGet]
        public ActionResult EditPMHistroy(int pmHistoryID)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {

                PMModel arm2 = PMModel.GetPMHistroy(pmHistoryID);
                ViewBag.PmFlag = arm2.pmFlag;
                ViewBag.TPFlag = arm2.TemporaryPFlag;
                ViewBag.PPFlag = arm2.PumpingDone;
                return View(arm2);
            }
            else
            {
                ViewBag.MessageType = "警告";
                ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                return View("Message");
            }
        }

        [HttpPost]
        public ActionResult EditAlarmReport(AlarmReportModel arm)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                if (ModelState.IsValid)
                {
                    string DBMsg = arm.Update((string)Session["UserName"]);
                    if (string.IsNullOrEmpty(DBMsg))
                        ViewBag.Message = "修改完成…";
                    else
                        ViewBag.Message = DBMsg;
                }
                else
                    ViewBag.Message = "資料不正確…";

                return View(arm);
            }
            else
            {
                ViewBag.MessageType = "警告";
                ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                return View("Message");
            }
        }

        [HttpPost]
        public ActionResult EditPMHistroy(PMModel arm2)
        {
            if (!string.IsNullOrEmpty(arm2.pmHistoryID) && !string.IsNullOrEmpty((string)Session["UserName"]))
            {
                if (ModelState.IsValid)
                {
                    string DBMsg = arm2.Update(arm2.pmHistoryID, (string)Session["UserName"]);
                    if (string.IsNullOrEmpty(DBMsg))
                        ViewBag.Message = "修改完成…";
                    else
                        ViewBag.Message = DBMsg;
                }
                else
                    ViewBag.Message = "資料不正確…";

                return View(arm2);
            }
            else
            {
                ViewBag.MessageType = "警告";
                ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                return View("Message");
            }
        }
        #endregion

        #region Emergency

        public ActionResult Emergency(string Vendor)
        {
            //ViewBag.phoneCallStatus = AlarmNowModel.phoneCallStatus();

            IEnumerable<ListModel> codes = ListModel.GetPCstatusList();
            ViewBag.phoneCallStatus = from c in codes
                                      select new SelectListItem
                                      {
                                          Value = c.Value,
                                          Text = c.Text
                                      };

            IEnumerable<ListModel> codes2 = ListModel.GetGroupList(Vendor);
            ViewBag.Device = from c in codes2
                             select new SelectListItem
                             {
                                 Value = c.Value,
                                 Text = c.Text
                             };

            ViewBag.dtMircoLayout = EmergencyModel.GetMircoLayout();

            return View();
        }

        public JsonResult JsonCutOffQuery(string gName, int page, int rp)
        {
            IEnumerable<EmergencyModel> ls = EmergencyModel.EQDisconnectedList(gName);
            return Json(new
            {
                page = page,
                total = ls.Count(),
                rows = from c in ls.Skip((page - 1) * rp).Take(rp)
                       select c
            });
        }

        [HttpPost]
        public JsonResult JsonCutOffgName()
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            var tid = EmergencyModel.AreaAlarmList();

            if (tid.Count() > 0)
            {
                foreach (var product in tid)
                {
                    items.Add(new KeyValuePair<string, string>(
                        product.Value,
                        product.Text
                    ));
                }
            }

            return this.Json(items);
        }

        public JsonResult JsonGetVendorgName()
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            var tid = EmergencyModel.GetVendorgName();


            if (tid.Count() > 0)
            {
                foreach (var product in tid)
                {
                    items.Add(new KeyValuePair<string, string>(
                       product.SendTo,
                        product.SendTo
                    ));
                }
            }

            return this.Json(items);
        }

        [HttpPost]
        public JsonResult JsonSetPhoneCallStatus(bool statusNow)
        {
            bool IsSuccess = false;
            string Msg = "設定失敗!!";
            try
            {
                string DBMsg = string.Empty;
                DBMsg = EmergencyModel.ResetPhoneCallStatus(statusNow);

                IsSuccess = true;
                Msg = DBMsg;
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Msg = ex.Message;
            }
            return Json(new { IsSuccess = IsSuccess, Msg = Msg });
        }

        [HttpPost]
        public JsonResult JsonWriteToAlarm(string SendTo, string SendBody)
        {
            bool IsSuccess = false;
            string Msg = "傳送失敗!!";
            try
            {
                string DBMsg = string.Empty;
                DBMsg = EmergencyModel.WriteToAlarm(SendTo, SendBody);

                IsSuccess = true;
                Msg = DBMsg;
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Msg = ex.Message;
            }
            return Json(new { IsSuccess = IsSuccess, Msg = Msg });
        }

        #endregion

        #region EmergencyDetail

        public ActionResult EmergencyDetail_byArea()
        {
            return View();
        }

        public JsonResult JsonCutOffDeatil(string locArea, string Area, int page, int rp)
        {
            IEnumerable<EmergencyModel> ls = EmergencyModel.EQDisconnectedDeatil(locArea, Area);
            return Json(new
            {
                page = page,
                total = ls.Count(),
                rows = from c in ls.Skip((page - 1) * rp).Take(rp)
                       select c
            });
        }

        #endregion

        #region PM frequency

        public ActionResult Frequency(string vName, string groupName, string output)
        {
            if (output != "export")
            {
                return View();
            }
            else
            {
                string tempvName = string.Empty;
                string tempgroupName = string.Empty;
                if (vName == "undefined")
                {
                    tempvName = "Main";
                    tempgroupName = groupName;
                }
                else if (vName != "undefined")
                {
                    tempvName = vName;
                    tempgroupName = groupName;
                }
                string file = PMfrequencySetModel.GetPMfrequencyexportFile(tempvName, tempgroupName);
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PM History_" + DateTime.Now.ToShortDateString() + ".xlsx");
            }
        }

        public ActionResult FrequencyDetail(string tName)
        {

            var tName1 = tName;
            return View();
        }

        public ActionResult EditFrequency(string Vendor)
        {
            IEnumerable<ListModel> codes = ListModel.GetDepartmentList();
            ViewBag.Vendor = from c in codes
                             select new SelectListItem
                             {
                                 Value = c.Text,
                                 Text = c.Text
                             };

            IEnumerable<ListModel> codes2 = ListModel.GetGroupList(Vendor);
            ViewBag.Device = from c in codes2
                             select new SelectListItem
                             {
                                 Value = c.Value,
                                 Text = c.Text
                             };

            IEnumerable<ListModel> codes3 = ListModel.GetToolIDPIDList(Vendor);
            ViewBag.ToolID = from c in codes3
                             select new SelectListItem
                             {
                                 Value = c.Value,
                                 Text = c.Text
                             };


            //待處理中清單
            ViewBag.List = from c in PMModel.NotyetPMList()
                           select c;

            ViewBag.UserLevel = (string)Session["UserLevelName"];

            IEnumerable<ListModel> codes4 = ListModel.GetPMfrequencyList();
            ViewBag.frequency = from c in codes4
                                select new SelectListItem
                                {
                                    Value = c.Value,
                                    Text = c.Text
                                };

            return View();
        }

        [HttpPost]
        public JsonResult frequencyToolid(string Vendor)
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            var tid = ListModel.GetToolIDPIDList(Vendor);

            if (tid.Count() > 0)
            {
                foreach (var product in tid)
                {
                    items.Add(new KeyValuePair<string, string>(
                        product.Value,
                        product.Text
                    ));
                }
            }
            return this.Json(items);
        }

        [HttpPost]
        public JsonResult frequencygName(string Vendor)
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            var tid = ListModel.GetGroupList(Vendor);

            if (tid.Count() > 0)
            {
                foreach (var product in tid)
                {
                    items.Add(new KeyValuePair<string, string>(
                        product.Text,
                        product.Text
                    ));
                }
            }
            return this.Json(items);
        }

        [HttpPost]
        public JsonResult PMfrequencySet(string selectvendor, string selectplcid, string selecttoolid, string selectfrequency, int selectfreq_id)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                string UsrID = (string)Session["UserName"];
                bool IsSuccess = false;
                string Msg = "PM失敗…";
                PMfrequencySetModel pm = new PMfrequencySetModel();
                try
                {
                    string DBMsg = pm.EditPM(selectvendor, selectplcid, selecttoolid, selectfrequency, selectfreq_id, UsrID);

                    if (!string.IsNullOrEmpty(DBMsg) && DBMsg.Contains("完成"))
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

        public JsonResult JsonPMSchedule(int page, int rp)
        {
            IEnumerable<PMfrequencySetModel> ls = PMfrequencySetModel.PMFrequencyList();
            return Json(new
            {
                page = page,
                total = ls.Count(),
                rows = from c in ls.Skip((page - 1) * rp).Take(rp)
                       select c
            });
        }

        public JsonResult JsonEditPMSchedule(int rbn, string groupName, int page, int rp)
        {
            IEnumerable<PMfrequencySetModel> ls = PMfrequencySetModel.EditPMFrequencyList(groupName, rbn);
            return Json(new
            {
                page = page,
                total = ls.Count(),
                rows = from c in ls.Skip((page - 1) * rp).Take(rp)
                       select c
            });
        }

        public JsonResult JsonEditPMSchedulevName(string vName, string groupName, int rbn, int page, int rp)
        {
            IEnumerable<PMfrequencySetModel> ls = PMfrequencySetModel.EditPMFrequencyvNameList(vName, groupName, rbn);
            return Json(new
            {
                page = page,
                total = ls.Count(),
                rows = from c in ls.Skip((page - 1) * rp).Take(rp)
                       select c
            });
        }

        public JsonResult JsonPMFrequencyDetailList(String vName, String groupName, int page, int rp)
        {

            IEnumerable<PMfrequencySetModel> ls = PMfrequencySetModel.PMFrequencyDetailList(vName, groupName);
            return Json(new
            {
                page = page,
                total = ls.Count(),
                rows = from c in ls.Skip((page - 1) * rp).Take(rp)
                       select c
            });

        }

        [HttpGet]
        public ActionResult EditPMSchedule(string toolid, int frequency, int tolerance, string userlogin)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                IEnumerable<ListModel> codes4 = ListModel.GetPMfrequencyList();
                ViewBag.frequency = from c in codes4
                                    select new SelectListItem
                                    {
                                        Value = c.Value,
                                        Text = c.Text
                                    };
                if (frequency != 0 && tolerance != 0)
                {
                    PMfrequencySetModel arm = PMfrequencySetModel.GetPMSchedule(toolid, frequency, tolerance, (string)Session["UserName"]);
                    ViewBag.Title = "修改";
                    return View(arm);
                }
                else
                {
                    //ViewBag.MessageType = "警告";
                    //ViewBag.Message = "此機台未新增PM週期初始值，請新增PM週期初始值..";
                    //return View("Message");
                    PMfrequencySetModel arm = PMfrequencySetModel.GetPMSchedule(toolid, frequency, tolerance, (string)Session["UserName"]);
                    ViewBag.Title = "新增";
                    return View(arm);
                }
            }
            else
            {
                ViewBag.MessageType = "警告";
                ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                return View("Message");
            }
        }

        [HttpGet]
        public ActionResult AddPMFrequency(string toolid, int frequency, int tolerance, string userlogin)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                IEnumerable<ListModel> codes4 = ListModel.GetPMfrequencyList();
                ViewBag.frequency = from c in codes4
                                    select new SelectListItem
                                    {
                                        Value = c.Value,
                                        Text = c.Text
                                    };

                //ViewBag.MessageType = "警告";
                //ViewBag.Message = "此機台未新增PM週期初始值，請新增PM週期初始值..";
                //return View("Message");
                PMfrequencySetModel arm = PMfrequencySetModel.GetPMSchedule1(toolid, frequency, tolerance, (string)Session["UserName"]);
                ViewBag.Title = "新增";
                return View(arm);

            }
            else
            {
                ViewBag.MessageType = "警告";
                ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                return View("Message");
            }
        }

        [HttpGet]
        public ActionResult AddPMFrequency1(string toolid, int frequency, int tolerance, string userlogin)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                IEnumerable<ListModel> codes4 = ListModel.GetPMfrequencyList();
                ViewBag.frequency = from c in codes4
                                    select new SelectListItem
                                    {
                                        Value = c.Value,
                                        Text = c.Text
                                    };
                if (frequency != 0 && tolerance != 0)
                {
                    PMfrequencySetModel arm = PMfrequencySetModel.GetPMSchedule(toolid, frequency, tolerance, (string)Session["UserName"]);
                    ViewBag.Title = "修改";
                    return View(arm);
                }
                else
                {
                    //ViewBag.MessageType = "警告";
                    //ViewBag.Message = "此機台未新增PM週期初始值，請新增PM週期初始值..";
                    //return View("Message");
                    PMfrequencySetModel arm = PMfrequencySetModel.GetPMSchedule(toolid, frequency, tolerance, (string)Session["UserName"]);
                    ViewBag.Title = "新增";
                    return View(arm);
                }
            }
            else
            {
                ViewBag.MessageType = "警告";
                ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                return View("Message");
            }
        }

        [HttpPost]
        public JsonResult EditPMScheduleSet(string selecttoolid, int selectfrequency, int selecttolerance, string selectstatus, string selectpowerondate, int selectUnSetTooLate)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                bool IsSuccess = false;
                string Msg = "PM上限天數範圍或是格式有誤";
                if (selectUnSetTooLate >= 0 && selectUnSetTooLate <= 999)
                {
                    string UsrID = (string)Session["UserName"];
                    string status = string.Empty;
                    if (selectstatus == "ON")
                    {
                        status = "1";
                    }
                    else if (selectstatus == "OFF")
                    {
                        status = "0";
                    }

                    PMfrequencySetModel pm = new PMfrequencySetModel();
                    try
                    {
                        string DBMsg = pm.Update(selecttoolid, selectfrequency, selecttolerance, status, selectpowerondate, UsrID, selectUnSetTooLate);

                        if (!string.IsNullOrEmpty(DBMsg) && DBMsg.Contains("完成"))
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
                {
                    //return this.Json("PM上限天數範圍或是格式有誤");
                    return Json(new { IsSuccess = IsSuccess, Msg = Msg });
                }
            }
            else
            {
                //ViewBag.MessageType = "警告";
                //ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                return this.Json("您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…");
            }
        }

        [HttpPost]
        public JsonResult AddPMScheduleSet(string selectplcid, string selecttoolid, int selectfreqid, int selectfrequency, int selecttolerance, string selectstatus, string selectpowerondate, int selectUnSetTooLate)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                bool IsSuccess = false;
                string Msg = "PM上限天數範圍或是格式有誤";
                if (selectUnSetTooLate >= 0 && selectUnSetTooLate <= 999)
                {
                    string UsrID = (string)Session["UserName"];
                    string status = string.Empty;
                    if (selectstatus == "ON")
                    {
                        status = "1";
                    }
                    else if (selectstatus == "OFF")
                    {
                        status = "0";
                    }

                    PMfrequencySetModel pm = new PMfrequencySetModel();
                    try
                    {
                        string DBMsg = pm.addPM_Schedule(selectplcid, selecttoolid, selectfreqid, selectfrequency, selecttolerance, status, selectpowerondate, UsrID, selectUnSetTooLate);
                        //string DBMsg = string.Empty;
                        if (!string.IsNullOrEmpty(DBMsg) && DBMsg.Contains("完成"))
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
                {
                    //return this.Json("PM上限天數範圍或是格式有誤");
                    return Json(new { IsSuccess = IsSuccess, Msg = Msg });
                }

            }
            else
            {
                //ViewBag.MessageType = "警告";
                //ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                return this.Json("您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…");
            }
        }

        [HttpPost]
        public JsonResult AddPMScheduleSet1(string selecttoolid, int selectfreqid, int selectfrequency, int selecttolerance)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                string UsrID = (string)Session["UserName"];

                bool IsSuccess = false;
                string Msg = "PM失敗…";
                PMfrequencySetModel pm = new PMfrequencySetModel();
                try
                {
                    string DBMsg = pm.addPM_ScheduleALL(selecttoolid, selectfreqid, selectfrequency, selecttolerance, UsrID);
                    //string DBMsg = string.Empty;
                    if (!string.IsNullOrEmpty(DBMsg) && DBMsg.Contains("完成"))
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
            {
                //ViewBag.MessageType = "警告";
                //ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                return this.Json("您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…");
            }
        }

        #endregion

        #region Report

        public ActionResult Report()
        {
            ViewBag.bar = "Report";
            return View();
        }

        #endregion

        #region FDC_Setting

        public ActionResult FDC_Setting()
        {
            ViewBag.bar = "Setting";
            return View();
        }

        public ActionResult FTP_Setting()
        {
            return View();
        }

        public JsonResult JsonSensorTag(string StartDate, string EndDate, string ToolID, string AlarmMsg, string Closed, string Vendor, string gname, string searchtag, int page, int rp)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                using (tsmc14BDataContext db = new tsmc14BDataContext())
                {
                    var vw = from row in db.vw_SensorTag_info select row;

                    if (!string.IsNullOrEmpty(searchtag))
                    {
                        vw = from row in db.vw_SensorTag_info where row.chamber.ToUpper().Contains(searchtag.ToUpper()) select row;
                    }

                    IEnumerable<vw_SensorTag_info> ls = vw.ToList();

                    return Json(new
                    {
                        page = page,
                        total = ls.Count(),
                        rows = from c in ls.Skip((page - 1) * rp).Take(rp)
                               select c
                    });
                }
            }
            else
                return null;

        }

        public ActionResult EditSensorTag_info(String SensorId)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                SensorTagModel SensorTag;

                SensorTag = SensorTagModel.GetSensorTag(SensorId);

                return View(SensorTag);
            }
            else
            {
                ViewBag.MessageType = "警告";
                ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                return View("Message");
            }
        }

        [HttpPost]
        public ActionResult EditSensorTag_info(SensorTagModel s)
        {
            if (!string.IsNullOrEmpty(s.Sensor))
            {
                if (ModelState.IsValid)
                {
                    string DBMsg = SensorTagModel.Update(s);
                    if (string.IsNullOrEmpty(DBMsg))
                        ViewBag.Message = "修改完成…";
                    else
                        ViewBag.Message = DBMsg;
                }
                else
                    ViewBag.Message = "資料不正確…";

                return View(s);
            }
            else
            {
                ViewBag.MessageType = "警告";
                ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                return View("Message");
            }
        }

        #endregion

        #region Limit
        public ActionResult TypeValues()
        {
            IEnumerable<ListModel> type_info = ListModel.GetTypeInfoList();
            ViewBag.type_info = from c in type_info
                                select new SelectListItem
                                {
                                    Value = c.Value,
                                    Text = c.Text,
                                    Selected = c.Text.Trim() == Session["UserdepartmentName"].ToString()
                                };

            return View();
        }

        public ActionResult AlarmLimit(string tool, string output)
        {
            if (output != "export")
            {
                ViewBag.bar = "AlarmLimit";
                if (!string.IsNullOrEmpty((string)Session["UserName"]))
                {

                    IEnumerable<ListModel> department_name = ListModel.GetDepartmentList();
                    ViewBag.department_name = from c in department_name
                                              select new SelectListItem
                                              {
                                                  Value = c.Text,
                                                  Text = c.Text,
                                                  Selected = c.Text.Trim() == Session["UserdepartmentName"].ToString()
                                              };

                    IEnumerable<ListModel> Tool = ListModel.GetToolIDList(Session["UserdepartmentName"].ToString());
                    ViewBag.Tool = from c in Tool
                                   select new SelectListItem
                                   {
                                       Value = c.Text,
                                       Text = c.Text
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
            else
            {
                byte[] file = AlarmLimitModel.GetAlarmLimitFile(tool);
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AlarmLimit_" + tool + "_" + DateTime.Now.ToString("yyyy_MM_dd") + ".xlsx");
            }
        }

        public JsonResult JsonTool(string dName)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                IEnumerable<ListModel> Tool = ListModel.GetToolIDList(dName);

                return Json(Tool);
            }
            else
                return null;
        }

        public JsonResult JsonTypeValues(string type_id, int page, int rp)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                IEnumerable<TypeValuesModel> al = null;
                al = TypeValuesModel.GetTypeValuesList(type_id);

                return Json(new
                {
                    page = page,
                    total = al.Count(),
                    rows = from c in al.Skip((page - 1) * rp).Take(rp)
                           select c
                }, JsonRequestBehavior.AllowGet);
            }
            else
                return null;
        }

        public JsonResult JsonAlarmLimit(string tool, int page, int rp)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                IEnumerable<AlarmLimitModel> al = null;
                al = AlarmLimitModel.GetLimitList(tool);

                return Json(new
                {
                    page = page,
                    total = al.Count(),
                    rows = from c in al.Skip((page - 1) * rp).Take(rp)
                           select c
                });
            }
            else
                return null;
        }

        [HttpGet]
        public ActionResult EditTypeValues(string type_id, string Tag_Name)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                TypeValuesModel arm = TypeValuesModel.GetEditTypeValues(type_id, Tag_Name);

                return View(arm);
            }
            else
            {
                ViewBag.MessageType = "警告";
                ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                return View("Message");
            }
        }

        [HttpPost]
        public ActionResult EditTypeValues(TypeValuesModel arm)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                if (ModelState.IsValid)
                {
                    string DBMsg = arm.Update((string)Session["UserName"]);
                    if (string.IsNullOrEmpty(DBMsg))
                        ViewBag.Message = "修改完成…";
                    else
                        ViewBag.Message = DBMsg;
                }
                else
                    ViewBag.Message = "資料不正確…";

                return View(arm);
            }
            else
            {
                ViewBag.MessageType = "警告";
                ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                return View("Message");
            }
        }

        [HttpGet]
        public ActionResult EditLimit(string tagname)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {

                AlarmLimitModel arm = AlarmLimitModel.GetLimit(tagname);

                return View(arm);
            }
            else
            {
                ViewBag.MessageType = "警告";
                ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                return View("Message");
            }
        }

        [HttpPost]
        public ActionResult EditLimit(AlarmLimitModel arm)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                if (ModelState.IsValid)
                {
                    string DBMsg = arm.Update((string)Session["UserName"]);
                    if (string.IsNullOrEmpty(DBMsg))
                        ViewBag.Message = "修改完成…";
                    else
                        ViewBag.Message = DBMsg;
                }
                else
                    ViewBag.Message = "資料不正確…";

                return View(arm);
            }
            else
            {
                ViewBag.MessageType = "警告";
                ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                return View("Message");
            }
        }

        public ActionResult AlarmLimitHistory()
        {
            return View();
        }

        public JsonResult JsonAlarmLimitHistory(string tagname, int page, int rp)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                IEnumerable<AlarmLimitModel> al = null;
                al = AlarmLimitModel.GetLimitHistory(tagname);

                return Json(new
                {
                    page = page,
                    total = al.Count(),
                    rows = from c in al.Skip((page - 1) * rp).Take(rp)
                           select c
                });
            }
            else
                return null;
        }
        #endregion

        #region PhoneCall
        public ActionResult PhoneCall(string tool, string phoneCallType, string output)
        {
            if (output != "export")
            {
                if (!string.IsNullOrEmpty((string)Session["UserName"]))
                {
                    ViewBag.bar = "PhoneCall";
                    IEnumerable<ListModel> department_name = ListModel.GetDepartmentList();
                    ViewBag.department_name = from c in department_name
                                              select new SelectListItem
                                              {
                                                  Value = c.Text,
                                                  Text = c.Text,
                                                  Selected = c.Text.Trim() == Session["UserdepartmentName"].ToString()
                                              };

                    IEnumerable<ListModel> Tool = ListModel.GetToolIDList(Session["UserdepartmentName"].ToString());
                    ViewBag.Tool = from c in Tool
                                   select new SelectListItem
                                   {
                                       Value = c.Text,
                                       Text = c.Text
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
            else
            {
                byte[] file = PhoneCallModel.GetPhoneCallFile(tool, phoneCallType);
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ENS_" + tool + "_" + DateTime.Now.ToString("yyyy_MM_dd") + ".xlsx");
            }
        }

        public JsonResult JsonPhoneCall(string tool, string phonecalltype, int page, int rp)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                IEnumerable<PhoneCallModel> al = null;
                al = PhoneCallModel.GetPhoneCallList(tool, phonecalltype);

                return Json(new
                {
                    page = page,
                    total = al.Count(),
                    rows = from c in al.Skip((page - 1) * rp).Take(rp)
                           select c
                });
            }
            else
                return null;
        }

        [HttpPost]
        public JsonResult JsonPhoneCallSetting(string fulltagnames)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {

                string[] data = fulltagnames.Split(',');

                string result = PhoneCallModel.SetPhoneCallSetting(data, (string)Session["UserName"]);

                return Json(new
                {
                    message = result
                });
            }
            else
                return null;
        }

        [HttpGet]
        public ActionResult EditPhoneCall(string tagname)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {

                PhoneCallModel arm = PhoneCallModel.GetPhoneCall(tagname);

                return View(arm);
            }
            else
            {
                ViewBag.MessageType = "警告";
                ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                return View("Message");
            }
        }

        [HttpPost]
        public ActionResult EditPhoneCall(PhoneCallModel arm)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                if (ModelState.IsValid)
                {
                    string DBMsg = arm.Update((string)Session["UserName"]);
                    if (string.IsNullOrEmpty(DBMsg))
                        ViewBag.Message = "修改完成…";
                    else
                        ViewBag.Message = DBMsg;
                }
                else
                    ViewBag.Message = "資料不正確…";

                return View(arm);
            }
            else
            {
                ViewBag.MessageType = "警告";
                ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                return View("Message");
            }
        }

        public JsonResult JsonPhoneCallAllSwitch(string tool, bool Switch)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                string result = PhoneCallModel.SetAllSwitch(tool, Switch, (string)Session["UserName"]);

                return Json(new
                {
                    Message = result
                });
            }
            else
                return null;
        }

        public ActionResult PhoneCallHistory()
        {
            return View();
        }

        public JsonResult JsonPhoneCallHistory(string tagname, int page, int rp)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                IEnumerable<PhoneCallModel> al = null;
                al = PhoneCallModel.GetPhoneCallHistory(tagname);

                return Json(new
                {
                    page = page,
                    total = al.Count(),
                    rows = from c in al.Skip((page - 1) * rp).Take(rp)
                           select c
                });
            }
            else
                return null;
        }
        #endregion

        #region FDCPM
        public ActionResult FDCPM(string history,string dept,string tool,string startdate,string enddate, string output)
        {
            if (output != "export")
            {
                if (!string.IsNullOrEmpty((string)Session["UserName"]))
                {
                    ViewBag.bar = "FDCPM";
                    IEnumerable<ListModel> department_name = ListModel.GetDepartmentList();
                    ViewBag.department_name = from c in department_name
                                              select new SelectListItem
                                              {
                                                  Value = c.Text,
                                                  Text = c.Text,
                                                  Selected = c.Text.Trim() == Session["UserdepartmentName"].ToString()
                                              };

                    IEnumerable<ListModel> Tool = ListModel.GetToolIDList(Session["UserdepartmentName"].ToString());
                    ViewBag.Tool = from c in Tool
                                   select new SelectListItem
                                   {
                                       Value = c.Text,
                                       Text = c.Text
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
            else
            {
                byte[] file = FDCPMModel.GetFDCPMFile(history == "1", dept,startdate,enddate,tool);
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Delayed_Upload_" + DateTime.Now.ToString("yyyy_MM_dd") + ".xlsx");
            }
        }

        public JsonResult JsonFDCPM(string history,string dept,string tool,string startdate,string enddate,int page, int rp)
        {
            IEnumerable<FDCPMModel> al = FDCPMModel.GetFDCPMList(history == "1", dept, startdate, enddate, tool);

            return Json(new
            {
                page = page,
                total = al.Count(),
                rows = from c in al.Skip((page - 1) * rp).Take(rp)
                       select c
            });

        }
        #endregion
    }
}
