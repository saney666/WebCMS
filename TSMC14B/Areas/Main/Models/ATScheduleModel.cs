using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using OfficeOpenXml;
using System.Data;
using WebCMS.Models;
using System.Web.Mvc;

namespace WebCMS.Areas.Main.Models
{
    public class ATScheduleModel
    {
        #region 組織成員屬性
        [Display(Name = "ATSID") ]
        public int ATSID { get; set; }

        [Display(Name = "ATName", ResourceType = typeof(Menu)), Required]
        public string ATName { get; set; }

        [Display(Name = "buileDate", ResourceType = typeof(Menu))]
        public string buileDate { get; set; }

        [Display(Name = "ExecuteType", ResourceType = typeof(Menu))]
        public string ExecuteType { get; set; }

        [Display(Name = "ExecuteCycle", ResourceType = typeof(Menu))]
        public Byte ExecuteCycle { get; set; }

        [Display(Name = "ExecuteTime", ResourceType = typeof(Menu))]
        public string ExecuteTime { get; set; }

        [Display(Name = "ExecuteHour"), Range(0, 23)]
        public byte ExecuteHour { get; set; }

        [Display(Name = "ExecuteMinute"), Range(0, 59)]
        public byte ExecuteMinute { get; set; }

        [Display(Name = "DataType", ResourceType = typeof(Menu))]
        public string DataType { get; set; }

        [Display(Name = "DataCycle", ResourceType = typeof(Menu)), Required, Range(1, 100)]
        public Byte DataCycle { get; set; }

        [Display(Name = "DataRangeTime", ResourceType = typeof(Menu))]
        public string DataRangeTime { get; set; }

        [Display(Name = "DataRangeTimeHour"), Range(0, 23)]
        public byte DataRangeHour { get; set; }

        [Display(Name = "DataRangeTimeMinute"), Range(0, 59)]
        public byte DataRangeMinute { get; set; }

        [Display(Name = "SamplingTime", ResourceType = typeof(Menu)), Range(1, 32766)]
        public int SamplingTime { get; set; }

        [Display(Name = "Login_name", ResourceType = typeof(Menu))]
        public string Login_name { get; set; }

        #endregion

        public static IEnumerable<ATScheduleModel> GetScheduleList(string Login_name)
        {
            List<ATScheduleModel> ATScheduleList = new List<ATScheduleModel>();
            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {

                ATScheduleList = (from row in db.ATSchedule_info
                                  where row.Login_name == Login_name
                                  select new ATScheduleModel
                                  {
                                      ATSID = row.ATSID,
                                      ATName = row.ATName,
                                      buileDate = DateTime.Parse (row.buileDate.ToString()).ToString("yyyy-MM-dd"),
                                      ExecuteType = row.ExecuteType,
                                      ExecuteCycle = row.ExecuteCycle,
                                      ExecuteTime = DateTime.FromBinary(row.ExecuteTime.Ticks).ToString("HH:mm"),
                                      DataType =row.DataType,
                                      DataCycle = row.DataCycle,
                                      DataRangeTime = DateTime.FromBinary(row.DataRangeTime.Ticks).ToString("HH:mm"),
                                      SamplingTime = row.SamplingTime,
                                      Login_name = row.Login_name
                                  }).ToList();
            }
            return ATScheduleList;
        }

        public static ATScheduleModel GetATSchedule(int ATSID)
        {
            ATScheduleModel ATSchedule = new ATScheduleModel();
            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                ATSchedule = (from row in db.ATSchedule_info
                                  where row.ATSID==ATSID
                                  select new ATScheduleModel
                                  {
                                      ATSID = row.ATSID,
                                      ATName = row.ATName,
                                      buileDate = DateTime.Parse(row.buileDate.ToString()).ToString("yyyy-MM-dd"),
                                      ExecuteType = row.ExecuteType,
                                      ExecuteCycle = row.ExecuteCycle,
                                      ExecuteTime = DateTime.FromBinary(row.ExecuteTime.Ticks).ToString("HH:mm"),
                                      ExecuteHour = (byte)row.ExecuteTime.Hours,
                                      ExecuteMinute = (byte)row.ExecuteTime.Minutes,
                                      DataType = row.DataType,
                                      DataCycle = row.DataCycle,
                                      DataRangeTime = DateTime.FromBinary(row.DataRangeTime.Ticks).ToString("HH:mm"),
                                      DataRangeHour = (byte)row.DataRangeTime.Hours,
                                      DataRangeMinute = (byte)row.DataRangeTime.Minutes,
                                      SamplingTime = row.SamplingTime,
                                      Login_name = row.Login_name
                                  }).SingleOrDefault();
            }

            return ATSchedule;
        }

        internal static byte[] GetScheduleFile(string Login_name)
        {
            throw new NotImplementedException();
        }

        internal string Add(string Login_name)
        {
            string tempString = "";
            try
            {
                using (tsmc14BDataContext db = new tsmc14BDataContext())
                {
                    ATSchedule_info ATS = new ATSchedule_info();

                    ATS.ATName = ATName;
                    ATS.buileDate = DateTime.Now;
                    ATS.DataCycle = DataCycle;
                    ATS.DataRangeTime =new TimeSpan(DataRangeHour,DataRangeMinute , 0);
                    ATS.DataType = DataType;
                    ATS.ExecuteCycle = ExecuteCycle;
                    ATS.ExecuteTime = new TimeSpan( ExecuteHour, ExecuteMinute,0);
                    ATS.ExecuteType = ExecuteType;
                    ATS.SamplingTime = SamplingTime;
                    ATS.Login_name = Login_name;

                    if (ExecuteType == "d")
                    {
                        ATS.ExecuteCycle = 0;
                    }

                    db.ATSchedule_info.InsertOnSubmit(ATS);
                    db.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                tempString = ex.Message;
            }
          
            return tempString;
        }

        internal string Update(string p)
        {
            string tempString = "";
            try
            {
                using (tsmc14BDataContext db = new tsmc14BDataContext())
                {
                    var r = db.ATSchedule_info.Where(x => x.ATSID == ATSID).SingleOrDefault();
                    if (r != null)
                    {
                        r.ATName = ATName;
                        r.DataCycle = DataCycle;
                        r.DataRangeTime = new TimeSpan( DataRangeHour, DataRangeMinute,0);
                        r.DataType = DataType;
                        r.ExecuteCycle = ExecuteCycle;
                        r.ExecuteTime = new TimeSpan( ExecuteHour, ExecuteMinute,0);
                        r.ExecuteType = ExecuteType;
                        r.SamplingTime = SamplingTime;


                        if (ExecuteType=="d")
                        {
                            r.ExecuteCycle = 0;
                        }


                        db.SubmitChanges();
                        tempString = WebCMS.Menu.Edit + "ok";
                    }
                }
            }
            catch (Exception ex)
            {
                tempString = ex.Message;
            }

            return tempString;
        }
        internal static string Delete(int ATSID)
        {
            string tempString = "";
            try
            {
                using (tsmc14BDataContext db = new tsmc14BDataContext())
                {
                    var r = db.ATSchedule_info.Where(x => x.ATSID == ATSID).SingleOrDefault();
                    if (r != null)
                    {
                        db.ATSchedule_info.DeleteOnSubmit(r);
                        db.SubmitChanges();
                        tempString = WebCMS.Menu.Delete + "ok";
                    }
                }
            }
            catch (Exception ex)
            {
                tempString = ex.Message;
            }
            
            return tempString;
        }
    }
}