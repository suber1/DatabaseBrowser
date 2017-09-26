namespace RebusSQL6
{
    partial class frmChildDlg
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
            this.lblTable = new System.Windows.Forms.Label();
            this.drpTable = new System.Windows.Forms.ComboBox();
            this.lstFlds = new System.Windows.Forms.ListBox();
            this.lstShowFlds = new System.Windows.Forms.ListBox();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.txtManualTable = new System.Windows.Forms.TextBox();
            this.btnManualTable = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblMasterFld = new System.Windows.Forms.Label();
            this.lblChildFld = new System.Windows.Forms.Label();
            this.txtMasterFlds = new System.Windows.Forms.TextBox();
            this.txtChildFlds = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnLink = new System.Windows.Forms.Button();
            this.btnClearLinks = new System.Windows.Forms.Button();
            this.lblOrderByFlds = new System.Windows.Forms.Label();
            this.txtOrderByFlds = new System.Windows.Forms.TextBox();
            this.btnClrOrderBy = new System.Windows.Forms.Button();
            this.chkDescending = new System.Windows.Forms.CheckBox();
            this.btnHelp = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTable
            // 
            this.lblTable.Location = new System.Drawing.Point(64, 35);
            this.lblTable.Name = "lblTable";
            this.lblTable.Size = new System.Drawing.Size(84, 25);
            this.lblTable.TabIndex = 0;
            this.lblTable.Text = "Child table:";
            this.lblTable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // drpTable
            // 
            this.drpTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpTable.FormattingEnabled = true;
            this.drpTable.Location = new System.Drawing.Point(154, 37);
            this.drpTable.Name = "drpTable";
            this.drpTable.Size = new System.Drawing.Size(508, 23);
            this.drpTable.TabIndex = 1;
            this.drpTable.SelectedIndexChanged += new System.EventHandler(this.drpTable_SelectedIndexChanged);
            // 
            // lstFlds
            // 
            this.lstFlds.FormattingEnabled = true;
            this.lstFlds.ItemHeight = 15;
            this.lstFlds.Location = new System.Drawing.Point(154, 150);
            this.lstFlds.Name = "lstFlds";
            this.lstFlds.Size = new System.Drawing.Size(219, 214);
            this.lstFlds.TabIndex = 2;
            this.lstFlds.DoubleClick += new System.EventHandler(this.lstFlds_DoubleClick);
            // 
            // lstShowFlds
            // 
            this.lstShowFlds.FormattingEnabled = true;
            this.lstShowFlds.ItemHeight = 15;
            this.lstShowFlds.Location = new System.Drawing.Point(443, 150);
            this.lstShowFlds.Name = "lstShowFlds";
            this.lstShowFlds.Size = new System.Drawing.Size(219, 214);
            this.lstShowFlds.TabIndex = 3;
            this.lstShowFlds.DoubleClick += new System.EventHandler(this.lstShowFlds_DoubleClick);
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(390, 160);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(30, 23);
            this.btnRight.TabIndex = 4;
            this.btnRight.Text = ">";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(390, 200);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(30, 23);
            this.btnLeft.TabIndex = 5;
            this.btnLeft.Text = "<";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // txtManualTable
            // 
            this.txtManualTable.Location = new System.Drawing.Point(155, 77);
            this.txtManualTable.Name = "txtManualTable";
            this.txtManualTable.Size = new System.Drawing.Size(421, 22);
            this.txtManualTable.TabIndex = 8;
            // 
            // btnManualTable
            // 
            this.btnManualTable.Location = new System.Drawing.Point(582, 77);
            this.btnManualTable.Name = "btnManualTable";
            this.btnManualTable.Size = new System.Drawing.Size(80, 24);
            this.btnManualTable.TabIndex = 9;
            this.btnManualTable.Text = "Load fields";
            this.btnManualTable.UseVisualStyleBackColor = true;
            this.btnManualTable.Click += new System.EventHandler(this.btnManualTable_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(677, 200);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(58, 23);
            this.btnClear.TabIndex = 10;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(640, 480);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(102, 23);
            this.btnOK.TabIndex = 11;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblMasterFld
            // 
            this.lblMasterFld.Location = new System.Drawing.Point(12, 393);
            this.lblMasterFld.Name = "lblMasterFld";
            this.lblMasterFld.Size = new System.Drawing.Size(136, 25);
            this.lblMasterFld.TabIndex = 12;
            this.lblMasterFld.Text = "Parent field link(s):";
            this.lblMasterFld.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblChildFld
            // 
            this.lblChildFld.Location = new System.Drawing.Point(15, 430);
            this.lblChildFld.Name = "lblChildFld";
            this.lblChildFld.Size = new System.Drawing.Size(133, 25);
            this.lblChildFld.TabIndex = 13;
            this.lblChildFld.Text = "Child field link(s):";
            this.lblChildFld.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtMasterFlds
            // 
            this.txtMasterFlds.Location = new System.Drawing.Point(155, 396);
            this.txtMasterFlds.Name = "txtMasterFlds";
            this.txtMasterFlds.Size = new System.Drawing.Size(407, 22);
            this.txtMasterFlds.TabIndex = 14;
            // 
            // txtChildFlds
            // 
            this.txtChildFlds.Location = new System.Drawing.Point(155, 433);
            this.txtChildFlds.Name = "txtChildFlds";
            this.txtChildFlds.Size = new System.Drawing.Size(407, 22);
            this.txtChildFlds.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 25);
            this.label1.TabIndex = 16;
            this.label1.Text = "Manual child table:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(152, 122);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(136, 25);
            this.label2.TabIndex = 17;
            this.label2.Text = "Available child fields";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(440, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 25);
            this.label3.TabIndex = 18;
            this.label3.Text = "Fields to show";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLbl});
            this.statusStrip1.Location = new System.Drawing.Point(0, 526);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(754, 22);
            this.statusStrip1.TabIndex = 19;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLbl
            // 
            this.statusLbl.Name = "statusLbl";
            this.statusLbl.Size = new System.Drawing.Size(0, 17);
            // 
            // btnLink
            // 
            this.btnLink.Location = new System.Drawing.Point(568, 396);
            this.btnLink.Name = "btnLink";
            this.btnLink.Size = new System.Drawing.Size(94, 59);
            this.btnLink.TabIndex = 20;
            this.btnLink.Text = "Add link...";
            this.btnLink.UseVisualStyleBackColor = true;
            this.btnLink.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // btnClearLinks
            // 
            this.btnClearLinks.Location = new System.Drawing.Point(668, 396);
            this.btnClearLinks.Name = "btnClearLinks";
            this.btnClearLinks.Size = new System.Drawing.Size(74, 59);
            this.btnClearLinks.TabIndex = 21;
            this.btnClearLinks.Text = "Clear links";
            this.btnClearLinks.UseVisualStyleBackColor = true;
            // 
            // lblOrderByFlds
            // 
            this.lblOrderByFlds.Location = new System.Drawing.Point(12, 467);
            this.lblOrderByFlds.Name = "lblOrderByFlds";
            this.lblOrderByFlds.Size = new System.Drawing.Size(133, 25);
            this.lblOrderByFlds.TabIndex = 22;
            this.lblOrderByFlds.Text = "ORDER BY fields:";
            this.lblOrderByFlds.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtOrderByFlds
            // 
            this.txtOrderByFlds.Location = new System.Drawing.Point(155, 470);
            this.txtOrderByFlds.Name = "txtOrderByFlds";
            this.txtOrderByFlds.Size = new System.Drawing.Size(331, 22);
            this.txtOrderByFlds.TabIndex = 23;
            // 
            // btnClrOrderBy
            // 
            this.btnClrOrderBy.Location = new System.Drawing.Point(492, 469);
            this.btnClrOrderBy.Name = "btnClrOrderBy";
            this.btnClrOrderBy.Size = new System.Drawing.Size(70, 23);
            this.btnClrOrderBy.TabIndex = 24;
            this.btnClrOrderBy.Text = "Clear";
            this.btnClrOrderBy.UseVisualStyleBackColor = true;
            this.btnClrOrderBy.Click += new System.EventHandler(this.btnClrOrderBy_Click);
            // 
            // chkDescending
            // 
            this.chkDescending.AutoSize = true;
            this.chkDescending.Location = new System.Drawing.Point(155, 499);
            this.chkDescending.Name = "chkDescending";
            this.chkDescending.Size = new System.Drawing.Size(95, 19);
            this.chkDescending.TabIndex = 25;
            this.chkDescending.Text = "Descending";
            this.chkDescending.UseVisualStyleBackColor = true;
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(677, 265);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(58, 23);
            this.btnHelp.TabIndex = 26;
            this.btnHelp.Text = "Help...";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // frmChildDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(754, 548);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.chkDescending);
            this.Controls.Add(this.btnClrOrderBy);
            this.Controls.Add(this.txtOrderByFlds);
            this.Controls.Add(this.lblOrderByFlds);
            this.Controls.Add(this.btnClearLinks);
            this.Controls.Add(this.btnLink);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtChildFlds);
            this.Controls.Add(this.txtMasterFlds);
            this.Controls.Add(this.lblChildFld);
            this.Controls.Add(this.lblMasterFld);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnManualTable);
            this.Controls.Add(this.txtManualTable);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.lstShowFlds);
            this.Controls.Add(this.lstFlds);
            this.Controls.Add(this.drpTable);
            this.Controls.Add(this.lblTable);
            this.Font = new System.Drawing.Font("Lucida Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmChildDlg";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create Child SQL";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmChildDlg_FormClosing);
            this.Shown += new System.EventHandler(this.frmChildDlg_Shown);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTable;
        private System.Windows.Forms.ComboBox drpTable;
        private System.Windows.Forms.ListBox lstFlds;
        private System.Windows.Forms.ListBox lstShowFlds;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.TextBox txtManualTable;
        private System.Windows.Forms.Button btnManualTable;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblMasterFld;
        private System.Windows.Forms.Label lblChildFld;
        private System.Windows.Forms.TextBox txtMasterFlds;
        private System.Windows.Forms.TextBox txtChildFlds;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLbl;
        private System.Windows.Forms.Button btnLink;
        private System.Windows.Forms.Button btnClearLinks;
        private System.Windows.Forms.Label lblOrderByFlds;
        private System.Windows.Forms.TextBox txtOrderByFlds;
        private System.Windows.Forms.Button btnClrOrderBy;
        private System.Windows.Forms.CheckBox chkDescending;
        private System.Windows.Forms.Button btnHelp;
    }
}