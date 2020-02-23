using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using Xunit;

namespace Blacksmith.Sql.Tests
{
    public static class Connections
    {
        public static IDbConnection getSqlServerConnection()
        {
            return new SqlConnection(@"Data Source=(localdb)\v11.0;Initial Catalog=Blacksmith-sql-tests;Integrated Security=True;Pooling=False");
        }

        public static IDbConnection getSqliteConnection()
        {
            string databaseFileFullPath;

            databaseFileFullPath = Path.Combine(Environment.CurrentDirectory, "test.sqlite");
            Assert.True(File.Exists(databaseFileFullPath));

            return new SQLiteConnection($@"Data Source={databaseFileFullPath}; Version=3;");
        }
    }
}
