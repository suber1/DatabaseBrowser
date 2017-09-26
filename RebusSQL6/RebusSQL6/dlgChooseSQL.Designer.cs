namespace RebusSQL6
{
    partial class dlgChooseSQL
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
            this.lstSQLinfo = new System.Windows.Forms.ListBox();
            this.btnDel = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lstSQL = new System.Windows.Forms.ListBox();
            this.lstInfo = new System.Windows.Forms.ListBox();
            this.lblSQLs = new System.Windows.Forms.Label();
            this.txtSQL = new System.Windows.Forms.TextBox();
            this.lblDesc = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lstSQLinfo
            // 
            this.lstSQLinfo.Font = new System.Drawing.Font("Lucida Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstSQLinfo.FormattingEnabled = true;
            this.lstSQLinfo.ItemHeight = 15;
            this.lstSQLinfo.Location = new System.Drawing.Point(14, 14);
            this.lstSQLinfo.Name = "lstSQLinfo";
            this.lstSQLinfo.Size = new System.Drawing.Size(30, 49);
            this.lstSQLinfo.TabIndex = 0;
            this.lstSQLinfo.TabStop = false;
            this.lstSQLinfo.Visible = false;
            // 
            // btnDel
            // 
            this.btnDel.Font = new System.Drawing.Font("Lucida Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDel.Location = new System.Drawing.Point(51, 308);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(87, 27);
            this.btnDel.TabIndex = 2;
            this.btnDel.TabStop = false;
            this.btnDel.Text = "Delete...";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("Lucida Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(308, 308);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 27);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("Lucida Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(202, 308);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(87, 27);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lstSQL
            // 
            this.lstSQL.Font = new System.Drawing.Font("Lucida Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstSQL.FormattingEnabled = true;
            this.lstSQL.ItemHeight = 15;
            this.lstSQL.Location = new System.Drawing.Point(51, 14);
            this.lstSQL.Name = "lstSQL";
            this.lstSQL.Size = new System.Drawing.Size(344, 274);
            this.lstSQL.TabIndex = 1;
            this.lstSQL.Click += new System.EventHandler(this.lstSQL_Click);
            this.lstSQL.SelectedIndexChanged += new System.EventHandler(this.lstSQL_SelectedIndexChanged);
            this.lstSQL.DoubleClick += new System.EventHandler(this.lstSQL_DoubleClick);
            // 
            // lstInfo
            // 
            this.lstInfo.Font = new System.Drawing.Font("Lucida Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstInfo.FormattingEnabled = true;
            this.lstInfo.ItemHeight = 15;
            this.lstInfo.Location = new System.Drawing.Point(412, 42);
            this.lstInfo.Name = "lstInfo";
            this.lstInfo.Size = new System.Drawing.Size(377, 184);
            this.lstInfo.TabIndex = 6;
            this.lstInfo.SelectedIndexChanged += new System.EventHandler(this.lstInfo_SelectedIndexChanged);
            // 
            // lblSQLs
            // 
            this.lblSQLs.AutoSize = true;
            this.lblSQLs.Location = new System.Drawing.Point(409, 14);
            this.lblSQLs.Name = "lblSQLs";
            this.lblSQLs.Size = new System.Drawing.Size(45, 15);
            this.lblSQLs.TabIndex = 5;
            this.lblSQLs.Text = "SQL(s):";
            // 
            // txtSQL
            // 
            this.txtSQL.Location = new System.Drawing.Point(416, 239);
            this.txtSQL.Multiline = true;
            this.txtSQL.Name = "txtSQL";
            this.txtSQL.ReadOnly = true;
            this.txtSQL.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSQL.Size = new System.Drawing.Size(372, 48);
            this.txtSQL.TabIndex = 7;
            this.txtSQL.TabStop = false;
            // 
            // lblDesc
            // 
            this.lblDesc.Font = new System.Drawing.Font("Lucida Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDesc.Location = new System.Drawing.Point(418, 298);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.Size = new System.Drawing.Size(370, 40);
            this.lblDesc.TabIndex = 8;
            this.lblDesc.Text = "lblDesc";
            // 
            // dlgChooseSQL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(811, 345);
            this.Controls.Add(this.lblDesc);
            this.Controls.Add(this.txtSQL);
            this.Controls.Add(this.lblSQLs);
            this.Controls.Add(this.lstInfo);
            this.Controls.Add(this.lstSQLinfo);
            this.Controls.Add(this.btnDel);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lstSQL);
            this.Font = new System.Drawing.Font("Lucida Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "dlgChooseSQL";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Choose Previously Saved SQL Set to Restore";
            this.Shown += new System.EventHandler(this.dlgChooseSQL_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ListBox lstSQLinfo;
        private System.Windows.Forms.Button btnDel;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        public System.Windows.Forms.ListBox lstSQL;
        public System.Windows.Forms.ListBox lstInfo;
        private System.Windows.Forms.Label lblSQLs;
        private System.Windows.Forms.TextBox txtSQL;
        private System.Windows.Forms.Label lblDesc;
    }
}