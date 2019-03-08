using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baldin.SebEJ.Gallery.Caching;
using Baldin.SebEJ.Gallery.Data;
using Baldin.SebEJ.Gallery.Search;
using Baldin.SebEJ.Gallery.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Baldin.SebEJ.Gallery.Web.Controllers
{
    [Route("api/v1/search")]
    [ApiController]
    public class SearchV1Controller : ControllerBase
    {
        private IDataAccess _dataAccess;
        private UserManager<IdentityUser> _userManager;
        private ICaching _caching;
        private ISearch _search;

        public SearchV1Controller(IDataAccess dataAccess, UserManager<IdentityUser> userManager, ICaching caching, ISearch search)
        {
            _dataAccess = dataAccess;
            _userManager = userManager;
            _caching = caching;
            _search = search;
        }

        [HttpGet("{index}")]
        public async Task<IActionResult> PaginatedSearch(int index, string query)
        {
            var result = await _search.SearchPhotosAsync(query, index);
            IEnumerable<User_Picture> tosend = null;
            IEnumerable<int> userPics = new int[0];

            if (result != null && result.Count() > 0)
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
                    tosend = result.Select(item => new User_Picture
                    {
                        Id = item.PhotoId,
                        Rating = item.Data.Rating,
                        Thumbnail_Url = item.Data.Thumbnail_Url,
                        Url = item.Data.Url,
                        Author = item.User.UserId,
                        Votes = item.Data.Votes,
                        IsVoted = userPics.Any(vote => vote == item.PhotoId)
                    });
                }
                else
                {
                    tosend = result.Select(item => new User_Picture
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
            var picCount = await _dataAccess.GetPictureCountAsync();
            return Ok(new
            {
                pageCount = (int)Math.Ceiling(picCount / 6D),
                photos = tosend
            });
        }
    }
}