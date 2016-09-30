using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using TSMC14B.Models;

namespace TSMC14B.Areas.Main.Models
{
    public class ChamberModel
    {
        public string ChamberName { get; set; }
        public string ChamberStatus { get; set; }
        public DataTable ChamberData { get; set; }

    }
}