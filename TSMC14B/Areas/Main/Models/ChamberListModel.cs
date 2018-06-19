using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using WebCMS.Models;

namespace WebCMS.Areas.Main.Models
{
    public class ChamberListModel
    {
        public string ChamberName { get; set; }
        public string ChamberStatus { get; set; }
    }
}