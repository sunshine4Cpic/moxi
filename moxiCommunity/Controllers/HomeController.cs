using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace moxiCommunity.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "moxi代购社区施工中.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "你可以通过以下方式联系我";

            return View();
        }
    }
}