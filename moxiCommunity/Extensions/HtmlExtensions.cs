using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace System.Web.Mvc
{
    public static class HtmlExtensions
    {


        public static HtmlString Pagination(this HtmlHelper helper, int page, int total, int rows,string getP="")
        {

            int lastPage = total / rows + 1;

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



            return new HtmlString(sb.ToString());
        }


        private static string PaginationLi(int pg, bool active = false,string getP = "")
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




        public static HtmlString nodeSelect(this HtmlHelper helper,int id)
        {
            StringBuilder sb = new StringBuilder();

            var nodes = moxiCommunity.Controllers.TopicController.nodes;



            var selectNode = nodes.FirstOrDefault(t => t.ThirdJsons.FirstOrDefault(s => s.proClassID == id) != null);
            if (selectNode==null)
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



            sb.Append("<select class=\"form-control\" data-val=\"true\" data-val-number=\"字段 节点 必须是一个数字。\" data-val-range=\"非法节点\" data-val-range-max=\"999\" data-val-range-min=\"1\" data-val-required=\"节点 字段是必需的。\" id=\"nodeID\" name=\"nodeID\">");

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

            sb.Append("<select class=\"form-control\" data-val=\"true\" data-val-number=\"\" data-val-range=\"\" data-val-range-max=\"999\" data-val-range-min=\"1\" data-val-required=\"\" name=\"rootNode\">");
            sb.Append("<option value=\"0\">请选择</option>");
            foreach (var n in nodes)
            {
                    sb.Append("<option value=\"" + n.proClassID + "\">" + n.proClassName + "</option>");
            }
            sb.Append("</select>");

            sb.Append("<select class=\"form-control\" data-val=\"true\" data-val-number=\"字段 节点 必须是一个数字。\" data-val-range=\"请选择节点\" data-val-range-max=\"999\" data-val-range-min=\"1\" data-val-required=\"节点 字段是必需的。\" id=\"nodeID\" name=\"nodeID\">");
            sb.Append("<option value=\"0\">请选择</option>");
            sb.Append("</select>");

            //错误提示
            sb.Append(" <span class=\"text-danger field-validation-valid\" data-valmsg-for=\"nodeID\" data-valmsg-replace=\"true\"></span>");
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