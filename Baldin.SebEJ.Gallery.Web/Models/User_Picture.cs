using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Baldin.SebEJ.Gallery.Web.Models
{
    public class User_Picture
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public double Rating { get; set; }
        public int Votes { get; set; }
        public bool IsVoted { get; set; }
        public string Author { get; set; }
    }
}
