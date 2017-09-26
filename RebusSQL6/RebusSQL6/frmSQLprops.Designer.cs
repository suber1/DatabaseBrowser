namespace RebusSQL6
{
    partial class frmSQLprops
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSQLprops));
            this.lblCaption = new System.Windows.Forms.Label();
            this.txtCaption = new System.Windows.Forms.TextBox();
            this.lblDesc = new System.Windows.Forms.Label();
            this.txtDesc = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCanc = new System.Windows.Forms.Button();
            this.lblGrpID = new System.Windows.Forms.Label();
            this.txtGrpID = new System.Windows.Forms.TextBox();
            this.lblGrpInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblCaption
            // 
            this.lblCaption.Location = new System.Drawing.Point(12, 9);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(110, 21);
            this.lblCaption.TabIndex = 0;
            this.lblCaption.Text = "Caption:";
            this.lblCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCaption
            // 
            this.txtCaption.Location = new System.Drawing.Point(128, 9);
            this.txtCaption.MaxLength = 250;
            this.txtCaption.Name = "txtCaption";
            this.txtCaption.Size = new System.Drawing.Size(374, 25);
            this.txtCaption.TabIndex = 1;
            // 
            // lblDesc
            // 
            this.lblDesc.Location = new System.Drawing.Point(12, 57);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.Size = new System.Drawing.Size(110, 21);
            this.lblDesc.TabIndex = 2;
            this.lblDesc.Text = "Description:";
            this.lblDesc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDesc
            // 
            this.txtDesc.Location = new System.Drawing.Point(128, 56);
            this.txtDesc.MaxLength = 50;
            this.txtDesc.Multiline = true;
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.Size = new System.Drawing.Size(374, 88);
            this.txtDesc.TabIndex = 3;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(263, 273);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(98, 29);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCanc
            // 
            this.btnCanc.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCanc.Location = new System.Drawing.Point(404, 273);
            this.btnCanc.Name = "btnCanc";
            this.btnCanc.Size = new System.Drawing.Size(98, 29);
            this.btnCanc.TabIndex = 7;
            this.btnCanc.Text = "Cancel";
            this.btnCanc.UseVisualStyleBackColor = true;
            this.btnCanc.Click += new System.EventHandler(this.btnCanc_Click);
            // 
            // lblGrpID
            // 
            this.lblGrpID.Location = new System.Drawing.Point(12, 165);
            this.lblGrpID.Name = "lblGrpID";
            this.lblGrpID.Size = new System.Drawing.Size(110, 21);
            this.lblGrpID.TabIndex = 4;
            this.lblGrpID.Text = "Group ID:";
            this.lblGrpID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtGrpID
            // 
            this.txtGrpID.Location = new System.Drawing.Point(128, 164);
            this.txtGrpID.MaxLength = 50;
            this.txtGrpID.Name = "txtGrpID";
            this.txtGrpID.Size = new System.Drawing.Size(142, 25);
            this.txtGrpID.TabIndex = 5;
            // 
            // lblGrpInfo
            // 
            this.lblGrpInfo.Font = new System.Drawing.Font("Lucida Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGrpInfo.Location = new System.Drawing.Point(288, 164);
            this.lblGrpInfo.Name = "lblGrpInfo";
            this.lblGrpInfo.Size = new System.Drawing.Size(226, 92);
            this.lblGrpInfo.TabIndex = 8;
            this.lblGrpInfo.Text = resources.GetString("lblGrpInfo.Text");
            // 
            // frmSQLprops
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCanc;
            this.ClientSize = new System.Drawing.Size(517, 314);
            this.Controls.Add(this.lblGrpInfo);
            this.Controls.Add(this.txtGrpID);
            this.Controls.Add(this.lblGrpID);
            this.Controls.Add(this.btnCanc);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtDesc);
            this.Controls.Add(this.lblDesc);
            this.Controls.Add(this.txtCaption);
            this.Controls.Add(this.lblCaption);
            this.Font = new System.Drawing.Font("Lucida Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSQLprops";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Properties";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.Label lblDesc;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCanc;
        public System.Windows.Forms.TextBox txtCaption;
        public System.Windows.Forms.TextBox txtDesc;
        private System.Windows.Forms.Label lblGrpID;
        public System.Windows.Forms.TextBox txtGrpID;
        private System.Windows.Forms.Label lblGrpInfo;
    }
}