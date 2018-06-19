using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Configuration;
using WebCMS.Models;

namespace WebCMS.Areas.Main.Models
{
    public class SensorTagModel
    {
        #region 組織成員屬性

        [Display(Name = "plc_id")]
        public int plc_id { get; set; }

        [Display(Name = "Sensor")]
        public string Sensor { get; set; }

        [Display(Name = "Tool")]
        public string Tool { get; set; }

        [Display(Name = "ToolType")]
        public string ToolType { get; set; }

        [Display(Name = "chamber")]
        public string chamber { get; set; }

        [Display(Name = "ReportInterval(minute)")]
        public double PeriodicReportInterval { get; set; }

        [Display(Name = "chamberTag")]
        public string chamberTag { get; set; }

        [Display(Name = "sensorTag")]
        public string sensorTag { get; set; }

        [Display(Name = "LinkProcessData")]
        public string LinkProcessData { get; set; }


        #endregion


        public static SensorTagModel GetSensorTag(String SensorId)
        { 
            SensorTagModel tmp = new SensorTagModel();

            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                var r = (from row in db.vw_SensorTag_info where row.Sensor == SensorId select row).FirstOrDefault();

                tmp.plc_id = r.plc_id;
                tmp.chamber = r.chamber;
                tmp.chamberTag = r.chamberTag;
                tmp.PeriodicReportInterval = r.PeriodicReportInterval;
                tmp.LinkProcessData = r.LinkProcessData;
                tmp.Sensor = r.Sensor;
                tmp.sensorTag = r.sensorTag;
                tmp.Tool = r.Tool;
                tmp.ToolType = r.ToolType;
            }

            return tmp;
        }

        public static string Update(SensorTagModel s)
        {
            try
            {
                using (tsmc14BDataContext db = new tsmc14BDataContext())
                {
                    var r = (from row in db.plc_info where row.plc_id == s.plc_id select row).SingleOrDefault();
                    
                    if (r!=null)
                    {
                        r.LinkProcessData = s.LinkProcessData;
                        r.SensorTag = s.sensorTag;
                        r.ChamberTag = s.chamberTag;

                        //if (!r.Any())
                        //{
                        //    db.SenSorTag_info.InsertOnSubmit(tmp);
                        //}
                        db.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return null;
        }
    }
}