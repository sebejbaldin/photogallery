using System;
using System.Collections.Generic;
using System.Text;

namespace Baldin.SebEJ.Gallery.Search.Models
{
    public class ES_DN_Photo
    {
        public int PhotoId { get; set; }
        public ES_DN_User User { get; set; }
        public ES_DN_Data Data { get; set; }
    }
}
