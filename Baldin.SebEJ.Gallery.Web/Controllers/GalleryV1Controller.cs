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

namespace Baldin.SebEJ.Gallery.Web.Controllers
{
    [Route("api/v1/gallery")]
    [ApiController]
    public class GalleryV1Controller : ControllerBase
    {
        private IDataAccess _dataAccess;
        private UserManager<IdentityUser> _userManager;

        public GalleryV1Controller(IDataAccess dataAccess, UserManager<IdentityUser> userManager)
        {
            _dataAccess = dataAccess;
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Vote(Vote vote)
        {
            if (vote.Rating > 5 || vote.Rating < 0)
                return ValidationProblem();
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            vote.User_Id = user.Id;
            var result = _dataAccess.InsertVote(vote);
            if (result)
            {
                var pic = _dataAccess.GetPicture(vote.Picture_Id);
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
        public IEnumerable<User_Picture> GetPictures()
        {
            var Pics = _dataAccess.GetPictures();
            if (!User.Identity.IsAuthenticated)
            {
                return Pics.Select(elem => new User_Picture
                {
                    Id = elem.Id,
                    Rating = elem.Rating,
                    Votes = elem.Votes,
                    Url = elem.Url,
                    IsVoted = true
                }).ToList();
            }
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var userPics = _dataAccess.GetVotesByUserId(user.Id);
            if (Pics != null && userPics != null)
            {
                return Pics.Select(elem => new User_Picture
                {
                    Id = elem.Id,
                    Rating = elem.Rating,
                    Votes = elem.Votes,
                    Url = elem.Url,
                    IsVoted = userPics.Any(item => item.Picture_Id == elem.Id)
                }).ToList();
            }
            else if (Pics != null)
            {
                return Pics.Select(elem => new User_Picture
                {
                    Id = elem.Id,
                    Rating = elem.Rating,
                    Votes = elem.Votes,
                    Url = elem.Url,
                    IsVoted = false
                });
            }
            return null;
        }
    }
}