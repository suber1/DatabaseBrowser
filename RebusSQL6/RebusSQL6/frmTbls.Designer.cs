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
            this.trvw = new System.Windows.Forms.TreeView();
            this.grd = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.popupMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmSeleTop = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmSeleAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmCount = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmSeleDistinct = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmSeleDistinctCounts = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmHighlight = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmUnhighlight = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemShowViews = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemShowProcs = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemProcParams = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuItemFldNames = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmShowAllIdxInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmColmSort = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.pan.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grd)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.popupMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // pan
            // 
            this.pan.Controls.Add(this.trvw);
            this.pan.Controls.Add(this.grd);
            this.pan.Controls.Add(this.statusStrip1);
            this.pan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pan.Location = new System.Drawing.Point(0, 0);
            this.pan.Name = "pan";
            this.pan.Size = new System.Drawing.Size(693, 390);
            this.pan.TabIndex = 0;
            // 
            // trvw
            // 
            this.trvw.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trvw.Location = new System.Drawing.Point(323, 197);
            this.trvw.Name = "trvw";
            this.trvw.Size = new System.Drawing.Size(121, 97);
            this.trvw.TabIndex = 2;
            this.trvw.Visible = false;
            this.trvw.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.trvw_BeforeExpand);
            this.trvw.DoubleClick += new System.EventHandler(this.trvw_DoubleClick);
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
            this.tsmSeleTop,
            this.tsmSeleAll,
            this.tsmCount,
            this.tsmSeleDistinct,
            this.tsmSeleDistinctCounts,
            this.toolStripSeparator2,
            this.tsmHighlight,
            this.tsmUnhighlight,
            this.toolStripSeparator1,
            this.menuItemShowViews,
            this.menuItemShowProcs,
            this.menuItemProcParams,
            this.toolStripMenuItem1,
            this.mnuItemFldNames,
            this.tsmShowAllIdxInfo,
            this.toolStripMenuItem2,
            this.tsmColmSort,
            this.tsmRefresh});
            this.popupMenu.Name = "popupMenu";
            this.popupMenu.Size = new System.Drawing.Size(331, 358);
            this.popupMenu.Opening += new System.ComponentModel.CancelEventHandler(this.popupMenu_Opening);
            // 
            // tsmSeleTop
            // 
            this.tsmSeleTop.Name = "tsmSeleTop";
            this.tsmSeleTop.Size = new System.Drawing.Size(330, 22);
            this.tsmSeleTop.Text = "SELECT TOP rows";
            this.tsmSeleTop.Click += new System.EventHandler(this.tsmSeleTop_Click);
            // 
            // tsmSeleAll
            // 
            this.tsmSeleAll.Name = "tsmSeleAll";
            this.tsmSeleAll.Size = new System.Drawing.Size(330, 22);
            this.tsmSeleAll.Text = "SELECT all rows";
            this.tsmSeleAll.Click += new System.EventHandler(this.tsmSeleAll_Click);
            // 
            // tsmCount
            // 
            this.tsmCount.Name = "tsmCount";
            this.tsmCount.Size = new System.Drawing.Size(330, 22);
            this.tsmCount.Text = "COUNT";
            this.tsmCount.Click += new System.EventHandler(this.tsmCount_Click);
            // 
            // tsmSeleDistinct
            // 
            this.tsmSeleDistinct.Name = "tsmSeleDistinct";
            this.tsmSeleDistinct.Size = new System.Drawing.Size(330, 22);
            this.tsmSeleDistinct.Text = "SELECT DISTINCT";
            this.tsmSeleDistinct.Click += new System.EventHandler(this.tsmSeleDistinct_Click);
            // 
            // tsmSeleDistinctCounts
            // 
            this.tsmSeleDistinctCounts.Name = "tsmSeleDistinctCounts";
            this.tsmSeleDistinctCounts.Size = new System.Drawing.Size(330, 22);
            this.tsmSeleDistinctCounts.Text = "SELECT DISTINCT (with counts)";
            this.tsmSeleDistinctCounts.Click += new System.EventHandler(this.tsmSeleDistinctCounts_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(327, 6);
            // 
            // tsmHighlight
            // 
            this.tsmHighlight.Name = "tsmHighlight";
            this.tsmHighlight.Size = new System.Drawing.Size(330, 22);
            this.tsmHighlight.Text = "Highlight...";
            this.tsmHighlight.Click += new System.EventHandler(this.tsmHighlight_Click);
            // 
            // tsmUnhighlight
            // 
            this.tsmUnhighlight.Name = "tsmUnhighlight";
            this.tsmUnhighlight.Size = new System.Drawing.Size(330, 22);
            this.tsmUnhighlight.Text = "Unhighlight";
            this.tsmUnhighlight.Click += new System.EventHandler(this.tsmUnhighlight_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(327, 6);
            // 
            // menuItemShowViews
            // 
            this.menuItemShowViews.Name = "menuItemShowViews";
            this.menuItemShowViews.Size = new System.Drawing.Size(330, 22);
            this.menuItemShowViews.Text = "Show database Views";
            this.menuItemShowViews.Click += new System.EventHandler(this.menuItemShowViews_Click);
            // 
            // menuItemShowProcs
            // 
            this.menuItemShowProcs.Name = "menuItemShowProcs";
            this.menuItemShowProcs.Size = new System.Drawing.Size(330, 22);
            this.menuItemShowProcs.Text = "Show database Procedures";
            this.menuItemShowProcs.Click += new System.EventHandler(this.menuItemShowProcs_Click);
            // 
            // menuItemProcParams
            // 
            this.menuItemProcParams.Name = "menuItemProcParams";
            this.menuItemProcParams.Size = new System.Drawing.Size(330, 22);
            this.menuItemProcParams.Text = "Show database Procedure Parameters";
            this.menuItemProcParams.Click += new System.EventHandler(this.menuItemProcParams_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(327, 6);
            // 
            // mnuItemFldNames
            // 
            this.mnuItemFldNames.Name = "mnuItemFldNames";
            this.mnuItemFldNames.Size = new System.Drawing.Size(330, 22);
            this.mnuItemFldNames.Text = "Field names instead of * for double-click of table";
            this.mnuItemFldNames.Click += new System.EventHandler(this.mnuItemFldNames_Click);
            // 
            // tsmShowAllIdxInfo
            // 
            this.tsmShowAllIdxInfo.Name = "tsmShowAllIdxInfo";
            this.tsmShowAllIdxInfo.Size = new System.Drawing.Size(330, 22);
            this.tsmShowAllIdxInfo.Text = "Show all index info on double-click";
            this.tsmShowAllIdxInfo.Click += new System.EventHandler(this.tsmShowAllIdxInfo_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(327, 6);
            // 
            // tsmColmSort
            // 
            this.tsmColmSort.Name = "tsmColmSort";
            this.tsmColmSort.Size = new System.Drawing.Size(330, 22);
            this.tsmColmSort.Text = "Sort columns by ordinal";
            this.tsmColmSort.Click += new System.EventHandler(this.tsmColmSort_Click);
            // 
            // tsmRefresh
            // 
            this.tsmRefresh.Name = "tsmRefresh";
            this.tsmRefresh.Size = new System.Drawing.Size(330, 22);
            this.tsmRefresh.Text = "Refresh";
            this.tsmRefresh.Click += new System.EventHandler(this.tsmRefresh_Click);
            // 
            // frmTbls
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(693, 390);
            this.ContextMenuStrip = this.popupMenu;
            this.Controls.Add(this.pan);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(100, 100);
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
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuItemFldNames;
        private System.Windows.Forms.TreeView trvw;
        private System.Windows.Forms.ToolStripMenuItem tsmSeleTop;
        private System.Windows.Forms.ToolStripMenuItem tsmSeleAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsmCount;
        private System.Windows.Forms.ToolStripMenuItem tsmSeleDistinct;
        private System.Windows.Forms.ToolStripMenuItem tsmSeleDistinctCounts;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem tsmHighlight;
        private System.Windows.Forms.ToolStripMenuItem tsmUnhighlight;
        private System.Windows.Forms.ToolStripMenuItem tsmShowAllIdxInfo;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem tsmRefresh;
        private System.Windows.Forms.ToolStripMenuItem tsmColmSort;
    }
}
