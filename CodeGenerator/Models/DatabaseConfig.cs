using System;
using System.Collections.Generic;

namespace CodeGenerator.Models
{
    public enum DatabaseType
    {
        SqlServer,
        MySQL,
        PostgreSQL,
        SQLite
    }

    public class DatabaseConfig
    {
        public string ConnectionString { get; set; }
        public DatabaseType DatabaseType { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        
        public Dictionary<string, List<TableInfo>> DatabaseSchema { get; set; } = new Dictionary<string, List<TableInfo>>();

        public string GetConnectionString()
        {
            switch (DatabaseType)
            {
                case DatabaseType.SqlServer:
                    return $"Server={Server},{Port};Database={Database};User Id={Username};Password={Password};TrustServerCertificate=True;";
                case DatabaseType.MySQL:
                    return $"Server={Server};Port={Port};Database={Database};Uid={Username};Pwd={Password};";
                case DatabaseType.PostgreSQL:
                    return $"Host={Server};Port={Port};Database={Database};Username={Username};Password={Password};";
                case DatabaseType.SQLite:
                    return $"Data Source={Database};";
                default:
                    throw new ArgumentException("未支持的数据库类型");
            }
        }
    }
} 