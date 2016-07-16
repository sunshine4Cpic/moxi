using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace moxiCommunity.Controllers
{
    [Authorize(Roles="admin")]
    public class SolutionController : Controller
    {
        // GET: Solution
        public ActionResult Index()
        {
            return View();
        }
    }
}