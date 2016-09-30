using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using TSMC14B.Models;

namespace TSMC14B.Areas.Main.Models
{
    public class ListModel
    {
        #region 組織成員屬性
        [Display(Name = "Text")]
        public string Text { get; set; }

        [Display(Name = "Value")]
        public string Value { get; set; }


        #endregion

        public ListModel() { }

        public ListModel(string _Text, string _Value)
        {
            Text = _Text;
            Value = _Value;
        }

        public static IEnumerable<ListModel> GetTypeInfoList()
        {
            //using (tsmc14BDataContext db = new tsmc14BDataContext())
            //{
            //    return (from row in db.vw_eq_status select new ListModel{Text=row.vName,Value=row.vid.ToString()}).Distinct();
            //}
            DataSet ds = DBConnector.executeQuery("Intouch", "SELECT * FROM type_info order by type_id");

            IEnumerable<ListModel> tempList = null;

            if (ds.Tables.Count > 0)
            {
                tempList = from dept in ds.Tables[0].AsEnumerable()
                           //orderby dept.Field<int>("display_order")
                           select new ListModel
                           {
                               Text = dept["type_name"].ToString(),
                               Value = dept["type_id"].ToString(),
                           };
            }

            return tempList;
        }

        public static IEnumerable<ListModel> GetDepartmentList()
        {
            //using (tsmc14BDataContext db = new tsmc14BDataContext())
            //{
            //    return (from row in db.vw_eq_status select new ListModel{Text=row.vName,Value=row.vid.ToString()}).Distinct();
            //}
            DataSet ds = DBConnector.executeQuery("Intouch", "SELECT DISTINCT department_id,department_name,display_order FROM department_info vw order by display_order");

            IEnumerable<ListModel> tempList = null;

            if (ds.Tables.Count > 0)
            {
                tempList = from dept in ds.Tables[0].AsEnumerable()
                           //orderby dept.Field<int>("display_order")
                           select new ListModel
                           {
                               Text = dept["department_name"].ToString(),
                               Value = dept["department_id"].ToString(),
                           };
            }

            return tempList;
        }

        public static IEnumerable<ListModel> GetGroupList(string vName)
        {
            //using (tsmc14BDataContext db = new tsmc14BDataContext())
            //{
            //    return (from row in db.vw_eq_status where row.vid.ToString() == vid select new ListModel { Text = row.gname, Value = row.gname }).Distinct();
            //}
            DataSet ds = null;
            if (vName == "DPM")
            {
                ds = DBConnector.executeQuery("Intouch", "SELECT DISTINCT groupName FROM vw_dpm_status WHERE groupName IS NOT NULL");
            }
            else
            {
                if (!string.IsNullOrEmpty(vName))
                    ds = DBConnector.executeQuery("Intouch", "SELECT DISTINCT groupName FROM vw_eq_status WHERE vName = '" + vName + "' AND groupName IS NOT NULL");
                else
                    ds = DBConnector.executeQuery("Intouch", "SELECT DISTINCT groupName FROM vw_eq_status WHERE groupName IS NOT NULL");
            }

            IEnumerable<ListModel> tempList = null;

            if (ds.Tables.Count > 0)
            {
                tempList = from dept in ds.Tables[0].AsEnumerable()
                           orderby dept.Field<string>("groupName")
                           select new ListModel
                           {
                               Text = dept.Field<string>("groupName"),
                               Value = dept.Field<string>("groupName"),
                           };
            }

            return tempList;
        }

        public static IEnumerable<ListModel> GetPMVendorList(string vName)
        {
            DataSet ds = DBConnector.executeQuery("Intouch", "SELECT DISTINCT vid,vName,display_order FROM vw_eq_status vw join  vendor_info vi on vi.vendor_id=vw.vid WHERE vName like '" + vName + "%' order by display_order");

            IEnumerable<ListModel> tempList = null;

            if (ds.Tables.Count > 0)
            {
                tempList = from dept in ds.Tables[0].AsEnumerable()
                           //orderby dept.Field<int>("display_order")
                           select new ListModel
                           {
                               Text = dept.Field<string>("vName"),
                               Value = dept.Field<Int16>("vid").ToString(),
                           };
            }

            return tempList;
        }

        public static IEnumerable<ListModel> GetPCstatusList()
        {
            DataSet ds = null;
            ds = DBConnector.executeQuery("Intouch", "SELECT si.server_name, CONVERT(varchar,ps.status_now) status_now  FROM phonecall_status ps JOIN Server_Info si ON PS.server_id = si.server_id ");

            IEnumerable<ListModel> tempList = null;
            if (ds.Tables.Count > 0)
            {
                tempList = from dept in ds.Tables[0].AsEnumerable()
                           select new ListModel
                           {
                               Text = dept.Field<string>("server_name"),
                               Value = Convert.ToString(dept.Field<string>("status_now")),
                           };
            }

            return tempList;
        }

        public static IEnumerable<ListModel> GetToolIDList(string department_name)
        {
            //using (tsmc14BDataContext db = new tsmc14BDataContext())
            //{
            //    return (from row in db.vw_eq_status where row.vid.ToString()==vid select new ListModel { Text = row.toolid, Value = row.pid.ToString() }).Distinct();
            //}
            string sqlStr = null;
            if (string.IsNullOrEmpty(department_name))
                sqlStr = "SELECT DISTINCT chamberName FROM vw_eq_status";
            else
                sqlStr = "SELECT DISTINCT chamberName FROM vw_eq_status WHERE department_name = '" + department_name + "'";

            DataSet ds = DBConnector.executeQuery("Intouch", sqlStr);

            IEnumerable<ListModel> tempList = null;

            if (ds.Tables.Count > 0)
            {
                tempList = (from dept in ds.Tables[0].AsEnumerable()
                            orderby dept.Field<string>("chamberName")
                            select new ListModel
                            {
                                Text = dept.Field<string>("chamberName"),
                                Value = dept.Field<string>("chamberName"),
                            }).ToList();
            }

            return tempList;
        }

        public static IEnumerable<ListModel> GetToolIDList(string vid, string groupName)
        {
            //using (tsmc14BDataContext db = new tsmc14BDataContext())
            //{
            //    return (from row in db.vw_eq_status where row.vid.ToString() == vid && row.gname == groupName select new ListModel { Text = row.toolid, Value = row.pid.ToString() }).Distinct();
            //}
            string sqlStr = null;
            if (!string.IsNullOrEmpty(groupName))
                sqlStr += " AND groupName = '" + groupName + "'";
            else
                sqlStr += " AND 1=1 ";

            if (!string.IsNullOrEmpty(vid))
                sqlStr += " AND department_name = '" + vid + "'";

            DataSet ds = null;
            if (vid == "DPM")
            {
                ds = DBConnector.executeQuery("Intouch", "SELECT DISTINCT pid,toolid FROM vw_dpm_status WHERE 1 = 1 " + sqlStr);
            }
            else
            {
                ds = DBConnector.executeQuery("Intouch", "SELECT DISTINCT pid,chamberName FROM vw_eq_status WHERE 1 = 1 " + sqlStr);
            }

            IEnumerable<ListModel> tempList = null;

            if (ds.Tables.Count > 0)
            {
                tempList = from dept in ds.Tables[0].AsEnumerable()
                           orderby dept.Field<string>("chamberName")
                           select new ListModel
                           {
                               Text = dept.Field<string>("chamberName"),
                               Value = dept.Field<string>("chamberName"),
                           };
            }
            return tempList;
        }

        public static IEnumerable<ListModel> GetTagNameList(string vName, string toolID)
        {
            string _pid = null;
            int _pid2 = 0;
            DataSet ds = null;
            if (vName == "DPM")
            {
                using (tsmc14BDataContext db = new tsmc14BDataContext())
                {
                    //_pid2 = (from row in db.vw_dpm_status where row.toolid == toolID select row.pid).FirstOrDefault();
                }
                ds = DBConnector.executeQuery("Intouch", "uSP_Select_ValueNames_DPM @pid=" + _pid2);
            }
            else
            {
                _pid = ListModel.GetPid(toolID).ToString();
                ds = DBConnector.executeQuery("Intouch", "uSP_Select_ValueNames @pid=" + _pid);
            }
            //BackPressure
            //BPCurrent
            //BPPower
            //MPCurrent
            //MPPower
            //MPTemp
            //N2Flow
            //WaterFlow
            //ds.Tables[0].Rows.Clear();

            //DataRow tempRow = null;
            //tempRow = ds.Tables[0].NewRow();
            //tempRow["Cht_Comment"] = "BackPressure";
            //tempRow["FullTagName"] =toolID+ "BackPressure";
            //ds.Tables[0].Rows.Add(tempRow);

            //tempRow = ds.Tables[0].NewRow();
            //tempRow["Cht_Comment"] = "BPCurrent";
            //tempRow["FullTagName"] = toolID + "BPCurrent";
            //ds.Tables[0].Rows.Add(tempRow);

            //tempRow = ds.Tables[0].NewRow();
            //tempRow["Cht_Comment"] = "BPPower";
            //tempRow["FullTagName"] = toolID + "BPPower";
            //ds.Tables[0].Rows.Add(tempRow);

            //tempRow = ds.Tables[0].NewRow();
            //tempRow["Cht_Comment"] = "MPCurrent";
            //tempRow["FullTagName"] = toolID + "MPCurrent";
            //ds.Tables[0].Rows.Add(tempRow);

            //tempRow = ds.Tables[0].NewRow();
            //tempRow["Cht_Comment"] = "MPPower";
            //tempRow["FullTagName"] = toolID + "MPPower";
            //ds.Tables[0].Rows.Add(tempRow);

            //tempRow = ds.Tables[0].NewRow();
            //tempRow["Cht_Comment"] = "MPTemp";
            //tempRow["FullTagName"] = toolID + "MPTemp";
            //ds.Tables[0].Rows.Add(tempRow);

            //tempRow = ds.Tables[0].NewRow();
            //tempRow["Cht_Comment"] = "N2Flow";
            //tempRow["FullTagName"] = toolID + "N2Flow";
            //ds.Tables[0].Rows.Add(tempRow);

            //tempRow = ds.Tables[0].NewRow();
            //tempRow["Cht_Comment"] = "WaterFlow";
            //tempRow["FullTagName"] = toolID + "WaterFlow";
            //ds.Tables[0].Rows.Add(tempRow);

            IEnumerable<ListModel> tempList = null;

            if (ds.Tables.Count > 0)
            {
                tempList = from dept in ds.Tables[0].AsEnumerable()
                           orderby dept.Field<string>("Cht_Comment")
                           select new ListModel
                           {
                               Text = dept.Field<string>("Cht_Comment"),
                               Value = dept.Field<string>("FullTagName"),
                           };
            }

            return tempList;
        }

        public static short GetVid(string vName)
        {
            //if (vName == "DPM")
            //{
            //    using (tsmc14BDataContext db = new tsmc14BDataContext())
            //    {
            //        return (from row in db.vw_dpm_status select row.vid).FirstOrDefault().Value;
            //    }
            //}
            //else
            //{
            //    using (tsmc14BDataContext db = new tsmc14BDataContext())
            //    {
            //        return (from row in db.vw_eq_status where row.vName == vName select row.vid).FirstOrDefault().Value;
            //    }
            //}
            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                return (from row in db.vendor_info where row.vendor_name == vName select row.vendor_id).FirstOrDefault();
            }
        }

        public static short GetDepartmentId(string dName)
        {
            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                return (from row in db.department_info where row.department_name == dName select row.department_id).FirstOrDefault();
            }
        }

        public static short Gettid(string tName)
        {
            //using (tsmc14BDataContext db = new tsmc14BDataContext())
            //{
            //    return (from row in db.vw_eq_status where row.tName == tName select row.typeid).FirstOrDefault();
            //}
            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                return (from row in db.type_info where row.type_name == tName select row.type_id).FirstOrDefault();
            }
        }

        public static string GettName(string tid)
        {
            //using (tsmc14BDataContext db = new tsmc14BDataContext())
            //{
            //    return (from row in db.vw_eq_status where row.typeid == Convert.ToInt16(tid) select row.tName).FirstOrDefault();
            //}
            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                return (from row in db.type_info where row.type_id == Convert.ToInt16(tid) select row.type_name).FirstOrDefault();
            }
        }

        public static int GetPid(string Toolid)
        {
            //using (tsmc14BDataContext db = new tsmc14BDataContext())
            //{
            //    return (from row in db.vw_eq_status where row.toolid == Toolid select row.pid).FirstOrDefault();
            //}
            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                return (from row in db.vw_eq_status where row.chamberName == Toolid select row.pid).FirstOrDefault();
            }
        }

        public static Int32 GetDPMPid(string Toolid)
        {
            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                return (from row in db.vw_dpm_status where row.toolid == Toolid select row.pid).FirstOrDefault();
            }
        }

        public static List<string> GetAlarmId(string ToolID, string AlarmTime)
        {
            DateTime _AlarmTime = Convert.ToDateTime(AlarmTime);
            //2014-01-28 11:34:57
            DataSet ds = DBConnector.executeQuery("Intouch", "exec [uSP_Select_AlarmHistory] @fromDate = N'" + AlarmTime + "',@toDate = N'" + _AlarmTime.AddSeconds(1).ToString("yyyy-MM-dd HH:mm:ss") + "',@toolID = N'" + ToolID + "',@onceadayFlag = 0");

            List<string> tmp = new List<string>();

            if (ds.Tables.Count > 0)
            {
                DataRow dr = ds.Tables[0].AsEnumerable().FirstOrDefault();

                tmp.Add(dr["alarmID"].ToString());
                tmp.Add(dr["vendor_name"].ToString());
                tmp.Add(dr["vendor_name"].ToString());

            }

            return tmp;

        }

        public static IEnumerable<ListModel> GetTypeByVname(string vName, string gName)
        {
            if (string.IsNullOrEmpty(gName))
                gName = "NULL";
            DataSet ds = DBConnector.executeQuery("Intouch", "exec [uSP_Select_EQ_Type] " + ListModel.GetVid(vName) + "," + gName);

            IEnumerable<ListModel> tempList = null;

            if (ds.Tables.Count > 0)
            {
                tempList = (from dept in ds.Tables[0].AsEnumerable()
                            orderby dept.Field<string>("tName")
                            select new ListModel
                            {
                                Text = dept.Field<string>("tName"),
                                Value = dept.Field<string>("tName"),
                            }).ToList();
            }

            return tempList;

        }

        public static IEnumerable<ListModel> GetTypeByVname2Id(string vName, string gName)
        {
            if (string.IsNullOrEmpty(gName))
                gName = "NULL";
            DataSet ds = DBConnector.executeQuery("Intouch", "exec [uSP_Select_EQ_Type] " + ListModel.GetVid(vName) + "," + gName);

            IEnumerable<ListModel> tempList = null;

            if (ds.Tables.Count > 0)
            {
                tempList = (from dept in ds.Tables[0].AsEnumerable()
                            orderby dept.Field<string>("tName")
                            select new ListModel
                            {
                                Text = dept.Field<string>("tName"),
                                Value = dept.Field<Int16>("typeid").ToString(),
                            }).ToList();
            }

            return tempList;
        }

        public static IEnumerable<ListModel> GetTagByType(string vName, string tName)
        {
            DataSet ds = DBConnector.executeQuery("Intouch", "exec [uSP_Select_EQ_Type] " + ListModel.GetVid(vName) + ", NULL ," + ListModel.Gettid(tName));

            IEnumerable<ListModel> tempList = null;

            if (ds.Tables.Count > 0)
            {
                tempList = (from dept in ds.Tables[1].AsEnumerable()
                            orderby dept.Field<string>("Cht_Comment")
                            select new ListModel
                            {
                                Text = dept.Field<string>("Cht_Comment"),
                                Value = dept.Field<string>("TagName"),
                            }).ToList();
            }

            return tempList;
        }
        public static IEnumerable<ListModel> GetToolidByType(string vName, string tName)
        {
            DataSet ds = DBConnector.executeQuery("Intouch", "exec [uSP_Select_EQ_Type] " + ListModel.GetVid(vName) + ", NULL ," + ListModel.Gettid(tName));

            IEnumerable<ListModel> tempList = null;

            if (ds.Tables.Count > 0)
            {
                tempList = (from dept in ds.Tables[0].AsEnumerable()
                            orderby dept.Field<string>("toolid")
                            select new ListModel
                            {
                                Text = dept.Field<string>("toolid"),
                                Value = dept.Field<string>("toolid"),
                            }).ToList();
            }

            return tempList;
        }
        public static List<ListModel> GetTagNameChtComment(string TagName)
        {
            List<ListModel> tempitems = new List<ListModel>();

            foreach (var item in TagName.Split(','))
            {
                DataSet ds = DBConnector.executeQuery("Intouch", "SELECT tool_id +' '+ Cht_Comment as 'tool_idCht_Comment' , 'P5_'+tool_id+Tag_Name as 'tool_idTag_Name' FROM plc_info P JOIN analog_comment A on P.type_id=A.type_id  where tool_id ='' and Tag_Name =''");

                if (ds.Tables[0].Rows.Count > 0)
                {
                    tempitems.Add(new ListModel(ds.Tables[0].Rows[0]["tool_idCht_Comment"].ToString(), ds.Tables[0].Rows[0]["tool_idTag_Name"].ToString()));
                }
            }

            return tempitems.ToList();
        }

        public static IEnumerable<ListModel> GetAlarmTagList(string Comment)
        {
            DataSet ds = null;
            ds = DBConnector.executeQuery("Intouch", "SELECT type_id,Tag_Name FROM AlarmTagName WHERE Comment = '" + Comment + "'");

            IEnumerable<ListModel> tempList = null;

            if (ds.Tables.Count > 0)
            {
                tempList = from dept in ds.Tables[0].AsEnumerable()
                           select new ListModel
                           {
                               Text = dept.Field<string>("Tag_Name"),
                               Value = dept.Field<string>("Tag_Name"),
                           };
            }
            return tempList;
        }

        public static IEnumerable<ListModel> GetToolIDPIDList(string vName)
        {
            //using (tsmc14BDataContext db = new tsmc14BDataContext())
            //{
            //    return (from row in db.vw_eq_status where row.vid.ToString()==vid select new ListModel { Text = row.toolid, Value = row.pid.ToString() }).Distinct();
            //}
            string sqlStr = null;
            if (string.IsNullOrEmpty(vName))
                sqlStr = "SELECT DISTINCT pid,toolid FROM vw_eq_status";
            else
                sqlStr = "SELECT DISTINCT pid,toolid FROM vw_eq_status WHERE vName = '" + vName + "'";

            DataSet ds = DBConnector.executeQuery("Intouch", sqlStr);
            IEnumerable<ListModel> tempList = null;

            if (ds.Tables.Count > 0)
            {
                tempList = (from dept in ds.Tables[0].AsEnumerable()
                            orderby dept.Field<string>("toolid")
                            select new ListModel
                            {
                                Text = dept.Field<string>("toolid"),
                                Value = dept.Field<int>("pid").ToString(),
                            }).ToList();
            }

            return tempList;
        }

        public static IEnumerable<ListModel> GetPMfrequencyList()
        {
            DataSet ds = DBConnector.executeQuery("Intouch", "SELECT DISTINCT PM_frequency,freq_id FROM PM_frequency_info");

            IEnumerable<ListModel> tempList = null;

            if (ds.Tables.Count > 0)
            {
                tempList = from dept in ds.Tables[0].AsEnumerable()
                           orderby dept.Field<Int16>("PM_frequency")
                           select new ListModel
                           {
                               Text = dept.Field<Int16>("PM_frequency").ToString(),
                               Value = dept.Field<Byte>("freq_id").ToString(),
                           };
            }
            return tempList;
        }
    }
}