using System;
using System.Collections.Generic;
using System.Text;

namespace Baldin.SebEJ.Gallery.Data.Models
{
    public class Picture
    {
        public int Id { get; set; }
        public string OriginalName { get; set; }
        public string Name { get; set; }
        public string User_Id { get; set; }
        public string Url { get; set; }
        public int Votes { get; set; }
        public long Total_Rating { get; set; }
        public string Thumbnail_Url { get; set; }

        public double Rating {
            get {
                if (Votes > 0)
                    return (double) Total_Rating / Votes;
                else
                    return 0;
            }
        }

    }
}
