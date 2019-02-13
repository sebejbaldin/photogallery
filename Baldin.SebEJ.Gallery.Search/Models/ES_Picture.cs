using System;
using System.Collections.Generic;
using System.Text;

namespace Baldin.SebEJ.Gallery.Search.Models
{
    public class ES_Picture
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Votes { get; set; }
        public double Rating { get; set; }
        public string Thumbnail_Url { get; set; }
    }
}
