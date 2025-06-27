namespace CodeGenerator
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabConnection = new System.Windows.Forms.TabPage();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.lblServer = new System.Windows.Forms.Label();
            this.txtDatabase = new System.Windows.Forms.TextBox();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.cmbDatabaseType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnOutputPath = new System.Windows.Forms.Button();
            this.txtOutputPath = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tabTables = new System.Windows.Forms.TabPage();
            this.lblTableStatus = new System.Windows.Forms.Label();
            this.btnLoadTable = new System.Windows.Forms.Button();
            this.lstTables = new System.Windows.Forms.ListBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tabColumns = new System.Windows.Forms.TabPage();
            this.grpColumnProperties = new System.Windows.Forms.GroupBox();
            this.btnApplyColumnSettings = new System.Windows.Forms.Button();
            this.lblColumnProperties = new System.Windows.Forms.Label();
            this.cmbControlType = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.chkReadOnly = new System.Windows.Forms.CheckBox();
            this.chkVisible = new System.Windows.Forms.CheckBox();
            this.txtDisplayName = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtColumnName = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.lstColumns = new System.Windows.Forms.ListBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tabGenerate = new System.Windows.Forms.TabPage();
            this.btnOpenOutputFolder = new System.Windows.Forms.Button();
            this.lblGenerateStatus = new System.Windows.Forms.Label();
            this.btnGenerateCode = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtContextName = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.chkUI = new System.Windows.Forms.CheckBox();
            this.chkService = new System.Windows.Forms.CheckBox();
            this.chkBaseService = new System.Windows.Forms.CheckBox();
            this.chkRepo = new System.Windows.Forms.CheckBox();
            this.chkBaseRepo = new System.Windows.Forms.CheckBox();
            this.chkDbContext = new System.Windows.Forms.CheckBox();
            this.chkEntity = new System.Windows.Forms.CheckBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.tabControl.SuspendLayout();
            this.tabConnection.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabTables.SuspendLayout();
            this.tabColumns.SuspendLayout();
            this.grpColumnProperties.SuspendLayout();
            this.tabGenerate.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabConnection);
            this.tabControl.Controls.Add(this.tabTables);
            this.tabControl.Controls.Add(this.tabColumns);
            this.tabControl.Controls.Add(this.tabGenerate);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(884, 561);
            this.tabControl.TabIndex = 0;
            // 
            // tabConnection
            // 
            this.tabConnection.Controls.Add(this.lblStatus);
            this.tabConnection.Controls.Add(this.btnConnect);
            this.tabConnection.Controls.Add(this.groupBox1);
            this.tabConnection.Controls.Add(this.groupBox2);
            this.tabConnection.Location = new System.Drawing.Point(4, 29);
            this.tabConnection.Name = "tabConnection";
            this.tabConnection.Padding = new System.Windows.Forms.Padding(3);
            this.tabConnection.Size = new System.Drawing.Size(876, 528);
            this.tabConnection.TabIndex = 0;
            this.tabConnection.Text = "数据库连接";
            this.tabConnection.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(20, 488);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 20);
            this.lblStatus.TabIndex = 7;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(682, 479);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(150, 35);
            this.btnConnect.TabIndex = 6;
            this.btnConnect.Text = "连接数据库";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnBrowse);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.lblPassword);
            this.groupBox1.Controls.Add(this.txtUsername);
            this.groupBox1.Controls.Add(this.lblUsername);
            this.groupBox1.Controls.Add(this.txtPort);
            this.groupBox1.Controls.Add(this.lblPort);
            this.groupBox1.Controls.Add(this.txtServer);
            this.groupBox1.Controls.Add(this.lblServer);
            this.groupBox1.Controls.Add(this.txtDatabase);
            this.groupBox1.Controls.Add(this.lblDatabase);
            this.groupBox1.Controls.Add(this.cmbDatabaseType);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(20, 17);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(812, 274);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "数据库连接信息";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(662, 229);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(100, 30);
            this.btnBrowse.TabIndex = 12;
            this.btnBrowse.Text = "浏览...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(140, 190);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(622, 26);
            this.txtPassword.TabIndex = 11;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(33, 193);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(49, 20);
            this.lblPassword.TabIndex = 10;
            this.lblPassword.Text = "密码:";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(140, 150);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(622, 26);
            this.txtUsername.TabIndex = 9;
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(33, 153);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(65, 20);
            this.lblUsername.TabIndex = 8;
            this.lblUsername.Text = "用户名:";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(140, 110);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(130, 26);
            this.txtPort.TabIndex = 7;
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(33, 113);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(49, 20);
            this.lblPort.TabIndex = 6;
            this.lblPort.Text = "端口:";
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(140, 70);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(622, 26);
            this.txtServer.TabIndex = 5;
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(33, 73);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(65, 20);
            this.lblServer.TabIndex = 4;
            this.lblServer.Text = "服务器:";
            // 
            // txtDatabase
            // 
            this.txtDatabase.Location = new System.Drawing.Point(140, 230);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.Size = new System.Drawing.Size(504, 26);
            this.txtDatabase.TabIndex = 3;
            // 
            // lblDatabase
            // 
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.Location = new System.Drawing.Point(33, 233);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(97, 20);
            this.lblDatabase.TabIndex = 2;
            this.lblDatabase.Text = "数据库名称:";
            // 
            // cmbDatabaseType
            // 
            this.cmbDatabaseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDatabaseType.FormattingEnabled = true;
            this.cmbDatabaseType.Location = new System.Drawing.Point(140, 30);
            this.cmbDatabaseType.Name = "cmbDatabaseType";
            this.cmbDatabaseType.Size = new System.Drawing.Size(250, 28);
            this.cmbDatabaseType.TabIndex = 1;
            this.cmbDatabaseType.SelectedIndexChanged += new System.EventHandler(this.cmbDatabaseType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "数据库类型:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnOutputPath);
            this.groupBox2.Controls.Add(this.txtOutputPath);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Location = new System.Drawing.Point(20, 308);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(812, 100);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "代码生成选项";
            // 
            // btnOutputPath
            // 
            this.btnOutputPath.Location = new System.Drawing.Point(662, 40);
            this.btnOutputPath.Name = "btnOutputPath";
            this.btnOutputPath.Size = new System.Drawing.Size(100, 30);
            this.btnOutputPath.TabIndex = 13;
            this.btnOutputPath.Text = "浏览...";
            this.btnOutputPath.UseVisualStyleBackColor = true;
            this.btnOutputPath.Click += new System.EventHandler(this.btnOutputPath_Click);
            // 
            // txtOutputPath
            // 
            this.txtOutputPath.Location = new System.Drawing.Point(140, 42);
            this.txtOutputPath.Name = "txtOutputPath";
            this.txtOutputPath.Size = new System.Drawing.Size(504, 26);
            this.txtOutputPath.TabIndex = 4;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(33, 45);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(81, 20);
            this.label8.TabIndex = 3;
            this.label8.Text = "输出路径:";
            // 
            // tabTables
            // 
            this.tabTables.Controls.Add(this.lblTableStatus);
            this.tabTables.Controls.Add(this.btnLoadTable);
            this.tabTables.Controls.Add(this.lstTables);
            this.tabTables.Controls.Add(this.label9);
            this.tabTables.Location = new System.Drawing.Point(4, 29);
            this.tabTables.Name = "tabTables";
            this.tabTables.Padding = new System.Windows.Forms.Padding(3);
            this.tabTables.Size = new System.Drawing.Size(876, 528);
            this.tabTables.TabIndex = 1;
            this.tabTables.Text = "选择表";
            this.tabTables.UseVisualStyleBackColor = true;
            // 
            // lblTableStatus
            // 
            this.lblTableStatus.AutoSize = true;
            this.lblTableStatus.Location = new System.Drawing.Point(20, 488);
            this.lblTableStatus.Name = "lblTableStatus";
            this.lblTableStatus.Size = new System.Drawing.Size(0, 20);
            this.lblTableStatus.TabIndex = 7;
            // 
            // btnLoadTable
            // 
            this.btnLoadTable.Enabled = false;
            this.btnLoadTable.Location = new System.Drawing.Point(682, 479);
            this.btnLoadTable.Name = "btnLoadTable";
            this.btnLoadTable.Size = new System.Drawing.Size(150, 35);
            this.btnLoadTable.TabIndex = 6;
            this.btnLoadTable.Text = "加载表";
            this.btnLoadTable.UseVisualStyleBackColor = true;
            this.btnLoadTable.Click += new System.EventHandler(this.btnLoadTable_Click);
            // 
            // lstTables
            // 
            this.lstTables.FormattingEnabled = true;
            this.lstTables.ItemHeight = 20;
            this.lstTables.Location = new System.Drawing.Point(20, 50);
            this.lstTables.Name = "lstTables";
            this.lstTables.Size = new System.Drawing.Size(812, 404);
            this.lstTables.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(20, 20);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(113, 20);
            this.label9.TabIndex = 0;
            this.label9.Text = "选择数据库表:";
            // 
            // tabColumns
            // 
            this.tabColumns.Controls.Add(this.grpColumnProperties);
            this.tabColumns.Controls.Add(this.lstColumns);
            this.tabColumns.Controls.Add(this.label10);
            this.tabColumns.Location = new System.Drawing.Point(4, 29);
            this.tabColumns.Name = "tabColumns";
            this.tabColumns.Size = new System.Drawing.Size(876, 528);
            this.tabColumns.TabIndex = 2;
            this.tabColumns.Text = "字段配置";
            this.tabColumns.UseVisualStyleBackColor = true;
            // 
            // grpColumnProperties
            // 
            this.grpColumnProperties.Controls.Add(this.btnApplyColumnSettings);
            this.grpColumnProperties.Controls.Add(this.lblColumnProperties);
            this.grpColumnProperties.Controls.Add(this.cmbControlType);
            this.grpColumnProperties.Controls.Add(this.label13);
            this.grpColumnProperties.Controls.Add(this.chkReadOnly);
            this.grpColumnProperties.Controls.Add(this.chkVisible);
            this.grpColumnProperties.Controls.Add(this.txtDisplayName);
            this.grpColumnProperties.Controls.Add(this.label12);
            this.grpColumnProperties.Controls.Add(this.txtColumnName);
            this.grpColumnProperties.Controls.Add(this.label11);
            this.grpColumnProperties.Enabled = false;
            this.grpColumnProperties.Location = new System.Drawing.Point(394, 50);
            this.grpColumnProperties.Name = "grpColumnProperties";
            this.grpColumnProperties.Size = new System.Drawing.Size(438, 404);
            this.grpColumnProperties.TabIndex = 2;
            this.grpColumnProperties.TabStop = false;
            this.grpColumnProperties.Text = "字段属性";
            // 
            // btnApplyColumnSettings
            // 
            this.btnApplyColumnSettings.Location = new System.Drawing.Point(309, 354);
            this.btnApplyColumnSettings.Name = "btnApplyColumnSettings";
            this.btnApplyColumnSettings.Size = new System.Drawing.Size(112, 32);
            this.btnApplyColumnSettings.TabIndex = 9;
            this.btnApplyColumnSettings.Text = "应用设置";
            this.btnApplyColumnSettings.UseVisualStyleBackColor = true;
            this.btnApplyColumnSettings.Click += new System.EventHandler(this.btnApplyColumnSettings_Click);
            // 
            // lblColumnProperties
            // 
            this.lblColumnProperties.Location = new System.Drawing.Point(20, 220);
            this.lblColumnProperties.Name = "lblColumnProperties";
            this.lblColumnProperties.Size = new System.Drawing.Size(401, 125);
            this.lblColumnProperties.TabIndex = 8;
            this.lblColumnProperties.Text = "属性信息";
            // 
            // cmbControlType
            // 
            this.cmbControlType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbControlType.FormattingEnabled = true;
            this.cmbControlType.Location = new System.Drawing.Point(142, 130);
            this.cmbControlType.Name = "cmbControlType";
            this.cmbControlType.Size = new System.Drawing.Size(279, 28);
            this.cmbControlType.TabIndex = 7;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(20, 133);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(81, 20);
            this.label13.TabIndex = 6;
            this.label13.Text = "控件类型:";
            // 
            // chkReadOnly
            // 
            this.chkReadOnly.AutoSize = true;
            this.chkReadOnly.Location = new System.Drawing.Point(142, 180);
            this.chkReadOnly.Name = "chkReadOnly";
            this.chkReadOnly.Size = new System.Drawing.Size(91, 24);
            this.chkReadOnly.TabIndex = 5;
            this.chkReadOnly.Text = "只读";
            this.chkReadOnly.UseVisualStyleBackColor = true;
            // 
            // chkVisible
            // 
            this.chkVisible.AutoSize = true;
            this.chkVisible.Location = new System.Drawing.Point(20, 180);
            this.chkVisible.Name = "chkVisible";
            this.chkVisible.Size = new System.Drawing.Size(91, 24);
            this.chkVisible.TabIndex = 4;
            this.chkVisible.Text = "可见";
            this.chkVisible.UseVisualStyleBackColor = true;
            // 
            // txtDisplayName
            // 
            this.txtDisplayName.Location = new System.Drawing.Point(142, 85);
            this.txtDisplayName.Name = "txtDisplayName";
            this.txtDisplayName.Size = new System.Drawing.Size(279, 26);
            this.txtDisplayName.TabIndex = 3;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(20, 88);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(81, 20);
            this.label12.TabIndex = 2;
            this.label12.Text = "显示名称:";
            // 
            // txtColumnName
            // 
            this.txtColumnName.Location = new System.Drawing.Point(142, 40);
            this.txtColumnName.Name = "txtColumnName";
            this.txtColumnName.ReadOnly = true;
            this.txtColumnName.Size = new System.Drawing.Size(279, 26);
            this.txtColumnName.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(20, 43);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(81, 20);
            this.label11.TabIndex = 0;
            this.label11.Text = "字段名称:";
            // 
            // lstColumns
            // 
            this.lstColumns.FormattingEnabled = true;
            this.lstColumns.ItemHeight = 20;
            this.lstColumns.Location = new System.Drawing.Point(20, 50);
            this.lstColumns.Name = "lstColumns";
            this.lstColumns.Size = new System.Drawing.Size(350, 404);
            this.lstColumns.TabIndex = 1;
            this.lstColumns.SelectedIndexChanged += new System.EventHandler(this.lstColumns_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(20, 20);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 20);
            this.label10.TabIndex = 0;
            this.label10.Text = "字段列表:";
            // 
            // tabGenerate
            // 
            this.tabGenerate.Controls.Add(this.btnOpenOutputFolder);
            this.tabGenerate.Controls.Add(this.lblGenerateStatus);
            this.tabGenerate.Controls.Add(this.btnGenerateCode);
            this.tabGenerate.Controls.Add(this.groupBox3);
            this.tabGenerate.Location = new System.Drawing.Point(4, 29);
            this.tabGenerate.Name = "tabGenerate";
            this.tabGenerate.Size = new System.Drawing.Size(876, 528);
            this.tabGenerate.TabIndex = 3;
            this.tabGenerate.Text = "生成代码";
            this.tabGenerate.UseVisualStyleBackColor = true;
            // 
            // btnOpenOutputFolder
            // 
            this.btnOpenOutputFolder.Location = new System.Drawing.Point(506, 479);
            this.btnOpenOutputFolder.Name = "btnOpenOutputFolder";
            this.btnOpenOutputFolder.Size = new System.Drawing.Size(150, 35);
            this.btnOpenOutputFolder.TabIndex = 9;
            this.btnOpenOutputFolder.Text = "打开输出目录";
            this.btnOpenOutputFolder.UseVisualStyleBackColor = true;
            this.btnOpenOutputFolder.Click += new System.EventHandler(this.btnOpenOutputFolder_Click);
            // 
            // lblGenerateStatus
            // 
            this.lblGenerateStatus.AutoSize = true;
            this.lblGenerateStatus.Location = new System.Drawing.Point(20, 488);
            this.lblGenerateStatus.Name = "lblGenerateStatus";
            this.lblGenerateStatus.Size = new System.Drawing.Size(0, 20);
            this.lblGenerateStatus.TabIndex = 8;
            // 
            // btnGenerateCode
            // 
            this.btnGenerateCode.Enabled = false;
            this.btnGenerateCode.Location = new System.Drawing.Point(682, 479);
            this.btnGenerateCode.Name = "btnGenerateCode";
            this.btnGenerateCode.Size = new System.Drawing.Size(150, 35);
            this.btnGenerateCode.TabIndex = 7;
            this.btnGenerateCode.Text = "生成代码";
            this.btnGenerateCode.UseVisualStyleBackColor = true;
            this.btnGenerateCode.Click += new System.EventHandler(this.btnGenerateCode_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtContextName);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.chkUI);
            this.groupBox3.Controls.Add(this.chkService);
            this.groupBox3.Controls.Add(this.chkBaseService);
            this.groupBox3.Controls.Add(this.chkRepo);
            this.groupBox3.Controls.Add(this.chkBaseRepo);
            this.groupBox3.Controls.Add(this.chkDbContext);
            this.groupBox3.Controls.Add(this.chkEntity);
            this.groupBox3.Location = new System.Drawing.Point(20, 20);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(812, 300);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "生成选项";
            // 
            // txtContextName
            // 
            this.txtContextName.Location = new System.Drawing.Point(160, 230);
            this.txtContextName.Name = "txtContextName";
            this.txtContextName.Size = new System.Drawing.Size(300, 26);
            this.txtContextName.TabIndex = 8;
            this.txtContextName.Text = "ApplicationDbContext";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(20, 233);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(129, 20);
            this.label14.TabIndex = 7;
            this.label14.Text = "DbContext 名称:";
            // 
            // chkUI
            // 
            this.chkUI.AutoSize = true;
            this.chkUI.Checked = true;
            this.chkUI.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUI.Location = new System.Drawing.Point(20, 180);
            this.chkUI.Name = "chkUI";
            this.chkUI.Size = new System.Drawing.Size(124, 24);
            this.chkUI.TabIndex = 6;
            this.chkUI.Text = "生成 UI 界面";
            this.chkUI.UseVisualStyleBackColor = true;
            // 
            // chkService
            // 
            this.chkService.AutoSize = true;
            this.chkService.Checked = true;
            this.chkService.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkService.Location = new System.Drawing.Point(20, 150);
            this.chkService.Name = "chkService";
            this.chkService.Size = new System.Drawing.Size(139, 24);
            this.chkService.TabIndex = 5;
            this.chkService.Text = "生成表 Service";
            this.chkService.UseVisualStyleBackColor = true;
            // 
            // chkBaseService
            // 
            this.chkBaseService.AutoSize = true;
            this.chkBaseService.Checked = true;
            this.chkBaseService.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBaseService.Location = new System.Drawing.Point(20, 120);
            this.chkBaseService.Name = "chkBaseService";
            this.chkBaseService.Size = new System.Drawing.Size(187, 24);
            this.chkBaseService.TabIndex = 4;
            this.chkBaseService.Text = "生成基础 Service 类";
            this.chkBaseService.UseVisualStyleBackColor = true;
            // 
            // chkRepo
            // 
            this.chkRepo.AutoSize = true;
            this.chkRepo.Checked = true;
            this.chkRepo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRepo.Location = new System.Drawing.Point(20, 90);
            this.chkRepo.Name = "chkRepo";
            this.chkRepo.Size = new System.Drawing.Size(172, 24);
            this.chkRepo.TabIndex = 3;
            this.chkRepo.Text = "生成表 Repository";
            this.chkRepo.UseVisualStyleBackColor = true;
            // 
            // chkBaseRepo
            // 
            this.chkBaseRepo.AutoSize = true;
            this.chkBaseRepo.Checked = true;
            this.chkBaseRepo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBaseRepo.Location = new System.Drawing.Point(20, 60);
            this.chkBaseRepo.Name = "chkBaseRepo";
            this.chkBaseRepo.Size = new System.Drawing.Size(220, 24);
            this.chkBaseRepo.TabIndex = 2;
            this.chkBaseRepo.Text = "生成基础 Repository 类";
            this.chkBaseRepo.UseVisualStyleBackColor = true;
            // 
            // chkDbContext
            // 
            this.chkDbContext.AutoSize = true;
            this.chkDbContext.Checked = true;
            this.chkDbContext.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDbContext.Location = new System.Drawing.Point(300, 30);
            this.chkDbContext.Name = "chkDbContext";
            this.chkDbContext.Size = new System.Drawing.Size(160, 24);
            this.chkDbContext.TabIndex = 1;
            this.chkDbContext.Text = "生成 DbContext";
            this.chkDbContext.UseVisualStyleBackColor = true;
            // 
            // chkEntity
            // 
            this.chkEntity.AutoSize = true;
            this.chkEntity.Checked = true;
            this.chkEntity.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEntity.Location = new System.Drawing.Point(20, 30);
            this.chkEntity.Name = "chkEntity";
            this.chkEntity.Size = new System.Drawing.Size(139, 24);
            this.chkEntity.TabIndex = 0;
            this.chkEntity.Text = "生成实体类";
            this.chkEntity.UseVisualStyleBackColor = true;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 561);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "代码生成器";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabConnection.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabTables.ResumeLayout(false);
            this.tabTables.PerformLayout();
            this.tabColumns.ResumeLayout(false);
            this.tabColumns.PerformLayout();
            this.grpColumnProperties.ResumeLayout(false);
            this.grpColumnProperties.PerformLayout();
            this.tabGenerate.ResumeLayout(false);
            this.tabGenerate.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabConnection;
        private System.Windows.Forms.TabPage tabTables;
        private System.Windows.Forms.TabPage tabColumns;
        private System.Windows.Forms.TabPage tabGenerate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbDatabaseType;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.TextBox txtDatabase;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnOutputPath;
        private System.Windows.Forms.TextBox txtOutputPath;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ListBox lstTables;
        private System.Windows.Forms.Button btnLoadTable;
        private System.Windows.Forms.Label lblTableStatus;
        private System.Windows.Forms.ListBox lstColumns;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox grpColumnProperties;
        private System.Windows.Forms.TextBox txtColumnName;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtDisplayName;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox chkReadOnly;
        private System.Windows.Forms.CheckBox chkVisible;
        private System.Windows.Forms.ComboBox cmbControlType;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblColumnProperties;
        private System.Windows.Forms.Button btnApplyColumnSettings;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkRepo;
        private System.Windows.Forms.CheckBox chkBaseRepo;
        private System.Windows.Forms.CheckBox chkDbContext;
        private System.Windows.Forms.CheckBox chkEntity;
        private System.Windows.Forms.CheckBox chkBaseService;
        private System.Windows.Forms.CheckBox chkService;
        private System.Windows.Forms.CheckBox chkUI;
        private System.Windows.Forms.TextBox txtContextName;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button btnGenerateCode;
        private System.Windows.Forms.Label lblGenerateStatus;
        private System.Windows.Forms.Button btnOpenOutputFolder;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
} 