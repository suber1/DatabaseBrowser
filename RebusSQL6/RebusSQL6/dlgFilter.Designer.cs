namespace RebusSQL6
{
    partial class dlgFilter
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
            this.lblContains = new System.Windows.Forms.Label();
            this.txt = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblContains
            // 
            this.lblContains.Location = new System.Drawing.Point(68, 26);
            this.lblContains.Name = "lblContains";
            this.lblContains.Size = new System.Drawing.Size(113, 22);
            this.lblContains.TabIndex = 0;
            this.lblContains.Text = "Contains:";
            this.lblContains.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt
            // 
            this.txt.Location = new System.Drawing.Point(185, 28);
            this.txt.Name = "txt";
            this.txt.Size = new System.Drawing.Size(111, 22);
            this.txt.TabIndex = 1;
            this.txt.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(487, 158);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(78, 29);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // dlgFilter
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(579, 198);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txt);
            this.Controls.Add(this.lblContains);
            this.Font = new System.Drawing.Font("Lucida Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "dlgFilter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Filter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.dlgFilter_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblContains;
        private System.Windows.Forms.TextBox txt;
        private System.Windows.Forms.Button btnOK;
    }
}