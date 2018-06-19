using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using WebCMS.Models;
using WebCMS.Areas.Main.Models;
using System.Web.UI.DataVisualization.Charting;

namespace WebCMS.Areas.Main.Controllers
{
    public class MachineGroupController : Controller
    {

        #region 設備數值資料統計
        public ActionResult PLCCheck(string vName, string Device, string ToolID, string TagName, string Typeid, string StartDate, string EndDate, string flag, string output)
        {
            if (output != "export")
            {
                IEnumerable<ListModel> codes2 = ListModel.GetGroupList(vName == null ? Request["vName"] : vName);
                ViewBag.Device = from c in codes2
                                 select new SelectListItem
                                 {
                                     Value = c.Value,
                                     Text = c.Text
                                 };
                IEnumerable<ListModel> codes3 = ListModel.GetToolIDList(vName == null ? Request["vName"] : vName, Device == null ? null : Device);
                ViewBag.ToolID = from c in codes3
                                 select new SelectListItem
                                 {
                                     Value = c.Value,
                                     Text = c.Text
                                 };
                if (!string.IsNullOrEmpty(ToolID))
                {
                    IEnumerable<ListModel> codes4 = ListModel.GetTagNameList(vName, ToolID == null ? null : ToolID);
                    ViewBag.TagName = from c in codes4
                                      select new SelectListItem
                                      {
                                          Value = c.Value,
                                          Text = c.Text
                                      };
                }

                if (vName != "DPM")
                {
                    IEnumerable<ListModel> codes5 = ListModel.GetTypeByVname(vName == null ? Request["vName"] : vName, Device == null ? null : Device);
                    ViewBag.Type = from c in codes5
                                   select new SelectListItem
                                   {
                                       Value = c.Value,
                                       Text = c.Text
                                   };
                }

                if (!string.IsNullOrEmpty(Typeid))
                {
                    IEnumerable<ListModel> codes7 = ListModel.GetToolidByType(vName == null ? Request["vName"] : vName, Typeid);
                    ViewBag.ToolID2 = from c in codes7
                                      select new SelectListItem
                                      {
                                          Value = c.Value,
                                          Text = c.Text
                                      };
                    IEnumerable<ListModel> codes6 = ListModel.GetTagByType(vName == null ? Request["vName"] : vName, Typeid);
                    ViewBag.TagName2 = from c in codes6
                                       select new SelectListItem
                                       {
                                           Value = c.Value,
                                           Text = c.Text
                                       };
                }
                return View();
            }
            else
            {
                if (flag == "1")
                {
                    byte[] file = PLCCheckModel.GetChartValueFile(vName, ToolID, TagName, StartDate, EndDate, flag);
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlPathEncode("設備數值資料_" + vName + "_" + ToolID + "_" + DateTime.Now.ToShortDateString() + ".xlsx"));
                }
                else if (flag == "2")
                {
                    byte[] file = PLCCheckModel.GetChartValueFile2(vName, ToolID, TagName, StartDate, EndDate, flag);
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlPathEncode("設備數值資料_" + vName + "_" + DateTime.Now.ToShortDateString() + ".xlsx"));
                }
                else
                {
                    byte[] file = PLCCheckModel.GetChartValueFile3(vName, ToolID, TagName, StartDate, EndDate, flag);
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlPathEncode("設備數值資料_" + vName + "_" + DateTime.Now.ToShortDateString() + ".xlsx"));
                }
            }

        }

        [HttpPost]
        public JsonResult JsonGettid(string Vendor, string Device)
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            var tid = ListModel.GetToolIDList(Vendor == null ? Request["vName"] : Device, null);

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
        public JsonResult JsonGettid2(string Vendor, string Typeid)
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            var tid = ListModel.GetToolidByType(Vendor == null ? Request["vName"] : Vendor, Typeid);

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
        public JsonResult JsonGetTag(string Vendor, string Device, string ToolID)
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            var tid = ListModel.GetTagNameList(Vendor, ToolID == null ? null : ToolID);

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
        public JsonResult JsonGetTypeid(string Vendor, string Device)
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            var tid = ListModel.GetTypeByVname(Vendor == null ? Request["vName"] : Vendor, Device == null ? null : Device);

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
        public JsonResult JsonGetTag2(string Vendor, string Typeid)
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            var tid = ListModel.GetTagByType(Vendor == null ? Request["vName"] : Vendor, Typeid);

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

        public ActionResult MyChart(string vName, string toolId, string TagName, string StartDate, string EndDate, string flag, string Y1Y2, bool isLimit = false, double? YMax = null, double? YMin = null, bool isStack = false)
        {
            Chart chart;
            if (flag == "1")
            {
                chart = PLCCheckModel.GetChartValue(vName, toolId, TagName, StartDate, EndDate, flag);
            }
            else if (flag == "2")
            {
                chart = PLCCheckModel.GetChartValue2(vName, toolId, TagName, StartDate, EndDate, flag);
            }
            else if (flag == "3")
            {
                chart = PLCCheckModel.GetChartValue3(vName, toolId, TagName, StartDate, EndDate, flag, Y1Y2, isLimit, YMax, YMin, isStack);
            }
            else
            {
                chart = PLCCheckModel.GetChartValue4(vName, toolId, TagName, StartDate, EndDate, flag);
            }

            chart.Customize += chart_Customize;

            MemoryStream ms = new MemoryStream();
            chart.SaveImage(ms, ChartImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);

            return File(ms, "image/png");
        }

        void chart_Customize(object sender, EventArgs e)
        {
            Session["chartYMax"] = ((Chart)sender).ChartAreas[0].AxisY.Maximum;
            Session["chartYMin"] = ((Chart)sender).ChartAreas[0].AxisY.Minimum;

            DateTime chartXMax = DateTime.FromOADate(((Chart)sender).ChartAreas[0].AxisX.Maximum);
            DateTime chartXMin = DateTime.FromOADate(((Chart)sender).ChartAreas[0].AxisX.Minimum);

            //chartXMax= chartXMax.AddDays();
            //chartXMin= chartXMin.AddDays(((Chart)sender).ChartAreas[0].AxisX.Minimum);

            Session["chartXMax"] = chartXMax.ToString("yyyy/MM/dd HH:mm:ss.ffff");
            Session["chartXMin"] = chartXMin.ToString("yyyy/MM/dd HH:mm:ss.ffff");
            Session["chartPositionX"] = ((Chart)sender).ChartAreas[0].InnerPlotPosition.X;
            Session["chartPositionY"] = ((Chart)sender).ChartAreas[0].InnerPlotPosition.Y;
            Session["chartPositionBottom"] = ((Chart)sender).ChartAreas[0].InnerPlotPosition.Bottom;
            Session["chartPositionRight"] = ((Chart)sender).ChartAreas[0].InnerPlotPosition.Right;
        }

        public ActionResult MyChart2(string vName, string toolId, string TagName, string StartDate, string EndDate, string flag, string Y1Y2)
        {
            var chart = PLCCheckModel.GetChart3(vName, toolId, TagName, StartDate, EndDate, flag, Y1Y2);
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            result.Append(getChartImage(chart));
            result.Append(chart.GetHtmlImageMap("ImageMap"));
            return Content(result.ToString());
        }

        private string getChartImage(Chart chart)
        {
            using (var stream = new MemoryStream())
            {
                string img = "<img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap'>";
                chart.SaveImage(stream, ChartImageFormat.Png);
                string encoded = Convert.ToBase64String(stream.ToArray());
                return String.Format(img, encoded);
            }
        }

        public ActionResult MyChart3(string vName, string toolId, string TagName, string StartDate, string EndDate, string flag, string Y1Y2, bool isLimit = false, bool isStack = false)
        {
            ViewData["vName"] = vName;
            ViewData["toolId"] = toolId;
            ViewData["TagName"] = TagName;
            ViewData["StartDate"] = DateTime.Parse(StartDate).ToString("yyyy/MM/dd HH:mm:ss");
            ViewData["EndDate"] = DateTime.Parse(EndDate).ToString("yyyy/MM/dd HH:mm:ss");
            ViewData["flag"] = flag;
            ViewData["Y1Y2"] = Y1Y2;
            ViewData["isLimit"] = isLimit;
            ViewData["isStack"] = isStack;
            return View();
        }

        public ActionResult MyChart4(string vName, string toolId, string TagName, string StartDate, string EndDate, string flag, string Y1Y2, bool isLimit = false, bool isStack = false)
        {
            ViewData["vName"] = vName;
            ViewData["toolId"] = toolId;
            ViewData["TagName"] = TagName;
            ViewData["StartDate"] = DateTime.Parse(StartDate).ToString("yyyy/MM/dd HH:mm:ss");
            ViewData["EndDate"] = DateTime.Parse(EndDate).ToString("yyyy/MM/dd HH:mm:ss");
            ViewData["flag"] = flag;
            ViewData["Y1Y2"] = Y1Y2;
            ViewData["isLimit"] = isLimit;
            ViewData["isStack"] = isStack;

            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                ViewData["FDCPMHistoryList"] = (from row in db.vw_FDCPMHistory where DateTime.Parse(StartDate) < row.FDCPMTime && DateTime.Parse(EndDate) > row.builtdate select row).ToList();
            }

            return View();
        }

        public ActionResult MyChart5(string vName, string toolId, string TagName, string StartDate, string EndDate, string flag, string Y1Y2, bool isLimit = false, bool isStack = false)
        {
            ViewData["vName"] = vName;
            ViewData["toolId"] = toolId;
            ViewData["TagName"] = TagName;
            ViewData["StartDate"] = DateTime.Parse(StartDate).ToString("yyyy/MM/dd HH:mm:ss");
            ViewData["EndDate"] = DateTime.Parse(EndDate).ToString("yyyy/MM/dd HH:mm:ss");
            ViewData["flag"] = flag;
            ViewData["Y1Y2"] = Y1Y2;
            ViewData["isLimit"] = isLimit;
            ViewData["isStack"] = isStack;
            return View();
        }

        public JsonResult JsonMyChart(string vName, string toolId, string TagName, string StartDate, string EndDate, string flag, string Y1Y2, bool isLimit = false, double? YMax = null, double? YMin = null, bool isStack = false)
        {
            Chart chart;

            if (flag == "1")
            {
                chart = PLCCheckModel.GetChartValue(vName, toolId, TagName, StartDate, EndDate, flag);
            }
            else if (flag == "2")
            {
                chart = PLCCheckModel.GetChartValue2(vName, toolId, TagName, StartDate, EndDate, flag);
            }
            else if (flag == "3")
            {
                chart = PLCCheckModel.GetChartValue3(vName, toolId, TagName, StartDate, EndDate, flag, Y1Y2, isLimit, YMax, YMin, isStack);
            }
            else
            {
                chart = PLCCheckModel.GetChartValue4(vName, toolId, TagName, StartDate, EndDate, flag);
            }

            chart.Customize += chart_Customize;

            string imgCode = PLCCheckModel.getChartEnCode(chart);

            string ImageMap = chart.GetHtmlImageMap("ChartImageMap");

            return Json(new
            {
                ImgCode = imgCode,
                ImageMap = ImageMap,
                YMax = Session["chartYMax"],
                YMin = Session["chartYMin"],
                XMax = Session["chartXMax"],
                XMin = Session["chartXMin"],
                PositionX = Session["chartPositionX"],
                PositionY = Session["chartPositionY"],
                PositionBottom = Session["chartPositionBottom"],
                PositionRight = Session["chartPositionRight"],
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 設備警告資料查詢
        public ActionResult AlarmCheck(string vName, string fromdate, string todate, string ToolID, string Device, string onceadayFlag, string output)
        {

            if (output != "export")
            {
                IEnumerable<ListModel> codes2 = ListModel.GetGroupList(vName == null ? Request["vName"] : vName);
                ViewBag.Device = from c in codes2
                                 select new SelectListItem
                                 {
                                     Value = c.Value,
                                     Text = c.Text
                                 };
                IEnumerable<ListModel> codes3 = ListModel.GetToolIDList(vName == null ? Request["vName"] : vName, Device == null ? null : Device);
                ViewBag.ToolID = from c in codes3
                                 select new SelectListItem
                                 {
                                     Value = c.Value,
                                     Text = c.Text
                                 };

                return View();
            }
            else
            {
                byte[] file = null;
                if (vName == "DPM")
                {
                    file = AlarmCheckModel.GetAlarmExportFile(vName, fromdate, todate, ToolID, Device, onceadayFlag, "810");
                }
                else
                {
                    file = AlarmCheckModel.GetAlarmExportFile(vName, fromdate, todate, ToolID, Device, onceadayFlag, ConfigurationManager.AppSettings["AlarmLevel"]);
                }

                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlPathEncode("設備警告資料查詢_" + DateTime.Now.ToShortDateString() + ".xlsx"));
            }
        }

        [HttpPost]
        public JsonResult JsonAlarmCheck(string Vendor, string Device)
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            var tid = ListModel.GetToolIDList(Vendor == null ? Request["vName"] : Vendor, Device == null ? null : Device);

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

        public JsonResult AlarmCheckList(string vName, string toolId, string Device, string fromDate, string toDate, string onceaday, string output, int page, int rp)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                IEnumerable<AlarmNowModel> al = AlarmCheckModel.GetAlarmList(vName, fromDate, toDate, toolId, Device, onceaday, vName != "DPM" ? ConfigurationManager.AppSettings["AlarmLevel"] : "810");

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

        #region 設備上下限

        public ActionResult PLCCheckLimit(string vName, string ToolID)
        {
            if (vName == "CS250")
            {
                IEnumerable<ListModel> codes3 = ListModel.GetToolIDList(vName == null ? Request["vName"] : vName);
                ViewBag.ToolID = from c in codes3
                                 select new SelectListItem
                                 {
                                     Value = c.Value,
                                     Text = c.Text
                                 };
                if (!string.IsNullOrEmpty(ToolID))
                {
                    IEnumerable<ListModel> codes4 = ListModel.GetTagNameList(vName, ToolID);
                    ViewBag.TagName = from c in codes4
                                      select new SelectListItem
                                      {
                                          Value = c.Value,
                                          Text = c.Text
                                      };
                }

                return View();
            }
            else
            {
                ViewBag.MessageType = "警告";
                ViewBag.Message = Request["vName"] + "無本功能，如有需增加請洽管理員，謝謝…";
                return View("Message");
            }
        }

        public JsonResult JsonPLCCheckLimit(int page, int rp)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                IEnumerable<AlarmCheckModel> al = null;

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

        #region 設備群組狀態顯示
        public ActionResult MachineStatus(string vName)
        {
            return View(MachineStatusModel.GetMachineStatus(vName));
        }

        public ActionResult StatusDetail(string vName, string gName)
        {
            IEnumerable<ListModel> codes2 = ListModel.GetToolIDList(vName == null ? "CS250" : vName, gName);

            ViewBag.ToolID = from c in codes2
                             select new SelectListItem
                             {
                                 Value = c.Value,
                                 Text = c.Text
                             };
            return View(StatusDetialModel.GetStatusDetial(vName, gName));
        }

        public JsonResult JsonStatusDetail(string vName, string gName, string Status, string toolId, int page, int rp)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                vName = vName.Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();
                IEnumerable<StatusDetialModel> ls = null;
                if (string.IsNullOrEmpty(Status) && string.IsNullOrEmpty(toolId))
                {
                    ls = StatusDetialModel.GetStatusDetial(vName, gName);
                }
                else
                {
                    ls = StatusDetialModel.GetStatusDetial(vName, gName, Status, toolId);
                }

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

        //單一設備歷史警告資料顯示
        public ActionResult SingleAlarmList()
        {
            return View();
        }
        public JsonResult SingleAlarmJson(string Toolid, int page, int rp)
        {
            IEnumerable<SingleAlarmModel> ls = SingleAlarmModel.GetSingleAlarm(Toolid);

            return Json(new
                    {
                        page = page,
                        total = ls.Count(),
                        rows = from c in ls.Skip((page - 1) * rp).Take(rp)
                               select c
                    });
        }
        #endregion

        #region 設備數值資料顯示
        public ActionResult MachineValue(string vName, string Device, string Toolid, string output, string ValueStyle)
        {
            IEnumerable<ListModel> codes2 = ListModel.GetGroupList(vName == null ? Request["vName"] : vName);
            ViewBag.Device = from c in codes2
                             select new SelectListItem
                             {
                                 Value = c.Value,
                                 Text = c.Text
                             };
            IEnumerable<ListModel> codes3 = ListModel.GetToolIDList(vName == null ? Request["vName"] : vName, Device == null ? null : Device);
            ViewBag.ToolID = from c in codes3
                             select new SelectListItem
                             {
                                 Value = c.Value,
                                 Text = c.Text
                             };

            if (output == "search")
            {
                if (vName == "DAS" && ValueStyle == string.Empty && Toolid == string.Empty)
                {

                    ViewBag.dtMachineValue_R = MachineValueModel.GetNowMachineValue(vName == null ? Request["vName"] : vName, Device, Toolid == "請選擇" ? string.Empty : Toolid, "R");
                    if (ViewBag.dtMachineValue_R == null || ViewBag.dtMachineValue_R.Rows.Count == 0)
                    {
                        ViewBag.dtMachineValue_R = null;
                    }
                    ViewBag.dtMachineValue_L = MachineValueModel.GetNowMachineValue(vName == null ? Request["vName"] : vName, Device, Toolid == "請選擇" ? string.Empty : Toolid, "L");
                    if (ViewBag.dtMachineValue_R == null || ViewBag.dtMachineValue_L.Rows.Count == 0)
                    {
                        ViewBag.dtMachineValue_L = null;
                    }
                    ViewBag.dtMachineValue = MachineValueModel.GetNowMachineValueDASAll(vName == null ? Request["vName"] : vName, Device, Toolid == "請選擇" ? string.Empty : Toolid, ValueStyle);
                }
                else
                {
                    ViewBag.dtMachineValue = MachineValueModel.GetNowMachineValue(vName == null ? Request["vName"] : vName, Device, Toolid == "請選擇" ? string.Empty : Toolid, ValueStyle);
                }
                ViewBag.gName = Request["vName"];
            }
            else if (output == "export")
            {
                byte[] file = MachineValueModel.GetNowMachineValueFile(vName == null ? Request["vName"] : vName, Device, Toolid == "請選擇" ? string.Empty : Toolid, ValueStyle);
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlPathEncode("設備數值資料_" + vName + "_" + DateTime.Now.ToShortDateString() + ".xlsx"));
            }
            return View();
        }

        [HttpPost]
        public JsonResult JsonMachineValue(string Vendor, string Device)
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            var tid = ListModel.GetToolIDList(Vendor == null ? Request["vName"] : Vendor, Device == null ? null : Device);

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
        #endregion

        #region 設備選項設定
        public ActionResult Settings(string vName, string Device, string Typeid, string Toolid)
        {
            //隔離LifeTime
            IEnumerable<ListModel> codes2 = ListModel.GetGroupList(vName == null ? Request["vName"] : vName);
            ViewBag.Device = from c in codes2
                             select new SelectListItem
                             {
                                 Value = c.Value,
                                 Text = c.Text
                             };
            if (!string.IsNullOrEmpty(vName == null ? Request["vName"] : vName))
            {
                IEnumerable<ListModel> codes1 = ListModel.GetTypeByVname2Id(vName == null ? Request["vName"] : vName, Device == null ? null : Device);
                ViewBag.TypeID = from c in codes1
                                 select new SelectListItem
                                 {
                                     Value = c.Value,
                                     Text = c.Text
                                 };
            }
            IEnumerable<ListModel> codes3 = ListModel.GetToolIDList(vName == null ? Request["vName"] : vName, Device == null ? null : Device);
            ViewBag.ToolID = from c in codes3
                             select new SelectListItem
                             {
                                 Value = c.Value,
                                 Text = c.Text
                             };

            //Alarm 對應數值設定
            ViewBag.TypeList = from c in AlarmTagNameModel.TypeList(Request["vName"])
                               select new SelectListItem
                               {
                                   Value = c.Typeid,
                                   Text = c.TName
                               };

            ViewBag.AlarmList = from c in AlarmTagNameModel.AlarmList(Request["vName"], string.Empty)
                                select new SelectListItem
                                {
                                    Value = c.AlarmValue,
                                    Text = c.AlarmText
                                };

            ViewBag.TagList = from c in AlarmTagNameModel.TagList(Request["vName"], string.Empty)
                              select new SelectListItem
                              {
                                  Value = c.TagValue,
                                  Text = c.TagText
                              };

            return View();
        }

        [HttpPost]
        public JsonResult JsonTypeCheck(string Vendor, string typeid)
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            var tid = AlarmTagNameModel.TagList(Vendor, typeid);

            if (tid.Count() > 0)
            {
                foreach (var product in tid)
                {
                    items.Add(new KeyValuePair<string, string>(
                        product.TagValue,
                        product.TagText
                    ));
                }
            }

            return this.Json(items);
        }

        [HttpPost]
        public JsonResult JsonTypeCheck2(string Vendor, string typeid)
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            var tid = AlarmTagNameModel.AlarmList(Vendor, typeid);

            if (tid.Count() > 0)
            {
                foreach (var product in tid)
                {
                    items.Add(new KeyValuePair<string, string>(
                        product.AlarmValue,
                        product.AlarmText
                    ));
                }
            }

            return this.Json(items);
        }

        //隔離LifeTime
        [HttpPost]
        public JsonResult JsonPMLifeTimetid(string Vendor, string Device)
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            var tid = ListModel.GetToolIDList(Vendor == null ? Request["vName"] : Vendor, Device == null ? null : Device);

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
        public JsonResult JsonPMLifeTime(string Option, string Toolid, string Typeid, int page, int rp)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                IEnumerable<PMLiftTimeModel> ls = PMLiftTimeModel.PMLiftTimeList(Option, Toolid, Typeid);
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

        [HttpPost]
        public JsonResult JsonSetPMLifeTime(string Option, string Toolid, string Typeid, string PmLT, string Reset)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                string UsrID = (string)Session["UserName"];
                bool IsSuccess = false;
                string Msg = "設定失敗…";
                PMModel pm = new PMModel();
                try
                {
                    if (Option == "1")
                    {
                        Toolid = string.Empty;
                    }
                    else
                    {
                        Typeid = string.Empty;
                    }
                    string DBMsg = string.Empty;
                    if (Reset == "Reset")
                    {
                        DBMsg = PMLiftTimeModel.ResetPMLiftTime(Toolid, Typeid);
                    }
                    else
                    {
                        DBMsg = PMLiftTimeModel.EditPMLiftTime(Toolid, Typeid, PmLT);
                    }

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

        //Alarm 對應數值設定
        public JsonResult JsonAlarmTagName(string vName, int page, int rp)
        {
            IEnumerable<AlarmTagNameModel> ls = AlarmTagNameModel.EQAlarmTagName(vName);
            return Json(new
            {
                page = page,
                total = ls.Count(),
                rows = from c in ls.Skip((page - 1) * rp).Take(rp)
                       select c
            });
        }

        [HttpPost]
        public JsonResult JsonAlermTagExe(int SeqNO, string Comment, string Tag_Name, string Type)
        {

            bool IsSuccess = false;
            string Msg = "失敗…";
            AlarmTagNameModel atn = new AlarmTagNameModel();
            try
            {
                string DBMsg = null;
                if (Type == "Insert")
                {
                    foreach (var Name in Tag_Name.Split(';'))
                    {
                        DBMsg += atn.Insert(Comment, Name.Split(',')[0], Name.Split(',')[1]) + "\n";
                    }
                }
                else if (Type == "delete")
                {
                    DBMsg = atn.Delete(SeqNO);
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

        [HttpGet]
        public ActionResult EditAlarmTagName(string vName, int SeqNO)
        {
            ViewBag.TypeList = from c in AlarmTagNameModel.TypeList(Request["vName"])
                               select new SelectListItem
                               {
                                   Value = c.Typeid,
                                   Text = c.TName
                               };

            ViewBag.TagList = from c in AlarmTagNameModel.TagList(vName, string.Empty)
                              select new SelectListItem
                              {
                                  Value = c.TagValue,
                                  Text = c.TagText
                              };

            AlarmTagNameModel atn = AlarmTagNameModel.Get(SeqNO);
            return View(atn);
        }

        [HttpPost]
        public ActionResult EditAlarmTagName(AlarmTagNameModel atn)
        {
            string DBMsg = atn.Update();
            if (string.IsNullOrEmpty(DBMsg))
                ViewBag.Message = "修改完成…";
            else
                ViewBag.Message = DBMsg;

            ViewBag.TypeList = from c in AlarmTagNameModel.TypeList(Request["vName"])
                               select new SelectListItem
                               {
                                   Value = c.Typeid,
                                   Text = c.TName
                               };

            ViewBag.TagList = from c in AlarmTagNameModel.TagList("DAS", string.Empty)
                              select new SelectListItem
                              {
                                  Value = c.TagValue,
                                  Text = c.TagText
                              };

            return View(atn);
        }

        #endregion

        #region Demo

        public ActionResult PLCCheck2(string vName, string type, string Device, string ToolID, string TagName, string Typeid, string StartDate, string EndDate, string flag, string output)
        {
            if (output != "export")
            {
                using (tsmc14BDataContext db = new tsmc14BDataContext())
                {
                    List<vw_eq_status> codes2 = (from row in db.vw_eq_status where row.department_name == vName orderby row.process_name, row.EQID, row.chamber, row.WEB_code descending select row).ToList();
                    ViewData["Device"] = codes2;
                    ViewData["bar"] = vName;
                    ViewData["process_name"] = (from row in codes2 select row.process_name).Distinct();
                }

                return View();
            }
            else
            {
                if (flag == "1")
                {
                    byte[] file = PLCCheckModel.GetChartValueFile(vName, ToolID, TagName, StartDate, EndDate, flag);
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlPathEncode("設備數值資料_" + vName + "_" + ToolID + "_" + DateTime.Now.ToShortDateString() + ".xlsx"));
                }
                else if (flag == "2")
                {
                    byte[] file = PLCCheckModel.GetChartValueFile2(vName, ToolID, TagName, StartDate, EndDate, flag);
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlPathEncode("設備數值資料_" + vName + "_" + DateTime.Now.ToShortDateString() + ".xlsx"));
                }
                else
                {
                    byte[] file = PLCCheckModel.GetChartValueFile3(vName, ToolID, TagName, StartDate, EndDate, flag);
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlPathEncode("設備數值資料_" + ToolID + "_" + DateTime.Now.ToShortDateString() + ".xlsx"));
                }
            }
        }

        public ActionResult Accessory(int pid, string vName, string Device, string ToolID, string TagName, string Typeid, string StartDate, string EndDate, string flag, string output)
        {
            if (output != "export")
            {
                vw_eq_status codes2; //PUMP

                using (tsmc14BDataContext db = new tsmc14BDataContext())
                {
                    codes2 = (from row in db.vw_eq_status where row.pid == pid select row).FirstOrDefault();

                    PM_Set PS = (from row in db.PM_Set where row.tool_id == codes2.chamberName select row).SingleOrDefault();

                    if (PS != null)
                    {
                        ViewData["IsPM"] = PS.pmFlag ? 2 : 3;
                    }
                    else
                    {
                        ViewData["IsPM"] = codes2.sid == 8 ? 0 : 1;
                    }

                    ViewBag.plcId = pid;
                    ViewBag.WEB_code = codes2.WEB_code;

                    ViewBag.Is7DayUp = codes2.built_date.AddDays(7) <= DateTime.Now;
                    ViewBag.pid = codes2.pid;
                    ViewBag.AccessoryType = codes2.WEB_code;
                    ViewBag.ToolID = codes2.chamberName;
                    ViewBag.plc_name = codes2.plc_name;
                    ViewBag.vName = codes2.department_name;
                    ViewBag.bar = codes2.department_name;
                    ViewBag.tImageUrl = "/images/" + codes2.tName + ".jpg";
                    //ViewBag.IsValueNow = db.vw_eq_valuenow.Where(x => x.pid == pid).Any();
                    ViewBag.IsValueNow = (from row in db.vw_eq_valuenow.AsParallel() where row.pid == pid select row).Any();
                    ViewBag.IsAuth = Session["UserdepartmentName"] != null && ((string)Session["UserdepartmentName"] == codes2.department_name && int.Parse(Session["UserLevel"].ToString()) >= 600) || (Session["UserLevel"] != null && int.Parse(Session["UserLevel"].ToString()) >= 900);
                    ViewBag.tName = codes2.tName;
                    ViewBag.IOGNO = codes2.channel_name;
                    ViewBag.chamber = codes2.chamber;

                    //ViewBag.LastFDCUploadTime = codes2.LastCaptureTime.ToString("yyyy-MM-dd HH:mm");
                    ViewBag.nextFDCUploadTime = codes2.LastCaptureTime > DateTime.Now.AddMinutes(2) ? "下次資料收集時間延至:" + codes2.LastCaptureTime.ToString("yyyy-MM-dd HH:mm") : "";
                    List<vw_eq_status> OtherAccessory = (from row in db.vw_eq_status where row.department_name == codes2.department_name && row.chamber == codes2.chamber && row.toolid == codes2.toolid && row.pid != pid select row).ToList();
                    ViewData["OtherAccessory"] = OtherAccessory;
                }

                return View();
            }
            else
            {
                if (flag == "1")
                {
                    //byte[] file = PLCCheckModel.GetChartValueFile(vName, ToolID, TagName, StartDate, EndDate, flag);
                    byte[] file = AlarmCheckModel.GetAlarmExportFile(vName, StartDate, EndDate, ToolID, Device, "0", "800");
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlPathEncode("24小時警告資料_" + ToolID + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx"));
                }
                else if (flag == "2")
                {
                    //byte[] file = PLCCheckModel.GetChartValueFile2(vName, ToolID, TagName, StartDate, EndDate, flag);
                    byte[] file = AlarmCheckModel.GetAlarmExportFile(vName, StartDate, EndDate, ToolID, Device, "0", "800");
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlPathEncode("一週警告資料_" + ToolID + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx"));
                }
                else
                {
                    byte[] file = PLCCheckModel.GetChartValueFile3(vName, ToolID, TagName, StartDate, EndDate, flag);
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlPathEncode("設備數值資料_" + vName + "_" + DateTime.Now.ToShortDateString() + ".xlsx"));
                }
            }
        }

        public JsonResult JsonEQAlarm(string ToolID, int page, int rp)
        {
            IEnumerable<AlarmNowModel> ls = AlarmNowModel.EQAlarmNowList(ConfigurationManager.AppSettings["AlarmLevel"], 0, 0, ToolID);
            return Json(new
            {
                page = page,
                total = ls.Count(),
                rows = from c in ls.Skip((page - 1) * rp).Take(rp)
                       select c
            });
        }

        public JsonResult JsonEQFDCPM(int pid, DateTime delayFDCUploadTime, bool SetFlag, string memo)
        {
            string resultString = string.Empty;
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                using (tsmc14BDataContext db = new tsmc14BDataContext())
                {
                    var r = (from row in db.plc_info where row.plc_id == pid && DateTime.Now < delayFDCUploadTime select row).SingleOrDefault();

                    if (r != null)
                    {
                        string SQL = "exec uSP_Change_FDCPM @plc_id=" + pid + ",@PMsetDateTime='" + delayFDCUploadTime.ToString("yyyy-MM-dd HH:mm:ss") + "',@login_name='" + (string)Session["UserName"] + "',@SetFlag=" + SetFlag + ",@memo=N'" + memo + "'";
                        try
                        {
                            db.ExecuteCommand(SQL);
                            resultString = "Setting OK";
                        }
                        catch (Exception ex)
                        {
                            resultString = "Setting fail," + ex.Message;
                        }
                    }
                    else
                    {
                        resultString = "Setting fail,設定時間不能小於現在時間";
                    }
                }
            }
            else
            {
                return null;
            }

            return Json(new { result = resultString });
        }

        public JsonResult JsonEQAlarmHistory(string ToolID, string StartDate, string EndDate, int page, int rp)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                IEnumerable<AlarmNowModel> al = AlarmCheckModel.GetPLCAlarmList(StartDate, EndDate, ToolID, ConfigurationManager.AppSettings["AlarmLevel"]);

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

        public ActionResult PLCCheckDemo(string vName, string Device, string ToolID, string TagName, string Typeid, string StartDate, string EndDate, string flag, string output)
        {
            if (output != "export")
            {
                try
                {
                    IEnumerable<ListModel> codes2 = ListModel.GetDepartmentTitleList();
                    ViewBag.Device = from c in codes2
                                     select new SelectListItem
                                     {
                                         Value = c.Value,
                                         Text = c.Text
                                     };
                    IEnumerable<ListModel> codes3 = ListModel.GetToolIDList(vName == null ? Request["vName"] : vName, Device == null ? null : Device);
                    ViewBag.ToolID = from c in codes3
                                     select new SelectListItem
                                     {
                                         Value = c.Value,
                                         Text = c.Text,
                                     };

                    if (vName != "DPM")
                    {
                        IEnumerable<ListModel> codes5 = ListModel.GetTypeByVname(vName == null ? Request["vName"] : vName, Device == null ? null : Device);
                        ViewBag.Type = from c in codes5
                                       select new SelectListItem
                                       {
                                           Value = c.Value,
                                           Text = c.Text
                                       };
                    }

                    if (!string.IsNullOrEmpty(Typeid))
                    {
                        IEnumerable<ListModel> codes7 = ListModel.GetToolidByType(vName == null ? Request["vName"] : vName, Typeid);
                        ViewBag.ToolID2 = from c in codes7
                                          select new SelectListItem
                                          {
                                              Value = c.Value,
                                              Text = c.Text
                                          };
                        IEnumerable<ListModel> codes6 = ListModel.GetTagByType(vName == null ? Request["vName"] : vName, Typeid);
                        ViewBag.TagName2 = from c in codes6
                                           select new SelectListItem
                                           {
                                               Value = c.Value,
                                               Text = c.Text
                                           };
                    }
                }
                catch (Exception)
                {

                }
                return View();
            }
            else
            {
                if (flag == "1")
                {
                    byte[] file = PLCCheckModel.GetChartValueFile(vName, ToolID, TagName, StartDate, EndDate, flag);
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlPathEncode("設備數值資料_" + vName + "_" + ToolID + "_" + DateTime.Now.ToShortDateString() + ".xlsx"));
                }
                else if (flag == "2")
                {
                    byte[] file = PLCCheckModel.GetChartValueFile2(vName, ToolID, TagName, StartDate, EndDate, flag);
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlPathEncode("設備數值資料_" + vName + "_" + DateTime.Now.ToShortDateString() + ".xlsx"));
                }
                else
                {
                    byte[] file = PLCCheckModel.GetChartValueFile3(vName, ToolID, TagName, StartDate, EndDate, flag);
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlPathEncode("設備數值資料_" + vName + "_" + DateTime.Now.ToShortDateString() + ".xlsx"));
                }
            }
        }

        public JsonResult JsonEQValueNow(int pid)
        {
            if (!string.IsNullOrEmpty((string)Session["UserName"]))
            {
                //IEnumerable<AlarmNowModel> al = AlarmCheckModel.GetPLCAlarmList(StartDate, EndDate, ToolID, ConfigurationManager.AppSettings["AlarmLevel"]);
                //List<vw_eq_valuenow> EQValueNow;
                using (tsmc14BDataContext db = new tsmc14BDataContext())
                {
                    //SELECT  TagName,round(Value,2,1) as Value,Cht_Comment,Unit,eqs.sid,eqs.status  FROM vw_eq_valuenow eqv join vw_eq_status eqs on eqv.pid=eqs.pid where eqs.pid=1 order by TagName
                    //string sqlStr = "SELECT  TagName,round(Value,2,1) as Value,Cht_Comment,Unit,eqs.sid,eqs.status  FROM vw_eq_valuenow eqv join vw_eq_status eqs on eqv.pid=eqs.pid where eqs.pid=1 order by TagName";
                    //DataSet DeptDS = DBConnector.executeQuery("Intouch", sqlStr);

                    var EQValueNow = (from row in db.vw_eq_valuenow.AsParallel() join SS in db.vw_eq_status.AsParallel() on row.pid equals SS.pid where row.pid == pid orderby row.TagName select new { TagName = row.TagName, Cht_Comment = row.Cht_Comment, Value = Math.Round(row.Value != null ? row.Value.Value : 0, 2, MidpointRounding.AwayFromZero), StatusId = SS.sid, Status = SS.status, sColor = SS.sColor, Unit = row.Unit }).ToList();
                    //var EQValueNow = (from dept in DeptDS.Tables[0].AsEnumerable()
                    //          select new 
                    //          {
                    //           TagName =dept["TagName"], Cht_Comment = dept["Cht_Comment"], Value = dept["Value"],StatusId=dept["sid"],Status = dept["Status"],Unit=dept["Unit"] 

                    //          }).ToList();

                    var EQStatus = (from c in EQValueNow select c).FirstOrDefault();

                    //var codes2 = (from row in db.vw_eq_status where row.pid == pid select new { chamberName = row.chamberName,sid=row.sid }).FirstOrDefault();

                    //PM_Set PS = (from row in db.PM_Set where row.tool_id == codes2.chamberName select row).SingleOrDefault();

                    int IsPM = 0;

                    //if (PS != null)
                    //{
                    //    IsPM = PS.pmFlag ? 2 : 3;
                    //}
                    //else
                    //{
                    //    IsPM = codes2.sid == 8 ? 0 : 1;
                    //}

                    return Json(new
                    {
                        rows = EQValueNow,
                        statusid = EQStatus.StatusId,
                        status = EQStatus.Status,
                        IsPM = IsPM,
                        sColor = EQStatus.sColor,
                        //nextFDCUpload = EQStatus.nextFDCUpload>DateTime.Now? "下次資料收集時間:"+ EQStatus.nextFDCUpload.ToString("yyyy-MM-dd HH:mm:ss") : ""
                    });
                }
            }
            else
                return null;
        }

        public ActionResult PLCReport(string vName, string Device, string ToolID, string TagName, string Typeid, string StartDate, string EndDate, string flag, string output)
        {
            if (output != "export")
            {
                return View();
            }
            else
            {
                if (flag == "1")
                {
                    byte[] file = PLCCheckModel.GetChartValueFile(vName, ToolID, TagName, StartDate, EndDate, flag);
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlPathEncode("設備數值資料_" + vName + "_" + ToolID + "_" + DateTime.Now.ToShortDateString() + ".xlsx"));
                }
                else if (flag == "2")
                {
                    byte[] file = PLCCheckModel.GetChartValueFile2(vName, ToolID, TagName, StartDate, EndDate, flag);
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlPathEncode("設備數值資料_" + vName + "_" + DateTime.Now.ToShortDateString() + ".xlsx"));
                }
                else
                {
                    byte[] file = PLCCheckModel.GetChartValueFile3(vName, ToolID, TagName, StartDate, EndDate, flag);
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlPathEncode("設備數值資料_" + vName + "_" + DateTime.Now.ToShortDateString() + ".xlsx"));
                }
            }
        }

        #endregion

        #region TCBookMark

        public JsonResult JsonTCBookList(int page, int rp)
        {
            if (!string.IsNullOrEmpty((string)Session["UserID"]))
            {
                using (tsmc14BDataContext db = new tsmc14BDataContext())
                {
                    var al = db.vw_TCBook_info.Where(x => x.Login_name == (string)Session["UserID"]).Select(x => new
                    {
                        TCBID = x.TCBID.ToString(),
                        TCName = x.TCName,
                        builedate = DateTime.Parse(x.builedate.ToString()).ToString("yyyy-MM-dd"),
                        Login_name = x.Login_name,
                        tagString = x.tagString,
                        toolidString = x.toolidString,
                        dataTagString = x.dataTagString
                    }).ToList();

                    return Json(new
                    {
                        page = page,
                        total = al.Count(),
                        rows = from c in al.Skip((page - 1) * rp).Take(rp)
                               select c
                    });
                }
            }
            else
            {
                return null;
            }
        }

        public JsonResult JsonTCBookDelete(int TCBID)
        {
            string s = string.Empty;

            if (!string.IsNullOrEmpty((string)Session["UserID"]))
            {
                try
                {
                    using (tsmc14BDataContext db = new tsmc14BDataContext())
                    {
                        var r = db.TCBook_info.Where(x => x.TCBID == TCBID).SingleOrDefault();
                        if (r != null)
                        {
                            db.TCBook_info.DeleteOnSubmit(r);
                            var rr = db.TCTag_info.Where(x => x.TCBID == TCBID).ToList();

                            if (rr != null)
                            {
                                db.TCTag_info.DeleteAllOnSubmit(rr);
                            }

                            db.SubmitChanges();
                            s = WebCMS.Menu.Delete + "ok";
                        }
                    }
                }
                catch (Exception ex)
                {
                    s = ex.Message;
                }
            }
            return Json(s);
        }

        public ActionResult AddTCBook(string tagName)
        {
            ViewBag.Title = WebCMS.Menu.AddTCBook;
            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                string[] S = tagName.Split(',');

                var rr = db.vw_Tag_info.Where(x => S.Contains(x.FullTagName)).ToList();
                ViewData["tagName"] = tagName;

                ViewData["SensorTagName"] = rr;
            }

            return View();
        }
        [HttpPost]
        public ActionResult AddTCBook(string tagName, string TCBookName)
        {
            ViewBag.Title = WebCMS.Menu.AddTCBook;
            if (!string.IsNullOrEmpty((string)Session["UserID"]))
            {
                try
                {


                    using (tsmc14BDataContext db = new tsmc14BDataContext())
                    {
                        if (tagName.Length > 0 && TCBookName.Length > 0)
                        {

                            string[] S = tagName.Split(',');

                            var rr = db.vw_Tag_info.Where(x => S.Contains(x.FullTagName)).ToList();
                            ViewData["tagName"] = tagName;

                            ViewData["SensorTagName"] = rr;

                            DateTime dateNow = DateTime.Now;

                            TCBook_info TCB = new TCBook_info();

                            TCB.TCName = TCBookName;
                            TCB.Login_name = (string)Session["UserID"];
                            TCB.builedate = dateNow;

                            foreach (string Tag in S)
                            {
                                TCTag_info TCT = new TCTag_info();
                                TCT.FullTagName = Tag;
                                TCT.toolid = rr.Where(x => x.FullTagName == Tag).SingleOrDefault().sensorID;
                                TCB.TCTag_info.Add(TCT);

                            }
                            db.TCBook_info.InsertOnSubmit(TCB);
                            db.SubmitChanges();
                            ViewBag.Message = WebCMS.Menu.Add + "ok";

                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            }
            else
            {
                ViewBag.MessageType = "警告";
                ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                return View("Message");
            }

            return View();
        }

        public ActionResult EditTCBook(string TCBID, string tagName)
        {
            ViewBag.Title = WebCMS.Menu.EditTCBook;
            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                string[] S = tagName.Split(',');

                var rr = db.vw_Tag_info.Where(x => S.Contains(x.FullTagName)).ToList();
                ViewData["tagName"] = tagName;

                ViewData["SensorTagName"] = rr;

                ViewData["TCName"] = db.TCBook_info.Where(x => x.TCBID.ToString() == TCBID).SingleOrDefault().TCName;

            }

            return View("AddTCBook");
        }
        [HttpPost]
        public ActionResult EditTCBook(string TCBID, string tagName, string TCBookName)
        {
            ViewBag.Title = WebCMS.Menu.AddTCBook;
            if (!string.IsNullOrEmpty((string)Session["UserID"]))
            {
                try
                {
                    using (tsmc14BDataContext db = new tsmc14BDataContext())
                    {
                        if (tagName.Length > 0 && TCBookName.Length > 0 && TCBID.Length > 0)
                        {

                            string[] S = tagName.Split(',');

                            var rr = db.vw_Tag_info.Where(x => S.Contains(x.FullTagName)).ToList();
                            ViewData["tagName"] = tagName;

                            ViewData["SensorTagName"] = rr;

                            DateTime dateNow = DateTime.Now;

                            TCBook_info TCB = db.TCBook_info.Where(x => x.TCBID.ToString() == TCBID).SingleOrDefault();

                            TCB.TCName = TCBookName;
                            TCB.builedate = dateNow;

                            var TCTList = TCB.TCTag_info.ToList();
                            if (TCTList != null && TCTList.Count > 0)
                            {
                                db.TCTag_info.DeleteAllOnSubmit(TCTList);

                                db.SubmitChanges();
                            }

                            foreach (string Tag in S)
                            {
                                TCTag_info TCT = new TCTag_info();
                                TCT.FullTagName = Tag;
                                TCT.toolid = rr.Where(x => x.FullTagName == Tag).SingleOrDefault().sensorID;
                                TCB.TCTag_info.Add(TCT);

                            }

                            db.SubmitChanges();
                            ViewBag.Message = WebCMS.Menu.Edit + "ok";

                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            }
            else
            {
                ViewBag.MessageType = "警告";
                ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                return View("Message");
            }

            return View("AddTCBook");
        }


        #endregion
    }
}
