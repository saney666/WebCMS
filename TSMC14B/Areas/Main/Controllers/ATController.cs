using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCMS.Areas.Main.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.IO;
using Drawing = DocumentFormat.OpenXml.Drawing;
using A14 = DocumentFormat.OpenXml.Office2010.Drawing;
using P = DocumentFormat.OpenXml.Presentation;
using A = DocumentFormat.OpenXml.Drawing;

namespace WebCMS.Areas.Main.Controllers
{
    public class ATController : Controller
    {
        // GET: /Main/AT/
        #region Schedule
        public ActionResult ScheduleList(string output)
        {
            if (!string.IsNullOrEmpty((string)Session["UserID"]))
            {
                if (output != "export")
                {
                    ViewBag.bar = "ATSchedule";
                    return View();
                }
                else
                {
                    byte[] file = ATScheduleModel.GetScheduleFile(Session["UserID"].ToString());
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Delayed_Upload_" + DateTime.Now.ToString("yyyy_MM_dd") + ".xlsx");
                }
            }
            else
            {
                ViewBag.MessageType = "警告";
                ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                return View("Message");
            }
        }
        public JsonResult JsonScheduleList(int page, int rp)
        {
            try
            {
                IEnumerable<ATScheduleModel> al = ATScheduleModel.GetScheduleList(Session["UserID"].ToString());

                return Json(new
                {
                    page = page,
                    total = al.Count(),
                    rows = from c in al.Skip((page - 1) * rp).Take(rp)
                           select c
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    ex.Message
                });
            }
        }
        public ActionResult AddSchedule()
        {
            ViewBag.Title = WebCMS.Menu.Add + WebCMS.Menu.Schedule;

            ATScheduleModel arm = new ATScheduleModel()
            {
                ExecuteType = "d",
                DataType = "d",
                ExecuteCycle = 1,
                DataCycle = 1,
                SamplingTime = 1
            };
            return View(arm);
        }

        public ActionResult editSchedule(int ATSID)
        {
            ViewBag.Title = WebCMS.Menu.Edit + WebCMS.Menu.Schedule;
            ATScheduleModel arm = ATScheduleModel.GetATSchedule(ATSID);

            return View("addSchedule", arm);
        }

        [HttpPost]
        public ActionResult editSchedule(ATScheduleModel arm)
        {
            ViewBag.Title = WebCMS.Menu.Edit + WebCMS.Menu.Schedule;

            if (!string.IsNullOrEmpty((string)Session["UserID"]))
            {
                if (ModelState.IsValid)
                {
                    string DBMsg = arm.Update((string)Session["UserID"]);
                    if (string.IsNullOrEmpty(DBMsg))
                        ViewBag.Message = "修改完成…";
                    else
                        ViewBag.Message = DBMsg;
                }
                else
                    ViewBag.Message = "資料不正確…";

                return View("addSchedule", arm);
            }
            else
            {
                ViewBag.MessageType = "警告";
                ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                return View("Message");
            }
        }

        [HttpPost]
        public ActionResult AddSchedule(ATScheduleModel arm)
        {
            if (!string.IsNullOrEmpty((string)Session["UserID"]))
            {
                if (ModelState.IsValid)
                {
                    string DBMsg = arm.Add((string)Session["UserID"]);
                    if (string.IsNullOrEmpty(DBMsg))
                        ViewBag.Message = "修改完成…";
                    else
                        ViewBag.Message = DBMsg;
                }
                else
                    ViewBag.Message = "資料不正確…";

                return View(arm);
            }
            else
            {
                ViewBag.MessageType = "警告";
                ViewBag.Message = "您無權限使用本功能或長時間未使用已自動登出，請重新登入，如有疑問請洽管理員，謝謝…";
                return View("Message");
            }
        }

        public ActionResult ScheduleHistory()
        {
            return View();
        }
        public JsonResult JsonScheduleDelete(int ATSID)
        {
            string s = string.Empty;

            if (!string.IsNullOrEmpty((string)Session["UserID"]))
            {
                string DBMsg = ATScheduleModel.Delete(ATSID);
                if (string.IsNullOrEmpty(DBMsg))
                    s = WebCMS.Menu.Delete + " ok";
                else
                    s = DBMsg;
            }

            return Json(s);
        }

        public ActionResult DownLoadATCPPT(int ATSID)
        {
            string ExportFileName = "test.pptx";

            try
            {

                System.IO.File.Copy(Server.MapPath("~/ATSchedule/") + "Sample.pptx", Server.MapPath("~/ATSchedule/") + "SampleTemp.pptx", true);

                using (PresentationDocument prstDoc = PresentationDocument.Open(Server.MapPath("~/ATSchedule/") + "SampleTemp.pptx", true))
                {

                    using (tsmc14BDataContext db = new tsmc14BDataContext())
                    {
                        var ATSLast = db.vw_ATScheduleLastHistory.Where(x => x.ATSID == ATSID).SingleOrDefault();

                        if (ATSLast != null)
                        {
                            string ATSFolderString = Server.MapPath("~/ATSchedule/") + ATSID + "/" + ATSLast.FolderName;

                            System.IO.DirectoryInfo ATSFolder = new System.IO.DirectoryInfo(ATSFolderString);

                            if (ATSFolder.Exists)
                            {
                                FileInfo[] ATSFiles = ATSFolder.GetFiles();

                                for (int i = 0; i < ATSFiles.Count(); i++)
                                {
                                    string imgId = "rId" + ATSFiles[i].Name;
                                    Slide S = InsertNewSlide(prstDoc, i, imgId, ATSFiles[i]);
                                    InsertImageInLastSlide(S, ATSFiles[i].FullName, "image/png");
                                }
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }

                    SlideIdList slideIdList = prstDoc.PresentationPart.Presentation.SlideIdList;
                    SlideId slideId = slideIdList.ChildElements[slideIdList.Count() - 1] as SlideId;
                    string slideRelId = slideId.RelationshipId;
                    slideIdList.RemoveChild(slideId);
                    SlidePart slidePart = prstDoc.PresentationPart.GetPartById(slideRelId) as SlidePart;
                    prstDoc.PresentationPart.DeletePart(slidePart);

                    prstDoc.PresentationPart.Presentation.Save();
                    return File(Server.MapPath("~/ATSchedule/") + "SampleTemp.pptx", "application/mspowerpoint", ExportFileName);

                }
            }
            catch (Exception ex)
            {
                MemoryStream fs = new MemoryStream();
                TextWriter tx = new StreamWriter(fs);
                tx.WriteLine(ex.Message);
                tx.Flush();
                return File(fs.ToArray(), "text/text", "error.txt");
            }
        }

        // Insert the specified slide into the presentation at the specified position.
        public static Slide InsertNewSlide(PresentationDocument presentationDocument, int position, string slideTitle, FileInfo ChartFile)
        {

            if (presentationDocument == null)
            {
                throw new ArgumentNullException("presentationDocument");
            }

            if (slideTitle == null)
            {
                throw new ArgumentNullException("slideTitle");
            }

            PresentationPart presentationPart = presentationDocument.PresentationPart;

            // Verify that the presentation is not empty.
            if (presentationPart == null)
            {
                throw new InvalidOperationException("The presentation document is empty.");
            }

            // Declare and instantiate a new slide.
            Slide slide = new Slide(new CommonSlideData(new ShapeTree()));
            //uint drawingObjectId = 1;

            // Construct the slide content.            
            // Specify the non-visual properties of the new slide.
            NonVisualGroupShapeProperties nonVisualProperties = slide.CommonSlideData.ShapeTree.AppendChild(new NonVisualGroupShapeProperties());
            nonVisualProperties.NonVisualDrawingProperties = new NonVisualDrawingProperties() { Id = 1, Name = "" };
            nonVisualProperties.NonVisualGroupShapeDrawingProperties = new NonVisualGroupShapeDrawingProperties();
            nonVisualProperties.ApplicationNonVisualDrawingProperties = new ApplicationNonVisualDrawingProperties();

            // Specify the group shape properties of the new slide.
            slide.CommonSlideData.ShapeTree.AppendChild(new GroupShapeProperties());

            //// Declare and instantiate the title shape of the new slide.
            //DocumentFormat.OpenXml.Presentation.Shape titleShape = slide.CommonSlideData.ShapeTree.AppendChild(new DocumentFormat.OpenXml.Presentation.Shape());

            //drawingObjectId++;

            //// Specify the required shape properties for the title shape. 
            //titleShape.NonVisualShapeProperties = new NonVisualShapeProperties
            //    (new NonVisualDrawingProperties() { Id = drawingObjectId, Name = "Title" },
            //    new NonVisualShapeDrawingProperties(new Drawing.ShapeLocks() { NoGrouping = true }),
            //    new ApplicationNonVisualDrawingProperties(new PlaceholderShape() { Type = PlaceholderValues.Title }));
            //titleShape.ShapeProperties = new DocumentFormat.OpenXml.Presentation.ShapeProperties();

            //// Specify the text of the title shape.
            //titleShape.TextBody = new TextBody(new Drawing.BodyProperties(),
            //        new Drawing.ListStyle(),
            //        new Drawing.Paragraph(new Drawing.Run(new Drawing.Text() { Text = slideTitle })));


            //// Declare and instantiate the body shape of the new slide.
            //DocumentFormat.OpenXml.Presentation.Shape bodyShape = slide.CommonSlideData.ShapeTree.AppendChild(new DocumentFormat.OpenXml.Presentation.Shape());
            //drawingObjectId++;

            //// Specify the required shape properties for the body shape.
            //bodyShape.NonVisualShapeProperties = new NonVisualShapeProperties(new NonVisualDrawingProperties() { Id = drawingObjectId, Name = "Content Placeholder" },
            //        new NonVisualShapeDrawingProperties(new Drawing.ShapeLocks() { NoGrouping = true }),
            //        new ApplicationNonVisualDrawingProperties(new PlaceholderShape() { Index = 1 }));
            //bodyShape.ShapeProperties = new DocumentFormat.OpenXml.Presentation.ShapeProperties();

            //// Specify the text of the body shape.
            //bodyShape.TextBody = new TextBody(new Drawing.BodyProperties(),
            //        new Drawing.ListStyle(),
            //        new Drawing.Paragraph());

            // Declare and instantiate the body shape of the new slide.
            //DocumentFormat.OpenXml.Presentation.Shape ImgShape = slide.CommonSlideData.ShapeTree.AppendChild(new DocumentFormat.OpenXml.Presentation.Shape());
            //drawingObjectId++;

            // Specify the required shape properties for the body shape.
            //ImgShape.NonVisualShapeProperties = new NonVisualShapeProperties(new NonVisualDrawingProperties() { Id = drawingObjectId, Name = "" },
            //        new NonVisualGroupShapeDrawingProperties(),
            //        new ApplicationNonVisualDrawingProperties());
            //ImgShape.ShapeProperties = new DocumentFormat.OpenXml.Presentation.ShapeProperties();

            // Specify the text of the body shape.
            //ImgShape.TextBody = new TextBody(new Drawing.BodyProperties(),
            //        new Drawing.ListStyle(),
            //        new Drawing.Paragraph());

            //Drawing.Picture picture = new Drawing.Picture();
            ////string embedId = string.Empty;
            //string embedId = "rId" + (slide.Elements().Count() + 915).ToString();
            //Drawing.NonVisualPictureProperties nonVisualPictureProperties = new Drawing.NonVisualPictureProperties(
            //    new Drawing.NonVisualDrawingProperties() { Id = (UInt32Value)4U, Name = "Picture" + ChartFile.Name },
            //    new Drawing.NonVisualPictureDrawingProperties(new Drawing.PictureLocks() { NoChangeAspect = true }),
            //    new ApplicationNonVisualDrawingProperties());


            //BlipFill blipFill1 = new BlipFill();
            //Drawing.Blip blip1 = new Drawing.Blip() { Embed = embedId, CompressionState = Drawing.BlipCompressionValues.Print };

            //// Creates an BlipExtensionList instance and adds its children 
            //Drawing.BlipExtensionList blipExtensionList = new Drawing.BlipExtensionList();
            //Drawing.BlipExtension blipExtension = new Drawing.BlipExtension() { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" };

            //A14.UseLocalDpi useLocalDpi = new A14.UseLocalDpi() { Val = false };
            //useLocalDpi.AddNamespaceDeclaration("a14",
            //    "http://schemas.microsoft.com/office/drawing/2010/main");

            //blipExtension.Append(useLocalDpi);
            //blipExtensionList.Append(blipExtension);
            //blip1.Append(blipExtensionList);


            //Drawing.Stretch stretch1 = new Drawing.Stretch();
            //Drawing.FillRectangle fillRectangle1 = new Drawing.FillRectangle();
            //stretch1.Append(fillRectangle1);

            //blipFill1.Append(blip1);
            //blipFill1.Append(stretch1);

            //Drawing.ShapeProperties shapeProperties = new Drawing.ShapeProperties();

            //Drawing.Transform2D transform2D = new Drawing.Transform2D();
            //Drawing.Offset offset = new Drawing.Offset() { X = 0L, Y = 0L };
            //Drawing.Extents extents = new Drawing.Extents() { Cx = 8000000L, Cy = 6000000L };

            //transform2D.Append(offset);
            //transform2D.Append(extents);


            //Drawing.PresetGeometry presetGeometry = new Drawing.PresetGeometry() { Preset = Drawing.ShapeTypeValues.Rectangle };
            //Drawing.AdjustValueList adjustValueList = new Drawing.AdjustValueList();


            //presetGeometry.Append(adjustValueList);


            //shapeProperties.Append(transform2D);
            //shapeProperties.Append(presetGeometry);


            //picture.Append(nonVisualPictureProperties);
            //picture.Append(blipFill1);
            //picture.Append(shapeProperties);


            //slide.CommonSlideData.ShapeTree.AppendChild(picture);



            // Create the slide part for the new slide.
            SlidePart slidePart = presentationPart.AddNewPart<SlidePart>();

            // Save the new slide part.
            slide.Save(slidePart);

            // Modify the slide ID list in the presentation part.
            // The slide ID list should not be null.
            SlideIdList slideIdList = presentationPart.Presentation.SlideIdList;

            // Find the highest slide ID in the current list.
            uint maxSlideId = 1;
            SlideId prevSlideId = null;

            foreach (SlideId slideId in slideIdList.ChildElements)
            {
                if (slideId.Id > maxSlideId)
                {
                    maxSlideId = slideId.Id;
                }

                position--;
                if (position == 0)
                {
                    prevSlideId = slideId;
                }

            }

            maxSlideId++;

            // Get the ID of the previous slide.
            SlidePart lastSlidePart;

            if (prevSlideId != null)
            {
                lastSlidePart = (SlidePart)presentationPart.GetPartById(prevSlideId.RelationshipId);
            }
            else
            {
                lastSlidePart = (SlidePart)presentationPart.GetPartById(((SlideId)(slideIdList.ChildElements[0])).RelationshipId);
            }

            // Use the same slide layout as that of the previous slide.
            if (null != lastSlidePart.SlideLayoutPart)
            {
                slidePart.AddPart(lastSlidePart.SlideLayoutPart);
            }

            // Insert the new slide into the slide list after the previous slide.
            SlideId newSlideId = slideIdList.InsertAfter(new SlideId(), prevSlideId);
            newSlideId.Id = maxSlideId;
            newSlideId.RelationshipId = presentationPart.GetIdOfPart(slidePart);

            // Save the modified presentation.
            //presentationPart.Presentation.Save();

            return GetSlideByRelationShipId(presentationPart, newSlideId.RelationshipId);
        }

        private static Slide GetSlideByRelationShipId(PresentationPart presentationPart, StringValue relationshipId)
        {
            // Get Slide object by Relationship ID 
            SlidePart slidePart = presentationPart.GetPartById(relationshipId) as SlidePart;
            if (slidePart != null)
            {
                return slidePart.Slide;
            }
            else
            {
                return null;
            }
        }

        public void InsertImageInLastSlide(Slide slide, string imagePath, string imageExt)
        {
            // Creates an Picture instance and adds its children. 
            P.Picture picture = new P.Picture();
            string embedId = string.Empty;
            embedId = "rId" + (slide.Elements().Count() + 915).ToString();
            P.NonVisualPictureProperties nonVisualPictureProperties = new P.NonVisualPictureProperties(
                new P.NonVisualDrawingProperties() { Id = (UInt32Value)4U, Name = "Picture 5" },
                new P.NonVisualPictureDrawingProperties(new A.PictureLocks() { NoChangeAspect = true }),
                new ApplicationNonVisualDrawingProperties());


            P.BlipFill blipFill = new P.BlipFill();
            Drawing.Blip blip = new Drawing.Blip() { Embed = embedId };


            // Creates an BlipExtensionList instance and adds its children 
            Drawing.BlipExtensionList blipExtensionList = new Drawing.BlipExtensionList();
            Drawing.BlipExtension blipExtension = new Drawing.BlipExtension() { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" };


            DocumentFormat.OpenXml.Office2010.Drawing.UseLocalDpi useLocalDpi = new DocumentFormat.OpenXml.Office2010.Drawing.UseLocalDpi() { Val = false };
            useLocalDpi.AddNamespaceDeclaration("a14",
                "http://schemas.microsoft.com/office/drawing/2010/main");


            blipExtension.Append(useLocalDpi);
            blipExtensionList.Append(blipExtension);
            blip.Append(blipExtensionList);


            Drawing.Stretch stretch = new Drawing.Stretch();
            Drawing.FillRectangle fillRectangle = new Drawing.FillRectangle();
            stretch.Append(fillRectangle);


            blipFill.Append(blip);
            blipFill.Append(stretch);


            // Creates an ShapeProperties instance and adds its children. 
            P.ShapeProperties shapeProperties = new P.ShapeProperties();


            A.Transform2D transform2D = new A.Transform2D();
            A.Offset offset = new A.Offset() { X = 2096400L, Y = 441000L };
            A.Extents extents = new A.Extents() { Cx = 8000000L, Cy = 6000000L };


            transform2D.Append(offset);
            transform2D.Append(extents);


            A.PresetGeometry presetGeometry = new A.PresetGeometry() { Preset = A.ShapeTypeValues.Rectangle };
            A.AdjustValueList adjustValueList = new A.AdjustValueList();


            presetGeometry.Append(adjustValueList);


            shapeProperties.Append(transform2D);
            shapeProperties.Append(presetGeometry);


            picture.Append(nonVisualPictureProperties);
            picture.Append(blipFill);
            picture.Append(shapeProperties);


            slide.CommonSlideData.ShapeTree.AppendChild(picture);


            // Generates content of imagePart. 
            ImagePart imagePart = slide.SlidePart.AddNewPart<ImagePart>(imageExt, embedId);
            FileStream fileStream = new FileStream(imagePath, FileMode.Open);
            imagePart.FeedData(fileStream);
            fileStream.Close();
        }

        #endregion

        #region editScheduleChart
        public ActionResult EditScheduleChart(int ATSID, string ATName)
        {
            if (!string.IsNullOrEmpty((string)Session["UserID"]))
            {
                ViewBag.Title = WebCMS.Menu.EditTCBook;
                using (tsmc14BDataContext db = new tsmc14BDataContext())
                {
                    ViewData["ATName"] = ATName;
                    ViewData["TCBookData"] = db.vw_TCBook_info.Where(x => x.Login_name == (string)Session["UserID"]).ToList();

                    ViewData["SelectTCBID"] = db.ATC_info.Where(x => x.ATSID == ATSID).ToList();
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
        [HttpPost]
        public ActionResult EditScheduleChart(int ATSID, string ATName, string TCBIDString)
        {
            if (!string.IsNullOrEmpty((string)Session["UserID"]))
            {
                ViewBag.Title = WebCMS.Menu.EditTCBook;
                try
                {
                    using (tsmc14BDataContext db = new tsmc14BDataContext())
                    {
                        ViewData["ATName"] = ATName;
                        ViewData["TCBookData"] = db.vw_TCBook_info.Where(x => x.Login_name == (string)Session["UserID"]).ToList();

                        var ATCList = db.ATC_info.Where(x => x.ATSID == ATSID).ToList();
                        if (ATCList != null && ATCList.Count > 0)
                        {
                            db.ATC_info.DeleteAllOnSubmit(ATCList);
                            db.SubmitChanges();
                        }

                        if (TCBIDString.Length > 0)
                        {
                            string[] S = TCBIDString.Split(',');
                            foreach (string item in S)
                            {
                                ATC_info ATC = new ATC_info();
                                ATC.ATSID = ATSID;
                                ATC.TCBID = int.Parse(item);

                                db.ATC_info.InsertOnSubmit(ATC);
                            }
                            db.SubmitChanges();
                        }

                        ViewBag.Message = WebCMS.Menu.Edit + "ok";
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
        #endregion
    }
}
