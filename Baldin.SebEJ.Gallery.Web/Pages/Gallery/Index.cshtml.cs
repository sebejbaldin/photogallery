using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baldin.SebEJ.Gallery.Data;
using Baldin.SebEJ.Gallery.Data.Models;
using Baldin.SebEJ.Gallery.ImageStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Baldin.SebEJ.Gallery.Web.Models;
using Microsoft.Extensions.Configuration;
using Baldin.SebEJ.Gallery.Caching;

namespace Baldin.SebEJ.Gallery.Web.Pages.Gallery
{
    public class IndexModel : PageModel
    {
        private IDataAccess _dataAccess;
        private IImageManager _imageManager;
        private IConfiguration _configuration;
        private ICaching _caching;
        private UserManager<IdentityUser> _userManager;

        public IndexModel(IDataAccess dataAccess, IImageManager imageManager, UserManager<IdentityUser> userManager, IConfiguration configuration, ICaching caching)
        {
            this._dataAccess = dataAccess;
            this._imageManager = imageManager;
            this._userManager = userManager;
            this._configuration = configuration;
            this._caching = caching;
        }

        [BindProperty]
        public IFormFile Photo { get; set; }
        public PaginatedList<User_Picture> Pictures { get; set; }

        public async Task OnGet(int index = 1)
        {
            IEnumerable<User_Picture> user_Pictures = null;
            var Pics = await _caching.GetPhotosAsync();
            if (Pics == null)
            {
                Pics = await _dataAccess.GetPicturesAsync();
                #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                _caching.InsertPhotosAsync(Pics);
                #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            if (Pics != null && !User.Identity.IsAuthenticated)
            {
                user_Pictures = Pics.Select(elem => new User_Picture
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
                    #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    _caching.InsertVotesAsync(picsVoted);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    userPics = picsVoted.Select(x => x.Picture_Id);
                }
                if (Pics != null && userPics != null)
                {
                    user_Pictures = Pics.Select(elem => new User_Picture
                    {
                        Id = elem.Id,
                        Rating = elem.Rating,
                        Votes = elem.Votes,
                        Url = elem.Url,
                        Thumbnail_Url = elem.Thumbnail_Url,
                        Author = elem.User_Id,
                        IsVoted = userPics.Any(item => item == elem.Id)
                    }).ToList();
                }
                else if (Pics != null)
                {
                    user_Pictures = Pics.Select(elem => new User_Picture
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
            Pictures = PaginatedList<User_Picture>.Create(user_Pictures.OrderBy(x => x.Id), index, 6);
        }

        public async Task<IActionResult> OnPost()
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();
            if (Photo != null && Photo.Length > 0)
            {
                var fileExtension = Photo.FileName.Substring(Photo.FileName.LastIndexOf('.'));
                var name = Guid.NewGuid().ToString() + fileExtension;
                var user = await _userManager.GetUserAsync(User);
                var pic = new Picture
                {
                    OriginalName = Photo.FileName,
                    Url = $"{_configuration["PhotoUrl"]}/{name}",
                    Name = name,
                    User_Id = user.Id
                };
                pic.Id = await _dataAccess.InsertPictureAsync(pic);
                pic.Thumbnail_Url = "";
                await _imageManager.SaveAsync(Photo.OpenReadStream(), name);
                await _caching.InsertPhotoAsync(pic);
            }
            return RedirectToPage("/Gallery/Index");
        }
    }
}