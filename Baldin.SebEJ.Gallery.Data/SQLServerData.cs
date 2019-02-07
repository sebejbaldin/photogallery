using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Baldin.SebEJ.Gallery.Data.Models;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Baldin.SebEJ.Gallery.Data
{
    public class SQLServerData : IDataAccess
    {
        private string ConnectionString;

        public SQLServerData(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public SQLServerData(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public bool DeleteComment(int Id)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"DELETE FROM [dbo].[Comments]
                               WHERE [Id] = @Id";
                return conn.Execute(sql, new { Id }) > 0;
            }
        }

        public async Task<bool> DeleteCommentAsync(int Id)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"DELETE FROM [dbo].[Comments]
                               WHERE [Id] = @Id";
                return await conn.ExecuteAsync(sql, new { Id }) > 0;
            }
        }

        public bool DeletePicture(int Id)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"DELETE FROM [dbo].[Pictures] 
                               WHERE [Id] = @Id";
                return conn.Execute(sql, new { Id }) > 0;
            }
        }

        public async Task<bool> DeletePictureAsync(int Id)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"DELETE FROM [dbo].[Pictures] 
                               WHERE [Id] = @Id";
                return await conn.ExecuteAsync(sql, new { Id }) > 0;
            }
        }

        public bool DeleteVote(int Id)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"DELETE FROM [dbo].[Votes] 
                               WHERE [Picture_Id] = @Id";
                return conn.Execute(sql, new { Id }) > 0;
            }
        }

        public async Task<bool> DeleteVoteAsync(int Id)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"DELETE FROM [dbo].[Votes] 
                               WHERE [Picture_Id] = @Id";
                return await conn.ExecuteAsync(sql, new { Id }) > 0;
            }
        }

        public Comment GetComment(int Id)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Comments]
                               WHERE [Id] = @Id";
                return conn.QuerySingleOrDefault<Comment>(sql, new { Id });
            }
        }

        public async Task<Comment> GetCommentAsync(int Id)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Comments]
                               WHERE [Id] = @Id";
                return await conn.QuerySingleOrDefaultAsync<Comment>(sql, new { Id });
            }
        }

        public IEnumerable<Comment> GetComments()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Comments]";
                return conn.Query<Comment>(sql);
            }
        }

        public async Task<IEnumerable<Comment>> GetCommentsAsync()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Comments]";
                return await conn.QueryAsync<Comment>(sql);
            }
        }

        public IEnumerable<Comment> GetCommentsByPhotoId(int photoId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Comments]
                               WHERE Picture_Id = @photoId";
                return conn.Query<Comment>(sql, new { photoId });
            }
        }

        public async Task<IEnumerable<Comment>> GetCommentsByPhotoIdAsync(int photoId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Comments]
                               WHERE Picture_Id = @photoId";
                return await conn.QueryAsync<Comment>(sql, new { photoId });
            }
        }

        public Picture GetPicture(int Id)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Pictures]
                               WHERE [Id] = @Id";
                return conn.QueryFirstOrDefault<Picture>(sql, new { Id });
            }
        }

        public async Task<Picture> GetPictureAsync(int Id)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Pictures]
                               WHERE [Id] = @Id";
                return await conn.QueryFirstOrDefaultAsync<Picture>(sql, new { Id });
            }
        }

        public Picture GetPictureByUrl(string url)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Pictures]
                               WHERE [Url] = @Url";
                return conn.QueryFirstOrDefault<Picture>(sql, new { Url = url });
            }
        }

        public async Task<Picture> GetPictureByUrlAsync(string url)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Pictures]
                               WHERE [Url] = @Url";
                return await conn.QueryFirstOrDefaultAsync<Picture>(sql, new { Url = url });
            }
        }

        public IEnumerable<Picture> GetPictures()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Pictures]";
                return conn.Query<Picture>(sql);
            }
        }

        public async Task<IEnumerable<Picture>> GetPicturesAsync()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Pictures]";
                return await conn.QueryAsync<Picture>(sql);
            }
        }

        public IEnumerable<Picture> GetPicturesRangeById(int startId, int endId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Pictures]
                               WHERE Id
                               BETWEEN @startId AND @endId";
                return conn.Query<Picture>(sql, new { startId, endId });
            }
        }

        public async Task<IEnumerable<Picture>> GetPicturesRangeByIdAsync(int startId, int endId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Pictures]
                               WHERE Id
                               BETWEEN @startId AND @endId";
                return await conn.QueryAsync<Picture>(sql, new { startId, endId });
            }
        }

        public IEnumerable<Vote> GetVotes()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Votes]";
                return conn.Query<Vote>(sql);
            }
        }

        public async Task<IEnumerable<Vote>> GetVotesAsync()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Votes]";
                return await conn.QueryAsync<Vote>(sql);
            }
        }

        public IEnumerable<Vote> GetVotesByUserId(string userId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Votes]
                               WHERE [User_Id] = @Id";
                return conn.Query<Vote>(sql, new { Id = userId });
            }
        }

        public async Task<IEnumerable<Vote>> GetVotesByUserIdAsync(string userId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Votes]
                               WHERE [User_Id] = @Id";
                return await conn.QueryAsync<Vote>(sql, new { Id = userId });
            }
        }

        public bool InsertComment(Comment comment)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"INSERT INTO [dbo].[Comments]
                                    ([Picture_Id]
                                    ,[Author]
                                    ,[Email]
                                    ,[Text]
                                    ,[InsertDate])
                                VALUES
                                    (@Picture_Id
                                    ,@Author
                                    ,@Email
                                    ,@Text
                                    ,@InsertDate)";
                return conn.Execute(sql, comment) > 0;
            }
        }

        public async Task<bool> InsertCommentAsync(Comment comment)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"INSERT INTO [dbo].[Comments]
                                    ([Picture_Id]
                                    ,[Author]
                                    ,[Email]
                                    ,[Text]
                                    ,[InsertDate])
                                VALUES
                                    (@Picture_Id
                                    ,@Author
                                    ,@Email
                                    ,@Text
                                    ,@InsertDate)";
                return await conn.ExecuteAsync(sql, comment) > 0;
            }
        }

        public int InsertPicture(Picture picture)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"INSERT INTO [dbo].[Pictures]
                                    ([Name]
                                    ,[Url]
                                    ,[Votes]
                                    ,[Total_Rating])
                               VALUES
                                    (@Name
                                    ,@Url
                                    ,@Votes
                                    ,@Total_Rating)";
                conn.Execute(sql, picture);
                sql = @"SELECT Id FROM [dbo].[Pictures] WHERE Url = @Url";
                return conn.ExecuteScalar<int>(sql, new { picture.Url });
            }
        }

        public async Task<int> InsertPictureAsync(Picture picture)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"INSERT INTO [dbo].[Pictures]
                                    ([Name]
                                    ,[Url]
                                    ,[Votes]
                                    ,[Total_Rating])
                               VALUES
                                    (@Name
                                    ,@Url
                                    ,@Votes
                                    ,@Total_Rating)";
                await conn.ExecuteAsync(sql, picture);
                sql = @"SELECT Id FROM [dbo].[Pictures] WHERE Url = @Url";
                return await conn.ExecuteScalarAsync<int>(sql, new { picture.Url });
            }
        }

        public bool InsertVote(Vote vote)
        {
            bool CVoted;
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (var trans = conn.BeginTransaction())
                {
                    string insVote = @"INSERT INTO [dbo].[Votes]
                                    ([User_Id]
                                    ,[Picture_Id]
                                    ,[Rating])
                               VALUES
                                    (@User_Id
                                    ,@Picture_Id
                                    ,@Rating)";
                    CVoted = conn.Execute(insVote, vote, trans) > 0;
                    string updateImage = @"UPDATE [dbo].[Pictures]
                                           SET [Votes] = [Votes] + 1
                                              ,[Total_Rating] = [Total_Rating] + @Rating
                                           WHERE Id = @Picture_Id";
                    CVoted = CVoted && conn.Execute(updateImage, vote, trans) > 0;
                    trans.Commit();
                }
                return CVoted;
            }
        }

        public async Task<bool> InsertVoteAsync(Vote vote)
        {
            bool CVoted;
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (var trans = conn.BeginTransaction())
                {
                    string insVote = @"INSERT INTO [dbo].[Votes]
                                    ([User_Id]
                                    ,[Picture_Id]
                                    ,[Rating])
                               VALUES
                                    (@User_Id
                                    ,@Picture_Id
                                    ,@Rating)";
                    CVoted = await conn.ExecuteAsync(insVote, vote, trans) > 0;
                    string updateImage = @"UPDATE [dbo].[Pictures]
                                           SET [Votes] = [Votes] + 1
                                              ,[Total_Rating] = [Total_Rating] + @Rating
                                           WHERE Id = @Picture_Id";
                    CVoted = CVoted && await conn.ExecuteAsync(updateImage, vote, trans) > 0;
                    trans.Commit();
                }
                return CVoted;
            }
        }

        public bool UpdateComment(Comment comment)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"UPDATE [dbo].[Comments]
                                SET [Text] = @Text
                                WHERE Id = @Id";
                return conn.Execute(sql, comment) > 0;
            }
        }

        public async Task<bool> UpdateCommentAsync(Comment comment)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"UPDATE [dbo].[Comments]
                                SET [Text] = @Text
                                WHERE Id = @Id";
                return await conn.ExecuteAsync(sql, comment) > 0;
            }
        }

        public bool UpdatePicture(Picture picture)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"UPDATE [dbo].[Pictures]
                               SET [Name] = @Name
                                  ,[Url] = @Url
                                  ,[Votes] = @Votes
                                  ,[Total_Rating] = @Total_Rating
                               WHERE [Id] = @Id";
                return conn.Execute(sql, picture) > 0;
            }
        }

        public async Task<bool> UpdatePictureAsync(Picture picture)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"UPDATE [dbo].[Pictures]
                               SET [Name] = @Name
                                  ,[Url] = @Url
                                  ,[Votes] = @Votes
                                  ,[Total_Rating] = @Total_Rating
                               WHERE [Id] = @Id";
                return await conn.ExecuteAsync(sql, picture) > 0;
            }
        }

        public bool UpdateVote(Vote vote)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"UPDATE [dbo].[Votes]
                               SET [Vote] = @Vote
                               WHERE [User_Id] = @User_Id AND [Picture_Id] = @Picture_Id";
                return conn.Execute(sql, vote) > 0;
            }
        }

        public async Task<bool> UpdateVoteAsync(Vote vote)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"UPDATE [dbo].[Votes]
                               SET [Vote] = @Vote
                               WHERE [User_Id] = @User_Id AND [Picture_Id] = @Picture_Id";
                return await conn.ExecuteAsync(sql, vote) > 0;
            }
        }
    }
}
