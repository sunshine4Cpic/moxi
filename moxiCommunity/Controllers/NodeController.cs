using moxiCommunity.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace moxiCommunity.Controllers
{
    public class NodeController : Controller
    {

        [Route("node/{id:int}")]
        public ActionResult Node(int id)
        {
            var nodes =  TopicController.nodes;
            return Json(nodes.FirstOrDefault(t => t.proClassID == id), JsonRequestBehavior.AllowGet);
            
        }
    }
}