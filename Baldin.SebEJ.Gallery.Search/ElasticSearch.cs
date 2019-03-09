using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baldin.SebEJ.Gallery.Search.Models;
using Microsoft.Extensions.Configuration;
using Nest;

namespace Baldin.SebEJ.Gallery.Search
{
    public class ElasticSearch : ISearch
    {
        private ElasticClient _client;

        public ElasticSearch(IConfiguration config)
        {
            var esConf = config.GetSection("Search:ElasticSearch");
            var connConfig = new ConnectionSettings(new Uri(esConf["ConnectionString"]))
                .EnableTcpKeepAlive(TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(3))
                .RequestTimeout(TimeSpan.FromSeconds(15))
                //.DefaultIndex("photoByUser")
                .DefaultMappingFor<ES_DN_Photo>(e => e
                    .IndexName("photos")
                    .TypeName("photo")
                    .IdProperty(id => id.PhotoId)
                );

            _client = new ElasticClient(connConfig);
        }

        public async Task<bool> InsertPhotoAsync(ES_DN_Photo photo)
        {
            var resp = await _client.IndexDocumentAsync(photo);
            return resp.IsValid;
        }

        public async Task<bool> InsertPhotosAsync(IEnumerable<ES_DN_Photo> photos)
        {
            var resp = await _client.BulkAsync(b => b.IndexMany(photos));
            return resp.IsValid;
        }
       
        public async Task<bool> UpdatePhotoAsync(ES_DN_Photo photo)
        {
            var resp = await _client.IndexDocumentAsync(photo);
            return resp.IsValid;
        }

        public async Task<IEnumerable<ES_DN_Photo>> SearchPhotosAsync(string query, int page = 1)
        {
            if (page < 1)
                page = 1;
            var resp = await _client.SearchAsync<ES_DN_Photo>(e => e
                .From((page - 1) * 6)
                .Size(page * 6)
                .Query(q => q
                    .MultiMatch(mm => mm                    
                        .Fields(f => f
                            .Field(fs => fs.Data.Name)
                            .Field(fs => fs.User.Email)
                            .Field(fs => fs.User.UserName)
                        )
                        .Query(query)
                    )
                )
            );
            if (resp.IsValid)
            {
                return resp.Documents.ToArray();
            }
            return new ES_DN_Photo[0];
        }

        public async Task<bool> UpdateScoreAsync(int photoId, long totalRating, int votes)
        {
            //var update = new UpdateDescriptor<ES_DN_Photo, object>("photos", "photo", photoId);
            //update.Doc(new { Data = new {  } });
            IUpdateRequest<ES_DN_Photo, ES_DN_Photo> updateRequest = new UpdateRequest<ES_DN_Photo, ES_DN_Photo>("photos", "photo", photoId);
            updateRequest.Doc = new ES_DN_Photo
            {
                Data = new ES_DN_Data
                {
                    TotalRating = totalRating,
                    Votes = votes
                },
                PhotoId = photoId
            };
            var resp = await _client.UpdateAsync<ES_DN_Photo>(updateRequest);
            return resp.IsValid;
        }
    }
}
