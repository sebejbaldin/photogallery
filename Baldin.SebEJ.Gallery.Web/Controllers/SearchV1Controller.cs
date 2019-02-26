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

        [HttpGet]
        public async Task<IEnumerable<User_Picture>> Search(string query)
        {
            var result = await _search.SearchPhotosAsync(query);
            var tosend = new List<User_Picture>();
            IEnumerable<int> userPics = new int[0];

            if (result != null && result.Count() > 0)
            {

                if (User.Identity.IsAuthenticated)
                {
                    var user = await _userManager.FindByNameAsync(User.Identity.Name);
                    userPics = await _caching.GetVotesByUserIdAsync(user.Id);
                    if (userPics == null || userPics.Count() == 0)
                    {
                        var picsVoted = await _dataAccess.GetVotesByUserIdAsync(user.Id);
                        _caching.InsertVotesAsync(picsVoted);
                        userPics = picsVoted.Select(x => x.Picture_Id);
                    }
                }

                foreach (var item in result)
                {
                    var userData = await _userManager.FindByEmailAsync(item.User.Email);
                    tosend.Add(new User_Picture
                    {
                        Id = item.PhotoId,
                        Rating = item.Data.Rating,
                        Thumbnail_Url = item.Data.Thumbnail_Url,
                        Url = item.Data.Url,
                        Author = userData.Id,
                        Votes = item.Data.Votes,
                        IsVoted = userPics.Any(vote => vote == item.PhotoId)
                    });
                }

            }

            return tosend;
        }
    }
}