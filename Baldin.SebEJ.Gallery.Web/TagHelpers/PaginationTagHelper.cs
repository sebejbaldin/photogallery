using Baldin.SebEJ.Gallery.Web.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baldin.SebEJ.Gallery.Web.TagHelpers
{
    [HtmlTargetElement("paging", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class PaginationTagHelper : TagHelper
    {
        public string Url { get; set; }
        public PaginatedList<User_Picture> Pictures { get; set; }
        public IDictionary<string, string> QueryStrings { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!Url.EndsWith("/"))
            {
                Url += "/";
            }
            string queryes = "";
            StringBuilder builder = new StringBuilder();

            if (QueryStrings != null && QueryStrings.Count > 0)
            {
                builder.Append("?");
                foreach (var item in QueryStrings)
                {
                    builder.Append($"{item.Key}={item.Value}&");
                }
                builder.Remove(builder.Length - 1, 1);
                queryes = builder.ToString();
            }
            builder.Clear();
            builder.Append("<nav aria-label=\"gallery pagination\">");
            builder.Append("<ul class=\"pagination justify-content-center\">");
            builder.Append($"<li id=\"prevPage\" class=\"page-item {(Pictures.HasPreviousPage ? "" : "disabled")}\">");
            builder.Append($"<a class=\"page-link\" href=\"{Url}{Pictures.PageIndex - 1}{queryes}\" aria-label=\"Previous\">");
            builder.Append("<span aria-hidden=\"true\">&laquo;</span><span class=\"sr-only\">Previous</span></a></li>");

            foreach (var index in Pictures.GetRoutePages())
            {
                builder.Append($"<li class=\"page-item {(Pictures.PageIndex == index ? "active" : "")}\"><a class=\"page-link\" href=\"{Url}{index}{queryes}\">{index}</a></li>");
            }

            builder.Append($"<li id=\"nexPage\" class=\"page-item {(Pictures.HasNextPage ? "" : "disabled")}\">");
            builder.Append($"<a class=\"page-link\" href=\"{Url}{Pictures.PageIndex + 1}{queryes}\" aria-label=\"Next\">");
            builder.Append("<span aria-hidden=\"true\">&raquo;</span>");
            builder.Append("<span class=\"sr-only\">Next</span>");
            builder.Append("</a></li></ul></nav>");
            output.TagName = "section";
            output.Attributes.SetAttribute("id", "pagination");
            output.Content.SetHtmlContent(builder.ToString());
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}
