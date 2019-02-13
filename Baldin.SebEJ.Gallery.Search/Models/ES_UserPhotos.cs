using System;
using System.Collections.Generic;
using System.Text;

namespace Baldin.SebEJ.Gallery.Search.Models
{
    public class ES_UserPhotos
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public IEnumerable<ES_Picture> Pictures { get; set; }
    }
}
