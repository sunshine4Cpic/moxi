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

            bs.deleteBody = md.body;

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


        [HttpPost]
        [Authorize]
        public ActionResult Edit(BuySolutionEditModel md)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpException(500, "异常提交");
            }

            int userID = User.Identity.userID();
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();

            //是否有效主题
            var bs = db.BuySolution.FirstOrDefault(t => t.ID == md.ID && t.userID == userID);
            if (bs == null) throw new HttpException(404, "page not found");

            //tic.replys += 1;


            bs.body = md.body;
            bs.goodsLink = md.goodsLink;



            db.SaveChanges();

            var url = Request.ServerVariables["Http_Referer"];
            return Redirect(url);
        }


        [HttpGet]
        [Authorize]
        public ActionResult Edit(int id)
        {
          
            int userID = User.Identity.userID();
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();

            //是否有效主题
            var bs = db.BuySolution.FirstOrDefault(t => t.ID == id && t.userID == userID);
            if (bs == null) throw new HttpException(404, "page not found");

            BuySolutionEditModel bem = new BuySolutionEditModel();
            bem.ID = bs.ID;
            bem.body = bs.body;
            bem.goodsLink = bs.goodsLink;

            return PartialView("_BuySolutionEdit", bem);
        }

       
    }
}