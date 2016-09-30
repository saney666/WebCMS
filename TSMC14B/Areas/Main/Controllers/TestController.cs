using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TSMC14B.Areas.Main.Models;

namespace TSMC14B.Areas.Main.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Main/Test/

        public ActionResult FDCExport()
        {
            byte[] file = FDCExportModel.GetFDCExportFile();
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FDCEXPORT_" + DateTime.Now.ToString("yyyy_MM_dd") + ".xlsx");
        }
    }
}
