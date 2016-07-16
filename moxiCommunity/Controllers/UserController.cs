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

            var user = moxiLogin(model.UserName, model.Password);
            if (!user.IsSuccess)
            {
                ModelState.AddModelError("", "用户名或密码错误。");
                return View(model);
            }

            //注册本地用户
            var moxiUser = user.ReturnObjects.result;
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();
            var localUser = db.CommunityUser.FirstOrDefault(t => t.ID == moxiUser.UserID);
            if (localUser == null)
            {
                CommunityUser u = new CommunityUser();
                u.Name = moxiUser.UserName;
                u.ID = moxiUser.UserID;
                u.UserLoginToken = moxiUser.UserLoginToken;
                u.joinDate = DateTime.Now;
                db.CommunityUser.Add(u);

            }
            db.SaveChanges();


            ClaimsIdentity _identity = new ClaimsIdentity("ApplicationCookie");
            _identity.AddClaim(new Claim(ClaimTypes.Name, user.ReturnObjects.result.UserName));
            _identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.ReturnObjects.result.UserID.ToString()));
            _identity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity"));

            _identity.AddClaim(new Claim("userName", model.UserName));

            _identity.AddClaim(new Claim(ClaimTypes.Role, "user"));


            var auth = new AuthenticationProperties() { IssuedUtc = DateTime.UtcNow, ExpiresUtc = DateTime.UtcNow.AddDays(30) };



            HttpContext.GetOwinContext().Authentication.SignOut("ApplicationCookie");
            HttpContext.GetOwinContext().Authentication.SignIn(auth, _identity);

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

        [NonAction]
        private loginModel moxiLogin(string userName, string password)
        {
            HttpClient _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://115.159.79.25:8060/");

            var parameters = new Dictionary<string, string>();
            parameters.Add("loginName", userName);
            parameters.Add("pwd", password);
            parameters.Add("clientType", "3");

            var response = _httpClient.PostAsync("api/Login/Login", new FormUrlEncodedContent(parameters)).Result;
            var responseValue = response.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<loginModel>(responseValue);


        }


    }
}