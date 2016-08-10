using moxiCommunity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace moxiCommunity.Controllers
{
    public class ReplyController : Controller
    {
        // GET: Reply
        public int up(int id)
        {
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();

            if (User.Identity.IsAuthenticated)//登录用户
            {
                int userID = User.Identity.userID();
                var ra = db.ReplyAgree.SingleOrDefault(t => t.replyID == id && t.userID == userID);
                if (ra == null)
                {
                    ra = new ReplyAgree();
                    ra.replyID = id;
                    ra.userID = userID;
                    db.ReplyAgree.Add(ra);
                }
                ra.agree = true;

            }
            else
            {
                string IP = User.userIP();
                var ra = db.ReplyAgree.SingleOrDefault(t => t.replyID == id && t.IP == IP);
                if (ra == null)
                {
                    ra = new ReplyAgree();
                    ra.replyID = id;
                    ra.IP = IP;
                    db.ReplyAgree.Add(ra);
                }
                ra.agree = true;
            }
            db.SaveChanges();

            return db.ReplyAgree.Count(t => t.replyID == id && t.agree == true);
        }

        public int unUp(int id)
        {
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();

            if (User.Identity.IsAuthenticated)//登录用户
            {
                int userID = User.Identity.userID();
                var ra = db.ReplyAgree.SingleOrDefault(t => t.replyID == id && t.userID == userID);
                if (ra == null)
                {
                    ra = new ReplyAgree();
                    ra.replyID = id;
                    ra.userID = userID;
                    db.ReplyAgree.Add(ra);
                }
                ra.agree = null;

            }
            else
            {
                string IP = User.userIP();
                var ra = db.ReplyAgree.SingleOrDefault(t => t.replyID == id && t.IP == IP);
                if (ra == null)
                {
                    ra = new ReplyAgree();
                    ra.replyID = id;
                    ra.IP = IP;
                    db.ReplyAgree.Add(ra);
                }
                ra.agree = null;
            }
            db.SaveChanges();

            return db.ReplyAgree.Count(t => t.replyID == id && t.agree == true);
        }

        public int down(int id)
        {

            moxiAgentBuyEntities db = new moxiAgentBuyEntities();

            if (User.Identity.IsAuthenticated)//登录用户
            {
                int userID = User.Identity.userID();
                var ra = db.ReplyAgree.SingleOrDefault(t => t.replyID == id && t.userID == userID);
                if (ra == null)
                {
                    ra = new ReplyAgree();
                    ra.replyID = id;
                    ra.userID = userID;
                    db.ReplyAgree.Add(ra);
                }
                ra.agree = false;

            }
            else
            {
                string IP = User.userIP();
                var ra = db.ReplyAgree.SingleOrDefault(t => t.replyID == id && t.IP == IP);
                if (ra == null)
                {
                    ra = new ReplyAgree();
                    ra.replyID = id;
                    ra.IP = IP;
                    db.ReplyAgree.Add(ra);
                }
                ra.agree = false;
            }
            db.SaveChanges();
            return db.ReplyAgree.Count(t => t.replyID == id && t.agree == false);
        }


        public int unDown(int id)
        {

            moxiAgentBuyEntities db = new moxiAgentBuyEntities();

            if (User.Identity.IsAuthenticated)//登录用户
            {
                int userID = User.Identity.userID();
                var ra = db.ReplyAgree.SingleOrDefault(t => t.replyID == id && t.userID == userID);
                if (ra == null)
                {
                    ra = new ReplyAgree();
                    ra.replyID = id;
                    ra.userID = userID;
                    db.ReplyAgree.Add(ra);
                }
                ra.agree = null;

            }
            else
            {
                string IP = User.userIP();
                var ra = db.ReplyAgree.SingleOrDefault(t => t.replyID == id && t.IP == IP);
                if (ra == null)
                {
                    ra = new ReplyAgree();
                    ra.replyID = id;
                    ra.IP = IP;
                    db.ReplyAgree.Add(ra);
                }
                ra.agree = null;
            }
            db.SaveChanges();
            return db.ReplyAgree.Count(t => t.replyID == id && t.agree == false);
        }

        [HttpPost]
        [Authorize(Roles="admin")]
        public void delete(int id)
        {
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();
            var rp = db.TopicReply.FirstOrDefault(t => t.ID == id);
            rp.state = 0;
            db.SaveChanges();
        }
    }
}