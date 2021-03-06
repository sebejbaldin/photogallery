﻿using System;
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

        public IndexModel(IDataAccess dataAccess, IImageManager imageManager, IConfiguration configuration, ICaching caching)
        {
            this._dataAccess = dataAccess;
            this._imageManager = imageManager;
            this._configuration = configuration;
            this._caching = caching;
        }

        [BindProperty]
        public IFormFile Photo { get; set; }
        public PaginatedList<User_Picture> Pictures { get; set; }

        public async Task OnGet(int index = 1)
        {
            IEnumerable<User_Picture> userPictures = null;
            var Pics = await _caching.GetPhotosByScoreAsync((index - 1) * 6, index * 6);
            if (Pics == null)
            {
                Pics = await _dataAccess.GetPaginatedPicturesAsync(index, 6);
                _caching.InsertPhotosAsync(Pics);
            }
            if (Pics != null)
            {
                if (!User.Identity.IsAuthenticated)
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
                    var userId = User.FindFirst("userId");
                    var userPics = await _caching.GetVotesByUserIdAsync(userId.Value);
                    if (userPics == null || userPics.Count() == 0)
                    {
                        var picsVoted = await _dataAccess.GetVotesByUserIdAsync(userId.Value);
                        _caching.InsertVotesAsync(picsVoted);
                        userPics = picsVoted.Select(x => x.Picture_Id);
                    }
                    if (userPics != null)
                    {
                        userPictures = Pics.Select(elem => new User_Picture
                        {
                            Id = elem.Id,
                            Rating = elem.Rating,
                            Votes = elem.Votes,
                            Url = elem.Url,
                            Thumbnail_Url = elem.Thumbnail_Url,
                            Author = elem.User_Id,
                            IsVoted = elem.User_Id == userId.Value || userPics.Any(item => item == elem.Id)
                        });
                    }
                    else
                    {
                        userPictures = Pics.Select(elem => new User_Picture
                        {
                            Id = elem.Id,
                            Rating = elem.Rating,
                            Votes = elem.Votes,
                            Url = elem.Url,
                            Thumbnail_Url = elem.Thumbnail_Url,
                            Author = elem.User_Id,
                            IsVoted = elem.User_Id == userId.Value
                        });
                    }
                }
            }
            var picCount = await _dataAccess.GetPictureCountAsync();
            Pictures = new PaginatedList<User_Picture>(userPictures, (int)picCount, index, 6);
        }

        public async Task<IActionResult> OnPost()
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();
            if (Photo != null && Photo.Length > 0)
            {
                var extensionIndex = Photo.FileName.LastIndexOf('.');
                var fileExtension = Photo.FileName.Substring(extensionIndex);
                var name = Guid.NewGuid().ToString() + fileExtension;
                string filename = Photo.FileName.Substring(0, Photo.FileName.Length - (Photo.FileName.Length - extensionIndex));
                var userId = User.FindFirst("userId");
                var pic = new Picture
                {
                    OriginalName = filename,
                    Url = $"{_configuration["PhotoUrl"]}/{name}",
                    Name = name,
                    User_Id = userId.Value
                };
                pic.Id = await _dataAccess.InsertPictureAsync(pic);
                //pic.Thumbnail_Url = "";
                await _imageManager.SaveAsync(Photo.OpenReadStream(), name);
                //await _caching.InsertPhotoAsync(pic);
            }
            return RedirectToPage("/Gallery/Index");
        }
    }
}