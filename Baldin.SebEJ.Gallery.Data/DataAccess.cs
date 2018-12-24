using System;
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
    }
}
