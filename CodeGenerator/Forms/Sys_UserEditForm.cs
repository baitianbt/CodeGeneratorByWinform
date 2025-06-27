using System;
using System.Drawing;
using System.Windows.Forms;

namespace CodeGenerator.Forms
{
    public partial class Sys_UserEditForm : Form
    {
        public Sys_User UserData { get; private set; }
        private bool _isNew = true;

        public Sys_UserEditForm()
        {
            InitializeComponent();
            UserData = new Sys_User();
            _isNew = true;
            this.Text = "新增用户";
        }

        public Sys_UserEditForm(Sys_User user)
        {
            InitializeComponent();
            UserData = new Sys_User
            {
                Id = user.Id,
                Username = user.Username,
                RealName = user.RealName,
                Email = user.Email,
                Phone = user.Phone,
                IsActive = user.IsActive
            };
            _isNew = false;
            this.Text = "编辑用户";
        }

        private void Sys_UserEditForm_Load(object sender, EventArgs e)
        {
            if (!_isNew)
            {
                txtUsername.Text = UserData.Username;
                txtRealName.Text = UserData.RealName;
                txtEmail.Text = UserData.Email;
                txtPhone.Text = UserData.Phone;
                chkIsActive.Checked = UserData.IsActive;
                
                // 用户名不可修改
                txtUsername.Enabled = false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                UserData.Username = txtUsername.Text.Trim();
                UserData.RealName = txtRealName.Text.Trim();
                UserData.Email = txtEmail.Text.Trim();
                UserData.Phone = txtPhone.Text.Trim();
                UserData.IsActive = chkIsActive.Checked;

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private bool ValidateInput()
        {
            errorProvider.Clear();
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                errorProvider.SetError(txtUsername, "用户名不能为空");
                isValid = false;
            }
            else if (txtUsername.Text.Length < 3)
            {
                errorProvider.SetError(txtUsername, "用户名长度不能小于3个字符");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtRealName.Text))
            {
                errorProvider.SetError(txtRealName, "姓名不能为空");
                isValid = false;
            }

            if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !IsValidEmail(txtEmail.Text))
            {
                errorProvider.SetError(txtEmail, "邮箱格式不正确");
                isValid = false;
            }

            if (!string.IsNullOrWhiteSpace(txtPhone.Text) && !IsValidPhone(txtPhone.Text))
            {
                errorProvider.SetError(txtPhone, "手机号格式不正确");
                isValid = false;
            }

            return isValid;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^1[3-9]\d{9}$");
        }
    }
} 