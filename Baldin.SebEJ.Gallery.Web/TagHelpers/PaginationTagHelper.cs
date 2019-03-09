﻿using Baldin.SebEJ.Gallery.Web.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baldin.SebEJ.Gallery.Web.TagHelpers
{
    public class PaginationTagHelper : TagHelper
    {
        public PaginatedList<User_Picture> Pictures { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<nav aria-label=\"gallery pagination\">");
            builder.Append("<ul class=\"pagination justify-content-center\">");
            builder.Append($"<li id=\"prevPage\" class=\"page-item {(Pictures.HasPreviousPage ? "" : "disabled")})\">");
            builder.Append($"<a class=\"page-link\" href=\"/Gallery/Index/{Pictures.PageIndex - 1}\" aria-label=\"Previous\">");
            builder.Append("<span aria-hidden=\"true\">&laquo;</span><span class=\"sr-only\">Previous</span></a></li>");

            foreach (var index in Pictures.GetRoutePages())
            {
                builder.Append($"<li class=\"page-item {(Pictures.PageIndex == index ? "active" : "")}\"><a class=\"page-link\" href=\"/Gallery/Index/{index}\">{index}</a></li>");
            }

            builder.Append($"<li id=\"nexPage\" class=\"page-item {(Pictures.HasNextPage ? "" : "disabled")}\">");
            builder.Append($"<a class=\"page-link\" href=\"/Gallery/Index/{Pictures.PageIndex + 1}\" aria-label=\"Next\">");
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
