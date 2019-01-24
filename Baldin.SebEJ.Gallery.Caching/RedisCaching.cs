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
            string connString = $"{redis["Host"]},password={redis["Password"]}";
            _multiplexer = ConnectionMultiplexer.Connect(connString);
            _client = _multiplexer.GetDatabase();
        }

        public async Task<IEnumerable<Picture>> GetPhotosAsync()
        {
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
                list[i] = new Picture()
                {
                    Id = res,
                    Name = "",
                    User_Id = image.First(x => x.Name == "user_id").Value,
                    Url = image.First(x => x.Name == "url").Value,
                    Thumbnail_Url = image.First(x => x.Name == "thumbnail_url").Value
                };
                if (int.TryParse(image.First(x => x.Name == "votes").Value, out int votes))
                {
                    list[i].Votes = votes;
                }
                if (long.TryParse(image.First(x => x.Name == "total_rating").Value, out long total_rating))
                {
                    list[i].Total_Rating = total_rating;
                }
            }
            //list = list.Where(x => x != null).ToArray();
            return list;
        }

        public async Task<Picture> GetPhotoAsync(int Id)
        {
            var photoData = await _client.HashGetAllAsync($"pic_{Id}");
            if (photoData != null && photoData.Length > 0)
            {
                var picture = new Picture()
                {
                    Id = Id,
                    Name = "",
                    User_Id = photoData.First(x => x.Name == "user_id").Value,
                    Url = photoData.First(x => x.Name == "url").Value,
                    Thumbnail_Url = photoData.First(x => x.Name == "thumbnail_url").Value
                };
                if (int.TryParse(photoData.First(x => x.Name == "votes").Value, out int votes))
                {
                    picture.Votes = votes;
                }
                if (long.TryParse(photoData.First(x => x.Name == "total_rating").Value, out long total_rating))
                {
                    picture.Total_Rating = total_rating;
                }
                return picture;
            }
            return null;
        }

        public async Task<bool> InsertPhotoAsync(Picture picture)
        {
            HashEntry[] imgData = new HashEntry[5];
            imgData[0] = new HashEntry("url", picture.Url);
            imgData[1] = new HashEntry("thumbnail_url", picture.Thumbnail_Url);
            imgData[2] = new HashEntry("user_id", picture.User_Id);
            imgData[3] = new HashEntry("votes", picture.Votes);
            imgData[4] = new HashEntry("total_rating", picture.Total_Rating);
            await _client.HashSetAsync($"pic_{picture.Id}", imgData);
            return await _client.SetAddAsync("photos", picture.Id);
        }

        public async Task<bool> InsertPhotosAsync(IEnumerable<Picture> pictures)
        {
            try
            {
                var imgKeys = new RedisValue[pictures.Count()];
                HashEntry[] imgData;
                Picture pic;
                for (int i = 0; i < pictures.Count(); i++)
                {
                    pic = pictures.ElementAt(i);
                    imgKeys[i] = pic.Id;
                    imgData = new HashEntry[5];
                    imgData[0] = new HashEntry("url", pic.Url);
                    imgData[1] = new HashEntry("thumbnail_url", pic.Thumbnail_Url);
                    imgData[2] = new HashEntry("user_id", pic.User_Id);
                    imgData[3] = new HashEntry("votes", pic.Votes);
                    imgData[4] = new HashEntry("total_rating", pic.Total_Rating);

                    await _client.HashSetAsync($"pic_{pic.Id}", imgData);
                }
                await _client.SetAddAsync("photos", imgKeys);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Task<IEnumerable<Vote>> GetVotesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Vote>> GetVotesByUserId(string userId)
        {
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
            if (vote != null)
            {
                return await _client.SetAddAsync(vote.User_Id, vote.Picture_Id);
            }
            return false;
        }

        public async Task<bool> InsertVotesAsync(IEnumerable<Vote> votes)
        {
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
    }
}
