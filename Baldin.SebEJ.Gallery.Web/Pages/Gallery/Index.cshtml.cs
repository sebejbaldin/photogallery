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

namespace Baldin.SebEJ.Gallery.Web.Pages.Gallery
{
    public class IndexModel : PageModel
    {
        private IDataAccess dataAccess;
        private IImageManager imageManager;
        private UserManager<IdentityUser> userManager;

        public IndexModel(IDataAccess dataAccess, IImageManager imageManager, UserManager<IdentityUser> userManager)
        {
            this.dataAccess = dataAccess;
            this.imageManager = imageManager;
            this.userManager = userManager;
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
                        IsVoted = false
                    });
                }
            }
        }

        public async Task<IActionResult> OnPost()
        {
            if (Photo != null && Photo.Length > 0)
            {
                var fileExtension = Photo.FileName.Substring(Photo.FileName.LastIndexOf('.'));
                var name = Guid.NewGuid().ToString() + fileExtension;
                await imageManager.SaveAsync(Photo.OpenReadStream(), name);
                var pic = new Picture
                {
                    Url = $@"/uploads/{name}",
                    Name = name
                };
                dataAccess.InsertPicture(pic);
                return RedirectToPage("/Gallery/Index");
            }
            return Page();
        }
    }
}