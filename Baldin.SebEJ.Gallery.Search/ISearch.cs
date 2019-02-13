using Baldin.SebEJ.Gallery.Search.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baldin.SebEJ.Gallery.Search
{
    public interface ISearch
    {
        Task<bool> BulkInsertUsersPicturesAsync(IEnumerable<ES_UserPhotos> usersPhotos);
        Task<bool> InsertUserPicturesAsync(ES_UserPhotos userPhotos);
        Task<bool> UpdateUserPicturesAsync(ES_UserPhotos userPhotos);
        Task<IEnumerable<ES_UserPhotos>> SearchAsync(string query);
        Task<IEnumerable<ES_Picture>> SearchPhotosAsync(string query);
    }
}
