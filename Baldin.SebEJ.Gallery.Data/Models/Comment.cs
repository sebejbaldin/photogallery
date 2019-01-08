using System;
using System.Collections.Generic;
using System.Text;

namespace Baldin.SebEJ.Gallery.Data.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int Picture_Id { get; set; }
        public string Author { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }
        public DateTime InsertDate { get; set; }
    }
}
