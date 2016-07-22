using moxiCommunity.Models;
using moxiCommunity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace moxiCommunity.Controllers
{
    
    public class SolutionController : Controller
    {
        //[HttpPost]
        //public ActionResult warn(SolutionHandleModel md)
        //{
        //    moxiAgentBuyEntities db = new moxiAgentBuyEntities();
        //    var bs = db.BuySolution.First(t => t.ID == md.bsID);
        //    bs.state = -1;

        //    var bsh = new BuySolutionHandle();
        //    bsh.bsID = bs.ID;
        //    bsh.body = md.body;
        //    bsh.closed = false;
        //    bsh.createDate = DateTime.Now;
        //    bsh.eventID = 101;
        //    db.BuySolutionHandle.Add(bsh);
        //    db.SaveChanges();

        //    return RedirectToAction(bs.ID.ToString(), "topic");
        //}

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult adminDelete(SolutionHandleModel md)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpException(500, "异常提交");
            }
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();
            var bs = db.BuySolution.First(t => t.ID == md.bsID);
            bs.state = -1;//管理员删除

            bs.BuySolutionHandle.Where(t => t.closed == false).All(t => t.closed = true);
          

            var bsh = new BuySolutionHandle();
            bsh.bsID = bs.ID;
            bsh.body = md.body;
            bsh.closed = false;
            bsh.createDate = DateTime.Now;
            bsh.eventID = 101;
            bsh.userID = User.Identity.userID();
            db.BuySolutionHandle.Add(bsh);
            db.SaveChanges();


            return RedirectToAction(bs.topicID.ToString(), "topic");
        }

        [Authorize]
        [HttpPost]
        public ActionResult delete(int id)
        {
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();
            var bs = db.BuySolution.First(t => t.ID == id);
            

            if(bs.userID != User.Identity.userID())
            {
                throw new HttpException(500, "异常提交");
            }

            bs.state = 0;//用户自己删
            db.SaveChanges();



            return RedirectToAction(bs.topicID.ToString(), "topic");
        }

        [Authorize]
        [HttpGet]
        public string deleteCause(int id)
        {
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();
            int userID = User.Identity.userID();
            var bs = db.BuySolution.First(t => t.ID == id);


            if (bs.userID != User.Identity.userID())
            {
                throw new HttpException(500, "异常提交");
            }

            var s = bs.BuySolutionHandle.LastOrDefault(t=>t.eventID==101);

            if (s == null)
                return "未知原因";
            else
                return s.body;
        }
    }
}