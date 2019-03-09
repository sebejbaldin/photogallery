using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baldin.SebEJ.Gallery.Caching;
using Baldin.SebEJ.Gallery.Data;
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
        private IDataAccess _dataAccess;
        private ISearch _search;
        private ICaching _caching;

        public SearchModel(IDataAccess dataAccess, ISearch search, ICaching caching)
        {
            _dataAccess = dataAccess;
            _search = search;
            _caching = caching;
        }

        public PaginatedList<User_Picture> Photos { get; set; }

        public async Task OnGet(int index = 1, string query = null)
        {
            IEnumerable<User_Picture> list = new User_Picture[0];
            PaginatedPhotos result = null;
            if (query != null)
            {
                result = await _search.PaginatedSearchAsync(query);
                IEnumerable<int> userPics = new int[0];

                if (result != null && result.Photos.Count() > 0)
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        var userId = User.FindFirst("userId");
                        userPics = await _caching.GetVotesByUserIdAsync(userId.Value);
                        if (userPics == null || userPics.Count() == 0)
                        {
                            var picsVoted = await _dataAccess.GetVotesByUserIdAsync(userId.Value);
                            _caching.InsertVotesAsync(picsVoted);
                            userPics = picsVoted.Select(x => x.Picture_Id);
                        }
                        list = result.Photos.Select(item => new User_Picture
                        {
                            Id = item.PhotoId,
                            Rating = item.Data.Rating,
                            Thumbnail_Url = item.Data.Thumbnail_Url,
                            Url = item.Data.Url,
                            Author = item.User.UserId,
                            Votes = item.Data.Votes,
                            IsVoted = userId.Value == item.User.UserId || userPics.Any(vote => vote == item.PhotoId)
                        });
                    }
                    else
                    {
                        list = result.Photos.Select(item => new User_Picture
                        {
                            Id = item.PhotoId,
                            Rating = item.Data.Rating,
                            Thumbnail_Url = item.Data.Thumbnail_Url,
                            Url = item.Data.Url,
                            Author = item.User.UserId,
                            Votes = item.Data.Votes,
                            IsVoted = true
                        });
                    }
                }
            }
            Photos = (result != null) ? new PaginatedList<User_Picture>(list, (int)result.TotalResults, index, 6) : null;
        }
    }
}