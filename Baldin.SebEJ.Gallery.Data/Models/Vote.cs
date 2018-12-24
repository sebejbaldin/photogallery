using System;
using System.Collections.Generic;
using System.Text;

namespace Baldin.SebEJ.Gallery.Data.Models
{
    public class Vote
    {
        public string User_Id { get; set; }
        public int Picture_Id { get; set; }
        public byte Rating { get; set; }
    }
}
