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
        private readonly string bucketName;
        private readonly string folderName;

        public AWSUploaderS3(IConfiguration configuration)
        {
            var amazon = configuration.GetSection("CloudStorage:Amazon");
            client = new AmazonS3Client(amazon["Account"], amazon["Secret"], Amazon.RegionEndpoint.EUWest1);
            bucketName = amazon["Bucket"];
            folderName = amazon["Folder"];
        }

        public async Task SaveAsync(string path, string name)
        {
            using (var transfer = new TransferUtility(client))
            {
                var originalName = new TransferUtilityUploadRequest()
                {
                    FilePath = path,
                    CannedACL = S3CannedACL.PublicRead,
                    BucketName = bucketName,
                    Key = folderName + "/" + name
                };
                await transfer.UploadAsync(originalName);
            }
        }

        public async Task SaveAsync(Stream image)
        {
            using (var transfer = new TransferUtility(client))
            {
                var originalName = new TransferUtilityUploadRequest()
                {
                    InputStream = image,
                    CannedACL = S3CannedACL.PublicRead,
                    BucketName = bucketName,
                    Key = folderName + "/" + Guid.NewGuid().ToString()
                };
                await transfer.UploadAsync(originalName);
            }
        }

        public async Task SaveAsync(Stream image, string name)
        {
            using (var transfer = new TransferUtility(client))
            {
                var originalName = new TransferUtilityUploadRequest()
                {
                    InputStream = image,
                    CannedACL = S3CannedACL.PublicRead,
                    BucketName = bucketName,
                    Key = folderName + "/" + name
                };

                await transfer.UploadAsync(originalName);
            }
        }
    }
}
