using Baldin.SebEJ.Gallery.Data.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Baldin.SebEJ.Gallery.Data
{
    public class PgSQLData : IDataAccess
    {
        private string ConnectionString;

        public PgSQLData(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public PgSQLData(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("PgSQLConnOnline");
        }

        public bool DeleteComment(int Id)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"DELETE FROM sebej_comments
                               WHERE id = @Id";
                return conn.Execute(sql, new { Id }) > 0;
            }
        }

        public async Task<bool> DeleteCommentAsync(int Id)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"DELETE FROM sebej_comments
                               WHERE id = @Id";
                return await conn.ExecuteAsync(sql, new { Id }) > 0;
            }
        }

        public bool DeletePicture(int Id)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"DELETE FROM sebej_pictures 
                               WHERE id = @Id";
                return conn.Execute(sql, new { Id }) > 0;
            }
        }

        public async Task<bool> DeletePictureAsync(int Id)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"DELETE FROM sebej_pictures 
                               WHERE id = @Id";
                return await conn.ExecuteAsync(sql, new { Id }) > 0;
            }
        }

        public bool DeleteVote(int Id)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"DELETE FROM sebej_votes 
                               WHERE picture_id = @Id";
                return conn.Execute(sql, new { Id }) > 0;
            }
        }

        public async Task<bool> DeleteVoteAsync(int Id)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"DELETE FROM sebej_votes 
                               WHERE picture_id = @Id";
                return await conn.ExecuteAsync(sql, new { Id }) > 0;
            }
        }

        public Comment GetComment(int Id)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"SELECT id AS Id
                               ,picture_id AS Picture_Id
                               ,author AS Author
                               ,email  AS Email
                               ,text AS Text
                               ,insert_date AS InsertDate
                               FROM sebej_comments
                               WHERE id = @Id";
                return conn.QuerySingleOrDefault<Comment>(sql, new { Id });
            }
        }

        public async Task<Comment> GetCommentAsync(int Id)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"SELECT id AS Id
                               ,picture_id AS Picture_Id
                               ,author AS Author
                               ,email  AS Email
                               ,text AS Text
                               ,insert_date AS InsertDate
                               FROM sebej_comments
                               WHERE id = @Id";
                return await conn.QuerySingleOrDefaultAsync<Comment>(sql, new { Id });
            }
        }

        public IEnumerable<Comment> GetComments()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"SELECT id AS Id
                               ,picture_id AS Picture_Id
                               ,author AS Author
                               ,email  AS Email
                               ,text AS Text
                               ,insert_date AS InsertDate
                               FROM sebej_comments";
                return conn.Query<Comment>(sql);
            }
        }

        public async Task<IEnumerable<Comment>> GetCommentsAsync()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"SELECT id AS Id
                               ,picture_id AS Picture_Id
                               ,author AS Author
                               ,email  AS Email
                               ,text AS Text
                               ,insert_date AS InsertDate
                               FROM sebej_comments";
                return await conn.QueryAsync<Comment>(sql);
            }
        }

        public IEnumerable<Comment> GetCommentsByPhotoId(int photoId)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"SELECT id AS Id
                               ,picture_id AS Picture_Id
                               ,author AS Author
                               ,email  AS Email
                               ,text AS Text
                               ,insert_date AS InsertDate
                               FROM sebej_comments
                               WHERE picture_id = @photoId";
                return conn.Query<Comment>(sql, new { photoId });
            }
        }

        public async Task<IEnumerable<Comment>> GetCommentsByPhotoIdAsync(int photoId)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"SELECT id AS Id
                               ,picture_id AS Picture_Id
                               ,author AS Author
                               ,email  AS Email
                               ,text AS Text
                               ,insert_date AS InsertDate
                               FROM sebej_comments
                               WHERE picture_id = @photoId";
                return await conn.QueryAsync<Comment>(sql, new { photoId });
            }
        }

        public Picture GetPicture(int Id)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"SELECT id AS Id
                               ,user_id AS User_Id
                               ,name AS Name
                               ,thumbnail_url AS Thumbnail_Url
                               ,url AS Url
                               ,votes AS Votes
                               ,total_rating AS Total_Rating
                               FROM sebej_pictures
                               WHERE id = @Id";
                return conn.QueryFirstOrDefault<Picture>(sql, new { Id });
            }
        }

        public async Task<Picture> GetPictureAsync(int Id)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"SELECT id AS Id
                               ,user_id AS User_Id
                               ,name AS Name
                               ,thumbnail_url AS Thumbnail_Url
                               ,url AS Url
                               ,votes AS Votes
                               ,total_rating AS Total_Rating
                               FROM sebej_pictures
                               WHERE id = @Id";
                return await conn.QueryFirstOrDefaultAsync<Picture>(sql, new { Id });
            }
        }

        public Picture GetPictureByUrl(string url)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"SELECT id AS Id
                               ,user_id AS User_Id
                               ,name AS Name
                               ,thumbnail_url AS Thumbnail_Url
                               ,url AS Url
                               ,votes AS Votes
                               ,total_rating AS Total_Rating
                               FROM sebej_pictures
                               WHERE url = @Url";
                return conn.QueryFirstOrDefault<Picture>(sql, new { Url = url });
            }
        }

        public async Task<Picture> GetPictureByUrlAsync(string url)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"SELECT id AS Id
                               ,user_id AS User_Id
                               ,name AS Name
                               ,thumbnail_url AS Thumbnail_Url
                               ,url AS Url
                               ,votes AS Votes
                               ,total_rating AS Total_Rating
                               FROM sebej_pictures
                               WHERE url = @Url";
                return await conn.QueryFirstOrDefaultAsync<Picture>(sql, new { Url = url });
            }
        }

        public IEnumerable<Picture> GetPictures()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"SELECT id AS Id
                               ,user_id AS User_Id
                               ,name AS Name
                               ,thumbnail_url AS Thumbnail_Url
                               ,url AS Url
                               ,votes AS Votes
                               ,total_rating AS Total_Rating
                               FROM sebej_pictures";
                return conn.Query<Picture>(sql);
            }
        }

        public async Task<IEnumerable<Picture>> GetPicturesAsync()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"SELECT id AS Id
                               ,user_id AS User_Id
                               ,name AS Name
                               ,thumbnail_url AS Thumbnail_Url
                               ,url AS Url
                               ,votes AS Votes
                               ,total_rating AS Total_Rating
                               FROM sebej_pictures";
                return await conn.QueryAsync<Picture>(sql);
            }
        }

        public IEnumerable<Vote> GetVotes()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"SELECT user_id AS User_Id
                               ,picture_id AS Picture_Id
                               ,rating AS Rating
                               FROM sebej_votes";
                return conn.Query<Vote>(sql);
            }
        }

        public async Task<IEnumerable<Vote>> GetVotesAsync()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"SELECT user_id AS User_Id
                               ,picture_id AS Picture_Id
                               ,rating AS Rating
                               FROM sebej_votes";
                return await conn.QueryAsync<Vote>(sql);
            }
        }

        public IEnumerable<Vote> GetVotesByUserId(string userId)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"SELECT user_id AS User_Id
                               ,picture_id AS Picture_Id
                               ,rating AS Rating
                               FROM sebej_votes
                               WHERE user_id = @Id";
                return conn.Query<Vote>(sql, new { Id = userId });
            }
        }

        public async Task<IEnumerable<Vote>> GetVotesByUserIdAsync(string userId)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"SELECT user_id AS User_Id
                               ,picture_id AS Picture_Id
                               ,rating AS Rating
                               FROM sebej_votes
                               WHERE user_id = @Id";
                return await conn.QueryAsync<Vote>(sql, new { Id = userId });
            }
        }

        public bool InsertComment(Comment comment)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"INSERT INTO sebej_comments
                                    (picture_id
                                    ,author
                                    ,email
                                    ,text
                                    ,insert_date)
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
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"INSERT INTO sebej_comments
                                    (picture_id
                                    ,author
                                    ,email
                                    ,text
                                    ,insert_date)
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
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"INSERT INTO sebej_pictures
                                    (name
                                    ,user_id
                                    ,url
                                    ,thumbnail_url
                                    ,votes
                                    ,total_rating)
                               VALUES
                                    (@Name
                                    ,@User_Id
                                    ,@Url
                                    ,@Thumbnail_Url
                                    ,@Votes
                                    ,@Total_Rating)";
                conn.Execute(sql, picture);
                sql = @"SELECT id FROM sebej_pictures WHERE url = @Url";
                return conn.ExecuteScalar<int>(sql, new { picture.Url });
            }
        }

        public async Task<int> InsertPictureAsync(Picture picture)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"INSERT INTO sebej_pictures
                                    (name
                                    ,user_id
                                    ,url
                                    ,thumbnail_url
                                    ,votes
                                    ,total_rating)
                               VALUES
                                    (@Name
                                    ,@User_Id
                                    ,@Url
                                    ,@Thumbnail_Url
                                    ,@Votes
                                    ,@Total_Rating)";
                await conn.ExecuteAsync(sql, picture);
                sql = @"SELECT id FROM sebej_pictures WHERE url = @Url";
                return await conn.ExecuteScalarAsync<int>(sql, new { picture.Url });
            }
        }

        public bool InsertVote(Vote vote)
        {
            bool CVoted;
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var trans = conn.BeginTransaction())
                {
                    string insVote = @"INSERT INTO sebej_votes
                                    (user_id
                                    ,picture_id
                                    ,rating)
                               VALUES
                                    (@User_Id
                                    ,@Picture_Id
                                    ,@Rating)";
                    CVoted = conn.Execute(insVote, vote, trans) > 0;
                    string updateImage = @"UPDATE sebej_pictures
                                           SET votes = votes + 1
                                              ,total_rating = total_rating + @Rating
                                           WHERE id = @Picture_Id";
                    CVoted = CVoted && conn.Execute(updateImage, vote, trans) > 0;
                    trans.Commit();
                }
                return CVoted;
            }
        }

        public async Task<bool> InsertVoteAsync(Vote vote)
        {
            bool CVoted;
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var trans = conn.BeginTransaction())
                {
                    string insVote = @"INSERT INTO sebej_votes
                                    (user_id
                                    ,picture_id
                                    ,rating)
                               VALUES
                                    (@User_Id
                                    ,@Picture_Id
                                    ,@Rating)";
                    CVoted = await conn.ExecuteAsync(insVote, vote, trans) > 0;
                    string updateImage = @"UPDATE sebej_pictures
                                           SET votes = votes + 1
                                              ,total_rating = total_rating + @Rating
                                           WHERE id = @Picture_Id";
                    CVoted = CVoted && await conn.ExecuteAsync(updateImage, vote, trans) > 0;
                    trans.Commit();
                }
                return CVoted;
            }
        }

        public bool UpdateComment(Comment comment)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"UPDATE sebej_comments
                                SET text = @Text
                                WHERE id = @Id";
                return conn.Execute(sql, comment) > 0;
            }
        }

        public async Task<bool> UpdateCommentAsync(Comment comment)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"UPDATE sebej_comments
                                SET text = @Text
                                WHERE id = @Id";
                return await conn.ExecuteAsync(sql, comment) > 0;
            }
        }

        public bool UpdatePicture(Picture picture)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"UPDATE sebej_pictures
                               SET name = @Name
                                  ,url = @Url
                                  ,votes = @Votes
                                  ,total_rating = @Total_Rating
                               WHERE id = @Id";
                return conn.Execute(sql, picture) > 0;
            }
        }

        public async Task<bool> UpdatePictureAsync(Picture picture)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"UPDATE sebej_pictures
                               SET name = @Name
                                  ,url = @Url
                                  ,votes = @Votes
                                  ,total_rating = @Total_Rating
                               WHERE id = @Id";
                return await conn.ExecuteAsync(sql, picture) > 0;
            }
        }

        public bool UpdateVote(Vote vote)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"UPDATE sebej_votes
                               SET vote = @Vote
                               WHERE user_id = @User_Id AND picture_id = @Picture_Id";
                return conn.Execute(sql, vote) > 0;
            }
        }

        public async Task<bool> UpdateVoteAsync(Vote vote)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"UPDATE sebej_votes
                               SET vote = @Vote
                               WHERE user_id = @User_Id AND picture_id = @Picture_Id";
                return await conn.ExecuteAsync(sql, vote) > 0;
            }
        }
    }
}
