using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCMS.Areas.Main.Models;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace WebCMS.Areas.Main.Controllers
{
    public class TestController : Controller
    {
        public ActionResult FDC()
        {
            ViewData["FDCDevice"] = FDCExportModel.GetAllDevice();

            return View();
        }

        public ActionResult FDCExport(string type)
        {
            byte[] file = FDCExportModel.GetFDCExportFile(type);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FDCEXPORT_" + DateTime.Now.ToString("yyyy_MM_dd") + "_" + type + ".xlsx");
        }

        public ActionResult FDCDeviceExport(string channel_name)
        {
            string strFileName = channel_name + ".csv";

            byte[] file = FDCExportModel.GetFDCDeviceExportFile(channel_name);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", strFileName);
        }

        public JsonResult JsonModbusTest(string hostName,int port,string data)
        {
            //string resultString = "";
            try
            {
                TcpClient mTcpClient = new TcpClient();
                mTcpClient.Connect(hostName,port);

                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(100);
                    if (mTcpClient.Connected)
                    {
                        break;
                    }
                }
                if (mTcpClient.Connected)
                {
                    NetworkStream ns = mTcpClient.GetStream();

                    int posistion = int.Parse(data)-1;
                    
                    if (ns.CanWrite)
                    {

                        byte[] writeBf = new byte[12];

                        writeBf[0] = 0x00;
                        writeBf[1] = 0x01;
                        writeBf[2] = 0x00;
                        writeBf[3] = 0x00;
                        writeBf[4] = 0x00;
                        writeBf[5] = 0x06;
                        writeBf[6] = 0x01;
                        writeBf[7] = 0x03;
                        writeBf[8] = (byte)(posistion / 256);
                        writeBf[9] = (byte)(posistion % 256);
                        writeBf[10] = 0x00;
                        writeBf[11] = 0x01;

                        ns.Write(writeBf, 0, 12);
                        ns.Flush();

                        Thread.Sleep(1000);
                        byte[] readBf = new byte[12];
                        //AsyncCallback callback = new AsyncCallback(CompleteRead);

                        if (ns.CanRead)
                        {
                            //ns.Read(readBf,0,12);

                            IAsyncResult result = ns.BeginRead(readBf,0,12,null,null);
                            //IAsyncResult result = ns.BeginRead(readBf, 0, 12, callback, ns);
                            while (!result.IsCompleted)
                            {
                                Thread.Sleep(10);
                            }
                        }

                        int data1 = readBf[9] * 256 + readBf[10];
                        ns.Close();
                        mTcpClient.Close();
                        return Json(data1, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("NetworkStream not CanWrite", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json("not Connected", JsonRequestBehavior.AllowGet);
                }
                
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        //public JsonResult JsonModbusDBTest()
        //{
            
        //    using (SensorTestDataContext db = new SensorTestDataContext())
        //    {
        //        var r = (from row in db.vw_SensorValueNow select row).ToList();

        //        return Json(from row in r select new { id = row.id,SensorName= row.SensorName,SensorData= row.SensorData,builtDate= row.builtDate.Value.ToString("yyyy-MM-dd HH:mm:ss") }, JsonRequestBehavior.AllowGet);
        //    }
            
        //}
        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
