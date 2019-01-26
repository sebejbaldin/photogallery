using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baldin.SebEJ.Gallery.Caching;
using Baldin.SebEJ.Gallery.Data;
using Baldin.SebEJ.Gallery.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Baldin.SebEJ.Gallery.Web.Pages.Gallery
{
    public class RankingModel : PageModel
    {
        private IDataAccess _dataAccess;
        private ICaching _caching;

        public RankingModel(IDataAccess dataAccess, ICaching caching)
        {
            _dataAccess = dataAccess;
            _caching = caching;
        }

        public IEnumerable<Picture> Pictures { get; set; }

        public async Task OnGet()
        {
            var rank = await _caching.GetRank(3);
            if (rank == null || rank.Count() == 0)
            {
                rank = _dataAccess.GetPictures().OrderByDescending(x => x.Rating * 100 + x.Votes);
                _caching.InsertPhotosAsync(rank);
            }
            Pictures = rank.Take(3);
        }
    }
}