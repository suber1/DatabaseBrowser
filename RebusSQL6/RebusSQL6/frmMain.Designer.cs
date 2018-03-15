namespace RebusSQL6
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.stat = new System.Windows.Forms.StatusStrip();
            this.statusLbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabMenu = new System.Windows.Forms.TabControl();
            this.pgFile = new System.Windows.Forms.TabPage();
            this.drpGrps = new System.Windows.Forms.ComboBox();
            this.lblGrps = new System.Windows.Forms.Label();
            this.btnPrevDB = new System.Windows.Forms.Button();
            this.drpViews2 = new System.Windows.Forms.ComboBox();
            this.lblActView = new System.Windows.Forms.Label();
            this.btnRestoreSQL = new System.Windows.Forms.Button();
            this.btnSaveSQL = new System.Windows.Forms.Button();
            this.btnCloseDB = new System.Windows.Forms.Button();
            this.btnShowTable = new System.Windows.Forms.Button();
            this.btnNewSQLwindow = new System.Windows.Forms.Button();
            this.btnNewDB = new System.Windows.Forms.Button();
            this.pgWindows = new System.Windows.Forms.TabPage();
            this.lblActiveWindow = new System.Windows.Forms.Label();
            this.drpWindows = new System.Windows.Forms.ComboBox();
            this.btnCloseAll = new System.Windows.Forms.Button();
            this.btnTileHorz = new System.Windows.Forms.Button();
            this.btnTileVert = new System.Windows.Forms.Button();
            this.btnCascade = new System.Windows.Forms.Button();
            this.pgOpts = new System.Windows.Forms.TabPage();
            this.btnOptions = new System.Windows.Forms.Button();
            this.btnExtProvs = new System.Windows.Forms.Button();
            this.btnODBC = new System.Windows.Forms.Button();
            this.pgMisc = new System.Windows.Forms.TabPage();
            this.btnAbout = new System.Windows.Forms.Button();
            this.tmr = new System.Windows.Forms.Timer(this.components);
            this.stat.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabMenu.SuspendLayout();
            this.pgFile.SuspendLayout();
            this.pgWindows.SuspendLayout();
            this.pgOpts.SuspendLayout();
            this.pgMisc.SuspendLayout();
            this.SuspendLayout();
            // 
            // stat
            // 
            this.stat.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.stat.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLbl,
            this.toolStripStatusLabel1});
            this.stat.Location = new System.Drawing.Point(0, 642);
            this.stat.Name = "stat";
            this.stat.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.stat.Size = new System.Drawing.Size(1221, 22);
            this.stat.TabIndex = 3;
            this.stat.Text = "statusStrip1";
            this.stat.DoubleClick += new System.EventHandler(this.stat_DoubleClick);
            // 
            // statusLbl
            // 
            this.statusLbl.Font = new System.Drawing.Font("Lucida Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLbl.Name = "statusLbl";
            this.statusLbl.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tabMenu);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1221, 141);
            this.panel1.TabIndex = 4;
            // 
            // tabMenu
            // 
            this.tabMenu.Controls.Add(this.pgFile);
            this.tabMenu.Controls.Add(this.pgWindows);
            this.tabMenu.Controls.Add(this.pgOpts);
            this.tabMenu.Controls.Add(this.pgMisc);
            this.tabMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMenu.Font = new System.Drawing.Font("Lucida Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabMenu.Location = new System.Drawing.Point(0, 0);
            this.tabMenu.Name = "tabMenu";
            this.tabMenu.SelectedIndex = 0;
            this.tabMenu.Size = new System.Drawing.Size(1221, 141);
            this.tabMenu.TabIndex = 1;
            this.tabMenu.Click += new System.EventHandler(this.tabMenu_Click);
            // 
            // pgFile
            // 
            this.pgFile.Controls.Add(this.drpGrps);
            this.pgFile.Controls.Add(this.lblGrps);
            this.pgFile.Controls.Add(this.btnPrevDB);
            this.pgFile.Controls.Add(this.drpViews2);
            this.pgFile.Controls.Add(this.lblActView);
            this.pgFile.Controls.Add(this.btnRestoreSQL);
            this.pgFile.Controls.Add(this.btnSaveSQL);
            this.pgFile.Controls.Add(this.btnCloseDB);
            this.pgFile.Controls.Add(this.btnShowTable);
            this.pgFile.Controls.Add(this.btnNewSQLwindow);
            this.pgFile.Controls.Add(this.btnNewDB);
            this.pgFile.Location = new System.Drawing.Point(4, 24);
            this.pgFile.Name = "pgFile";
            this.pgFile.Padding = new System.Windows.Forms.Padding(3);
            this.pgFile.Size = new System.Drawing.Size(1213, 113);
            this.pgFile.TabIndex = 0;
            this.pgFile.Text = "Database";
            this.pgFile.UseVisualStyleBackColor = true;
            // 
            // drpGrps
            // 
            this.drpGrps.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpGrps.Font = new System.Drawing.Font("Lucida Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.drpGrps.FormattingEnabled = true;
            this.drpGrps.Location = new System.Drawing.Point(945, 47);
            this.drpGrps.Name = "drpGrps";
            this.drpGrps.Size = new System.Drawing.Size(195, 22);
            this.drpGrps.TabIndex = 18;
            this.drpGrps.SelectedIndexChanged += new System.EventHandler(this.drpGrps_SelectedIndexChanged);
            // 
            // lblGrps
            // 
            this.lblGrps.Font = new System.Drawing.Font("Lucida Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGrps.Location = new System.Drawing.Point(817, 45);
            this.lblGrps.Name = "lblGrps";
            this.lblGrps.Size = new System.Drawing.Size(122, 32);
            this.lblGrps.TabIndex = 17;
            this.lblGrps.Text = "Bring group to front:";
            this.lblGrps.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnPrevDB
            // 
            this.btnPrevDB.Font = new System.Drawing.Font("Lucida Sans", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrevDB.Image = global::RebusSQL6.Properties.Resources.dbs3;
            this.btnPrevDB.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnPrevDB.Location = new System.Drawing.Point(116, 11);
            this.btnPrevDB.Name = "btnPrevDB";
            this.btnPrevDB.Size = new System.Drawing.Size(176, 98);
            this.btnPrevDB.TabIndex = 16;
            this.btnPrevDB.Text = "Open Previous Database...";
            this.btnPrevDB.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnPrevDB.UseVisualStyleBackColor = true;
            this.btnPrevDB.Click += new System.EventHandler(this.btnPrevDB_Click);
            // 
            // drpViews2
            // 
            this.drpViews2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpViews2.Font = new System.Drawing.Font("Lucida Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.drpViews2.FormattingEnabled = true;
            this.drpViews2.Location = new System.Drawing.Point(945, 21);
            this.drpViews2.Name = "drpViews2";
            this.drpViews2.Size = new System.Drawing.Size(246, 22);
            this.drpViews2.TabIndex = 15;
            this.drpViews2.SelectedIndexChanged += new System.EventHandler(this.drpViews2_SelectedIndexChanged);
            // 
            // lblActView
            // 
            this.lblActView.Font = new System.Drawing.Font("Lucida Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblActView.Location = new System.Drawing.Point(817, 18);
            this.lblActView.Name = "lblActView";
            this.lblActView.Size = new System.Drawing.Size(122, 32);
            this.lblActView.TabIndex = 14;
            this.lblActView.Text = "Active SQL Window:";
            this.lblActView.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnRestoreSQL
            // 
            this.btnRestoreSQL.Font = new System.Drawing.Font("Lucida Sans", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRestoreSQL.Location = new System.Drawing.Point(702, 11);
            this.btnRestoreSQL.Name = "btnRestoreSQL";
            this.btnRestoreSQL.Size = new System.Drawing.Size(90, 97);
            this.btnRestoreSQL.TabIndex = 7;
            this.btnRestoreSQL.Text = "Restore SQLs...";
            this.btnRestoreSQL.UseVisualStyleBackColor = true;
            this.btnRestoreSQL.Click += new System.EventHandler(this.btnRestoreSQL_Click);
            // 
            // btnSaveSQL
            // 
            this.btnSaveSQL.Font = new System.Drawing.Font("Lucida Sans", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveSQL.Image = global::RebusSQL6.Properties.Resources.hdd;
            this.btnSaveSQL.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSaveSQL.Location = new System.Drawing.Point(589, 9);
            this.btnSaveSQL.Name = "btnSaveSQL";
            this.btnSaveSQL.Size = new System.Drawing.Size(107, 97);
            this.btnSaveSQL.TabIndex = 6;
            this.btnSaveSQL.Text = "Save SQLs...";
            this.btnSaveSQL.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSaveSQL.UseVisualStyleBackColor = true;
            this.btnSaveSQL.Click += new System.EventHandler(this.btnSaveSQL_Click);
            // 
            // btnCloseDB
            // 
            this.btnCloseDB.Font = new System.Drawing.Font("Lucida Sans", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCloseDB.Image = global::RebusSQL6.Properties.Resources.db_closeB;
            this.btnCloseDB.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCloseDB.Location = new System.Drawing.Point(396, 9);
            this.btnCloseDB.Name = "btnCloseDB";
            this.btnCloseDB.Size = new System.Drawing.Size(108, 98);
            this.btnCloseDB.TabIndex = 5;
            this.btnCloseDB.Text = "Close Database...";
            this.btnCloseDB.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCloseDB.UseVisualStyleBackColor = true;
            this.btnCloseDB.Click += new System.EventHandler(this.btnCloseDB_Click);
            // 
            // btnShowTable
            // 
            this.btnShowTable.Font = new System.Drawing.Font("Lucida Sans", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShowTable.Location = new System.Drawing.Point(298, 11);
            this.btnShowTable.Name = "btnShowTable";
            this.btnShowTable.Size = new System.Drawing.Size(92, 98);
            this.btnShowTable.TabIndex = 4;
            this.btnShowTable.Text = "Show Database Tables";
            this.btnShowTable.UseVisualStyleBackColor = true;
            this.btnShowTable.Click += new System.EventHandler(this.btnShowTable_Click);
            this.btnShowTable.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnShowTable_MouseUp);
            // 
            // btnNewSQLwindow
            // 
            this.btnNewSQLwindow.Font = new System.Drawing.Font("Lucida Sans", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewSQLwindow.Image = global::RebusSQL6.Properties.Resources.window_plusC;
            this.btnNewSQLwindow.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnNewSQLwindow.Location = new System.Drawing.Point(510, 11);
            this.btnNewSQLwindow.Name = "btnNewSQLwindow";
            this.btnNewSQLwindow.Size = new System.Drawing.Size(73, 96);
            this.btnNewSQLwindow.TabIndex = 3;
            this.btnNewSQLwindow.Text = "New SQL";
            this.btnNewSQLwindow.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnNewSQLwindow.UseVisualStyleBackColor = true;
            this.btnNewSQLwindow.Click += new System.EventHandler(this.btnNewSQLwindow_Click);
            // 
            // btnNewDB
            // 
            this.btnNewDB.Font = new System.Drawing.Font("Lucida Sans", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewDB.Image = global::RebusSQL6.Properties.Resources.db4plus;
            this.btnNewDB.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnNewDB.Location = new System.Drawing.Point(8, 8);
            this.btnNewDB.Name = "btnNewDB";
            this.btnNewDB.Size = new System.Drawing.Size(102, 98);
            this.btnNewDB.TabIndex = 2;
            this.btnNewDB.Text = "Open Database...";
            this.btnNewDB.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnNewDB.UseVisualStyleBackColor = true;
            this.btnNewDB.Click += new System.EventHandler(this.btnNewDB_Click);
            // 
            // pgWindows
            // 
            this.pgWindows.Controls.Add(this.lblActiveWindow);
            this.pgWindows.Controls.Add(this.drpWindows);
            this.pgWindows.Controls.Add(this.btnCloseAll);
            this.pgWindows.Controls.Add(this.btnTileHorz);
            this.pgWindows.Controls.Add(this.btnTileVert);
            this.pgWindows.Controls.Add(this.btnCascade);
            this.pgWindows.Location = new System.Drawing.Point(4, 24);
            this.pgWindows.Name = "pgWindows";
            this.pgWindows.Size = new System.Drawing.Size(1213, 113);
            this.pgWindows.TabIndex = 3;
            this.pgWindows.Text = "SQLs";
            this.pgWindows.UseVisualStyleBackColor = true;
            // 
            // lblActiveWindow
            // 
            this.lblActiveWindow.Font = new System.Drawing.Font("Lucida Sans", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblActiveWindow.Location = new System.Drawing.Point(406, 26);
            this.lblActiveWindow.Name = "lblActiveWindow";
            this.lblActiveWindow.Size = new System.Drawing.Size(134, 19);
            this.lblActiveWindow.TabIndex = 13;
            this.lblActiveWindow.Text = "Active SQL Window:";
            this.lblActiveWindow.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // drpWindows
            // 
            this.drpWindows.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpWindows.FormattingEnabled = true;
            this.drpWindows.Location = new System.Drawing.Point(546, 26);
            this.drpWindows.Name = "drpWindows";
            this.drpWindows.Size = new System.Drawing.Size(450, 23);
            this.drpWindows.TabIndex = 12;
            this.drpWindows.SelectedIndexChanged += new System.EventHandler(this.drpWindows_SelectedIndexChanged);
            // 
            // btnCloseAll
            // 
            this.btnCloseAll.Font = new System.Drawing.Font("Lucida Sans", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCloseAll.Location = new System.Drawing.Point(286, 8);
            this.btnCloseAll.Name = "btnCloseAll";
            this.btnCloseAll.Size = new System.Drawing.Size(63, 85);
            this.btnCloseAll.TabIndex = 11;
            this.btnCloseAll.Text = "Close All...";
            this.btnCloseAll.UseVisualStyleBackColor = true;
            this.btnCloseAll.Click += new System.EventHandler(this.btnCloseAll_Click);
            // 
            // btnTileHorz
            // 
            this.btnTileHorz.Font = new System.Drawing.Font("Lucida Sans", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTileHorz.Location = new System.Drawing.Point(184, 8);
            this.btnTileHorz.Name = "btnTileHorz";
            this.btnTileHorz.Size = new System.Drawing.Size(96, 85);
            this.btnTileHorz.TabIndex = 10;
            this.btnTileHorz.Text = "Tile Horizontally";
            this.btnTileHorz.UseVisualStyleBackColor = true;
            this.btnTileHorz.Click += new System.EventHandler(this.btnTileHorz_Click);
            // 
            // btnTileVert
            // 
            this.btnTileVert.Font = new System.Drawing.Font("Lucida Sans", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTileVert.Location = new System.Drawing.Point(90, 8);
            this.btnTileVert.Name = "btnTileVert";
            this.btnTileVert.Size = new System.Drawing.Size(88, 85);
            this.btnTileVert.TabIndex = 9;
            this.btnTileVert.Text = "Tile Vertically";
            this.btnTileVert.UseVisualStyleBackColor = true;
            this.btnTileVert.Click += new System.EventHandler(this.btnTileVert_Click);
            // 
            // btnCascade
            // 
            this.btnCascade.Font = new System.Drawing.Font("Lucida Sans", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCascade.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCascade.Location = new System.Drawing.Point(8, 8);
            this.btnCascade.Name = "btnCascade";
            this.btnCascade.Size = new System.Drawing.Size(76, 85);
            this.btnCascade.TabIndex = 8;
            this.btnCascade.Text = "Cascade";
            this.btnCascade.UseVisualStyleBackColor = true;
            this.btnCascade.Click += new System.EventHandler(this.btnCascade_Click);
            // 
            // pgOpts
            // 
            this.pgOpts.Controls.Add(this.btnOptions);
            this.pgOpts.Controls.Add(this.btnExtProvs);
            this.pgOpts.Controls.Add(this.btnODBC);
            this.pgOpts.Location = new System.Drawing.Point(4, 24);
            this.pgOpts.Name = "pgOpts";
            this.pgOpts.Size = new System.Drawing.Size(1213, 113);
            this.pgOpts.TabIndex = 2;
            this.pgOpts.Text = "Options";
            this.pgOpts.UseVisualStyleBackColor = true;
            // 
            // btnOptions
            // 
            this.btnOptions.Font = new System.Drawing.Font("Lucida Sans", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOptions.Location = new System.Drawing.Point(322, 8);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(88, 85);
            this.btnOptions.TabIndex = 7;
            this.btnOptions.Text = "Settings...";
            this.btnOptions.UseVisualStyleBackColor = true;
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            // 
            // btnExtProvs
            // 
            this.btnExtProvs.Font = new System.Drawing.Font("Lucida Sans", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExtProvs.Location = new System.Drawing.Point(144, 8);
            this.btnExtProvs.Name = "btnExtProvs";
            this.btnExtProvs.Size = new System.Drawing.Size(162, 85);
            this.btnExtProvs.TabIndex = 6;
            this.btnExtProvs.Text = "Manage Additional Providers...";
            this.btnExtProvs.UseVisualStyleBackColor = true;
            this.btnExtProvs.Click += new System.EventHandler(this.btnExtProvs_Click);
            // 
            // btnODBC
            // 
            this.btnODBC.Font = new System.Drawing.Font("Lucida Sans", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnODBC.Location = new System.Drawing.Point(12, 8);
            this.btnODBC.Name = "btnODBC";
            this.btnODBC.Size = new System.Drawing.Size(116, 85);
            this.btnODBC.TabIndex = 5;
            this.btnODBC.Text = "ODBC Data Source Administrator...";
            this.btnODBC.UseVisualStyleBackColor = true;
            this.btnODBC.Click += new System.EventHandler(this.btnODBC_Click);
            // 
            // pgMisc
            // 
            this.pgMisc.Controls.Add(this.btnAbout);
            this.pgMisc.Location = new System.Drawing.Point(4, 24);
            this.pgMisc.Name = "pgMisc";
            this.pgMisc.Size = new System.Drawing.Size(1213, 113);
            this.pgMisc.TabIndex = 4;
            this.pgMisc.Text = "Misc";
            this.pgMisc.UseVisualStyleBackColor = true;
            // 
            // btnAbout
            // 
            this.btnAbout.Font = new System.Drawing.Font("Lucida Sans", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAbout.Location = new System.Drawing.Point(9, 8);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(94, 85);
            this.btnAbout.TabIndex = 12;
            this.btnAbout.Text = "About...";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // tmr
            // 
            this.tmr.Interval = 500;
            this.tmr.Tick += new System.EventHandler(this.tmr_Tick);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1221, 664);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.stat);
            this.Font = new System.Drawing.Font("Lucida Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Name = "frmMain";
            this.Text = "set by ShowOpenDB() - have a nice day - even if you\'ve had 63 nice days in a row," +
    " and you just want your !@#$ change (G. Carlin, paraphrased)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.frmMain_ControlRemoved);
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.frmMain_Layout);
            this.Move += new System.EventHandler(this.frmMain_Move);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            this.stat.ResumeLayout(false);
            this.stat.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tabMenu.ResumeLayout(false);
            this.pgFile.ResumeLayout(false);
            this.pgWindows.ResumeLayout(false);
            this.pgOpts.ResumeLayout(false);
            this.pgMisc.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip stat;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tabMenu;
        private System.Windows.Forms.TabPage pgFile;
        private System.Windows.Forms.TabPage pgOpts;
        private System.Windows.Forms.TabPage pgWindows;
        private System.Windows.Forms.TabPage pgMisc;
        private System.Windows.Forms.Button btnNewDB;
        private System.Windows.Forms.Button btnNewSQLwindow;
        private System.Windows.Forms.Button btnShowTable;
        private System.Windows.Forms.ToolStripStatusLabel statusLbl;
        private System.Windows.Forms.Button btnCloseDB;
        private System.Windows.Forms.Button btnRestoreSQL;
        private System.Windows.Forms.Button btnSaveSQL;
        private System.Windows.Forms.Button btnCascade;
        private System.Windows.Forms.Button btnTileHorz;
        private System.Windows.Forms.Button btnTileVert;
        private System.Windows.Forms.Button btnCloseAll;
        private System.Windows.Forms.Button btnODBC;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.ComboBox drpWindows;
        private System.Windows.Forms.Label lblActiveWindow;
        private System.Windows.Forms.ComboBox drpViews2;
        private System.Windows.Forms.Label lblActView;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button btnExtProvs;
        private System.Windows.Forms.Timer tmr;
        private System.Windows.Forms.Button btnPrevDB;
        private System.Windows.Forms.ComboBox drpGrps;
        private System.Windows.Forms.Label lblGrps;
        private System.Windows.Forms.Button btnOptions;
    }
}

