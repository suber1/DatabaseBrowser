namespace RebusSQL6
{
    partial class frmSQL2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSQL2));
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabSQL = new System.Windows.Forms.TabControl();
            this.pgSQL = new System.Windows.Forms.TabPage();
            this.txtSQL = new System.Windows.Forms.RichTextBox();
            this.popupMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.enableEditingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuLockSQL = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuLastSQL = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuColorize = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmColorizing = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.newViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemChildSQL = new System.Windows.Forms.ToolStripMenuItem();
            this.tstGetRubyStyleFKs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripShowMasterChildInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.exportToTSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToJSON = new System.Windows.Forms.ToolStripMenuItem();
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmChooseChild = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmLinkChild = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmDateTimeToLongFormat = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmDateTimeFormatString = new System.Windows.Forms.ToolStripMenuItem();
            this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pgData = new System.Windows.Forms.TabPage();
            this.grd = new System.Windows.Forms.DataGridView();
            this.pgStruc = new System.Windows.Forms.TabPage();
            this.grdStruc = new System.Windows.Forms.DataGridView();
            this.pgIdxs = new System.Windows.Forms.TabPage();
            this.grdIdxs = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnExec = new System.Windows.Forms.Button();
            this.dlgSave = new System.Windows.Forms.SaveFileDialog();
            this.stat = new System.Windows.Forms.StatusStrip();
            this.statLbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.tmrRowChange = new System.Windows.Forms.Timer(this.components);
            this.prtDoc = new System.Drawing.Printing.PrintDocument();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.panel1.SuspendLayout();
            this.tabSQL.SuspendLayout();
            this.pgSQL.SuspendLayout();
            this.popupMenu.SuspendLayout();
            this.pgData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grd)).BeginInit();
            this.pgStruc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdStruc)).BeginInit();
            this.pgIdxs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdIdxs)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.stat.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tabSQL);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(676, 463);
            this.panel1.TabIndex = 1;
            // 
            // tabSQL
            // 
            this.tabSQL.Controls.Add(this.pgSQL);
            this.tabSQL.Controls.Add(this.pgData);
            this.tabSQL.Controls.Add(this.pgStruc);
            this.tabSQL.Controls.Add(this.pgIdxs);
            this.tabSQL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabSQL.Font = new System.Drawing.Font("Lucida Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabSQL.Location = new System.Drawing.Point(0, 0);
            this.tabSQL.Name = "tabSQL";
            this.tabSQL.SelectedIndex = 0;
            this.tabSQL.Size = new System.Drawing.Size(676, 419);
            this.tabSQL.TabIndex = 0;
            this.tabSQL.SelectedIndexChanged += new System.EventHandler(this.tabSQL_SelectedIndexChanged);
            this.tabSQL.DoubleClick += new System.EventHandler(this.tabSQL_DoubleClick);
            // 
            // pgSQL
            // 
            this.pgSQL.Controls.Add(this.txtSQL);
            this.pgSQL.Location = new System.Drawing.Point(4, 24);
            this.pgSQL.Name = "pgSQL";
            this.pgSQL.Padding = new System.Windows.Forms.Padding(3);
            this.pgSQL.Size = new System.Drawing.Size(668, 391);
            this.pgSQL.TabIndex = 0;
            this.pgSQL.Text = "SQL";
            this.pgSQL.UseVisualStyleBackColor = true;
            // 
            // txtSQL
            // 
            this.txtSQL.AcceptsTab = true;
            this.txtSQL.ContextMenuStrip = this.popupMenu;
            this.txtSQL.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSQL.HideSelection = false;
            this.txtSQL.Location = new System.Drawing.Point(25, 27);
            this.txtSQL.Name = "txtSQL";
            this.txtSQL.Size = new System.Drawing.Size(275, 214);
            this.txtSQL.TabIndex = 0;
            this.txtSQL.Text = "";
            this.txtSQL.TextChanged += new System.EventHandler(this.txtSQL_TextChanged);
            this.txtSQL.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSQL_KeyDown);
            this.txtSQL.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSQL_KeyUp);
            this.txtSQL.MouseUp += new System.Windows.Forms.MouseEventHandler(this.txtSQL_MouseUp);
            // 
            // popupMenu
            // 
            this.popupMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.popupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enableEditingToolStripMenuItem,
            this.toolStripMenuLockSQL,
            this.toolStripSeparator1,
            this.toolStripMenuLastSQL,
            this.toolStripMenuColorize,
            this.tsmColorizing,
            this.toolStripMenuItem1,
            this.propertiesToolStripMenuItem,
            this.toolStripMenuItem2,
            this.newViewToolStripMenuItem,
            this.toolStripMenuItemChildSQL,
            this.tstGetRubyStyleFKs,
            this.toolStripShowMasterChildInfo,
            this.toolStripMenuItem3,
            this.exportToTSVToolStripMenuItem,
            this.mnuToJSON,
            this.printToolStripMenuItem,
            this.toolStripMenuItem4,
            this.tsmChooseChild,
            this.tsmLinkChild,
            this.toolStripMenuItem5,
            this.tsmDateTimeToLongFormat,
            this.tsmDateTimeFormatString,
            this.infoToolStripMenuItem});
            this.popupMenu.Name = "popupMenu";
            this.popupMenu.Size = new System.Drawing.Size(215, 436);
            this.popupMenu.Opening += new System.ComponentModel.CancelEventHandler(this.popupMenu_Opening);
            // 
            // enableEditingToolStripMenuItem
            // 
            this.enableEditingToolStripMenuItem.Name = "enableEditingToolStripMenuItem";
            this.enableEditingToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.enableEditingToolStripMenuItem.Text = "Allow Changes";
            this.enableEditingToolStripMenuItem.Click += new System.EventHandler(this.enableEditingToolStripMenuItem_Click);
            // 
            // toolStripMenuLockSQL
            // 
            this.toolStripMenuLockSQL.Name = "toolStripMenuLockSQL";
            this.toolStripMenuLockSQL.Size = new System.Drawing.Size(214, 22);
            this.toolStripMenuLockSQL.Text = "Lock SQL";
            this.toolStripMenuLockSQL.CheckedChanged += new System.EventHandler(this.toolStripMenuLockSQL_CheckedChanged);
            this.toolStripMenuLockSQL.Click += new System.EventHandler(this.toolStripMenuLockSQL_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(211, 6);
            // 
            // toolStripMenuLastSQL
            // 
            this.toolStripMenuLastSQL.Name = "toolStripMenuLastSQL";
            this.toolStripMenuLastSQL.Size = new System.Drawing.Size(214, 22);
            this.toolStripMenuLastSQL.Text = "Last SQL...";
            this.toolStripMenuLastSQL.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
            // 
            // toolStripMenuColorize
            // 
            this.toolStripMenuColorize.Name = "toolStripMenuColorize";
            this.toolStripMenuColorize.Size = new System.Drawing.Size(214, 22);
            this.toolStripMenuColorize.Text = "Colorize SQL";
            this.toolStripMenuColorize.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
            // 
            // tsmColorizing
            // 
            this.tsmColorizing.Name = "tsmColorizing";
            this.tsmColorizing.Size = new System.Drawing.Size(214, 22);
            this.tsmColorizing.Text = "Colorizing";
            this.tsmColorizing.Click += new System.EventHandler(this.tsmColorizing_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(211, 6);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.propertiesToolStripMenuItem.Text = "Properties...";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(211, 6);
            // 
            // newViewToolStripMenuItem
            // 
            this.newViewToolStripMenuItem.Name = "newViewToolStripMenuItem";
            this.newViewToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.newViewToolStripMenuItem.Text = "New SQL";
            this.newViewToolStripMenuItem.Click += new System.EventHandler(this.newViewToolStripMenuItem_Click);
            // 
            // toolStripMenuItemChildSQL
            // 
            this.toolStripMenuItemChildSQL.Name = "toolStripMenuItemChildSQL";
            this.toolStripMenuItemChildSQL.Size = new System.Drawing.Size(214, 22);
            this.toolStripMenuItemChildSQL.Text = "Child SQL...";
            this.toolStripMenuItemChildSQL.Click += new System.EventHandler(this.toolStripMenuItemChildSQL_Click);
            // 
            // tstGetRubyStyleFKs
            // 
            this.tstGetRubyStyleFKs.Name = "tstGetRubyStyleFKs";
            this.tstGetRubyStyleFKs.Size = new System.Drawing.Size(214, 22);
            this.tstGetRubyStyleFKs.Text = "Get Ruby/Rails FKs...";
            this.tstGetRubyStyleFKs.Click += new System.EventHandler(this.tstGetRubyStyleFKs_Click);
            // 
            // toolStripShowMasterChildInfo
            // 
            this.toolStripShowMasterChildInfo.Name = "toolStripShowMasterChildInfo";
            this.toolStripShowMasterChildInfo.Size = new System.Drawing.Size(214, 22);
            this.toolStripShowMasterChildInfo.Text = "Parent/Child Info...";
            this.toolStripShowMasterChildInfo.Click += new System.EventHandler(this.toolStripShowMasterChildInfo_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(211, 6);
            // 
            // exportToTSVToolStripMenuItem
            // 
            this.exportToTSVToolStripMenuItem.Name = "exportToTSVToolStripMenuItem";
            this.exportToTSVToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.exportToTSVToolStripMenuItem.Text = "Export to TSV...";
            this.exportToTSVToolStripMenuItem.Click += new System.EventHandler(this.exportToTSVToolStripMenuItem_Click);
            // 
            // mnuToJSON
            // 
            this.mnuToJSON.Name = "mnuToJSON";
            this.mnuToJSON.Size = new System.Drawing.Size(214, 22);
            this.mnuToJSON.Text = "Export to JSON...";
            this.mnuToJSON.Click += new System.EventHandler(this.mnuToJSON_Click);
            // 
            // printToolStripMenuItem
            // 
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            this.printToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.printToolStripMenuItem.Text = "Print...";
            this.printToolStripMenuItem.Click += new System.EventHandler(this.printToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(211, 6);
            // 
            // tsmChooseChild
            // 
            this.tsmChooseChild.Name = "tsmChooseChild";
            this.tsmChooseChild.Size = new System.Drawing.Size(214, 22);
            this.tsmChooseChild.Text = "Choose Child...";
            this.tsmChooseChild.Click += new System.EventHandler(this.tsmChooseChild_Click);
            // 
            // tsmLinkChild
            // 
            this.tsmLinkChild.Name = "tsmLinkChild";
            this.tsmLinkChild.Size = new System.Drawing.Size(214, 22);
            this.tsmLinkChild.Text = "Link as Child...";
            this.tsmLinkChild.Click += new System.EventHandler(this.tsmLinkChild_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(211, 6);
            // 
            // tsmDateTimeToLongFormat
            // 
            this.tsmDateTimeToLongFormat.Name = "tsmDateTimeToLongFormat";
            this.tsmDateTimeToLongFormat.Size = new System.Drawing.Size(214, 22);
            this.tsmDateTimeToLongFormat.Text = "DateTimes Long Format";
            this.tsmDateTimeToLongFormat.Click += new System.EventHandler(this.tsmDateTimeToLongFormat_Click);
            // 
            // tsmDateTimeFormatString
            // 
            this.tsmDateTimeFormatString.Name = "tsmDateTimeFormatString";
            this.tsmDateTimeFormatString.Size = new System.Drawing.Size(214, 22);
            this.tsmDateTimeFormatString.Text = "DateTimes Format String...";
            this.tsmDateTimeFormatString.Click += new System.EventHandler(this.tsmDateTimeFormatString_Click);
            // 
            // infoToolStripMenuItem
            // 
            this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            this.infoToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.infoToolStripMenuItem.Text = "Info...";
            this.infoToolStripMenuItem.Click += new System.EventHandler(this.infoToolStripMenuItem_Click);
            // 
            // pgData
            // 
            this.pgData.Controls.Add(this.grd);
            this.pgData.Location = new System.Drawing.Point(4, 24);
            this.pgData.Name = "pgData";
            this.pgData.Padding = new System.Windows.Forms.Padding(3);
            this.pgData.Size = new System.Drawing.Size(668, 391);
            this.pgData.TabIndex = 1;
            this.pgData.Text = "Data";
            this.pgData.UseVisualStyleBackColor = true;
            // 
            // grd
            // 
            this.grd.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grd.Location = new System.Drawing.Point(3, 3);
            this.grd.Name = "grd";
            this.grd.Size = new System.Drawing.Size(662, 385);
            this.grd.TabIndex = 0;
            this.grd.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grd_ColumnHeaderMouseClick);
            this.grd.ColumnSortModeChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.grd_ColumnSortModeChanged);
            this.grd.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.grd_ColumnWidthChanged);
            this.grd.CurrentCellDirtyStateChanged += new System.EventHandler(this.grd_CurrentCellDirtyStateChanged);
            this.grd.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.grd_DataError);
            this.grd.RowLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.grd_RowLeave);
            this.grd.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.grd_RowPostPaint);
            this.grd.RowStateChanged += new System.Windows.Forms.DataGridViewRowStateChangedEventHandler(this.grd_RowStateChanged);
            // 
            // pgStruc
            // 
            this.pgStruc.Controls.Add(this.grdStruc);
            this.pgStruc.Location = new System.Drawing.Point(4, 24);
            this.pgStruc.Name = "pgStruc";
            this.pgStruc.Size = new System.Drawing.Size(668, 391);
            this.pgStruc.TabIndex = 2;
            this.pgStruc.Text = "Structure";
            this.pgStruc.UseVisualStyleBackColor = true;
            // 
            // grdStruc
            // 
            this.grdStruc.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdStruc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdStruc.Location = new System.Drawing.Point(0, 0);
            this.grdStruc.Name = "grdStruc";
            this.grdStruc.Size = new System.Drawing.Size(668, 391);
            this.grdStruc.TabIndex = 0;
            // 
            // pgIdxs
            // 
            this.pgIdxs.Controls.Add(this.grdIdxs);
            this.pgIdxs.Location = new System.Drawing.Point(4, 24);
            this.pgIdxs.Name = "pgIdxs";
            this.pgIdxs.Size = new System.Drawing.Size(668, 391);
            this.pgIdxs.TabIndex = 3;
            this.pgIdxs.Text = "Indices";
            this.pgIdxs.UseVisualStyleBackColor = true;
            // 
            // grdIdxs
            // 
            this.grdIdxs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdIdxs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdIdxs.Location = new System.Drawing.Point(0, 0);
            this.grdIdxs.Name = "grdIdxs";
            this.grdIdxs.Size = new System.Drawing.Size(668, 391);
            this.grdIdxs.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnZoomIn);
            this.panel2.Controls.Add(this.btnZoomOut);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 419);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(676, 44);
            this.panel2.TabIndex = 1;
            this.panel2.DoubleClick += new System.EventHandler(this.panel2_DoubleClick);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Font = new System.Drawing.Font("Lucida Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnZoomIn.Location = new System.Drawing.Point(51, 10);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(28, 26);
            this.btnZoomIn.TabIndex = 5;
            this.btnZoomIn.Text = "+";
            this.toolTip.SetToolTip(this.btnZoomIn, "Increase font size");
            this.btnZoomIn.UseVisualStyleBackColor = true;
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Font = new System.Drawing.Font("Lucida Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnZoomOut.Location = new System.Drawing.Point(13, 10);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(28, 26);
            this.btnZoomOut.TabIndex = 4;
            this.btnZoomOut.Text = "-";
            this.toolTip.SetToolTip(this.btnZoomOut, "Decrease font size");
            this.btnZoomOut.UseVisualStyleBackColor = true;
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.btnSave);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(415, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(133, 44);
            this.panel4.TabIndex = 3;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(5, 8);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(122, 29);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save Changes";
            this.toolTip.SetToolTip(this.btnSave, "Save changes to data");
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnExec);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(548, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(128, 44);
            this.panel3.TabIndex = 2;
            // 
            // btnExec
            // 
            this.btnExec.Location = new System.Drawing.Point(9, 8);
            this.btnExec.Name = "btnExec";
            this.btnExec.Size = new System.Drawing.Size(109, 29);
            this.btnExec.TabIndex = 2;
            this.btnExec.Text = "Execute";
            this.toolTip.SetToolTip(this.btnExec, "Execute current SQL (also via <F5> (<Ctrl + F5> for Asynchronous (SQL only)))");
            this.btnExec.UseVisualStyleBackColor = true;
            this.btnExec.Click += new System.EventHandler(this.btnExec_Click);
            // 
            // dlgSave
            // 
            this.dlgSave.FileOk += new System.ComponentModel.CancelEventHandler(this.dlgSave_FileOk);
            // 
            // stat
            // 
            this.stat.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.stat.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statLbl});
            this.stat.Location = new System.Drawing.Point(0, 463);
            this.stat.Name = "stat";
            this.stat.Size = new System.Drawing.Size(676, 22);
            this.stat.TabIndex = 3;
            this.stat.Text = "statusStrip1";
            this.stat.DoubleClick += new System.EventHandler(this.stat_DoubleClick);
            // 
            // statLbl
            // 
            this.statLbl.Name = "statLbl";
            this.statLbl.Size = new System.Drawing.Size(0, 17);
            // 
            // tmrRowChange
            // 
            this.tmrRowChange.Interval = 500;
            this.tmrRowChange.Tick += new System.EventHandler(this.tmrRowChange_Tick);
            // 
            // prtDoc
            // 
            this.prtDoc.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.prtDoc_PrintPage);
            // 
            // frmSQL2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(676, 485);
            this.ContextMenuStrip = this.popupMenu;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.stat);
            this.Font = new System.Drawing.Font("Lucida Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormType = RebusSQL6.MDIType.SQL;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(100, 100);
            this.Name = "frmSQL2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Query/Command";
            this.Activated += new System.EventHandler(this.frmSQL2_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSQL2_FormClosing);
            this.Load += new System.EventHandler(this.frmSQL2_Load);
            this.Shown += new System.EventHandler(this.frmSQL2_Shown);
            this.ResizeEnd += new System.EventHandler(this.frmSQL2_ResizeEnd);
            this.DoubleClick += new System.EventHandler(this.frmSQL2_DoubleClick);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmSQL2_KeyUp);
            this.panel1.ResumeLayout(false);
            this.tabSQL.ResumeLayout(false);
            this.pgSQL.ResumeLayout(false);
            this.popupMenu.ResumeLayout(false);
            this.pgData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grd)).EndInit();
            this.pgStruc.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdStruc)).EndInit();
            this.pgIdxs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdIdxs)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.stat.ResumeLayout(false);
            this.stat.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tabSQL;
        private System.Windows.Forms.TabPage pgSQL;
        private System.Windows.Forms.TabPage pgData;
        private System.Windows.Forms.RichTextBox txtSQL;
        private System.Windows.Forms.TabPage pgStruc;
        private System.Windows.Forms.TabPage pgIdxs;
        private System.Windows.Forms.DataGridView grd;
        private System.Windows.Forms.DataGridView grdStruc;
        private System.Windows.Forms.DataGridView grdIdxs;
        private System.Windows.Forms.SaveFileDialog dlgSave;
        private System.Windows.Forms.StatusStrip stat;
        private System.Windows.Forms.ToolStripStatusLabel statLbl;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnExec;
        private System.Windows.Forms.ContextMenuStrip popupMenu;
        private System.Windows.Forms.ToolStripMenuItem enableEditingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToTSVToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuLastSQL;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuColorize;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuLockSQL;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemChildSQL;
        private System.Windows.Forms.Timer tmrRowChange;
        private System.Windows.Forms.ToolStripMenuItem toolStripShowMasterChildInfo;
        private System.Drawing.Printing.PrintDocument prtDoc;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmLinkChild;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem tsmDateTimeToLongFormat;
        private System.Windows.Forms.ToolStripMenuItem tsmChooseChild;
        private System.Windows.Forms.ToolStripMenuItem mnuToJSON;
        private System.Windows.Forms.ToolStripMenuItem tsmColorizing;
        private System.Windows.Forms.ToolStripMenuItem tstGetRubyStyleFKs;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripMenuItem tsmDateTimeFormatString;
    }
}
