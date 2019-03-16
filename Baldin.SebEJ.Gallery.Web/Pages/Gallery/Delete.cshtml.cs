using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baldin.SebEJ.Gallery.Caching;
using Baldin.SebEJ.Gallery.Data;
using Baldin.SebEJ.Gallery.Data.Models;
using Baldin.SebEJ.Gallery.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Baldin.SebEJ.Gallery.Web.Pages.Gallery
{
    [Authorize]
    [ValidateAntiForgeryToken]
    public class DeleteModel : PageModel
    {
        private IDataAccess _dataAccess;
        private ISearch _search;
        private ICaching _caching;
        private UserManager<IdentityUser> _userManager;

        public DeleteModel(IDataAccess dataAccess, ISearch search, ICaching caching, UserManager<IdentityUser> userManager)
        {
            _dataAccess = dataAccess;
            _search = search;
            _caching = caching;
            _userManager = userManager;
        }

        public Picture Picture { get; set; }

        public async Task OnGet(int photoId)
        {
            Picture = await _dataAccess.GetPictureAsync(photoId);
        }

        public async Task<IActionResult> OnPost(int photoId)
        {
            var pic = await _dataAccess.GetPictureAsync(photoId);
            var userId = User.FindFirst("userId");
            if (userId.Value == pic.User_Id)
            {
                await _dataAccess.DeletePictureAsync(photoId);
                await _search.DeletePhotoAsync(photoId);
                await _caching.DeletePictureAsync(photoId);
            }
            return RedirectToPage("/Gallery/Index");
        }
    }
}