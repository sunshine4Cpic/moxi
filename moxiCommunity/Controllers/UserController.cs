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
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace moxiCommunity.Controllers
{
    public class UserController : Controller
    {
        
        [AllowAnonymous]
        public ActionResult Login(string ReturnUrl)
        {
            //Request.IsAjaxRequest();
            //headh中 X-Requested-With:XMLHttpRequest
            //ajax 调用时反悔错误信息(coding)
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

            if(loginLocal(model.UserName,model.Password))
            {
                return Redirect("/");
            }
            

          
            if (!moxiLogin(model.UserName, model.Password))
            {
                ModelState.AddModelError("", "用户名或密码错误。");
                return View(model);
            }
            //跳转到请求页面
            //if (Url.IsLocalUrl(ReturnUrl))
            //{
            //    return Redirect(ReturnUrl);
            //}
            return Redirect("/");



        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            HttpContext.GetOwinContext().Authentication.SignOut("ApplicationCookie");
            return Redirect("/");
        }



        [Authorize]
        public ActionResult userInfo(int page = 1)
        {
            string userName = User.Identity.userName();
            userInfoModel model = new userInfoModel(userName);

            moxiAgentBuyEntities db = new moxiAgentBuyEntities();
            int userID = User.Identity.userID();
            var tps = from t in db.Topic
                      where t.userID == userID
                      orderby t.ID descending
                      select new userTopicModel
                      {
                          ID = t.ID,
                          title = t.title,
                          replyCount = t.replys,
                          solutionCount = t.BuySolution.Count,
                          state = t.state,
                          nodeID = t.node,
                          createDate = t.creatDate
                      };
            ViewBag.action = "Topics";
            ViewBag.panelList = tps.Skip(20 * (page - 1)).Take(20).ToList();

            return View(model);
        }

     
        [Authorize]
        public ActionResult Solutions(int page = 1)
        {
            string userName = User.Identity.userName();
            userInfoModel model = new userInfoModel(userName);

            moxiAgentBuyEntities db = new moxiAgentBuyEntities();
            int userID = User.Identity.userID();
            var tps = from t in db.BuySolution
                      where t.userID == userID
                      orderby t.ID descending
                      select new userSolutionModel
                      {
                          ID = t.ID,
                          topicID = t.topicID,
                          topicTitle = t.Topic.title,
                          body = t.body,
                          state = t.state,
                          createDate = t.creatDate,
                          goodsLink = t.goodsLink
                      };
            ViewBag.action = "Solutions";
            ViewBag.panelList = tps.Skip(20 * (page - 1)).Take(20).ToList();

            return View("userInfo",model);
        }
        [Authorize]
        public ActionResult Replys(int page =1 )
        {
            string userName = User.Identity.userName();
            userInfoModel model = new userInfoModel(userName);

            moxiAgentBuyEntities db = new moxiAgentBuyEntities();
            int userID = User.Identity.userID();
            var tps = from t in db.TopicReply
                      where t.userID == userID
                      orderby t.ID descending
                      select new userReplyModel
                      {
                          ID = t.ID,
                          topicID = t.topicID,
                          topicTitle = t.Topic.title,
                          body = t.body,
                          state = t.state,
                          createDate = t.creatDate
                      };
            ViewBag.action = "Replys";
            ViewBag.panelList = tps.Skip(20 * (page - 1)).Take(20).ToList();

            return View("userInfo", model);
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

            var user =  JsonConvert.DeserializeObject<loginModel>(responseValue);


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


      
        [NonAction]
        private bool loginLocal(string userName, string password)
        {
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();
            var localUser = db.CommunityUser.FirstOrDefault(t => t.userName == userName && t.password == password && t.lv==99);//暂时只支持admin用户登录
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