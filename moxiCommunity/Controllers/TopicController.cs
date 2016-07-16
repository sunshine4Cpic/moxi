using moxiCommunity.Models;
using moxiCommunity.ViewModels;
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
    public class TopicController : Controller
    {
        private static List<proNode> _pns;
        public static List<proNode> pns { get { return _pns.Take(5).ToList(); } }

        static TopicController()
        {
            //初始化节点信息

            var _jsonFilePath = HostingEnvironment.MapPath("~/App_Data/node.json");

            if (System.IO.File.Exists(_jsonFilePath))
            {
                var json = System.IO.File.ReadAllText(_jsonFilePath);
                var jo = JObject.Parse(json);

                var proNodeJson = (jo["ReturnObjects"] as JObject)["result"].ToString();

                _pns = JsonConvert.DeserializeObject<List<proNode>>(proNodeJson);
            }
        }

        // GET: Topic
        public ActionResult Index()
        {
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();
            var lsv = from t in db.Topic
                      where t.state != 0
                      select new topicPrevModel
                      {
                          ID = t.ID,
                          title = t.title,
                          creatDate = t.creatDate,
                          User = new UserModel { ID = t.CommunityUser.ID, Name = t.CommunityUser.Name },
                          budget = t.BuyDemand.budget,
                          replyCnt = t.replys,
                          solutionCnt = t.BuySolution.Count
                      };

            ViewBag.nodes = pns;
            ViewBag.nodeText = "全部";

            return View(lsv.ToList());
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("topic/{id:int}")]
        public ActionResult Topic(int id)
        {
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();

            var tic = db.Topic.FirstOrDefault(t => t.ID == id && t.state != 0);
            if (tic == null)
                return HttpNotFound();

            var md = new topicViewModel();
            md.ID = tic.ID;
            md.title = tic.title;
            md.state = tic.state;
            md.body = tic.body;
            md.creatDate = tic.creatDate;
            md.budget = tic.BuyDemand.budget;
            md.User = new UserModel { ID = tic.CommunityUser.ID, Name = tic.CommunityUser.Name };

            md.replies = (from t in tic.TopicReply
                         select new replyViewModel
                         {
                             ID = t.ID,
                             body = t.body,
                             state = t.state,
                             creatDate = t.creatDate,
                             floor = t.floor,
                             User = new UserModel { ID = t.CommunityUser.ID, Name = t.CommunityUser.Name }
                         }).ToList();

            md.Solutions = (from t in tic.BuySolution
                            select new SolutionModel
                            {
                                ID = t.ID,
                                reason = t.reason,
                                goodsLink = t.goodsLink
                            }).ToList();


            return View(md);
        }

        public ActionResult search(string q)
        {
            ViewBag.q = q;
            return View("Index");
        }

        [Authorize]
        [HttpPost]
        public ActionResult adopt(int id)
        {
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();
            var ts = db.BuySolution.First(t => t.ID == id);

            var tid = ts.topicID.ToString();
            if (ts.Topic.state == 1 || ts.Topic.state==0)
                return RedirectToAction(tid.ToString());

            if (ts.userID == User.Identity.userID())
            {
                ts.state = 2;//1可见 2采纳
                ts.Topic.state = 1;//关闭本需求
                db.SaveChanges();
            }

            return RedirectToAction(tid.ToString());
        }

        [HttpPost]
        [Authorize]
        public ActionResult add(topicAddModel md)
        {
            int userID = User.Identity.userID();

            moxiAgentBuyEntities db = new moxiAgentBuyEntities();
            var tp = new Topic();
            tp.userID = userID;
            tp.creatDate = DateTime.Now;
            tp.replys = 0;
            tp.title = md.title;
            tp.body = md.body;
            db.Topic.Add(tp);

            BuyDemand bd = new BuyDemand();
            bd.topicID = tp.ID;
            bd.budget = md.budget;
            db.BuyDemand.Add(bd);


            db.SaveChanges();
            return RedirectToAction(tp.ID.ToString());
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddSolution(BuySolutionAddModel md)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpException(500, "异常提交");
            }

            int userID = User.Identity.userID();
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();

            //是否有效主题
            var tic = db.Topic.FirstOrDefault(t => t.ID == md.topicID && t.state != 0);
            if (tic == null) throw new HttpException(404, "page not found");

            tic.replys += 1;


            var bs = new BuySolution();
            bs.topicID = md.topicID;
            bs.userID = userID;
            bs.body = md.body;
            bs.goodsLink = md.goodsLink;
            bs.creatDate = DateTime.Now;
            bs.state = 0;

            db.BuySolution.Add(bs);


            db.SaveChanges();

            return RedirectToAction(md.topicID.ToString());


        }

        [HttpPost]
        [Authorize]
        public ActionResult Reply(TopicReplyAddModel md)
        {


            if (!ModelState.IsValid)
            {
                throw new HttpException(500, "异常提交");
            }

            int userID = User.Identity.userID();
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();
            //是否有效主题
            var tic = db.Topic.FirstOrDefault(t => t.ID == md.topicID && t.state != 0);
            if (tic == null) throw new HttpException(404, "page not found");

            tic.replys += 1;


            var reply = new TopicReply();
            reply.topicID = md.topicID;
            reply.userID = userID;
            reply.body = md.body;
            reply.creatDate = DateTime.Now;
            reply.floor = tic.replys;


            db.TopicReply.Add(reply);


            db.SaveChanges();

            return RedirectToAction(md.topicID.ToString());

        }

    }
}