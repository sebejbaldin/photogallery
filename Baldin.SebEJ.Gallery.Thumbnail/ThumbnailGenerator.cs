using System.Collections.Generic;
using System.IO;
using ImageResizer;
using Microsoft.Azure.WebJobs;


namespace Baldin.SebEJ.Gallery.Thumbnail
{
    public static class ThumbnailGenerator
    {
        [FunctionName("ResizeImage")]
        public static void Run(
            [BlobTrigger("galleryimg/{name}")] Stream image,
            [Blob("thumbnails-small/{name}", FileAccess.Write)] Stream imageSmall,
            [Blob("thumbnails-medium/{name}", FileAccess.Write)] Stream imageMedium)
        {
            var imageBuilder = ImageBuilder.Current;
            var size = imageDimensionsTable[ImageSize.Small];

            imageBuilder.Build(image, imageSmall,
                new ResizeSettings(size.Item1, size.Item2, FitMode.Max, null), false);

            image.Position = 0;
            size = imageDimensionsTable[ImageSize.Medium];

            imageBuilder.Build(image, imageMedium,
                new ResizeSettings(size.Item1, size.Item2, FitMode.Max, null), false);
        }

        public enum ImageSize { ExtraSmall, Small, Medium }

        private static Dictionary<ImageSize, (int, int)> imageDimensionsTable = new Dictionary<ImageSize, (int, int)>() {
            { ImageSize.ExtraSmall, (320, 200) },
            { ImageSize.Small,      (640, 400) },
            { ImageSize.Medium,     (800, 600) }
        };
    }
}
