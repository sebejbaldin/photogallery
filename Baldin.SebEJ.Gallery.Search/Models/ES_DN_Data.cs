using System;
using System.Collections.Generic;
using System.Text;

namespace Baldin.SebEJ.Gallery.Search.Models
{
    public class ES_DN_Data
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Thumbnail_Url { get; set; }
        public long TotalRating { get; set; }
        public int Votes { get; set; }

        public double Rating {
            get {
                if (Votes > 0)
                    return (double)TotalRating / Votes;
                else
                    return 0;
            }
        }
    }
}
