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
        private IDataAccess dataAccess;
        private IImageManager imageManager;
        private IConfiguration configuration;
        private ICaching caching;
        private UserManager<IdentityUser> userManager;

        public IndexModel(IDataAccess dataAccess, IImageManager imageManager, UserManager<IdentityUser> userManager, IConfiguration configuration, ICaching caching)
        {
            this.dataAccess = dataAccess;
            this.imageManager = imageManager;
            this.userManager = userManager;
            this.configuration = configuration;
            this.caching = caching;
        }

        [BindProperty]
        public IFormFile Photo { get; set; }
        public PaginatedList<User_Picture> Pictures { get; set; }

        public async Task OnGet(int index = 1)
        {
            IEnumerable<User_Picture> user_Pictures = null;
            var Pics = await caching.GetPhotosAsync();
            if (Pics == null)
            {
                Pics = await dataAccess.GetPicturesAsync();
                #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                caching.InsertPhotosAsync(Pics);
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
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                var userPics = await caching.GetVotesByUserIdAsync(user.Id);
                if (userPics == null || userPics.Count() == 0)
                {
                    var picsVoted = await dataAccess.GetVotesByUserIdAsync(user.Id);
                    #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    caching.InsertVotesAsync(picsVoted);
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
                var user = await userManager.GetUserAsync(User);
                var storageUrlConf = "CloudStorage:" + configuration["Storage"] + ":Storage";
                var pic = new Picture
                {
                    Url = configuration[storageUrlConf] + "images/" + name,
                    Name = name,
                    User_Id = user.Id
                };
                pic.Id = await dataAccess.InsertPictureAsync(pic);
                pic.Thumbnail_Url = "";
                await imageManager.SaveAsync(Photo.OpenReadStream(), name);
                await caching.InsertPhotoAsync(pic);
            }
            return RedirectToPage("/Gallery/Index");
        }
    }
}