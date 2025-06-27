using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using CodeGenerator.Models;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using Npgsql;
using Microsoft.Data.Sqlite;

namespace CodeGenerator.Services
{
    public class DatabaseService
    {
        public async Task<bool> TestConnectionAsync(DatabaseConfig config)
        {
            try
            {
                using (var connection = GetConnection(config))
                {
                    await connection.OpenAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<string>> GetDatabasesAsync(DatabaseConfig config)
        {
            var databases = new List<string>();
            
            try
            {
                using (var connection = GetConnection(config))
                {
                    await connection.OpenAsync();
                    
                    string query = GetDatabasesQuery(config.DatabaseType);
                    
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query;
                        
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                databases.Add(reader.GetString(0));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"获取数据库列表时出错: {ex.Message}", ex);
            }
            
            return databases;
        }

        public async Task<List<TableInfo>> GetTablesAsync(DatabaseConfig config)
        {
            var tables = new List<TableInfo>();
            
            try
            {
                using (var connection = GetConnection(config))
                {
                    await connection.OpenAsync();
                    
                    string query = GetTablesQuery(config.DatabaseType);
                    
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query;
                        
                        if (config.DatabaseType != DatabaseType.SQLite)
                        {
                            var parameter = command.CreateParameter();
                            parameter.ParameterName = GetDatabaseParameterName(config.DatabaseType);
                            parameter.Value = config.Database;
                            command.Parameters.Add(parameter);
                        }
                        
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string schema = config.DatabaseType == DatabaseType.SQLite ? "main" : reader.GetString(0);
                                string tableName = reader.GetString(1);
                                
                                tables.Add(new TableInfo(tableName, schema));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"获取表列表时出错: {ex.Message}", ex);
            }
            
            return tables;
        }

        public async Task<List<ColumnInfo>> GetColumnsAsync(DatabaseConfig config, string tableName, string schemaName)
        {
            var columns = new List<ColumnInfo>();
            
            try
            {
                using (var connection = GetConnection(config))
                {
                    await connection.OpenAsync();
                    
                    string query = GetColumnsQuery(config.DatabaseType);
                    
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query;
                        
                        var tableParam = command.CreateParameter();
                        tableParam.ParameterName = "@TableName";
                        tableParam.Value = tableName;
                        command.Parameters.Add(tableParam);
                        
                        if (config.DatabaseType != DatabaseType.SQLite)
                        {
                            var schemaParam = command.CreateParameter();
                            schemaParam.ParameterName = "@SchemaName";
                            schemaParam.Value = schemaName;
                            command.Parameters.Add(schemaParam);
                            
                            if (config.DatabaseType != DatabaseType.PostgreSQL)
                            {
                                var dbParam = command.CreateParameter();
                                dbParam.ParameterName = GetDatabaseParameterName(config.DatabaseType);
                                dbParam.Value = config.Database;
                                command.Parameters.Add(dbParam);
                            }
                        }
                        
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var column = new ColumnInfo
                                {
                                    Name = reader.GetString(reader.GetOrdinal("COLUMN_NAME")),
                                    DisplayName = reader.GetString(reader.GetOrdinal("COLUMN_NAME")),
                                    DbType = reader.GetString(reader.GetOrdinal("DATA_TYPE")),
                                    IsNullable = reader.GetString(reader.GetOrdinal("IS_NULLABLE")) == "YES",
                                    CSharpType = MapDbTypeToClrType(reader.GetString(reader.GetOrdinal("DATA_TYPE")), reader.GetString(reader.GetOrdinal("IS_NULLABLE")) == "YES")
                                };
                                
                                if (!reader.IsDBNull(reader.GetOrdinal("CHARACTER_MAXIMUM_LENGTH")))
                                {
                                    column.MaxLength = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("CHARACTER_MAXIMUM_LENGTH")));
                                }
                                
                                if (!reader.IsDBNull(reader.GetOrdinal("NUMERIC_PRECISION")))
                                {
                                    column.Precision = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("NUMERIC_PRECISION")));
                                }
                                
                                if (!reader.IsDBNull(reader.GetOrdinal("NUMERIC_SCALE")))
                                {
                                    column.Scale = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("NUMERIC_SCALE")));
                                }
                                
                                // Set appropriate control type based on DB type
                                column.ControlType = GetDefaultControlType(column.DbType);
                                
                                columns.Add(column);
                            }
                        }
                    }
                    
                    // Get primary keys
                    await GetPrimaryKeysAsync(connection, columns, config.DatabaseType, tableName, schemaName, config.Database);
                    
                    // Get identity columns (auto-increment)
                    await GetIdentityColumnsAsync(connection, columns, config.DatabaseType, tableName, schemaName, config.Database);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"获取列信息时出错: {ex.Message}", ex);
            }
            
            return columns;
        }

        private async Task GetPrimaryKeysAsync(DbConnection connection, List<ColumnInfo> columns, DatabaseType dbType, string tableName, string schemaName, string databaseName)
        {
            string query = GetPrimaryKeysQuery(dbType);
            
            if (string.IsNullOrEmpty(query))
                return;
            
            using (var command = connection.CreateCommand())
            {
                command.CommandText = query;
                
                var tableParam = command.CreateParameter();
                tableParam.ParameterName = "@TableName";
                tableParam.Value = tableName;
                command.Parameters.Add(tableParam);
                
                if (dbType != DatabaseType.SQLite)
                {
                    var schemaParam = command.CreateParameter();
                    schemaParam.ParameterName = "@SchemaName";
                    schemaParam.Value = schemaName;
                    command.Parameters.Add(schemaParam);
                    
                    if (dbType != DatabaseType.PostgreSQL)
                    {
                        var dbParam = command.CreateParameter();
                        dbParam.ParameterName = GetDatabaseParameterName(dbType);
                        dbParam.Value = databaseName;
                        command.Parameters.Add(dbParam);
                    }
                }
                
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string columnName = reader.GetString(reader.GetOrdinal("COLUMN_NAME"));
                        
                        var column = columns.Find(c => c.Name == columnName);
                        if (column != null)
                        {
                            column.IsPrimaryKey = true;
                        }
                    }
                }
            }
        }

        private async Task GetIdentityColumnsAsync(DbConnection connection, List<ColumnInfo> columns, DatabaseType dbType, string tableName, string schemaName, string databaseName)
        {
            string query = GetIdentityColumnsQuery(dbType);
            
            if (string.IsNullOrEmpty(query))
                return;
            
            using (var command = connection.CreateCommand())
            {
                command.CommandText = query;
                
                var tableParam = command.CreateParameter();
                tableParam.ParameterName = "@TableName";
                tableParam.Value = tableName;
                command.Parameters.Add(tableParam);
                
                if (dbType != DatabaseType.SQLite)
                {
                    var schemaParam = command.CreateParameter();
                    schemaParam.ParameterName = "@SchemaName";
                    schemaParam.Value = schemaName;
                    command.Parameters.Add(schemaParam);
                    
                    if (dbType != DatabaseType.PostgreSQL)
                    {
                        var dbParam = command.CreateParameter();
                        dbParam.ParameterName = GetDatabaseParameterName(dbType);
                        dbParam.Value = databaseName;
                        command.Parameters.Add(dbParam);
                    }
                }
                
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string columnName = reader.GetString(0);
                        
                        var column = columns.Find(c => c.Name == columnName);
                        if (column != null)
                        {
                            column.IsIdentity = true;
                        }
                    }
                }
            }
        }

        private DbConnection GetConnection(DatabaseConfig config)
        {
            DbConnection connection;
            
            switch (config.DatabaseType)
            {
                case DatabaseType.SqlServer:
                    connection = new SqlConnection(config.GetConnectionString());
                    break;
                
                case DatabaseType.MySQL:
                    connection = new MySqlConnection(config.GetConnectionString());
                    break;
                
                case DatabaseType.PostgreSQL:
                    connection = new NpgsqlConnection(config.GetConnectionString());
                    break;
                
                case DatabaseType.SQLite:
                    connection = new SqliteConnection(config.GetConnectionString());
                    break;
                
                default:
                    throw new ArgumentException("未支持的数据库类型");
            }
            
            return connection;
        }

        private string GetDatabasesQuery(DatabaseType dbType)
        {
            switch (dbType)
            {
                case DatabaseType.SqlServer:
                    return "SELECT name FROM sys.databases WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb') ORDER BY name";
                
                case DatabaseType.MySQL:
                    return "SELECT SCHEMA_NAME FROM information_schema.SCHEMATA WHERE SCHEMA_NAME NOT IN ('information_schema', 'mysql', 'performance_schema', 'sys') ORDER BY SCHEMA_NAME";
                
                case DatabaseType.PostgreSQL:
                    return "SELECT datname FROM pg_database WHERE datistemplate = false AND datname NOT IN ('postgres') ORDER BY datname";
                
                case DatabaseType.SQLite:
                    return string.Empty; // SQLite doesn't have multiple databases in a single connection
                
                default:
                    throw new ArgumentException("未支持的数据库类型");
            }
        }

        private string GetTablesQuery(DatabaseType dbType)
        {
            switch (dbType)
            {
                case DatabaseType.SqlServer:
                    return @"SELECT s.name AS SchemaName, t.name AS TableName 
                            FROM sys.tables t 
                            INNER JOIN sys.schemas s ON t.schema_id = s.schema_id 
                            WHERE t.is_ms_shipped = 0 
                            AND t.name <> 'sysdiagrams' 
                            AND DB_NAME() = @Database 
                            ORDER BY s.name, t.name";
                
                case DatabaseType.MySQL:
                    return @"SELECT TABLE_SCHEMA AS SchemaName, TABLE_NAME AS TableName 
                            FROM information_schema.TABLES 
                            WHERE TABLE_SCHEMA = @Database 
                            AND TABLE_TYPE = 'BASE TABLE' 
                            ORDER BY TABLE_NAME";
                
                case DatabaseType.PostgreSQL:
                    return @"SELECT table_schema AS SchemaName, table_name AS TableName 
                            FROM information_schema.tables 
                            WHERE table_schema NOT IN ('pg_catalog', 'information_schema') 
                            AND table_type = 'BASE TABLE' 
                            ORDER BY table_schema, table_name";
                
                case DatabaseType.SQLite:
                    return "SELECT 'main' AS SchemaName, name AS TableName FROM sqlite_master WHERE type = 'table' AND name NOT LIKE 'sqlite_%' ORDER BY name";
                
                default:
                    throw new ArgumentException("未支持的数据库类型");
            }
        }

        private string GetColumnsQuery(DatabaseType dbType)
        {
            switch (dbType)
            {
                case DatabaseType.SqlServer:
                    return @"SELECT 
                            c.COLUMN_NAME, 
                            c.DATA_TYPE, 
                            c.IS_NULLABLE,
                            c.CHARACTER_MAXIMUM_LENGTH,
                            c.NUMERIC_PRECISION,
                            c.NUMERIC_SCALE,
                            c.COLUMN_DEFAULT
                            FROM INFORMATION_SCHEMA.COLUMNS c
                            WHERE c.TABLE_NAME = @TableName
                            AND c.TABLE_SCHEMA = @SchemaName
                            AND c.TABLE_CATALOG = @Database
                            ORDER BY c.ORDINAL_POSITION";
                
                case DatabaseType.MySQL:
                    return @"SELECT 
                            COLUMN_NAME, 
                            DATA_TYPE, 
                            IS_NULLABLE,
                            CHARACTER_MAXIMUM_LENGTH,
                            NUMERIC_PRECISION,
                            NUMERIC_SCALE,
                            COLUMN_DEFAULT
                            FROM INFORMATION_SCHEMA.COLUMNS
                            WHERE TABLE_NAME = @TableName
                            AND TABLE_SCHEMA = @Database
                            ORDER BY ORDINAL_POSITION";
                
                case DatabaseType.PostgreSQL:
                    return @"SELECT 
                            COLUMN_NAME, 
                            DATA_TYPE, 
                            IS_NULLABLE,
                            CHARACTER_MAXIMUM_LENGTH,
                            NUMERIC_PRECISION,
                            NUMERIC_SCALE,
                            COLUMN_DEFAULT
                            FROM INFORMATION_SCHEMA.COLUMNS
                            WHERE TABLE_NAME = @TableName
                            AND TABLE_SCHEMA = @SchemaName
                            ORDER BY ORDINAL_POSITION";
                
                case DatabaseType.SQLite:
                    return @"PRAGMA table_info(@TableName)";
                
                default:
                    throw new ArgumentException("未支持的数据库类型");
            }
        }

        private string GetPrimaryKeysQuery(DatabaseType dbType)
        {
            switch (dbType)
            {
                case DatabaseType.SqlServer:
                    return @"SELECT k.COLUMN_NAME
                            FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS c
                            INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS k ON c.CONSTRAINT_NAME = k.CONSTRAINT_NAME
                            WHERE c.CONSTRAINT_TYPE = 'PRIMARY KEY'
                            AND k.TABLE_NAME = @TableName
                            AND k.TABLE_SCHEMA = @SchemaName
                            AND k.TABLE_CATALOG = @Database";
                
                case DatabaseType.MySQL:
                    return @"SELECT k.COLUMN_NAME
                            FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS c
                            INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS k ON c.CONSTRAINT_NAME = k.CONSTRAINT_NAME
                            WHERE c.CONSTRAINT_TYPE = 'PRIMARY KEY'
                            AND k.TABLE_NAME = @TableName
                            AND k.TABLE_SCHEMA = @Database";
                
                case DatabaseType.PostgreSQL:
                    return @"SELECT k.COLUMN_NAME
                            FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS c
                            INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS k ON c.CONSTRAINT_NAME = k.CONSTRAINT_NAME
                            WHERE c.CONSTRAINT_TYPE = 'PRIMARY KEY'
                            AND k.TABLE_NAME = @TableName
                            AND k.TABLE_SCHEMA = @SchemaName";
                
                case DatabaseType.SQLite:
                    return null; // For SQLite, we get primary key info from PRAGMA table_info
                
                default:
                    throw new ArgumentException("未支持的数据库类型");
            }
        }

        private string GetIdentityColumnsQuery(DatabaseType dbType)
        {
            switch (dbType)
            {
                case DatabaseType.SqlServer:
                    return @"SELECT c.name
                            FROM sys.columns c
                            INNER JOIN sys.tables t ON c.object_id = t.object_id
                            INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                            WHERE t.name = @TableName
                            AND s.name = @SchemaName
                            AND c.is_identity = 1";
                
                case DatabaseType.MySQL:
                    return @"SELECT COLUMN_NAME
                            FROM INFORMATION_SCHEMA.COLUMNS
                            WHERE TABLE_NAME = @TableName
                            AND TABLE_SCHEMA = @Database
                            AND EXTRA LIKE '%auto_increment%'";
                
                case DatabaseType.PostgreSQL:
                    return @"SELECT a.attname
                            FROM pg_catalog.pg_class c
                            JOIN pg_catalog.pg_namespace n ON n.oid = c.relnamespace
                            JOIN pg_catalog.pg_attribute a ON c.oid = a.attrelid
                            JOIN pg_catalog.pg_attrdef d ON d.adrelid = c.oid AND d.adnum = a.attnum
                            WHERE c.relname = @TableName
                            AND n.nspname = @SchemaName
                            AND d.adsrc LIKE '%nextval%'";
                
                case DatabaseType.SQLite:
                    return null; // SQLite doesn't have true identity columns
                
                default:
                    throw new ArgumentException("未支持的数据库类型");
            }
        }

        private string GetDatabaseParameterName(DatabaseType dbType)
        {
            switch (dbType)
            {
                case DatabaseType.SqlServer:
                    return "@Database";
                
                case DatabaseType.MySQL:
                    return "@Database";
                
                case DatabaseType.PostgreSQL:
                    return "@Database";
                
                default:
                    return "@Database";
            }
        }

        private Type MapDbTypeToClrType(string dbType, bool isNullable)
        {
            Type clrType;
            
            switch (dbType.ToLower())
            {
                case "bigint":
                    clrType = typeof(long);
                    break;
                
                case "binary":
                case "image":
                case "varbinary":
                    clrType = typeof(byte[]);
                    break;
                
                case "bit":
                    clrType = typeof(bool);
                    break;
                
                case "char":
                case "nchar":
                case "nvarchar":
                case "varchar":
                case "text":
                case "ntext":
                case "xml":
                case "longtext":
                    clrType = typeof(string);
                    break;
                
                case "datetime":
                case "date":
                case "datetime2":
                case "smalldatetime":
                case "timestamp":
                    clrType = typeof(DateTime);
                    break;
                
                case "decimal":
                case "money":
                case "numeric":
                case "smallmoney":
                    clrType = typeof(decimal);
                    break;
                
                case "float":
                case "real":
                    clrType = typeof(double);
                    break;
                
                case "int":
                case "integer":
                    clrType = typeof(int);
                    break;
                
                case "smallint":
                    clrType = typeof(short);
                    break;
                
                case "tinyint":
                    clrType = typeof(byte);
                    break;
                
                case "uniqueidentifier":
                    clrType = typeof(Guid);
                    break;
                
                default:
                    clrType = typeof(string);
                    break;
            }
            
            return clrType;
        }

        private string GetDefaultControlType(string dbType)
        {
            switch (dbType.ToLower())
            {
                case "bit":
                    return "CheckBox";
                
                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                case "timestamp":
                    return "DateTimePicker";
                
                case "decimal":
                case "float":
                case "int":
                case "integer":
                case "money":
                case "numeric":
                case "real":
                case "smallint":
                case "smallmoney":
                case "tinyint":
                    return "NumericUpDown";
                
                case "text":
                case "ntext":
                case "longtext":
                    return "TextArea";
                
                default:
                    return "TextBox";
            }
        }
    }
} 