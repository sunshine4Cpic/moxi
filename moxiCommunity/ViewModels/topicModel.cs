using moxiCommunity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace moxiCommunity.ViewModels
{
    public class topicAddModel
    {
        [Required]
        [Display(Name = "标题")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "标题必须在4~50个字")]
        public string title { get; set; }

        [Required]
        [Display(Name = "预算")]
        [Range(1, 200000, ErrorMessage = "预算不能超过200000")]
        public int? budget { get; set; }

        [Required]
        [Display(Name = "详细描述")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "详细描述必须在10~500个字")]
        public string body { get; set; }

        [Required]
        [Display(Name = "节点")]
        [Range(1, 999, ErrorMessage = "非法节点")]
        public int? nodeID { get; set; }


       
    }

    public class topicEditModel : topicAddModel
    {
        public int ID { get; set; }
    }

    

    public class topicIndexViewModel
    {
        public List<topicPrevModel> topicList { get; set; }
        public List<proNode> nodes { get; set; }
        public proNode selectNode { get; set; }


    }

    public class topicPrevModel
    {

        public int ID { get; set; }

        public UserModel User { get; set; }

        public int state { get; set; }

        public int? budget { get; set; }

        public DateTime creatDate { get; set; }

        //public string timeago
        //{
        //    get
        //    {
        //        var ts = DateTime.Now.Subtract(creatDate);
        //        if (ts.TotalDays > 1)
        //        {
        //            return ts.Days + " 天前";
        //        }
        //        else if (ts.TotalHours > 1)
        //        {
        //            return ts.Hours + " 小时前";
        //        }
        //        return ts.Minutes + " 分钟前";
        //    }
        //}

        public string title { get; set; }

        public int replyCnt { get; set; }

        private int _solutionCnt;
        public int? solutionCnt { get { return _solutionCnt; } set { _solutionCnt = value == null ? 0 : value.Value; } }

    }

    public class topicViewModel : topicPrevModel
    {


        public string body { get; set; }


        public List<replyViewModel> replies { get; set; }

        public List<SolutionModel> Solutions { get; set; }

        public SolutionModel AdoptSolution { get; set; }
    }

    public class replyViewModel
    {

        public int ID { get; set; }


        public int? state { get; set; }


        public DateTime creatDate { get; set; }


        public string body { get; set; }





        public UserModel User { get; set; }

        public int floor { get; set; }
    }

    public class SolutionModel : replyViewModel
    {
        public string goodsLink { get; set; }

    }

    public class SolutionInfoModel : SolutionModel
    {
        public string deleteBody { get; set; }

    }




    public class UserModel
    {
        public string avatar { get; set; }
        public int ID { get; set; }
        public int lv { get; set; }
        public string Name { get; set; }
    }


    public class BuySolutionAddModel
    {
        [Required]
        public int topicID { get; set; }

        [Required]
        [Display(Name = "商品链接：")]
        [RegularExpression(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", ErrorMessage="必须为超链接")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "标题必须在10~200个字")]
        public string goodsLink { get; set; }

        [Required]
        [Display(Name = "推荐理由：")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "推荐理由必须在10~500个字")]
        public string body { get; set; }
    }

    public class BuySolutionEditModel
    {
        [Required]
        public int ID { get; set; }

        [Required]
        [Display(Name = "商品链接：")]
        [RegularExpression(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", ErrorMessage = "必须为超链接")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "标题必须在10~200个字")]
        public string goodsLink { get; set; }

        [Required]
        [Display(Name = "推荐理由：")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "推荐理由必须在10~500个字")]
        public string body { get; set; }
    }

    public class TopicReplyAddModel
    {
        [Required]
        public int topicID { get; set; }

        [Required]
        [Display(Name = "评论")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "必须在10~500个字之间")]
        public string body { get; set; }
    }


    public class SolutionHandleModel
    {
        [Required]
        public int bsID { get; set; }

        [Required]
        [Display(Name = "原因")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "必须在5~500个字之间")]
        public string body { get; set; }
    }
    
}