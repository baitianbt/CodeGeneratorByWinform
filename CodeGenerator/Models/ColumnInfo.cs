using System;

namespace CodeGenerator.Models
{
    public class ColumnInfo
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string DbType { get; set; }
        public Type CSharpType { get; set; }
        public bool IsNullable { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public int MaxLength { get; set; }
        public int Precision { get; set; }
        public int Scale { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsComputed { get; set; }
        public string DefaultValue { get; set; }
        
        // UI相关属性
        public string ControlType { get; set; } = "TextBox";
        public bool IsVisible { get; set; } = true;
        public bool IsReadOnly { get; set; } = false;
        public bool IsRequired { get; set; }
        public string ValidationMessage { get; set; }

        public ColumnInfo()
        {
        }

        public ColumnInfo(string name)
        {
            Name = name;
            DisplayName = name;
            IsVisible = true;
            IsReadOnly = false;
            ControlType = "TextBox";
        }

        private void MapDbTypeToClrType(string dbType)
        {
            // Default mapping, will be expanded based on database type
            dbType = dbType.ToLower();

            if (dbType.Contains("varchar") || dbType.Contains("nvarchar") || dbType.Contains("char") || dbType.Contains("text"))
                CSharpType = typeof(string);
            else if (dbType.Contains("int"))
                CSharpType = typeof(int);
            else if (dbType.Contains("bigint"))
                CSharpType = typeof(long);
            else if (dbType.Contains("bit"))
                CSharpType = typeof(bool);
            else if (dbType.Contains("decimal") || dbType.Contains("numeric") || dbType.Contains("money"))
                CSharpType = typeof(decimal);
            else if (dbType.Contains("float") || dbType.Contains("real"))
                CSharpType = typeof(double);
            else if (dbType.Contains("date") || dbType.Contains("time"))
                CSharpType = typeof(DateTime);
            else if (dbType.Contains("binary") || dbType.Contains("image") || dbType.Contains("blob"))
                CSharpType = typeof(byte[]);
            else if (dbType.Contains("uniqueidentifier"))
                CSharpType = typeof(Guid);
            else
                CSharpType = typeof(string);
        }
    }
} 