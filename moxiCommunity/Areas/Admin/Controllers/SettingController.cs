using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace moxiCommunity.Areas.Admin.Controllers
{
    [Authorize(Roles ="admin")]
    public class SettingController : Controller
    {
        // GET: Admin/Setting
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 提交方法
        /// </summary>
        /// <param name="tm">模型数据</param>
        /// <param name="file">上传的文件对象，此处的参数名称要与View中的上传标签名称相同</param>
        /// <returns></returns>
        [HttpPost]
        public string UploadNode()
        {
            HttpPostedFileBase FileData = Request.Files[0];
            if (FileData.ContentLength > 0)
            {
                var fileName = Path.GetFileName(FileData.FileName);
                var path = Server.MapPath("~/App_Data/node.json");
                FileData.SaveAs(path);
            }
            return "Files was uploaded successfully!";
        }
    }
}