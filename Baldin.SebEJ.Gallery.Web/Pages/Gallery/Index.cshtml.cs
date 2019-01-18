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

namespace Baldin.SebEJ.Gallery.Web.Pages.Gallery
{
    public class IndexModel : PageModel
    {
        private IDataAccess dataAccess;
        private IImageManager imageManager;
        private IConfiguration configuration;
        private UserManager<IdentityUser> userManager;

        public IndexModel(IDataAccess dataAccess, IImageManager imageManager, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            this.dataAccess = dataAccess;
            this.imageManager = imageManager;
            this.userManager = userManager;
            this.configuration = configuration;
        }

        [BindProperty]
        public IFormFile Photo { get; set; }
        public IEnumerable<User_Picture> Pictures { get; set; }

        public void OnGet()
        {
            var Pics = dataAccess.GetPictures();
            if (Pics != null && !User.Identity.IsAuthenticated)
            {
                Pictures = Pics.Select(elem => new User_Picture
                {
                    Id = elem.Id,
                    Rating = elem.Rating,
                    Votes = elem.Votes,
                    Url = elem.Url,
                    Thumbnail_Url = elem.Thumbnail_Url,
                    Author = elem.User_Id,
                    IsVoted = true
                }).ToList();
            }
            else
            {
                var user = userManager.FindByNameAsync(User.Identity.Name).Result;
                var userPics = dataAccess.GetVotesByUserId(user.Id);
                if (Pics != null && userPics != null)
                {
                    Pictures = Pics.Select(elem => new User_Picture
                    {
                        Id = elem.Id,
                        Rating = elem.Rating,
                        Votes = elem.Votes,
                        Url = elem.Url,
                        Thumbnail_Url = elem.Thumbnail_Url,
                        Author = elem.User_Id,
                        IsVoted = userPics.Any(item => item.Picture_Id == elem.Id)
                    }).ToList();
                }
                else if (Pics != null)
                {
                    Pictures = Pics.Select(elem => new User_Picture
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
                await imageManager.SaveAsync(Photo.OpenReadStream(), name);
                var pic = new Picture
                {
                    Url = configuration["CDN:Amazon"] + "images/" + name,
                    Name = name,
                    User_Id = user.Id
                };
                dataAccess.InsertPicture(pic);
            }
            return RedirectToPage("/Gallery/Index");
        }
    }
}