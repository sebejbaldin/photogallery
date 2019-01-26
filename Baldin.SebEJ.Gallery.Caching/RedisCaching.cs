using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baldin.SebEJ.Gallery.Data.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Baldin.SebEJ.Gallery.Caching
{
    public class RedisCaching : ICaching
    {
        private ConnectionMultiplexer _multiplexer;
        private IDatabase _client;

        public RedisCaching(IConfiguration config)
        {
            var redis = config.GetSection("Caching:Redis");
            ConfigurationOptions confRedis = new ConfigurationOptions
            {
                EndPoints =
                {
                    { redis["Host"], 6379 }
                },
                CommandMap = CommandMap.Create(new HashSet<string>()
                { // EXCLUDE a few commands
                    "INFO", "CONFIG", "CLUSTER",
                    "PING", "ECHO", "CLIENT"
                }, available: false),
                KeepAlive = 180,
                Password = redis["Password"],
                ReconnectRetryPolicy = new ExponentialRetry(10000),
                ConnectRetry = 2,
                AbortOnConnectFail = false
            };
            //string connString = $"{redis["Host"]},password={redis["Password"]}";
            _multiplexer = ConnectionMultiplexer.Connect(confRedis);
            _client = _multiplexer.GetDatabase();
        }

        public async Task<IEnumerable<Picture>> GetPhotosAsync()
        {
            if (!_multiplexer.IsConnected)
                return null;
            var imagesList = await _client.SetMembersAsync("photos");
            if (imagesList == null || imagesList.Length == 0)
            {
                return null;
            }
            var list = new Picture[imagesList.Length];
            HashEntry[] image;
            for (int i = 0; i < imagesList.Length; i++)
            {
                if (int.TryParse(imagesList[i], out int res))
                {
                    image = await _client.HashGetAllAsync($"pic_{res}");
                    if (image.Length == 0)
                        return null;
                }
                else
                    continue;
                list[i] = HashMapToPicture(image, res);
            }
            //list = list.Where(x => x != null).ToArray();
            return list;
        }

        public async Task<Picture> GetPhotoAsync(int Id)
        {
            if (!_multiplexer.IsConnected)
                return null;
            var photoData = await _client.HashGetAllAsync($"pic_{Id}");
            if (photoData != null && photoData.Length > 0)
            {
                return HashMapToPicture(photoData, Id);
            }
            return null;
        }

        public async Task<bool> InsertPhotoAsync(Picture picture)
        {
            if (!_multiplexer.IsConnected)
                return false;
            HashEntry[] imgData = PictureToHashMap(picture);
            await _client.HashSetAsync($"pic_{picture.Id}", imgData);
            return await _client.SetAddAsync("photos", picture.Id);
        }

        public async Task<bool> InsertPhotosAsync(IEnumerable<Picture> pictures)
        {
            if (!_multiplexer.IsConnected)
                return false;
            SortedSetEntry[] rank = new SortedSetEntry[pictures.Count()];
            var imgKeys = new RedisValue[pictures.Count()];
            Picture pic;
            for (int i = 0; i < pictures.Count(); i++)
            {
                pic = pictures.ElementAt(i);
                imgKeys[i] = pic.Id;
                rank[i] = new SortedSetEntry(pic.Id, pic.Rating * 100 + pic.Votes);
                await _client.HashSetAsync($"pic_{pic.Id}", PictureToHashMap(pic));
            }
            await _client.SetAddAsync("photos", imgKeys);
            await _client.SortedSetAddAsync("ranking", rank);
            return true;
        }

        public Task<IEnumerable<Vote>> GetVotesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Vote>> GetVotesByUserId(string userId)
        {
            if (!_multiplexer.IsConnected)
                return null;
            if (userId != null)
            {
                var votes = await _client.SetMembersAsync(userId);
                List<Vote> userVotes = new List<Vote>(votes.Count());
                foreach (var vote in votes)
                {
                    userVotes.Add(new Vote()
                    {
                        User_Id = userId,
                        Picture_Id = (int)vote
                    });
                }
                return userVotes;
            }
            return null;
        }

        public Task<Vote> GetVoteAsync(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> InsertVoteAsync(Vote vote)
        {
            if (!_multiplexer.IsConnected)
                return false;
            if (vote != null)
            {
                return await _client.SetAddAsync(vote.User_Id, vote.Picture_Id);
            }
            return false;
        }

        public async Task<bool> InsertVotesAsync(IEnumerable<Vote> votes)
        {
            if (!_multiplexer.IsConnected)
                return false;
            if (votes != null)
            {
                RedisValue[] picturesId;
                var userGrouped = votes.GroupBy(x => x.User_Id);
                foreach (var userVotes in userGrouped)
                {
                    picturesId = new RedisValue[userVotes.Count()];
                    for (int i = 0; i < userVotes.Count(); i++)
                    {
                        picturesId[i] = userVotes.ElementAt(i).Picture_Id;
                    }
                    await _client.SetAddAsync(userVotes.Key, picturesId);
                }
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Picture>> GetRank(int topN = -1)
        {
            if (!_multiplexer.IsConnected)
                return null;
            var topPics = await _client.SortedSetRangeByRankAsync("ranking", 0, topN, Order.Descending);
            var photos = new Picture[topPics.Length];
            int currentId;
            for (int e = 0; e < topPics.Length; e++)
            {
                currentId = (int)topPics[e];
                photos[e] = HashMapToPicture(await _client.HashGetAllAsync($"pic_{currentId}"), currentId);
            }
            return photos;
        }

        private Picture HashMapToPicture(HashEntry[] hash, int Id)
        {
            var picture = new Picture()
            {
                Id = Id,
                Name = "",
                User_Id = hash.First(x => x.Name == "user_id").Value,
                Url = hash.First(x => x.Name == "url").Value,
                Thumbnail_Url = hash.First(x => x.Name == "thumbnail_url").Value
            };
            if (int.TryParse(hash.First(x => x.Name == "votes").Value, out int votes))
            {
                picture.Votes = votes;
            }
            if (long.TryParse(hash.First(x => x.Name == "total_rating").Value, out long total_rating))
            {
                picture.Total_Rating = total_rating;
            }
            return picture;
        }

        private HashEntry[] PictureToHashMap(Picture picture)
        {
            HashEntry[] imgData = new HashEntry[5];
            imgData[0] = new HashEntry("url", picture.Url);
            imgData[1] = new HashEntry("thumbnail_url", picture.Thumbnail_Url ?? "");
            imgData[2] = new HashEntry("user_id", picture.User_Id);
            imgData[3] = new HashEntry("votes", picture.Votes);
            imgData[4] = new HashEntry("total_rating", picture.Total_Rating);
            return imgData;
        }

        public bool IsConnected()
        {
            return _multiplexer.IsConnected;
        }
    }
}
