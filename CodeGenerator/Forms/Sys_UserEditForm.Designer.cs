using System;
using System.Windows.Forms;

namespace CodeGenerator.Forms
{
    partial class Sys_UserEditForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            panel1 = new Panel();
            lblTitle = new Label();
            panel2 = new Panel();
            btnCancel = new Button();
            btnSave = new Button();
            panel3 = new Panel();
            chkIsActive = new CheckBox();
            txtPhone = new TextBox();
            lblPhone = new Label();
            txtEmail = new TextBox();
            lblEmail = new Label();
            txtRealName = new TextBox();
            lblRealName = new Label();
            txtUsername = new TextBox();
            lblUsername = new Label();
            errorProvider = new ErrorProvider(components);
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(0, 122, 204);
            panel1.Controls.Add(lblTitle);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(479, 60);
            panel1.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(74, 22);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "用户信息";
            // 
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(240, 240, 240);
            panel2.Controls.Add(btnCancel);
            panel2.Controls.Add(btnSave);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 319);
            panel2.Name = "panel2";
            panel2.Size = new Size(479, 60);
            panel2.TabIndex = 1;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancel.BackColor = Color.FromArgb(230, 230, 230);
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Microsoft YaHei UI", 9F);
            btnCancel.Location = new Point(277, 15);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(90, 30);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "取消";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSave.BackColor = Color.FromArgb(0, 122, 204);
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Font = new Font("Microsoft YaHei UI", 9F);
            btnSave.ForeColor = Color.White;
            btnSave.Location = new Point(377, 15);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(90, 30);
            btnSave.TabIndex = 0;
            btnSave.Text = "保存";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // panel3
            // 
            panel3.BackColor = Color.White;
            panel3.Controls.Add(chkIsActive);
            panel3.Controls.Add(txtPhone);
            panel3.Controls.Add(lblPhone);
            panel3.Controls.Add(txtEmail);
            panel3.Controls.Add(lblEmail);
            panel3.Controls.Add(txtRealName);
            panel3.Controls.Add(lblRealName);
            panel3.Controls.Add(txtUsername);
            panel3.Controls.Add(lblUsername);
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(0, 60);
            panel3.Name = "panel3";
            panel3.Size = new Size(479, 259);
            panel3.TabIndex = 2;
            // 
            // chkIsActive
            // 
            chkIsActive.AutoSize = true;
            chkIsActive.Checked = true;
            chkIsActive.CheckState = CheckState.Checked;
            chkIsActive.Font = new Font("Microsoft YaHei UI", 9F);
            chkIsActive.Location = new Point(120, 220);
            chkIsActive.Name = "chkIsActive";
            chkIsActive.Size = new Size(75, 21);
            chkIsActive.TabIndex = 8;
            chkIsActive.Text = "启用账户";
            chkIsActive.UseVisualStyleBackColor = true;
            // 
            // txtPhone
            // 
            txtPhone.Font = new Font("Microsoft YaHei UI", 9F);
            txtPhone.Location = new Point(120, 170);
            txtPhone.Name = "txtPhone";
            txtPhone.Size = new Size(300, 23);
            txtPhone.TabIndex = 7;
            // 
            // lblPhone
            // 
            lblPhone.AutoSize = true;
            lblPhone.Font = new Font("Microsoft YaHei UI", 9F);
            lblPhone.Location = new Point(50, 173);
            lblPhone.Name = "lblPhone";
            lblPhone.Size = new Size(68, 17);
            lblPhone.TabIndex = 6;
            lblPhone.Text = "手机号码：";
            // 
            // txtEmail
            // 
            txtEmail.Font = new Font("Microsoft YaHei UI", 9F);
            txtEmail.Location = new Point(120, 120);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(300, 23);
            txtEmail.TabIndex = 5;
            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.Font = new Font("Microsoft YaHei UI", 9F);
            lblEmail.Location = new Point(50, 123);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(68, 17);
            lblEmail.TabIndex = 4;
            lblEmail.Text = "电子邮箱：";
            // 
            // txtRealName
            // 
            txtRealName.Font = new Font("Microsoft YaHei UI", 9F);
            txtRealName.Location = new Point(120, 70);
            txtRealName.Name = "txtRealName";
            txtRealName.Size = new Size(300, 23);
            txtRealName.TabIndex = 3;
            // 
            // lblRealName
            // 
            lblRealName.AutoSize = true;
            lblRealName.Font = new Font("Microsoft YaHei UI", 9F);
            lblRealName.Location = new Point(50, 73);
            lblRealName.Name = "lblRealName";
            lblRealName.Size = new Size(68, 17);
            lblRealName.TabIndex = 2;
            lblRealName.Text = "真实姓名：";
            // 
            // txtUsername
            // 
            txtUsername.Font = new Font("Microsoft YaHei UI", 9F);
            txtUsername.Location = new Point(120, 20);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(300, 23);
            txtUsername.TabIndex = 1;
            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.Font = new Font("Microsoft YaHei UI", 9F);
            lblUsername.Location = new Point(50, 23);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(68, 17);
            lblUsername.TabIndex = 0;
            lblUsername.Text = "用户名称：";
            // 
            // errorProvider
            // 
            errorProvider.ContainerControl = this;
            // 
            // Sys_UserEditForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(479, 379);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Sys_UserEditForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "用户信息";
            Load += Sys_UserEditForm_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtRealName;
        private System.Windows.Forms.Label lblRealName;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.Label lblPhone;
        private System.Windows.Forms.CheckBox chkIsActive;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
} 