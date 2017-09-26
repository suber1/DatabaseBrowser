namespace RebusSQL6
{
    partial class frmManageExtProvs
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
            this.lblName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtOdbcConnStr = new System.Windows.Forms.TextBox();
            this.txtOdbcConnStrTrusted = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtOleDbConnStrTrusted = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtOleDbConnStr = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSQLConnStrTrusted = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSQLConnStr = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtFileExt = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.drpProvs = new System.Windows.Forms.ComboBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnExt = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.Location = new System.Drawing.Point(28, 35);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(156, 24);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Provider Name:";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(28, 87);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 24);
            this.label1.TabIndex = 2;
            this.label1.Text = "ODBC connection string:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtOdbcConnStr
            // 
            this.txtOdbcConnStr.Location = new System.Drawing.Point(187, 89);
            this.txtOdbcConnStr.Multiline = true;
            this.txtOdbcConnStr.Name = "txtOdbcConnStr";
            this.txtOdbcConnStr.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOdbcConnStr.Size = new System.Drawing.Size(477, 56);
            this.txtOdbcConnStr.TabIndex = 3;
            this.txtOdbcConnStr.TextChanged += new System.EventHandler(this.txtOdbcConnStr_TextChanged);
            // 
            // txtOdbcConnStrTrusted
            // 
            this.txtOdbcConnStrTrusted.Location = new System.Drawing.Point(187, 151);
            this.txtOdbcConnStrTrusted.Multiline = true;
            this.txtOdbcConnStrTrusted.Name = "txtOdbcConnStrTrusted";
            this.txtOdbcConnStrTrusted.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOdbcConnStrTrusted.Size = new System.Drawing.Size(477, 56);
            this.txtOdbcConnStrTrusted.TabIndex = 5;
            this.txtOdbcConnStrTrusted.TextChanged += new System.EventHandler(this.txtOdbcConnStrTrusted_TextChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(28, 149);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(156, 46);
            this.label2.TabIndex = 4;
            this.label2.Text = "ODBC connection string (trusted):";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtOleDbConnStrTrusted
            // 
            this.txtOleDbConnStrTrusted.Location = new System.Drawing.Point(187, 302);
            this.txtOleDbConnStrTrusted.Multiline = true;
            this.txtOleDbConnStrTrusted.Name = "txtOleDbConnStrTrusted";
            this.txtOleDbConnStrTrusted.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOleDbConnStrTrusted.Size = new System.Drawing.Size(477, 56);
            this.txtOleDbConnStrTrusted.TabIndex = 9;
            this.txtOleDbConnStrTrusted.TextChanged += new System.EventHandler(this.txtOleDbConnStrTrusted_TextChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(28, 300);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(156, 46);
            this.label3.TabIndex = 8;
            this.label3.Text = "OleDb connection string (trusted):";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtOleDbConnStr
            // 
            this.txtOleDbConnStr.Location = new System.Drawing.Point(187, 240);
            this.txtOleDbConnStr.Multiline = true;
            this.txtOleDbConnStr.Name = "txtOleDbConnStr";
            this.txtOleDbConnStr.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOleDbConnStr.Size = new System.Drawing.Size(477, 56);
            this.txtOleDbConnStr.TabIndex = 7;
            this.txtOleDbConnStr.TextChanged += new System.EventHandler(this.txtOleDbConnStr_TextChanged);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(28, 238);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(156, 24);
            this.label4.TabIndex = 6;
            this.label4.Text = "OleDB connection string:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtSQLConnStrTrusted
            // 
            this.txtSQLConnStrTrusted.Location = new System.Drawing.Point(187, 451);
            this.txtSQLConnStrTrusted.Multiline = true;
            this.txtSQLConnStrTrusted.Name = "txtSQLConnStrTrusted";
            this.txtSQLConnStrTrusted.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSQLConnStrTrusted.Size = new System.Drawing.Size(477, 56);
            this.txtSQLConnStrTrusted.TabIndex = 13;
            this.txtSQLConnStrTrusted.TextChanged += new System.EventHandler(this.txtSQLConnStrTrusted_TextChanged);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(28, 449);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(156, 46);
            this.label5.TabIndex = 12;
            this.label5.Text = "SQL connection string (trusted):";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtSQLConnStr
            // 
            this.txtSQLConnStr.Location = new System.Drawing.Point(187, 389);
            this.txtSQLConnStr.Multiline = true;
            this.txtSQLConnStr.Name = "txtSQLConnStr";
            this.txtSQLConnStr.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSQLConnStr.Size = new System.Drawing.Size(477, 56);
            this.txtSQLConnStr.TabIndex = 11;
            this.txtSQLConnStr.TextChanged += new System.EventHandler(this.txtSQLConnStr_TextChanged);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(28, 387);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(156, 24);
            this.label6.TabIndex = 10;
            this.label6.Text = "SQL connection string:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFileExt
            // 
            this.txtFileExt.Location = new System.Drawing.Point(187, 555);
            this.txtFileExt.Name = "txtFileExt";
            this.txtFileExt.Size = new System.Drawing.Size(414, 22);
            this.txtFileExt.TabIndex = 15;
            this.txtFileExt.TextChanged += new System.EventHandler(this.txtFileExt_TextChanged);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(28, 538);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(156, 39);
            this.label7.TabIndex = 14;
            this.label7.Text = "File extension lookup string:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // drpProvs
            // 
            this.drpProvs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpProvs.FormattingEnabled = true;
            this.drpProvs.Location = new System.Drawing.Point(190, 37);
            this.drpProvs.Name = "drpProvs";
            this.drpProvs.Size = new System.Drawing.Size(319, 23);
            this.drpProvs.TabIndex = 1;
            this.drpProvs.SelectedIndexChanged += new System.EventHandler(this.drpProvs_SelectedIndexChanged);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(318, 602);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(103, 25);
            this.btnAdd.TabIndex = 17;
            this.btnAdd.Text = "Add...";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnExt
            // 
            this.btnExt.Location = new System.Drawing.Point(620, 552);
            this.btnExt.Name = "btnExt";
            this.btnExt.Size = new System.Drawing.Size(44, 25);
            this.btnExt.TabIndex = 16;
            this.btnExt.Text = "...";
            this.btnExt.UseVisualStyleBackColor = true;
            this.btnExt.Click += new System.EventHandler(this.btnExt_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(561, 602);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(103, 25);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(440, 602);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(103, 25);
            this.btnRemove.TabIndex = 18;
            this.btnRemove.Text = "Remove...";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // frmManageExtProvs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 639);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnExt);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.drpProvs);
            this.Controls.Add(this.txtFileExt);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtSQLConnStrTrusted);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtSQLConnStr);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtOleDbConnStrTrusted);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtOleDbConnStr);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtOdbcConnStrTrusted);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtOdbcConnStr);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblName);
            this.Font = new System.Drawing.Font("Lucida Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmManageExtProvs";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Additional Database Providers";
            this.Load += new System.EventHandler(this.frmManageExtProvs_Load);
            this.Shown += new System.EventHandler(this.frmManageExtProvs_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtOdbcConnStr;
        private System.Windows.Forms.TextBox txtOdbcConnStrTrusted;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtOleDbConnStrTrusted;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtOleDbConnStr;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSQLConnStrTrusted;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtSQLConnStr;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtFileExt;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox drpProvs;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnExt;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnRemove;
    }
}