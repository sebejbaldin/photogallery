using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Baldin.SebEJ.Gallery.ImageStorage
{
    public interface IImageManager
    {
        Task SaveAsync(string path, string name);
        Task SaveAsync(Stream image);
        Task SaveAsync(Stream image, string name);
    }
}
