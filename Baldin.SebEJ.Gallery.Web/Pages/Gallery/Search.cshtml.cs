using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baldin.SebEJ.Gallery.Data.Models;
using Baldin.SebEJ.Gallery.Search;
using Baldin.SebEJ.Gallery.Search.Models;
using Baldin.SebEJ.Gallery.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Baldin.SebEJ.Gallery.Web.Pages.Gallery
{
    public class SearchModel : PageModel
    {
        private ISearch _search;
        private UserManager<IdentityUser> _userManager;

        public SearchModel(ISearch search, UserManager<IdentityUser> userManager)
        {
            _search = search;
            _userManager = userManager;
        }

        public IEnumerable<User_Picture> Photos { get; set; }

        public async Task OnGet(string query = null)
        {
            var list = new List<User_Picture>();
            if (query != null)
            {
                var res = await _search.SearchPhotosAsync(query);
                foreach (var item in res)
                {
                    var userData = await _userManager.FindByEmailAsync(item.User.Email);
                    list.Add(new User_Picture
                    {
                        Id = item.PhotoId,
                        Rating = item.Data.Rating,
                        Thumbnail_Url = item.Data.Thumbnail_Url,
                        Url = item.Data.Url,
                        Author = userData.Id,
                        Votes = item.Data.Votes,
                        IsVoted = true
                    });
                }
            }
            Photos = list;
        }
    }
}