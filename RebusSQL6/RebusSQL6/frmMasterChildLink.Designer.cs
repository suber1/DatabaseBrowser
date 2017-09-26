namespace RebusSQL6
{
    partial class frmMasterChildLink
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
            this.lblMastFlds = new System.Windows.Forms.Label();
            this.lstMastFlds = new System.Windows.Forms.ListBox();
            this.lstChildFlds = new System.Windows.Forms.ListBox();
            this.lblChildFlds = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnFK = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblMastFlds
            // 
            this.lblMastFlds.AutoSize = true;
            this.lblMastFlds.Location = new System.Drawing.Point(25, 22);
            this.lblMastFlds.Name = "lblMastFlds";
            this.lblMastFlds.Size = new System.Drawing.Size(78, 15);
            this.lblMastFlds.TabIndex = 0;
            this.lblMastFlds.Text = "Parent fields";
            // 
            // lstMastFlds
            // 
            this.lstMastFlds.FormattingEnabled = true;
            this.lstMastFlds.ItemHeight = 15;
            this.lstMastFlds.Location = new System.Drawing.Point(28, 46);
            this.lstMastFlds.Name = "lstMastFlds";
            this.lstMastFlds.Size = new System.Drawing.Size(220, 229);
            this.lstMastFlds.TabIndex = 1;
            // 
            // lstChildFlds
            // 
            this.lstChildFlds.FormattingEnabled = true;
            this.lstChildFlds.ItemHeight = 15;
            this.lstChildFlds.Location = new System.Drawing.Point(276, 46);
            this.lstChildFlds.Name = "lstChildFlds";
            this.lstChildFlds.Size = new System.Drawing.Size(220, 229);
            this.lstChildFlds.TabIndex = 3;
            // 
            // lblChildFlds
            // 
            this.lblChildFlds.AutoSize = true;
            this.lblChildFlds.Location = new System.Drawing.Point(273, 22);
            this.lblChildFlds.Name = "lblChildFlds";
            this.lblChildFlds.Size = new System.Drawing.Size(71, 15);
            this.lblChildFlds.TabIndex = 2;
            this.lblChildFlds.Text = "Child fields";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLbl});
            this.statusStrip1.Location = new System.Drawing.Point(0, 326);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(523, 22);
            this.statusStrip1.TabIndex = 4;
            // 
            // statusLbl
            // 
            this.statusLbl.Name = "statusLbl";
            this.statusLbl.Size = new System.Drawing.Size(251, 17);
            this.statusLbl.Text = "Choose a parent field and its related child field";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(421, 289);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnFK
            // 
            this.btnFK.Location = new System.Drawing.Point(276, 289);
            this.btnFK.Name = "btnFK";
            this.btnFK.Size = new System.Drawing.Size(130, 23);
            this.btnFK.TabIndex = 6;
            this.btnFK.TabStop = false;
            this.btnFK.Text = "Foreign Key Link...";
            this.btnFK.UseVisualStyleBackColor = true;
            this.btnFK.Click += new System.EventHandler(this.btnFK_Click);
            // 
            // frmMasterChildLink
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(523, 348);
            this.Controls.Add(this.btnFK);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.lstChildFlds);
            this.Controls.Add(this.lblChildFlds);
            this.Controls.Add(this.lstMastFlds);
            this.Controls.Add(this.lblMastFlds);
            this.Font = new System.Drawing.Font("Lucida Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMasterChildLink";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Parent - Child Link";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMasterChildLink_FormClosing);
            this.DoubleClick += new System.EventHandler(this.frmMasterChildLink_DoubleClick);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMastFlds;
        private System.Windows.Forms.ListBox lstMastFlds;
        private System.Windows.Forms.ListBox lstChildFlds;
        private System.Windows.Forms.Label lblChildFlds;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLbl;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnFK;
    }
}