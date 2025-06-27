using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeGenerator.Models;
using CodeGenerator.Services;

namespace CodeGenerator
{
    public partial class MainForm : Form
    {
        private readonly DatabaseService _databaseService;
        private  CodeGenerationService _codeGenerationService;
        private DatabaseConfig _databaseConfig;
        private TableInfo _selectedTable;
        private string _outputPath;

        public MainForm()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _outputPath = Path.Combine(Application.StartupPath, "Generated");
            _codeGenerationService = new CodeGenerationService(_outputPath);
            _databaseConfig = new DatabaseConfig();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            cmbDatabaseType.Items.AddRange(Enum.GetNames(typeof(DatabaseType)));
            cmbDatabaseType.SelectedIndex = 0;
            
            txtOutputPath.Text = _outputPath;
            UpdateConnectionControls();
        }

        private void UpdateConnectionControls()
        {
            // 根据所选数据库类型显示/隐藏相关控件
            bool isSqlite = cmbDatabaseType.SelectedIndex == (int)DatabaseType.SQLite;
            
            lblServer.Visible = !isSqlite;
            txtServer.Visible = !isSqlite;
            lblPort.Visible = !isSqlite;
            txtPort.Visible = !isSqlite;
            lblUsername.Visible = !isSqlite;
            txtUsername.Visible = !isSqlite;
            lblPassword.Visible = !isSqlite;
            txtPassword.Visible = !isSqlite;
            
            if (isSqlite)
            {
                lblDatabase.Text = "数据库文件:";
                btnBrowse.Visible = true;
            }
            else
            {
                lblDatabase.Text = "数据库名称:";
                btnBrowse.Visible = false;
            }
        }

        private void cmbDatabaseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateConnectionControls();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "SQLite 数据库|*.db;*.sqlite;*.sqlite3|所有文件|*.*";
                dialog.Title = "选择 SQLite 数据库文件";
                
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtDatabase.Text = dialog.FileName;
                }
            }
        }

        private void btnOutputPath_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "选择代码输出路径";
                dialog.SelectedPath = _outputPath;
                
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _outputPath = dialog.SelectedPath;
                    txtOutputPath.Text = _outputPath;
                    _codeGenerationService = new CodeGenerationService(_outputPath);
                }
            }
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            if (!ValidateConnectionInfo())
                return;
            
            try
            {
                _databaseConfig = new DatabaseConfig
                {
                    DatabaseType = (DatabaseType)cmbDatabaseType.SelectedIndex,
                    Server = txtServer.Text,
                    Port = string.IsNullOrEmpty(txtPort.Text) ? 0 : int.Parse(txtPort.Text),
                    Username = txtUsername.Text,
                    Password = txtPassword.Text,
                    Database = txtDatabase.Text
                };
                
                btnConnect.Enabled = false;
                lblStatus.Text = "正在连接数据库...";
                Application.DoEvents();
                
                bool isConnected = await _databaseService.TestConnectionAsync(_databaseConfig);
                
                if (isConnected)
                {
                    lblStatus.Text = "连接成功";
                    await LoadTablesAsync();
                    tabControl.SelectedIndex = 1; // 切换到表格选择选项卡
                }
                else
                {
                    lblStatus.Text = "连接失败";
                    MessageBox.Show("无法连接到数据库，请检查连接信息。", "连接错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "连接错误";
                MessageBox.Show($"连接数据库时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnConnect.Enabled = true;
            }
        }

        private bool ValidateConnectionInfo()
        {
            errorProvider.Clear();
            bool isValid = true;
            
            if (cmbDatabaseType.SelectedIndex == -1)
            {
                errorProvider.SetError(cmbDatabaseType, "请选择数据库类型");
                isValid = false;
            }
            
            if (cmbDatabaseType.SelectedIndex != (int)DatabaseType.SQLite)
            {
                if (string.IsNullOrWhiteSpace(txtServer.Text))
                {
                    errorProvider.SetError(txtServer, "请输入服务器地址");
                    isValid = false;
                }
                
                if (!string.IsNullOrWhiteSpace(txtPort.Text) && !int.TryParse(txtPort.Text, out _))
                {
                    errorProvider.SetError(txtPort, "端口必须是有效的数字");
                    isValid = false;
                }
            }
            
            if (string.IsNullOrWhiteSpace(txtDatabase.Text))
            {
                errorProvider.SetError(txtDatabase, "请输入数据库名称");
                isValid = false;
            }
            
            return isValid;
        }

        private async Task LoadTablesAsync()
        {
            try
            {
                lstTables.Items.Clear();
                var tables = await _databaseService.GetTablesAsync(_databaseConfig);
                
                foreach (var table in tables)
                {
                    lstTables.Items.Add(table.Name);
                }
                
                if (tables.Count > 0)
                {
                    btnLoadTable.Enabled = true;
                }
                else
                {
                    btnLoadTable.Enabled = false;
                    MessageBox.Show("数据库中没有找到表", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载表时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnLoadTable_Click(object sender, EventArgs e)
        {
            if (lstTables.SelectedIndex == -1)
            {
                MessageBox.Show("请选择一个表", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            string tableName = lstTables.SelectedItem.ToString();
            string schemaName = "dbo"; // 默认为SQL Server的dbo模式
            
            try
            {
                btnLoadTable.Enabled = false;
                lblTableStatus.Text = "正在加载表结构...";
                Application.DoEvents();
                
                var columns = await _databaseService.GetColumnsAsync(_databaseConfig, tableName, schemaName);
                
                _selectedTable = new TableInfo(tableName, schemaName);
                _selectedTable.Columns = columns;
                
                // 显示字段
                lstColumns.Items.Clear();
                foreach (var column in columns)
                {
                    lstColumns.Items.Add($"{column.Name} ({column.DbType})");
                }
                
                // 启用字段配置和代码生成选项卡
                tabControl.SelectedIndex = 2; // 切换到字段配置选项卡
                btnGenerateCode.Enabled = true;
                lblTableStatus.Text = $"已加载表 {tableName}";
            }
            catch (Exception ex)
            {
                lblTableStatus.Text = "加载表结构错误";
                MessageBox.Show($"加载表结构时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLoadTable.Enabled = true;
            }
        }

        private void lstColumns_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstColumns.SelectedIndex != -1 && _selectedTable != null)
            {
                var column = _selectedTable.Columns[lstColumns.SelectedIndex];
                
                txtColumnName.Text = column.Name;
                txtDisplayName.Text = column.DisplayName;
                chkVisible.Checked = column.IsVisible;
                chkReadOnly.Checked = column.IsReadOnly;
                
                cmbControlType.Items.Clear();
                cmbControlType.Items.AddRange(new[] { "TextBox", "TextArea", "NumericUpDown", "CheckBox", "DateTimePicker", "ComboBox" });
                cmbControlType.SelectedItem = column.ControlType;
                
                // 显示列属性
                lblColumnProperties.Text = $"数据类型: {column.DbType}\n" +
                                        $"C# 类型: {column.CSharpType?.Name ?? "未知"}\n" +
                                        $"允许空值: {(column.IsNullable ? "是" : "否")}\n" +
                                        $"主键: {(column.IsPrimaryKey ? "是" : "否")}\n" +
                                        $"外键: {(column.IsForeignKey ? "是" : "否")}\n" +
                                        $"自增: {(column.IsIdentity ? "是" : "否")}\n" +
                                        $"计算列: {(column.IsComputed ? "是" : "否")}";
                
                grpColumnProperties.Enabled = true;
            }
            else
            {
                grpColumnProperties.Enabled = false;
            }
        }

        private void btnApplyColumnSettings_Click(object sender, EventArgs e)
        {
            if (lstColumns.SelectedIndex != -1 && _selectedTable != null)
            {
                var column = _selectedTable.Columns[lstColumns.SelectedIndex];
                
                column.DisplayName = txtDisplayName.Text;
                column.IsVisible = chkVisible.Checked;
                column.IsReadOnly = chkReadOnly.Checked;
                column.ControlType = cmbControlType.SelectedItem?.ToString() ?? "TextBox";
                
                // 更新列表显示
                lstColumns.Items[lstColumns.SelectedIndex] = $"{column.Name} ({column.DbType}) - {column.DisplayName}";
                
                MessageBox.Show("列设置已更新", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnGenerateCode_Click(object sender, EventArgs e)
        {
            if (_selectedTable == null || _selectedTable.Columns.Count == 0)
            {
                MessageBox.Show("请先加载表结构", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            try
            {
                // 收集生成选项
                bool generateEntity = chkEntity.Checked;
                bool generateDbContext = chkDbContext.Checked;
                bool generateBaseRepo = chkBaseRepo.Checked;
                bool generateRepo = chkRepo.Checked;
                bool generateBaseService = chkBaseService.Checked;
                bool generateService = chkService.Checked;
                bool generateUI = chkUI.Checked;
                
                if (!generateEntity && !generateDbContext && !generateBaseRepo && !generateRepo && 
                    !generateBaseService && !generateService && !generateUI)
                {
                    MessageBox.Show("请至少选择一个代码生成选项", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                
                // 开始生成
                lblGenerateStatus.Text = "正在生成代码...";
                Application.DoEvents();
                
                if (generateEntity)
                {
                    _codeGenerationService.GenerateEntity(_selectedTable);
                }
                
                if (generateDbContext)
                {
                    string contextName = txtContextName.Text;
                    if (string.IsNullOrWhiteSpace(contextName))
                    {
                        contextName = "ApplicationDbContext";
                    }
                    
                    _codeGenerationService.GenerateDbContext(contextName, new List<TableInfo> { _selectedTable });
                }
                
                if (generateBaseRepo)
                {
                    _codeGenerationService.GenerateBaseRepository();
                }
                
                if (generateRepo)
                {
                    _codeGenerationService.GenerateRepository(_selectedTable);
                }
                
                if (generateBaseService)
                {
                    _codeGenerationService.GenerateBaseService();
                }
                
                if (generateService)
                {
                    _codeGenerationService.GenerateService(_selectedTable);
                }
                
                if (generateUI)
                {
                    _codeGenerationService.GenerateWinFormUI(_selectedTable);
                }
                
                lblGenerateStatus.Text = "代码生成完成";
                MessageBox.Show($"代码已成功生成到: {_outputPath}", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblGenerateStatus.Text = "代码生成错误";
                MessageBox.Show($"生成代码时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOpenOutputFolder_Click(object sender, EventArgs e)
        {
            try
            {
                if (Directory.Exists(_outputPath))
                {
                    System.Diagnostics.Process.Start("explorer.exe", _outputPath);
                }
                else
                {
                    MessageBox.Show("输出目录不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开输出目录时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
} 