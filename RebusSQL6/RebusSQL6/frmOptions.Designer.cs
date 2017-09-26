namespace RebusSQL6
{
    partial class frmOptions
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
            this.lblTop = new System.Windows.Forms.Label();
            this.txtTop = new System.Windows.Forms.TextBox();
            this.lblTopDesc = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTop
            // 
            this.lblTop.Location = new System.Drawing.Point(18, 62);
            this.lblTop.Name = "lblTop";
            this.lblTop.Size = new System.Drawing.Size(115, 27);
            this.lblTop.TabIndex = 0;
            this.lblTop.Text = "SELECT TOP:";
            this.lblTop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtTop
            // 
            this.txtTop.Location = new System.Drawing.Point(138, 63);
            this.txtTop.MaxLength = 6;
            this.txtTop.Name = "txtTop";
            this.txtTop.Size = new System.Drawing.Size(89, 25);
            this.txtTop.TabIndex = 1;
            // 
            // lblTopDesc
            // 
            this.lblTopDesc.Location = new System.Drawing.Point(243, 64);
            this.lblTopDesc.Name = "lblTopDesc";
            this.lblTopDesc.Size = new System.Drawing.Size(580, 24);
            this.lblTopDesc.TabIndex = 2;
            this.lblTopDesc.Text = "The default TOP records to select when opening a new SQL window, 0=no TOP";
            this.lblTopDesc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(760, 164);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(124, 34);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // frmOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(898, 211);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblTopDesc);
            this.Controls.Add(this.txtTop);
            this.Controls.Add(this.lblTop);
            this.Font = new System.Drawing.Font("Lucida Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Shown += new System.EventHandler(this.frmOptions_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTop;
        private System.Windows.Forms.TextBox txtTop;
        private System.Windows.Forms.Label lblTopDesc;
        private System.Windows.Forms.Button btnOK;
    }
}