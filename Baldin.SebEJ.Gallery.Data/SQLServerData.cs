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
                               WHERE [Picture_Id] = @photoId";
                return conn.Query<Comment>(sql, new { photoId });
            }
        }

        public async Task<IEnumerable<Comment>> GetCommentsByPhotoIdAsync(int photoId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Comments]
                               WHERE [Picture_Id] = @photoId";
                return await conn.QueryAsync<Comment>(sql, new { photoId });
            }
        }

        public IEnumerable<Picture> GetPaginatedPictures(int index, int pageCount)
        {
            if (index < 1)
                index = 1;
           
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Pictures]
                               WHERE [Thumbnail_Url] IS NOT NULL
                               ORDER BY [dbo].[Id]
                               OFFSET ((@PageNumber - 1) * @RowspPage) ROWS
                               FETCH NEXT @RowspPage ROWS ONLY";
                return conn.Query<Picture>(sql, new { PageNumber=index, RowspPage=pageCount });
            }
        }

        public async Task<IEnumerable<Picture>> GetPaginatedPicturesAsync(int index, int pageCount)
        {
            if (index < 1)
                index = 1;

            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Pictures]
                               WHERE [Thumbnail_Url] IS NOT NULL
                               ORDER BY [dbo].[Id]
                               OFFSET ((@PageNumber - 1) * @RowspPage) ROWS
                               FETCH NEXT @RowspPage ROWS ONLY";
                return await conn.QueryAsync<Picture>(sql, new { PageNumber = index, RowspPage = pageCount });
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
                               WHERE [Url] = @Url
                               OR [Thumbnail_Url] = @Url";
                return conn.QueryFirstOrDefault<Picture>(sql, new { Url = url });
            }
        }

        public async Task<Picture> GetPictureByUrlAsync(string url)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Pictures]
                               WHERE [Url] = @Url
                               OR [Thumbnail_Url] = @Url";
                return await conn.QueryFirstOrDefaultAsync<Picture>(sql, new { Url = url });
            }
        }

        public long GetPictureCount()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT COUNT(*)
                               FROM [dbo].[Pictures]
                               WHERE [Thumbnail_Url] IS NOT NULL";
                return conn.ExecuteScalar<long>(sql);
            }
        }

        public async Task<long> GetPictureCountAsync()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT COUNT(*)
                               FROM [dbo].[Pictures]
                               WHERE [Thumbnail_Url] IS NOT NULL";
                return await conn.ExecuteScalarAsync<long>(sql);
            }
        }

        public IEnumerable<Picture> GetPictures()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Pictures]
                               WHERE [Thumbnail_Url] IS NOT NULL";
                return conn.Query<Picture>(sql);
            }
        }

        public async Task<IEnumerable<Picture>> GetPicturesAsync()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Pictures]
                               WHERE [Thumbnail_Url] IS NOT NULL";
                return await conn.QueryAsync<Picture>(sql);
            }
        }

        public IEnumerable<Picture> GetPicturesRangeById(int startId, int endId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Pictures]
                               WHERE [Id]
                               BETWEEN @startId AND @endId
                               AND [Thumbnail_Url] IS NOT NULL";
                return conn.Query<Picture>(sql, new { startId, endId });
            }
        }

        public async Task<IEnumerable<Picture>> GetPicturesRangeByIdAsync(int startId, int endId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Pictures]
                               WHERE [Id]
                               BETWEEN @startId AND @endId
                               AND [Thumbnail_Url] IS NOT NULL";
                return await conn.QueryAsync<Picture>(sql, new { startId, endId });
            }
        }

        public IEnumerable<Picture> GetRank(int topN = 3)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT TOP @top *
                                FROM [dbo].[Pictures]
                                WHERE [Votes] != 0
                                AND [Thumbnail_Url] IS NOT NULL
                                ORDER BY CAST(Total_Rating AS decimal) / Votes * 100 + Votes DESC";
                return conn.Query<Picture>(sql, new { top = topN });
            }
        }

        public async Task<IEnumerable<Picture>> GetRankAsync(int topN = 3)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT TOP @top *
                                FROM [dbo].[Pictures]
                                WHERE [Votes] != 0
                                AND [Thumbnail_Url] IS NOT NULL
                                ORDER BY CAST(Total_Rating AS decimal) / Votes * 100 + Votes DESC";
                return await conn.QueryAsync<Picture>(sql, new { top = topN });
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

        public int InsertComment(Comment comment)
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
                bool completed = conn.Execute(sql, comment) > 0;
                if (completed)
                {
                    sql = @"SELECT [Id] FROM [dbo].[Comments] 
                            WHERE [Author] = @Author AND [Text] = @Text AND [InsertDate] = @InsertDate";
                    return conn.QuerySingle<int>(sql, comment);
                }
                return -1;
            }
        }

        public async Task<int> InsertCommentAsync(Comment comment)
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
                bool completed = await conn.ExecuteAsync(sql, comment) > 0;
                if (completed)
                {
                    sql = @"SELECT [Id] FROM [dbo].[Comments] 
                            WHERE [Author] = @Author AND [Text] = @Text AND [InsertDate] = @InsertDate";
                    return await conn.QuerySingleAsync<int>(sql, comment);
                }
                return -1;
            }
        }

        public int InsertPicture(Picture picture)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"INSERT INTO [dbo].[Pictures]
                                    ([Name]
                                    ,[OriginalName]
                                    ,[User_Id]
                                    ,[Thumbnail_Url]
                                    ,[Url]
                                    ,[Votes]
                                    ,[Total_Rating])
                               VALUES
                                    (@Name
                                    ,@OriginalName
                                    ,@User_Id
                                    ,@Thumbnail_Url
                                    ,@Url
                                    ,@Votes
                                    ,@Total_Rating)";
                conn.Execute(sql, picture);
                sql = @"SELECT [Id] FROM [dbo].[Pictures] WHERE [Url] = @Url";
                return conn.ExecuteScalar<int>(sql, new { picture.Url });
            }
        }

        public async Task<int> InsertPictureAsync(Picture picture)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"INSERT INTO [dbo].[Pictures]
                                    ([Name]
                                    ,[OriginalName]
                                    ,[User_Id]
                                    ,[Thumbnail_Url]
                                    ,[Url]
                                    ,[Votes]
                                    ,[Total_Rating])
                               VALUES
                                    (@Name
                                    ,@OriginalName
                                    ,@User_Id
                                    ,@Thumbnail_Url
                                    ,@Url
                                    ,@Votes
                                    ,@Total_Rating)";
                await conn.ExecuteAsync(sql, picture);
                sql = @"SELECT [Id] FROM [dbo].[Pictures] WHERE [Url] = @Url";
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
                                  ,[Thumbnail_Url] = @Thumbnail_Url
                                  ,[OriginalName] = @OriginalName
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
                                  ,[Thumbnail_Url] = @Thumbnail_Url
                                  ,[OriginalName] = @OriginalName
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
