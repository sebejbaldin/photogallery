using Baldin.SebEJ.Gallery.Data.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

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

        public bool DeletePicture(int Id)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"DELETE FROM sebej_pictures 
                               WHERE id = @Id";
                return conn.Execute(sql, new { Id }) > 0;
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

        public Picture GetPicture(int Id)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"SELECT id AS Id
                               ,name AS Name
                               ,url AS Url
                               ,votes AS Votes
                               ,total_rating AS Total_Rating
                               FROM sebej_pictures
                               WHERE id = @Id";
                return conn.QueryFirstOrDefault<Picture>(sql, new { Id });
            }
        }

        public IEnumerable<Picture> GetPictures()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"SELECT id AS Id
                               ,name AS Name
                               ,url AS Url
                               ,votes AS Votes
                               ,total_rating AS Total_Rating
                               FROM sebej_pictures";
                return conn.Query<Picture>(sql);
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

        public bool InsertPicture(Picture picture)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                string sql = @"INSERT INTO sebej_pictures
                                    (name
                                    ,url
                                    ,votes
                                    ,total_rating)
                               VALUES
                                    (@Name
                                    ,@Url
                                    ,@Votes
                                    ,@Total_Rating)";
                return conn.Execute(sql, picture) > 0;
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
    }
}
