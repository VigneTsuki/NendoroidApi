using Microsoft.Extensions.Configuration;
using System.Data;
using System;
using MySql.Data.MySqlClient;

namespace NendoroidApi.Data.Base
{
    public sealed class DbSession : IDisposable
    {
        public IDbConnection Connection { get; }
        public IDbTransaction Transaction { get; set; }

        public DbSession(IConfiguration configuration)
        {
            Connection = new MySqlConnection(configuration.GetConnectionString("MySql"));
            Connection.Open();
        }

        public void Dispose() => Connection?.Dispose();
    }
}
