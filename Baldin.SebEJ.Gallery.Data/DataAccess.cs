using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Baldin.SebEJ.Gallery.Data.Models;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Baldin.SebEJ.Gallery.Data
{
    public class DataAccess : IDataAccess
    {
        private string ConnectionString;

        public DataAccess(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public DataAccess(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
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

        public bool DeleteVote(int Id)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"DELETE FROM [dbo].[Votes] 
                               WHERE [Picture_Id] = @Id";
                return conn.Execute(sql, new { Id }) > 0;
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

        public IEnumerable<Picture> GetPictures()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string sql = @"SELECT *
                               FROM [dbo].[Pictures]";
                return conn.Query<Picture>(sql);
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

        public bool InsertPicture(Picture picture)
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
                return conn.Execute(sql, picture) > 0;
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
    }
}
