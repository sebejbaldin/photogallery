using System;
using System.Collections.Generic;
using System.Text;

namespace Baldin.SebEJ.Gallery.Search.Models
{
    public class PaginatedPhotos
    {
        public long TotalResults { get; set; }
        public IReadOnlyCollection<ES_DN_Photo> Photos { get; set; }
    }
}
