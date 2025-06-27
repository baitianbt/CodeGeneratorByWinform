using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using CodeGenerator.Models;

namespace CodeGenerator.Forms
{
    public partial class Sys_UserListForm : Form
    {
        private List<Sys_User> _users;
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalPages = 1;
        private string _searchKeyword = string.Empty;

        public Sys_UserListForm()
        {
            InitializeComponent();
            InitializeStyle();
        }

        private void Sys_UserListForm_Load(object sender, EventArgs e)
        {
            // 初始化页码大小下拉框
            cmbPageSize.SelectedIndex = 0;
            
            // 加载数据
            LoadData();
        }

        private void InitializeStyle()
        {
            // 设置DataGridView样式
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            dataGridView.ColumnHeadersHeight = 40;
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            
            // 设置按钮样式
            btnSearch.FlatAppearance.BorderSize = 0;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnExport.FlatAppearance.BorderSize = 0;
            btnPrev.FlatAppearance.BorderSize = 0;
            btnNext.FlatAppearance.BorderSize = 0;
        }

        private void LoadData()
        {
            // 模拟加载数据，实际应用中应该从数据库获取
            if (_users == null)
            {
                _users = new List<Sys_User>();
                for (int i = 1; i <= 100; i++)
                {
                    _users.Add(new Sys_User
                    {
                        Id = i,
                        Username = $"user{i}",
                        RealName = $"用户{i}",
                        Email = $"user{i}@example.com",
                        Phone = $"1380000{i.ToString().PadLeft(4, '0')}",
                        IsActive = i % 5 != 0,
                        CreateTime = DateTime.Now.AddDays(-i)
                    });
                }
            }

            // 筛选
            var filteredUsers = _users;
            if (!string.IsNullOrWhiteSpace(_searchKeyword))
            {
                filteredUsers = _users.Where(u => 
                    u.Username.Contains(_searchKeyword) || 
                    u.RealName.Contains(_searchKeyword) || 
                    (u.Email != null && u.Email.Contains(_searchKeyword)) || 
                    (u.Phone != null && u.Phone.Contains(_searchKeyword))
                ).ToList();
            }

            // 计算分页
            _totalPages = (filteredUsers.Count + _pageSize - 1) / _pageSize;
            if (_currentPage > _totalPages && _totalPages > 0)
            {
                _currentPage = _totalPages;
            }

            // 获取当前页数据
            var pagedUsers = filteredUsers
                .Skip((_currentPage - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();

            // 更新数据源
            dataGridView.Rows.Clear();
            
            foreach (var user in pagedUsers)
            {
                int rowIndex = dataGridView.Rows.Add();
                DataGridViewRow row = dataGridView.Rows[rowIndex];
                row.Cells[colId.Index].Value = user.Id;
                row.Cells[colUsername.Index].Value = user.Username;
                row.Cells[colRealName.Index].Value = user.RealName;
                row.Cells[colEmail.Index].Value = user.Email;
                row.Cells[colPhone.Index].Value = user.Phone;
                row.Cells[colIsActive.Index].Value = user.IsActive ? "启用" : "禁用";
                row.Cells[colCreateTime.Index].Value = user.CreateTime.ToString("yyyy-MM-dd HH:mm");
                
                // 设置启用/禁用单元格的颜色
                if (user.IsActive)
                {
                    row.Cells[colIsActive.Index].Style.ForeColor = Color.Green;
                }
                else
                {
                    row.Cells[colIsActive.Index].Style.ForeColor = Color.Red;
                }
            }

            // 更新分页信息
            labelPageInfo.Text = $"第 {_currentPage} 页，共 {_totalPages} 页，总计 {filteredUsers.Count} 条记录";
            
            // 更新分页按钮状态
            btnPrev.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage < _totalPages;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            _searchKeyword = txtSearch.Text.Trim();
            _currentPage = 1;
            LoadData();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadData();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                LoadData();
            }
        }

        private void cmbPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            _pageSize = Convert.ToInt32(cmbPageSize.SelectedItem);
            _currentPage = 1;
            LoadData();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var form = new Sys_UserEditForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                // 添加新用户
                int newId = _users.Count > 0 ? _users.Max(u => u.Id) + 1 : 1;
                _users.Add(new Sys_User
                {
                    Id = newId,
                    Username = form.UserData.Username,
                    RealName = form.UserData.RealName,
                    Email = form.UserData.Email,
                    Phone = form.UserData.Phone,
                    IsActive = form.UserData.IsActive,
                    CreateTime = DateTime.Now
                });
                LoadData();
            }
        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // 编辑按钮列
            if (e.ColumnIndex == colEdit.Index)
            {
                int userId = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells[colId.Index].Value);
                var user = _users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    EditUser(user);
                }
            }
            // 删除按钮列
            else if (e.ColumnIndex == colDelete.Index)
            {
                int userId = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells[colId.Index].Value);
                DeleteUser(userId);
            }
        }

        private void EditUser(Sys_User user)
        {
            var form = new Sys_UserEditForm(user);
            if (form.ShowDialog() == DialogResult.OK)
            {
                // 更新用户信息
                var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
                if (existingUser != null)
                {
                    existingUser.Username = form.UserData.Username;
                    existingUser.RealName = form.UserData.RealName;
                    existingUser.Email = form.UserData.Email;
                    existingUser.Phone = form.UserData.Phone;
                    existingUser.IsActive = form.UserData.IsActive;
                    LoadData();
                }
            }
        }

        private void DeleteUser(int userId)
        {
            if (MessageBox.Show("确定要删除此用户吗？", "确认删除", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var user = _users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    _users.Remove(user);
                    LoadData();
                }
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (_users == null || _users.Count == 0)
            {
                MessageBox.Show("没有可导出的数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel文件|*.xlsx|CSV文件|*.csv",
                Title = "导出用户数据",
                FileName = $"用户数据_{DateTime.Now:yyyyMMdd}"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (saveFileDialog.FileName.EndsWith(".xlsx"))
                    {
                        ExportToExcel(saveFileDialog.FileName);
                    }
                    else if (saveFileDialog.FileName.EndsWith(".csv"))
                    {
                        ExportToCsv(saveFileDialog.FileName);
                    }
                    MessageBox.Show("数据导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"导出失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ExportToExcel(string filePath)
        {
            using (var workbook = new XSSFWorkbook())
            {
                var sheet = workbook.CreateSheet("用户数据");

                // 创建标题行
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("ID");
                headerRow.CreateCell(1).SetCellValue("用户名");
                headerRow.CreateCell(2).SetCellValue("姓名");
                headerRow.CreateCell(3).SetCellValue("邮箱");
                headerRow.CreateCell(4).SetCellValue("电话");
                headerRow.CreateCell(5).SetCellValue("状态");
                headerRow.CreateCell(6).SetCellValue("创建时间");

                // 添加数据行
                int rowIndex = 1;
                foreach (var user in _users)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(user.Id);
                    row.CreateCell(1).SetCellValue(user.Username);
                    row.CreateCell(2).SetCellValue(user.RealName ?? "");
                    row.CreateCell(3).SetCellValue(user.Email ?? "");
                    row.CreateCell(4).SetCellValue(user.Phone ?? "");
                    row.CreateCell(5).SetCellValue(user.IsActive ? "启用" : "禁用");
                    row.CreateCell(6).SetCellValue(user.CreateTime.ToString("yyyy-MM-dd HH:mm"));
                }

                // 自动调整列宽
                for (int i = 0; i < 7; i++)
                {
                    sheet.AutoSizeColumn(i);
                }

                // 保存文件
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fileStream);
                }
            }
        }

        private void ExportToCsv(string filePath)
        {
            using (var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                // 写入标题行
                writer.WriteLine("ID,用户名,姓名,邮箱,电话,状态,创建时间");

                // 写入数据行
                foreach (var user in _users)
                {
                    writer.WriteLine($"{user.Id},{user.Username},{user.RealName ?? ""},{user.Email ?? ""},{user.Phone ?? ""},{(user.IsActive ? "启用" : "禁用")},{user.CreateTime:yyyy-MM-dd HH:mm}");
                }
            }
        }
    }

    // 用户模型类
    public class Sys_User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string RealName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateTime { get; set; }
    }
} 