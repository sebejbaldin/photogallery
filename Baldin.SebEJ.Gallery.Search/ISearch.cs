using Baldin.SebEJ.Gallery.Search.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baldin.SebEJ.Gallery.Search
{
    public interface ISearch
    {
        //Task<bool> BulkInsertUsersPicturesAsync(IEnumerable<ES_UserPhotos> usersPhotos);
        //Task<bool> InsertUserPicturesAsync(ES_UserPhotos userPhotos);
        //Task<bool> UpdateUserPicturesAsync(ES_UserPhotos userPhotos);
        //Task<IEnumerable<ES_UserPhotos>> SearchAsync(string query);
        //Task<IEnumerable<ES_Picture>> SearchPhotosAsync(string query);

        Task<bool> InsertPhotosAsync(IEnumerable<ES_DN_Photo> photos);
        Task<bool> InsertPhotoAsync(ES_DN_Photo photo);
        Task<bool> UpdatePhotoAsync(ES_DN_Photo photo);
        Task<bool> UpdateScoreAsync(int photoId, long totalRating, int votes);
        Task<bool> DeletePhotoAsync(int Id);
        Task<PaginatedPhotos> PaginatedSearchAsync(string query, int page = 1);
        Task<IReadOnlyCollection<ES_DN_Photo>> SearchPhotosAsync(int qty, string query);
    }
}
