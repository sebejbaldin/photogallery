using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baldin.SebEJ.Gallery.Data;
using Baldin.SebEJ.Gallery.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Baldin.SebEJ.Gallery.Web.Pages.Gallery
{
    public class RankingModel : PageModel
    {
        private IDataAccess _dataAccess;

        public RankingModel(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public IEnumerable<Picture> Pictures { get; set; }

        public void OnGet()
        {
            //var list = _dataAccess.GetPictures();
            var ordinated = _dataAccess.GetPictures().ToList();
            ordinated.Sort((x, y) =>
            {
                if (x.Rating == y.Rating)
                {
                    if (x.Votes == y.Votes)
                        return 0;
                    else if (x.Votes > y.Votes)
                        return 1;
                    else
                        return -1;
                }
                else
                {
                    return (x.Rating > y.Rating) ? 1 : -1;
                }
            });
            ordinated.Reverse();
            Pictures = ordinated;
        }
    }
}