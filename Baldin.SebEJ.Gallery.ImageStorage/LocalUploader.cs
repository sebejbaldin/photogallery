using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Baldin.SebEJ.Gallery.ImageStorage
{
    public class LocalUploader : IImageManager
    {
        private string WebRoot;

        public LocalUploader(string webrootpath)
        {
            WebRoot = webrootpath;
        }

        public async Task SaveAsync(string path, string name)
        {
            throw new NotImplementedException();
        }

        public async Task SaveAsync(Stream image)
        {
            try
            {
                string path = WebRoot + @"\uploads\" + Guid.NewGuid().ToString();
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
            }
            finally
            {
                image.Close();
            }
        }

        public async Task SaveAsync(Stream image, string name)
        {
            try
            {
                string path = WebRoot + @"\uploads\" + name;
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
            }
            finally
            {
                image.Close();
            }
        }
    }
}
