using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using WebCMS.Models;

namespace WebCMS.Areas.Main.Models
{
    public class MachineStatusModel
    {
        #region 組織成員屬性
        [Display(Name = "課別")]
        public string GroupName { get; set; }

        [Display(Name = "總共設備數")]
        public int MachineCount { get; set; }

        [Display(Name = "正常設備數")]
        public int NormlCount { get; set; }

        [Display(Name = "警告/異常數")]
        public int AlarmCount { get; set; }

        [Display(Name = "PM 隔離")]
        public int PMCount { get; set; }

        [Display(Name = "其他狀態數")]
        public int OtherCount { get; set; }

        [Display(Name = "Service數")]
        public int ServiceCount { get; set; }

        #endregion

        public static List<MachineStatusModel> GetMachineStatus(string vName)
        {
            List<MachineStatusModel> MachineStatusList = new List<MachineStatusModel>();

            using (tsmc14BDataContext db = new tsmc14BDataContext())
            {
                var vw_eq_status = (from row in db.vw_eq_status select row).ToList();
                var machineGroup = (from row in vw_eq_status where row.vName == vName select row.groupName).Distinct();

                foreach (string gname in machineGroup)
                {
                    MachineStatusModel MachineStatusModel1 = new MachineStatusModel();

                    MachineStatusModel1.GroupName = gname;
                    MachineStatusModel1.MachineCount = (from row in vw_eq_status where row.groupName == gname && row.vName == vName select row).Count();
                    MachineStatusModel1.NormlCount = (from row in vw_eq_status where row.groupName == gname && row.vName == vName && row.sid == 0 select row).Count();
                    MachineStatusModel1.AlarmCount = (from row in vw_eq_status where row.groupName == gname && row.vName == vName && (row.sid == 1 || row.sid == 2) select row).Count();
                    MachineStatusModel1.PMCount = (from row in vw_eq_status where row.groupName == gname && row.vName == vName && row.sid == 8 select row).Count();
                    MachineStatusModel1.OtherCount = (from row in vw_eq_status where row.groupName == gname && row.vName == vName && (row.sid != 0 && row.sid != 1 && row.sid != 2 && row.sid != 8 && row.sid != 16) select row).Count();
                    MachineStatusModel1.ServiceCount = (from row in vw_eq_status where row.groupName == gname && row.vName == vName && row.sid == 16 select row).Count();
                    MachineStatusList.Add(MachineStatusModel1);
                }
            }

            return MachineStatusList;
        }

        public static List<MachineStatusModel> GetMachineStatusdt(string vName)
        {
            short vid = ListModel.GetVid(vName);

            DataSet ds = DBConnector.executeQuery("Intouch", "exec uSP_Select_EQ_StatusCount  @vid = " + vid + ",@sid = null,@gFlag = NULL ");

            List<MachineStatusModel> tempList = null;

            if (ds.Tables.Count > 0)
            {
                tempList = (from dept in ds.Tables[0].AsEnumerable()
                            select new MachineStatusModel
                            {
                                GroupName = dept.IsNull("gname") ? string.Empty : dept.Field<string>("gname"),
                                MachineCount = dept.Field<int>("eqCount"),
                                NormlCount = dept.Field<int>("normalCount"),
                                AlarmCount = dept.Field<int>("troubleCount"),
                                PMCount = dept.Field<int>("pmCount"),
                                OtherCount = dept.Field<int>("otherCount")
                            }).ToList();
            }

            return tempList;
        }
    }
}