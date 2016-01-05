using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace REMSolution.Controllers
{
    [Authorize]
    public class PropertiesController : Controller
    {
        // GET: Properties
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EditProperty (int PropId = 0)
        {

            var model = new Models.RealPropertiesModel();

            model.GetSpecificProperty(PropId);

            return View(model);
        }

        [HttpPost]
        public ActionResult EditProperty(Models.RealPropertiesModel model)
        {

            if (model.DefaultImageUpload != null)
            {
                var fileName = Path.GetFileName(model.DefaultImageUpload.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/PropImages/"), fileName);
                model.DefaultImageUpload.SaveAs(path);

                model.DefaultImgUrl = "/Content/PropImages/" + model.DefaultImageUpload.FileName;
            }
            
            model.SavePropertyUpdate(model);



            model.GetSpecificProperty(model.PropertyID);

            return View(model);
        }

        public ActionResult UploadPic (int PropertyID, HttpPostedFileBase PicUpload)
        {

            var fileName = "Prop" + PropertyID + "_" + Path.GetFileName(PicUpload.FileName);
            var path = Path.Combine(Server.MapPath("~/Content/PropImages/"), fileName);
            PicUpload.SaveAs(path);

            var model = new Models.RealPropertiesModel();

            model.AddPropertyPic("/Content/PropImages/" + fileName, PropertyID);

            return RedirectToAction("EditProperty", new { PropId = PropertyID });

        }

    }
}