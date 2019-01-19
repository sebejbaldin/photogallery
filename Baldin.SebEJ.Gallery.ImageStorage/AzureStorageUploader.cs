using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Baldin.SebEJ.Gallery.ImageStorage
{
    public class AzureStorageUploader : IImageManager
    {
        private CloudBlobClient _client;
        private readonly string _container;

        public AzureStorageUploader(IConfiguration config)
        {
            var azure = config.GetSection("CloudStorage:Azure");
            var storageCredentials = new StorageCredentials(azure["Account"], azure["Secret"]);
            var storageAccount = new CloudStorageAccount(storageCredentials, true);
            _client = storageAccount.CreateCloudBlobClient();
            _container = azure["Folder"];
        }

        public async Task SaveAsync(string path, string name)
        {
            CloudBlobContainer container = _client.GetContainerReference(_container);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);
            await blockBlob.UploadFromFileAsync(path);
        }

        public async Task SaveAsync(Stream image)
        {
            string name = Guid.NewGuid().ToString();
            CloudBlobContainer container = _client.GetContainerReference(_container);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);
            await blockBlob.UploadFromStreamAsync(image);
        }

        public async Task SaveAsync(Stream image, string name)
        {
            CloudBlobContainer container = _client.GetContainerReference(_container);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);
            await blockBlob.UploadFromStreamAsync(image);
        }
    }
}
