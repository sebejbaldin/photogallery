using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baldin.SebEJ.Gallery.Caching;
using Baldin.SebEJ.Gallery.Data;
using Baldin.SebEJ.Gallery.Search;
using Baldin.SebEJ.Gallery.Search.Models;
using Baldin.SebEJ.Gallery.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Baldin.SebEJ.Gallery.Web.Controllers
{
    [Route("api/v1/picture")]
    [ApiController]
    public class PictureV1Controller : ControllerBase
    {
        private IDataAccess _dataAccess;
        private ISearch _search;
        private ICaching _caching;
        private UserManager<IdentityUser> _userManager;

        public PictureV1Controller(IDataAccess dataAccess, ISearch search, ICaching caching, UserManager<IdentityUser> userManager)
        {
            _dataAccess = dataAccess;
            _search = search;
            _caching = caching;
            _userManager = userManager;
        }

        [HttpPost(Name = "ImageReady")]
        public async Task<IActionResult> ImageReady(ThisShouldNotExist textBody)
        {
            if (textBody == null || textBody.TextBody == null)
            {
                return BadRequest("No url provided");
            }
            var url = textBody.TextBody;
            var pic = await _dataAccess.GetPictureByUrlAsync(url);
            if (pic != null)
            {
                string filename = pic.OriginalName ?? pic.Name;
                filename = filename.Replace('_', '-').Replace('.', '-');
                var user = await _userManager.FindByIdAsync(pic.User_Id);
                bool result = await _caching.InsertPhotoAsync(pic);
                ES_DN_Photo photo = new ES_DN_Photo
                {
                    PhotoId = pic.Id,
                    User = new ES_DN_User
                    {
                        Email = user.Email,
                        UserName = user.UserName,
                        UserId = user.Id
                    },
                    Data = new ES_DN_Data
                    {
                        Name = filename,
                        Thumbnail_Url = pic.Thumbnail_Url,
                        TotalRating = pic.Total_Rating,
                        Url = pic.Url,
                        Votes = pic.Votes
                    }
                };
                result = await _search.InsertPhotoAsync(photo);
                return NoContent();
            }
            return NotFound($"The {url} was not found on this server.");
        }
    }
}