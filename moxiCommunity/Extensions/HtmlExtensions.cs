using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace System.Web.Mvc
{
    public static class HtmlExtensions
    {


        public static HtmlString Pagination(this HtmlHelper helper, int page, int total, int row, string getP = "")
        {

            int lastPage = total / row + 1;

            int prev = page - 1;
            int next = page + 1;


            StringBuilder sb = new StringBuilder();

            sb.Append("<ul class=\"pagination\">");
            if (prev < 1)
                sb.Append("<li class=\"prev previous_page disabled\"><a rel=\"prev\" href=\"#\">上一页</a></li>");
            else
                sb.Append("<li class=\"prev previous_page\"><a rel=\"prev\" href=\"?page=" + prev + getP + "\">上一页</a></li>");


            for (int i = 1; i < 4; i++)
            {
                if (i == page)
                    sb.Append(PaginationLi(i, lastPage, true, getP));
                else
                    sb.Append(PaginationLi(i, lastPage, false, getP));

            }
            if (page > 6)
                sb.Append("<li class=\"disabled\"><a href=\"#\">…</a></li>");

            for (int i = (page - 2) < 4 ? 4 : (page - 2); i < page + 3; i++)
            {

                if (i == page)
                    sb.Append(PaginationLi(i, lastPage, true, getP));
                else
                    sb.Append(PaginationLi(i, lastPage, false, getP));

            }

            if (page + 2 < lastPage - 1)
            {
                sb.Append("<li class=\"disabled\"><a href=\"#\">…</a></li>");


                sb.Append(PaginationLi(lastPage - 1));
                sb.Append(PaginationLi(lastPage));
            }

            if (next > lastPage)
                sb.Append("<li class=\"next next_page disabled\"><a rel=\"next\" href=\"#\">下一页</a></li>");
            else
                sb.Append("<li class=\"next next_page\"><a rel=\"next\" href=\"?page=" + next + getP + "\">下一页</a></li>");


            sb.Append(" </ul>");

            sb.Append("<form action=\"#\" id=\"pagination-from\" class=\"pagination\" method=\"get\">");

            sb.Append(" <ul class=\"pagination fast info\"><li><div>共1页,到第 </div><input value=\"" + page + "\" id=\"fase-input\" /><div>页</div><input type=\"submit\" value=\"确定\" /></li></ul>");
            sb.Append("</form>");

            sb.Append("<script>");
            sb.Append("$('#pagination-from').submit(function () {location.href = '?page=' + $('#fase-input').val()+'" + getP + "'; return false;})");

            sb.Append("</script>");
            return new HtmlString(sb.ToString());
        }

        public static HtmlString sortTopic(this HtmlHelper helper,string url, int page, int total, int row, string catchall,string queryString)
        {
            StringBuilder sb = new StringBuilder();

            if (catchall == null)
                catchall = "time-desc-null";

            var getP = catchall.Split('-');


            string timeSort = "<a class=\"btn btn-default\" href=\"" + url + "/time-desc-" + getP[2] + queryString + "\">时间排序</a>";
            string hotSort = "<a class=\"btn btn-default \" href=\"" + url + "/hot-desc-" + getP[2] + queryString + "\">热度</a>";

            if (getP[0] == "time")
            {
                if(getP[1] == "desc")
                    timeSort = "<a class=\"btn btn-default active\" href=\"" + url + "/time-asc-" + getP[2] + queryString + "\">时间排序<i class=\"glyphicon glyphicon-arrow-down\"></i></a>";
                else
                    timeSort = "<a class=\"btn btn-default active\" href=\"" + url + "/time-desc-" + getP[2] + queryString + "\">时间排序<i class=\"glyphicon glyphicon-arrow-up\"></i></a>";
            }
            else if (getP[0] == "hot")
            {
                if (getP[1] == "desc")
                    hotSort = "<a class=\"btn btn-default active\" href=\"" + url + "/hot-asc-" + getP[2] + queryString + "\">热度<i class=\"glyphicon glyphicon-arrow-down\"></i></a>";
                else
                    hotSort = "<a class=\"btn btn-default active\" href=\"" + url + "/hot-desc-" + getP[2] + queryString + "\">热度<i class=\"glyphicon glyphicon-arrow-up\"></i></a>";
            }


            string adopt = "<a class=\"btn btn-default\" href=\"" + url + "/" + getP[0] + "-" + getP[1] + "-adopt" + queryString + "\">已采纳</a>";
            string have = "<a class=\"btn btn-default\" href=\"" + url + "/" + getP[0] + "-" + getP[1] + "-have" + queryString + "\">已有解决方案</a>";
            if (getP[2] == "have")
                have = "<a class=\"btn btn-default active\" href=\"" + url + "/" + getP[0] + "-" + getP[1] + "-null" + queryString + "\">已有解决方案</a>";
            else if (getP[2] == "adopt")
                adopt = "<a class=\"btn btn-default active\" href=\"" + url + "/" + getP[0] + "-" + getP[1] + "-null" + queryString + "\">已采纳</a>";
            
            sb.Append("<div class=\"panel panel-default topics-sort\">");
            sb.Append(timeSort + hotSort + adopt + have);
            sb.Append("<div class=\"pull-right\">共找到到<span>" + total + "</span>个需求 &#60;<span class=\"moxi-active\">" + page + "</span>/" + (total / row + 1) + "&#62;</div>");
            sb.Append("</div>");

            return new HtmlString(sb.ToString());
        }



        private static string PaginationLi(int pg, bool active = false, string getP = "")
        {
            if (active)
                return "<li class=\"active\"><a href=\"?page=" + pg + getP + "\">" + pg + "</a></li>";
            else
                return "<li><a href=\"?page=" + pg + getP + "\">" + pg + "</a></li>";
        }

        private static string PaginationLi(int pg, int lastPage, bool active = false, string getP = "")
        {
            if (lastPage >= pg)
                return PaginationLi(pg, active, getP);
            return "";
        }




        public static HtmlString nodeSelect(this HtmlHelper helper, int id)
        {
            StringBuilder sb = new StringBuilder();

            var nodes = moxiCommunity.Controllers.TopicController.nodes;



            var selectNode = nodes.FirstOrDefault(t => t.ThirdJsons.FirstOrDefault(s => s.proClassID == id) != null);
            if (selectNode == null)
                return new HtmlString("");//有问题,以后处理

            sb.Append("<div class='node-select'>");

            sb.Append("<select class=\"form-control\" data-val=\"true\" data-val-number=\"\" data-val-range=\"\" data-val-range-max=\"999\" data-val-range-min=\"1\" data-val-required=\"\" name=\"rootNode\">");
            foreach (var n in nodes)
            {
                if (selectNode == n)
                    sb.Append("<option value=\"" + n.proClassID + "\" selected=\"selected\">" + n.proClassName + "</option>");
                else
                    sb.Append("<option value=\"" + n.proClassID + "\">" + n.proClassName + "</option>");
            }
            sb.Append("</select>");



            sb.Append("<select class=\"form-control\" data-val=\"true\" data-val-number=\"字段 分类 必须是一个数字。\" data-val-range=\"非法分类\" data-val-range-max=\"999\" data-val-range-min=\"1\" data-val-required=\"分类 字段是必需的。\" id=\"nodeID\" name=\"nodeID\">");

            sb.Append("<option value=\"0\">请选择</option>");
            foreach (var n in selectNode.ThirdJsons)
            {
                if (n.proClassID == id)
                    sb.Append("<option value=\"" + n.proClassID + "\" selected=\"selected\">" + n.proClassName + "</option>");
                else
                    sb.Append("<option value=\"" + n.proClassID + "\">" + n.proClassName + "</option>");
            }
            sb.Append("</select>");

            //错误提示
            sb.Append(" <span class=\"text-danger field-validation-valid\" data-valmsg-for=\"nodeID\" data-valmsg-replace=\"true\"></span>");
            sb.Append("</div>");
            nodeSelectJS(sb);



            return new HtmlString(sb.ToString());
        }

        public static HtmlString nodeSelect(this HtmlHelper helper)
        {
            StringBuilder sb = new StringBuilder();

            var nodes = moxiCommunity.Controllers.TopicController.nodes;

            sb.Append("<div class='node-select'>");

            sb.Append("<div style='float: left;'>");
            sb.Append("<select class=\"form-control\" data-val=\"true\" data-val-number=\"\" data-val-range=\"\" data-val-range-max=\"999\" data-val-range-min=\"1\" data-val-required=\"\" name=\"rootNode\">");
            sb.Append("<option value=\"0\">请选择</option>");
            foreach (var n in nodes)
            {
                sb.Append("<option value=\"" + n.proClassID + "\">" + n.proClassName + "</option>");
            }
            sb.Append("</select>");

            sb.Append("</div>");

            sb.Append("<div style='float: left;'>");
            sb.Append("<select class=\"form-control\" data-val=\"true\" data-val-number=\"字段 分类 必须是一个数字。\" data-val-range=\"请选择分类\" data-val-range-max=\"999\" data-val-range-min=\"1\" data-val-required=\"分类 字段是必需的。\" id=\"nodeID\" name=\"nodeID\">");
            sb.Append("<option value=\"0\">请选择</option>");
            sb.Append("</select>");

            //错误提示
            sb.Append(" <span class=\"text-danger field-validation-valid\" data-valmsg-for=\"nodeID\" data-valmsg-replace=\"true\"></span>");
            sb.Append("</div>");

            sb.Append("</div>");
            nodeSelectJS(sb);



            return new HtmlString(sb.ToString());
        }

        private static void nodeSelectJS(StringBuilder sb)
        {

            //js
            sb.Append("<script>$(\"select[name='rootNode']\").change(function(){var id=$(this).val();var nodeSel=$(\"select[name='nodeID']\");nodeSel.empty();nodeSel.append(\"<option value='0'>请选择</option>\");if(id<1)return;$.get('/node/'+id,function(data){for(var i=0;i<data.ThirdJsons.length;i++){var node=data.ThirdJsons[i];nodeSel.append(\"<option value='\"+node.proClassID+\"'>\"+node.proClassName+\"</option>\")}})});</script>");
        }




    }


}