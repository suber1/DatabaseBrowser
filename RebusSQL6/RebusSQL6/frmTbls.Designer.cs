namespace RebusSQL6
{
    partial class frmTbls
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTbls));
            this.pan = new System.Windows.Forms.Panel();
            this.grd = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.popupMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemShowViews = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemShowProcs = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemProcParams = new System.Windows.Forms.ToolStripMenuItem();
            this.pan.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grd)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.popupMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // pan
            // 
            this.pan.Controls.Add(this.grd);
            this.pan.Controls.Add(this.statusStrip1);
            this.pan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pan.Location = new System.Drawing.Point(0, 0);
            this.pan.Name = "pan";
            this.pan.Size = new System.Drawing.Size(693, 390);
            this.pan.TabIndex = 0;
            // 
            // grd
            // 
            this.grd.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grd.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grd.Location = new System.Drawing.Point(27, 29);
            this.grd.Name = "grd";
            this.grd.ReadOnly = true;
            this.grd.Size = new System.Drawing.Size(206, 160);
            this.grd.TabIndex = 0;
            this.grd.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grd_CellDoubleClick);
            this.grd.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.grd_ColumnWidthChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLbl});
            this.statusStrip1.Location = new System.Drawing.Point(0, 368);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(693, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLbl
            // 
            this.statusLbl.Name = "statusLbl";
            this.statusLbl.Size = new System.Drawing.Size(0, 17);
            // 
            // popupMenu
            // 
            this.popupMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.popupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemShowViews,
            this.menuItemShowProcs,
            this.menuItemProcParams});
            this.popupMenu.Name = "popupMenu";
            this.popupMenu.Size = new System.Drawing.Size(334, 110);
            this.popupMenu.Opening += new System.ComponentModel.CancelEventHandler(this.popupMenu_Opening);
            // 
            // menuItemShowViews
            // 
            this.menuItemShowViews.Name = "menuItemShowViews";
            this.menuItemShowViews.Size = new System.Drawing.Size(333, 26);
            this.menuItemShowViews.Text = "Show database Views";
            this.menuItemShowViews.Click += new System.EventHandler(this.menuItemShowViews_Click);
            // 
            // menuItemShowProcs
            // 
            this.menuItemShowProcs.Name = "menuItemShowProcs";
            this.menuItemShowProcs.Size = new System.Drawing.Size(333, 26);
            this.menuItemShowProcs.Text = "Show database Procedures";
            this.menuItemShowProcs.Click += new System.EventHandler(this.menuItemShowProcs_Click);
            // 
            // menuItemProcParams
            // 
            this.menuItemProcParams.Name = "menuItemProcParams";
            this.menuItemProcParams.Size = new System.Drawing.Size(333, 26);
            this.menuItemProcParams.Text = "Show database Procedure Parameters";
            this.menuItemProcParams.Click += new System.EventHandler(this.menuItemProcParams_Click);
            // 
            // frmTbls
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 17F);
            this.ClientSize = new System.Drawing.Size(693, 390);
            this.ContextMenuStrip = this.popupMenu;
            this.Controls.Add(this.pan);
            this.Font = new System.Drawing.Font("Lucida Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmTbls";
            this.Text = "Database Tables";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTbls_FormClosing);
            this.Load += new System.EventHandler(this.frmTbls_Load);
            this.Shown += new System.EventHandler(this.frmTbls_Shown);
            this.ResizeEnd += new System.EventHandler(this.frmTbls_ResizeEnd);
            this.pan.ResumeLayout(false);
            this.pan.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grd)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.popupMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pan;
        private System.Windows.Forms.DataGridView grd;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLbl;
        private System.Windows.Forms.ContextMenuStrip popupMenu;
        private System.Windows.Forms.ToolStripMenuItem menuItemShowViews;
        private System.Windows.Forms.ToolStripMenuItem menuItemShowProcs;
        private System.Windows.Forms.ToolStripMenuItem menuItemProcParams;
    }
}
