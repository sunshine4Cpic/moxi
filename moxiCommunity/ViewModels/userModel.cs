using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using moxiCommunity.Models;

namespace moxiCommunity.ViewModels
{
    public class userInfoModel
    {

        public int ID { get; set; }

        public string Name { get; set; }

        public string userName { get; set; }


        public string Avatar { get; set; }

        public DateTime creatDate { get; set; }

        //暂时不统计
        //public int replyCount { get; set; }

        //public int solutionCount { get; set; }

        //public int topicCount { get; set; }


        public List<userDemandsModel> Demands { get; set; }

    }

    public class userDemandsModel
    {
        public int ID { get; set; }

        public string title { get; set; }
        public int state { get; set; }


        public DateTime createDate { get; set; }
        public int replyCount { get; set; }
        public int solutionCount { get; set; }

        public int budget { get; set; }

        public int nodeID { get; set; }
    }

    public class userSolutionsModel
    {
        public int ID { get; set; }
        public int topicID { get; set; }
        public string topicTitle { get; set; }
        public int state { get; set; }

        public DateTime createDate { get; set; }

        public string body { get; set; }

        public string goodsLink { get; set; }

    }

    public class userReplyModel
    {
        public int ID { get; set; }

        public int topicID { get; set; }

        public string topicTitle { get; set; }
        public string body { get; set; }
        public int state { get; set; }

        public DateTime createDate { get; set; }

    }

    public class DemandSolutionsModel
    {
        public int ID { get; set; }
        public string body { get; set; }

        public int state { get; set; }

        public List<SolutionModel> userSolutions { get; set; }

        public List<SolutionModel> moxiSolutions { get; set; }
    }

    public class mySolutionsModel
    {
        public int ID { get; set; }
        public string body { get; set; }

        public int state { get; set; }

        public List<SolutionInfoModel> mySolutions { get; set; }

    }

}