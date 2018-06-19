using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Linq;
using System.IO;
using System.Web.UI.DataVisualization.Charting;
using WebCMS.Models;
using System.Linq;
using KRTDBAPI;
using Sorba;


namespace WebCMS.Areas.Main.Models
{
    public class PLCCheckModel
    {
        #region 組織成員屬性

        [Display(Name = "代碼型態編碼")]
        public int Code { set; get; }

        [Display(Name = "代碼型態名稱")]
        public string Name { set; get; }

        [Display(Name = "toolId")]
        public string toolId { get; set; }

        [Display(Name = "DateTime")]
        public string _DateTime { get; set; }

        [Display(Name = "Comment")]
        public string Cht_Comment { get; set; }

        [Display(Name = "Trend Chart Value")]
        public bool TrendChartValue { get; set; }
        #endregion

        public static IEnumerable<PLCCheckModel> eqDL(string vName, string gName)
        {
            string sqlStr = null;
            sqlStr = "SELECT pid,toolid FROM vw_eq_status ";
            if (!string.IsNullOrEmpty(vName))
            {
                sqlStr += "where vName = '" + vName + "'";
            }
            else
            {
                sqlStr += "where vName = 'CS250'";
            }
            if (!string.IsNullOrEmpty(gName))
            {
                sqlStr += "AND groupname = '" + gName + "'";
            }
            else
            {
                sqlStr += "AND groupname IS NULL";
            }
            DataSet DeptDS = DBConnector.executeQuery("Intouch", sqlStr);

            IEnumerable<PLCCheckModel> tempList = null;

            if (DeptDS.Tables.Count > 0)
            {
                tempList = from dept in DeptDS.Tables[0].AsEnumerable()
                           select new PLCCheckModel
                           {
                               Code = dept.Field<int>("pid"),
                               Name = dept.Field<string>("toolid")
                           };
            }

            return tempList;
        }

        public static IEnumerable<PLCCheckModel> GetPMToolidList(string vName, string Device, string toolId, string pmFlag)
        {
            DataSet ds = DBConnector.executeQuery("Intouch", "SELECT distinct vid,vName FROM vw_eq_status where vName ='" + vName + "'");
            string sqlStr = null;
            if (string.IsNullOrEmpty(Device))
            {
                Device = "NULL";
            }
            else
            {
                Device = "'" + Device + "'";
            }
            if (string.IsNullOrEmpty(toolId))
            {
                toolId = "NULL";
            }
            else
            {
                toolId = "'" + toolId + "'";
            }

            sqlStr = "EXEC [dbo].[uSP_Select_PMTrigger] " + pmFlag + "," + ds.Tables[0].Rows[0][0].ToString() + "," + Device + "," + toolId;

            DataSet DeptDS = DBConnector.executeQuery("Intouch", sqlStr);

            IEnumerable<PLCCheckModel> tempList = null;

            if (DeptDS.Tables.Count > 0)
            {
                tempList = from dept in DeptDS.Tables[0].AsEnumerable()
                           orderby dept.Field<string>("toolid")
                           select new PLCCheckModel
                           {
                               Code = dept.Field<int>("pid"),
                               Name = dept.Field<string>("toolid")
                           };
            }

            return tempList;
        }

        public static IEnumerable<PLCCheckModel> GetPMToolidList(string vName, string Toolid, string pmFlag)
        {
            DataSet DeptDS = DBConnector.executeQuery("Intouch", "EXEC [dbo].[uSP_Select_PMTrigger] " + pmFlag + ", NULL ,NULL, '" + Toolid + "'");

            IEnumerable<PLCCheckModel> tempList = null;

            if (DeptDS.Tables.Count > 0)
            {
                if (vName == "系統管理者" || vName == "帳號管理者" || vName == "工程師")
                {
                    tempList = from dept in DeptDS.Tables[0].AsEnumerable()
                               orderby dept.Field<string>("toolid")
                               select new PLCCheckModel
                               {
                                   Code = dept.Field<int>("pid"),
                                   Name = dept.Field<string>("toolid")
                               };
                }
                else
                {
                    tempList = from dept in DeptDS.Tables[0].AsEnumerable()
                               where dept.Field<string>("vName").Contains(vName)
                               orderby dept.Field<string>("toolid")
                               select new PLCCheckModel
                               {
                                   Code = dept.Field<int>("pid"),
                                   Name = dept.Field<string>("toolid")
                               };
                }
            }
            return tempList;
        }

        public static IEnumerable<PLCCheckModel> Device(string vName)
        {
            DataSet DeptDS = DBConnector.executeQuery("Intouch", "SELECT distinct(groupname) FROM vw_eq_status where vName = '" + vName + "'");
            IEnumerable<PLCCheckModel> tempList = null;

            if (DeptDS.Tables.Count > 0)
            {
                tempList = from dept in DeptDS.Tables[0].AsEnumerable()
                           select new PLCCheckModel
                           {
                               Name = dept.Field<string>("groupname")
                           };
            }
            return tempList;
        }

        private static Connection m_Con = null;
        public static DataTable GetChartValueKHdt(string vName, string tool_id, string TagName, string StartDate, string EndDate, string KHIP, string action)
        {
            DataTable dt = new DataTable();

            int pid = 0;
            int type_id = 0;
            string Cht_Comment = string.Empty;
            //string KHIP = System.Configuration.ConfigurationManager.AppSettings["KHDBIP"];
            string sqlstr = string.Empty;
            double SamplingInterval = Math.Round((DateTime.Parse(EndDate) - DateTime.Parse(StartDate)).TotalDays * 100000);

            try
            {
                //if (m_Con == null)
                //{
                    m_Con = new Connection();
                    m_Con.Connect(KHIP, "5678", "sa", "sa", 3000, "");
                //}
            }
            catch (Exception)
            {
                return null;
            }

            try
            {
                //sqlstr += "SET @@SamplingMode = 'Trend'; ";

                sqlstr += "SET @@SamplingMode = 'INTERPOLATED'; ";
                //sqlstr += "SET @@SamplingInterval =" + SamplingInterval.ToString() + "; ";

                //sqlstr += "SET @@SamplingInterval =5000; ";
                sqlstr += "SET @@SamplingInterval =" + Math.Ceiling(SamplingInterval).ToString() + "; ";
                sqlstr += "SELECT case when  DataValue is null then 0 else DataValue end as [Value],DataTime,EngineeringUnit as [Unit],History.tagname ";
                sqlstr += " FROM History ";
                sqlstr += " join tag on History.tagname=tag.tagname ";
                sqlstr += " where History.TagName in ('" + TagName.Replace(",", "','") + "')";
                sqlstr += " and DataTime>'" + StartDate + "' ";
                sqlstr += " and DataTime<'" + EndDate + "' ";

                DateTime aaa = m_Con.ServerTime;

                dt.Load(m_Con.SqlExecute(sqlstr));

                dt.Columns.Add("pid");
                dt.Columns.Add("tool_id");
                dt.Columns.Add("Cht_Comment");

                foreach (DataRow item in dt.Rows)
                {
                    item["DataTime"] = ((DateTime)item["DataTime"]).ToLocalTime();
                }

                string[] s = TagName.Split(',');

                using (tsmc14BDataContext db = new tsmc14BDataContext())
                {
                    foreach (string tg in s)
                    {
                        string[] tga = tg.Split('_');
                        vw_type_parameter tt;

                        var bb = (from row in db.vw_Tag_info where row.FullTagName==tg select row).First();
                        if (bb != null) {
                            pid = bb.plc_id;
                            type_id = bb.type_id;
                            tool_id = bb.sensorID;
                        }

                        //var aa = (from row in db.vw_eq_status where row.chamberName == tool_id select row).First();

                        //if (aa != null)
                        //{
                        //    pid = aa.pid;
                        //    type_id = aa.typeid;
                        //}
                        string paramiter = "";
                        if (bb.WEB_code != "Sensor")
                        {
                            string tmpTagName = tg.Substring(tg.IndexOf(tool_id) + tool_id.Length, tg.Length - (tg.IndexOf(tool_id) + tool_id.Length));
                            tt = (from row in db.vw_type_parameter where row.Tag_Name==tmpTagName select row).First();
                            if (tt != null)
                            {
                                Cht_Comment = tt.Cht_Comment;
                                paramiter = tt.Tag_Name;
                            }
                        }
                        else if (bb.WEB_code == "Sensor")
                        {

                            //tt = (from row in db.vw_type_parameter where tg == tool_id + "_" + tga[2] + row.Tag_Name select row).First();
                            var sensorData = (from row in db.ExtraSensor_info where row.pid == pid && TagName.IndexOf(row.FullTagName)>0 select row).First();

                            if (sensorData != null)
                            {
                                Cht_Comment = sensorData.Sensor_Name;
                                paramiter = sensorData.FullTagName;
                            }

                        }

                        var tempDt = from row in dt.AsEnumerable() where row["tagname"].ToString() == tg select row;
                        foreach (var item in tempDt)
                        {
                            item["pid"] = pid;
                            item["tool_id"] = tool_id;
                            item["Cht_Comment"] = Cht_Comment;
                        }
                    }
                }
                dt.AcceptChanges();
            }
            catch (Exception )
            {
                return null;
            }

            return dt;
        }

        public static DataTable GetChartValuedt(string vName, string toolId, string TagName, string StartDate, string EndDate, string action)
        {
            DataTable dt = new DataTable();
            DataSet DeptDS = new DataSet();

            if (action == "1")
                DeptDS = DBConnector.executeQuery("Intouch", "EXEC uSP_Select_ValueHistory_BestFit  @pid= " + ListModel.GetPid(toolId) + ", @valueTags= '" + TagName + "' , @StartDate = '" + StartDate + "', @EndDate = '" + EndDate + "'");
            else
            {
                string pid = null;
                string strSP = null;
                if (vName == "DPM")
                {
                    using (tsmc14BDataContext db = new tsmc14BDataContext())
                    {
                        foreach (var item in toolId.Split(','))
                        {
                            pid += (from row in db.vw_dpm_status where row.toolid == item select row.pid).FirstOrDefault() + ",";
                        }
                    }
                    strSP = "uSP_Select_ValueHistory_BestFit_EQType_DPM";
                }
                else
                {
                    foreach (var item in toolId.Split(','))
                    {
                        pid += ListModel.GetPid(item) + ",";
                    }
                    strSP = "uSP_Select_ValueHistory_BestFit_EQType";
                }
                pid = pid.Substring(0, pid.Length - 1);
                DeptDS = DBConnector.executeQuery("Intouch", "EXEC " + strSP + "  @pids= '" + pid + "', @valueTags= '" + TagName + "' , @StartDate = '" + StartDate + "', @EndDate = '" + EndDate + "'");
            }

            //using (SqlConnection conn = new SqlConnection())
            //{
            //    conn.ConnectionString = WebConfigurationManager.ConnectionStrings["Intouch"].ConnectionString;
            //    conn.Open();

            //    if (action == "1")
            //        dt.Load(new SqlCommand("EXEC uSP_Select_ValueHistory_BestFit  @pid= " + ListModel.GetPid(toolId) + ", @valueTags= '" + TagName + "' , @StartDate = '" + StartDate + "', @EndDate = '" + EndDate + "'", conn).ExecuteReader());
            //    else
            //    {
            //        string pid = null;
            //        foreach (var item in toolId.Split(','))
            //        {
            //            pid += ListModel.GetPid(item) + ",";
            //        }
            //        pid = pid.Substring(0, pid.Length - 1);
            //        dt.Load(new SqlCommand("EXEC uSP_Select_ValueHistory_BestFit_EQType  @pids= '" + pid + "', @valueTags= '" + TagName + "' , @StartDate = '" + StartDate + "', @EndDate = '" + EndDate + "'", conn).ExecuteReader());
            //    }
            //}
            return DeptDS.Tables[0];
        }

        public static DataTable GetChartValuedtDemo(string vName, string toolId, string TagName, string StartDate, string EndDate, string action)
        {
            DataTable dt = new DataTable();
            DataSet DeptDS = new DataSet();

            if (action == "1")
                DeptDS = DBConnector.executeQuery("Intouch", "EXEC uSP_Select_ValueHistory_Cyclic  @pid= " + ListModel.GetPid(toolId) + ", @valueTags= '" + TagName + "' , @StartDate = '" + StartDate + "', @EndDate = '" + EndDate + "'");
            else
            {
                string pid = null;
                string strSP = null;
                if (vName == "DPM")
                {
                    using (tsmc14BDataContext db = new tsmc14BDataContext())
                    {
                        foreach (var item in toolId.Split(','))
                        {
                            pid += (from row in db.vw_dpm_status where row.toolid == item select row.pid).FirstOrDefault() + ",";
                        }
                    }
                    strSP = "uSP_Select_ValueHistory_BestFit_EQType_DPM";
                }
                else
                {
                    foreach (var item in toolId.Split(','))
                    {
                        pid += ListModel.GetPid(item) + ",";
                    }
                    strSP = "uSP_Select_ValueHistory_Cyclic_EQType";
                }
                pid = pid.Substring(0, pid.Length - 1);
                DeptDS = DBConnector.executeQuery("Intouch", "EXEC " + strSP + "  @pids= '" + pid + "', @valueTags= '" + TagName + "' , @StartDate = '" + StartDate + "', @EndDate = '" + EndDate + "'");
            }

            //using (SqlConnection conn = new SqlConnection())
            //{
            //    conn.ConnectionString = WebConfigurationManager.ConnectionStrings["Intouch"].ConnectionString;
            //    conn.Open();
            //    if (action == "1")
            //        dt.Load(new SqlCommand("EXEC uSP_Select_ValueHistory_BestFit  @pid= " + ListModel.GetPid(toolId) + ", @valueTags= '" + TagName + "' , @StartDate = '" + StartDate + "', @EndDate = '" + EndDate + "'", conn).ExecuteReader());
            //    else
            //    {
            //        string pid = null;
            //        foreach (var item in toolId.Split(','))
            //        {
            //            pid += ListModel.GetPid(item) + ",";
            //        }
            //        pid = pid.Substring(0, pid.Length - 1);
            //        dt.Load(new SqlCommand("EXEC uSP_Select_ValueHistory_BestFit_EQType  @pids= '" + pid + "', @valueTags= '" + TagName + "' , @StartDate = '" + StartDate + "', @EndDate = '" + EndDate + "'", conn).ExecuteReader());
            //    }
            //}
            return DeptDS.Tables[0];
        }

        public static DataTable GetLimitValuedt(string TagName)
        {

            //string limitSql = "SELECT  l.FullTagName,case when p.typeid in (202,203) then ESI.Sensor_Name else  a.Cht_Comment end as Cht_Comment,l.Limit_Max_Setting,l.Limit_Min_Setting,Limit_Max_Type,Limit_Min_Type,p.chamberName as tool_id FROM vw_eq_status p left join  vw_ExtraSensor_info ESI on ESI.pid=p.pid join  vw_eq_limit l on p.chamberName =SUBSTRING(SUBSTRING(l.FullTagName, CHARINDEX('_',l.FullTagName,0)+1,LEN(l.FullTagName)),0,CHARINDEX('_',SUBSTRING(l.FullTagName, CHARINDEX('_',l.FullTagName,0)+1,LEN(l.FullTagName)),0)) and p.typeid<>202 and p.typeid<>203 or l.FullTagName=ESI.FullTagName left join analog_comment a on SUBSTRING(l.FullTagName, CHARINDEX('_',l.FullTagName,10),LEN(l.FullTagName)) = a.Tag_Name and p.typeid=a.type_id where l.FullTagName ='" + TagName + "'";
            string limitSql = "SELECT l.FullTagName,case when p.typeid between  202 and 250 then ESI.Sensor_Name else  a.Cht_Comment end as Cht_Comment,l.Limit_Max_Setting,l.Limit_Min_Setting,Limit_Max_Type,Limit_Min_Type,p.chamberName as tool_id FROM vw_eq_status p left join  vw_ExtraSensor_info ESI on ESI.pid=p.plc_id join  vw_eq_limit l on p.pid=l.pid and (p.typeid  not between 202 and 250) or l.FullTagName=ESI.FullTagName left join analog_comment a on SUBSTRING(l.FullTagName, CHARINDEX('_',l.FullTagName,10),LEN(l.FullTagName)) = a.Tag_Name and p.typeid=a.type_id where l.FullTagName ='" + TagName + "'";

            DataSet LimitDS = DBConnector.executeQuery("Intouch", limitSql);

            return LimitDS.Tables[0];
        }

        public static Chart GetChartValue(string vName, string toolId, string TagName, string StartDate, string EndDate, string action)
        {
            DataTable dt = PLCCheckModel.GetChartValuedt(vName, toolId, TagName, StartDate, EndDate, action);

            //建立一個新的Chart
            Chart chart = new Chart();

            //建立一個ChartAreas，並對ChartAreas設定格式
            chart.ChartAreas.Add("ChartAreas1");
            chart.ChartAreas["ChartAreas1"].BorderColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].BorderDashStyle = ChartDashStyle.Solid;
            chart.ChartAreas["ChartAreas1"].BackSecondaryColor = System.Drawing.Color.White;
            chart.ChartAreas["ChartAreas1"].BackColor = System.Drawing.Color.FromArgb(64, 165, 191, 228);
            chart.ChartAreas["ChartAreas1"].ShadowColor = System.Drawing.Color.Transparent;
            chart.ChartAreas["ChartAreas1"].BackGradientStyle = GradientStyle.TopBottom;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.Rotation = 10;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.Perspective = 10;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.Inclination = 15;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.IsRightAngleAxes = true;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.WallWidth = 0;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.IsClustered = true;
            chart.ChartAreas["ChartAreas1"].AxisY.IsStartedFromZero = false;
            //chart.ChartAreas["ChartAreas1"].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chart.ChartAreas["ChartAreas1"].AxisY.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25f, System.Drawing.FontStyle.Bold);
            chart.ChartAreas["ChartAreas1"].AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisX.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25f, System.Drawing.FontStyle.Bold);
            chart.ChartAreas["ChartAreas1"].AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
            //chart.ChartAreas["ChartAreas1"].AxisX.IsLabelAutoFit =true;
            //chart.ChartAreas["ChartAreas1"].AxisY.IsLabelAutoFit = true;
            //chart.ChartAreas["ChartAreas1"].AxisX.IntervalType = DateTimeIntervalType.Hours;
            chart.ChartAreas["ChartAreas1"].AxisX.LabelStyle.Format = "yyyy-MM-dd \n HH:mm:ss";

            chart.Legends["Default"].Alignment = System.Drawing.StringAlignment.Near;
            chart.Legends["Default"].Docking = Docking.Top;
            chart.ChartAreas["ChartAreas1"].AxisX.LabelStyle.TruncatedLabels = true;
            chart.ChartAreas["ChartAreas1"].AxisX.IsInterlaced = true;
            /* 20140214 ISABELLA */
            //設置X軸座標的間隔為1
            //chart.ChartAreas["ChartAreas1"].AxisX.Interval = 1;
            //設置X軸座標偏移為1
            //chart.ChartAreas["ChartAreas1"].AxisX.IntervalOffset = 1;
            ////設置是否交錯顯示，比如數據多時分成兩行來顯示
            //chart.ChartAreas["ChartAreas1"].AxisX.LabelStyle.IsStaggered = true;
            chart.Width = 800;// (Int32)(dt.Rows.Count / TagName.Split(',').Length + 300);
            chart.Height = 600;

            double YMax = 0;
            chart.Titles.Add(toolId);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    if (chart.Series.IsUniqueName(dt.Rows[i][5].ToString()))
                    {
                        chart.Series.Add(dt.Rows[i][5].ToString());
                        chart.Series[dt.Rows[i][5].ToString()].ChartType = SeriesChartType.FastLine;
                        chart.Series[dt.Rows[i][5].ToString()].ChartArea = "ChartAreas1";
                        chart.Series[dt.Rows[i][5].ToString()].IsVisibleInLegend = true;
                        chart.Series[dt.Rows[i][5].ToString()].Legend = "Default";
                        chart.Series[dt.Rows[i][5].ToString()].BorderWidth = 1;
                        chart.Series["Series1"]["BarLabelStyle"] = "Center";
                    }

                    chart.Series[dt.Rows[i][5].ToString()].Points.AddXY(Convert.ToDateTime(dt.Rows[i][3]), dt.Rows[i][2]);
                    chart.Series[dt.Rows[i][5].ToString()].ToolTip = dt.Rows[i][2].ToString();
                    chart.Series[dt.Rows[i][5].ToString()].IsValueShownAsLabel = true;
                    if (dt.Rows[i][2] != null && dt.Rows[i][2].ToString().ToUpper() != "NULL" && dt.Rows[i][2].ToString().Length > 0)
                    {
                        if (YMax < Convert.ToDouble(dt.Rows[i][2]))
                        {
                            YMax = Convert.ToDouble(dt.Rows[i][2]) + Convert.ToDouble(dt.Rows[i][2]) * 0.5;
                        }
                    }
                }
            }
            chart.ChartAreas["ChartAreas1"].AxisY.Maximum = YMax + 5;
            if (YMax == 0)
            {
                chart.ChartAreas["ChartAreas1"].AxisY.Minimum = -5;
            }

            //MemoryStream ms = new MemoryStream();
            //chart.SaveImage(ms, ChartImageFormat.Png);
            //ms.Seek(0, SeekOrigin.Begin);
            return chart;
        }

        public static Chart GetChartValue2(string vName, string toolId, string TagName, string StartDate, string EndDate, string action)
        {
            //DataTable dt = PLCCheckModel.GetChartValuedt(vName, toolId, TagName, StartDate, EndDate, action);
            //
            ////建立一個新的Chart
            //Chart chart = new Chart();
            //
            ////建立一個ChartAreas，並對ChartAreas設定格式
            //chart.ChartAreas.Add("ChartAreas1");
            //chart.ChartAreas["ChartAreas1"].BorderColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            //chart.ChartAreas["ChartAreas1"].BorderDashStyle = ChartDashStyle.Solid;
            //chart.ChartAreas["ChartAreas1"].BackSecondaryColor = System.Drawing.Color.White;
            //chart.ChartAreas["ChartAreas1"].BackColor = System.Drawing.Color.FromArgb(64, 165, 191, 228);
            //chart.ChartAreas["ChartAreas1"].ShadowColor = System.Drawing.Color.Transparent;
            //chart.ChartAreas["ChartAreas1"].BackGradientStyle = GradientStyle.TopBottom;
            //chart.ChartAreas["ChartAreas1"].Area3DStyle.Rotation = 10;
            //chart.ChartAreas["ChartAreas1"].Area3DStyle.Perspective = 10;
            //chart.ChartAreas["ChartAreas1"].Area3DStyle.Inclination = 15;
            //chart.ChartAreas["ChartAreas1"].Area3DStyle.IsRightAngleAxes = true;
            //chart.ChartAreas["ChartAreas1"].Area3DStyle.WallWidth = 0;
            //chart.ChartAreas["ChartAreas1"].Area3DStyle.IsClustered = true;
            //chart.ChartAreas["ChartAreas1"].AxisY.IsStartedFromZero = false;
            ////chart.ChartAreas["ChartAreas1"].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
            //
            //chart.ChartAreas["ChartAreas1"].AxisY.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            //chart.ChartAreas["ChartAreas1"].AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25f, System.Drawing.FontStyle.Bold);
            //chart.ChartAreas["ChartAreas1"].AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            //chart.ChartAreas["ChartAreas1"].AxisX.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            //chart.ChartAreas["ChartAreas1"].AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25f, System.Drawing.FontStyle.Bold);
            //chart.ChartAreas["ChartAreas1"].AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            //chart.ChartAreas["ChartAreas1"].AxisX.LabelStyle.Format = "yyyy-MM-dd \n HH:mm:ss";
            //chart.ChartAreas["ChartAreas1"].AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
            ////chart.ChartAreas["ChartAreas1"].AxisX.IsLabelAutoFit = true;
            ////chart.ChartAreas["ChartAreas1"].AxisY.IsLabelAutoFit = true;
            ////chart.ChartAreas["ChartAreas1"].AxisX.IntervalType = DateTimeIntervalType.Hours;
            //chart.ChartAreas["ChartAreas1"].AxisX.LabelStyle.TruncatedLabels = true;
            //chart.ChartAreas["ChartAreas1"].AxisX.IsInterlaced = true;
            //
            //chart.Legends.Add("Default");
            //chart.Legends["Default"].Alignment = System.Drawing.StringAlignment.Near;
            //chart.Legends["Default"].Docking = Docking.Top;
            ///* 20140214 ISABELLA */
            ////設置X軸座標的間隔為1
            ////chart.ChartAreas["ChartAreas1"].AxisX.Interval = 1;
            ////設置X軸座標偏移為1
            ////chart.ChartAreas["ChartAreas1"].AxisX.IntervalOffset = 1;
            //////設置是否交錯顯示，比如數據多時分成兩行來顯示
            ////chart.ChartAreas["ChartAreas1"].AxisX.LabelStyle.IsStaggered = true;
            ////string tName = TagName.Split(',')[0].ToString();
            ////tName = tName.Substring(7, tName.Length-1);
            ////tName = tName.Substring(toolId.Split(',')[0].ToString().Length-1);
            //DataSet DeptDS = DBConnector.executeQuery("Intouch", "SELECT Cht_Comment FROM analog_comment where Tag_Name = '" + TagName.Split(',')[0].Substring(7 + toolId.Split(',')[0].ToString().Length, TagName.Split(',')[0].ToString().Length - (7 + toolId.Split(',')[0].ToString().Length)) + "'");
            ////DataSet DeptDS = DBConnector.executeQuery("Intouch", "SELECT Cht_Comment FROM analog_comment where Tag_Name = '" + TagName.Split(toolId.Split(',').Last().ToCharArray())[1].ToString() + "'");
            //
            //chart.Width = 800;// (Int32)(dt.Rows.Count / TagName.Split(',').Length + 300);
            //chart.Height = 600;
            //
            //double YMax = 0;
            //chart.Titles.Add(DeptDS.Tables[0].Rows[0][0].ToString());
            ////chart.Titles.Add("數值統計表");
            //if (dt.Rows.Count > 0)
            //{
            //    for (int i = 0; i <= dt.Rows.Count - 1; i++)
            //    {
            //        if (chart.Series.IndexOf(dt.Rows[i][1].ToString()) < 0)
            //        {
            //            chart.Series.Add(dt.Rows[i][1].ToString());
            //            chart.Series[dt.Rows[i][1].ToString()].ChartType = SeriesChartType.FastLine;
            //            chart.Series[dt.Rows[i][1].ToString()].IsVisibleInLegend = true;
            //            chart.Series[dt.Rows[i][1].ToString()].Legend = "Default";
            //            //chart.Series[dt.Rows[i][1].ToString()].BorderWidth = 2;
            //        }
            //
            //        chart.Series[dt.Rows[i][1].ToString()].Points.AddXY(Convert.ToDateTime(dt.Rows[i][3]), dt.Rows[i][2]);
            //        chart.Series[dt.Rows[i][1].ToString()].ToolTip = dt.Rows[i][2].ToString();
            //        chart.Series[dt.Rows[i][1].ToString()].IsValueShownAsLabel = true;
            //
            //        if (dt.Rows[i][2] != null && dt.Rows[i][2].ToString().ToUpper() != "NULL" && dt.Rows[i][2].ToString().Length > 0)
            //        {
            //            if (YMax < Convert.ToDouble(dt.Rows[i][2]))
            //            {
            //                YMax = Convert.ToDouble(dt.Rows[i][2]) + Convert.ToDouble(dt.Rows[i][2]) * 0.5;
            //            }
            //        }
            //    }
            //}
            ////chart.ChartAreas["ChartAreas1"].AxisY.Maximum = YMax + 5;
            ////MemoryStream ms = new MemoryStream();
            ////chart.SaveImage(ms, ChartImageFormat.Png);
            ////ms.Seek(0, SeekOrigin.Begin);
            //

            DateTime _StartDate = DateTime.Parse(StartDate);
            DateTime _EndDate = DateTime.Parse(EndDate);

            DataTable dt = null;

            if (_StartDate.AddHours(2) >= _EndDate)
            {

                dt = PLCCheckModel.GetChartValuedtDemo(vName, toolId, TagName, StartDate, EndDate, action);
            }
            else
            {
                dt = PLCCheckModel.GetChartValuedt(vName, toolId, TagName, StartDate, EndDate, action);
            }

            //建立一個新的Chart
            Chart chart = new Chart();

            //建立一個ChartAreas，並對ChartAreas設定格式
            chart.ChartAreas.Add("ChartAreas1");
            chart.ChartAreas["ChartAreas1"].BorderColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].BorderDashStyle = ChartDashStyle.Solid;
            chart.ChartAreas["ChartAreas1"].BackSecondaryColor = System.Drawing.Color.White;
            chart.ChartAreas["ChartAreas1"].BackColor = System.Drawing.Color.FromArgb(64, 165, 191, 228);
            chart.ChartAreas["ChartAreas1"].ShadowColor = System.Drawing.Color.Transparent;
            chart.ChartAreas["ChartAreas1"].BackGradientStyle = GradientStyle.TopBottom;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.Rotation = 10;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.Perspective = 10;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.Inclination = 15;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.IsRightAngleAxes = true;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.WallWidth = 0;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.IsClustered = true;
            chart.ChartAreas["ChartAreas1"].AxisY.IsStartedFromZero = false;
            chart.ChartAreas["ChartAreas1"].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chart.ChartAreas["ChartAreas1"].AxisY.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25f, System.Drawing.FontStyle.Bold);
            chart.ChartAreas["ChartAreas1"].AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisX.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25f, System.Drawing.FontStyle.Bold);
            chart.ChartAreas["ChartAreas1"].AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisX.LabelStyle.Format = "yy-MM-dd \n HH:mm:ss";
            chart.ChartAreas["ChartAreas1"].AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
            chart.ChartAreas["ChartAreas1"].AxisX.IntervalType = DateTimeIntervalType.Seconds;

            //chart.ChartAreas["ChartAreas1"].AxisX.IsLabelAutoFit = false;
            chart.ChartAreas["ChartAreas1"].AxisY.IsLabelAutoFit = true;
            //chart.ChartAreas["ChartAreas1"].AxisX.IntervalType = DateTimeIntervalType.Hours;
            chart.ChartAreas["ChartAreas1"].AxisX.LabelStyle.TruncatedLabels = false;
            chart.ChartAreas["ChartAreas1"].AxisX.IsInterlaced = true;

            chart.ChartAreas["ChartAreas1"].AxisY2.Enabled = AxisEnabled.Auto;
            chart.ChartAreas["ChartAreas1"].AxisY2.IsStartedFromZero = false;
            chart.ChartAreas["ChartAreas1"].AxisY2.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chart.ChartAreas["ChartAreas1"].AxisY2.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisY2.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25f, System.Drawing.FontStyle.Bold);
            chart.ChartAreas["ChartAreas1"].AxisY2.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);


            //----
            //chart.ChartAreas["ChartAreas1"].AxisY.ScaleBreakStyle.Enabled = true;
            //chart.ChartAreas["ChartAreas1"].AxisY.ScaleBreakStyle.CollapsibleSpaceThreshold = 10;
            //chart.ChartAreas["ChartAreas1"].AxisY.ScaleBreakStyle.MaxNumberOfBreaks = 2;
            //chart.ChartAreas["ChartAreas1"].AxisY.ScaleBreakStyle.LineDashStyle = ChartDashStyle.Solid;
            //chart.ChartAreas["ChartAreas1"].AxisY.ScaleBreakStyle.LineWidth = 2;
            //chart.ChartAreas["ChartAreas1"].AxisY.ScaleBreakStyle.Spacing = 3;
            //Spacing
            //----

            chart.Legends.Add("Default");
            chart.Legends["Default"].Alignment = System.Drawing.StringAlignment.Near;
            chart.Legends["Default"].Docking = Docking.Top;

            /* 20140214 ISABELLA */
            //設置X軸座標的間隔為1
            //chart.ChartAreas["ChartAreas1"].AxisX.Interval = 1;
            //設置X軸座標偏移為1
            //chart.ChartAreas["ChartAreas1"].AxisX.IntervalOffset = 1;
            ////設置是否交錯顯示，比如數據多時分成兩行來顯示
            //chart.ChartAreas["ChartAreas1"].AxisX.LabelStyle.IsStaggered = true;
            //string tName = TagName.Split(',')[0].ToString();
            //tName = tName.Substring(7, tName.Length-1);
            //tName = tName.Substring(toolId.Split(',')[0].ToString().Length-1);
            //DataSet DeptDS = DBConnector.executeQuery("Intouch", "SELECT Cht_Comment FROM analog_comment where Tag_Name = '" + TagName.Split(',')[0].Substring(7 + toolId.Split(',')[0].ToString().Length, TagName.Split(',')[0].ToString().Length - (7 + toolId.Split(',')[0].ToString().Length)) + "'");
            //DataSet DeptDS = DBConnector.executeQuery("Intouch", "SELECT Cht_Comment FROM analog_comment where Tag_Name = '" + TagName.Split(toolId.Split(',').Last().ToCharArray())[1].ToString() + "'");

            chart.Width = 800;// (Int32)(dt.Rows.Count / TagName.Split(',').Length + 300);
            chart.Height = 600;
            //chart.Titles.Add(DeptDS.Tables[0].Rows[0][0].ToString());

            //string[] Y1Y2arr = Y1Y2.Split(',');

            //double YMax = 0;

            //chart.Titles.Add("數值統計表");
            if (dt.Rows.Count > 0)
            {
                System.Drawing.Color[] colors = { System.Drawing.Color.DarkBlue, System.Drawing.Color.DarkOrange, System.Drawing.Color.DarkRed, System.Drawing.Color.DarkSeaGreen, System.Drawing.Color.DarkViolet, System.Drawing.Color.DarkOliveGreen, System.Drawing.Color.DarkMagenta, System.Drawing.Color.DarkGreen, System.Drawing.Color.DarkCyan, System.Drawing.Color.DarkOrchid };

                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    if (chart.Series.IndexOf(dt.Rows[i][1].ToString() + dt.Rows[i][5].ToString()) < 0)
                    {
                        chart.Series.Add(dt.Rows[i][1].ToString() + dt.Rows[i][5].ToString());
                        chart.Series[dt.Rows[i][1].ToString() + dt.Rows[i][5].ToString()].ChartType = SeriesChartType.FastLine;
                        chart.Series[dt.Rows[i][1].ToString() + dt.Rows[i][5].ToString()].IsVisibleInLegend = true;
                        chart.Series[dt.Rows[i][1].ToString() + dt.Rows[i][5].ToString()].Legend = "Default";
                        chart.Series[dt.Rows[i][1].ToString() + dt.Rows[i][5].ToString()].BorderWidth = 2;
                        if (colors.Count() > chart.Series.Count)
                        {
                            chart.Series[dt.Rows[i][1].ToString() + dt.Rows[i][5].ToString()].Color = colors[chart.Series.Count - 1];
                        }
                    }

                    chart.Series[dt.Rows[i][1].ToString() + dt.Rows[i][5].ToString()].Points.AddXY(Convert.ToDateTime(dt.Rows[i][3]), (double)dt.Rows[i][2]);
                    chart.Series[dt.Rows[i][1].ToString() + dt.Rows[i][5].ToString()].ToolTip = dt.Rows[i][2].ToString();
                    chart.Series[dt.Rows[i][1].ToString() + dt.Rows[i][5].ToString()].IsValueShownAsLabel = true;

                }
            }

            return chart;
        }

        public static Chart GetChartValue3(string vName, string toolId, string TagName, string StartDate, string EndDate, string action, string Y1Y2, bool isLimit, double? YMax, double? YMin, bool isStack = false)
        {
            DateTime _StartDate = DateTime.Parse(StartDate);
            DateTime _EndDate = DateTime.Parse(EndDate);

            DataTable dt = null;

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["DB"]) && System.Configuration.ConfigurationManager.AppSettings["DB"] == "KH")
            {
                dt = PLCCheckModel.GetChartValueKHdt(vName, toolId, TagName, StartDate, EndDate, System.Configuration.ConfigurationManager.AppSettings["KHDBIP"], action);
            }
            else
            {
                if (_StartDate.AddDays(7) >= _EndDate)
                {
                    dt = PLCCheckModel.GetChartValuedtDemo(vName, toolId, TagName, StartDate, EndDate, action);
                }
                else
                {
                    dt = PLCCheckModel.GetChartValuedt(vName, toolId, TagName, StartDate, EndDate, action);
                }
            }

            //建立一個新的Chart
            Chart chart = new Chart();

            //建立一個ChartAreas，並對ChartAreas設定格式
            chart.ChartAreas.Add("ChartAreas1");
            chart.ChartAreas["ChartAreas1"].BorderColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].BorderDashStyle = ChartDashStyle.Solid;
            chart.ChartAreas["ChartAreas1"].BackSecondaryColor = System.Drawing.Color.White;
            chart.ChartAreas["ChartAreas1"].BackColor = System.Drawing.Color.FromArgb(64, 165, 191, 228);
            chart.ChartAreas["ChartAreas1"].ShadowColor = System.Drawing.Color.Transparent;
            chart.ChartAreas["ChartAreas1"].BackGradientStyle = GradientStyle.TopBottom;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.Rotation = 10;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.Perspective = 10;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.Inclination = 15;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.IsRightAngleAxes = true;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.WallWidth = 0;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.IsClustered = true;
            chart.ChartAreas["ChartAreas1"].AxisY.IsStartedFromZero = false;
            chart.ChartAreas["ChartAreas1"].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chart.ChartAreas["ChartAreas1"].AxisY.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25f, System.Drawing.FontStyle.Regular);
            chart.ChartAreas["ChartAreas1"].AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisY.IsLabelAutoFit = true;
            chart.ChartAreas["ChartAreas1"].AxisY.IntervalType = DateTimeIntervalType.Number;
            chart.ChartAreas["ChartAreas1"].AxisY.LabelStyle.Format = "{0:0.##}";
            chart.ChartAreas["ChartAreas1"].AxisY.MajorTickMark.IntervalType = DateTimeIntervalType.Number;

            chart.ChartAreas["ChartAreas1"].AxisX.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25f, System.Drawing.FontStyle.Regular);
            chart.ChartAreas["ChartAreas1"].AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisX.LabelStyle.Format = "yy-MM-dd \n HH:mm:ss";
            chart.ChartAreas["ChartAreas1"].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chart.ChartAreas["ChartAreas1"].AxisX.IntervalType = DateTimeIntervalType.Auto;
            chart.ChartAreas["ChartAreas1"].AxisX.LabelStyle.TruncatedLabels = false;
            chart.ChartAreas["ChartAreas1"].AxisX.IsInterlaced = true;
            chart.ChartAreas["ChartAreas1"].AxisX.MajorTickMark.IntervalType = DateTimeIntervalType.Auto;
            chart.ChartAreas["ChartAreas1"].AxisX.IsLabelAutoFit = false;

            //chart.ChartAreas["ChartAreas1"].AxisX.Interval = 1;
            //chart.ChartAreas["ChartAreas1"].AxisX.IntervalOffset = 1;

            chart.ChartAreas["ChartAreas1"].AxisY2.Enabled = AxisEnabled.Auto;
            chart.ChartAreas["ChartAreas1"].AxisY2.IsStartedFromZero = false;
            chart.ChartAreas["ChartAreas1"].AxisY2.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chart.ChartAreas["ChartAreas1"].AxisY2.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisY2.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25f, System.Drawing.FontStyle.Bold);
            chart.ChartAreas["ChartAreas1"].AxisY2.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisY2.LabelStyle.Format = "{0:0.##}";

            //chart.ChartAreas["ChartAreas1"].Position.X = 50;
            //chart.ChartAreas["ChartAreas1"].InnerPlotPosition.X = 0;

            //----
            //chart.ChartAreas["ChartAreas1"].AxisY.ScaleBreakStyle.Enabled = true;
            //chart.ChartAreas["ChartAreas1"].AxisY.ScaleBreakStyle.CollapsibleSpaceThreshold = 10;
            //chart.ChartAreas["ChartAreas1"].AxisY.ScaleBreakStyle.MaxNumberOfBreaks = 2;
            //chart.ChartAreas["ChartAreas1"].AxisY.ScaleBreakStyle.LineDashStyle = ChartDashStyle.Solid;
            //chart.ChartAreas["ChartAreas1"].AxisY.ScaleBreakStyle.LineWidth = 2;
            //chart.ChartAreas["ChartAreas1"].AxisY.ScaleBreakStyle.Spacing = 3;
            //Spacing
            //----

            chart.Legends.Add("Default");
            chart.Legends["Default"].Alignment = System.Drawing.StringAlignment.Near;
            chart.Legends["Default"].Docking = Docking.Top;

            /* 20140214 ISABELLA */
            //設置X軸座標的間隔為1
            //chart.ChartAreas["ChartAreas1"].AxisX.Interval = 1;
            //設置X軸座標偏移為1
            //chart.ChartAreas["ChartAreas1"].AxisX.IntervalOffset = 1;
            ////設置是否交錯顯示，比如數據多時分成兩行來顯示
            //chart.ChartAreas["ChartAreas1"].AxisX.LabelStyle.IsStaggered = true;
            //string tName = TagName.Split(',')[0].ToString();
            //tName = tName.Substring(7, tName.Length-1);
            //tName = tName.Substring(toolId.Split(',')[0].ToString().Length-1);
            //DataSet DeptDS = DBConnector.executeQuery("Intouch", "SELECT Cht_Comment FROM analog_comment where Tag_Name = '" + TagName.Split(',')[0].Substring(7 + toolId.Split(',')[0].ToString().Length, TagName.Split(',')[0].ToString().Length - (7 + toolId.Split(',')[0].ToString().Length)) + "'");
            //DataSet DeptDS = DBConnector.executeQuery("Intouch", "SELECT Cht_Comment FROM analog_comment where Tag_Name = '" + TagName.Split(toolId.Split(',').Last().ToCharArray())[1].ToString() + "'");

            chart.Width = 800;// (Int32)(dt.Rows.Count / TagName.Split(',').Length + 300);
            chart.Height = 600;
            //chart.Titles.Add(DeptDS.Tables[0].Rows[0][0].ToString());

            string[] Y1Y2arr = Y1Y2.Split(',');

            //chart.Titles.Add("數值統計表");
            if (dt.Rows.Count > 0)
            {
                System.Drawing.Color[] colors = { System.Drawing.Color.DarkBlue, System.Drawing.Color.DarkOrange, System.Drawing.Color.DarkRed, System.Drawing.Color.DarkSeaGreen, System.Drawing.Color.DarkViolet, System.Drawing.Color.DarkOliveGreen, System.Drawing.Color.DarkMagenta, System.Drawing.Color.DarkGreen, System.Drawing.Color.DarkCyan, System.Drawing.Color.DarkOrchid };

                using (tsmc14BDataContext db = new tsmc14BDataContext())
                {
                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        if (chart.Series.IndexOf(dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()) < 0)
                        {
                            int pid = (from row in db.vw_eq_status where row.chamberName == dt.Rows[i][1].ToString() select row.pid).SingleOrDefault();

                            chart.Series.Add(dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString());
                            chart.Series[dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].ChartType = SeriesChartType.FastLine;
                            chart.Series[dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].IsVisibleInLegend = true;
                            chart.Series[dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].Legend = "Default";
                            chart.Series[dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].BorderWidth = 2;
                            chart.Series[dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].XValueType = ChartValueType.DateTime;
                            chart.Series[dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].Tag = pid;

                            if (colors.Count() > chart.Series.Count)
                            {
                                chart.Series[dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].Color = colors[chart.Series.Count - 1];
                            }
                            if (Y1Y2arr.Length >= chart.Series.Count - 1 && Y1Y2arr[chart.Series.Count - 1] == "Y2")
                            {
                                chart.Series[dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].YAxisType = AxisType.Secondary;
                            }
                        }

                        chart.Series[dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].Points.AddXY(Convert.ToDateTime(dt.Rows[i]["DataTime"]), Convert.ToDouble(dt.Rows[i]["Value"]));

                        //if ((from row in r where row.builtdate<=Convert.ToDateTime(dt.Rows[i][3]) &&  row.PMsetDateTime>=Convert.ToDateTime(dt.Rows[i][3]) select row).Any())
                        //{
                        //DataPoint dp = chart.Series[dt.Rows[i][1].ToString()  + dt.Rows[i][5].ToString()].Points.Last();
                        //dp.Color = System.Drawing.Color.Red;
                        //}
                        //chart.Series[dt.Rows[i][1].ToString()  + dt.Rows[i][5].ToString()].ToolTip = dt.Rows[i][2].ToString();
                        //chart.Series[dt.Rows[i][1].ToString()  + dt.Rows[i][5].ToString()].IsValueShownAsLabel = true;
                        //if (dt.Rows[i][2] != null && dt.Rows[i][2].ToString().ToUpper() != "NULL" && dt.Rows[i][2].ToString().Length > 0)
                        //{

                        //}
                    }

                    //foreach (Series ss in chart.Series)
                    //{
                    //    foreach (var item in from row in db.FDCPMHistory where row.plc_id == (int)ss.Tag && DateTime.Parse(StartDate) <= row.PMsetDateTime && DateTime.Parse(EndDate) >= row.builtdate select row)
                    //    {
                    //        var rr = from p in ss.Points where p.XValue > item.builtdate.ToOADate() && p.XValue < item.PMsetDateTime.ToOADate() select p;
                    //        foreach (DataPoint dp in rr)
                    //        {
                    //            //dp.MarkerStyle = MarkerStyle.Diamond;
                    //            //dp.MarkerSize = 10;
                    //            //dp.MarkerColor = System.Drawing.Color.Red;
                    //            //dp.BorderColor = System.Drawing.Color.Red;
                    //            dp.Color = System.Drawing.Color.Red;
                    //        }
                    //    }
                    //}

                    //foreach (DataPoint dp in chart.Series[0].Points)
                    //{
                    //dp.MarkerStyle = MarkerStyle.Diamond;
                    //dp.MarkerSize = 10;
                    //dp.MarkerColor = System.Drawing.Color.Red;
                    //dp.BorderColor = System.Drawing.Color.Red;
                    //dp.Color = System.Drawing.Color.Red;
                    //}

                    string[] TagNameArr = TagName.Split(',');

                    int seriesCount = chart.Series.Count;

                    foreach (String item in TagNameArr)
                    {
                        DataTable LimitDt = PLCCheckModel.GetLimitValuedt(item);

                        var r = (from row in db.Limit_history where row.TagName == item && row.builddate <= Convert.ToDateTime(EndDate) && row.builddate >= Convert.ToDateTime(StartDate) orderby row.builddate descending select row).ToList();

                        if (LimitDt != null && LimitDt.Rows.Count > 0)
                        {
                            string MaxSeriesName = LimitDt.Rows[0]["tool_id"].ToString() + LimitDt.Rows[0]["Cht_Comment"].ToString() + "上限";
                            string MinSeriesName = LimitDt.Rows[0]["tool_id"].ToString() + LimitDt.Rows[0]["Cht_Comment"].ToString() + "下限";

                            DataPoint smax = chart.Series[LimitDt.Rows[0]["tool_id"].ToString() + LimitDt.Rows[0]["Cht_Comment"].ToString()].Points.FindMaxByValue();
                            DataPoint smin = chart.Series[LimitDt.Rows[0]["tool_id"].ToString() + LimitDt.Rows[0]["Cht_Comment"].ToString()].Points.FindMinByValue();
                            DataRow limitRow = LimitDt.Rows[0];

                            if ((!limitRow.IsNull("Limit_Max_Setting") && (((double)limitRow["Limit_Max_Setting"] < 32766 && (double)limitRow["Limit_Max_Setting"] * 0.9 < smax.YValues[0]) || (isLimit && (double)limitRow["Limit_Max_Setting"] < 32766)) && (YMax == null || (YMax != null && (double)limitRow["Limit_Max_Setting"] < YMax))) && (!isStack || seriesCount == 1))
                            {
                                chart.Series.Add(MaxSeriesName);

                                chart.Series[MaxSeriesName].ChartType = SeriesChartType.Line;
                                chart.Series[MaxSeriesName].BorderDashStyle = ChartDashStyle.Dash;
                                chart.Series[MaxSeriesName].IsVisibleInLegend = false;
                                chart.Series[MaxSeriesName].Legend = "Default";
                                //chart.Series[MaxSeriesName].Color = chart.Series[j].Color;
                                chart.Series[MaxSeriesName].Color = System.Drawing.Color.Red;
                                chart.Series[MaxSeriesName].BorderWidth = 2;

                                //chart.Series[MaxSeriesName].Points.AddXY(Convert.ToDateTime(StartDate), LimitDt.Rows[0]["Limit_Max_Setting"]);
                                chart.Series[MaxSeriesName].Points.AddXY(Convert.ToDateTime(EndDate), LimitDt.Rows[0]["Limit_Max_Setting"]);

                                if (r.Count > 0)
                                {
                                    double? lastMaxLimitValue = null;
                                    foreach (Limit_history i in r)
                                    {
                                        if (i.builddate <= Convert.ToDateTime(StartDate))
                                        {
                                            lastMaxLimitValue = i.Limit_Max_Setting;
                                            chart.Series[MaxSeriesName].Points.AddXY(Convert.ToDateTime(StartDate), i.Limit_Max_Setting);
                                            break;
                                        }
                                        if (lastMaxLimitValue.HasValue)
                                        {
                                            chart.Series[MaxSeriesName].Points.AddXY(Convert.ToDateTime(i.builddate.AddSeconds(1)), lastMaxLimitValue);
                                            chart.Series[MaxSeriesName].Points.AddXY(Convert.ToDateTime(i.builddate), i.Limit_Max_Setting);
                                        }
                                        lastMaxLimitValue = i.Limit_Max_Setting;
                                    }
                                    if (r.Last().builddate >= Convert.ToDateTime(StartDate))
                                    {
                                        chart.Series[MaxSeriesName].Points.AddXY(Convert.ToDateTime(StartDate), lastMaxLimitValue);
                                    }
                                }
                                else
                                {
                                    chart.Series[MaxSeriesName].Points.AddXY(Convert.ToDateTime(StartDate), LimitDt.Rows[0]["Limit_Max_Setting"]);
                                }
                            }

                            if ((!limitRow.IsNull("Limit_Min_Setting") && (((double)limitRow["Limit_Min_Setting"] > -32767 && (double)limitRow["Limit_Min_Setting"] * 1.1 > smin.YValues[0]) || (isLimit && (double)limitRow["Limit_Min_Setting"] > -32767)) && (YMin == null || (YMin != null && (double)limitRow["Limit_Min_Setting"] > YMin))) && (!isStack || seriesCount == 1))
                            {
                                chart.Series.Add(MinSeriesName);

                                chart.Series[MinSeriesName].ChartType = SeriesChartType.Line;
                                chart.Series[MinSeriesName].BorderDashStyle = ChartDashStyle.Dash;
                                chart.Series[MinSeriesName].IsVisibleInLegend = false;
                                chart.Series[MinSeriesName].Legend = "Default";
                                //chart.Series[MinSeriesName].Color = chart.Series[j].Color;
                                chart.Series[MinSeriesName].Color = System.Drawing.Color.Red;
                                chart.Series[MinSeriesName].BorderWidth = 2;

                                //chart.Series[MinSeriesName].Points.AddXY(Convert.ToDateTime(StartDate), LimitDt.Rows[0]["Limit_Min_Setting"]);
                                chart.Series[MinSeriesName].Points.AddXY(Convert.ToDateTime(EndDate), LimitDt.Rows[0]["Limit_Min_Setting"]);

                                if (r.Count > 0)
                                {
                                    double? lastMinLimitValue = null;
                                    foreach (Limit_history i in r)
                                    {
                                        if (i.builddate <= Convert.ToDateTime(StartDate))
                                        {
                                            lastMinLimitValue = i.Limit_Min_Setting;
                                            chart.Series[MinSeriesName].Points.AddXY(Convert.ToDateTime(StartDate), i.Limit_Min_Setting);
                                            break;
                                        }
                                        if (lastMinLimitValue.HasValue)
                                        {
                                            chart.Series[MinSeriesName].Points.AddXY(Convert.ToDateTime(i.builddate.AddSeconds(1)), lastMinLimitValue);
                                            chart.Series[MinSeriesName].Points.AddXY(Convert.ToDateTime(i.builddate), i.Limit_Min_Setting);
                                        }
                                        lastMinLimitValue = i.Limit_Min_Setting;
                                    }
                                    if (r.Last().builddate >= Convert.ToDateTime(StartDate))
                                    {
                                        chart.Series[MinSeriesName].Points.AddXY(Convert.ToDateTime(StartDate), lastMinLimitValue);
                                    }
                                }
                                else
                                {
                                    chart.Series[MinSeriesName].Points.AddXY(Convert.ToDateTime(StartDate), LimitDt.Rows[0]["Limit_Min_Setting"]);
                                }
                            }
                        }
                    }
                }

                foreach (Series item in chart.Series)
                {
                    if (!item.Name.Contains("限"))
                    {
                        if (YMax != null && YMin != null)
                        {
                            var rr = from row in item.Points where row.YValues[0] < YMax.Value && row.YValues[0] > YMin.Value select row;
                            var rrr = from row in item.Points where row.YValues[0] < YMax.Value && row.YValues[0] > YMin.Value orderby row.YValues[0] select row;

                            //if (rr.Count()<=6)
                            //{
                            //    foreach (var itemRow in rr)
                            //    {
                            //        itemRow.IsValueShownAsLabel = true;
                            //        itemRow.Label = DateTime.FromOADate(itemRow.XValue).ToString("HH:mm:ss") + " " + Math.Round(itemRow.YValues[0], 2);
                            //        itemRow.Font = new System.Drawing.Font("Trebuchet MS", 10, System.Drawing.FontStyle.Bold);
                            //    }
                            //    item.ChartType = SeriesChartType.Line;
                            //}

                            if (Convert.ToDateTime(StartDate).AddMinutes(3) > Convert.ToDateTime(EndDate))
                            {
                                double TempYValue = 0;

                                foreach (var pointitem in rr)
                                {
                                    if (pointitem.YValues[0] != TempYValue)
                                    {
                                        pointitem.IsValueShownAsLabel = true;
                                        pointitem.Label = DateTime.FromOADate(pointitem.XValue).ToString("HH:mm:ss") + " " + Math.Round(pointitem.YValues[0], 2);
                                        pointitem.Font = new System.Drawing.Font("Trebuchet MS", 10, System.Drawing.FontStyle.Bold);
                                        pointitem.MarkerSize = 1;
                                        pointitem.MarkerStyle = MarkerStyle.Circle;

                                        TempYValue = pointitem.YValues[0];
                                    }
                                }

                                DataPoint maxdp = rrr.LastOrDefault();

                                DataPoint mindp = rrr.FirstOrDefault();

                                if (maxdp != null && mindp != null)
                                {
                                    maxdp.IsValueShownAsLabel = true;
                                    mindp.IsValueShownAsLabel = true;

                                    maxdp.Label = DateTime.FromOADate(maxdp.XValue).ToString("HH:mm:ss") + " " + Math.Round(maxdp.YValues[0], 2);
                                    mindp.Label = DateTime.FromOADate(mindp.XValue).ToString("HH:mm:ss") + " " + Math.Round(mindp.YValues[0], 2);

                                    maxdp.Font = new System.Drawing.Font("Trebuchet MS", 10, System.Drawing.FontStyle.Bold);
                                    mindp.Font = new System.Drawing.Font("Trebuchet MS", 10, System.Drawing.FontStyle.Bold);
                                }

                                item.ChartType = SeriesChartType.Line;
                            }
                            else
                            {
                                item.ChartType = SeriesChartType.FastLine;
                            }

                            //foreach (var itemRow in dp2)
                            //{
                            //    itemRow.IsValueShownAsLabel = true;
                            //    itemRow.Label = DateTime.FromOADate(itemRow.XValue).ToString("HH:mm:ss") + " " + Math.Round(itemRow.YValues[0], 2);
                            //    itemRow.Font = new System.Drawing.Font("Trebuchet MS", 10, System.Drawing.FontStyle.Bold);
                            //}
                        }
                        else
                        {
                            item.ChartType = SeriesChartType.FastLine;
                        }
                    }
                }

                //StripLine sl = new StripLine();
                //sl.Text = dt.Rows[0][5].ToString();
                //sl.BorderColor = System.Drawing.Color.Red;
                //sl.IntervalOffset = 8;
                //sl.TextLineAlignment = System.Drawing.StringAlignment.Near;
                //sl.TextAlignment = System.Drawing.StringAlignment.Far;
                //chart.ChartAreas[0].AxisY2.CustomLabels.Add(10, 10, dt.Rows[0][5].ToString());
                ////sl.StripWidthType = DateTimeIntervalType.Auto;
                ////sl.StripWidth = 0.5;
                ////chart.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;
                ////chart.ChartAreas[0].AxisY2.Maximum = chart.ChartAreas[0].AxisY.Maximum;
                ////chart.ChartAreas[0].AxisY2.Minimum = chart.ChartAreas[0].AxisY.Minimum;
                //chart.ChartAreas["ChartAreas1"].AxisY.StripLines.Add(sl);

            }
            if (YMax != null)
            {
                chart.ChartAreas["ChartAreas1"].AxisY.Maximum = YMax.Value;
            }

            if (YMin != null)
            {
                chart.ChartAreas["ChartAreas1"].AxisY.Minimum = YMin.Value;
            }

            return chart;
        }

        internal static Chart GetChartValue4(string vName, string toolId, string TagName, string StartDate, string EndDate, string action)
        {
            DataTable dt = null;

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["DB"]) && System.Configuration.ConfigurationManager.AppSettings["DB"] == "KH")
            {
                dt = PLCCheckModel.GetChartValueKHdt(vName, toolId, TagName, StartDate, EndDate, System.Configuration.ConfigurationManager.AppSettings["KHDBIP"], action);
            }
            else
            {
                dt = PLCCheckModel.GetChartValuedt(vName, toolId, TagName, StartDate, EndDate, action);
            }

            //DataTable dt = null;
            //if (action=="5") {
            //    dt = PLCCheckModel.GetChartValuedtDemo(vName, toolId, TagName, StartDate, EndDate, action);
            //}
            //else
            //{
            //    dt = PLCCheckModel.GetChartValuedt(vName, toolId, TagName, StartDate, EndDate, action);
            //}

            //建立一個新的Chart
            Chart chart = new Chart();

            //建立一個ChartAreas，並對ChartAreas設定格式

            /* 20140214 ISABELLA */
            //設置X軸座標的間隔為1
            //chart.ChartAreas["ChartAreas1"].AxisX.Interval = 1;
            //設置X軸座標偏移為1
            //chart.ChartAreas["ChartAreas1"].AxisX.IntervalOffset = 1;
            ////設置是否交錯顯示，比如數據多時分成兩行來顯示
            //chart.ChartAreas["ChartAreas1"].AxisX.LabelStyle.IsStaggered = true;
            //string tName = TagName.Split(',')[0].ToString();
            //tName = tName.Substring(7, tName.Length-1);
            //tName = tName.Substring(toolId.Split(',')[0].ToString().Length-1);
            //DataSet DeptDS = DBConnector.executeQuery("Intouch", "SELECT Cht_Comment FROM analog_comment where Tag_Name = '" + TagName.Split(',')[0].Substring(7 + toolId.Split(',')[0].ToString().Length, TagName.Split(',')[0].ToString().Length - (7 + toolId.Split(',')[0].ToString().Length)) + "'");
            //DataSet DeptDS = DBConnector.executeQuery("Intouch", "SELECT Cht_Comment FROM analog_comment where Tag_Name = '" + TagName.Split(toolId.Split(',').Last().ToCharArray())[1].ToString() + "'");
            chart.Width = 800;// (Int32)(dt.Rows.Count / TagName.Split(',').Length + 300);
            chart.Height = 100;
            var chartHeight = 100;
            var chartSeriesCount = 100;

            //chart.Titles.Add(DeptDS.Tables[0].Rows[0][0].ToString());
            double YMax = 0;

            //chart.Titles.Add("數值統計表");
            if (dt.Rows.Count > 0)
            {
                DateTime StartPointDate = Convert.ToDateTime(dt.AsEnumerable().Max(x => x["DataTime"]));
                DateTime EndPointDate = Convert.ToDateTime(dt.AsEnumerable().Min(x => x["DataTime"]));

                System.Drawing.Color[] colors = { System.Drawing.Color.DarkBlue, System.Drawing.Color.DarkOrange, System.Drawing.Color.DarkRed, System.Drawing.Color.DarkSeaGreen, System.Drawing.Color.DarkViolet, System.Drawing.Color.DarkOliveGreen, System.Drawing.Color.DarkMagenta, System.Drawing.Color.DarkGreen, System.Drawing.Color.DarkCyan, System.Drawing.Color.DarkOrchid };
                string ChartAreas = string.Empty;
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    if (chart.Series.IndexOf(dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()) < 0)
                    {
                        YMax = 0;
                        ChartAreas = "ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString();
                        chart.ChartAreas.Add("ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString());
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].BorderColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].BorderDashStyle = ChartDashStyle.Solid;
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].BackSecondaryColor = System.Drawing.Color.White;
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].BackColor = System.Drawing.Color.FromArgb(64, 165, 191, 228);
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].ShadowColor = System.Drawing.Color.Transparent;
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].BackGradientStyle = GradientStyle.TopBottom;
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].Area3DStyle.Rotation = 10;
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].Area3DStyle.Perspective = 10;
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].Area3DStyle.Inclination = 15;
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].Area3DStyle.IsRightAngleAxes = true;
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].Area3DStyle.WallWidth = 0;
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].Area3DStyle.IsClustered = true;
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].AxisY.IsStartedFromZero = false;
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].AxisY.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25f, System.Drawing.FontStyle.Bold);
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].AxisX.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25f, System.Drawing.FontStyle.Bold);
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].AxisX.LabelStyle.Format = "yyyy-MM-dd \n HH:mm:ss";
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].AxisX.IsLabelAutoFit = false;
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].AxisY.IsLabelAutoFit = true;
                        //chart.ChartAreas["ChartAreas1"].AxisX.IntervalType = DateTimeIntervalType.Hours;
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].AxisX.LabelStyle.TruncatedLabels = true;
                        chart.ChartAreas["ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].AxisX.IsInterlaced = true;

                        chart.Legends.Add("Default" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString());
                        chart.Legends["Default" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].Alignment = System.Drawing.StringAlignment.Near;
                        chart.Legends["Default" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].MaximumAutoSize = 5;
                        chart.Legends["Default" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].Docking = Docking.Top;
                        chart.Legends["Default" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].DockedToChartArea = "ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString();
                        chart.Legends["Default" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].BackColor = System.Drawing.Color.FromArgb(0, 0, 0, 0);
                        chart.Legends["Default" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].Font = new System.Drawing.Font("Trebuchet MS", 8.25f, System.Drawing.FontStyle.Bold);

                        chart.Series.Add(dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString());
                        chart.Series[dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].ChartType = SeriesChartType.FastLine;
                        chart.Series[dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].IsVisibleInLegend = true;
                        chart.Series[dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].Legend = "Default" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString();
                        chart.Series[dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].BorderWidth = 2;
                        chart.Series[dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].ChartArea = "ChartAreas1" + dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString();

                        if (colors.Count() > chart.Series.Count)
                        {
                            chart.Series[dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].Color = colors[chart.Series.Count - 1];
                        }
                    }

                    chart.Series[dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].Points.AddXY(Convert.ToDateTime(dt.Rows[i]["DataTime"]), dt.Rows[i]["Value"]);
                    chart.Series[dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].ToolTip = dt.Rows[i]["Value"].ToString();
                    chart.Series[dt.Rows[i]["tool_id"].ToString() + dt.Rows[i]["Cht_Comment"].ToString()].IsValueShownAsLabel = true;

                    if (dt.Rows[i]["Value"] != null && dt.Rows[i]["Value"].ToString().ToUpper() != "NULL" && dt.Rows[i]["Value"].ToString().Length > 0)
                    {
                        if (YMax < Convert.ToDouble(dt.Rows[i]["Value"]))
                        {
                            YMax = Convert.ToDouble(dt.Rows[i]["Value"]) + Convert.ToDouble(dt.Rows[i]["Value"]) * 0.5;
                        }
                    }
                    if (YMax < -1 || YMax > 1)
                    {
                        //chart.ChartAreas["ChartAreas1" + dt.Rows[i][1].ToString()  + dt.Rows[i]["Cht_Comment"].ToString()].AxisY.Maximum = YMax + 5;
                    }

                }
                chartHeight = 300 + (chart.Series.Count * 300);
                chartSeriesCount = chart.Series.Count;
                string[] TagNameArr = TagName.Split(',');

                foreach (String item in TagNameArr)
                {
                    DataTable LimitDt = PLCCheckModel.GetLimitValuedt(item);

                    if (LimitDt != null && LimitDt.Rows.Count > 0)
                    {
                        string MaxSeriesName = LimitDt.Rows[0]["tool_id"].ToString() + LimitDt.Rows[0]["Cht_Comment"].ToString() + "上限";
                        string MinSeriesName = LimitDt.Rows[0]["tool_id"].ToString() + LimitDt.Rows[0]["Cht_Comment"].ToString() + "下限";

                        DataPoint smax = chart.Series[LimitDt.Rows[0]["tool_id"].ToString() + LimitDt.Rows[0]["Cht_Comment"].ToString()].Points.FindMaxByValue();
                        DataPoint smin = chart.Series[LimitDt.Rows[0]["tool_id"].ToString() + LimitDt.Rows[0]["Cht_Comment"].ToString()].Points.FindMinByValue();
                        DataRow limitRow = LimitDt.Rows[0];

                        if (!limitRow.IsNull("Limit_Max_Setting") && (double)limitRow["Limit_Max_Setting"] < 32766 && (double)limitRow["Limit_Max_Setting"] * 0.9 < smax.YValues[0] || !limitRow.IsNull("Limit_Max_Setting") && (double)limitRow["Limit_Max_Setting"] < 32766)
                        {
                            chart.Series.Add(MaxSeriesName);

                            chart.Series[MaxSeriesName].ChartType = SeriesChartType.Line;
                            chart.Series[MaxSeriesName].BorderDashStyle = ChartDashStyle.Dash;
                            chart.Series[MaxSeriesName].IsVisibleInLegend = false;
                            chart.Series[MaxSeriesName].Legend = "Default" + LimitDt.Rows[0]["tool_id"].ToString() + LimitDt.Rows[0]["Cht_Comment"].ToString();
                            //chart.Series[MaxSeriesName].Color = chart.Series[j].Color;
                            chart.Series[MaxSeriesName].Color = System.Drawing.Color.Red;
                            chart.Series[MaxSeriesName].BorderWidth = 2;
                            chart.Series[MaxSeriesName].Points.AddXY(Convert.ToDateTime(StartDate), LimitDt.Rows[0]["Limit_Max_Setting"]);
                            chart.Series[MaxSeriesName].Points.AddXY(Convert.ToDateTime(EndDate), LimitDt.Rows[0]["Limit_Max_Setting"]);
                            chart.Series[MaxSeriesName].ChartArea = "ChartAreas1" + LimitDt.Rows[0]["tool_id"].ToString() + LimitDt.Rows[0]["Cht_Comment"].ToString();
                            //if (YMax < Convert.ToDouble(LimitDt.Rows[0]["Limit_Max_Setting"]))
                            //{
                            //    YMax = Convert.ToDouble(LimitDt.Rows[0]["Limit_Max_Setting"]) * 1.5;
                            //}
                        }

                        if (!limitRow.IsNull("Limit_Min_Setting") && (double)limitRow["Limit_Min_Setting"] > -32767 && (double)limitRow["Limit_Min_Setting"] * 1.1 > smin.YValues[0] || !limitRow.IsNull("Limit_Min_Setting") && (double)limitRow["Limit_Min_Setting"] > -32767)
                        {
                            chart.Series.Add(MinSeriesName);

                            chart.Series[MinSeriesName].ChartType = SeriesChartType.Line;
                            chart.Series[MinSeriesName].BorderDashStyle = ChartDashStyle.Dash;
                            chart.Series[MinSeriesName].IsVisibleInLegend = false;
                            chart.Series[MinSeriesName].Legend = "Default" + LimitDt.Rows[0]["tool_id"].ToString() + LimitDt.Rows[0]["Cht_Comment"].ToString();
                            //chart.Series[MinSeriesName].Color = chart.Series[j].Color;
                            chart.Series[MinSeriesName].Color = System.Drawing.Color.Red;
                            chart.Series[MinSeriesName].BorderWidth = 2;
                            chart.Series[MinSeriesName].Points.AddXY(Convert.ToDateTime(StartDate), LimitDt.Rows[0]["Limit_Min_Setting"]);
                            chart.Series[MinSeriesName].Points.AddXY(Convert.ToDateTime(EndDate), LimitDt.Rows[0]["Limit_Min_Setting"]);
                            chart.Series[MinSeriesName].ChartArea = "ChartAreas1" + LimitDt.Rows[0]["tool_id"].ToString() + LimitDt.Rows[0]["Cht_Comment"].ToString();
                        }
                    }
                }

                //StripLine sl = new StripLine();
                //sl.Text = dt.Rows[0][5].ToString();
                //sl.BorderColor = System.Drawing.Color.Red;
                //sl.IntervalOffset = 8;
                //sl.TextLineAlignment = System.Drawing.StringAlignment.Near;
                //sl.TextAlignment = System.Drawing.StringAlignment.Far;
                //chart.ChartAreas[0].AxisY2.CustomLabels.Add(10, 10, dt.Rows[0][5].ToString());
                ////sl.StripWidthType = DateTimeIntervalType.Auto;
                ////sl.StripWidth = 0.5;
                ////chart.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;
                ////chart.ChartAreas[0].AxisY2.Maximum = chart.ChartAreas[0].AxisY.Maximum;
                ////chart.ChartAreas[0].AxisY2.Minimum = chart.ChartAreas[0].AxisY.Minimum;
                //chart.ChartAreas["ChartAreas1"].AxisY.StripLines.Add(sl);
            }
            //if (YMax < -1 || YMax > 1)
            //{
            //    chart.ChartAreas["ChartAreas1"].AxisY.Maximum = YMax + 5;
            //}
            chart.Height = chartHeight;

            if (chart.ChartAreas.Count > 0)
            {
                chart.ChartAreas[0].Position.Y = ((float)70 / chartHeight) * 100;
                chart.ChartAreas[0].Position.Height = (((float)(chartHeight - 70) / chartSeriesCount) / chartHeight) * 100;
                chart.ChartAreas[0].Position.X = 0;
                chart.ChartAreas[0].Position.Width = 100;

                for (int i = 1; i < chart.ChartAreas.Count; i++)
                {
                    chart.ChartAreas[i].Position.Y = chart.ChartAreas[i - 1].Position.Bottom;
                    chart.ChartAreas[i].Position.Height = (((float)(chartHeight - 70) / chartSeriesCount) / chartHeight) * 100;
                    chart.ChartAreas[i].Position.X = 0;
                    chart.ChartAreas[i].Position.Width = 100;
                }
            }

            var rYS = from row in chart.Series where !row.Name.Contains("限") select row;

            foreach (var item in rYS)
            {
                double YValueMax = Math.Ceiling(chart.Series[item.Name].Points.FindMaxByValue().YValues[0]);
                double YValueMin = Math.Floor(chart.Series[item.Name].Points.FindMinByValue().YValues[0]);

                if (YValueMax - YValueMin < 5) { }
                chart.ChartAreas[chart.Series[item.Name].ChartArea].AxisY.Maximum = YValueMax + 5;
                chart.ChartAreas[chart.Series[item.Name].ChartArea].AxisY.Minimum = YValueMin - 5;
            }

            //MemoryStream ms = new MemoryStream();
            //chart.SaveImage(ms, ChartImageFormat.Png);
            //ms.Seek(0, SeekOrigin.Begin);
            return chart;
        }

        public static byte[] GetChartValueFile(string vName, string toolId, string TagName, string StartDate, string EndDate, string action)
        {
            ExcelPackage ep = new ExcelPackage();
            ep.Workbook.Worksheets.Add("Sheet1");
            ExcelWorksheet sheet1 = ep.Workbook.Worksheets["Sheet1"];
            DataTable dt = PLCCheckModel.GetChartValuedt(vName, toolId, TagName, StartDate, EndDate, action);

            if (dt.Rows.Count > 0)
            {
                dt.Columns.Remove("pid");
                dt.Columns.Remove("Unit");

                sheet1.Cells["A1"].LoadFromDataTable(dt, true);
                sheet1.Cells["A1"].Value = "ToolID";
                sheet1.Cells["B1"].Value = "數值";
                sheet1.Cells["C1"].Value = "時間";
                sheet1.Cells["D1"].Value = "數值名稱";

                for (int i = 1; i <= dt.Rows.Count; i++)
                {
                    sheet1.Cells[i, 4].Style.Numberformat.Format = "yyyy-mm-dd HH:mm:ss";
                }
                sheet1.Cells.AutoFitColumns();
            }

            return ep.GetAsByteArray();
        }

        public static byte[] GetChartValueFile2(string vName, string toolId, string TagName, string StartDate, string EndDate, string action)
        {
            ExcelPackage ep = new ExcelPackage();
            ep.Workbook.Worksheets.Add("Sheet1");
            ExcelWorksheet sheet1 = ep.Workbook.Worksheets["Sheet1"];
            DataTable dt = PLCCheckModel.GetChartValuedt(vName, toolId, TagName, StartDate, EndDate, action);

            if (dt.Rows.Count > 0)
            {
                dt.Columns.Remove("pid");
                dt.Columns.Remove("Unit");

                sheet1.Cells["A1"].LoadFromDataTable(dt, true);
                sheet1.Cells["A1"].Value = "ToolID";
                sheet1.Cells["B1"].Value = "數值";
                sheet1.Cells["C1"].Value = "時間";
                sheet1.Cells["D1"].Value = "數值名稱";

                for (int i = 1; i <= dt.Rows.Count; i++)
                {
                    sheet1.Cells[i, 4].Style.Numberformat.Format = "yyyy-mm-dd HH:mm:ss";
                }
                sheet1.Cells.AutoFitColumns();
            }

            return ep.GetAsByteArray();
        }

        public static byte[] GetChartValueFile3(string vName, string toolId, string TagName, string StartDate, string EndDate, string action)
        {
            ExcelPackage ep = new ExcelPackage();
            ep.Workbook.Worksheets.Add("Sheet1");
            ExcelWorksheet sheet1 = ep.Workbook.Worksheets["Sheet1"];

            DataTable dt = PLCCheckModel.GetChartValuedt(vName, toolId, TagName, StartDate, EndDate, action);

            if (dt.Rows.Count > 0)
            {
                dt.Columns.Remove("pid");
                dt.Columns.Remove("Unit");

                sheet1.Cells["A1"].LoadFromDataTable(dt, true);
                sheet1.Cells["A1"].Value = "ToolID";
                sheet1.Cells["B1"].Value = "數值";
                sheet1.Cells["C1"].Value = "時間";
                sheet1.Cells["D1"].Value = "數值名稱";

                for (int i = 1; i <= dt.Rows.Count; i++)
                {
                    sheet1.Cells[i, 4].Style.Numberformat.Format = "yyyy-mm-dd HH:mm:ss";
                }
                sheet1.Cells.AutoFitColumns();
            }

            return ep.GetAsByteArray();
        }

        public static Chart GetChart3(string vName, string toolId, string TagName, string StartDate, string EndDate, string action, string Y1Y2)
        {
            DateTime _StartDate = DateTime.Parse(StartDate);
            DateTime _EndDate = DateTime.Parse(EndDate);

            DataTable dt = null;

            if (_StartDate.AddHours(2) >= _EndDate)
            {
                dt = PLCCheckModel.GetChartValuedtDemo(vName, toolId, TagName, StartDate, EndDate, action);
            }
            else
            {
                dt = PLCCheckModel.GetChartValuedt(vName, toolId, TagName, StartDate, EndDate, action);
            }

            //建立一個新的Chart
            Chart chart = new Chart();

            //建立一個ChartAreas，並對ChartAreas設定格式
            chart.ChartAreas.Add("ChartAreas1");
            chart.ChartAreas["ChartAreas1"].BorderColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].BorderDashStyle = ChartDashStyle.Solid;
            chart.ChartAreas["ChartAreas1"].BackSecondaryColor = System.Drawing.Color.White;
            chart.ChartAreas["ChartAreas1"].BackColor = System.Drawing.Color.FromArgb(64, 165, 191, 228);
            chart.ChartAreas["ChartAreas1"].ShadowColor = System.Drawing.Color.Transparent;
            chart.ChartAreas["ChartAreas1"].BackGradientStyle = GradientStyle.TopBottom;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.Rotation = 10;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.Perspective = 10;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.Inclination = 15;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.IsRightAngleAxes = true;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.WallWidth = 0;
            chart.ChartAreas["ChartAreas1"].Area3DStyle.IsClustered = true;
            chart.ChartAreas["ChartAreas1"].AxisY.IsStartedFromZero = false;
            chart.ChartAreas["ChartAreas1"].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chart.ChartAreas["ChartAreas1"].AxisY.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25f, System.Drawing.FontStyle.Bold);
            chart.ChartAreas["ChartAreas1"].AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisX.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25f, System.Drawing.FontStyle.Bold);
            chart.ChartAreas["ChartAreas1"].AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisX.LabelStyle.Format = "yyyy-MM-dd \n HH:mm:ss";
            chart.ChartAreas["ChartAreas1"].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chart.ChartAreas["ChartAreas1"].AxisX.IntervalType = DateTimeIntervalType.Seconds;

            //chart.ChartAreas["ChartAreas1"].AxisX.IsLabelAutoFit = false;
            chart.ChartAreas["ChartAreas1"].AxisY.IsLabelAutoFit = true;
            //chart.ChartAreas["ChartAreas1"].AxisX.IntervalType = DateTimeIntervalType.Hours;
            chart.ChartAreas["ChartAreas1"].AxisX.LabelStyle.TruncatedLabels = true;
            chart.ChartAreas["ChartAreas1"].AxisX.IsInterlaced = true;

            chart.ChartAreas["ChartAreas1"].AxisY2.Enabled = AxisEnabled.Auto;
            chart.ChartAreas["ChartAreas1"].AxisY2.IsStartedFromZero = false;
            chart.ChartAreas["ChartAreas1"].AxisY2.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chart.ChartAreas["ChartAreas1"].AxisY2.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chart.ChartAreas["ChartAreas1"].AxisY2.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25f, System.Drawing.FontStyle.Bold);
            chart.ChartAreas["ChartAreas1"].AxisY2.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);

            chart.Legends.Add("Default");
            chart.Legends["Default"].Alignment = System.Drawing.StringAlignment.Near;
            chart.Legends["Default"].Docking = Docking.Top;

            chart.Width = 800;// (Int32)(dt.Rows.Count / TagName.Split(',').Length + 300);
            chart.Height = 600;

            string[] Y1Y2arr = Y1Y2.Split(',');

            double YMax = 0;

            chart.Titles.Add("數值統計表");
            if (dt.Rows.Count > 0)
            {
                System.Drawing.Color[] colors = { System.Drawing.Color.DarkBlue, System.Drawing.Color.DarkOrange, System.Drawing.Color.DarkRed, System.Drawing.Color.DarkSeaGreen, System.Drawing.Color.DarkViolet, System.Drawing.Color.DarkOliveGreen, System.Drawing.Color.DarkMagenta, System.Drawing.Color.DarkGreen, System.Drawing.Color.DarkCyan, System.Drawing.Color.DarkOrchid };

                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    if (chart.Series.IndexOf(dt.Rows[i][1].ToString() + dt.Rows[i][5].ToString()) < 0)
                    {
                        chart.Series.Add(dt.Rows[i][1].ToString() + dt.Rows[i][5].ToString());
                        chart.Series[dt.Rows[i][1].ToString() + dt.Rows[i][5].ToString()].ChartType = SeriesChartType.FastLine;
                        chart.Series[dt.Rows[i][1].ToString() + dt.Rows[i][5].ToString()].IsVisibleInLegend = true;
                        chart.Series[dt.Rows[i][1].ToString() + dt.Rows[i][5].ToString()].Legend = "Default";
                        chart.Series[dt.Rows[i][1].ToString() + dt.Rows[i][5].ToString()].BorderWidth = 2;
                        if (colors.Count() > chart.Series.Count)
                        {
                            chart.Series[dt.Rows[i][1].ToString() + dt.Rows[i][5].ToString()].Color = colors[chart.Series.Count - 1];
                        }
                        if (Y1Y2arr[chart.Series.Count - 1] == "Y2")
                        {
                            chart.Series[dt.Rows[i][1].ToString() + dt.Rows[i][5].ToString()].YAxisType = AxisType.Secondary;
                        }
                    }

                    chart.Series[dt.Rows[i][1].ToString() + dt.Rows[i][5].ToString()].Points.AddXY(Convert.ToDateTime(dt.Rows[i][3]), dt.Rows[i][2]);
                    chart.Series[dt.Rows[i][1].ToString() + dt.Rows[i][5].ToString()].ToolTip = dt.Rows[i][2].ToString();
                    chart.Series[dt.Rows[i][1].ToString() + dt.Rows[i][5].ToString()].IsValueShownAsLabel = true;
                    if (dt.Rows[i][2] != null && dt.Rows[i][2].ToString().ToUpper() != "NULL" && dt.Rows[i][2].ToString().Length > 0)
                    {
                        if (YMax < Convert.ToDouble(dt.Rows[i][2]))
                        {
                            YMax = Convert.ToDouble(dt.Rows[i][2]) + Convert.ToDouble(dt.Rows[i][2]) * 0.5;
                        }
                    }
                }

                string[] TagNameArr = TagName.Split(',');

            }
            if (YMax < -1 || YMax > 1)
            {
                //chart.ChartAreas["ChartAreas1"].AxisY.Maximum = YMax + 5;
            }

            return chart;
        }

        public static string getChartEnCode(Chart chart)
        {
            using (var stream = new MemoryStream())
            {
                string img = "data:image/png;base64,{0}";
                chart.SaveImage(stream, ChartImageFormat.Png);
                string encoded = Convert.ToBase64String(stream.ToArray());
                return String.Format(img, encoded);
            }
        }

        public static string GetChartImageHtml(Chart chart)
        {
            using (var stream = new MemoryStream())
            {
                string ImageMap = "";

                chart.SaveImage(stream, ChartImageFormat.Png);

                string img = "<img src='data:image/png;base64,{0}' alt='' usemap='#ChartImageMap'>{1}";

                string encoded = Convert.ToBase64String(stream.ToArray());
                ImageMap = chart.GetHtmlImageMap("ChartImageMap");

                return String.Format(img, encoded, ImageMap);
            }
        }

        public static Chart CreateDummyChart()
        {
            var chart = new Chart() { Width = 600, Height = 400 };
            chart.Palette = ChartColorPalette.Excel;
            chart.Legends.Add(new Legend("legend1") { Docking = Docking.Bottom });

            var title = new Title("Test chart", Docking.Top, new System.Drawing.Font("Arial", 15, System.Drawing.FontStyle.Bold), System.Drawing.Color.Brown);
            chart.Titles.Add(title);
            chart.ChartAreas.Add("Area 1");

            chart.Series.Add("Series 1");
            chart.Series.Add("Series 2");

            chart.BackColor = System.Drawing.Color.Azure;
            var random = new Random();

            //add random data: series 1
            foreach (int value in new List<int>() { random.Next(100), random.Next(100), random.Next(100), random.Next(100) })
            {
                chart.Series["Series 1"].Points.AddY(value);

                //attach JavaScript events - it can also be ajax call
                chart.Series["Series 1"].Points.Last().MapAreaAttributes = "onclick=\"alert('value: #VAL, series: #SER');\"";
            }

            //add random data: series 2
            foreach (int value in new List<int>() { random.Next(100), random.Next(100), random.Next(100), random.Next(100) })
            {
                chart.Series["Series 2"].Points.AddY(value);

                //attach JavaScript events - it can also be ajax call
                chart.Series["Series 2"].Points.Last().MapAreaAttributes = "onclick=\"alert('value: #VAL, series: #SER');\"";
            }

            return chart;
        }
    }
}