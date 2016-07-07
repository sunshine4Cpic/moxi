using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace moxiCommunity.Controllers
{
    public class TopicController : Controller
    {
        // GET: Topic
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult search(string q)
        {
            ViewBag.q = q;
            return View("Index");
        }

        [Authorize]
        public ActionResult add()
        {
            return View();
        }
    }
}