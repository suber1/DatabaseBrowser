namespace RebusSQL6
{
    partial class frmChooseDB
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
            this.lstDBs = new System.Windows.Forms.ListBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnChgDesc = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblDB = new System.Windows.Forms.Label();
            this.lblConnStr = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lstDBs
            // 
            this.lstDBs.FormattingEnabled = true;
            this.lstDBs.ItemHeight = 17;
            this.lstDBs.Location = new System.Drawing.Point(12, 12);
            this.lstDBs.Name = "lstDBs";
            this.lstDBs.Size = new System.Drawing.Size(730, 174);
            this.lstDBs.TabIndex = 0;
            this.lstDBs.SelectedIndexChanged += new System.EventHandler(this.lstDBs_SelectedIndexChanged);
            this.lstDBs.DoubleClick += new System.EventHandler(this.lstDBs_DoubleClick);
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(460, 337);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(100, 26);
            this.btnOpen.TabIndex = 2;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(588, 337);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(154, 26);
            this.btnRemove.TabIndex = 3;
            this.btnRemove.Text = "Remove From List...";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnChgDesc
            // 
            this.btnChgDesc.Location = new System.Drawing.Point(12, 337);
            this.btnChgDesc.Name = "btnChgDesc";
            this.btnChgDesc.Size = new System.Drawing.Size(230, 26);
            this.btnChgDesc.TabIndex = 1;
            this.btnChgDesc.TabStop = false;
            this.btnChgDesc.Text = "Change Database Description...";
            this.btnChgDesc.UseVisualStyleBackColor = true;
            this.btnChgDesc.Click += new System.EventHandler(this.btnChgDesc_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(39, 215);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Database:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(21, 235);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(156, 52);
            this.label2.TabIndex = 5;
            this.label2.Text = "Optional manual connection string:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblDB
            // 
            this.lblDB.Location = new System.Drawing.Point(183, 215);
            this.lblDB.Name = "lblDB";
            this.lblDB.Size = new System.Drawing.Size(560, 20);
            this.lblDB.TabIndex = 6;
            this.lblDB.Text = "database";
            this.lblDB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblDB.DoubleClick += new System.EventHandler(this.lblDB_DoubleClick);
            // 
            // lblConnStr
            // 
            this.lblConnStr.Location = new System.Drawing.Point(183, 254);
            this.lblConnStr.Name = "lblConnStr";
            this.lblConnStr.Size = new System.Drawing.Size(560, 63);
            this.lblConnStr.TabIndex = 7;
            this.lblConnStr.Text = "connection string";
            // 
            // frmChooseDB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(755, 375);
            this.Controls.Add(this.lblConnStr);
            this.Controls.Add(this.lblDB);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnChgDesc);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.lstDBs);
            this.Font = new System.Drawing.Font("Lucida Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmChooseDB";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Choose Previously Opened Database to Open Now";
            this.Load += new System.EventHandler(this.frmChooseDB_Load);
            this.Shown += new System.EventHandler(this.frmChooseDB_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstDBs;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnChgDesc;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblDB;
        private System.Windows.Forms.Label lblConnStr;
    }
}