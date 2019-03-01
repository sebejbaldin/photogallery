using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baldin.SebEJ.Gallery.Data;
using Baldin.SebEJ.Gallery.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Baldin.SebEJ.Gallery.Web.Models;
using Baldin.SebEJ.Gallery.Caching;
using Baldin.SebEJ.Gallery.Search;

namespace Baldin.SebEJ.Gallery.Web.Controllers
{
    [Route("api/v1/gallery")]
    [ApiController]
    public class GalleryV1Controller : ControllerBase
    {
        private IDataAccess _dataAccess;
        private UserManager<IdentityUser> _userManager;
        private ICaching _caching;
        private ISearch _search;

        public GalleryV1Controller(IDataAccess dataAccess, UserManager<IdentityUser> userManager, ICaching caching, ISearch search)
        {
            _dataAccess = dataAccess;
            _userManager = userManager;
            _caching = caching;
            _search = search;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Vote(Vote vote)
        {
            if (vote.Rating > 5 || vote.Rating < 0)
                return ValidationProblem();
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            vote.User_Id = user.Id;
            var result = await _dataAccess.InsertVoteAsync(vote);
            if (result)
            {
                _caching.InsertVoteAsync(vote);
                var pic = await _caching.GetPhotoAsync(vote.Picture_Id);
                if (pic != null)
                {
                    pic.Total_Rating += vote.Rating;
                    pic.Votes++;
                }
                else
                    pic = await _dataAccess.GetPictureAsync(vote.Picture_Id);

                _caching.UpdatePictureAsync(pic);
                return Ok(new {
                    average = pic.Rating,
                    count = pic.Votes
                });
            }
            else
            {
                return ValidationProblem();
            }
        }

        [HttpGet]
        public async Task<IEnumerable<User_Picture>> GetPictures()
        {
            var Pics = await _caching.GetPhotosAsync();
            if (Pics == null)
            {
                Pics = await _dataAccess.GetPicturesAsync();
                _caching.InsertPhotosAsync(Pics);
            }
            if (Pics != null && !User.Identity.IsAuthenticated)
            {
                return Pics.Select(elem => new User_Picture
                {
                    Id = elem.Id,
                    Rating = elem.Rating,
                    Votes = elem.Votes,
                    Url = elem.Url,
                    Thumbnail_Url = elem.Thumbnail_Url,
                    Author = elem.User_Id,
                    IsVoted = true
                });
            }
            else
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var userPics = await _caching.GetVotesByUserIdAsync(user.Id);
                if (userPics == null || userPics.Count() == 0)
                {
                    var picsVoted = await _dataAccess.GetVotesByUserIdAsync(user.Id);
                    _caching.InsertVotesAsync(picsVoted);
                    userPics = picsVoted.Select(x => x.Picture_Id);
                }
                if (Pics != null && userPics != null)
                {
                    return Pics.Select(elem => new User_Picture
                    {
                        Id = elem.Id,
                        Rating = elem.Rating,
                        Votes = elem.Votes,
                        Url = elem.Url,
                        Thumbnail_Url = elem.Thumbnail_Url,
                        Author = elem.User_Id,
                        IsVoted = userPics.Any(item => item == elem.Id)
                    });
                }
                else if (Pics != null)
                {
                    return Pics.Select(elem => new User_Picture
                    {
                        Id = elem.Id,
                        Rating = elem.Rating,
                        Votes = elem.Votes,
                        Url = elem.Url,
                        Thumbnail_Url = elem.Thumbnail_Url,
                        Author = elem.User_Id,
                        IsVoted = false
                    });
                }
            }
            return null;
        }

        [HttpGet("{index}")]
        public async Task<IActionResult> GetPaginatedPictures(int index)
        {
            IEnumerable<User_Picture> userPictures = null;
            var Pics = await _caching.GetPhotosByScoreAsync((index - 1) * 6, index * 6);
            if (Pics == null)
            {
                Pics = await _dataAccess.GetPaginatedPicturesAsync(index, 6);
                _caching.InsertPhotosAsync(Pics);
            }
            if (Pics != null && !User.Identity.IsAuthenticated)
            {
                userPictures = Pics.Select(elem => new User_Picture
                {
                    Id = elem.Id,
                    Rating = elem.Rating,
                    Votes = elem.Votes,
                    Url = elem.Url,
                    Thumbnail_Url = elem.Thumbnail_Url,
                    Author = elem.User_Id,
                    IsVoted = true
                });
            }
            else
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var userPics = await _caching.GetVotesByUserIdAsync(user.Id);
                if (userPics == null || userPics.Count() == 0)
                {
                    var picsVoted = await _dataAccess.GetVotesByUserIdAsync(user.Id);
                    _caching.InsertVotesAsync(picsVoted);
                    userPics = picsVoted.Select(x => x.Picture_Id);
                }
                if (Pics != null && userPics != null)
                {
                    userPictures = Pics.Select(elem => new User_Picture
                    {
                        Id = elem.Id,
                        Rating = elem.Rating,
                        Votes = elem.Votes,
                        Url = elem.Url,
                        Thumbnail_Url = elem.Thumbnail_Url,
                        Author = elem.User_Id,
                        IsVoted = elem.User_Id == user.Id || userPics.Any(item => item == elem.Id)
                    });
                }
                else if (Pics != null)
                {
                    userPictures = Pics.Select(elem => new User_Picture
                    {
                        Id = elem.Id,
                        Rating = elem.Rating,
                        Votes = elem.Votes,
                        Url = elem.Url,
                        Thumbnail_Url = elem.Thumbnail_Url,
                        Author = elem.User_Id,
                        IsVoted = false
                    });
                }
            }
            var picCount = await _dataAccess.GetPictureCountAsync();
            return Ok(new {
                pageCount = (int)Math.Ceiling(picCount / 6D),
                photos = userPictures
            });
        }
    }
}