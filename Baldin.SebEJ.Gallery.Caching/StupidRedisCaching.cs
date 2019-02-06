using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Baldin.SebEJ.Gallery.Data.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Baldin.SebEJ.Gallery.Caching
{
    public class StupidRedisCaching : ICaching
    {
        private ConnectionMultiplexer _multiplexer;

        public StupidRedisCaching(IConfiguration config)
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
                ConnectTimeout = 2000,
                Password = redis["Password"],
                ReconnectRetryPolicy = new ExponentialRetry(7500),
                ConnectRetry = 2,
                AbortOnConnectFail = false
            };
            //string connString = $"{redis["Host"]},password={redis["Password"]}";
            _multiplexer = ConnectionMultiplexer.Connect(confRedis);
        }

        public async Task<Picture> GetPhotoAsync(int Id)
        {
            if (!_multiplexer.IsConnected)
                return null;
            var database = GetDatabase();
            var res = (await database.SortedSetRangeByScoreAsync("photos", Id, Id)).SingleOrDefault();
            if (!res.HasValue)
                return null;
            return JsonConvert.DeserializeObject<Picture>(res);
        }

        public async Task<IEnumerable<Picture>> GetPhotosAsync()
        {
            if (!_multiplexer.IsConnected)
                return null;
            var database = GetDatabase();
            var res = await database.SortedSetRangeByScoreAsync("photos");
            var list = new List<Picture>(res.Count());
            foreach (var item in res)
            {
                list.Add(JsonConvert.DeserializeObject<Picture>(item));
            }
            return list;
        }

        public async Task<IEnumerable<Picture>> GetPhotosByScoreAsync(int start, int end = -1)
        {
            if (!_multiplexer.IsConnected)
                return null;
            var database = GetDatabase();
            var res = await database.SortedSetRangeByRankAsync("photos", start, end);
            var list = new List<Picture>(res.Count());
            foreach (var item in res)
            {
                list.Add(JsonConvert.DeserializeObject<Picture>(item));
            }
            return list;
        }

        public async Task<IEnumerable<Picture>> GetRank(int topN = -1)
        {
            if (!_multiplexer.IsConnected)
                return null;
            var database = GetDatabase();
            var res = await database.SortedSetRangeByRankAsync("ranking", 0, topN, Order.Descending);
            var rankRes = new List<string>();
            foreach (var item in res)
            {
                if (int.TryParse(item, out int conv))
                {
                    rankRes.Add((await database.SortedSetRangeByScoreAsync("photos", conv, conv)).SingleOrDefault());
                }
            }
            var list = new List<Picture>(res.Count());
            foreach (var item in rankRes)
            {
                list.Add(JsonConvert.DeserializeObject<Picture>(item));
            }
            return list;
        }

        public async Task<IEnumerable<int>> GetVotesByUserId(string userId)
        {
            if (!_multiplexer.IsConnected)
                return null;
            var db = GetDatabase();
            var votes = await db.SetMembersAsync(userId);
            return votes.Select(x => (int)x);
        }

        public async Task<bool> InsertPhotoAsync(Picture picture)
        {
            if (!_multiplexer.IsConnected)
                return false;
            string pic = JsonConvert.SerializeObject(picture);
            var database = GetDatabase();
            bool status = await database.SortedSetAddAsync("photos", pic, picture.Id);
            return status && (await database.SortedSetAddAsync("ranking", picture.Id, GetRedisRank(picture)));
        }

        public async Task<bool> InsertPhotosAsync(IEnumerable<Picture> pictures)
        {
            if (!_multiplexer.IsConnected)
                return false;
            long counter = pictures.Count();
            var picsJSON = new SortedSetEntry[counter];
            var rank = new SortedSetEntry[counter];
            Picture pic = null;
            for (int i = 0; i < counter; i++)
            {
                pic = pictures.ElementAt(i);
                picsJSON[i] = new SortedSetEntry(JsonConvert.SerializeObject(pic), pic.Id);
                rank[i] = new SortedSetEntry(pic.Id, GetRedisRank(pic));
            }
            var database = GetDatabase();

            return (await database.SortedSetAddAsync("photos", picsJSON)) > 0 
                && (await database.SortedSetAddAsync("ranking", rank)) > 0;
        }

        public async Task<bool> InsertVoteAsync(Vote vote)
        {
            if (!_multiplexer.IsConnected)
                return false;
            return await GetDatabase().SetAddAsync(vote.User_Id, vote.Picture_Id);
        }

        public async Task<bool> InsertVotesAsync(IEnumerable<Vote> votes)
        {
            if (!_multiplexer.IsConnected)
                return false;
            var groups = votes.GroupBy(x => x.User_Id);
            bool result = true;
            var database = GetDatabase();
            foreach (var group in groups)
            {
                var picIds = group.Select(x => (RedisValue)x.Picture_Id).ToArray();
                result = result && await database.SetAddAsync(group.Key, picIds) > 0;
            }
            return result;
        }

        public bool IsConnected()
        {
            return _multiplexer.IsConnected;
        }

        public async Task<bool> UpdatePictureAsync(Picture picture)
        {
            if (!_multiplexer.IsConnected)
                return false;
            var database = GetDatabase();
            var result = (await database.SortedSetRemoveRangeByScoreAsync("photos", picture.Id, picture.Id)) > 0;
            result = result && await database.SortedSetRemoveAsync("ranking", picture.Id);
            result = result && await database.SortedSetAddAsync("photos", JsonConvert.SerializeObject(picture), picture.Id);
            result = result && await database.SortedSetAddAsync("ranking", picture.Id, GetRedisRank(picture));
            return result;
        }

        private IDatabase GetDatabase()
        {
            return _multiplexer.GetDatabase();
        }

        private double GetRedisRank(Picture pic)
        {
            return pic.Rating * 100 + pic.Votes;
        }
    }
}
