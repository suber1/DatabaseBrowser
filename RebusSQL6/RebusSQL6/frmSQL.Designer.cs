namespace RebusSQL4
{
    partial class frmSQL
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabSQL = new System.Windows.Forms.TabControl();
            this.pgSQL = new System.Windows.Forms.TabPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.txtSQL = new System.Windows.Forms.TextBox();
            this.pgData = new System.Windows.Forms.TabPage();
            this.pgStruc = new System.Windows.Forms.TabPage();
            this.pgIdxs = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnExec = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.tabSQL.SuspendLayout();
            this.pgSQL.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 377);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(735, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            this.statusStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.statusStrip1_ItemClicked);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tabSQL);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(735, 377);
            this.panel1.TabIndex = 2;
            // 
            // tabSQL
            // 
            this.tabSQL.Controls.Add(this.pgSQL);
            this.tabSQL.Controls.Add(this.pgData);
            this.tabSQL.Controls.Add(this.pgStruc);
            this.tabSQL.Controls.Add(this.pgIdxs);
            this.tabSQL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabSQL.Font = new System.Drawing.Font("Lucida Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabSQL.Location = new System.Drawing.Point(0, 0);
            this.tabSQL.Name = "tabSQL";
            this.tabSQL.SelectedIndex = 0;
            this.tabSQL.Size = new System.Drawing.Size(735, 330);
            this.tabSQL.TabIndex = 2;
            // 
            // pgSQL
            // 
            this.pgSQL.Controls.Add(this.panel3);
            this.pgSQL.Location = new System.Drawing.Point(4, 24);
            this.pgSQL.Name = "pgSQL";
            this.pgSQL.Padding = new System.Windows.Forms.Padding(3);
            this.pgSQL.Size = new System.Drawing.Size(727, 302);
            this.pgSQL.TabIndex = 3;
            this.pgSQL.Text = "SQL";
            this.pgSQL.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.txtSQL);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(721, 296);
            this.panel3.TabIndex = 0;
            // 
            // txtSQL
            // 
            this.txtSQL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSQL.Location = new System.Drawing.Point(0, 0);
            this.txtSQL.Multiline = true;
            this.txtSQL.Name = "txtSQL";
            this.txtSQL.Size = new System.Drawing.Size(721, 296);
            this.txtSQL.TabIndex = 0;
            // 
            // pgData
            // 
            this.pgData.Location = new System.Drawing.Point(4, 24);
            this.pgData.Name = "pgData";
            this.pgData.Padding = new System.Windows.Forms.Padding(3);
            this.pgData.Size = new System.Drawing.Size(512, 206);
            this.pgData.TabIndex = 0;
            this.pgData.Text = "Data";
            this.pgData.UseVisualStyleBackColor = true;
            // 
            // pgStruc
            // 
            this.pgStruc.Location = new System.Drawing.Point(4, 24);
            this.pgStruc.Name = "pgStruc";
            this.pgStruc.Padding = new System.Windows.Forms.Padding(3);
            this.pgStruc.Size = new System.Drawing.Size(512, 206);
            this.pgStruc.TabIndex = 1;
            this.pgStruc.Text = "Structure";
            this.pgStruc.UseVisualStyleBackColor = true;
            // 
            // pgIdxs
            // 
            this.pgIdxs.Location = new System.Drawing.Point(4, 24);
            this.pgIdxs.Name = "pgIdxs";
            this.pgIdxs.Size = new System.Drawing.Size(512, 206);
            this.pgIdxs.TabIndex = 2;
            this.pgIdxs.Text = "Indices";
            this.pgIdxs.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 330);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(735, 47);
            this.panel2.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.btnExec);
            this.panel4.Controls.Add(this.btnSave);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(137, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(598, 47);
            this.panel4.TabIndex = 0;
            // 
            // btnExec
            // 
            this.btnExec.Font = new System.Drawing.Font("Lucida Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExec.Location = new System.Drawing.Point(469, 6);
            this.btnExec.Name = "btnExec";
            this.btnExec.Size = new System.Drawing.Size(123, 32);
            this.btnExec.TabIndex = 1;
            this.btnExec.Text = "Execute <F5>";
            this.btnExec.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Lucida Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(312, 6);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(123, 32);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save Changes";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // frmSQL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(735, 399);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "frmSQL";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SQL";
            this.panel1.ResumeLayout(false);
            this.tabSQL.ResumeLayout(false);
            this.pgSQL.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tabSQL;
        private System.Windows.Forms.TabPage pgSQL;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox txtSQL;
        private System.Windows.Forms.TabPage pgData;
        private System.Windows.Forms.TabPage pgStruc;
        private System.Windows.Forms.TabPage pgIdxs;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnExec;
    }
}