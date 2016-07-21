using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace System.Web.Mvc
{
    public static class HtmlExtensions
    {


        public static HtmlString Pagination(this HtmlHelper helper, int page, int total, int rows)
        {

            int lastPage = total / rows + 1;

            int prev = page - 1;
            int next = page + 1;


            StringBuilder sb = new StringBuilder();

            sb.Append("<div class=\"clearfix\"><ul class=\"pagination\">");
            if (prev < 1)
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

            if (page + 2 < lastPage - 1)
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






    }
    
   
}