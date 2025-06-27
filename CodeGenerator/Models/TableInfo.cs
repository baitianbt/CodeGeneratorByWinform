using System.Collections.Generic;

namespace CodeGenerator.Models
{
    public class TableInfo
    {
        public string Name { get; set; }
        public string SchemaName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public List<ColumnInfo> Columns { get; set; } = new List<ColumnInfo>();
        public List<PrimaryKeyInfo> PrimaryKeys { get; set; } = new List<PrimaryKeyInfo>();
        public List<ForeignKeyInfo> ForeignKeys { get; set; } = new List<ForeignKeyInfo>();

        public TableInfo()
        {
        }

        public TableInfo(string name, string schema = "dbo")
        {
            Name = name;
            SchemaName = schema;
            DisplayName = name;
        }
    }

    public class PrimaryKeyInfo
    {
        public string ColumnName { get; set; }
        public int KeyOrdinal { get; set; }
        public string ConstraintName { get; set; }
    }

    public class ForeignKeyInfo
    {
        public string ConstraintName { get; set; }
        public string ColumnName { get; set; }
        public string ReferencedTableName { get; set; }
        public string ReferencedTableSchema { get; set; }
        public string ReferencedColumnName { get; set; }
    }
} 