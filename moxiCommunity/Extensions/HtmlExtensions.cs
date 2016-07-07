using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace System.Web.Mvc
{
    public static class HtmlExtensions
    {
        public static HtmlString Pagination(this HtmlHelper helper, int page)
        {

            int prev = page > 1 ? page - 1 : 1;
            int next = page + 1;



            StringBuilder sb = new StringBuilder();

            sb.Append("<div class=\"clearfix\"><ul class=\"pagination\">");

            sb.Append("<li class=\"prev previous_page\"><a rel=\"prev\" href=\"?page=" + prev + "\">← 上一页</a></li>");


            for (int i = 1; i < 4; i++)
            {
                if (i == page)
                    sb.Append(PaginationLi(i, true));
                else
                    sb.Append(PaginationLi(i));

            }
            if (page > 6)
                sb.Append("<li class=\"disabled\"><a href=\"#\">…</a></li>");

            for (int i = (page - 2) < 4 ? 4 : (page - 2); i < page + 3; i++)
            {

                if (i == page)
                    sb.Append(PaginationLi(i, true));
                else
                    sb.Append(PaginationLi(i));

            }
            sb.Append("<li class=\"disabled\"><a href=\"#\">…</a></li>");


            sb.Append("<li><a href=\"?page=199\">199</a></li>");
            sb.Append("<li><a href=\"?page=200\">200</a></li>");


            sb.Append("<li class=\"next next_page\"><a rel=\"next\" href=\"?page=" + next + "\">下一页 →</a></li>");

            sb.Append(" </ul></div>");

            return new HtmlString(sb.ToString());
        }


        public static HtmlString Pagination(this HtmlHelper helper, int page, int total, int rows)
        {

            int lastPage = total / rows + 1;

            int prev = page - 1;
            int next = page + 1;
          
            
            StringBuilder sb = new StringBuilder();

            sb.Append("<div class=\"clearfix\"><ul class=\"pagination\">");
            if (prev<1)
                sb.Append("<li class=\"prev previous_page disabled\"><a rel=\"prev\" href=\"#\">← 上一页</a></li>");
            else
                sb.Append("<li class=\"prev previous_page\"><a rel=\"prev\" href=\"?page=" + prev + "\">← 上一页</a></li>");


            for (int i = 1; i < 4; i++)
            {
                if (i == page)
                    sb.Append(PaginationLi(i, lastPage, true));
                else
                    sb.Append(PaginationLi(i, lastPage));

            }
            if (page > 6)
                sb.Append("<li class=\"disabled\"><a href=\"#\">…</a></li>");

            for (int i = (page - 2) < 4 ? 4 : (page - 2); i < page + 3; i++)
            {

                if (i == page)
                    sb.Append(PaginationLi(i, lastPage, true));
                else
                    sb.Append(PaginationLi(i, lastPage));

            }

            if (page + 2 < lastPage-1)
            {
                sb.Append("<li class=\"disabled\"><a href=\"#\">…</a></li>");


                sb.Append(PaginationLi(lastPage - 1));
                sb.Append(PaginationLi(lastPage));
            }

            if (next > lastPage)
                sb.Append("<li class=\"next next_page disabled\"><a rel=\"next\" href=\"#\">下一页 →</a></li>");
            else
                sb.Append("<li class=\"next next_page\"><a rel=\"next\" href=\"?page=" + next + "\">下一页 →</a></li>");


            sb.Append(" </ul></div>");

            return new HtmlString(sb.ToString());
        }


        private static string PaginationLi(int pg, bool active = false)
        {
            if (active)
                return "<li class=\"active\"><a href=\"?page=" + pg + "\">" + pg + "</a></li>";
            else
                return "<li><a href=\"?page=" + pg + "\">" + pg + "</a></li>";
        }

        private static string PaginationLi(int pg, int lastPage, bool active = false)
        {
            if (lastPage >= pg)
                return PaginationLi(pg, active);
            return "";
        }



        public static HtmlString expirationDate(this HtmlHelper helper, DateTime? startDate, DateTime? endDate)
        {
            DateTime now = DateTime.Now;

            StringBuilder sb = new StringBuilder();

            if (now < startDate)
            {
                sb.Append("<div class=\"project-expiration-wrap\">");
                sb.Append("<span class=\"glyphicon glyphicon-time\"></span> <span> 任务开始时间：" + startDate.Value.ToString("yyyy-MM-dd h:mm") + "</span>");
                sb.Append("</div>");
                return new HtmlString(sb.ToString());
            }


            if (endDate == null)//没有截至时间
            {
                 sb.Append("<div class=\"project-expiration-wrap\">");
                sb.Append("<span class=\"glyphicon glyphicon-time\"></span> <span> 任务进行中</span>");
                sb.Append("</div>");
                return new HtmlString(sb.ToString());
            }

            if (endDate < now)
            {
                sb.Append("<div class=\"project-expiration-wrap\">");
                sb.Append("<span class=\"glyphicon glyphicon-lock high\"></span> <span> 任务已关闭</span>");
                sb.Append("</div>");
                return new HtmlString(sb.ToString());
            }

            var ts =  endDate.Value.Subtract(now);

            

            sb.Append("<div class=\"project-expiration-wrap\">");
            sb.Append("<span class=\"glyphicon glyphicon-time\"></span> <span> 任务时间仅剩：</span>");
            sb.Append("</div>");
            sb.Append("<div class=\"project-expiration-count-down clearfix\">");
            sb.Append("<div class=\"show-time-wrap\">");

            int day = ts.Days;

            if (day < 10)//加0
                sb.Append("<span class=\"show-time-number\" id=\"left_day_one\">0</span>");


            foreach (var s in day.ToString())
            {
                sb.Append("<span class=\"show-time-number\" id=\"left_day_one\">" + s + "</span>");
            }
            
          
            sb.Append("<span class=\"show-time-separate\">天</span>");
            sb.Append("</div>");
            sb.Append("<div class=\"show-time-wrap\">");

            var hour = ts.Hours;

            sb.Append("<span class=\"show-time-number\" id=\"left_hour_ten\">" + hour / 10 + "</span>");
            sb.Append("<span class=\"show-time-number\" id=\"left_hour_ten\">" + hour % 10 + "</span>");
            sb.Append("<span class=\"show-time-separate\">小时</span>");
            sb.Append("</div>");
            sb.Append("</div>");

           

            return new HtmlString(sb.ToString());
        }
        
    }

    

    
   
}