namespace RebusSQL6
{
    partial class frmPickOne
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPickOne));
            this.lblPrompt = new System.Windows.Forms.Label();
            this.drp = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCanc = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblPrompt
            // 
            this.lblPrompt.Location = new System.Drawing.Point(12, 106);
            this.lblPrompt.Name = "lblPrompt";
            this.lblPrompt.Size = new System.Drawing.Size(129, 26);
            this.lblPrompt.TabIndex = 0;
            this.lblPrompt.Text = "- set by caller -";
            this.lblPrompt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // drp
            // 
            this.drp.FormattingEnabled = true;
            this.drp.Location = new System.Drawing.Point(147, 106);
            this.drp.Name = "drp";
            this.drp.Size = new System.Drawing.Size(330, 23);
            this.drp.TabIndex = 1;
            this.drp.SelectedIndexChanged += new System.EventHandler(this.drp_SelectedIndexChanged);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(261, 279);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(95, 30);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCanc
            // 
            this.btnCanc.Location = new System.Drawing.Point(382, 279);
            this.btnCanc.Name = "btnCanc";
            this.btnCanc.Size = new System.Drawing.Size(95, 30);
            this.btnCanc.TabIndex = 3;
            this.btnCanc.Text = "Cancel";
            this.btnCanc.UseVisualStyleBackColor = true;
            this.btnCanc.Click += new System.EventHandler(this.btnCanc_Click);
            // 
            // frmPickOne
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(499, 317);
            this.Controls.Add(this.btnCanc);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.drp);
            this.Controls.Add(this.lblPrompt);
            this.Font = new System.Drawing.Font("Lucida Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPickOne";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "- set by caller -";
            this.Load += new System.EventHandler(this.frmPickOne_Load);
            this.Shown += new System.EventHandler(this.frmPickOne_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblPrompt;
        private System.Windows.Forms.ComboBox drp;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCanc;
    }
}