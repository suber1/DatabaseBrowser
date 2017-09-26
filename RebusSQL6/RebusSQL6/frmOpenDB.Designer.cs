namespace RebusSQL6
{
    partial class frmOpenDB
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOpenDB));
            this.panManual = new System.Windows.Forms.Panel();
            this.lblManual = new System.Windows.Forms.Label();
            this.txtConnStr = new System.Windows.Forms.TextBox();
            this.lblConnStr = new System.Windows.Forms.Label();
            this.chkManual = new System.Windows.Forms.CheckBox();
            this.panBasic = new System.Windows.Forms.Panel();
            this.txtDesc = new System.Windows.Forms.TextBox();
            this.lblDesc = new System.Windows.Forms.Label();
            this.drpSQLServerDB = new System.Windows.Forms.ComboBox();
            this.drpSQLServerSrc = new System.Windows.Forms.ComboBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.drpConnType = new System.Windows.Forms.ComboBox();
            this.lblConnType = new System.Windows.Forms.Label();
            this.txtPswd = new System.Windows.Forms.TextBox();
            this.lblPswd = new System.Windows.Forms.Label();
            this.txtUserID = new System.Windows.Forms.TextBox();
            this.lblUser = new System.Windows.Forms.Label();
            this.txtDB = new System.Windows.Forms.TextBox();
            this.lblDB = new System.Windows.Forms.Label();
            this.txtSrc = new System.Windows.Forms.TextBox();
            this.lblSrc = new System.Windows.Forms.Label();
            this.drpProv = new System.Windows.Forms.ComboBox();
            this.lblProv = new System.Windows.Forms.Label();
            this.panBtns = new System.Windows.Forms.Panel();
            this.btnCanc = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.dlg = new System.Windows.Forms.OpenFileDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statLbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.panManual.SuspendLayout();
            this.panBasic.SuspendLayout();
            this.panBtns.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panManual
            // 
            this.panManual.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panManual.Controls.Add(this.lblManual);
            this.panManual.Controls.Add(this.txtConnStr);
            this.panManual.Controls.Add(this.lblConnStr);
            this.panManual.Controls.Add(this.chkManual);
            this.panManual.Location = new System.Drawing.Point(12, 304);
            this.panManual.Name = "panManual";
            this.panManual.Size = new System.Drawing.Size(725, 172);
            this.panManual.TabIndex = 18;
            // 
            // lblManual
            // 
            this.lblManual.Location = new System.Drawing.Point(14, 11);
            this.lblManual.Name = "lblManual";
            this.lblManual.Size = new System.Drawing.Size(179, 21);
            this.lblManual.TabIndex = 19;
            this.lblManual.Text = "Manual connection string:";
            this.lblManual.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtConnStr
            // 
            this.txtConnStr.Location = new System.Drawing.Point(199, 41);
            this.txtConnStr.Multiline = true;
            this.txtConnStr.Name = "txtConnStr";
            this.txtConnStr.Size = new System.Drawing.Size(509, 115);
            this.txtConnStr.TabIndex = 22;
            this.txtConnStr.TextChanged += new System.EventHandler(this.txtConnStr_TextChanged);
            // 
            // lblConnStr
            // 
            this.lblConnStr.Location = new System.Drawing.Point(17, 41);
            this.lblConnStr.Name = "lblConnStr";
            this.lblConnStr.Size = new System.Drawing.Size(176, 21);
            this.lblConnStr.TabIndex = 21;
            this.lblConnStr.Text = "Connection string:";
            this.lblConnStr.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkManual
            // 
            this.chkManual.AutoSize = true;
            this.chkManual.Location = new System.Drawing.Point(199, 18);
            this.chkManual.Name = "chkManual";
            this.chkManual.Size = new System.Drawing.Size(15, 14);
            this.chkManual.TabIndex = 20;
            this.chkManual.UseVisualStyleBackColor = true;
            this.chkManual.CheckedChanged += new System.EventHandler(this.chkManual_CheckedChanged);
            this.chkManual.Click += new System.EventHandler(this.chkManual_Click);
            // 
            // panBasic
            // 
            this.panBasic.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panBasic.Controls.Add(this.txtDesc);
            this.panBasic.Controls.Add(this.lblDesc);
            this.panBasic.Controls.Add(this.drpSQLServerDB);
            this.panBasic.Controls.Add(this.drpSQLServerSrc);
            this.panBasic.Controls.Add(this.btnBrowse);
            this.panBasic.Controls.Add(this.drpConnType);
            this.panBasic.Controls.Add(this.lblConnType);
            this.panBasic.Controls.Add(this.txtPswd);
            this.panBasic.Controls.Add(this.lblPswd);
            this.panBasic.Controls.Add(this.txtUserID);
            this.panBasic.Controls.Add(this.lblUser);
            this.panBasic.Controls.Add(this.txtDB);
            this.panBasic.Controls.Add(this.lblDB);
            this.panBasic.Controls.Add(this.txtSrc);
            this.panBasic.Controls.Add(this.lblSrc);
            this.panBasic.Controls.Add(this.drpProv);
            this.panBasic.Controls.Add(this.lblProv);
            this.panBasic.Location = new System.Drawing.Point(12, 12);
            this.panBasic.Name = "panBasic";
            this.panBasic.Size = new System.Drawing.Size(725, 274);
            this.panBasic.TabIndex = 0;
            // 
            // txtDesc
            // 
            this.txtDesc.Location = new System.Drawing.Point(199, 229);
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.Size = new System.Drawing.Size(509, 22);
            this.txtDesc.TabIndex = 17;
            // 
            // lblDesc
            // 
            this.lblDesc.Location = new System.Drawing.Point(14, 229);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.Size = new System.Drawing.Size(179, 21);
            this.lblDesc.TabIndex = 16;
            this.lblDesc.Text = "Description:";
            this.lblDesc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // drpSQLServerDB
            // 
            this.drpSQLServerDB.FormattingEnabled = true;
            this.drpSQLServerDB.Location = new System.Drawing.Point(432, 187);
            this.drpSQLServerDB.Name = "drpSQLServerDB";
            this.drpSQLServerDB.Size = new System.Drawing.Size(276, 23);
            this.drpSQLServerDB.TabIndex = 15;
            this.drpSQLServerDB.Visible = false;
            this.drpSQLServerDB.SelectedIndexChanged += new System.EventHandler(this.drpSQLServerDB_SelectedIndexChanged);
            this.drpSQLServerDB.TextChanged += new System.EventHandler(this.drpSQLServerDB_TextChanged);
            this.drpSQLServerDB.KeyUp += new System.Windows.Forms.KeyEventHandler(this.drpSQLServerDB_KeyUp);
            // 
            // drpSQLServerSrc
            // 
            this.drpSQLServerSrc.FormattingEnabled = true;
            this.drpSQLServerSrc.Location = new System.Drawing.Point(432, 157);
            this.drpSQLServerSrc.Name = "drpSQLServerSrc";
            this.drpSQLServerSrc.Size = new System.Drawing.Size(276, 23);
            this.drpSQLServerSrc.TabIndex = 14;
            this.drpSQLServerSrc.Visible = false;
            this.drpSQLServerSrc.SelectedIndexChanged += new System.EventHandler(this.drpSQLServerSrc_SelectedIndexChanged);
            this.drpSQLServerSrc.TextChanged += new System.EventHandler(this.drpSQLServerSrc_TextChanged);
            this.drpSQLServerSrc.KeyUp += new System.Windows.Forms.KeyEventHandler(this.drpSQLServerSrc_KeyUp);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(514, 127);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(42, 23);
            this.btnBrowse.TabIndex = 9;
            this.btnBrowse.TabStop = false;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // drpConnType
            // 
            this.drpConnType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpConnType.FormattingEnabled = true;
            this.drpConnType.Location = new System.Drawing.Point(199, 24);
            this.drpConnType.Name = "drpConnType";
            this.drpConnType.Size = new System.Drawing.Size(129, 23);
            this.drpConnType.TabIndex = 2;
            this.drpConnType.SelectedIndexChanged += new System.EventHandler(this.drpConnType_SelectedIndexChanged);
            // 
            // lblConnType
            // 
            this.lblConnType.Location = new System.Drawing.Point(17, 24);
            this.lblConnType.Name = "lblConnType";
            this.lblConnType.Size = new System.Drawing.Size(176, 21);
            this.lblConnType.TabIndex = 1;
            this.lblConnType.Text = "Connection type:";
            this.lblConnType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPswd
            // 
            this.txtPswd.Location = new System.Drawing.Point(199, 187);
            this.txtPswd.Name = "txtPswd";
            this.txtPswd.PasswordChar = '*';
            this.txtPswd.Size = new System.Drawing.Size(203, 22);
            this.txtPswd.TabIndex = 13;
            // 
            // lblPswd
            // 
            this.lblPswd.Location = new System.Drawing.Point(14, 187);
            this.lblPswd.Name = "lblPswd";
            this.lblPswd.Size = new System.Drawing.Size(179, 21);
            this.lblPswd.TabIndex = 12;
            this.lblPswd.Text = "Password:";
            this.lblPswd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtUserID
            // 
            this.txtUserID.Location = new System.Drawing.Point(199, 157);
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.Size = new System.Drawing.Size(203, 22);
            this.txtUserID.TabIndex = 11;
            // 
            // lblUser
            // 
            this.lblUser.Location = new System.Drawing.Point(14, 157);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(179, 21);
            this.lblUser.TabIndex = 10;
            this.lblUser.Text = "User ID:";
            this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDB
            // 
            this.txtDB.Location = new System.Drawing.Point(199, 126);
            this.txtDB.Name = "txtDB";
            this.txtDB.Size = new System.Drawing.Size(309, 22);
            this.txtDB.TabIndex = 8;
            this.txtDB.TextChanged += new System.EventHandler(this.txtDB_TextChanged);
            // 
            // lblDB
            // 
            this.lblDB.Location = new System.Drawing.Point(14, 127);
            this.lblDB.Name = "lblDB";
            this.lblDB.Size = new System.Drawing.Size(179, 21);
            this.lblDB.TabIndex = 7;
            this.lblDB.Text = "Database:";
            this.lblDB.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtSrc
            // 
            this.txtSrc.Location = new System.Drawing.Point(199, 97);
            this.txtSrc.Name = "txtSrc";
            this.txtSrc.Size = new System.Drawing.Size(411, 22);
            this.txtSrc.TabIndex = 6;
            this.txtSrc.TextChanged += new System.EventHandler(this.txtSrc_TextChanged);
            // 
            // lblSrc
            // 
            this.lblSrc.Location = new System.Drawing.Point(14, 97);
            this.lblSrc.Name = "lblSrc";
            this.lblSrc.Size = new System.Drawing.Size(179, 21);
            this.lblSrc.TabIndex = 5;
            this.lblSrc.Text = "Source:";
            this.lblSrc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblSrc.Click += new System.EventHandler(this.lblSrc_Click);
            // 
            // drpProv
            // 
            this.drpProv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpProv.FormattingEnabled = true;
            this.drpProv.Location = new System.Drawing.Point(199, 61);
            this.drpProv.Name = "drpProv";
            this.drpProv.Size = new System.Drawing.Size(309, 23);
            this.drpProv.TabIndex = 4;
            this.drpProv.SelectedIndexChanged += new System.EventHandler(this.drpProv_SelectedIndexChanged);
            // 
            // lblProv
            // 
            this.lblProv.Location = new System.Drawing.Point(14, 61);
            this.lblProv.Name = "lblProv";
            this.lblProv.Size = new System.Drawing.Size(179, 21);
            this.lblProv.TabIndex = 3;
            this.lblProv.Text = "Provider:";
            this.lblProv.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panBtns
            // 
            this.panBtns.Controls.Add(this.btnCanc);
            this.panBtns.Controls.Add(this.btnOpen);
            this.panBtns.Controls.Add(this.btnTest);
            this.panBtns.Location = new System.Drawing.Point(12, 493);
            this.panBtns.Name = "panBtns";
            this.panBtns.Size = new System.Drawing.Size(727, 50);
            this.panBtns.TabIndex = 23;
            this.panBtns.DoubleClick += new System.EventHandler(this.panBtns_DoubleClick);
            // 
            // btnCanc
            // 
            this.btnCanc.Location = new System.Drawing.Point(607, 13);
            this.btnCanc.Name = "btnCanc";
            this.btnCanc.Size = new System.Drawing.Size(103, 24);
            this.btnCanc.TabIndex = 26;
            this.btnCanc.Text = "Cancel";
            this.btnCanc.UseVisualStyleBackColor = true;
            this.btnCanc.Click += new System.EventHandler(this.btnCanc_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(459, 13);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(128, 24);
            this.btnOpen.TabIndex = 25;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(275, 13);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(165, 24);
            this.btnTest.TabIndex = 24;
            this.btnTest.TabStop = false;
            this.btnTest.Text = "Test Connection";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // dlg
            // 
            this.dlg.FileName = "openFileDialog1";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statLbl});
            this.statusStrip1.Location = new System.Drawing.Point(0, 549);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(751, 22);
            this.statusStrip1.TabIndex = 24;
            this.statusStrip1.Text = "statusStrip";
            // 
            // statLbl
            // 
            this.statLbl.Name = "statLbl";
            this.statLbl.Size = new System.Drawing.Size(0, 17);
            // 
            // frmOpenDB
            // 
            this.AcceptButton = this.btnOpen;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(751, 571);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.panBtns);
            this.Controls.Add(this.panBasic);
            this.Controls.Add(this.panManual);
            this.Font = new System.Drawing.Font("Lucida Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmOpenDB";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Open Database";
            this.Activated += new System.EventHandler(this.frmOpenDB_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmOpenDB_FormClosing);
            this.Load += new System.EventHandler(this.frmOpenDB_Load);
            this.Shown += new System.EventHandler(this.frmOpenDB_Shown);
            this.DoubleClick += new System.EventHandler(this.frmOpenDB_DoubleClick);
            this.panManual.ResumeLayout(false);
            this.panManual.PerformLayout();
            this.panBasic.ResumeLayout(false);
            this.panBasic.PerformLayout();
            this.panBtns.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panManual;
        private System.Windows.Forms.TextBox txtConnStr;
        private System.Windows.Forms.Label lblConnStr;
        private System.Windows.Forms.CheckBox chkManual;
        private System.Windows.Forms.Panel panBasic;
        private System.Windows.Forms.TextBox txtPswd;
        private System.Windows.Forms.Label lblPswd;
        private System.Windows.Forms.TextBox txtUserID;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.TextBox txtDB;
        private System.Windows.Forms.Label lblDB;
        private System.Windows.Forms.TextBox txtSrc;
        private System.Windows.Forms.Label lblSrc;
        private System.Windows.Forms.ComboBox drpProv;
        private System.Windows.Forms.Label lblProv;
        private System.Windows.Forms.Panel panBtns;
        private System.Windows.Forms.Label lblManual;
        private System.Windows.Forms.ComboBox drpConnType;
        private System.Windows.Forms.Label lblConnType;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnCanc;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.OpenFileDialog dlg;
        private System.Windows.Forms.ComboBox drpSQLServerDB;
        private System.Windows.Forms.ComboBox drpSQLServerSrc;
        private System.Windows.Forms.TextBox txtDesc;
        private System.Windows.Forms.Label lblDesc;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statLbl;

    }
}