using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Baldin.SebEJ.Gallery.Data;
using Baldin.SebEJ.Gallery.Data.Models;
using Baldin.SebEJ.Gallery.ImageStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Baldin.SebEJ.Gallery.Web.Pages.Gallery
{
    public class IndexModel : PageModel
    {
        private IDataAccess dataAccess;
        private IImageManager imageManager;

        public IndexModel(IDataAccess dataAccess, IImageManager imageManager)
        {
            this.dataAccess = dataAccess;
            this.imageManager = imageManager;
        }

        public class PicturesOfUser
        {
            public int Id { get; set; }
            public string Url { get; set; }
            public double Rating { get; set; }
            public int Votes { get; set; }
            public bool IsVoted { get; set; }
        }

        [BindProperty]
        public IFormFile Photo { get; set; }
        public IEnumerable<PicturesOfUser> Pictures { get; set; }

        public void OnGet()
        {
            var Pics = dataAccess.GetPictures();
            var userPics = dataAccess.GetVotesByUserId(User.Identity.Name);
            if(Pics != null && userPics != null)
            {
                Pictures = Pics.Select(elem => new PicturesOfUser
                {
                    Id = elem.Id,
                    Rating = elem.Rating,
                    Votes = elem.Votes,
                    Url = elem.Url,
                    IsVoted = userPics.Any(item => item.Picture_Id == elem.Id)
                });
            }
            else if(Pics != null)
            {
                Pictures = Pics.Select(elem => new PicturesOfUser
                {
                    Id = elem.Id,
                    Rating = elem.Rating,
                    Votes = elem.Votes,
                    Url = elem.Url,
                    IsVoted = false
                });
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