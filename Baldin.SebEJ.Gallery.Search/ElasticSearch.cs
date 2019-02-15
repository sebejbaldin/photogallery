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
                .DefaultMappingFor<ES_UserPhotos>(e => e
                    .IndexName("photos")
                    .TypeName("userPhotos")
                )
                .DefaultMappingFor<ES_Picture>(e => e
                    .IndexName("photos")
                    .TypeName("userPhotos")
                );

            _client = new ElasticClient(connConfig);
        }

        public Task<bool> BulkInsertUsersPicturesAsync(IEnumerable<ES_UserPhotos> usersPhotos)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> InsertUserPicturesAsync(ES_UserPhotos userPhotos)
        {
            var resp = await _client.IndexDocumentAsync(userPhotos);
            return resp.IsValid;
        }

        public async Task<IEnumerable<ES_UserPhotos>> SearchAsync(string query)
        {
            var resp = await _client.SearchAsync<ES_UserPhotos>(e => e
                .From(0)
                .Size(6)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.UserName)
                        .Field(f => f.Email)
                        .Query(query)
                    )
                )
            );
            return resp.Documents.ToArray();
        }

        public async Task<IEnumerable<ES_Picture>> SearchPhotosAsync(string query)
        {
            var resp = await _client.SearchAsync<ES_Picture>(e => e
                .From(0)
                .Size(6)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Name)
                        .Query(query)
                    )
                )
            );
            return resp.Documents.ToArray();
        }

        public Task<bool> UpdateUserPicturesAsync(ES_UserPhotos userPhotos)
        {
            throw new NotImplementedException();
        }
    }
}
