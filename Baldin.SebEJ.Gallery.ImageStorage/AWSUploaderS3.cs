using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Baldin.SebEJ.Gallery.ImageStorage
{
    public class AWSUploaderS3 : IImageManager
    {
        private readonly IAmazonS3 client;

        public AWSUploaderS3(IConfiguration configuration)
        {
            var key = configuration["Amazon:Id_Key"];
            Console.WriteLine(key);
            var config = new AmazonS3Config();
            client = new AmazonS3Client(configuration["Amazon:Id_Key"], configuration["Amazon:Secret"], Amazon.RegionEndpoint.EUWest1);
        }

        public async Task SaveAsync(string path, string name)
        {
            using (var transfer = new TransferUtility(client))
            {
                await transfer.UploadAsync(path, "tsac2018-baldin-photo", name);
            }
        }

        public async Task SaveAsync(Stream image)
        {
            using (var transfer = new TransferUtility(client))
            {
                await transfer.UploadAsync(image, "tsac2018-baldin-photo", Guid.NewGuid().ToString());
            }
        }

        public async Task SaveAsync(Stream image, string name)
        {
            using (var transfer = new TransferUtility(client))
            {
                var qwetrt121223 = new TransferUtilityUploadRequest()
                {
                    InputStream = image,
                    CannedACL = S3CannedACL.PublicRead,
                    BucketName = "tsac2018-baldin-photo",
                    Key = name
                };

                await transfer.UploadAsync(qwetrt121223);
            }
        }
    }
}
