using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CodeGenerator.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeGenerator.Services
{
    public class CodeGenerationService
    {
        private readonly string _outputPath;

        public CodeGenerationService(string outputPath)
        {
            _outputPath = outputPath;
            if (!Directory.Exists(_outputPath))
            {
                Directory.CreateDirectory(_outputPath);
            }
        }
        
        // 辅助方法，用于获取C#类型名称
        private string GetCSharpTypeName(Type type, bool isNullable)
        {
            if (type == null)
                return "string";
                
            string typeName;
            
            if (type == typeof(int))
                typeName = "int";
            else if (type == typeof(long))
                typeName = "long";
            else if (type == typeof(short))
                typeName = "short";
            else if (type == typeof(byte))
                typeName = "byte";
            else if (type == typeof(bool))
                typeName = "bool";
            else if (type == typeof(decimal))
                typeName = "decimal";
            else if (type == typeof(float))
                typeName = "float";
            else if (type == typeof(double))
                typeName = "double";
            else if (type == typeof(char))
                typeName = "char";
            else if (type == typeof(DateTime))
                typeName = "DateTime";
            else if (type == typeof(Guid))
                typeName = "Guid";
            else
                typeName = type.Name;
                
            // 对于值类型，如果可为空，添加?
            if (isNullable && type.IsValueType)
                return typeName + "?";
                
            return typeName;
        }

        public void GenerateEntity(TableInfo table)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("using System;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
            sb.AppendLine();
            sb.AppendLine("namespace Generated.Entities");
            sb.AppendLine("{");
            sb.AppendLine($"    [Table(\"{table.Name}\", Schema = \"{table.SchemaName}\")]");
            sb.AppendLine($"    public class {table.Name}");
            sb.AppendLine("    {");
            
            foreach (var column in table.Columns)
            {
                // 添加列属性
                if (column.IsPrimaryKey)
                {
                    sb.AppendLine("        [Key]");
                }
                
                if (column.IsIdentity)
                {
                    sb.AppendLine("        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]");
                }
                
                if (!column.IsNullable && !column.IsPrimaryKey && column.CSharpType == typeof(string))
                {
                    sb.AppendLine("        [Required]");
                }
                
                if (column.MaxLength > 0 && column.CSharpType == typeof(string))
                {
                    sb.AppendLine($"        [MaxLength({column.MaxLength})]");
                }
                
                // 添加属性
                string typeName = GetCSharpTypeName(column.CSharpType, column.IsNullable);
                sb.AppendLine($"        public {typeName} {column.Name} {{ get; set; }}");
                sb.AppendLine();
            }
            
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            // 保存到文件
            string entityPath = Path.Combine(_outputPath, "Entities");
            if (!Directory.Exists(entityPath))
            {
                Directory.CreateDirectory(entityPath);
            }
            
            File.WriteAllText(Path.Combine(entityPath, $"{table.Name}.cs"), sb.ToString());
        }

        public void GenerateDbContext(string contextName, List<TableInfo> tables)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine("using System;");
            sb.AppendLine("using Generated.Entities;");
            sb.AppendLine();
            sb.AppendLine("namespace Generated.Data");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {contextName} : DbContext");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {contextName}(DbContextOptions<{contextName}> options)");
            sb.AppendLine("            : base(options)");
            sb.AppendLine("        {");
            sb.AppendLine("        }");
            sb.AppendLine();
            
            // 添加 DbSets
            foreach (var table in tables)
            {
                sb.AppendLine($"        public DbSet<{table.Name}> {table.Name}s {{ get; set; }}");
            }
            
            sb.AppendLine();
            sb.AppendLine("        protected override void OnModelCreating(ModelBuilder modelBuilder)");
            sb.AppendLine("        {");
            sb.AppendLine("            base.OnModelCreating(modelBuilder);");
            sb.AppendLine();
            
            // 配置表
            foreach (var table in tables)
            {
                sb.AppendLine($"            // Configure {table.Name}");
                sb.AppendLine($"            modelBuilder.Entity<{table.Name}>(entity =>");
                sb.AppendLine("            {");
                
                // 配置主键
                if (table.PrimaryKeys.Count > 0)
                {
                    sb.Append("                entity.HasKey(e => ");
                    if (table.PrimaryKeys.Count == 1)
                    {
                        sb.AppendLine($"e.{table.PrimaryKeys[0].ColumnName});");
                    }
                    else
                    {
                        sb.AppendLine("new {");
                        for (int i = 0; i < table.PrimaryKeys.Count; i++)
                        {
                            string comma = i < table.PrimaryKeys.Count - 1 ? "," : "";
                            sb.AppendLine($"                    e.{table.PrimaryKeys[i].ColumnName}{comma}");
                        }
                        sb.AppendLine("                });");
                    }
                }
                
                // 配置列
                foreach (var column in table.Columns)
                {
                    sb.AppendLine($"                entity.Property(e => e.{column.Name})");
                    
                    // 字符串特定配置
                    if (column.CSharpType == typeof(string) && column.MaxLength > 0)
                    {
                        sb.AppendLine($"                    .HasMaxLength({column.MaxLength})");
                    }
                    
                    // 数字特定配置
                    if ((column.CSharpType == typeof(decimal) || column.CSharpType == typeof(double)) && column.Precision > 0)
                    {
                        sb.AppendLine($"                    .HasPrecision({column.Precision}, {column.Scale})");
                    }
                    
                    if (!column.IsNullable)
                    {
                        sb.AppendLine("                    .IsRequired()");
                    }
                    
                    sb.AppendLine("                    ;");
                }
                
                sb.AppendLine("            });");
                sb.AppendLine();
            }
            
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            // 保存到文件
            string dataPath = Path.Combine(_outputPath, "Data");
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            
            File.WriteAllText(Path.Combine(dataPath, $"{contextName}.cs"), sb.ToString());
        }

        public void GenerateRepository(TableInfo table, bool generateInterface = true)
        {
            string entityName = table.Name;
            string repositoryName = $"{entityName}Repository";
            string interfaceName = $"I{entityName}Repository";
            
            // 生成接口（如果需要）
            if (generateInterface)
            {
                var sbInterface = new StringBuilder();
                
                sbInterface.AppendLine("using Generated.Entities;");
                sbInterface.AppendLine("using System;");
                sbInterface.AppendLine("using System.Collections.Generic;");
                sbInterface.AppendLine("using System.Threading.Tasks;");
                sbInterface.AppendLine();
                sbInterface.AppendLine("namespace Generated.Repositories");
                sbInterface.AppendLine("{");
                sbInterface.AppendLine($"    public interface {interfaceName} : IRepository<{entityName}>");
                sbInterface.AppendLine("    {");
                sbInterface.AppendLine("        // 在此添加自定义仓储方法");
                sbInterface.AppendLine("    }");
                sbInterface.AppendLine("}");
                
                // 保存接口
                string newRepoPath = Path.Combine(_outputPath, "Repositories");
                if (!Directory.Exists(newRepoPath))
                {
                    Directory.CreateDirectory(newRepoPath);
                }
                
                File.WriteAllText(Path.Combine(newRepoPath, $"{interfaceName}.cs"), sbInterface.ToString());
            }
            
            // 生成实现
            var sb = new StringBuilder();
            
            sb.AppendLine("using Generated.Data;");
            sb.AppendLine("using Generated.Entities;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine();
            sb.AppendLine("namespace Generated.Repositories");
            sb.AppendLine("{");
            
            if (generateInterface)
            {
                sb.AppendLine($"    public class {repositoryName} : Repository<{entityName}>, {interfaceName}");
            }
            else
            {
                sb.AppendLine($"    public class {repositoryName} : Repository<{entityName}>");
            }
            
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly DbContext _context;");
            sb.AppendLine();
            sb.AppendLine($"        public {repositoryName}(DbContext context) : base(context)");
            sb.AppendLine("        {");
            sb.AppendLine("            _context = context;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        // 在此添加自定义仓储方法");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            // 保存实现
            string repoPath = Path.Combine(_outputPath, "Repositories");
            if (!Directory.Exists(repoPath))
            {
                Directory.CreateDirectory(repoPath);
            }
            
            File.WriteAllText(Path.Combine(repoPath, $"{repositoryName}.cs"), sb.ToString());
        }

        public void GenerateBaseRepository()
        {
            // 生成 IRepository 接口
            var sbInterface = new StringBuilder();
            
            sbInterface.AppendLine("using System;");
            sbInterface.AppendLine("using System.Collections.Generic;");
            sbInterface.AppendLine("using System.Linq.Expressions;");
            sbInterface.AppendLine("using System.Threading.Tasks;");
            sbInterface.AppendLine();
            sbInterface.AppendLine("namespace Generated.Repositories");
            sbInterface.AppendLine("{");
            sbInterface.AppendLine("    public interface IRepository<T> where T : class");
            sbInterface.AppendLine("    {");
            sbInterface.AppendLine("        Task<IEnumerable<T>> GetAllAsync();");
            sbInterface.AppendLine("        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);");
            sbInterface.AppendLine("        Task<T> GetByIdAsync(object id);");
            sbInterface.AppendLine("        Task AddAsync(T entity);");
            sbInterface.AppendLine("        Task AddRangeAsync(IEnumerable<T> entities);");
            sbInterface.AppendLine("        Task RemoveAsync(T entity);");
            sbInterface.AppendLine("        Task RemoveRangeAsync(IEnumerable<T> entities);");
            sbInterface.AppendLine("        Task UpdateAsync(T entity);");
            sbInterface.AppendLine("        Task<int> SaveChangesAsync();");
            sbInterface.AppendLine("    }");
            sbInterface.AppendLine("}");
            
            // 生成 Repository 基类
            var sbRepo = new StringBuilder();
            
            sbRepo.AppendLine("using Microsoft.EntityFrameworkCore;");
            sbRepo.AppendLine("using System;");
            sbRepo.AppendLine("using System.Collections.Generic;");
            sbRepo.AppendLine("using System.Linq;");
            sbRepo.AppendLine("using System.Linq.Expressions;");
            sbRepo.AppendLine("using System.Threading.Tasks;");
            sbRepo.AppendLine();
            sbRepo.AppendLine("namespace Generated.Repositories");
            sbRepo.AppendLine("{");
            sbRepo.AppendLine("    public class Repository<T> : IRepository<T> where T : class");
            sbRepo.AppendLine("    {");
            sbRepo.AppendLine("        protected readonly DbContext Context;");
            sbRepo.AppendLine();
            sbRepo.AppendLine("        public Repository(DbContext context)");
            sbRepo.AppendLine("        {");
            sbRepo.AppendLine("            Context = context;");
            sbRepo.AppendLine("        }");
            sbRepo.AppendLine();
            sbRepo.AppendLine("        public async Task<IEnumerable<T>> GetAllAsync()");
            sbRepo.AppendLine("        {");
            sbRepo.AppendLine("            return await Context.Set<T>().ToListAsync();");
            sbRepo.AppendLine("        }");
            sbRepo.AppendLine();
            sbRepo.AppendLine("        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)");
            sbRepo.AppendLine("        {");
            sbRepo.AppendLine("            return await Context.Set<T>().Where(predicate).ToListAsync();");
            sbRepo.AppendLine("        }");
            sbRepo.AppendLine();
            sbRepo.AppendLine("        public async Task<T> GetByIdAsync(object id)");
            sbRepo.AppendLine("        {");
            sbRepo.AppendLine("            return await Context.Set<T>().FindAsync(id);");
            sbRepo.AppendLine();
            sbRepo.AppendLine("        public async Task AddAsync(T entity)");
            sbRepo.AppendLine("        {");
            sbRepo.AppendLine("            await Context.Set<T>().AddAsync(entity);");
            sbRepo.AppendLine("        }");
            sbRepo.AppendLine();
            sbRepo.AppendLine("        public async Task AddRangeAsync(IEnumerable<T> entities)");
            sbRepo.AppendLine("        {");
            sbRepo.AppendLine("            await Context.Set<T>().AddRangeAsync(entities);");
            sbRepo.AppendLine("        }");
            sbRepo.AppendLine();
            sbRepo.AppendLine("        public Task RemoveAsync(T entity)");
            sbRepo.AppendLine("        {");
            sbRepo.AppendLine("            Context.Set<T>().Remove(entity);");
            sbRepo.AppendLine("            return Task.CompletedTask;");
            sbRepo.AppendLine();
            sbRepo.AppendLine("        public Task RemoveRangeAsync(IEnumerable<T> entities)");
            sbRepo.AppendLine("        {");
            sbRepo.AppendLine("            Context.Set<T>().RemoveRange(entities);");
            sbRepo.AppendLine("            return Task.CompletedTask;");
            sbRepo.AppendLine();
            sbRepo.AppendLine("        public Task UpdateAsync(T entity)");
            sbRepo.AppendLine("        {");
            sbRepo.AppendLine("            Context.Set<T>().Update(entity);");
            sbRepo.AppendLine("            return Task.CompletedTask;");
            sbRepo.AppendLine();
            sbRepo.AppendLine("        public async Task<int> SaveChangesAsync()");
            sbRepo.AppendLine("        {");
            sbRepo.AppendLine("            return await Context.SaveChangesAsync();");
            sbRepo.AppendLine("        }");
            sbRepo.AppendLine("    }");
            sbRepo.AppendLine("}");
            
            // 保存文件
            string baseRepoPath = Path.Combine(_outputPath, "Repositories");
            if (!Directory.Exists(baseRepoPath))
            {
                Directory.CreateDirectory(baseRepoPath);
            }
            
            File.WriteAllText(Path.Combine(baseRepoPath, "IRepository.cs"), sbInterface.ToString());
            File.WriteAllText(Path.Combine(baseRepoPath, "Repository.cs"), sbRepo.ToString());
        }

        public void GenerateService(TableInfo table, bool generateInterface = true)
        {
            string entityName = table.Name;
            string serviceName = $"{entityName}Service";
            string interfaceName = $"I{entityName}Service";
            string repositoryName = $"{entityName}Repository";
            string interfaceRepoName = $"I{entityName}Repository";
            
            // 生成接口（如果需要）
            if (generateInterface)
            {
                var sbInterface = new StringBuilder();
                
                sbInterface.AppendLine("using Generated.Entities;");
                sbInterface.AppendLine("using System;");
                sbInterface.AppendLine("using System.Collections.Generic;");
                sbInterface.AppendLine("using System.Threading.Tasks;");
                sbInterface.AppendLine();
                sbInterface.AppendLine("namespace Generated.Services");
                sbInterface.AppendLine("{");
                sbInterface.AppendLine($"    public interface {interfaceName} : IService<{entityName}>");
                sbInterface.AppendLine("    {");
                sbInterface.AppendLine("        // 在此添加自定义服务方法");
                sbInterface.AppendLine("    }");
                sbInterface.AppendLine("}");
                
                // 保存接口
                string newServicePath = Path.Combine(_outputPath, "Services");
                if (!Directory.Exists(newServicePath))
                {
                    Directory.CreateDirectory(newServicePath);
                }
                
                File.WriteAllText(Path.Combine(newServicePath, $"{interfaceName}.cs"), sbInterface.ToString());
            }
            
            // 生成实现
            var sb = new StringBuilder();
            
            sb.AppendLine("using Generated.Entities;");
            sb.AppendLine("using Generated.Repositories;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine();
            sb.AppendLine("namespace Generated.Services");
            sb.AppendLine("{");
            
            if (generateInterface)
            {
                sb.AppendLine($"    public class {serviceName} : Service<{entityName}>, {interfaceName}");
            }
            else
            {
                sb.AppendLine($"    public class {serviceName} : Service<{entityName}>");
            }
            
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly {interfaceRepoName} _repository;");
            sb.AppendLine();
            sb.AppendLine($"        public {serviceName}({interfaceRepoName} repository) : base(repository)");
            sb.AppendLine("        {");
            sb.AppendLine("            _repository = repository;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        // 在此添加自定义服务方法");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            // 保存实现
            string servicePath = Path.Combine(_outputPath, "Services");
            if (!Directory.Exists(servicePath))
            {
                Directory.CreateDirectory(servicePath);
            }
            
            File.WriteAllText(Path.Combine(servicePath, $"{serviceName}.cs"), sb.ToString());
        }

        public void GenerateBaseService()
        {
            // 生成 IService 接口
            var sbInterface = new StringBuilder();
            
            sbInterface.AppendLine("using System;");
            sbInterface.AppendLine("using System.Collections.Generic;");
            sbInterface.AppendLine("using System.Linq.Expressions;");
            sbInterface.AppendLine("using System.Threading.Tasks;");
            sbInterface.AppendLine();
            sbInterface.AppendLine("namespace Generated.Services");
            sbInterface.AppendLine("{");
            sbInterface.AppendLine("    public interface IService<T> where T : class");
            sbInterface.AppendLine("    {");
            sbInterface.AppendLine("        Task<IEnumerable<T>> GetAllAsync();");
            sbInterface.AppendLine("        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);");
            sbInterface.AppendLine("        Task<T> GetByIdAsync(object id);");
            sbInterface.AppendLine("        Task AddAsync(T entity);");
            sbInterface.AppendLine("        Task AddRangeAsync(IEnumerable<T> entities);");
            sbInterface.AppendLine("        Task RemoveAsync(T entity);");
            sbInterface.AppendLine("        Task RemoveRangeAsync(IEnumerable<T> entities);");
            sbInterface.AppendLine("        Task UpdateAsync(T entity);");
            sbInterface.AppendLine("        Task<int> SaveChangesAsync();");
            sbInterface.AppendLine("    }");
            sbInterface.AppendLine("}");
            
            // 生成 Service 基类
            var sbService = new StringBuilder();
            
            sbService.AppendLine("using Generated.Repositories;");
            sbService.AppendLine("using System;");
            sbService.AppendLine("using System.Collections.Generic;");
            sbService.AppendLine("using System.Linq.Expressions;");
            sbService.AppendLine("using System.Threading.Tasks;");
            sbService.AppendLine();
            sbService.AppendLine("namespace Generated.Services");
            sbService.AppendLine("{");
            sbService.AppendLine("    public class Service<T> : IService<T> where T : class");
            sbService.AppendLine("    {");
            sbService.AppendLine("        protected readonly IRepository<T> Repository;");
            sbService.AppendLine();
            sbService.AppendLine("        public Service(IRepository<T> repository)");
            sbService.AppendLine("        {");
            sbService.AppendLine("            Repository = repository;");
            sbService.AppendLine("        }");
            sbService.AppendLine();
            sbService.AppendLine("        public async Task<IEnumerable<T>> GetAllAsync()");
            sbService.AppendLine("        {");
            sbService.AppendLine("            return await Repository.GetAllAsync();");
            sbService.AppendLine("        }");
            sbService.AppendLine();
            sbService.AppendLine("        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)");
            sbService.AppendLine("        {");
            sbService.AppendLine("            return await Repository.FindAsync(predicate);");
            sbService.AppendLine("        }");
            sbService.AppendLine();
            sbService.AppendLine("        public async Task<T> GetByIdAsync(object id)");
            sbService.AppendLine("        {");
            sbService.AppendLine("            return await Repository.GetByIdAsync(id);");
            sbService.AppendLine();
            sbService.AppendLine("        public async Task AddAsync(T entity)");
            sbService.AppendLine("        {");
            sbService.AppendLine("            await Repository.AddAsync(entity);");
            sbService.AppendLine("            await Repository.SaveChangesAsync();");
            sbService.AppendLine("        }");
            sbService.AppendLine();
            sbService.AppendLine("        public async Task AddRangeAsync(IEnumerable<T> entities)");
            sbService.AppendLine("        {");
            sbService.AppendLine("            await Repository.AddRangeAsync(entities);");
            sbService.AppendLine("            await Repository.SaveChangesAsync();");
            sbService.AppendLine("        }");
            sbService.AppendLine();
            sbService.AppendLine("        public async Task RemoveAsync(T entity)");
            sbService.AppendLine("        {");
            sbService.AppendLine("            await Repository.RemoveAsync(entity);");
            sbService.AppendLine("            await Repository.SaveChangesAsync();");
            sbService.AppendLine("        }");
            sbService.AppendLine();
            sbService.AppendLine("        public async Task RemoveRangeAsync(IEnumerable<T> entities)");
            sbService.AppendLine("        {");
            sbService.AppendLine("            await Repository.RemoveRangeAsync(entities);");
            sbService.AppendLine("            await Repository.SaveChangesAsync();");
            sbService.AppendLine("        }");
            sbService.AppendLine();
            sbService.AppendLine("        public async Task UpdateAsync(T entity)");
            sbService.AppendLine("        {");
            sbService.AppendLine("            await Repository.UpdateAsync(entity);");
            sbService.AppendLine("            await Repository.SaveChangesAsync();");
            sbService.AppendLine("        }");
            sbService.AppendLine();
            sbService.AppendLine("        public async Task<int> SaveChangesAsync()");
            sbService.AppendLine("        {");
            sbService.AppendLine("            return await Repository.SaveChangesAsync();");
            sbService.AppendLine("        }");
            sbService.AppendLine("    }");
            sbService.AppendLine("}");
            
            // 保存文件
            string baseServicePath = Path.Combine(_outputPath, "Services");
            if (!Directory.Exists(baseServicePath))
            {
                Directory.CreateDirectory(baseServicePath);
            }
            
            File.WriteAllText(Path.Combine(baseServicePath, "IService.cs"), sbInterface.ToString());
            File.WriteAllText(Path.Combine(baseServicePath, "Service.cs"), sbService.ToString());
        }

        public void GenerateWinFormUI(TableInfo table)
        {
            GenerateListForm(table);
            GenerateEditForm(table);
        }

        private void GenerateListForm(TableInfo table)
        {
            string entityName = table.Name;
            
            // 生成表单代码文件
            var sbCode = new StringBuilder();
            
            sbCode.AppendLine("using Generated.Entities;");
            sbCode.AppendLine("using Generated.Services;");
            sbCode.AppendLine("using System;");
            sbCode.AppendLine("using System.Collections.Generic;");
            sbCode.AppendLine("using System.Data;");
            sbCode.AppendLine("using System.Linq;");
            sbCode.AppendLine("using System.Threading.Tasks;");
            sbCode.AppendLine("using System.Windows.Forms;");
            sbCode.AppendLine("using NPOI.SS.UserModel;");
            sbCode.AppendLine("using NPOI.XSSF.UserModel;");
            sbCode.AppendLine("using System.IO;");
            sbCode.AppendLine();
            sbCode.AppendLine("namespace Generated.Forms");
            sbCode.AppendLine("{");
            sbCode.AppendLine($"    public partial class {entityName}ListForm : Form");
            sbCode.AppendLine("    {");
            sbCode.AppendLine($"        private readonly I{entityName}Service _service;");
            sbCode.AppendLine("        private List<" + entityName + "> _items;");
            sbCode.AppendLine("        private int _currentPage = 1;");
            sbCode.AppendLine("        private int _pageSize = 20;");
            sbCode.AppendLine("        private int _totalPages = 1;");
            sbCode.AppendLine();
            sbCode.AppendLine($"        public {entityName}ListForm(I{entityName}Service service)");
            sbCode.AppendLine("        {");
            sbCode.AppendLine("            InitializeComponent();");
            sbCode.AppendLine("            _service = service;");
            sbCode.AppendLine("        }");
            sbCode.AppendLine();
            sbCode.AppendLine("        private async void OnFormLoad(object sender, EventArgs e)");
            sbCode.AppendLine("        {");
            sbCode.AppendLine("            await LoadDataAsync();");
            sbCode.AppendLine("        }");
            sbCode.AppendLine();
            sbCode.AppendLine("        private async Task LoadDataAsync()");
            sbCode.AppendLine("        {");
            sbCode.AppendLine("            try");
            sbCode.AppendLine("            {");
            sbCode.AppendLine("                // 获取所有数据并在内存中处理分页");
            sbCode.AppendLine("                _items = (await _service.GetAllAsync()).ToList();");
            sbCode.AppendLine("                _totalPages = (_items.Count + _pageSize - 1) / _pageSize;");
            sbCode.AppendLine("                UpdatePagingInfo();");
            sbCode.AppendLine("                DisplayCurrentPage();");
            sbCode.AppendLine("            }");
            sbCode.AppendLine("            catch (Exception ex)");
            sbCode.AppendLine("            {");
            sbCode.AppendLine("                MessageBox.Show($\"加载数据错误: {ex.Message}\", \"错误\", MessageBoxButtons.OK, MessageBoxIcon.Error);");
            sbCode.AppendLine("            }");
            sbCode.AppendLine("        }");
            sbCode.AppendLine();
            sbCode.AppendLine("        private void UpdatePagingInfo()");
            sbCode.AppendLine("        {");
            sbCode.AppendLine("            lblPaging.Text = $\"第 {_currentPage} 页，共 {_totalPages} 页\";");
            sbCode.AppendLine("            btnPrevious.Enabled = _currentPage > 1;");
            sbCode.AppendLine("            btnNext.Enabled = _currentPage < _totalPages;");
            sbCode.AppendLine("        }");
            sbCode.AppendLine();
            sbCode.AppendLine("        private void DisplayCurrentPage()");
            sbCode.AppendLine("        {");
            sbCode.AppendLine("            var pagedItems = _items.Skip((_currentPage - 1) * _pageSize).Take(_pageSize).ToList();");
            sbCode.AppendLine("            dataGridView.DataSource = pagedItems;");
            sbCode.AppendLine("        }");
            sbCode.AppendLine();
            sbCode.AppendLine("        private void btnAdd_Click(object sender, EventArgs e)");
            sbCode.AppendLine("        {");
            sbCode.AppendLine($"            var form = new {entityName}EditForm(_service);");
            sbCode.AppendLine("            if (form.ShowDialog() == DialogResult.OK)");
            sbCode.AppendLine("            {");
            sbCode.AppendLine("                LoadDataAsync().ConfigureAwait(false);");
            sbCode.AppendLine("            }");
            sbCode.AppendLine("        }");
            sbCode.AppendLine();
            sbCode.AppendLine("        private void btnEdit_Click(object sender, EventArgs e)");
            sbCode.AppendLine("        {");
            sbCode.AppendLine("            if (dataGridView.SelectedRows.Count == 0) return;");
            sbCode.AppendLine();
            sbCode.AppendLine($"            var item = dataGridView.SelectedRows[0].DataBoundItem as {entityName};");
            sbCode.AppendLine($"            var form = new {entityName}EditForm(_service, item);");
            sbCode.AppendLine();
            sbCode.AppendLine("            if (form.ShowDialog() == DialogResult.OK)");
            sbCode.AppendLine("            {");
            sbCode.AppendLine("                LoadDataAsync().ConfigureAwait(false);");
            sbCode.AppendLine("            }");
            sbCode.AppendLine("        }");
            sbCode.AppendLine();
            sbCode.AppendLine("        private async void btnDelete_Click(object sender, EventArgs e)");
            sbCode.AppendLine("        {");
            sbCode.AppendLine("            if (dataGridView.SelectedRows.Count == 0) return;");
            sbCode.AppendLine();
            sbCode.AppendLine("            if (MessageBox.Show(\"确定要删除这条记录吗？\", \"确认删除\", ");
            sbCode.AppendLine("                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)");
            sbCode.AppendLine("            {");
            sbCode.AppendLine("                return;");
            sbCode.AppendLine("            }");
            sbCode.AppendLine();
            sbCode.AppendLine("            try");
            sbCode.AppendLine("            {");
            sbCode.AppendLine($"                var item = dataGridView.SelectedRows[0].DataBoundItem as {entityName};");
            sbCode.AppendLine("                await _service.RemoveAsync(item);");
            sbCode.AppendLine("                await LoadDataAsync();");
            sbCode.AppendLine("            }");
            sbCode.AppendLine("            catch (Exception ex)");
            sbCode.AppendLine("            {");
            sbCode.AppendLine("                MessageBox.Show($\"删除记录错误: {ex.Message}\", \"错误\", MessageBoxButtons.OK, MessageBoxIcon.Error);");
            sbCode.AppendLine("            }");
            sbCode.AppendLine("        }");
            sbCode.AppendLine();
            sbCode.AppendLine("        private void btnPrevious_Click(object sender, EventArgs e)");
            sbCode.AppendLine("        {");
            sbCode.AppendLine("            if (_currentPage > 1)");
            sbCode.AppendLine("            {");
            sbCode.AppendLine("                _currentPage--;");
            sbCode.AppendLine("                UpdatePagingInfo();");
            sbCode.AppendLine("                DisplayCurrentPage();");
            sbCode.AppendLine("            }");
            sbCode.AppendLine("        }");
            sbCode.AppendLine();
            sbCode.AppendLine("        private void btnNext_Click(object sender, EventArgs e)");
            sbCode.AppendLine("        {");
            sbCode.AppendLine("            if (_currentPage < _totalPages)");
            sbCode.AppendLine("            {");
            sbCode.AppendLine("                _currentPage++;");
            sbCode.AppendLine("                UpdatePagingInfo();");
            sbCode.AppendLine("                DisplayCurrentPage();");
            sbCode.AppendLine("            }");
            sbCode.AppendLine("        }");
            sbCode.AppendLine();
            sbCode.AppendLine("        private void btnExport_Click(object sender, EventArgs e)");
            sbCode.AppendLine("        {");
            sbCode.AppendLine("            if (_items == null || _items.Count == 0)");
            sbCode.AppendLine("            {");
            sbCode.AppendLine("                MessageBox.Show(\"没有可导出的数据\", \"提示\", MessageBoxButtons.OK, MessageBoxIcon.Information);");
            sbCode.AppendLine("                return;");
            sbCode.AppendLine("            }");
            sbCode.AppendLine();
            sbCode.AppendLine("            var saveFileDialog = new SaveFileDialog");
            sbCode.AppendLine("            {");
            sbCode.AppendLine($"                FileName = \"{entityName}_导出_\" + DateTime.Now.ToString(\"yyyyMMdd\"),");
            sbCode.AppendLine("                Filter = \"Excel文件|*.xlsx|CSV文件|*.csv\"");
            sbCode.AppendLine("            };");
            sbCode.AppendLine();
            sbCode.AppendLine("            if (saveFileDialog.ShowDialog() == DialogResult.OK)");
            sbCode.AppendLine("            {");
            sbCode.AppendLine("                if (saveFileDialog.FileName.EndsWith(\".xlsx\"))");
            sbCode.AppendLine("                {");
            sbCode.AppendLine("                    ExportToExcel(saveFileDialog.FileName);");
            sbCode.AppendLine("                }");
            sbCode.AppendLine("                else if (saveFileDialog.FileName.EndsWith(\".csv\"))");
            sbCode.AppendLine("                {");
            sbCode.AppendLine("                    ExportToCsv(saveFileDialog.FileName);");
            sbCode.AppendLine("                }");
            sbCode.AppendLine("            }");
            sbCode.AppendLine();
            sbCode.AppendLine("        }");
            sbCode.AppendLine();
            sbCode.AppendLine("        private void ExportToExcel(string filePath)");
            sbCode.AppendLine("        {");
            sbCode.AppendLine("            try");
            sbCode.AppendLine("            {");
            sbCode.AppendLine("                using (var workbook = new XSSFWorkbook())");
            sbCode.AppendLine("                {");
            sbCode.AppendLine($"                    var sheet = workbook.CreateSheet(\"{entityName}\");");
            sbCode.AppendLine("                    var headerRow = sheet.CreateRow(0);");
            sbCode.AppendLine();
            sbCode.AppendLine("                    // 创建表头");
            sbCode.AppendLine("                    for (int i = 0; i < dataGridView.Columns.Count; i++)");
            sbCode.AppendLine("                    {");
            sbCode.AppendLine("                        if (dataGridView.Columns[i].Visible)");
            sbCode.AppendLine("                        {");
            sbCode.AppendLine("                            headerRow.CreateCell(i).SetCellValue(dataGridView.Columns[i].HeaderText);");
            sbCode.AppendLine("                        }");
            sbCode.AppendLine("                    }");
            sbCode.AppendLine();
            sbCode.AppendLine("                    // 添加数据行");
            sbCode.AppendLine("                    int rowIndex = 1;");
            sbCode.AppendLine("                    foreach (var item in _items)");
            sbCode.AppendLine("                    {");
            sbCode.AppendLine("                        var row = sheet.CreateRow(rowIndex++);");
            sbCode.AppendLine("                        int cellIndex = 0;");
            sbCode.AppendLine();
            
            // 为表中的具体列添加数据
            foreach (var column in table.Columns)
            {
                if (column.IsVisible)
                {
                    sbCode.AppendLine($"                        row.CreateCell(cellIndex++).SetCellValue(item.{column.Name}?.ToString() ?? \"\");");
                }
            }
            
            sbCode.AppendLine("                    }");
            sbCode.AppendLine();
            sbCode.AppendLine("                    // 自动调整列宽");
            sbCode.AppendLine("                    for (int i = 0; i < dataGridView.Columns.Count; i++)");
            sbCode.AppendLine("                    {");
            sbCode.AppendLine("                        if (dataGridView.Columns[i].Visible)");
            sbCode.AppendLine("                        {");
            sbCode.AppendLine("                            sheet.AutoSizeColumn(i);");
            sbCode.AppendLine("                        }");
            sbCode.AppendLine("                    }");
            sbCode.AppendLine();
            sbCode.AppendLine("                    // 保存工作簿");
            sbCode.AppendLine("                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))");
            sbCode.AppendLine("                    {");
            sbCode.AppendLine("                        workbook.Write(fileStream);");
            sbCode.AppendLine("                    }");
            sbCode.AppendLine("                }");
            sbCode.AppendLine();
            sbCode.AppendLine("                MessageBox.Show(\"导出完成\", \"成功\", MessageBoxButtons.OK, MessageBoxIcon.Information);");
            sbCode.AppendLine("            }");
            sbCode.AppendLine("            catch (Exception ex)");
            sbCode.AppendLine("            {");
            sbCode.AppendLine("                MessageBox.Show($\"导出到Excel错误: {ex.Message}\", \"错误\", MessageBoxButtons.OK, MessageBoxIcon.Error);");
            sbCode.AppendLine("            }");
            sbCode.AppendLine("        }");
            sbCode.AppendLine();
            sbCode.AppendLine("        private void ExportToCsv(string filePath)");
            sbCode.AppendLine("        {");
            sbCode.AppendLine("            try");
            sbCode.AppendLine("            {");
            sbCode.AppendLine("                using (var streamWriter = new StreamWriter(filePath))");
            sbCode.AppendLine("                {");
            sbCode.AppendLine("                    // 写入表头");
            sbCode.AppendLine("                    var headers = new List<string>();");
            sbCode.AppendLine("                    for (int i = 0; i < dataGridView.Columns.Count; i++)");
            sbCode.AppendLine("                    {");
            sbCode.AppendLine("                        if (dataGridView.Columns[i].Visible)");
            sbCode.AppendLine("                        {");
            sbCode.AppendLine("                            headers.Add(dataGridView.Columns[i].HeaderText);");
            sbCode.AppendLine("                        }");
            sbCode.AppendLine("                    }");
            sbCode.AppendLine("                    streamWriter.WriteLine(string.Join(\",\", headers));");
            sbCode.AppendLine();
            sbCode.AppendLine("                    // 写入数据行");
            sbCode.AppendLine("                    foreach (var item in _items)");
            sbCode.AppendLine("                    {");
            sbCode.AppendLine("                        var values = new List<string>();");
            
            // 为表中的具体列添加数据
            foreach (var column in table.Columns)
            {
                if (column.IsVisible)
                {
                    sbCode.AppendLine($"                        var value = item.{column.Name}?.ToString() ?? \"\";");
                    sbCode.AppendLine("                        // 处理逗号和引号");
                    sbCode.AppendLine("                        if (value.Contains(\",\") || value.Contains(\"\\\"\"))");
                    sbCode.AppendLine("                        {");
                    sbCode.AppendLine("                            value = $\"\\\"{{value.Replace(\"\\\"\", \"\\\"\\\"\")}}\\\"\";");
                    sbCode.AppendLine("                        }");
                    sbCode.AppendLine("                        values.Add(value);");
                }
            }
            
            sbCode.AppendLine("                        streamWriter.WriteLine(string.Join(\",\", values));");
            sbCode.AppendLine("                    }");
            sbCode.AppendLine("                }");
            sbCode.AppendLine();
            sbCode.AppendLine("                MessageBox.Show(\"导出完成\", \"成功\", MessageBoxButtons.OK, MessageBoxIcon.Information);");
            sbCode.AppendLine("            }");
            sbCode.AppendLine("            catch (Exception ex)");
            sbCode.AppendLine("            {");
            sbCode.AppendLine("                MessageBox.Show($\"导出到CSV错误: {ex.Message}\", \"错误\", MessageBoxButtons.OK, MessageBoxIcon.Error);");
            sbCode.AppendLine("            }");
            sbCode.AppendLine("        }");
            sbCode.AppendLine("    }");
            sbCode.AppendLine("}");

            // 生成设计器文件
            var sbDesigner = new StringBuilder();
            
            sbDesigner.AppendLine("namespace Generated.Forms");
            sbDesigner.AppendLine("{");
            sbDesigner.AppendLine($"    partial class {entityName}ListForm");
            sbDesigner.AppendLine("    {");
            sbDesigner.AppendLine("        /// <summary>");
            sbDesigner.AppendLine("        /// 必需的设计器变量。");
            sbDesigner.AppendLine("        /// </summary>");
            sbDesigner.AppendLine("        private System.ComponentModel.IContainer components = null;");
            sbDesigner.AppendLine();
            sbDesigner.AppendLine("        /// <summary>");
            sbDesigner.AppendLine("        /// 清理所有正在使用的资源。");
            sbDesigner.AppendLine("        /// </summary>");
            sbDesigner.AppendLine("        /// <param name=\"disposing\">如果应释放托管资源，为 true；否则为 false。</param>");
            sbDesigner.AppendLine("        protected override void Dispose(bool disposing)");
            sbDesigner.AppendLine("        {");
            sbDesigner.AppendLine("            if (disposing && (components != null))");
            sbDesigner.AppendLine("            {");
            sbDesigner.AppendLine("                components.Dispose();");
            sbDesigner.AppendLine("            }");
            sbDesigner.AppendLine("            base.Dispose(disposing);");
            sbDesigner.AppendLine("        }");
            sbDesigner.AppendLine();
            sbDesigner.AppendLine("        #region Windows 窗体设计器生成的代码");
            sbDesigner.AppendLine();
            sbDesigner.AppendLine("        /// <summary>");
            sbDesigner.AppendLine("        /// 设计器支持所需的方法 - 不要修改");
            sbCode.AppendLine("        /// 使用代码编辑器修改此方法的内容。");
            sbDesigner.AppendLine("        /// </summary>");
            sbDesigner.AppendLine("        private void InitializeComponent()");
            sbDesigner.AppendLine("        {");
            sbDesigner.AppendLine("            this.panel1 = new System.Windows.Forms.Panel();");
            sbDesigner.AppendLine("            this.btnExport = new System.Windows.Forms.Button();");
            sbDesigner.AppendLine("            this.btnDelete = new System.Windows.Forms.Button();");
            sbDesigner.AppendLine("            this.btnEdit = new System.Windows.Forms.Button();");
            sbDesigner.AppendLine("            this.btnAdd = new System.Windows.Forms.Button();");
            sbDesigner.AppendLine("            this.dataGridView = new System.Windows.Forms.DataGridView();");
            sbDesigner.AppendLine("            this.panel2 = new System.Windows.Forms.Panel();");
            sbDesigner.AppendLine("            this.btnNext = new System.Windows.Forms.Button();");
            sbDesigner.AppendLine("            this.btnPrevious = new System.Windows.Forms.Button();");
            sbDesigner.AppendLine("            this.lblPaging = new System.Windows.Forms.Label();");
            sbDesigner.AppendLine("            this.panel1.SuspendLayout();");
            sbDesigner.AppendLine("            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();");
            sbDesigner.AppendLine("            this.panel2.SuspendLayout();");
            sbDesigner.AppendLine("            this.SuspendLayout();");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            // panel1");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            this.panel1.Controls.Add(this.btnExport);");
            sbDesigner.AppendLine("            this.panel1.Controls.Add(this.btnDelete);");
            sbDesigner.AppendLine("            this.panel1.Controls.Add(this.btnEdit);");
            sbDesigner.AppendLine("            this.panel1.Controls.Add(this.btnAdd);");
            sbDesigner.AppendLine("            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;");
            sbDesigner.AppendLine("            this.panel1.Location = new System.Drawing.Point(0, 0);");
            sbDesigner.AppendLine("            this.panel1.Name = \"panel1\";");
            sbDesigner.AppendLine("            this.panel1.Size = new System.Drawing.Size(984, 60);");
            sbDesigner.AppendLine("            this.panel1.TabIndex = 0;");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            // btnExport");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));");
            sbDesigner.AppendLine("            this.btnExport.Location = new System.Drawing.Point(877, 15);");
            sbDesigner.AppendLine("            this.btnExport.Name = \"btnExport\";");
            sbDesigner.AppendLine("            this.btnExport.Size = new System.Drawing.Size(95, 30);");
            sbDesigner.AppendLine("            this.btnExport.TabIndex = 3;");
            sbDesigner.AppendLine("            this.btnExport.Text = \"导出\";");
            sbDesigner.AppendLine("            this.btnExport.UseVisualStyleBackColor = true;");
            sbDesigner.AppendLine("            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            // btnDelete");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            this.btnDelete.Location = new System.Drawing.Point(214, 15);");
            sbDesigner.AppendLine("            this.btnDelete.Name = \"btnDelete\";");
            sbDesigner.AppendLine("            this.btnDelete.Size = new System.Drawing.Size(95, 30);");
            sbDesigner.AppendLine("            this.btnDelete.TabIndex = 2;");
            sbDesigner.AppendLine("            this.btnDelete.Text = \"删除\";");
            sbDesigner.AppendLine("            this.btnDelete.UseVisualStyleBackColor = true;");
            sbDesigner.AppendLine("            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            // btnEdit");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            this.btnEdit.Location = new System.Drawing.Point(113, 15);");
            sbDesigner.AppendLine("            this.btnEdit.Name = \"btnEdit\";");
            sbDesigner.AppendLine("            this.btnEdit.Size = new System.Drawing.Size(95, 30);");
            sbDesigner.AppendLine("            this.btnEdit.TabIndex = 1;");
            sbDesigner.AppendLine("            this.btnEdit.Text = \"编辑\";");
            sbDesigner.AppendLine("            this.btnEdit.UseVisualStyleBackColor = true;");
            sbDesigner.AppendLine("            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            // btnAdd");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            this.btnAdd.Location = new System.Drawing.Point(12, 15);");
            sbDesigner.AppendLine("            this.btnAdd.Name = \"btnAdd\";");
            sbDesigner.AppendLine("            this.btnAdd.Size = new System.Drawing.Size(95, 30);");
            sbDesigner.AppendLine("            this.btnAdd.TabIndex = 0;");
            sbDesigner.AppendLine("            this.btnAdd.Text = \"添加\";");
            sbDesigner.AppendLine("            this.btnAdd.UseVisualStyleBackColor = true;");
            sbDesigner.AppendLine("            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            // dataGridView");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            this.dataGridView.AllowUserToAddRows = false;");
            sbDesigner.AppendLine("            this.dataGridView.AllowUserToDeleteRows = false;");
            sbDesigner.AppendLine("            this.dataGridView.AllowUserToOrderColumns = true;");
            sbDesigner.AppendLine("            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;");
            sbDesigner.AppendLine("            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.Control;");
            sbDesigner.AppendLine("            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;");
            sbDesigner.AppendLine("            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;");
            sbDesigner.AppendLine("            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;");
            sbDesigner.AppendLine("            this.dataGridView.Location = new System.Drawing.Point(0, 60);");
            sbDesigner.AppendLine("            this.dataGridView.MultiSelect = false;");
            sbDesigner.AppendLine("            this.dataGridView.Name = \"dataGridView\";");
            sbDesigner.AppendLine("            this.dataGridView.ReadOnly = true;");
            sbDesigner.AppendLine("            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;");
            sbDesigner.AppendLine("            this.dataGridView.Size = new System.Drawing.Size(984, 440);");
            sbDesigner.AppendLine("            this.dataGridView.TabIndex = 1;");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            // panel2");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            this.panel2.Controls.Add(this.btnNext);");
            sbDesigner.AppendLine("            this.panel2.Controls.Add(this.btnPrevious);");
            sbDesigner.AppendLine("            this.panel2.Controls.Add(this.lblPaging);");
            sbDesigner.AppendLine("            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;");
            sbDesigner.AppendLine("            this.panel2.Location = new System.Drawing.Point(0, 500);");
            sbDesigner.AppendLine("            this.panel2.Name = \"panel2\";");
            sbDesigner.AppendLine("            this.panel2.Size = new System.Drawing.Size(984, 50);");
            sbDesigner.AppendLine("            this.panel2.TabIndex = 2;");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            // btnNext");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));");
            sbDesigner.AppendLine("            this.btnNext.Location = new System.Drawing.Point(877, 10);");
            sbDesigner.AppendLine("            this.btnNext.Name = \"btnNext\";");
            sbDesigner.AppendLine("            this.btnNext.Size = new System.Drawing.Size(95, 30);");
            sbDesigner.AppendLine("            this.btnNext.TabIndex = 2;");
            sbDesigner.AppendLine("            this.btnNext.Text = \"下一页 >\";");
            sbDesigner.AppendLine("            this.btnNext.UseVisualStyleBackColor = true;");
            sbDesigner.AppendLine("            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            // btnPrevious");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            this.btnPrevious.Location = new System.Drawing.Point(12, 10);");
            sbDesigner.AppendLine("            this.btnPrevious.Name = \"btnPrevious\";");
            sbDesigner.AppendLine("            this.btnPrevious.Size = new System.Drawing.Size(95, 30);");
            sbDesigner.AppendLine("            this.btnPrevious.TabIndex = 1;");
            sbDesigner.AppendLine("            this.btnPrevious.Text = \"< 上一页\";");
            sbDesigner.AppendLine("            this.btnPrevious.UseVisualStyleBackColor = true;");
            sbDesigner.AppendLine("            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            // lblPaging");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            this.lblPaging.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));");
            sbDesigner.AppendLine("            this.lblPaging.Location = new System.Drawing.Point(113, 10);");
            sbDesigner.AppendLine("            this.lblPaging.Name = \"lblPaging\";");
            sbDesigner.AppendLine("            this.lblPaging.Size = new System.Drawing.Size(758, 30);");
            sbDesigner.AppendLine("            this.lblPaging.TabIndex = 0;");
            sbDesigner.AppendLine("            this.lblPaging.Text = \"第 1 页，共 1 页\";");
            sbDesigner.AppendLine("            this.lblPaging.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine($"            // {entityName}ListForm");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);");
            sbDesigner.AppendLine("            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;");
            sbDesigner.AppendLine("            this.ClientSize = new System.Drawing.Size(984, 550);");
            sbDesigner.AppendLine("            this.Controls.Add(this.dataGridView);");
            sbDesigner.AppendLine("            this.Controls.Add(this.panel1);");
            sbDesigner.AppendLine("            this.Controls.Add(this.panel2);");
            sbDesigner.AppendLine("            this.Font = new System.Drawing.Font(\"Microsoft Sans Serif\", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));");
            sbDesigner.AppendLine("            this.Margin = new System.Windows.Forms.Padding(4);");
            sbDesigner.AppendLine("            this.MinimumSize = new System.Drawing.Size(800, 500);");
            sbDesigner.AppendLine($"            this.Name = \"{entityName}ListForm\";");
            sbDesigner.AppendLine($"            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;");
            sbDesigner.AppendLine($"            this.Text = \"{entityName} 列表\";");
            sbDesigner.AppendLine("            this.Load += new System.EventHandler(this.OnFormLoad);");
            sbDesigner.AppendLine("            this.panel1.ResumeLayout(false);");
            sbDesigner.AppendLine("            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();");
            sbDesigner.AppendLine("            this.panel2.ResumeLayout(false);");
            sbDesigner.AppendLine("            this.ResumeLayout(false);");
            sbDesigner.AppendLine("        }");
            sbDesigner.AppendLine();
            sbDesigner.AppendLine($"        #endregion");
            sbDesigner.AppendLine();
            sbDesigner.AppendLine($"        private System.Windows.Forms.Panel panel1;");
            sbDesigner.AppendLine($"        private System.Windows.Forms.Button btnExport;");
            sbDesigner.AppendLine($"        private System.Windows.Forms.Button btnDelete;");
            sbDesigner.AppendLine($"        private System.Windows.Forms.Button btnEdit;");
            sbDesigner.AppendLine($"        private System.Windows.Forms.Button btnAdd;");
            sbDesigner.AppendLine($"        private System.Windows.Forms.DataGridView dataGridView;");
            sbDesigner.AppendLine($"        private System.Windows.Forms.Panel panel2;");
            sbDesigner.AppendLine($"        private System.Windows.Forms.Button btnNext;");
            sbDesigner.AppendLine($"        private System.Windows.Forms.Button btnPrevious;");
            sbDesigner.AppendLine($"        private System.Windows.Forms.Label lblPaging;");
            sbDesigner.AppendLine("    }");
            sbDesigner.AppendLine($"}}");
            
            // 保存文件
            string formsPath = Path.Combine(_outputPath, "Forms");
            if (!Directory.Exists(formsPath))
            {
                Directory.CreateDirectory(formsPath);
            }
            
            File.WriteAllText(Path.Combine(formsPath, $"{entityName}ListForm.cs"), sbCode.ToString());
            File.WriteAllText(Path.Combine(formsPath, $"{entityName}ListForm.Designer.cs"), sbDesigner.ToString());
        }

        private void GenerateEditForm(TableInfo table)
        {
            string entityName = table.Name;
            
            // 生成表单代码文件
            var sbCode = new StringBuilder();
            
            sbCode.AppendLine("using Generated.Entities;");
            sbCode.AppendLine("using Generated.Services;");
            sbCode.AppendLine("using System;");
            sbCode.AppendLine("using System.Threading.Tasks;");
            sbCode.AppendLine("using System.Windows.Forms;");
            sbCode.AppendLine();
            sbCode.AppendLine("namespace Generated.Forms");
            sbCode.AppendLine("{");
            sbCode.AppendLine($"    public partial class {entityName}EditForm : Form");
            sbCode.AppendLine("    {");
            sbCode.AppendLine($"        private readonly I{entityName}Service _service;");
            sbCode.AppendLine($"        private {entityName} _item;");
            sbCode.AppendLine("        private bool _isNew = true;");
            sbCode.AppendLine();
            sbCode.AppendLine($"        public {entityName}EditForm(I{entityName}Service service)");
            sbCode.AppendLine("        {");
            sbCode.AppendLine("            InitializeComponent();");
            sbCode.AppendLine("            _service = service;");
            sbCode.AppendLine($"            _item = new {entityName}();");
            sbCode.AppendLine("            _isNew = true;");
            sbCode.AppendLine("        }");
            sbCode.AppendLine();
            sbCode.AppendLine($"        public {entityName}EditForm(I{entityName}Service service, {entityName} item)");
            sbCode.AppendLine("        {");
            sbCode.AppendLine("            InitializeComponent();");
            sbCode.AppendLine("            _service = service;");
            sbCode.AppendLine("            _item = item;");
            sbCode.AppendLine("            _isNew = false;");
            sbCode.AppendLine("        }");
            sbCode.AppendLine();
            sbCode.AppendLine("        private void OnFormLoad(object sender, EventArgs e)");
            sbCode.AppendLine("        {");
            sbCode.AppendLine("            if (!_isNew)");
            sbCode.AppendLine("            {");
            sbCode.AppendLine("                PopulateForm();");
            sbCode.AppendLine("            }");
            sbCode.AppendLine("        }");
            sbCode.AppendLine();
            sbCode.AppendLine("        private void PopulateForm()");
            sbCode.AppendLine("        {");
            
            // 为每个可编辑的列添加绑定
            foreach (var column in table.Columns)
            {
                if (!column.IsComputed && !column.IsIdentity)
                {
                    string controlName = $"txt{column.Name}";
                    if (column.ControlType == "CheckBox")
                    {
                        controlName = $"chk{column.Name}";
                        sbCode.AppendLine($"            {controlName}.Checked = _item.{column.Name};");
                    }
                    else if (column.ControlType == "DateTimePicker")
                    {
                        controlName = $"dtp{column.Name}";
                        sbCode.AppendLine($"            {controlName}.Value = _item.{column.Name} ?? DateTime.Now;");
                    }
                    else if (column.ControlType == "NumericUpDown")
                    {
                        controlName = $"num{column.Name}";
                        sbCode.AppendLine($"            {controlName}.Value = Convert.ToDecimal(_item.{column.Name});");
                    }
                    else if (column.ControlType == "ComboBox")
                    {
                        controlName = $"cmb{column.Name}";
                        sbCode.AppendLine($"            {controlName}.SelectedItem = _item.{column.Name};");
                    }
                    else // TextBox或其他
                    {
                        sbCode.AppendLine($"            {controlName}.Text = _item.{column.Name}?.ToString() ?? \"\";");
                    }
                }
            }
            
            sbCode.AppendLine("        }");
            sbCode.AppendLine();
            sbCode.AppendLine("        private bool ValidateForm()");
            sbCode.AppendLine("        {");
            sbCode.AppendLine("            bool isValid = true;");
            sbCode.AppendLine("            errorProvider.Clear();");
            sbCode.AppendLine();
            
            // 为必填字段添加验证
            foreach (var column in table.Columns)
            {
                if (!column.IsNullable && !column.IsIdentity && !column.IsComputed)
                {
                    string controlName = $"txt{column.Name}";
                    if (column.ControlType == "CheckBox")
                    {
                        controlName = $"chk{column.Name}";
                        // 复选框通常不需要验证
                    }
                    else if (column.ControlType == "DateTimePicker")
                    {
                        controlName = $"dtp{column.Name}";
                        // 日期选择器通常不需要验证
                    }
                    else if (column.ControlType == "NumericUpDown")
                    {
                        controlName = $"num{column.Name}";
                        // 数字控件通常不需要验证
                    }
                    else if (column.ControlType == "ComboBox")
                    {
                        controlName = $"cmb{column.Name}";
                        sbCode.AppendLine($"            if ({controlName}.SelectedItem == null)");
                        sbCode.AppendLine("            {");
                        sbCode.AppendLine($"                errorProvider.SetError({controlName}, \"{column.DisplayName} 为必填项\");");
                        sbCode.AppendLine("                isValid = false;");
                        sbCode.AppendLine("            }");
                    }
                    else // TextBox或其他
                    {
                        sbCode.AppendLine($"            if (string.IsNullOrWhiteSpace({controlName}.Text))");
                        sbCode.AppendLine("            {");
                        sbCode.AppendLine($"                errorProvider.SetError({controlName}, \"{column.DisplayName} 为必填项\");");
                        sbCode.AppendLine("                isValid = false;");
                        sbCode.AppendLine("            }");
                    }
                }
            }
            
            sbCode.AppendLine("            return isValid;");
            sbCode.AppendLine("        }");
            sbCode.AppendLine();
            sbCode.AppendLine("        private void GatherFormData()");
            sbCode.AppendLine("        {");
            
            // 从表单控件收集数据
            foreach (var column in table.Columns)
            {
                if (!column.IsComputed && !column.IsIdentity)
                {
                    string controlName = $"txt{column.Name}";
                    if (column.ControlType == "CheckBox")
                    {
                        controlName = $"chk{column.Name}";
                        sbCode.AppendLine($"            _item.{column.Name} = {controlName}.Checked;");
                    }
                    else if (column.ControlType == "DateTimePicker")
                    {
                        controlName = $"dtp{column.Name}";
                        sbCode.AppendLine($"            _item.{column.Name} = {controlName}.Value;");
                    }
                    else if (column.ControlType == "NumericUpDown")
                    {
                        controlName = $"num{column.Name}";
                        
                        // 处理不同的数值类型
                        if (column.CSharpType == typeof(int))
                        {
                            sbCode.AppendLine($"            _item.{column.Name} = (int){controlName}.Value;");
                        }
                        else if (column.CSharpType == typeof(long))
                        {
                            sbCode.AppendLine($"            _item.{column.Name} = (long){controlName}.Value;");
                        }
                        else if (column.CSharpType == typeof(decimal))
                        {
                            sbCode.AppendLine($"            _item.{column.Name} = {controlName}.Value;");
                        }
                        else if (column.CSharpType == typeof(double))
                        {
                            sbCode.AppendLine($"            _item.{column.Name} = (double){controlName}.Value;");
                        }
                        else
                        {
                            sbCode.AppendLine($"            _item.{column.Name} = Convert.ToInt32({controlName}.Value);");
                        }
                    }
                    else if (column.ControlType == "ComboBox")
                    {
                        controlName = $"cmb{column.Name}";
                        sbCode.AppendLine($"            _item.{column.Name} = {controlName}.SelectedItem?.ToString();");
                    }
                    else // TextBox或其他
                    {
                        // 处理不同的类型
                        if (column.CSharpType == typeof(string))
                        {
                            sbCode.AppendLine($"            _item.{column.Name} = {controlName}.Text;");
                        }
                        else if (column.CSharpType == typeof(int))
                        {
                            sbCode.AppendLine($"            _item.{column.Name} = string.IsNullOrWhiteSpace({controlName}.Text) ? 0 : int.Parse({controlName}.Text);");
                        }
                        else if (column.CSharpType == typeof(long))
                        {
                            sbCode.AppendLine($"            _item.{column.Name} = string.IsNullOrWhiteSpace({controlName}.Text) ? 0 : long.Parse({controlName}.Text);");
                        }
                        else if (column.CSharpType == typeof(decimal))
                        {
                            sbCode.AppendLine($"            _item.{column.Name} = string.IsNullOrWhiteSpace({controlName}.Text) ? 0 : decimal.Parse({controlName}.Text);");
                        }
                        else if (column.CSharpType == typeof(double))
                        {
                            sbCode.AppendLine($"            _item.{column.Name} = string.IsNullOrWhiteSpace({controlName}.Text) ? 0 : double.Parse({controlName}.Text);");
                        }
                        else if (column.CSharpType == typeof(DateTime))
                        {
                            sbCode.AppendLine($"            _item.{column.Name} = string.IsNullOrWhiteSpace({controlName}.Text) ? DateTime.Now : DateTime.Parse({controlName}.Text);");
                        }
                        else if (column.CSharpType == typeof(bool))
                        {
                            sbCode.AppendLine($"            _item.{column.Name} = string.IsNullOrWhiteSpace({controlName}.Text) ? false : bool.Parse({controlName}.Text);");
                        }
                        else if (column.CSharpType == typeof(Guid))
                        {
                            sbCode.AppendLine($"            _item.{column.Name} = string.IsNullOrWhiteSpace({controlName}.Text) ? Guid.Empty : Guid.Parse({controlName}.Text);");
                        }
                        else
                        {
                            sbCode.AppendLine($"            _item.{column.Name} = {controlName}.Text;");
                        }
                    }
                }
            }
            
            sbCode.AppendLine("        }");
            sbCode.AppendLine("    }");
            sbCode.AppendLine("}");

            // 生成设计器文件
            var sbDesigner = new StringBuilder();
            
            sbDesigner.AppendLine("namespace Generated.Forms");
            sbDesigner.AppendLine("{");
            sbDesigner.AppendLine($"    partial class {entityName}EditForm");
            sbDesigner.AppendLine("    {");
            sbDesigner.AppendLine("        /// <summary>");
            sbDesigner.AppendLine("        /// 必需的设计器变量。");
            sbDesigner.AppendLine("        /// </summary>");
            sbDesigner.AppendLine("        private System.ComponentModel.IContainer components = null;");
            sbDesigner.AppendLine();
            sbDesigner.AppendLine("        /// <summary>");
            sbDesigner.AppendLine("        /// 清理所有正在使用的资源。");
            sbDesigner.AppendLine("        /// </summary>");
            sbDesigner.AppendLine("        /// <param name=\"disposing\">如果应释放托管资源，为 true；否则为 false。</param>");
            sbDesigner.AppendLine("        protected override void Dispose(bool disposing)");
            sbDesigner.AppendLine("        {");
            sbDesigner.AppendLine("            if (disposing && (components != null))");
            sbDesigner.AppendLine("            {");
            sbDesigner.AppendLine("                components.Dispose();");
            sbDesigner.AppendLine("            }");
            sbDesigner.AppendLine("            base.Dispose(disposing);");
            sbDesigner.AppendLine("        }");
            sbDesigner.AppendLine();
            sbDesigner.AppendLine("        #region Windows 窗体设计器生成的代码");
            sbDesigner.AppendLine();
            sbDesigner.AppendLine("        /// <summary>");
            sbDesigner.AppendLine("        /// 设计器支持所需的方法 - 不要修改");
            sbDesigner.AppendLine("        /// 使用代码编辑器修改此方法的内容。");
            sbDesigner.AppendLine("        /// </summary>");
            sbDesigner.AppendLine("        private void InitializeComponent()");
            sbDesigner.AppendLine("        {");
            sbDesigner.AppendLine("            this.components = new System.ComponentModel.Container();");
            sbDesigner.AppendLine("            this.panel1 = new System.Windows.Forms.Panel();");
            sbDesigner.AppendLine("            this.btnCancel = new System.Windows.Forms.Button();");
            sbDesigner.AppendLine("            this.btnSave = new System.Windows.Forms.Button();");
            sbDesigner.AppendLine("            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);");
            
            // 为每个可编辑的列添加控件
            int verticalPosition = 20;
            foreach (var column in table.Columns)
            {
                if (!column.IsComputed && !column.IsIdentity)
                {
                    string labelName = $"lbl{column.Name}";
                    string controlName = $"txt{column.Name}";
                    
                    if (column.ControlType == "CheckBox")
                    {
                        controlName = $"chk{column.Name}";
                        sbDesigner.AppendLine($"            this.{controlName} = new System.Windows.Forms.CheckBox();");
                    }
                    else if (column.ControlType == "DateTimePicker")
                    {
                        controlName = $"dtp{column.Name}";
                        sbDesigner.AppendLine($"            this.{labelName} = new System.Windows.Forms.Label();");
                        sbDesigner.AppendLine($"            this.{controlName} = new System.Windows.Forms.DateTimePicker();");
                    }
                    else if (column.ControlType == "NumericUpDown")
                    {
                        controlName = $"num{column.Name}";
                        sbDesigner.AppendLine($"            this.{labelName} = new System.Windows.Forms.Label();");
                        sbDesigner.AppendLine($"            this.{controlName} = new System.Windows.Forms.NumericUpDown();");
                    }
                    else if (column.ControlType == "ComboBox")
                    {
                        controlName = $"cmb{column.Name}";
                        sbDesigner.AppendLine($"            this.{labelName} = new System.Windows.Forms.Label();");
                        sbDesigner.AppendLine($"            this.{controlName} = new System.Windows.Forms.ComboBox();");
                    }
                    else // TextBox或其他
                    {
                        sbDesigner.AppendLine($"            this.{labelName} = new System.Windows.Forms.Label();");
                        sbDesigner.AppendLine($"            this.{controlName} = new System.Windows.Forms.TextBox();");
                    }
                }
            }
            
            sbDesigner.AppendLine("            this.panel1.SuspendLayout();");
            
            // 添加对数值控件的初始化代码
            foreach (var column in table.Columns)
            {
                if (!column.IsComputed && !column.IsIdentity && column.ControlType == "NumericUpDown")
                {
                    string controlName = $"num{column.Name}";
                    sbDesigner.AppendLine($"            ((System.ComponentModel.ISupportInitialize)(this.{controlName})).BeginInit();");
                }
            }
            
            sbDesigner.AppendLine("            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();");
            sbDesigner.AppendLine("            this.SuspendLayout();");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            // panel1");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            this.panel1.Controls.Add(this.btnCancel);");
            sbDesigner.AppendLine("            this.panel1.Controls.Add(this.btnSave);");
            sbDesigner.AppendLine("            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;");
            sbDesigner.AppendLine("            this.panel1.Location = new System.Drawing.Point(0, 400);");
            sbDesigner.AppendLine("            this.panel1.Name = \"panel1\";");
            sbDesigner.AppendLine("            this.panel1.Size = new System.Drawing.Size(600, 50);");
            sbDesigner.AppendLine("            this.panel1.TabIndex = 0;");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            // btnCancel");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));");
            sbDesigner.AppendLine("            this.btnCancel.Location = new System.Drawing.Point(493, 10);");
            sbDesigner.AppendLine("            this.btnCancel.Name = \"btnCancel\";");
            sbDesigner.AppendLine("            this.btnCancel.Size = new System.Drawing.Size(95, 30);");
            sbDesigner.AppendLine("            this.btnCancel.TabIndex = 1;");
            sbDesigner.AppendLine("            this.btnCancel.Text = \"取消\";");
            sbDesigner.AppendLine("            this.btnCancel.UseVisualStyleBackColor = true;");
            sbDesigner.AppendLine("            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            // btnSave");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));");
            sbDesigner.AppendLine("            this.btnSave.Location = new System.Drawing.Point(392, 10);");
            sbDesigner.AppendLine("            this.btnSave.Name = \"btnSave\";");
            sbDesigner.AppendLine("            this.btnSave.Size = new System.Drawing.Size(95, 30);");
            sbDesigner.AppendLine("            this.btnSave.TabIndex = 0;");
            sbDesigner.AppendLine("            this.btnSave.Text = \"保存\";");
            sbDesigner.AppendLine("            this.btnSave.UseVisualStyleBackColor = true;");
            sbDesigner.AppendLine("            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            // errorProvider");
            sbDesigner.AppendLine("            // ");
            sbDesigner.AppendLine("            this.errorProvider.ContainerControl = this;");
            
            // 为每个可编辑的列添加控件的布局
            verticalPosition = 20;
            int tabIndex = 0;
            
            foreach (var column in table.Columns)
            {
                if (!column.IsComputed && !column.IsIdentity)
                {
                    string labelName = $"lbl{column.Name}";
                    string controlName = $"txt{column.Name}";
                    string displayName = column.DisplayName ?? column.Name;
                    
                    if (column.ControlType == "CheckBox")
                    {
                        controlName = $"chk{column.Name}";
                        
                        sbDesigner.AppendLine("            // ");
                        sbDesigner.AppendLine($"            // {controlName}");
                        sbDesigner.AppendLine("            // ");
                        sbDesigner.AppendLine($"            this.{controlName}.AutoSize = true;");
                        sbDesigner.AppendLine($"            this.{controlName}.Location = new System.Drawing.Point(120, {verticalPosition});");
                        sbDesigner.AppendLine($"            this.{controlName}.Name = \"{controlName}\";");
                        sbDesigner.AppendLine($"            this.{controlName}.Size = new System.Drawing.Size(150, 20);");
                        sbDesigner.AppendLine($"            this.{controlName}.TabIndex = {tabIndex++};");
                        sbDesigner.AppendLine($"            this.{controlName}.Text = \"{displayName}\";");
                        sbDesigner.AppendLine($"            this.{controlName}.UseVisualStyleBackColor = true;");
                        
                        verticalPosition += 30;
                    }
                    else if (column.ControlType == "DateTimePicker" || column.ControlType == "NumericUpDown" || column.ControlType == "ComboBox" || column.ControlType == "TextBox")
                    {
                        sbDesigner.AppendLine("            // ");
                        sbDesigner.AppendLine($"            // {labelName}");
                        sbDesigner.AppendLine("            // ");
                        sbDesigner.AppendLine($"            this.{labelName}.AutoSize = true;");
                        sbDesigner.AppendLine($"            this.{labelName}.Location = new System.Drawing.Point(12, {verticalPosition + 3});");
                        sbDesigner.AppendLine($"            this.{labelName}.Name = \"{labelName}\";");
                        sbDesigner.AppendLine($"            this.{labelName}.Size = new System.Drawing.Size(100, 17);");
                        sbDesigner.AppendLine($"            this.{labelName}.TabIndex = {tabIndex++};");
                        sbDesigner.AppendLine($"            this.{labelName}.Text = \"{displayName}:\";");
                        
                        if (column.ControlType == "DateTimePicker")
                        {
                            controlName = $"dtp{column.Name}";
                            
                            sbDesigner.AppendLine("            // ");
                            sbDesigner.AppendLine($"            // {controlName}");
                            sbDesigner.AppendLine("            // ");
                            sbDesigner.AppendLine($"            this.{controlName}.Format = System.Windows.Forms.DateTimePickerFormat.Short;");
                            sbDesigner.AppendLine($"            this.{controlName}.Location = new System.Drawing.Point(120, {verticalPosition});");
                            sbDesigner.AppendLine($"            this.{controlName}.Name = \"{controlName}\";");
                            sbDesigner.AppendLine($"            this.{controlName}.Size = new System.Drawing.Size(200, 23);");
                            sbDesigner.AppendLine($"            this.{controlName}.TabIndex = {tabIndex++};");
                        }
                        else if (column.ControlType == "NumericUpDown")
                        {
                            controlName = $"num{column.Name}";
                            
                            sbDesigner.AppendLine("            // ");
                            sbDesigner.AppendLine($"            // {controlName}");
                            sbDesigner.AppendLine("            // ");
                            sbDesigner.AppendLine($"            this.{controlName}.Location = new System.Drawing.Point(120, {verticalPosition});");
                            sbDesigner.AppendLine($"            this.{controlName}.Name = \"{controlName}\";");
                            sbDesigner.AppendLine($"            this.{controlName}.Size = new System.Drawing.Size(200, 23);");
                            sbDesigner.AppendLine($"            this.{controlName}.TabIndex = {tabIndex++};");
                            
                            // 根据列类型设置数值控件的属性
                            if (column.CSharpType == typeof(decimal) || column.CSharpType == typeof(double))
                            {
                                sbDesigner.AppendLine($"            this.{controlName}.DecimalPlaces = {column.Scale};");
                            }
                        }
                        else if (column.ControlType == "ComboBox")
                        {
                            controlName = $"cmb{column.Name}";
                            
                            sbDesigner.AppendLine("            // ");
                            sbDesigner.AppendLine($"            // {controlName}");
                            sbDesigner.AppendLine("            // ");
                            sbDesigner.AppendLine($"            this.{controlName}.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;");
                            sbDesigner.AppendLine($"            this.{controlName}.FormattingEnabled = true;");
                            sbDesigner.AppendLine($"            this.{controlName}.Location = new System.Drawing.Point(120, {verticalPosition});");
                            sbDesigner.AppendLine($"            this.{controlName}.Name = \"{controlName}\";");
                            sbDesigner.AppendLine($"            this.{controlName}.Size = new System.Drawing.Size(200, 24);");
                            sbDesigner.AppendLine($"            this.{controlName}.TabIndex = {tabIndex++};");
                        }
                        else // TextBox
                        {
                            sbDesigner.AppendLine("            // ");
                            sbDesigner.AppendLine($"            // {controlName}");
                            sbDesigner.AppendLine("            // ");
                            sbDesigner.AppendLine($"            this.{controlName}.Location = new System.Drawing.Point(120, {verticalPosition});");
                            sbDesigner.AppendLine($"            this.{controlName}.Name = \"{controlName}\";");
                            
                            // 文本区域的特殊处理
                            if (column.ControlType == "TextArea")
                            {
                                sbDesigner.AppendLine($"            this.{controlName}.Multiline = true;");
                                sbDesigner.AppendLine($"            this.{controlName}.Size = new System.Drawing.Size(450, 80);");
                                verticalPosition += 60; // 增加更多空间
                            }
                            else
                            {
                                sbDesigner.AppendLine($"            this.{controlName}.Size = new System.Drawing.Size(450, 23);");
                            }
                            
                            sbDesigner.AppendLine($"            this.{controlName}.TabIndex = {tabIndex++};");
                            
                            // 如果有最大长度限制，添加该属性
                            if (column.MaxLength > 0 && column.CSharpType == typeof(string))
                            {
                                sbDesigner.AppendLine($"            this.{controlName}.MaxLength = {column.MaxLength};");
                            }
                        }
                        
                        verticalPosition += 35;
                    }
                }
            }
            
            // 编辑表单
            int formHeight = verticalPosition + 100; // 确保有足够的空间
            
            sbDesigner.AppendLine($"            // ");
            sbDesigner.AppendLine($"            // {entityName}EditForm");
            sbDesigner.AppendLine($"            // ");
            sbDesigner.AppendLine($"            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);");
            sbDesigner.AppendLine($"            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;");
            sbDesigner.AppendLine($"            this.ClientSize = new System.Drawing.Size(600, {formHeight});");
            sbDesigner.AppendLine($"            this.Controls.Add(this.panel1);");
            
            // 添加所有控件
            foreach (var column in table.Columns)
            {
                if (!column.IsComputed && !column.IsIdentity)
                {
                    string labelName = $"lbl{column.Name}";
                    string controlName = $"txt{column.Name}";
                    
                    if (column.ControlType == "CheckBox")
                    {
                        controlName = $"chk{column.Name}";
                        sbDesigner.AppendLine($"            this.Controls.Add(this.{controlName});");
                    }
                    else if (column.ControlType == "DateTimePicker")
                    {
                        controlName = $"dtp{column.Name}";
                        sbDesigner.AppendLine($"            this.Controls.Add(this.{controlName});");
                        sbDesigner.AppendLine($"            this.Controls.Add(this.{labelName});");
                    }
                    else if (column.ControlType == "NumericUpDown")
                    {
                        controlName = $"num{column.Name}";
                        sbDesigner.AppendLine($"            this.Controls.Add(this.{controlName});");
                        sbDesigner.AppendLine($"            this.Controls.Add(this.{labelName});");
                    }
                    else if (column.ControlType == "ComboBox")
                    {
                        controlName = $"cmb{column.Name}";
                        sbDesigner.AppendLine($"            this.Controls.Add(this.{controlName});");
                        sbDesigner.AppendLine($"            this.Controls.Add(this.{labelName});");
                    }
                    else // TextBox或其他
                    {
                        sbDesigner.AppendLine($"            this.Controls.Add(this.{controlName});");
                        sbDesigner.AppendLine($"            this.Controls.Add(this.{labelName});");
                    }
                }
            }
            
            sbDesigner.AppendLine($"            this.Font = new System.Drawing.Font(\"Microsoft Sans Serif\", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));");
            sbDesigner.AppendLine($"            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;");
            sbDesigner.AppendLine($"            this.Margin = new System.Windows.Forms.Padding(4);");
            sbDesigner.AppendLine($"            this.MaximizeBox = false;");
            sbDesigner.AppendLine($"            this.MinimizeBox = false;");
            sbDesigner.AppendLine($"            this.Name = \"{entityName}EditForm\";");
            sbDesigner.AppendLine($"            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;");
            sbDesigner.AppendLine($"            this.Text = \"编辑{entityName}\";");
            sbDesigner.AppendLine($"            this.Load += new System.EventHandler(this.OnFormLoad);");
            sbDesigner.AppendLine($"            this.panel1.ResumeLayout(false);");
            
            // 添加数值控件的初始化结束代码
            foreach (var column in table.Columns)
            {
                if (!column.IsComputed && !column.IsIdentity && column.ControlType == "NumericUpDown")
                {
                    string controlName = $"num{column.Name}";
                    sbDesigner.AppendLine($"            ((System.ComponentModel.ISupportInitialize)(this.{controlName})).EndInit();");
                }
            }
            
            sbDesigner.AppendLine($"            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();");
            sbDesigner.AppendLine($"            this.ResumeLayout(false);");
            sbDesigner.AppendLine($"            this.PerformLayout();");
            sbDesigner.AppendLine($"        }}");
            sbDesigner.AppendLine();
            sbDesigner.AppendLine($"        #endregion");
            sbDesigner.AppendLine();
            sbDesigner.AppendLine($"        private System.Windows.Forms.Panel panel1;");
            sbDesigner.AppendLine($"        private System.Windows.Forms.Button btnCancel;");
            sbDesigner.AppendLine($"        private System.Windows.Forms.Button btnSave;");
            sbDesigner.AppendLine($"        private System.Windows.Forms.ErrorProvider errorProvider;");
            
            // 声明所有控件
            foreach (var column in table.Columns)
            {
                if (!column.IsComputed && !column.IsIdentity)
                {
                    string labelName = $"lbl{column.Name}";
                    string controlName = $"txt{column.Name}";
                    
                    if (column.ControlType == "CheckBox")
                    {
                        controlName = $"chk{column.Name}";
                        sbDesigner.AppendLine($"        private System.Windows.Forms.CheckBox {controlName};");
                    }
                    else if (column.ControlType == "DateTimePicker")
                    {
                        controlName = $"dtp{column.Name}";
                        sbDesigner.AppendLine($"        private System.Windows.Forms.Label {labelName};");
                        sbDesigner.AppendLine($"        private System.Windows.Forms.DateTimePicker {controlName};");
                    }
                    else if (column.ControlType == "NumericUpDown")
                    {
                        controlName = $"num{column.Name}";
                        sbDesigner.AppendLine($"        private System.Windows.Forms.Label {labelName};");
                        sbDesigner.AppendLine($"        private System.Windows.Forms.NumericUpDown {controlName};");
                    }
                    else if (column.ControlType == "ComboBox")
                    {
                        controlName = $"cmb{column.Name}";
                        sbDesigner.AppendLine($"        private System.Windows.Forms.Label {labelName};");
                        sbDesigner.AppendLine($"        private System.Windows.Forms.ComboBox {controlName};");
                    }
                    else // TextBox或其他
                    {
                        sbDesigner.AppendLine($"        private System.Windows.Forms.Label {labelName};");
                        sbDesigner.AppendLine($"        private System.Windows.Forms.TextBox {controlName};");
                    }
                }
            }
            
            sbDesigner.AppendLine($"    }}");
            sbDesigner.AppendLine($"}}");
            
            // 保存文件
            string formsPath = Path.Combine(_outputPath, "Forms");
            if (!Directory.Exists(formsPath))
            {
                Directory.CreateDirectory(formsPath);
            }
            
            File.WriteAllText(Path.Combine(formsPath, $"{entityName}EditForm.cs"), sbCode.ToString());
            File.WriteAllText(Path.Combine(formsPath, $"{entityName}EditForm.Designer.cs"), sbDesigner.ToString());
        }
    }
}
