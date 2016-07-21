using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using moxiCommunity.Models;

namespace moxiCommunity.ViewModels
{
    public class userInfoModel
    {

        public userInfoModel(string userName)
        {
            moxiAgentBuyEntities db = new moxiAgentBuyEntities();



            var user = db.CommunityUser.First(t => t.userName == userName);

            this.ID = user.ID;
            this.userName = userName;
            this.Avatar = user.avatar;
            this.userName = user.userName;
            this.Name = user.Name;
            this.creatDate = user.joinDate;



            topicCount = db.Topic.Where(t => t.userID == ID && t.state>0).Count();

            replyCount = db.TopicReply.Where(t => t.userID == ID && t.state > 0).Count();

            solutionCount = db.BuySolution.Where(t => t.userID == ID && t.state > 0).Count();

            

        }


        public int ID { get; set; }

        public string Name { get; set; }

        public string userName { get; set; }

        public string framework { get; set; }

        public string Avatar { get; set; }

        public DateTime creatDate { get; set; }

        public int replyCount { get; set; }

        public int solutionCount { get; set; }

        public int topicCount { get; set; }



    }

    public class userTopicModel
    {
        public int ID { get; set; }

        public string title { get; set; }
        public int state { get; set; }

        public DateTime createDate { get; set; }
        public int replyCount { get; set; }


        public int solutionCount { get; set; }

        public int nodeID { get; set; }
    }

    public class userSolutionModel
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

}