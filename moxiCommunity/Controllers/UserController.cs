using Microsoft.Owin.Security;
using moxiCommunity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using moxiCommunity.Models;

namespace moxiCommunity.Controllers
{
    public class UserController : Controller
    {

        [AllowAnonymous]
        public ActionResult Login(string ReturnUrl)
        {
            ViewBag.returnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]//对应@Html.AntiForgeryToken()
        public ActionResult Login(LoginViewModel model, string ReturnUrl)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (loginLocal(model.UserName, model.Password))
            {
                return Redirect("/");
            }



            if (!moxiLogin(model.UserName, model.Password))
            {
                ModelState.AddModelError("", "用户名或密码错误。");
                return View(model);
            }
            //跳转到请求页面
            if (Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }
            return Redirect("/");
        }

        public ActionResult Login302()
        {
            var logincookie = HttpContext.Request.Cookies["logincookie"];

            try
            {
                var userstr = encode.AESDecrypt(logincookie.Value, "moxi");

                var lg = JsonConvert.DeserializeObject<LoginCookieViewModel>(userstr);

                moxiRegister(lg.username, lg.name, lg.id, lg.overtime);
            }
            catch (Exception)
            {
                //出错处理
            }
            

            return Redirect("/");
        }
        


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            HttpContext.GetOwinContext().Authentication.SignOut("ApplicationCookie");
            return Redirect("/");
        }



        //[Authorize]
        //public ActionResult userInfo(int page = 1)
        //{
        //    string userName = User.Identity.userName();
        //    userInfoModel model = new userInfoModel(userName);

        //    moxiAgentBuyEntities db = new moxiAgentBuyEntities();
        //    int userID = User.Identity.userID();
        //    var tps = from t in db.Topic
        //              where t.userID == userID
        //              orderby t.ID descending
        //              select new userTopicModel
        //              {
        //                  ID = t.ID,
        //                  title = t.title,
        //                  replyCount = t.replys,
        //                  solutionCount = t.BuySolution.Count,
        //                  state = t.state,
        //                  nodeID = t.node,
        //                  createDate = t.creatDate
        //              };
        //    ViewBag.action = "Topics";
        //    ViewBag.panelList = tps.Skip(20 * (page - 1)).Take(20).ToList();

        //    return View(model);
        //}


        [Authorize]
        public ActionResult Demands(string id = "all", int page = 1, int row = 200)
        {
            int userID = User.Identity.userID();


            moxiAgentBuyEntities db = new moxiAgentBuyEntities();

            var userInfo = (from t in db.CommunityUser
                            where t.ID == userID
                            select new userInfoModel
                            {
                                ID = t.ID,
                                Name = t.Name,
                                Avatar = t.avatar,
                                creatDate = t.joinDate,
                                userName = t.userName
                            }).First();

            IQueryable<Topic> baseTopics = db.Topic;
            switch (id)
            {
                case "all":

                    break;
                case "have":
                    baseTopics = baseTopics.Where(t => t.BuySolution.Count > 0);
                    break;
                case "noAdopt":
                    baseTopics = baseTopics.Where(t => t.state != 2);
                    break;
                case "Adopt":
                    baseTopics = baseTopics.Where(t => t.state == 2);
                    break;
                default:
                    break;
            }




            var tps = from t in baseTopics
                      where t.userID == userID
                      orderby t.ID descending
                      select new userDemandsModel
                      {
                          ID = t.ID,
                          title = t.title,
                          budget = t.BuyDemand.budget,
                          nodeID = t.node,
                          state = t.state,
                          solutionCount = t.BuySolution.Count(s => s.state > 0),
                          createDate = t.creatDate
                      };
            userInfo.Demands = tps.Skip(row * (page - 1)).Take(row).ToList();

            ViewBag.action = "Demands";
            ViewBag.id = id;


            return View("userInfo", userInfo);
        }


        [Authorize]
        public ActionResult Solutions(int page = 1, int row = 200)
        {
            int userID = User.Identity.userID();


            moxiAgentBuyEntities db = new moxiAgentBuyEntities();

            var userInfo = (from t in db.CommunityUser
                            where t.ID == userID
                            select new userInfoModel
                            {
                                ID = t.ID,
                                Name = t.Name,
                                Avatar = t.avatar,
                                creatDate = t.joinDate,
                                userName = t.userName
                            }).First();

            var tps = (from t in db.BuySolution
                       where t.userID == userID
                       select new userDemandsModel
                       {
                           ID = t.Topic.ID,
                           title = t.Topic.title,
                           budget = t.Topic.BuyDemand.budget,
                           nodeID = t.Topic.node,
                           state = t.Topic.state,
                           solutionCount = t.Topic.BuySolution.Count(s => s.state > 0),
                           createDate = t.Topic.creatDate
                       }).Distinct().OrderByDescending(t => t.ID);
            userInfo.Demands = tps.Skip(row * (page - 1)).Take(row).ToList();

            ViewBag.action = "Solutions";

            return View("userInfo", userInfo);
        }



        [NonAction]
        private bool moxiLogin(string userName, string password)
        {
            HttpClient _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://115.159.79.25:8060/");

            var parameters = new Dictionary<string, string>();
            parameters.Add("loginName", userName);
            parameters.Add("pwd", password);
            parameters.Add("clientType", "3");

            var response = _httpClient.PostAsync("api/Login/Login", new FormUrlEncodedContent(parameters)).Result;
            var responseValue = response.Content.ReadAsStringAsync().Result;

            var user = JsonConvert.DeserializeObject<loginModel>(responseValue);


            if (!user.IsSuccess)
            {
                return false;
            }

            //注册本地用户
            var moxiUser = user.ReturnObjects.result;
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();
            var localUser = db.CommunityUser.FirstOrDefault(t => t.moxiID == moxiUser.UserID);
            if (localUser == null)
            {
                CommunityUser u = new CommunityUser();
                u.Name = moxiUser.UserName;
                u.moxiID = moxiUser.UserID;
                u.UserLoginToken = moxiUser.UserLoginToken;
                u.joinDate = DateTime.Now;
                u.userName = moxiUser.UserEmail;//email作为帐号;
                u.avatar = "/Content/img/auto.jpg";
                Random random = new Random();
                u.password = random.Next(10000001, 99999999).ToString();//随机密码
                db.CommunityUser.Add(u);

                localUser = u;

            }
            db.SaveChanges();


            ClaimsIdentity _identity = new ClaimsIdentity("ApplicationCookie");
            _identity.AddClaim(new Claim(ClaimTypes.Name, user.ReturnObjects.result.UserName));
            _identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, localUser.ID.ToString()));
            _identity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity"));

            _identity.AddClaim(new Claim("userName", localUser.userName));

            _identity.AddClaim(new Claim(ClaimTypes.Role, "user"));


            var auth = new AuthenticationProperties() { IssuedUtc = DateTime.UtcNow, ExpiresUtc = DateTime.UtcNow.AddDays(30) };



            HttpContext.GetOwinContext().Authentication.SignOut("ApplicationCookie");
            HttpContext.GetOwinContext().Authentication.SignIn(auth, _identity);

            return true;

        }


        private bool moxiRegister(string username, string name,int id,DateTime overtime)
        {
            

            //注册本地用户
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();
            var localUser = db.CommunityUser.FirstOrDefault(t => t.moxiID == id);
            if (localUser == null)
            {
                CommunityUser u = new CommunityUser();
                u.Name = name;
                u.moxiID = id;
                //u.UserLoginToken = moxiUser.UserLoginToken;
                u.joinDate = DateTime.Now;
                u.userName = username;//email作为帐号;
                u.avatar = "/Content/img/auto.jpg";
                Random random = new Random();
                u.password = random.Next(10000001, 99999999).ToString();//随机密码
                db.CommunityUser.Add(u);
                db.SaveChanges();
                localUser = u;
                
            }
            


            ClaimsIdentity _identity = new ClaimsIdentity("ApplicationCookie");
            _identity.AddClaim(new Claim(ClaimTypes.Name, name));
            _identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, localUser.ID.ToString()));
            _identity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity"));

            _identity.AddClaim(new Claim("userName", username));

            _identity.AddClaim(new Claim(ClaimTypes.Role, "user"));

            
            var auth = new AuthenticationProperties() { IssuedUtc = DateTime.UtcNow, ExpiresUtc = overtime, IsPersistent=true };



            HttpContext.GetOwinContext().Authentication.SignOut("ApplicationCookie");
            HttpContext.GetOwinContext().Authentication.SignIn(auth, _identity);

            return true;

        }
        

        [NonAction]
        private bool loginLocal(string userName, string password)
        {
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();
            var localUser = db.CommunityUser.FirstOrDefault(t => t.userName == userName && t.password == password && t.lv == 99);//暂时只支持admin用户登录
            if (localUser != null)
            {
                ClaimsIdentity _identity = new ClaimsIdentity("ApplicationCookie");
                _identity.AddClaim(new Claim(ClaimTypes.Name, localUser.Name));
                _identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, localUser.ID.ToString()));
                _identity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity"));

                _identity.AddClaim(new Claim("userName", localUser.userName));

                _identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
                _identity.AddClaim(new Claim(ClaimTypes.Role, "admin"));


                var auth = new AuthenticationProperties() { IssuedUtc = DateTime.UtcNow, ExpiresUtc = DateTime.UtcNow.AddDays(30) };



                HttpContext.GetOwinContext().Authentication.SignOut("ApplicationCookie");
                HttpContext.GetOwinContext().Authentication.SignIn(auth, _identity);
                return true;

            }
            else
            {
                return false;
            }

        }


    }


}