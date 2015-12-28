using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Xml;
using System.Text.RegularExpressions;

namespace REMSolution.Controllers
{
    public class SiteScrubberController : Controller
    {
        // GET: SiteScrubber
        public ActionResult Index()
        {

            PropertyScraper oScrape = new PropertyScraper();

            oScrape.RealCompOnline("http://matrix.realcomponline.com/Matrix/Public/Portal.aspx?ID=0-658084431-10");

            oScrape.RealtorDotCom("http://www.realtor.com/realestateandhomes-search/Ferndale_MI/price-na-50000");

            return View();
        }
    }
}