using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baldin.SebEJ.Gallery.Data;
using Baldin.SebEJ.Gallery.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Baldin.SebEJ.Gallery.Web.Pages.Gallery
{
    public class PhotoModel : PageModel
    {
        private IDataAccess _dataAccess;
        private UserManager<IdentityUser> _userManager;

        public PhotoModel(IDataAccess dataAccess, UserManager<IdentityUser> userManager)
        {
            _dataAccess = dataAccess;
            _userManager = userManager;
        }

        public Picture Picture { get; set; }
        public string AuthorEmail { get; set; }
        public IEnumerable<Comment> Comments { get; set; }

        public async Task OnGet(int photoId)
        {
            Picture = await _dataAccess.GetPictureAsync(photoId);
            Comments = await _dataAccess.GetCommentsByPhotoIdAsync(photoId);
            AuthorEmail = (await _userManager.FindByIdAsync(Picture.User_Id)).Email;
        }
    }
}