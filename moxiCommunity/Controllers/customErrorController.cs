using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace moxiCommunity.Controllers
{
    public class customErrorController : Controller
    {
        // GET: customError
        public ActionResult Error404()
        {
            Response.StatusCode = 404;
            ViewBag.code = 404;
            ViewBag.msg = "你来到了一个未被开发的区域...";
            return View("Error");
        }

        public ActionResult Error500()
        {
            Response.StatusCode = 500;
            ViewBag.code = 500;
            ViewBag.msg = "服务器异常,请稍微再试...";
            return View("Error");
        }
    }
}