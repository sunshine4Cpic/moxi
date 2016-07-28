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

        public static List<proNode> nodes
        {
            get
            {
                System.Web.Caching.Cache objCache = HttpRuntime.Cache;

                List<proNode> ns = objCache.Get("nodes") as List<proNode>;
                if (ns == null)
                {
                    var newNodes = getNodes();
                    objCache.Insert("nodes", newNodes, null, DateTime.Now.AddMinutes(10), TimeSpan.Zero);
                    return newNodes;
                }
                else
                    return ns;

            }
        }

        public static Dictionary<int, string> nodesDic { get; set; }

        [NonAction]
        private static List<proNode> getNodes()
        {
            //初始化节点信息

            var _jsonFilePath = HostingEnvironment.MapPath("~/App_Data/node.json");

            if (System.IO.File.Exists(_jsonFilePath))
            {
                var json = System.IO.File.ReadAllText(_jsonFilePath);
                var jo = JObject.Parse(json);

                var proNodeJson = jo["nodes"].ToString();

                var objs = JsonConvert.DeserializeObject<List<proNode>>(proNodeJson);

                return objs;
            }
            return null;
        }

        [NonAction]
        public static proNode getNode(int id)
        {
            foreach (var rn in nodes)
            {
                foreach (var n in rn.ThirdJsons)
                {
                    if (n.proClassID == id)
                    {
                        return n;
                    }
                }
            }

            return new proNode { proClassID = id, proClassName = "未知分类" };

        }


        public ActionResult search(string q,int page=1,int row = 20)
        {
            if(string.IsNullOrEmpty(q))
            {
                return Redirect("/");
            }

            moxiAgentBuyEntities db = new moxiAgentBuyEntities();


            var lsv = from t in db.Topic
                      where t.state > 0 && t.title.Contains(q)
                      orderby t.ID descending
                      select new topicPrevModel
                      {
                          ID = t.ID,
                          title = t.title,
                          creatDate = t.creatDate,
                          state = t.state,
                          User = new UserModel { ID = t.CommunityUser.ID, Name = t.CommunityUser.Name, avatar = t.CommunityUser.avatar, lv = t.CommunityUser.lv },
                          budget = t.BuyDemand.budget,
                          replyCnt = t.replys,
                          solutionCnt = t.BuySolution.Count
                      };

            ViewBag.row = row;
            ViewBag.page = page;
            ViewBag.total = lsv.Count();
            ViewBag.q = q;

            return View(lsv.Skip(row * (page - 1) * row).Take(row).ToList());


        }

        // GET: Topic
        public ActionResult Index(string id, int page = 1)
        {

            var topics = this.getTopics(null, id, page);

            topicIndexViewModel model = new topicIndexViewModel();
            model.nodes = nodes;
            model.selectNode = new proNode { proClassID = 0, proClassName = "全部" };
            model.topicList = topics;


            return View(model);


        }

      


        // GET: Topic
        public ActionResult node(int id, string catchall, int page = 1)
        {
            var topics = this.getTopics(id, catchall, page);



            topicIndexViewModel model = new topicIndexViewModel();
            model.nodes = nodes;
            model.selectNode = getNode(id);
            model.topicList = topics;


            return View("index", model);
        }

        [NonAction]
        private List<topicPrevModel> getTopics(int? nodeID, string catchall, int page)
        {

            string order = "time";
            string by = "desc";
            string Adopt = "false";
            string have = "false";

            if (catchall != null)
            {
                string[] ps = catchall.Split('-');
                order = ps[0];
                by = ps[1];
                Adopt = ps[2];
                have = ps[3];
            }

            moxiAgentBuyEntities db = new moxiAgentBuyEntities();

            IQueryable<Topic> tps = db.Topic;


            if (nodeID != null)
                tps = tps.Where(t => t.node == nodeID);


            if (Adopt == "adopt" && have == "have")
                ;
            else if (Adopt == "adopt")
                tps = tps.Where(t => t.state == 2);
            else if (have == "have")
                tps = tps.Where(t => t.BuySolution.Count > 0 );
            else
                tps = tps.Where(t => t.state > 0);

            //分页
            ViewBag.page = page;
            ViewBag.total = tps.Count();
            ViewBag.row = 20;

            if (order == "hot")
            {
                if (by == "asc")
                {
                    tps = tps.OrderBy(t => t.replys);
                }
                else
                {
                    tps = tps.OrderByDescending(t => t.replys);
                }
            }
            else
            {
                if (by == "asc")
                {
                    tps = tps.OrderBy(t => t.creatDate);
                }
                else
                {
                    tps = tps.OrderByDescending(t => t.creatDate);
                }
            }

            var lsv = from t in tps
                      select new topicPrevModel
                      {
                          ID = t.ID,
                          title = t.title,
                          creatDate = t.creatDate,
                          state = t.state,
                          User = new UserModel { ID = t.CommunityUser.ID, Name = t.CommunityUser.Name, avatar = t.CommunityUser.avatar, lv = t.CommunityUser.lv },
                          budget = t.BuyDemand.budget,
                          replyCnt = t.replys,
                          solutionCnt = t.BuySolution.Count
                      };
            return lsv.Skip((page - 1) * 20).Take(20).ToList();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult allSolutions(int id)
        {
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();

            var tic = db.Topic.FirstOrDefault(t => t.ID == id && t.state > 0);
            if (tic == null)
                return HttpNotFound();

            var md = new topicViewModel();
            md.ID = tic.ID;
            md.title = tic.title;
            md.state = tic.state;
            md.body = tic.body;
            md.creatDate = tic.creatDate;
            md.budget = tic.BuyDemand.budget;
            md.User = new UserModel { ID = tic.CommunityUser.ID, Name = tic.CommunityUser.Name, avatar = tic.CommunityUser.avatar, lv = tic.CommunityUser.lv };


            md.solutionCnt = tic.BuySolution.Count(t => t.state > 0);
            md.Solutions = (from t in tic.BuySolution
                            where t.state != 2 && t.state > 0
                            orderby t.ID descending
                            select new SolutionModel
                            {
                                ID = t.ID,
                                body = t.body,
                                state = t.state,
                                creatDate = t.creatDate,
                                User = new UserModel { ID = t.CommunityUser.ID, Name = t.CommunityUser.Name, avatar = t.CommunityUser.avatar, lv = t.CommunityUser.lv },
                                goodsLink = t.goodsLink
                            }).ToList();

            md.AdoptSolution = (from t in tic.BuySolution
                                where t.state == 2
                                orderby t.ID descending
                                select new SolutionModel
                                {
                                    ID = t.ID,
                                    body = t.body,
                                    state = t.state,
                                    creatDate = t.creatDate,
                                    User = new UserModel { ID = t.CommunityUser.ID, Name = t.CommunityUser.Name, avatar = t.CommunityUser.avatar, lv = t.CommunityUser.lv },
                                    goodsLink = t.goodsLink
                                }).FirstOrDefault();
            ViewBag.all = true;
            return View("Solutions", md);
        }


        [AllowAnonymous]
        [HttpGet]
        public ActionResult Solutions(int id)
        {
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();

            var tic = db.Topic.FirstOrDefault(t => t.ID == id && t.state > 0);
            if (tic == null)
                return HttpNotFound();

            var md = new topicViewModel();
            md.ID = tic.ID;
            md.title = tic.title;
            md.state = tic.state;
            md.body = tic.body;
            md.creatDate = tic.creatDate;
            md.budget = tic.BuyDemand.budget;
            md.User = new UserModel { ID = tic.CommunityUser.ID, Name = tic.CommunityUser.Name, avatar = tic.CommunityUser.avatar, lv = tic.CommunityUser.lv };


            md.solutionCnt = tic.BuySolution.Count(t => t.state > 0);
            md.Solutions = (from t in tic.BuySolution
                            where t.state != 2 && t.state > 0
                            orderby t.ID descending
                            select new SolutionModel
                            {
                                ID = t.ID,
                                body = t.body,
                                state = t.state,
                                creatDate = t.creatDate,
                                User = new UserModel { ID = t.CommunityUser.ID, Name = t.CommunityUser.Name, avatar = t.CommunityUser.avatar, lv = t.CommunityUser.lv },
                                goodsLink = t.goodsLink
                            }).Take(2).ToList();

            md.AdoptSolution = (from t in tic.BuySolution
                                where t.state == 2
                                orderby t.ID descending
                                select new SolutionModel
                                {
                                    ID = t.ID,
                                    body = t.body,
                                    state = t.state,
                                    creatDate = t.creatDate,
                                    User = new UserModel { ID = t.CommunityUser.ID, Name = t.CommunityUser.Name, avatar = t.CommunityUser.avatar, lv = t.CommunityUser.lv },
                                    goodsLink = t.goodsLink
                                }).FirstOrDefault();

            return View("Solutions", md);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("topic/{id:int}")]
        public ActionResult Topic(int id)
        {
            return Solutions(id);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Replys(int id,int page=1)
        {
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();

            var tic = db.Topic.FirstOrDefault(t => t.ID == id && t.state > 0);
            if (tic == null)
                return HttpNotFound();

            var md = new topicViewModel();
            md.ID = tic.ID;
            md.title = tic.title;
            md.state = tic.state;
            md.body = tic.body;
            md.creatDate = tic.creatDate;
            md.budget = tic.BuyDemand.budget;
            md.User = new UserModel { ID = tic.CommunityUser.ID, Name = tic.CommunityUser.Name, avatar = tic.CommunityUser.avatar, lv = tic.CommunityUser.lv };

            md.replies = (from t in tic.TopicReply
                          where t.state > 0
                          orderby t.ID descending
                          select new replyViewModel
                          {
                              ID = t.ID,
                              body = t.body,
                              state = t.state,
                              creatDate = t.creatDate,
                              floor = t.floor,
                              User = new UserModel { ID = t.CommunityUser.ID, Name = t.CommunityUser.Name, avatar = t.CommunityUser.avatar, lv = t.CommunityUser.lv }
                          }).Skip((page - 1) * 15).Take(15).ToList();

            ViewBag.page = page;
            ViewBag.row = 15;
            ViewBag.total = tic.TopicReply.Count(t => t.state > 0);

            return View(md);
        }



        [Authorize]
        [HttpPost]
        public ActionResult adopt(int id)
        {
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();
            var ts = db.BuySolution.First(t => t.ID == id);

            var tid = ts.topicID.ToString();
            if (ts.Topic.state != 1)
                return RedirectToAction(tid.ToString());

            if (ts.Topic.userID == User.Identity.userID())
            {
                ts.state = 2;//1可见 2采纳
                ts.Topic.state = 2;//需求已采纳
                db.SaveChanges();
            }
            var url = Request.ServerVariables["Http_Referer"];
            return Redirect(url);
            //return RedirectToAction(tid.ToString());
        }

        [HttpGet]
        public ActionResult add()
        {
            ViewBag.nodesList = NodesList();

            return PartialView("_topicAdd", new topicAddModel());
        }

        [HttpPost]
        [Authorize]
        public ActionResult add(topicAddModel md)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpException(500, "异常提交");
            }

            int userID = User.Identity.userID();

            moxiAgentBuyEntities db = new moxiAgentBuyEntities();
            var tp = new Topic();
            tp.userID = userID;
            tp.creatDate = DateTime.Now;
            tp.replys = 0;
            tp.title = md.title;
            tp.body = md.body;
            tp.state = 1;
            tp.node = md.nodeID.Value;
            db.Topic.Add(tp);

            BuyDemand bd = new BuyDemand();
            bd.topicID = tp.ID;
            bd.budget = md.budget.Value;
            db.BuyDemand.Add(bd);


            db.SaveChanges();
            return RedirectToAction(tp.ID.ToString());
        }

        [HttpGet]
        [Authorize]
        public ActionResult Edit(int id)
        {


            int userID = User.Identity.userID();
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();

            //是否有效主题
            var tic = db.Topic.FirstOrDefault(t => t.ID == id && t.state > 0 && t.userID == userID);
            if (tic == null) throw new HttpException(404, "page not found");
            topicEditModel model = new topicEditModel();
            model.ID = tic.ID;
            model.title = tic.title;
            model.body = tic.body;
            model.budget = tic.BuyDemand.budget;
            model.nodeID = tic.node;

            ViewBag.nodesList = NodesList();
            return PartialView("_topicEdit", model);
        }


        [HttpPost]
        [Authorize]
        public ActionResult Edit(topicEditModel md)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpException(500, "异常提交");
            }

            int userID = User.Identity.userID();
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();

            //是否有效主题
            var tic = db.Topic.FirstOrDefault(t => t.ID == md.ID && t.state > 0 && t.userID == userID);
            if (tic == null) throw new HttpException(404, "page not found");

            tic.title = md.title;
            tic.body = md.body;
            tic.node = md.nodeID.Value;
            tic.BuyDemand.budget = md.budget.Value;

            db.SaveChanges();

            string url = Request.ServerVariables["Http_Referer"];
            return Redirect(url);
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
            var tic = db.Topic.FirstOrDefault(t => t.ID == md.topicID && t.state > 0);
            if (tic == null) throw new HttpException(404, "page not found");

            //tic.replys += 1;


            var bs = new BuySolution();
            bs.topicID = md.topicID;
            bs.userID = userID;
            bs.body = md.body;
            bs.goodsLink = md.goodsLink;
            bs.creatDate = DateTime.Now;
            bs.state = 1;

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
            var tic = db.Topic.FirstOrDefault(t => t.ID == md.topicID && t.state > 0);
            if (tic == null) throw new HttpException(404, "page not found");

            tic.replys += 1;


            var reply = new TopicReply();
            reply.topicID = md.topicID;
            reply.userID = userID;
            reply.body = md.body;
            reply.creatDate = DateTime.Now;
            reply.floor = tic.replys;
            reply.state = 1;


            db.TopicReply.Add(reply);


            db.SaveChanges();

            return RedirectToAction("replys", new { ID = md.topicID });


        }

        [Authorize]
        public ActionResult DemandSolutions(int id)
        {
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();
            int userID = User.Identity.userID();
            var topic = db.Topic.First(t => t.ID == id && t.userID == userID);
            DemandSolutionsModel model = new DemandSolutionsModel();
            model.ID = topic.ID;
            model.body = topic.body;
            model.state = topic.state;

            model.moxiSolutions = (from t in topic.BuySolution
                                  where t.CommunityUser.lv == 99 && t.state > 0
                                  orderby t.state descending, t.ID descending
                                  select new SolutionModel
                                  {
                                      ID = t.ID,
                                      state = t.state,
                                      creatDate = t.creatDate,
                                      body = t.body,
                                      goodsLink = t.goodsLink
                                  }).ToList();

            model.userSolutions = (from t in topic.BuySolution
                                   where t.CommunityUser.lv < 99 && t.state > 0
                                   orderby t.state descending, t.ID descending
                                   select new SolutionModel
                                   {
                                       ID = t.ID,
                                       state = t.state,
                                       creatDate = t.creatDate,
                                       body = t.body,
                                       goodsLink = t.goodsLink,
                                       User = new UserModel { ID = t.CommunityUser.ID, Name = t.CommunityUser.Name, avatar = t.CommunityUser.avatar, lv = t.CommunityUser.lv }
                                   }).ToList();

            return PartialView("_DemandSolutions", model);
                                  
        }


        [Authorize]
        public ActionResult mySolutions(int id)
        {
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();
            int userID = User.Identity.userID();

            var topic = db.Topic.First(t => t.ID == id);

            mySolutionsModel model = new mySolutionsModel();
            model.ID = topic.ID;
            model.body = topic.body;
            model.state = topic.state;

            model.mySolutions = (from t in topic.BuySolution
                                 where t.userID == userID
                                 select new SolutionInfoModel
                                {
                                    ID = t.ID,
                                    state = t.state,
                                    creatDate = t.creatDate,
                                    body = t.body,
                                    goodsLink = t.goodsLink,
                                    deleteBody = t.deleteBody
                                }).ToList();

            return PartialView("_mySolutions", model);
                                  
        }

        



        /// <summary>
        /// 用户可用节点
        /// </summary>
        [NonAction]
        private List<SelectListItem> NodesList()
        {
            List<SelectListItem> SLI = new List<SelectListItem>();
            SLI.Add(new SelectListItem { Text = "请选择节点", Value = "" });


            foreach (var Rnode in nodes)
            {
                var group = new SelectListGroup { Name = Rnode.proClassName };
                foreach (var node in Rnode.ThirdJsons)
                {
                    var item = new SelectListItem { Text = node.proClassName, Value = node.proClassID.ToString(), Group = group };
                    SLI.Add(item);
                }
            }

            return SLI;

        }



    }
}