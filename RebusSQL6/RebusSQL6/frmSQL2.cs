using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using RebusData6;
using RebusTextFinder;

namespace RebusSQL6
{
    public struct ReportStyle
    {
        public string FontName;
        public float FontSize;
        public float FontSizeTitle;
        public string Title;
    }

    public struct ChildInfo
    {
        public string GUID;
        public List<string> Fields;
        public string OrderByClause;
    }

    public enum ExecutingSQL { NotCurrently, Synchronously, Asynchronously }

    public partial class frmSQL2 : RebusSQL6.frmBaseMDI
    {
        const string EXECUTE_CANCEL = "Cancel";

        const string ALLOWABLE_KEYWORD_AND_VARIABLE_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ_.@#0123456789";
        const string KEYWORDS = "AS/ASC/OR/ORDER/IN/INDEX/INNER/INSERT/IF/BY/TO/TOP/SET/AND/ADD/ON/SELECT/FROM/IS/INT/DATE/TEXT/NTEXT/WHERE/NOT/WITH/UNION/HAVING/EXISTS/NULL/DECLARE/UPDATE/LIKE/DELETE/VALUES/CREATE/ALTER/TABLE/COLUMN/DROP/DESC/GROUP/LEFT/RIGHT/JOIN/LEFT/*CAST/OUTER/DISTINCT/DATABASE/CASE/WHEN/THEN/BEGIN/END/ELSE/IIF/PIVOT/FOR/FOREIGN/EXEC/ROLLBACK/VIEW/USE/TRAN/PROCEDURE/FUNCTION/CLOSE/CONSTRAINT/CURSOR/PRIMARY/RETURN/FETCH/FLOAT/NVARCHAR/VARCHAR/UNIQUE/BIT/DATETIME/COUNT/RETURNS/AVG/SUM/MIN/MAX/END/GO/OF/OFF/INTO/WHILE/*DATEDIFF/*DATEADD/*GETDATE/*DATEPART/STDEV/STDEVP/*MONTH/*DAY/*YEAR/*LOWER/*LTRIM/*RTRIM/*UPPER/*REPLACE/*ROUND/*LEFT/*RIGHT/*SUBSTR/*LEN/*CONCAT/*ABS/*IIF";

        private int miDbID;
        public int DbID { get { return miDbID; } set { miDbID = value; } }

        private DB moDB;
        public DB DB { get { return moDB; } set { moDB = value; if (moDB != null) moDB.OnExecuteQueryAsyncComplete += AsyncCallback; } }
        public string Password { get; set; }

        public ExecutingSQL ExecutionState { get; internal set; }

        //public string ChildsParentTable = "";

        private DataTable moTbl;

        private string msSpecTable = "";
        private string msLastColmClicked = "";
        private string msDesc = "";
        private string msGrpID = "";
        private string msLastSQL = "";
        private bool mbDlgSaveOK = false;
        private bool mbAllowEdits = false;
        private int miCurrRowToPrint = -1;
        private ReportStyle mrReportStyle;

        private string msChildWhereClause = "";
        private string msLastChildWhereClause = "";

        private int miLastRow = -1;

        private string msCoreCaption = "";

        private TextFinder moFinder;

        private bool mbSQLDirty = false;

        //private bool mbTrace = false;

        private int miTopStyle = -1;
        string ExecBtnOrgCaption;
        Timer AsyncCallbackTimer = null;
        string ExecuteSQLChildOrderByClause = "";
        bool AsyncComplete = false;
        int AsyncAnimDots = 0;
        DateTime AsyncAnimLast;


        // colorizing editor
        bool EditorInQuote = false;
        bool EditorInComment = false;
        bool EditorInCommentSingleLine = false;
        bool EditorNeedReformatFromCurrentPostionToEnd = false;
        bool EditorColorizingOn = true;
        int EditorReformatFromThisPositionToEnd = -1;
        bool EditorProgrammaticChangeUnderway = false;
        bool EditorRecheckInQuote = false;
        bool EditorRecheckInComment = false;
        bool EditorInOverwriteMode = false;
        int EditorCurrentInQuoteStartPos = 0;
        int EditorCurrentInCommentStartPos = 0;
        bool EditorForceFontMatching = false;
        bool EditorKeyDown = false;
        Font EditorOrgFont;
        string QuoteChr = "'";
        List<Keyword> Keywords;
        Color RegularColor = Color.Black;
        Color InQuoteTextColor = Color.Red;
        Color CommentsColor = Color.Silver;
        Color VariableColor = Color.Green;
        Color KeywordColor = Color.DodgerBlue;
        Color FunctionColor = Color.HotPink;
        Color AtAtColor = Color.Purple;

        //
        // need to save this shit on the SAVE SQLs stuff, and use it for a RESTORE
        // 
        // needs to be a new MEMO field, structured properly to do this, and work in both OLD STYLE, and this NEW STYLE
        //
        // structure like so:
        //
        //      line 1: GUID
        //      line 2: MasterGUID
        //      line 3: comma-delimited master fields
        //      line 4..x: comma-delimited GUID, fld1, fldx,..
        //
        public string GUID { get; set; }

        // if a CHILD, this is set, else blank
        public string MasterGUID { get; set; }      // if non-blank, points to master which drives data for this child instance

        // for CHILD instances
        public List<string> MasterFields { get; set; }          // these are designed to LINE UP with corresponding ChildInfo.Fields[]

        // for MASTER instances
        public List<ChildInfo> ChildrenSqlWindows { get; set; }



        public frmSQL2()
        {
            InitializeComponent();
            ExecBtnOrgCaption = btnExec.Text;
            ExecutionState = ExecutingSQL.NotCurrently;
            EditorOrgFont = txtSQL.Font;
            this.GUID = null;
            this.MasterGUID = "";
            ChildrenSqlWindows = new List<ChildInfo>(0);
            MasterFields = new List<string>(0);
            moDB = null;
            moTbl = null;
            dlgSave.FileOk += dlgSave_FileOk;
            mrReportStyle.FontName = "Lucida Sans";
            mrReportStyle.FontSize = 9;
            mrReportStyle.FontSizeTitle = 12;
            mrReportStyle.Title = "";
        }

        public void SetSpecTable(string psTable)
        {
            msSpecTable = psTable;
        }

        public string GetSpecTable()
        {
            CheckSpecTable();
            return (msSpecTable);
        }

        public void UpdateFromParent(string psWhere, string psOrderByClause = "")
        {
            string xsWhere = psWhere.Trim();

            if (xsWhere.Length > 0)
            {
                msChildWhereClause = xsWhere;
                ExecuteSQL(psOrderByClause, true);
            }
        }

        public string CoreCaption()
        {
            return (msCoreCaption);
        }

        public void SetSQL(string psSQL, bool pbColorize = false)
        {
            bool xbOrg = EditorProgrammaticChangeUnderway;
            EditorProgrammaticChangeUnderway = true;
            txtSQL.Text = psSQL.Trim();
            EditorProgrammaticChangeUnderway = xbOrg;
            if (EditorColorizingOn && pbColorize) ColorizeSQL();
        }

        public void SetCaption(string psCaption = "")
        {
            string xsCaption = msCoreCaption;
            if (psCaption.Length > 0) xsCaption = psCaption;
            this.Text = xsCaption;
        }

        public string GetSQL()
        {
            return (txtSQL.Text.Trim());
        }

        private void LoadIndices(string psTable)
        {
            List<string> xoIdxs = moDB.GetIndices(psTable);
            if (moDB.Message.Length == 0)
            {
                moDB.ToGridFromList(this.grdIdxs, xoIdxs);
            }
            else
            {
                // need something here, but not a show message considering where we are (Shown event)
            }
        }
        
        private void LoadStructure(string psTable)
        {
            List<string> xoFlds = moDB.GetStructure(psTable);
            if (moDB.Message.Length == 0)
            {
                moDB.ToGridFromList(this.grdStruc, xoFlds);
            }
            else
            {
                // need something here, but not a show message considering where we are (Shown event)
            }
        }

        public void HideTableInfoTabs()
        {
            // this has no effect - strange
            //this.tabSQL.TabPages[2].Hide();
            //this.tabSQL.TabPages[3].Hide();

            //this.pgSQL.TabPages.Remove(pgIdxs);
            //this.pgSQL.TabPages.Remove(pgStruc);

            tabSQL.TabPages.Remove(pgIdxs);
            tabSQL.TabPages.Remove(pgStruc);
        }

        public void SetDesc(string psDesc)
        {
            msDesc = psDesc;
        }

        public string GetDesc()
        {
            return (msDesc.Trim());
        }

        public void SetGrpID(string psGrpID)
        {
            msGrpID = psGrpID;
        }

        public string GetGrpID()
        {
            return (msGrpID.Trim());
        }

        public void SetTable(string psTable, string psCaption = "", string psSQL = "", bool pbSkipExecute = false)
        {
            string xsTbl = psTable.Trim();
            if (xsTbl.Length > 1)
            {
                if (psTable.ToLower() == xsTbl.ToLower())
                {
                    xsTbl = xsTbl.Substring(0, 1).ToUpper() + xsTbl.Substring(1);
                }
            }
            msCoreCaption = xsTbl;
            SetCaption(psCaption);
            string xsSQL = "", xsTop = "";

            if (psSQL.Length == 0)
            {
                if (miTopStyle < 0)
                {
                    if (xsTbl.Length > 0)
                    {
                        miTopStyle = 0;
                        DataTable xoTbl = new DataTable();
                        if (moDB.SQL("SELECT TOP(1) * FROM " + xsTbl, xoTbl))
                        {
                            miTopStyle = 1;
                        }
                        else
                        {
                            if (moDB.SQL("SELECT TOP 1 * FROM " + xsTbl, xoTbl)) miTopStyle = 2;
                        }
                        xoTbl.Dispose();
                        xoTbl = null;
                    }
                }
                if (miTopStyle >= 1)
                {
                    if (Global.SelectTop <= 0)
                    {
                        //if (moDB.Connection.Provider.IsMsSQLServer) xsTop = "TOP(2500)";
                        //if (moDB.Connection.Provider.IsAccess) xsTop = "TOP 2500";
                        if (miTopStyle == 1) xsTop = "TOP(2500)";
                        if (miTopStyle == 2) xsTop = "TOP 2500";
                        object xo = moDB.Connection.Provider.TopPhrase;
                        if (xo != null)
                        {
                            string xs = xo.ToString().Trim();
                            if (xs.Length > 0) xsTop = xs;
                        }
                    }
                    else
                    {
                        //if (moDB.Connection.Provider.IsMsSQLServer) xsTop = "TOP(" + Global.SelectTop.ToString() + ")";
                        //if (moDB.Connection.Provider.IsAccess) xsTop = "TOP " + Global.SelectTop.ToString();
                        if (miTopStyle == 1) xsTop = "TOP(" + Global.SelectTop.ToString() + ")";
                        if (miTopStyle == 2) xsTop = "TOP " + Global.SelectTop.ToString();
                    }
                    xsTop = xsTop.Trim();
                    if (xsTop.Length > 0) xsTop = " " + xsTop;
                    xsSQL = "SELECT" + xsTop + " * FROM [" + xsTbl + "]";
                }
            }
            else
            {
                xsSQL = psSQL;
            }

            this.txtSQL.Text = xsSQL;
            toolStripMenuLockSQL.Checked = false;

            msSpecTable = psTable;

            LoadStructure(psTable);
            LoadIndices(psTable);

            if (!pbSkipExecute) ExecuteSQL();
        }

        public string SpecificTable()
        {
            CheckSpecTable();
            return (msSpecTable);
        }

        private void SetControlsAvails()
        {
            grd.ReadOnly = !mbAllowEdits;
            if (tabSQL.SelectedTab == pgData)
            {
                // on DATA tab
                if (!btnSave.Visible)
                {
                    btnSave.Visible = true;
                }

                btnSave.Enabled = (mbAllowEdits && (grd.IsCurrentCellDirty || grd.IsCurrentRowDirty));
            }
            else
            {
                // NOT on DATA tab
                if (btnSave.Visible) btnSave.Visible = false;
            }
            bool xb = (tabSQL.SelectedTab == pgSQL);
            btnZoomIn.Enabled = (xb & (txtSQL.ZoomFactor <= 5.5f));
            btnZoomOut.Enabled = (xb & (txtSQL.ZoomFactor >= 1.5f));
        }

        private void AsyncCallbackTimer_Tick(object sender, EventArgs e)
        {
            AsyncCallbackTimer.Enabled = false;
            if (AsyncComplete)
            {
                UpdateStatusBar("");
                btnExec.Enabled = false;
                btnExec.Text = ExecBtnOrgCaption;
                btnExec.Refresh();
                SQL_Executed(ExecuteSQLChildOrderByClause);
                ExecutionState = ExecutingSQL.NotCurrently;
                btnExec.Enabled = true;
            }
            else
            {
                DateTime xd = DateTime.Now;
                TimeSpan xoSpan = xd - AsyncAnimLast;
                double xnMsElapsed = xoSpan.TotalMilliseconds; // Math.Abs(xoSpan.Milliseconds) + (Math.Abs(xoSpan.Seconds) * 1000);
                if (xnMsElapsed >= 600d)
                {
                    AsyncAnimDots++;
                    if (AsyncAnimDots > 4) AsyncAnimDots = 0;
                    string xs = EXECUTE_CANCEL;
                    for (int xii = 1; xii <= AsyncAnimDots; xii++) { xs += " ."; }
                    btnExec.Text = xs;
                    btnExec.Refresh();
                    this.Refresh();
                    AsyncAnimLast = DateTime.Now;
                }
                else
                {
                    //this.Text = xd.ToString() + "    " + AsyncAnimLast.ToString() + "    " + xnMsElapsed.ToString() + "      x";
                    //this.Refresh();
                }
                AsyncCallbackTimer.Enabled = true;          // keep watching for completion
            }
        }

        private void AsyncCallback(DataTable poTable)
        {
            moTbl = poTable;
            AsyncComplete = true;
        }

        private void SQL_Executed(string psChildOrderByClause)
        {
            moDB.Connection.Binding = new BindingSource();
            moDB.Connection.Binding.DataSource = moTbl;

            this.grd.DataSource = moDB.Connection.Binding;
            this.grd.Refresh();

            tabSQL.SelectedTab = tabSQL.TabPages[1];

            RestoreColumnWidths();
            
            long xiRecs = moDB.LastNumberOfRecordsActioned;
            string xsMsg = xiRecs.ToString() + " record" + Global.iifs(xiRecs != 1, "s", "");
            if (msChildWhereClause.Length > 0) xsMsg = xsMsg + "  (WHERE " + msChildWhereClause + ")";
            if (psChildOrderByClause.Length > 0) xsMsg = xsMsg + "  (ORDER BY " + psChildOrderByClause + ")";
            UpdateStatusBar(xsMsg);
        }

        public void ExecuteSQL(string psChildOrderByClause = "", bool pbFromParent = false, bool pbAsynchronously = false, bool pbViaF5Key = false)
        {
            const string EXECUTE_SQL = "Execute SQL";
            bool xbSQLErr = false;

            if (ExecutionState != ExecutingSQL.NotCurrently)
            {
                if (ExecutionState == ExecutingSQL.Asynchronously)
                {
                    bool xbCancel = true;

                    if (pbViaF5Key)
                    {
                        if (Global.ShowMessage("Cancel current asynchronous SQL call?", EXECUTE_SQL, MessageBoxButtons.YesNo) == DialogResult.No) xbCancel = false;
                    }

                    if (xbCancel)
                    {
                        btnExec.Text = ExecBtnOrgCaption;
                        string xsMsg = "";
                        try
                        {
                            moDB.Connection.Command.Cancel();
                            xsMsg = "Asynchronous SQL call canceled.";
                        }
                        catch (Exception xoExc)
                        {
                            xsMsg = xoExc.Message;
                        }
                        Global.ShowMessage(xsMsg, EXECUTE_SQL);
                        ExecutionState = ExecutingSQL.NotCurrently;
                        UpdateStatusBar("");
                    }
                }
                else
                {
                    Global.ShowMessage("Currently executing SQL.  Try again later.", EXECUTE_SQL);
                }
                return;
            }

            if (pbAsynchronously)
            {
                ExecutionState = ExecutingSQL.Asynchronously;
                btnExec.Text = EXECUTE_CANCEL;
                UpdateStatusBar("Executing SQL asynchronously...");
            }
            else
            {
                ExecutionState = ExecutingSQL.Synchronously;
                btnExec.Text = "Executing...";
                btnExec.Enabled = false;
                UpdateStatusBar("Executing SQL...");
            }

            if (moTbl != null)
            {
                moTbl.Dispose();
                moTbl = null;
            }
            moTbl = new DataTable();

            Cursor xoOrgCursor = Cursor.Current;
            if (!pbAsynchronously) Cursor.Current = Cursors.WaitCursor;

            string xsSQL = txtSQL.Text;

            if (msChildWhereClause.Length > 0) xsSQL = xsSQL + " WHERE " + msChildWhereClause;
            if (psChildOrderByClause.Length > 0) xsSQL = xsSQL + " ORDER BY " + psChildOrderByClause;

            if (moDB.Connection.Provider.IsAccess)
            {
                // Jet does not allow comments, but we will allow TSQL comments, and remove before passing
                if (xsSQL.IndexOf("/*") >= 0 || xsSQL.IndexOf("--") >= 0)
                {
                    xsSQL = ParseOutComments(xsSQL);
                }
            }

            msLastSQL = xsSQL;

            if (pbAsynchronously)
            {
                if (moDB.Connection.Connectivity == ConnectivityType.DotNet)
                {
                    ExecuteSQLChildOrderByClause = psChildOrderByClause;
                    AsyncComplete = false;
                    AsyncAnimDots = 0;
                    
                    if (AsyncCallbackTimer == null)
                    {
                        AsyncCallbackTimer = new Timer();
                        AsyncCallbackTimer.Interval = 500;
                        AsyncCallbackTimer.Tick += new System.EventHandler(AsyncCallbackTimer_Tick);
                    }
                    
                    if (moDB.SQL(xsSQL, moTbl, true))
                    {
                        AsyncCallbackTimer.Enabled = true;      // watch for return
                        AsyncAnimLast = DateTime.Now;
                    }
                    else
                    {
                        xbSQLErr = true;
                    }
                }
                else
                {
                    Global.ShowMessage("Currently, only SQL connection type supports asynchronous calls.", EXECUTE_SQL);
                }
            }
            else
            {
                if (moDB.SQL(xsSQL, moTbl))
                {
                    SQL_Executed(psChildOrderByClause);
                    Cursor.Current = xoOrgCursor;
                }
                else
                {
                    Cursor.Current = xoOrgCursor;
                    xbSQLErr = true;
                }
            }

            if (xbSQLErr)
            {
                string xsMsg = moDB.Message + "  (" + xsSQL + ")";
                if (pbFromParent)
                {
                    UpdateStatusBar(xsMsg);
                }
                else
                {
                    UpdateStatusBar("");
                    Global.ShowMessage(xsMsg, EXECUTE_SQL);
                }
            }

            if (!pbAsynchronously)
            {
                btnExec.Text = ExecBtnOrgCaption;
                btnExec.Enabled = true;
                ExecutionState = ExecutingSQL.NotCurrently;
            }
            else
            {
                if (xbSQLErr) ExecutionState = ExecutingSQL.NotCurrently;
            }
        }

        public void FocusSQL()
        {
            txtSQL.Focus();
        }

        public void UpdateStatusBar(string psMsg)
        {
            statLbl.Text = psMsg;
            stat.Refresh();
        }

        private void RestoreColumnWidths()
        {
            string xsErrMsg = "";

            List<string> xsCols = new List<string>();
            List<int> xiWdts = new List<int>();
            for (int xii = 0; xii < grd.Columns.Count; xii++)
            {
                xsCols.Add(grd.Columns[xii].HeaderText.TrimEnd());
            }

            Global.RetrieveColumnWidths(xsCols, out xiWdts, out xsErrMsg);
            for (int xii = 0; xii < xiWdts.Count; xii++)
            {
                if (xiWdts[xii] > 0) grd.Columns[xii].Width = xiWdts[xii];
            }
        }

        private void SaveDataToCSV(string psOutFile)
        {
            const string xcsTitle = "Save Data to CSV";
            DB xoDB = new DB();
            if (xoDB.DumpTableToCSV(moTbl, psOutFile))
            {
                Global.ShowMessage("File " + psOutFile + " has been saved.", xcsTitle);
            }
            else
            {
                Global.ShowMessage(xoDB.Message, xcsTitle);
            }
            xoDB = null;
        }

        private void SaveDataToJSON(string psOutFile)
        {
            const string xcsTitle = "Save Data to JSON";
            DB xoDB = new DB();
            if (xoDB.DumpTableToJSON(moTbl, psOutFile))
            {
                Global.ShowMessage("File " + psOutFile + " has been saved.", xcsTitle);
            }
            else
            {
                Global.ShowMessage(xoDB.Message, xcsTitle);
            }
            xoDB = null;
        }

        // when popup added
        private void sendToBackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SendToBack();
        }

        private void frmSQL2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (AsyncCallbackTimer != null)
            {
                AsyncCallbackTimer.Dispose();
                AsyncCallbackTimer = null;
            }

            if (mbSQLDirty) mbSQLDirty = false;

            if (mbSQLDirty)
            {
                if (Global.ShowMessage("SQL has been changed without being saved.  Do you still want to close this window?", "Close SQL", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                    e.Cancel = true;
            }
            else
            {
                if (moFinder != null)
                {
                    moFinder = null;
                }

                if (moTbl != null)
                {
                    moTbl.Dispose();
                    moTbl = null;
                }
                if (moDB != null)
                {
                    if (moDB.DatabaseIsOpen()) moDB.CloseDatabase();
                    moDB = null;
                }
            }
        }

        private void frmSQL2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.F5)
            {
                if (e.Control)
                {
                    ExecuteSQL("", false, true, true);
                }
                else
                {
                    ExecuteSQL("", false, false, true);
                }
            }
            if (EditorKeyDown) ProcessEditorKey();
        }

        private void frmSQL2_Activated(object sender, EventArgs e)
        {
            frmMain xoMain = (frmMain)this.MdiParent;
            xoMain.ChildWindowFocused();
            xoMain = null;
        }

        private void frmSQL2_Load(object sender, EventArgs e)
        {
            txtSQL.Dock = DockStyle.Fill;
        }

        private void grd_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            string xsCaption = e.Column.HeaderText.Trim();
            int xiWidth = Convert.ToInt16(e.Column.Width);

            string xsErrMsg = "";
            Global.StoreColumnWidth(xsCaption, xiWidth, out xsErrMsg);
        }

        private void dlgSave_FileOk(object sender, CancelEventArgs e)
        {
            mbDlgSaveOK = true;
        }

        private void btnExec_Click(object sender, EventArgs e)
        {
            string xs = txtSQL.Text.Trim();
            if (xs.Length > 0)
            {
                ExecuteSQL();
            }
            else
            {
                Global.ShowMessage("Please enter SQL text to execute.", "Execute");
            }
        }

        private void exportToCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mbDlgSaveOK = false;
            dlgSave.DefaultExt = "txt";
            dlgSave.OverwritePrompt = true;
            dlgSave.Title = "Save Data to Tab-Delimited CSV File";
            dlgSave.RestoreDirectory = false;
            dlgSave.ShowDialog();
            if (mbDlgSaveOK) SaveDataToCSV(dlgSave.FileName);
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSQLprops xoFrm = new frmSQLprops();
            xoFrm.OK = false;
            xoFrm.txtCaption.Text = this.Text;
            xoFrm.txtDesc.Text = msDesc;
            xoFrm.txtGrpID.Text = msGrpID;
            xoFrm.ShowDialog();
            if (xoFrm.OK)
            {
                msCoreCaption = xoFrm.txtCaption.Text.Trim();
                SetCaption();
                msDesc = xoFrm.txtDesc.Text.Trim();
                msGrpID = xoFrm.txtGrpID.Text.Trim();
                frmMain xoParent = (frmMain)this.MdiParent;
                xoParent.RefreshChildWindowList();
                xoParent = null;
            }
            xoFrm.Dispose();
            xoFrm = null;
        }

        private void newViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMain xoFrm = (frmMain)this.MdiParent;
            xoFrm.CreateNewSQLWindow();
            xoFrm = null;
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintData();
        }

        private void enableEditingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mbAllowEdits = (!mbAllowEdits);     //toggle
            SetControlsAvails();
        }

        private void popupMenu_Opening(object sender, CancelEventArgs e)
        {
            //bool xbNotLocked = (MasterGUID.Length == 0);
            //bool xbChildless = (ChildrenSqlWindows.Count == 0);
            enableEditingToolStripMenuItem.Checked = mbAllowEdits;
            tsmColorizing.Checked = EditorColorizingOn;
            //enableEditingToolStripMenuItem.Enabled = xbNotLocked;
            //toolStripMenuLockSQL.Enabled = (xbNotLocked && xbChildless);
            tsmLinkChild.Enabled = (msLastColmClicked.Length > 0 && this.MasterGUID.Length == 0);
            tsmChooseChild.Enabled = msSpecTable.Trim().Length > 0;
            tstGetRubyStyleFKs.Enabled = moDB.Connection.Connectivity == ConnectivityType.DotNet;
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            if (txtSQL.ZoomFactor <= 5.5)
            {
                txtSQL.ZoomFactor = txtSQL.ZoomFactor + 0.5f;
                SetControlsAvails();
            }
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            if (txtSQL.ZoomFactor >= 1.5f)
            {
                txtSQL.ZoomFactor = txtSQL.ZoomFactor - 0.5f;
                SetControlsAvails();
            }
        }

        private void tabSQL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetControlsAvails();
        }

        private void txtSQL_KeyDown(object sender, KeyEventArgs e)
        {
            if (moFinder == null) moFinder = new TextFinder();
            moFinder.KeyDown(sender, e);
            string xsMsg = moFinder.GetMessage();
            if (xsMsg.Length > 0) Global.ShowMessage(xsMsg);

            if (EditorColorizingOn)
            {
                EditorKeyDown = true;
                if (txtSQL.SelectionLength > 0)
                {
                    if (e.Control)
                    {
                        if (e.KeyCode == Keys.C)
                        {
                            // copy - do nothing here
                            //this.Text = e.KeyCode.ToString() + "; " + e.KeyValue.ToString();
                            EditorReformatFromThisPositionToEnd = -1;
                        }
                        else
                        {
                            //this.Text = e.KeyCode.ToString() + ", " + e.KeyValue.ToString();
                            EditorReformatFromThisPositionToEnd = 0;        // go brute
                        }
                    }
                    else
                    {
                        EditorReformatFromThisPositionToEnd = 0;        // go brute
                    }
                }
                else
                {
                    if (e.KeyCode == Keys.Delete)
                    {
                        if (txtSQL.SelectionStart < txtSQL.Text.Length)
                        {
                            string xsCharToRight = txtSQL.Text.Substring(txtSQL.SelectionStart, 1);
                            if (xsCharToRight == QuoteChr)
                            {
                                EditorRecheckInQuote = true;
                            }
                            else
                            {
                                if (xsCharToRight == "*" || xsCharToRight == "/")
                                {
                                    EditorRecheckInComment = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (e.KeyCode == Keys.Back)
                        {
                            if (txtSQL.SelectionStart > 0)
                            {
                                string xsCharToLeft = txtSQL.Text.Substring(txtSQL.SelectionStart - 1, 1);
                                if (xsCharToLeft == QuoteChr)
                                {
                                    EditorRecheckInQuote = true;
                                }
                                else
                                {
                                    if (xsCharToLeft == "*" || xsCharToLeft == "/")
                                    {
                                        EditorRecheckInComment = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (e.KeyCode == Keys.OemQuotes)
                            {
                                EditorRecheckInQuote = true;
                            }
                            else
                            {
                                if ((e.KeyCode == Keys.Divide || (e.KeyValue == 191 && !e.Shift)) || (e.KeyCode == Keys.Multiply || (e.KeyValue == 56 && e.Shift)) || (e.KeyCode == Keys.OemMinus))
                                {
                                    EditorRecheckInComment = true;
                                }
                                else
                                {
                                    if (e.KeyCode == Keys.Home || e.KeyCode == Keys.End || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Down || e.KeyCode == Keys.Up || e.KeyCode == Keys.PageDown || e.KeyCode == Keys.PageUp)
                                    {
                                        EditorRecheckInQuote = true;
                                        EditorRecheckInComment = true;
                                    }
                                    else
                                    {
                                        if (e.KeyCode == Keys.Insert)
                                        {
                                            EditorInOverwriteMode = !EditorInOverwriteMode;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (e.Control)
                    {
                        if (e.KeyCode == Keys.V)
                        {
                            // "paste" about to occur
                            EditorReformatFromThisPositionToEnd = 0;
                        }
                        else
                        {
                            if (e.KeyCode == Keys.X)
                            {
                                // "cut" about to occur
                                EditorReformatFromThisPositionToEnd = 0;
                            }
                            else
                            {
                                if (e.KeyCode == Keys.Delete)
                                {
                                    // "delete to end" about to occur
                                }
                                else
                                {
                                    if (e.KeyCode == Keys.Back)
                                    {
                                        // delete prior char or delete selected text about to occur
                                    }
                                    else
                                    {
                                        if (e.KeyCode == Keys.F9) EditorReformatFromThisPositionToEnd = 0;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (e.KeyCode == Keys.Delete)
                        {
                            //
                            // DELETE
                            //
                            if (!e.Alt && !e.Shift)
                            {
                                // char to right or selected text about to be deleted
                                if (txtSQL.SelectionStart < txtSQL.Text.Length)
                                {
                                    //
                                    // char to right about to be deleted
                                    //
                                    string xsCharToRight = txtSQL.Text.Substring(txtSQL.SelectionStart, 1);
                                    if (xsCharToRight == QuoteChr)
                                    {
                                        if (!EditorInComment)
                                        {
                                            //Log("quote char about to be deleted");
                                            //
                                            // if cursor not currently inside quoted text, then
                                            // the text following the qoute about to be deleted
                                            // is the start of quoted text, so upon the removal
                                            // of the quote character, we'll need to re-color
                                            // the formerly quoted text which is currently in
                                            // quoted text color
                                            //
                                            EditorNeedReformatFromCurrentPostionToEnd = true;
                                            //EditorInQuote = !EditorInQuote;     // toggle
                                        }
                                    }
                                    else
                                    {
                                        //if (!EditorInComment)
                                        {
                                            if (xsCharToRight == "/" || xsCharToRight == "*" || xsCharToRight == "-")       // possible comment start?
                                            {
                                                if (EditorInComment)
                                                {
                                                    EditorReformatFromThisPositionToEnd = EditorCurrentInCommentStartPos;
                                                }
                                                else
                                                {
                                                    EditorNeedReformatFromCurrentPostionToEnd = true;
                                                    //EditorReformatFromThisPositionToEnd = EditorCurrentInCommentStartPos;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (e.KeyCode == Keys.OemQuotes)
                            {
                                //
                                // QUOTE (apostrophe)
                                //
                                if (!EditorInComment)
                                {
                                    if (!e.Alt && !e.Shift)
                                    {
                                        if (txtSQL.SelectionStart == txtSQL.Text.Length)
                                        {

                                        }
                                        else
                                        {
                                            EditorNeedReformatFromCurrentPostionToEnd = true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (e.KeyCode == Keys.Back)
                                {
                                    //
                                    // BACKSPACE
                                    //
                                    if (txtSQL.SelectionStart > 0)
                                    {
                                        // char to left about to be deleted
                                        string xsPrevChr = txtSQL.Text.Substring(txtSQL.SelectionStart - 1, 1);
                                        if (xsPrevChr == QuoteChr)
                                        {
                                            if (!EditorInComment)
                                            {
                                                //Log("quote char about to be backspaced over");
                                                //
                                                // if cursor not currently inside quoted text, then
                                                // the text following the qoute about to be deleted
                                                // is the start of quoted text, so upon the removal
                                                // of the quote character, we'll need to re-color
                                                // the formerly quoted text which is currently in
                                                // quoted text color
                                                //
                                                EditorNeedReformatFromCurrentPostionToEnd = true;
                                            }
                                        }
                                        else
                                        {
                                            if (!EditorInQuote)
                                            {
                                                if (EditorInComment)
                                                {
                                                    if (xsPrevChr == "-" || xsPrevChr == "*" || xsPrevChr == "/")
                                                    {
                                                        EditorReformatFromThisPositionToEnd = EditorCurrentInCommentStartPos;
                                                    }
                                                }
                                                else
                                                {
                                                    if (xsPrevChr == "/")           // char about to be deleted possible end of comment?
                                                    {
                                                        if (txtSQL.SelectionStart > 1 && txtSQL.Text.Substring(txtSQL.SelectionStart - 2, 1) == "*")
                                                        {
                                                            // force reformat from beginning of comment
                                                            int xiPos = txtSQL.SelectionStart - 2;
                                                            while (xiPos > 0)
                                                            {
                                                                string xs = txtSQL.Text.Substring(xiPos, 1);
                                                                if (xs == "*" && xiPos > 0 && txtSQL.Text.Substring(xiPos - 1, 1) == "/")
                                                                {
                                                                    xiPos--;
                                                                    break;
                                                                }
                                                                xiPos--;
                                                            }
                                                            EditorReformatFromThisPositionToEnd = xiPos;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    //this.Text = e.KeyValue.ToString() + ", " + e.Shift.ToString();
                                    if (!EditorInQuote)
                                    {
                                        if ((e.KeyCode == Keys.Divide || (e.KeyValue == 191 && !e.Shift)) || (e.KeyCode == Keys.Multiply || (e.KeyValue == 56 && e.Shift)) || (e.KeyCode == Keys.OemMinus))
                                        {
                                            if (txtSQL.SelectionStart == txtSQL.Text.Length)
                                            {

                                            }
                                            else
                                            {
                                                if (e.KeyCode == Keys.OemMinus)
                                                {
                                                    if (txtSQL.Text.Substring(txtSQL.SelectionStart + 0, 1) == "-")       // about to insert a "-" in front of current one?
                                                    {
                                                        EditorReformatFromThisPositionToEnd = txtSQL.SelectionStart - 0;
                                                        //Log("To reformat from position " + EditorReformatFromThisPositionToEnd.ToString());
                                                    }
                                                    else
                                                    {
                                                        if (!EditorInComment && txtSQL.SelectionStart > 0 && txtSQL.Text.Substring(txtSQL.SelectionStart - 1, 1) == "-")
                                                        {
                                                            EditorReformatFromThisPositionToEnd = txtSQL.SelectionStart - 1;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    int xiCmtSt, xiCmtLen;
                                                    bool xbInline, xbOth;
                                                    EditorInComment = IsCurrentCursorPositionInCommentOrQuote(true, out xiCmtSt, out xiCmtLen, out xbInline, out xbOth, txtSQL.SelectionStart);
                                                    if (EditorInComment) EditorInQuote = false;

                                                    if (e.KeyCode == Keys.Divide || (e.KeyValue == 191 && !e.Shift))
                                                    {
                                                        if (!EditorInComment && txtSQL.Text.Substring(txtSQL.SelectionStart, 1) == "*")       // about to insert a "/" in front of "*"?
                                                        {
                                                            EditorReformatFromThisPositionToEnd = txtSQL.SelectionStart + 1;
                                                            //Log("To reformat from position " + EditorReformatFromThisPositionToEnd.ToString());
                                                        }
                                                        else
                                                        {
                                                            if (EditorInComment && txtSQL.SelectionStart > 1 && txtSQL.Text.Substring(txtSQL.SelectionStart - 1, 1) == "*")
                                                            {

                                                                EditorReformatFromThisPositionToEnd = xiCmtSt;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (e.KeyCode == Keys.Multiply || (e.KeyValue == 56 && e.Shift))
                                                        {
                                                            if (!EditorInComment && txtSQL.SelectionStart > 0 && txtSQL.Text.Substring(txtSQL.SelectionStart - 1, 1) == "/")
                                                            {
                                                                EditorReformatFromThisPositionToEnd = txtSQL.SelectionStart - 1;
                                                            }
                                                            else
                                                            {
                                                                if (EditorInComment && txtSQL.SelectionStart < txtSQL.Text.Length && txtSQL.Text.Substring(txtSQL.SelectionStart + 0, 1) == "/")
                                                                {
                                                                    EditorReformatFromThisPositionToEnd = xiCmtSt;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (EditorInComment && e.KeyCode == Keys.Enter)
                                            {
                                                EditorNeedReformatFromCurrentPostionToEnd = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private string ParseOutComments(string psSQL)
        {
            string xsSQL = psSQL.Trim(), xsSQLout = "";
            bool xbAppend = false;
            bool xbInCommentTypeSlashStar = false;
            bool xbInCommentTypeSlashStarMaybe = false;
            bool xbInCommentTypeDashDash = false;
            char xcLastQtChar = '\0', xcPrevChar = '\0';
            bool xbInQt = false, xbInComment = false;

            xsSQL = xsSQL.Replace("\r\n", "\n");

            foreach (Char xcChr in xsSQL)
            {
                if (xbInQt)
                {
                    // just need to see if we're going "out of quote"
                    if (xcChr == xcLastQtChar)
                    {
                        xcLastQtChar = xcChr;
                        xbInQt = false;
                    }
                    // everything gets appended when inside a quote, so we don't look for special char(s) that begin comments
                    xsSQLout += xcChr;
                }
                else
                {
                    // let's see if we're going "into quote"
                    if (xbInComment)
                    {
                        ///
                        // let's see if the comment is ending
                        //
                        if (xbInCommentTypeSlashStar)
                        {
                            if (xcChr.Equals('/'))
                            {
                                if (xcPrevChar.Equals('*'))
                                {
                                    xbInComment = false;
                                    xbInCommentTypeSlashStar = false;
                                }
                            }
                        }
                        else
                        {
                            if (xbInCommentTypeDashDash)
                            {
                                if (xcChr == '\r' || xcChr == '\n')
                                {
                                    xbInComment = false;
                                    xbInCommentTypeDashDash = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        // we're not INCOMMENT or INQUOTE, so take action accordingly
                        xbAppend = true;
                        if (xcChr.Equals('\''))
                        {
                            xcLastQtChar = xcChr;
                            xbInQt = true;
                        }
                        else
                        {
                            if (xcChr.Equals('"'))
                            {
                                xcLastQtChar = xcChr;
                                xbInQt = true;
                            }
                            else
                            {
                                if (xcChr == '/')
                                {
                                    // are we ending commented text?
                                    if (xbInCommentTypeSlashStarMaybe)
                                    {
                                        if (xcPrevChar == '*')
                                        {
                                            // we are closing a comment...
                                            xbInComment = false;
                                            xbInCommentTypeSlashStarMaybe = false;
                                            xsSQLout = xsSQLout.Substring(0, xsSQLout.Length - 1);  // drop the previous "/"
                                            xbAppend = false;
                                            xbInCommentTypeSlashStarMaybe = false;
                                        }
                                        else
                                        {
                                            xbInCommentTypeSlashStarMaybe = false;
                                        }
                                    }
                                    else
                                    {
                                        xbInCommentTypeSlashStarMaybe = true;
                                    }
                                }
                                else
                                {
                                    if (xcChr == '*')
                                    {
                                        // are we starting block commented text?
                                        if (xcPrevChar == '/')
                                        {
                                            xbInComment = true;
                                            xsSQLout = xsSQLout.Substring(0, xsSQLout.Length - 1);  // drop the previous "/"
                                            xbAppend = false;
                                            xbInCommentTypeSlashStar = true;
                                            xbInCommentTypeSlashStarMaybe = false;
                                        }
                                    }
                                    else
                                    {
                                        if (xcChr == '-')
                                        {
                                            // are we starting single/in line comment text?
                                            if (xcPrevChar == '-')
                                            {
                                                xbInComment = true;
                                                xsSQLout = xsSQLout.Substring(0, xsSQLout.Length - 1);  // drop the previous "-"
                                                xbAppend = false;
                                                xbInCommentTypeDashDash = true;
                                                //xbInCommentTypeDashDashMaybe = false;
                                            }
                                        }
                                        else
                                        {

                                        }
                                    }
                                }
                            }
                        }
                        if (xbAppend) xsSQLout += xcChr;
                    }
                }
                xcPrevChar = xcChr;
            }

            
            return (xsSQLout);
        }

        private void frmSQL2_DoubleClick(object sender, EventArgs e)
        {
            string xs = "";
            if (MasterFields.Count > 0) xs = this.MasterFields[0];
            statLbl.Text = this.GUID + ", " + this.MasterGUID + ", " + xs;
        }

        private void panel2_DoubleClick(object sender, EventArgs e)
        {
            frmSQL2_DoubleClick(sender, e);
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            Global.ShowMessage(msLastSQL, "Last SQL");
        }

        public void ColorizeSQL()   //out bool pbIsActionSQL)
        {
            //bool xbIsActionSQL = false;
            EditorReformatFromPosition(0);
            /*bool xbInCommentTypeSlashStar = false;
            bool xbInCommentTypeSlashStarMaybe = false;
            bool xbInCommentTypeDashDash = false;
            bool xbKeyWordChar = false;
            string xsKeyWord = "";
            int xiPos = -1, xiCommentStartPos = -1, xiQtStartPos = -1, xiKeyWordStartPos = -1;
            int xiCursorPos;
            char xcLastQtChar = '\0', xcPrevChar = '\0', xcChr;
            bool xbInQt = false, xbInComment = false;

            for (xiPos = 0; xiPos < txtSQL.Text.Length; xiPos++)
            {
                xiCursorPos = txtSQL.SelectionStart;
                xcChr = txtSQL.Text[xiPos];
                if (xbInQt)
                {
                    // just need to see if we're going "out of quote"
                    if (xcChr == xcLastQtChar)
                    {
                        xcLastQtChar = xcChr;
                        xbInQt = false;
                        txtSQL.Select(xiQtStartPos + 1, xiPos - xiQtStartPos - 1);
                        txtSQL.SelectionColor = InQuoteTextColor;
                        txtSQL.Select(0, 0);
                    }
                }
                else
                {
                    // let's see if we're going "into quote"
                    if (xbInComment)
                    {
                        //
                        // no...check for quote...
                        //
                        // let's see if the comment is ending
                        //
                        if (xbInCommentTypeSlashStar)
                        {
                            if (xcChr.Equals('/'))
                            {
                                if (xcPrevChar.Equals('*'))
                                {
                                    xbInComment = false;
                                    xbInCommentTypeSlashStar = false;
                                    txtSQL.Select(xiCommentStartPos, xiPos - xiCommentStartPos + 1);
                                    txtSQL.SelectionColor = CommentsColor;
                                    txtSQL.Select(0, 0);
                                }
                            }
                        }
                        else
                        {
                            if (xbInCommentTypeDashDash)
                            {
                                if (xcChr == '\r' || xcChr == '\n')
                                {
                                    xbInComment = false;
                                    xbInCommentTypeDashDash = false;
                                    txtSQL.Select(xiCommentStartPos, xiPos - xiCommentStartPos + 1);
                                    txtSQL.SelectionColor = CommentsColor;
                                    txtSQL.Select(0, 0);
                                }
                            }
                        }
                    }
                    else
                    {
                        //
                        // we're not INCOMMENT or INQUOTE, so take action accordingly
                        //
                        xbKeyWordChar = false;
                        if ((xcChr >= 'A' && xcChr <= 'Z') || (xcChr == '_' || xcChr == '-'))
                        {
                            xbKeyWordChar = true;
                        }
                        else
                        {
                            if ((xcChr >= 'a' && xcChr <= 'z') || (xcChr == '_' || xcChr == '-'))
                            {
                                xbKeyWordChar = true;
                            }
                            else
                            {
                                if (xcChr == '@')
                                {
                                    xbKeyWordChar = true;
                                }
                                else
                                {
                                    if (xcChr == '(')
                                    {
                                        if (xsKeyWord.Length > 0)
                                        {
                                            txtSQL.Select(xiKeyWordStartPos, xiPos - xiKeyWordStartPos + 1);
                                            txtSQL.SelectionColor = KeywordColor;
                                            txtSQL.Select(0, 0);
                                            txtSQL.SelectionStart = xiCursorPos;
                                            xiKeyWordStartPos = -1;
                                            xsKeyWord = "";
                                        }
                                    }
                                }
                            }
                        }
                        if (xbKeyWordChar)
                        {
                            if (xiKeyWordStartPos < 0) xiKeyWordStartPos = xiPos;
                            xsKeyWord += xcChr;
                        }
                        else
                        {
                            //
                            // let's check for end-of-word
                            //
                            //if (xcChr == "\t" || xcChr = '\r' || xcChr || '\n' || xcChr = ' ')
                            if (xsKeyWord.Length > 0)
                            {
                                if (IsKeyWord(xsKeyWord))
                                {
                                    if (!xbIsActionSQL) xbIsActionSQL = IsActionKeyWord(xsKeyWord);
                                    txtSQL.Select(xiKeyWordStartPos, xiPos - xiKeyWordStartPos + 1);
                                    txtSQL.SelectionColor = KeywordColor;
                                    txtSQL.Select(0, 0);
                                    txtSQL.SelectionStart = xiCursorPos;
                                    xiKeyWordStartPos = -1;
                                    xsKeyWord = "";
                                }
                                else
                                {
                                    if (xsKeyWord.Substring(0, 1) == "@")
                                    {
                                        txtSQL.Select(xiKeyWordStartPos, xiPos - xiKeyWordStartPos + 1);
                                        txtSQL.SelectionColor = VariableColor;
                                        txtSQL.Select(0, 0);
                                        txtSQL.SelectionStart = xiCursorPos;
                                        xiKeyWordStartPos = -1;
                                        xsKeyWord = "";
                                    }
                                    else
                                    {
                                        txtSQL.Select(xiKeyWordStartPos, xiPos - xiKeyWordStartPos + 1);
                                        txtSQL.SelectionColor = RegularColor;
                                        txtSQL.Select(0, 0);
                                        txtSQL.SelectionStart = xiCursorPos;
                                        xsKeyWord = "";
                                        xiKeyWordStartPos = -1;
                                    }
                                }
                            }
                            else
                            {
                                xiKeyWordStartPos = -1;
                            }
                        }
                        if (!xbKeyWordChar)
                        {
                            xiKeyWordStartPos = -1;
                            if (xcChr.Equals('\''))
                            {
                                xcLastQtChar = xcChr;
                                xbInQt = true;
                                xiQtStartPos = xiPos;
                            }
                            else
                            {
                                if (xcChr.Equals('"'))
                                {
                                    xcLastQtChar = xcChr;
                                    xbInQt = true;
                                    xiQtStartPos = xiPos;
                                }
                                else
                                {
                                    if (xcChr == '/')
                                    {
                                        // are we ending commented text?
                                        if (xbInCommentTypeSlashStarMaybe)
                                        {
                                            if (xcPrevChar == '*')
                                            {
                                                // we are closing a comment...
                                                xbInComment = false;
                                                xbInCommentTypeSlashStarMaybe = false;
                                                //xsSQLout = xsSQLout.Substring(0, xsSQLout.Length - 1);  // drop the previous "/"
                                                xbInCommentTypeSlashStarMaybe = false;
                                            }
                                            else
                                            {
                                                xbInCommentTypeSlashStarMaybe = false;
                                            }
                                        }
                                        else
                                        {
                                            xbInCommentTypeSlashStarMaybe = true;
                                        }
                                    }
                                    else
                                    {
                                        if (xcChr == '*')
                                        {
                                            // are we starting block commented text?
                                            if (xcPrevChar == '/')
                                            {
                                                xbInComment = true;
                                                //xsSQLout = xsSQLout.Substring(0, xsSQLout.Length - 1);  // drop the previous "/"
                                                xiCommentStartPos = xiPos - 1;
                                                xbInCommentTypeSlashStar = true;
                                                xbInCommentTypeSlashStarMaybe = false;
                                            }
                                        }
                                        else
                                        {
                                            if (xcChr == '-')
                                            {
                                                // are we starting single/in line comment text?
                                                if (xcPrevChar == '-')
                                                {
                                                    xbInComment = true;
                                                    xiCommentStartPos = xiPos - 1;
                                                    //xsSQLout = xsSQLout.Substring(0, xsSQLout.Length - 1);  // drop the previous "-"
                                                    xbInCommentTypeDashDash = true;
                                                }
                                            }
                                            else
                                            {

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                xcPrevChar = xcChr;
            }*/

            //pbIsActionSQL = xbIsActionSQL;
            return;
        }

        private bool IsActionKeyWord(string psWord)
        {
            bool xbIs = false;
            const string xcsActionKeywords = "/INSERT/UPDATE/DELETE/CREATE/ALTER/EXEC/ROLLBACK/GO/DROP/      ";

            string xsWord = psWord.ToUpper();

            int xi = xcsActionKeywords.IndexOf(xsWord);
            if (xi >= 0)
            {
                string xsKeyWord = xcsActionKeywords.Substring(xi, 10);
                xi = xsKeyWord.IndexOf("/");
                xsKeyWord = xsKeyWord.Substring(0, xi);
                xbIs = xsWord == xsKeyWord;
            }

            return (xbIs);
        }

        private bool IsKeyWord(string psWord)
        {
            bool xb = false;
            string xsWord = psWord.ToUpper();

            int xi = KEYWORDS.IndexOf(xsWord);
            if (xi >= 0)
            {
                string xsKeyWord = KEYWORDS.Substring(xi, 10);
                xi = xsKeyWord.IndexOf("/");
                xsKeyWord = xsKeyWord.Substring(0, xi);
                xb = xsWord == xsKeyWord;
            }
            return (xb);
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            //bool xb = false;
            ColorizeSQL();      //out xb);
        }

        public bool IsLocked()
        {
            return (txtSQL.ReadOnly);
        }

        public void LockSQL(bool pb)
        {
            if (pb != toolStripMenuLockSQL.Checked) toolStripMenuLockSQL.Checked = pb;
        }

        private void toolStripMenuLockSQL_Click(object sender, EventArgs e)
        {
            toolStripMenuLockSQL.Checked = !toolStripMenuLockSQL.Checked;
        }

        private void toolStripMenuLockSQL_CheckedChanged(object sender, EventArgs e)
        {
            txtSQL.ReadOnly = toolStripMenuLockSQL.Checked;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string xsErrMsg = "";
            try
            {
                moDB.Update(moTbl);
            }
            catch (Exception xoExc)
            {
                xsErrMsg = xoExc.Message;
            }
            if (xsErrMsg.Length > 0)
            {
                Global.ShowMessage(xsErrMsg, "Save Changes");
            }
            SetControlsAvails();
        }

        private void grd_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            SetControlsAvails();
        }

        private void frmSQL2_Shown(object sender, EventArgs e)
        {
            SetControlsAvails();
            if (EditorColorizingOn) ColorizeSQL();
            //statLbl.Text = "Right-click for more info";
        }

        private string GetParentTable()
        {
            // get parent table of this child window (if it is a child)
            string xsParentTable = "";
            frmMain xoParent = (frmMain)this.MdiParent;

            if (this.MasterGUID.Length > 0)
            {
                foreach (Form xoForm in xoParent.MdiChildren)
                {
                    bool xbSQLForm = true;
                    frmSQL2 xoSQL = null;
                    try
                    {
                        xoSQL = (frmSQL2)xoForm;
                    }
                    catch
                    {
                        xbSQLForm = false;
                    }
                    if (xbSQLForm)
                    {
                        if (xoSQL.GUID == this.MasterGUID)
                        {
                            xsParentTable = xoSQL.GetSpecTable();
                            break;
                        }
                    }
                    if (xoSQL != null) xoSQL = null;
                }
            }

            xoParent = null;

            return (xsParentTable);
        }

        private void CheckSpecTable()
        {
            if (msSpecTable.Trim().Length == 0)
            {
                string xsSQL = txtSQL.Text.Trim();
                xsSQL = xsSQL.Replace("\t", " ");
                int xi = xsSQL.ToLower().IndexOf(" from ");
                if (xi > 0)
                {
                    string xs = xsSQL.Substring(xi + 5).Trim();
                    xi = xs.IndexOf(" ");
                    if (xi > 0)
                    {
                        msSpecTable = xs.Substring(0, xi).Trim();
                    }
                    else
                    {
                        msSpecTable = xs;
                    }
                }
            }
        }

        private void toolStripMenuItemChildSQL_Click(object sender, EventArgs e)
        {
            bool xbRefresh = false;

            frmChildDlg xoDlg = new frmChildDlg(moDB, this.DbID);
            CheckSpecTable();
            xoDlg.ParentTable = msSpecTable;
            //xoDlg.ChildsParentTable = "";
            if (this.MasterGUID.Length > 0)
            {
                // this table is a child, let other's know its parent
                //xoDlg.ChildsParentTable = this.ChildsParentTable;
                // find parent table, and get it's msSpecTable
                xoDlg.ParentTable = GetParentTable();
            }
            List<DataField> xrMasterFlds = new List<DataField>(0);
            for (int xii = 0; xii < grd.Columns.Count; xii++)
            {
                DataField xrFld = new DataField();
                xrFld.Name = grd.Columns[xii].Name;
                xrFld.Type = grd.Columns[xii].ValueType;
                xrMasterFlds.Add(xrFld);
            }
            xoDlg.AvailableMasterFields = xrMasterFlds;
            xoDlg.ShowDialog();

            if (xoDlg.OK())
            {
                frmMain xoFrm = (frmMain)this.MdiParent;
                frmSQL2 xoSQL = xoFrm.CreateNewSQLWindow();
                xoFrm = null;
                //xoSQL.ChildsParentTable = msSpecTable;
                string xsChildTable = xoDlg.ChildTable();
                xoSQL.SetSpecTable(xsChildTable);
                List<string> xsFlds = xoDlg.ShowFields();
                string xsSQL = "SELECT ";
                for (int xii = 0; xii < xsFlds.Count; xii++)
                {
                    if (xii > 0) xsSQL = xsSQL + ", ";
                    xsSQL = xsSQL + xsFlds[xii];
                }
                xsSQL = xsSQL + " FROM " + xoDlg.ChildTable();
                xoSQL.SetSQL(xsSQL);
                this.LockSQL(true);
                xoSQL.LockSQL(true);
                xoSQL.Text = Proper(xoDlg.ChildTable()) + " < " + Proper(this.Text);

                if (xoSQL != null)
                {
                    // update this instance (MASTER)
                    ChildInfo xrChild = new ChildInfo();
                    string xsOrderByClause = xoDlg.OrderByClause();
                    xsFlds = new List<string>(0);
                    xrChild.GUID = xoSQL.GUID;
                    xrChild.Fields = xsFlds;
                    xrChild.OrderByClause = xsOrderByClause;
                    xsFlds.Add(xoDlg.ChildField());
                    ChildrenSqlWindows.Add(xrChild);

                    // update CHILD instance
                    xoSQL.MasterGUID = this.GUID;
                    xsFlds = new List<string>(0);
                    xsFlds.Add(xoDlg.MasterField());
                    xoSQL.MasterFields = xsFlds;
                    //xoSQL.SetChildOrderByClause(xsOrderByClause);
                    xbRefresh = true;
                }
            }
            xoDlg.Close();
            xoDlg.Dispose();
            xoDlg = null;

            if (xbRefresh) UpdateChildrenForNewRow(true);
        }

        private string Proper(string ps)
        {
            string xsReturn = ps;

            if (ps.Length > 1 && ps.ToLower() == ps)
            {
                xsReturn = ps.Substring(0, 1).ToUpper() + ps.Substring(1);
            }

            return (xsReturn);
        }

        private void stat_DoubleClick(object sender, EventArgs e)
        {
            Global.ShowMessage(statLbl.Text, "Current Status Bar Message");

            //statLbl.Text = this.GUID;
            //grd.Columns[3].DefaultCellStyle.Format = "G";

            //var x = grd.Columns[3].ValueType;
            //string xs = x.ToString();
            //x = null;

        }

        private void SetGridDateTimeColumnsToLongFormat()
        {
            string xsErr = "";
            SetGridDateTimeColumnsFormat("G", out xsErr);      // "G"eneral
        }

        private bool SetGridDateTimeColumnsFormat(string psFormatString, out string psErrMsg)
        {
            string xsErrMsg = "";
            try
            {
                for (int xii = 0; xii < grd.Columns.Count; xii++)
                {
                    if (grd.Columns[xii].ValueType.ToString() == "System.DateTime")
                    {
                        grd.Columns[xii].DefaultCellStyle.Format = psFormatString;
                    }
                }
            }
            catch (Exception xoExc)
            {
                xsErrMsg = xoExc.Message;
            }

            psErrMsg = xsErrMsg;
            return (xsErrMsg.Length == 0);
        }

        public void UpdateChildrenForNewRow(bool pbForce = false)
        {
            int xiCurrRow = -1;
            try
            {
                xiCurrRow = grd.CurrentRow.Index;
            }
            catch { }
            bool xbSkip = (xiCurrRow == miLastRow  || xiCurrRow < 0);
            if (xbSkip)
            {
                if (pbForce)
                {
                    xbSkip = false;
                }
                else
                {
                    if (msLastChildWhereClause != msChildWhereClause) xbSkip = false;
                }
            }

            if (ChildrenSqlWindows.Count > 0 && (!xbSkip))       // is this instance a master (has one or more children)?
            {
                frmMain xoParent = (frmMain)this.MdiParent;
                foreach (Form xoForm in xoParent.MdiChildren)
                {
                    bool xbSQLForm = true;
                    frmSQL2 xoSQL = null;
                    try
                    {
                        xoSQL = (frmSQL2)xoForm;
                    }
                    catch
                    {
                        xbSQLForm = false;
                    }
                    if (xbSQLForm)
                    {
                        for (int xiChild = 0; xiChild < ChildrenSqlWindows.Count; xiChild++)
                        {
                            if (xoSQL.GUID == ChildrenSqlWindows[xiChild].GUID)
                            {
                                // found a child
                                string xsWhereClause = "";

                                for (int xiFld = 0; xiFld < xoSQL.MasterFields.Count; xiFld++)
                                {
                                    string xsFld = xoSQL.MasterFields[xiFld];
                                    for (int xiCol = 0; xiCol < grd.Columns.Count; xiCol++)
                                    {
                                        if (grd.Columns[xiCol].Name.ToUpper() == xsFld.ToUpper())
                                        {
                                            if (xsWhereClause.Length > 0) xsWhereClause += " AND ";
                                            object xo = null;
                                            try
                                            {
                                                xo = grd.CurrentRow.Cells[xiCol].Value;
                                            }
                                            catch { }
                                            xsWhereClause += ChildrenSqlWindows[xiChild].Fields[xiFld] + " = ";
                                            string xsType = grd.Columns[xiCol].ValueType.ToString().ToUpper();
                                            string xsValue = "";
                                            bool xbNull = (xo == null);

                                            if (xsType.IndexOf(".INT") >= 0)
                                            {
                                                if (xbNull) xsValue = "0"; else
                                                xsValue = xo.ToString();
                                            }
                                            else
                                            {
                                                if (xsType.IndexOf(".DATE") >= 0)
                                                {
                                                    string xsQt = "'";
                                                    if (moDB.Connection.Provider.IsAccess)
                                                    {
                                                        xsQt = "#";
                                                    }
                                                    if (xbNull) xo = Convert.ToDateTime("1900-01-01");
                                                    xsValue = xsQt + Convert.ToDateTime(xo).ToString() + xsQt;
                                                }
                                                else
                                                {
                                                    if (xsType.IndexOf(".STRING") >= 0)
                                                    {
                                                        if (xbNull) xo = "";
                                                        xsValue = "'" + xo.ToString() + "'";
                                                    }
                                                    else
                                                    {
                                                        if (xsType.IndexOf(".BOOL") >= 0)
                                                        {
                                                            if (xbNull) xo = false;
                                                            if (Convert.ToBoolean(xo)) xsValue = "true"; else xsValue = "false";
                                                        }
                                                    }
                                                }
                                            }
                                            xsWhereClause = xsWhereClause + xsValue;
                                        }
                                    }
                                }
                                if (xsWhereClause.Length > 0)
                                {
                                    xoSQL.UpdateFromParent(xsWhereClause, ChildrenSqlWindows[xiChild].OrderByClause);
                                }
                            }
                        }
                        xoSQL = null;
                    }
                }
                miLastRow = xiCurrRow;          // don't want to repeat if row did not change (performance and other issues)
                msLastChildWhereClause = msChildWhereClause;
                xoParent = null;
            }
        }

        private void grd_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            //UpdateChildrenForNewRow();
            tmrRowChange.Enabled = true;
        }

        private void tmrRowChange_Tick(object sender, EventArgs e)
        {
            tmrRowChange.Enabled = false;
            tmrRowChange.Interval = 100;
            //if (mbTrace)
            //{
            //    string xs = "trace - put break pt here";
            //    xs = "";
            //    mbTrace = false;
            //}
            UpdateChildrenForNewRow();
        }

        private bool ChildWindowExists(string psGUID)
        {
            bool xbFound = false;
            frmMain xoParent = (frmMain)this.MdiParent;

            foreach (Form xoForm in xoParent.MdiChildren)
            {
                bool xbSQLForm = true;
                frmSQL2 xoSQL = null;
                try
                {
                    xoSQL = (frmSQL2)xoForm;
                }
                catch
                {
                    xbSQLForm = false;
                }
                if (xbSQLForm)
                {
                    if (xoSQL.GUID == psGUID)
                    {
                        xbFound = true;
                        break;
                    }
                }
                if (xoSQL != null) xoSQL = null;
            }

            xoParent = null;

            return (xbFound);
        }

        public string MasterChildPrepForDataStorage()
        {
            //      line 1: GUID
            //      line 2: MasterGUID
            //      line 3: comma-delimited master fields
            //      line 4..x: comma-delimited GUID, fld1, fldx,.. 

            string xsData = this.GUID + "\r\n";
            xsData += this.MasterGUID + "\r\n";

            string xsLine = "";
            for (int xii = 0; xii < this.MasterFields.Count; xii++)
            {
                if (xii > 0) xsLine = xsLine + ", ";
                xsLine += this.MasterFields[xii];
            }
            xsData += xsLine;

            for (int xii = 0; xii < this.ChildrenSqlWindows.Count; xii++)
            {
                xsLine = this.ChildrenSqlWindows[xii].GUID;
                if (ChildWindowExists(xsLine))
                {
                    for (int xiFld = 0; xiFld < this.ChildrenSqlWindows[xii].Fields.Count; xiFld++)
                    {
                        xsLine = xsLine + ", " + this.ChildrenSqlWindows[xii].Fields[xiFld];
                    }
                    xsData = xsData + "\r\n" + xsLine;
                    string xs = Global.NullToString(this.ChildrenSqlWindows[xii].OrderByClause).Trim();
                    if (xs.Length > 0) xsData = xsData + "^ORDER BY " + xs;
                    string xs2 = this.Text;
                    xs2 = "";
                }
                else
                {
                    //string xs3 = "child was dropped";
                    // go ahead and drop the child from the ChildSqlWindows array, but should do that from the FormClosing
                }
            }
            return (xsData);
        }

        public void MasterChildBreakdownFromDataStorage(string psData)
        {
            if (psData.Length > 0)
            {
                string xsMasterFlds = "";
                string[] xsSep = {"\r\n", "\r", "\n"};
                string[] xsLines = psData.Split(xsSep, StringSplitOptions.None);

                if (xsLines.Length >= 1) this.GUID = xsLines[0].Trim();
                if (xsLines.Length >= 2) this.MasterGUID = xsLines[1].Trim();
                if (xsLines.Length >= 3) xsMasterFlds = xsLines[2].Trim();

                if (xsMasterFlds.Length > 0)
                {
                    try
                    {
                        string[] xsSep2 = { "," };
                        string[] xsFlds = xsMasterFlds.Split(xsSep2, StringSplitOptions.None);
                        MasterFields = new List<string>(0);
                        for (int xi2 = 0; xi2 <= xsFlds.GetUpperBound(0); xi2++)
                        {
                            MasterFields.Add(xsFlds[xi2]);
                        }
                    }
                    catch { }
                }

                // attach this child, if so
                if (xsLines.Length >= 4)
                {
                    int xiIdx = 3;
                    while (xiIdx + 1 <= xsLines.Length)
                    {
                        string xsOrderByClause = "";
                        string xsData = xsLines[xiIdx].Trim();
                        if (xsData.Length > 0)
                        {
                            int xi = xsData.IndexOf("^ORDER BY ");
                            if (xi >= 0)
                            {
                                xsOrderByClause = xsData.Substring(xi);
                                xsData = xsData.Substring(0, xi);
                                xsOrderByClause = xsOrderByClause.Replace("^ORDER BY ", "").Trim();
                            }
                        }
                        if (xsData.Length > 0)
                        {
                            string[] xsSep2 = { "," };
                            string[] xsFlds = xsData.Split(xsSep2, StringSplitOptions.None);
                            if (xsFlds.Length >= 2)
                            {
                                ChildInfo xrChild = new ChildInfo();
                                xrChild.GUID = xsFlds[0].Trim();
                                List<string> xsChildFlds = new List<string>(0);
                                for (int xi3 = 1; xi3 < xsFlds.Length; xi3++)
                                {
                                    xsChildFlds.Add(xsFlds[xi3]);
                                }
                                xrChild.Fields = xsChildFlds;
                                xrChild.OrderByClause = xsOrderByClause;
                                ChildrenSqlWindows.Add(xrChild);
                                //msChildOrderByClause = xsOrderByClause;
                            }
                        }
                        xiIdx++;
                    }
                }
            }
        }

        private void toolStripShowMasterChildInfo_Click(object sender, EventArgs e)
        {
            string xsInfo = "";

            bool xbIsChild = (this.MasterGUID.Length > 0);
            bool xbIsMaster = (this.ChildrenSqlWindows.Count > 0);
            if (xbIsMaster)
            {
                if (xbIsChild) xsInfo = "This window is BOTH a PARENT and a CHILD."; else xsInfo = "This window is a PARENT.";
            }
            else
            {
                if (xbIsChild) xsInfo = "This window is a CHILD.";
            }
            if (xsInfo.Length > 0) xsInfo += "\r\n";

            xsInfo += "\r\nThis Window's GUID: " + this.GUID;
            CheckSpecTable();
            xsInfo += "\r\n        Spec Table: " + msSpecTable;
            if (MasterGUID.Length > 0)
            {
                xsInfo += "\r\n        Our Parent: " + this.MasterGUID;
                xsInfo += "\r\n      WHERE Clause: " + msChildWhereClause;
                xsInfo += "\r\n   Master Field(s): ";
                string xsFlds = "";
                for (int xii = 0; xii < MasterFields.Count; xii++)
                {
                    if (xii > 0) xsFlds += ", ";
                    xsFlds += MasterFields[xii];
                }
                xsInfo += xsFlds;
            }

            if (ChildrenSqlWindows.Count > 0)
            {
                for (int xii = 0; xii < ChildrenSqlWindows.Count; xii++)
                {
                    xsInfo += "\r\n";
                    xsInfo += "\r\n Child Window GUID: " + ChildrenSqlWindows[xii].GUID;
                    xsInfo += "\r\n    Child Field(s): ";
                    string xsFlds = "";
                    for (int xi2 = 0; xi2 < ChildrenSqlWindows[xii].Fields.Count; xi2++)
                    {
                        if (xi2 > 0) xsFlds += ", ";
                        xsFlds += ChildrenSqlWindows[xii].Fields[xi2];
                    }
                    xsInfo += xsFlds;
                    if (ChildrenSqlWindows[xii].OrderByClause.Length > 0)
                    {
                        xsInfo += "\r\n   Order By Clause: " + ChildrenSqlWindows[xii].OrderByClause;
                    }
                }
            }

            //Global.ShowMessage(xsInfo);
            frmInfo xoFrm = new frmInfo(xsInfo);

            xoFrm.Text = "Parent/Child Info: " + Proper(this.Text);
            xoFrm.ShowDialog();
        }

        public void PrintData()
        {
            string xsErrMsg = "";

            System.Windows.Forms.DialogResult xeDlgResult;
            System.Drawing.Printing.StandardPrintController xoPrtController = new StandardPrintController();
            PrintDialog xoPrtDlg = new PrintDialog();

            // see if we have any data to print
            if (grd.Rows.Count == 0)
            {
                xsErrMsg = "There is no data to print.";
            }

            if (xsErrMsg.Length == 0)
            {
                xoPrtDlg.UseEXDialog = false;
                xeDlgResult = xoPrtDlg.ShowDialog();
                if (xeDlgResult == System.Windows.Forms.DialogResult.OK)
                {
                    prtDoc.PrinterSettings = xoPrtDlg.PrinterSettings;
                    prtDoc.DocumentName = this.Text;
                    prtDoc.PrintController = xoPrtController;
                    miCurrRowToPrint = 1;
                    prtDoc.Print();
                }
                else
                {
                    MessageBox.Show("Printing canceled.", "Print Data");
                }
            }
            else
            {
                MessageBox.Show(xsErrMsg, "Print Data");
            }
            xoPrtController = null;
            xoPrtDlg = null;
        }

        private void prtDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            float xnLinesPerPage = 0;
            float xnX = 0, xnY = 0;
            int xiCol = 0, xiLinesUsed = 0, xiWidth = 0;
            float xnLeftMargin = e.MarginBounds.Left;
            float xnTopMargin = e.MarginBounds.Top;
            string xs = "";
            Font xoPrFont = new Font(mrReportStyle.FontName, mrReportStyle.FontSizeTitle, FontStyle.Bold);
            object xv = null;

            // report title, if supplied
            xs = mrReportStyle.Title;
            if (xs.Length == 0) xs = this.Text;
            if (xs.Length > 0)
            {
                xnX = (e.Graphics.DpiX - xiWidth) / 2;
                xnY = xnTopMargin + (xiLinesUsed * xoPrFont.GetHeight(e.Graphics));
                e.Graphics.DrawString(xs, xoPrFont, Brushes.Black, xnX, xnY, new StringFormat());
                xiLinesUsed++;
                xnX = xnLeftMargin;
                xnY = xnTopMargin + (xiLinesUsed * xoPrFont.GetHeight(e.Graphics));
                e.Graphics.DrawString("", xoPrFont, Brushes.Black, xnX, xnY, new StringFormat());       // blank line separator
                xiLinesUsed++;
                xoPrFont = new Font(mrReportStyle.FontName, mrReportStyle.FontSize, FontStyle.Bold);
            }

            // column headings next
            xnX = xnLeftMargin;
            xnY = xnTopMargin + (xiLinesUsed * xoPrFont.GetHeight(e.Graphics));
            for (xiCol = 1; xiCol <= grd.Columns.Count; xiCol++)
            {
                xs = grd.Columns[xiCol - 1].HeaderText;
                e.Graphics.DrawString(xs, xoPrFont, Brushes.Black, xnX, xnY, new StringFormat());
                xnX = xnX + grd.Columns[xiCol - 1].Width;
            }
            xiLinesUsed++;
            xnY = xnTopMargin + (xiLinesUsed * xoPrFont.GetHeight(e.Graphics));

            // column separator line
            Pen xoPen = new Pen(System.Drawing.Color.Black);
            Point xoPt1 = new Point(), xoPt2 = new Point();
            xoPt1.X = Convert.ToInt16(xnLeftMargin);
            xoPt1.Y = Convert.ToInt16(xnTopMargin + (xiLinesUsed * xoPrFont.GetHeight(e.Graphics)));
            xoPt2.X = Convert.ToInt16(xnX);
            xoPt2.Y = xoPt1.Y;
            e.Graphics.DrawLine(xoPen, xoPt1, xoPt2);
            xiLinesUsed++;

            xoPrFont = new Font(mrReportStyle.FontName, mrReportStyle.FontSize);       // non-bold for data
            xnY = xnTopMargin + (xiLinesUsed * xoPrFont.GetHeight(e.Graphics));
            xnLinesPerPage = (e.MarginBounds.Height - xnY) / xoPrFont.GetHeight(e.Graphics);

            while (xiLinesUsed < xnLinesPerPage && (miCurrRowToPrint < grd.Rows.Count))
            {
                xnY = xnTopMargin + (xiLinesUsed * xoPrFont.GetHeight(e.Graphics));
                xnX = xnLeftMargin;

                for (xiCol = 1; xiCol <= grd.Columns.Count; xiCol++)
                {
                    try
                    {
                        xv = grd.Rows[miCurrRowToPrint - 1].Cells[xiCol - 1].Value;
                    }
                    catch { xv = null; }
                    if (xv != null)
                    {
                        xs = xv.ToString();
                        e.Graphics.DrawString(xs, xoPrFont, Brushes.Black, xnX, xnY, new StringFormat());
                    }
                    xnX = xnX + grd.Columns[xiCol - 1].Width;
                }
                xiLinesUsed++;
                miCurrRowToPrint++;
            }

            // if more to print, keep on going, else send document to printer
            if (miCurrRowToPrint < grd.Rows.Count)
                e.HasMorePages = true;
            else
                e.HasMorePages = false;

            xoPrFont = null;
            xoPen.Dispose();
        }

        private void txtSQL_TextChanged(object sender, EventArgs e)
        {
            mbSQLDirty = true;

            if (!EditorProgrammaticChangeUnderway && EditorColorizingOn)
            {
                int xiQtStart, xiQtdLen, xiCmtStart, xiCmtLen;
                int xiCursorPos = txtSQL.SelectionStart;
                bool xbOth, xbInLine;

                //ProcessEditorKey();

                if (xiCursorPos >= 0)
                {
                    if (xiCursorPos > 0 && txtSQL.Text.Substring(xiCursorPos - 1, 1) == QuoteChr  && !EditorInComment)       // key just typed a quote (but not inside a comment)?
                    {
                        //if (!EditorInComment)
                        //{
                            if (IsCurrentCursorPositionInCommentOrQuote(false, out xiQtStart, out xiQtdLen, out xbInLine, out xbOth, xiCursorPos + 1))
                            {
                                EditorInQuote = true;
                                EditorInComment = false;
                                if (xiQtdLen > 0)
                                {
                                    ColorizeText(xiQtStart, xiQtdLen + 1, InQuoteTextColor, xiCursorPos);
                                }
                                else
                                {
                                    ColorizeText(xiCursorPos - 1, 1, InQuoteTextColor, xiCursorPos);
                                }
                            }
                            else
                            {
                                EditorInQuote = false;
                            }
                        //}
                    }
                    else
                    {
                        if (!EditorInComment && xiCursorPos > 1 && (txtSQL.Text.Substring(xiCursorPos - 2, 2) == "/*" || txtSQL.Text.Substring(xiCursorPos - 2, 2) == "--" || txtSQL.Text.Substring(xiCursorPos - 2, 2) == "*/"))       // key just typed one that can affect comments?
                        {
                            bool xbInline = false;
                            if (IsCurrentCursorPositionInCommentOrQuote(true, out xiCmtStart, out xiCmtLen, out xbInline, out xbOth, xiCursorPos + 1))
                            {
                                EditorInComment = true;
                                EditorInQuote = false;
                                EditorInCommentSingleLine = xbInline;
                                if (xiCmtLen > 0)
                                {
                                    ColorizeText(xiCmtStart, xiCmtLen + 2, CommentsColor, xiCursorPos);
                                }
                                else
                                {
                                    ColorizeText(xiCursorPos - 2, 2, CommentsColor, xiCursorPos);
                                }
                            }
                            else
                            {
                                EditorInComment = false;
                            }
                        }
                        else
                        {
                            //
                            // process all other keys (non-quote, non-comment)
                            //
                            int xiCursorPosInWord = 0;
                            string xsChrToLeft = "";
                            string xsWord = WordAtCursor(out xiCursorPosInWord, out xsChrToLeft);



                            //this.Text = DateTime.Now.ToString() + "  \"" + xsWord + "\"   \"" + xsChrToLeft + "\"    " + xiCursorPosInWord.ToString() + "   " + xiCursorPos.ToString();



                            if (EditorRecheckInQuote || EditorInOverwriteMode)
                            {
                                int xiSt, xiLn;
                                bool xbIn;
                                EditorInQuote = IsCurrentCursorPositionInCommentOrQuote(false, out xiSt, out xiLn, out xbIn, out xbOth);
                                //if (EditorInQuote) EditorInComment = false;
                                EditorRecheckInQuote = false;
                            }

                            if (EditorRecheckInComment || EditorInOverwriteMode)
                            {
                                int xiSt, xiLn;
                                bool xbIn;
                                EditorInComment = IsCurrentCursorPositionInCommentOrQuote(true, out xiSt, out xiLn, out xbIn, out xbOth);
                                //if (EditorInComment) EditorInQuote = false;
                                EditorInCommentSingleLine = xbIn;
                                EditorRecheckInComment = false;
                            }

                            if (!EditorInComment && CharToLeftIsCloseQuote())           // do we need to close a quote?
                            {
                                ColorizeText(xiCursorPos - 1, 1, InQuoteTextColor, xiCursorPos);
                            }
                            else
                            {
                                if (!EditorInComment && CharsToLeftAreCloseComment())       // do we need to close a comment?
                                {
                                    ColorizeText(xiCursorPos - 2, 1, CommentsColor, xiCursorPos);
                                }
                                else
                                {
                                    Color xiColor = RegularColor;
                                    if (EditorInQuote)
                                    {
                                        xiColor = InQuoteTextColor;
                                    }
                                    else
                                    {
                                        if (EditorInComment)
                                        {
                                            xiColor = CommentsColor;
                                        }
                                        else
                                        {
                                            /*int xi = xsWord.IndexOf("(");       // let's see if potential start of a SQL keyword...if so, handle the parens stuff separately
                                            if (xi >= 0)
                                            {
                                                if (xi == 0)
                                                {
                                                    string xsWord2 = "";
                                                    string xsNonWord = "";
                                                    while (xi < xsWord.Length)
                                                    {
                                                        string xs = xsWord.Substring(xi, 1);
                                                        if ("ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(xs.ToUpper()) >= 0)
                                                        {
                                                            xsWord2 = xsWord.Substring(xi);
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            xsNonWord += xs;
                                                        }
                                                        xi++;
                                                    }
                                                    if (xsNonWord.Length > 0)
                                                    {
                                                        txtSQL.Select(xiCursorPos - xsNonWord.Length, xsNonWord.Length);
                                                        txtSQL.SelectionColor = RegularColor;
                                                    }
                                                    xsWord = xsWord2;
                                                    xiCursorPosInWord -= xi;
                                                }
                                                else
                                                {
                                                    string xsNonWord = xsWord.Substring(xi);
                                                    if (xsNonWord.Length > 0)
                                                    {
                                                        txtSQL.Select(xiCursorPos - xsNonWord.Length, xsNonWord.Length);
                                                        txtSQL.SelectionColor = RegularColor;
                                                    }
                                                    xsWord = xsWord.Substring(0, xi);
                                                }
                                            }*/
                                            bool xbIsFunc;
                                            if (IsKeyword(xsWord, out xbIsFunc))      // check if what came in (or what remains after parens processing) is a keyword
                                            {
                                                xiColor = (xbIsFunc ? FunctionColor : KeywordColor);
                                            }
                                            else
                                            {
                                                if (xsWord.Length > 0 && xsWord.Substring(0, 1) == "@")
                                                {
                                                    if (xsWord.Length > 1 && xsWord.Substring(1, 1) == "@")
                                                    {
                                                        xiColor = AtAtColor;
                                                    }
                                                    else
                                                    {
                                                        xiColor = VariableColor;        // or a variable
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    //if (xsWord.Length > 0)
                                    //{
                                        ColorizeText(xiCursorPos - xiCursorPosInWord - 0, xsWord.Length, xiColor, xiCursorPos);
                                    //}
                                    //else
                                    //{
                                        if (xsChrToLeft.Length > 0)
                                        {
                                            xiColor = RegularColor;
                                            if (EditorInQuote) xiColor = InQuoteTextColor;
                                            if (EditorInComment) xiColor = CommentsColor;
                                            ColorizeText(xiCursorPos - 1, 1, xiColor, xiCursorPos);
                                        }
                                    //}
                                }
                            }
                        }
                    }
                }
                ProcessEditorKey();
            }
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string xs = "This popup context menu allows you to: \r\n\r\n";

            xs += "  Allow Changes: by default, new SQL windows cannot have the data edited...toggle here\r\n";
            xs += "  Lock SQL:  by default, window opened via TABLE_NAME double-click are locked...toggle here\r\n";
            xs += "  Last SQL:  shows the last SQL statement sent to the provider, resulting in current data\r\n";
            xs += "  Colorize SQL:  attempts to highlight commands/variables/etc but is not locked into any particular provider's syntax\r\n";
            xs += "  Colorizing:  SQL is dynamically colored for syntax as you type\r\n";
            xs += "  Properties:  allows you to set the caption for the window, plus a description, and to group windows together\r\n";
            xs += "     visually...all optional, but very powerful\r\n";
            xs += "  New SQL:  opens a brand new, fresh window, for an SQL query/command\r\n";
            xs += "  Child SQL:  very powerful feature...allows you to open a new SQL window, related to the window clicked on, of which\r\n";
            xs += "     changes in the parent window filter data for the newly created child\r\n";
            xs += "  Get Ruby/Rails FKs: for databases with tables and columns named in the Rails convention for primary keys,\r\n";
            xs += "     automatically build a list from the database, and optionally save it for later connecting child data to parent data windows\r\n";
            xs += "  Parent/Child info:  shows the info of this table as it relates to whether or not it is a child, a parent, or even both\r\n";
            xs += "  Export to CSV:  sends current data in this windows' table to a text/line Comma-Delimited-Variable table of your choice\r\n";
            xs += "  Export to JSON:  sends current data in this windows' table to a text file with JSON notation\r\n";
            xs += "  Print:  sends current data in this windows' table to a printer\r\n";
            xs += "  Choose Child:  opens a new SQL child window using previously defined links\r\n";
            xs += "  Link as Child:  click the column in the parent SQL window, then click the column in the child SQL Window, and\r\n";
            xs += "     choose this option to make a quick parent-child link";
            xs += "  DateTimes Long Format:  show date/time data columns with full time stats";
            xs += "  DateTimes Format String:  enter a custom date/time format string using .NET encoding";
            Global.ShowMessage(xs, "About " + Application.ProductName);
        }

        private void tabSQL_DoubleClick(object sender, EventArgs e)
        {
            //object xo = null;

            //MessageBox.Show(xo.ToString());
        }

        private void grd_ColumnSortModeChanged(object sender, DataGridViewColumnEventArgs e)
        {
            //string xs = "fuck";
        }

        private void grd_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            //tmrRowChange.Enabled = true;
        }

        private void grd_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            //tmrRowChange.Enabled = true;
        }

        private void grd_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //mbTrace = true;
            tmrRowChange.Enabled = true;
            miLastRow = -1;
            
            msLastColmClicked = grd.Columns[e.ColumnIndex].DataPropertyName.ToString();
        }

        public string LastColmClicked()
        {
            return (msLastColmClicked);
        }

        private void tsmLinkChild_Click(object sender, EventArgs e)
        {
            LinkMeAsChild();
        }

        private void LinkMeAsChild()
        {
            string xsErrMsg = "", xsTable = "", xsColm = "";
            frmMain xoParent = (frmMain)this.MdiParent;
            List<string> xsChoices = new List<string>(0);
            List<frmSQL2> xoSQLs = new List<frmSQL2>(0);
            frmSQL2 xoSQL = null;

            foreach (Form xoForm in xoParent.MdiChildren)
            {
                bool xbSQLForm = true;
                try
                {
                    xoSQL = (frmSQL2)xoForm;
                }
                catch
                {
                    xbSQLForm = false;
                }
                if (xbSQLForm)
                {
                    xsTable = xoSQL.GetSpecTable().Trim();
                    if (xsTable.Length > 0 && xsTable.Trim().ToLower() != msSpecTable.Trim().ToLower())
                    {
                        xsColm = xoSQL.LastColmClicked().Trim();
                        if (xsColm.Length > 0)
                        {
                            // found another SQL window which from a single table, which has had a column clicked
                            // so present this as a choice to link as parent
                            xsChoices.Add(xsTable + "." + xsColm);
                            xoSQLs.Add(xoSQL);
                        }
                    }
                }
                if (xoSQL != null) xoSQL = null;
            }

            xoParent = null;

            if (xsChoices.Count > 0)
            {
                frmPickOne xoPick = new frmPickOne();
                xoPick.SetTitlePromptList("Choose Parent", "Table/Column", xsChoices);
                xoPick.ShowDialog();
                if (xoPick.OK())
                {
                    string xsPick = xsChoices[xoPick.SelectedIndex()];
                    xsTable = xsPick.Substring(0, xsPick.IndexOf("."));
                    xsColm = xsPick.Substring(xsPick.IndexOf(".") + 1);
                    xoSQL = xoSQLs[xoPick.SelectedIndex()];

                    // insert into ForeignKeys if not already there
                    DB xoAppDB = new DB();
                    if (Global.OpenThisAppsDatabase(ref xoAppDB))
                    {
                        DataTable xoTbl = new DataTable();
                        string xsSQL = "SELECT * FROM ForeignKeys WHERE databaseID = " + miDbID + " AND tableA = '" + xsTable + "' AND tableB = '" + msSpecTable + "' AND fieldA = '" + xsColm + "' AND fieldB = '" + msLastColmClicked + "'";
                        if (xoAppDB.SQL(xsSQL, xoTbl))
                        {
                            if (xoTbl.Rows.Count == 0)
                            {
                                xsSQL = "INSERT INTO ForeignKeys (databaseID, tableA, tableB, fieldA, fieldB) VALUES (";
                                xsSQL += miDbID.ToString();
                                xsSQL += ",'" + xsTable + "'";
                                xsSQL += ", '" + msSpecTable + "'";
                                xsSQL += ", '" + xsColm + "'";
                                xsSQL += ", '" + msLastColmClicked + "')";
                                if (!xoAppDB.SQL(xsSQL)) xsErrMsg = xoAppDB.Message;
                            }
                        }
                        else
                        {
                            xsErrMsg = xoAppDB.Message;
                        }
                        xoTbl.Dispose();
                        xoTbl = null;
                    }
                    Global.CloseThisAppsDatabase(ref xoAppDB);

                    //// make this window a child of selected window
                    //xoSQL.LockSQL(true);
                    //this.LockSQL(true);
                    //this.Text = Proper(this.Text) + " < " + Proper(xsTable);

                    //List<string> xsFlds = new List<string>(0);
                    //ChildInfo xrChild = new ChildInfo();

                    //// set up the master to point to this instance
                    //xrChild.OrderByClause = "";
                    //xrChild.GUID = this.GUID;
                    //xsFlds.Add(msLastColmClicked);
                    //xrChild.Fields = xsFlds;
                    //xoSQL.ChildrenSqlWindows.Add(xrChild);
                    
                    //// set up this instance to now be a child of the master
                    //xsFlds = new List<string>(0);
                    //this.MasterGUID = xoSQL.GUID;
                    //xsFlds.Add(xsColm);
                    //this.MasterFields = xsFlds;

                    //xoSQL.UpdateChildrenForNewRow(true);
                    LinkAsChild(xoSQL, xsTable, xsColm, msLastColmClicked);
                }

                xoPick.Dispose();
                xoPick = null;

                // detach pointers to other SQL windows
                xoSQL = null;
                for (int xii = 0; xii < xoSQLs.Count; xii++)
                {
                    xoSQLs[xii] = null;
                }
            }
            else
            {
                xsErrMsg = "No other SQL windows have a column clicked.";
            }

            if (xsErrMsg.Length > 0)
            {
                Global.ShowMessage(xsErrMsg, "Link as Child");
            }
        }

        public void LinkAsChild(frmSQL2 poSQL, string psTable, string psColm, string psChildColm)
        {
            // make this window a child of selected window
            poSQL.LockSQL(true);
            this.LockSQL(true);
            this.Text = Proper(this.Text) + " < " + Proper(psTable);

            List<string> xsFlds = new List<string>(0);
            ChildInfo xrChild = new ChildInfo();

            // set up the master to point to this instance
            xrChild.OrderByClause = "";
            xrChild.GUID = this.GUID;
            xsFlds.Add(psChildColm);
            xrChild.Fields = xsFlds;
            poSQL.ChildrenSqlWindows.Add(xrChild);

            // set up this instance to now be a child of the master
            xsFlds = new List<string>(0);
            this.MasterGUID = poSQL.GUID;
            xsFlds.Add(psColm);
            this.MasterFields = xsFlds;

            poSQL.UpdateChildrenForNewRow(true);
        }

        private void grd_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            string xs = e.Exception.Message;
            UpdateStatusBar("grd error: " + xs);
        }

        private void tsmDateTimeToLongFormat_Click(object sender, EventArgs e)
        {
            SetGridDateTimeColumnsToLongFormat();
        }

        private void tsmChooseChild_Click(object sender, EventArgs e)
        {
            SpawnChildWindowFromForeignKey();
        }

        private void SpawnChildWindowFromForeignKey()
        {
            const string xcsSep = " -> ";
            string xsErrMsg = "";

            RebusData6.DB xoDB = new DB();
            if (Global.OpenThisAppsDatabase(ref xoDB))
            {
                string xsSQL = "SELECT fieldA, tableB, fieldB FROM ForeignKeys WHERE databaseID = " + miDbID.ToString() + " AND tableA = '" + msSpecTable + "' AND fieldA IS NOT NULL AND LEN(TRIM(fieldA)) > 0 AND tableB IS NOT NULL AND LEN(TRIM(tableB)) > 0 AND fieldB IS NOT NULL AND LEN(TRIM(fieldB)) > 0";
                DataTable xoTbl = new DataTable();
                if (xoDB.SQL(xsSQL, xoTbl))
                {
                    if (xoTbl.Rows.Count > 0)
                    {
                        List<string> xsChoices = new List<string>(0);
                        for (int xii = 0; xii < xoTbl.Rows.Count; xii++)
                        {
                            string xsChoice = Global.NullToString(xoTbl.Rows[xii][0]).Trim() + xcsSep + Global.NullToString(xoTbl.Rows[xii][1]).Trim() + "." + Global.NullToString(xoTbl.Rows[xii][2]).Trim();
                            xsChoices.Add(xsChoice);
                        }
                        frmPickOne xoPick = new frmPickOne();
                        xoPick.SetTitlePromptList("Choose Child", msSpecTable + ".{Column}" + xcsSep + " {Table}.{Column}", xsChoices);
                        xoPick.ShowDialog();
                        if (xoPick.OK())
                        {
                            string xsPick = xsChoices[xoPick.SelectedIndex()];
                            string xsParentColm = xsPick.Substring(0, xsPick.IndexOf(xcsSep)).Trim();
                            string xsChildColm = xsPick.Substring(xsPick.IndexOf(xcsSep) + xcsSep.Length).Trim();
                            string xsChildTable = xsChildColm.Substring(0, xsChildColm.IndexOf("."));
                            xsChildColm = xsChildColm.Substring(xsChildColm.IndexOf(".") + 1);

                            frmMain xoFrm = (frmMain)this.MdiParent;
                            frmSQL2 xoSQL = xoFrm.CreateNewSQLWindow(xsChildTable, "SELECT * FROM [" + xsChildTable + "]");
                            xoFrm = null;
                            xoSQL.LinkAsChild(this, msSpecTable, xsParentColm, xsChildColm);
                            xoSQL = null;
                        }
                        xoPick.Close();
                        xoPick.Dispose();
                    }
                    else
                    {
                        xsErrMsg = "No previously linked child tables for table \"" + msSpecTable + "\".";
                    }
                }
                else
                {
                    xsErrMsg = xoDB.Message + "  (SQL: " + xsSQL + ")";
                }
                xoTbl.Dispose();
            }
            xoDB.CloseDatabase();

            if (xsErrMsg.Length > 0) Global.ShowMessage(xsErrMsg, "Child From Foreign Key");
        }

        private void mnuToJSON_Click(object sender, EventArgs e)
        {
            mbDlgSaveOK = false;
            dlgSave.DefaultExt = "txt";
            dlgSave.OverwritePrompt = true;
            dlgSave.Title = "Save Data to JSON File";
            dlgSave.RestoreDirectory = false;
            dlgSave.ShowDialog();
            if (mbDlgSaveOK) SaveDataToJSON(dlgSave.FileName);
        }

        /*private string WordToCursorLeftOrRight(bool pbToLeft, out int piCursorPosInWord)      // this not yet working
        {
            string xsWord = "";
            int xiCursorPosInWord = -1;

            int xiPos = txtSQL.SelectionStart;
            while ((pbToLeft && xiPos > 0) || (!pbToLeft && xiPos < txtSQL.Text.Length))
            {
                string xsCh = txtSQL.Text.Substring(xiPos - (pbToLeft ? 1 : 0), 1);
                if ("ABCDEFGHIJKLMNOPQRSTUVWXYZ_@#".IndexOf(xsCh.ToUpper()) >= 0)
                {
                    if (pbToLeft)
                    {
                        if (xsWord.Length == 0) xiCursorPosInWord = -2;     // signal at end of word;
                        xsWord = xsCh + xsWord;
                        xiPos--;
                    }
                    else
                    {
                        if (xsWord.Length == 0) xiCursorPosInWord = 0;      // signal at beginning of word;
                        xsWord = xsWord + xsCh;
                        xiPos++;
                    }
                }
                else
                {
                    break;
                }
            }

            piCursorPosInWord = xiCursorPosInWord;
            return (xsWord);
        }*/

        private string WordAtCursor(out int piCursorPosInWord, out string psChrToLeft)
        {
            string xsWord = "", xsChrToLeft = "";
            int xiCursorPosInWord = -1;

            for (int xiDirection = 1; xiDirection <= 2; xiDirection++)
            {
                int xiPos = txtSQL.SelectionStart;
                if (xiDirection == 2)
                {
                    // look to the right
                    while (xiPos < txtSQL.Text.Length)
                    {
                        string xsCh = txtSQL.Text.Substring(xiPos - 0, 1);
                        if (ALLOWABLE_KEYWORD_AND_VARIABLE_CHARS.IndexOf(xsCh.ToUpper()) < 0)
                        {
                            if (xsWord.Length == 0)
                            {
                                //xsWord = xsCh;
                                xiCursorPosInWord = 0;
                            }
                            break;
                        }
                        else
                        {
                            if (!EditorInQuote)
                            {
                                if (xsCh == QuoteChr) break;
                            }
                            if (!EditorInComment)
                            {
                                if (xsCh == "/")
                                {
                                    if (xiPos + 1 < txtSQL.Text.Length && txtSQL.Text.Substring(xiPos + 1, 1) == "*") break;
                                }
                            }
                            xsWord = xsWord + xsCh;
                        }
                        xiPos++;
                    }
                }
                else
                {
                    // look to the left
                    xiCursorPosInWord = 0;
                    while (xiPos > 0)
                    {
                        string xsCh = txtSQL.Text.Substring(xiPos - 1, 1);
                        if (ALLOWABLE_KEYWORD_AND_VARIABLE_CHARS.IndexOf(xsCh.ToUpper()) < 0)
                        {
                            if (xsWord.Length == 0)
                            {
                                //xsWord = xsCh;
                                xsChrToLeft = xsCh;
                                xiCursorPosInWord = 0;
                            }
                            break;
                        }
                        else
                        {
                            if (!EditorInQuote)
                            {
                                if (xsCh == QuoteChr) break;
                            }
                            if (!EditorInComment)
                            {
                                if (xsCh == "/")
                                {
                                    if (xiPos > 1 && txtSQL.Text.Substring(xiPos - 2, 1) == "*") break;
                                }
                            }
                            //if (xiCursorPosInWord < 0) xiCursorPosInWord = 0;
                            xsWord = xsCh + xsWord;
                            xiCursorPosInWord++;
                        }
                        xiPos--;
                    }
                }
            }

            piCursorPosInWord = xiCursorPosInWord;
            psChrToLeft = xsChrToLeft;
            return (xsWord);
        }

        /*private string xx_WordAtCursor(out int piCursorPosInWord)
        {
            string xsWord = "";
            int xiCursorPosInWord = -1;

            for (int xiDirection = 1; xiDirection <= 2; xiDirection++)
            {
                int xiPos = txtSQL.SelectionStart;
                if (xiDirection == 2)
                {
                    // look to the right
                    while (xiPos < txtSQL.Text.Length)
                    {
                        string xsCh = txtSQL.Text.Substring(xiPos - 0, 1);
                        if (xsCh == " " || xsCh == "\t" || xsCh == "\r" || xsCh == "\n")    // || xsCh == "(" || xsCh == ")")                // whitespace?
                        {
                            if ((xsCh == "(" || xsCh == ")") && xsWord.Length == 0)
                            {
                                xsWord = xsCh;
                                xiCursorPosInWord = 0;
                            }
                            break;
                        }
                        else
                        {
                            if (!EditorInQuote)
                            {
                                if (xsCh == QuoteChr) break;
                            }
                            if (!EditorInComment)
                            {
                                if (xsCh == "/")
                                {
                                    if (xiPos + 1 < txtSQL.Text.Length && txtSQL.Text.Substring(xiPos + 1, 1) == "*") break;
                                }
                            }
                            xsWord = xsWord + xsCh;
                        }
                        xiPos++;
                    }
                }
                else
                {
                    // look to the left
                    while (xiPos > 0)
                    {
                        string xsCh = txtSQL.Text.Substring(xiPos - 1, 1);
                        if (xsCh == " " || xsCh == "\t" || xsCh == "\r" || xsCh == "\n")    // || xsCh == "(" || xsCh == ")")                // whitespace?
                        {
                            if ((xsCh == "(" || xsCh == ")") && xsWord.Length == 0)
                            {
                                xsWord = xsCh;
                                xiCursorPosInWord = 0;
                            }
                            break;
                        }
                        else
                        {
                            if (!EditorInQuote)
                            {
                                if (xsCh == QuoteChr) break;
                            }
                            if (!EditorInComment)
                            {
                                if (xsCh == "/")
                                {
                                    if (xiPos > 1 && txtSQL.Text.Substring(xiPos - 2, 1) == "*") break;
                                }
                            }
                            xsWord = xsCh + xsWord;
                            xiCursorPosInWord++;
                        }
                        xiPos--;
                    }
                }
            }

            piCursorPosInWord = xiCursorPosInWord;
            return (xsWord);
        }*/

        private void InitKeywords()
        {
            Keywords = new List<Keyword>(0);
            string[] xsKeywords = KEYWORDS.Split(new string[] { "/" }, StringSplitOptions.None);
            for (int xii = 0; xii < xsKeywords.Length; xii++)
            {
                bool xbIsFunc = false;
                string xsKeyword = xsKeywords[xii];
                if (xsKeyword.IndexOf("*") >= 0)
                {
                    xbIsFunc = true;
                    xsKeyword = xsKeyword.Replace("*", "");
                }
                Keywords.Add(new Keyword(xsKeyword, xbIsFunc));
            }
        }

        private bool IsKeyword(string psWord, out bool pbIsFunc)
        {
            bool xbIsKeyword = false;
            bool xbIsFunc = false;
            string xsWord = psWord.ToUpper();

            if (Keywords == null)
            {
                InitKeywords();
            }

            foreach (Keyword xoKeyWord in Keywords)
            {
                if (xoKeyWord.Word == xsWord)
                {
                    xbIsKeyword = true;
                    xbIsFunc = xoKeyWord.IsFunction;
                    break;
                }
            }

            pbIsFunc = xbIsFunc;
            return (xbIsKeyword);
        }

        private bool CharsToLeftAreCloseComment()
        {
            bool xbIsCloseCmt = false;

            if (txtSQL.SelectionStart > 1)
            {
                if (txtSQL.Text.Substring(txtSQL.SelectionStart - 2, 2) == "*/")
                {
                    int xiCmtSt, xiCmtLen;
                    bool xbInline, xbOth;
                    if (IsCurrentCursorPositionInCommentOrQuote(true, out xiCmtSt, out xiCmtLen, out xbInline, out xbOth, txtSQL.SelectionStart - 1)) xbIsCloseCmt = true;
                }
            }

            return (xbIsCloseCmt);
        }

        private bool CharToLeftIsCloseQuote()
        {
            bool xbIsCloseQuote = false;

            if (txtSQL.SelectionStart > 0)
            {
                if (txtSQL.Text.Substring(txtSQL.SelectionStart - 1, 1) == QuoteChr)
                {
                    int xiQtSt, xiQtLen;
                    bool xbOth, xbInline;
                    if (IsCurrentCursorPositionInCommentOrQuote(false, out xiQtSt, out xiQtLen, out xbInline, out xbOth, txtSQL.SelectionStart - 1)) xbIsCloseQuote = true;
                }
            }

            return (xbIsCloseQuote);
        }

        private bool IsCurrentCursorPositionInCommentOrQuote(bool pbTestForInComment, out int piCmtOrQtStartPos, out int piCmtOrQtLen, out bool pbCmtInline, out bool pbOtherValue, int piOverridePos = -1)
        {
            bool xbCurrPosInCmt = false;
            bool xbInQt = false, xbInlineCmt = false;
            int xiCmtOrQtStartPos = -1, xiCmtOrQtLen = -1;
            int xiCursorPos = (piOverridePos > 0 ? piOverridePos : txtSQL.SelectionStart);

            int xiPos = 0;
            while (xiPos <= xiCursorPos - 1 && xiPos <= txtSQL.Text.Length - 1)
            {
                string xsCh = txtSQL.Text.Substring(xiPos, 1);

                if (xbInQt)
                {
                    if (xsCh == QuoteChr) xbInQt = false;
                }
                else
                {
                    if (xsCh == QuoteChr && !xbCurrPosInCmt)
                    {
                        xbInQt = true;
                        xiCmtOrQtStartPos = xiPos;
                    }
                    else
                    {
                        if (xbCurrPosInCmt)
                        {
                            if (xbInlineCmt)
                            {
                                if (xsCh == "\r" || xsCh == "\n") xbCurrPosInCmt = false;       // found end of the inline comment
                            }
                            else
                            {
                                if (xsCh == "/")
                                {
                                    if (xiPos > 0)
                                    {
                                        string xsPrevCh = txtSQL.Text.Substring(xiPos - 1, 1);
                                        if (xsPrevCh == "*") xbCurrPosInCmt = false;            // found end of potential multi-line comment
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (xsCh == "*" || xsCh == "-")
                            {
                                // look backward (prev char)
                                if (xiPos > 0)
                                {
                                    string xsPrevCh = txtSQL.Text.Substring(xiPos - 1, 1);
                                    if ((xsCh == "*" && xsPrevCh == "/") || (xsCh == "-" && xsPrevCh == "-"))
                                    {
                                        xbCurrPosInCmt = true;
                                        xbInlineCmt = (xsCh == "-");
                                        xiCmtOrQtStartPos = xiPos - 1;
                                    }
                                }
                            }
                        }
                    }
                }
                xiPos++;
            }

            /*if (!xbCurrPosInCmt)
            {
                if (xiCursorPos > 0 && xiCursorPos < txtSQL.Text.Length)
                {
                    string xs = txtSQL.Text.Substring(xiCursorPos - 1, 2);
                    if (xs == "--" || xs == "/*")
                    {
                        xiCmtStartPos = xiCursorPos - 1;
                        xbCurrPosInCmt = true;
                        xbInlineCmt = (xs == "-");
                        xiCursorPos++;
                    }
                }
            }*/

            if (xbCurrPosInCmt || xbInQt)
            {
                //
                // calculate comment or quoted string length
                //
                xiCmtOrQtLen = 0;
                xiPos = xiCmtOrQtStartPos + (pbTestForInComment ? 2 : 1);
                while (xiPos <= txtSQL.Text.Length - 1)
                {
                    xiCmtOrQtLen++;
                    string xsCh = txtSQL.Text.Substring(xiPos, 1);
                    if (pbTestForInComment)
                    {
                        if (xbInlineCmt)
                        {
                            if (xsCh == "\r" || xsCh == "\n") break;
                        }
                        else
                        {
                            if (xsCh == "/" && txtSQL.Text.Substring(xiPos - 1, 1) == "*") break;
                        }
                    }
                    else
                    {
                        if (xsCh == QuoteChr) break;
                    }
                    xiPos++;
                }
            }
            else
            {
                xiCmtOrQtStartPos = -1;
                xiCmtOrQtLen = -1;
            }

            piCmtOrQtStartPos = xiCmtOrQtStartPos;
            piCmtOrQtLen = xiCmtOrQtLen;
            pbCmtInline = xbInlineCmt;

            if (pbTestForInComment)
            {
                EditorCurrentInCommentStartPos = xiCmtOrQtStartPos;
                pbOtherValue = xbInQt;
            }
            else
            {
                EditorCurrentInQuoteStartPos = xiCmtOrQtStartPos;
                pbOtherValue = xbCurrPosInCmt;
            }

            return (pbTestForInComment ? xbCurrPosInCmt : xbInQt);
        }

        /*private bool IsCurrentCursorPositionInQuote(out int piQuoteStartPos, out int piQuotedLen, int piOverridePos = -1)
        {
            bool xbCurrPosInQt = false;
            int xiQuoteStartPos = -1, xiQuotedLen = -1;
            int xiCursorPos = (piOverridePos > 0 ? piOverridePos : txtSQL.SelectionStart);

            int xiPos = 0;
            while (xiPos <= xiCursorPos - 1 && xiPos < txtSQL.Text.Length)
            {
                string xsCh = txtSQL.Text.Substring(xiPos, 1);
                if (xbCurrPosInQt)
                {
                    if (xsCh == QuoteChr) xbCurrPosInQt = false;
                }
                else
                {
                    if (xsCh == QuoteChr)
                    {
                        xbCurrPosInQt = true;
                        xiQuoteStartPos = xiPos;
                    }
                }
                xiPos++;
            }

            if (xbCurrPosInQt)
            {
                xiQuotedLen = 0;
                xiPos = xiQuoteStartPos + 1;
                while (xiPos <= txtSQL.Text.Length - 1)
                {
                    xiQuotedLen++;
                    if (txtSQL.Text.Substring(xiPos, 1) == QuoteChr)
                    {
                        break;
                    }
                    xiPos++;
                }
            }
            else
            {
                xiQuoteStartPos = -1;
                xiQuotedLen = -1;
            }

            piQuoteStartPos = xiQuoteStartPos;
            piQuotedLen = xiQuotedLen;
            EditorCurrentInQuoteStartPos = xiQuoteStartPos;
            return (xbCurrPosInQt);
        }*/

        private void ProcessEditorKey()
        {
            //
            // if a key was detected in KeyDown to need some processing to event
            // after the keystroke has occurred (TextChanged), here we respond
            // to such
            //
            int xiStart, xiLen;
            bool xbOth = false;
            bool xbInline = false;
            EditorKeyDown = false;  // signal that last key down is being processed
            if (EditorReformatFromThisPositionToEnd >= 0)
            {
                int xiPos = EditorReformatFromThisPositionToEnd;
                EditorReformatFromThisPositionToEnd = -1;
                EditorReformatFromPosition(xiPos);
            }
            else
            {
                if (EditorNeedReformatFromCurrentPostionToEnd)
                {
                    EditorNeedReformatFromCurrentPostionToEnd = false;
                    //EditorInQuote = IsCurrentCursorPositionInCommentOrQuote(false, out xiStart, out xiLen, out xbInline, out xbOth);
                    EditorRecheckInQuote = false;
                    EditorInComment = IsCurrentCursorPositionInCommentOrQuote(true, out xiStart, out xiLen, out xbInline, out xbOth);
                    EditorInCommentSingleLine = xbInline;
                    EditorInQuote = xbOth;
                    EditorRecheckInComment = false;
                    ReformatFromCurrentPostionToEnd();
                }
            }

            if (EditorRecheckInQuote || EditorInOverwriteMode)
            {
                
                EditorInQuote = IsCurrentCursorPositionInCommentOrQuote(false, out xiStart, out xiLen, out xbInline, out xbOth);
                if (EditorInQuote) EditorInComment = false;
                EditorRecheckInQuote = false;
            }

            if (EditorRecheckInComment || EditorInOverwriteMode)
            {
                EditorInComment = IsCurrentCursorPositionInCommentOrQuote(true, out xiStart, out xiLen, out xbInline, out xbOth);
                if (EditorInComment) EditorInQuote = false;
                EditorInCommentSingleLine = xbInline;
                EditorRecheckInComment = false;
            }
        }

        private void ColorizeWord(int piPos, string psWord, bool pbInQuote = false, bool pbInComment = false)
        {
            txtSQL.Select(piPos, psWord.Length);
            if (pbInComment)
            {
                txtSQL.SelectionColor = CommentsColor;
                if (EditorForceFontMatching) txtSQL.Font = EditorOrgFont;
            }
            else
            {
                bool xbIsFunc = false;
                Color xiColor = (pbInQuote ? InQuoteTextColor : (IsKeyword(psWord, out xbIsFunc) ? KeywordColor : RegularColor));
                if (psWord.Substring(0, 1) == "@")
                {
                    if (psWord.Length > 1 && psWord.Substring(1, 1) == "@")
                    {
                        xiColor = AtAtColor;
                    }
                    else
                    {
                        xiColor = VariableColor;
                    }
                }
                else
                {
                    if (xbIsFunc) xiColor = FunctionColor;
                }
                txtSQL.SelectionColor = xiColor;
                if (EditorForceFontMatching) txtSQL.Font = EditorOrgFont;
            }
            txtSQL.Select(0, 0);
        }

        private void ColorizeText(int piStart, int piLen, Color poColor, int piCursorPos)
        {
            if (piLen > 0)
            {
                txtSQL.Select(piStart, piLen);
                txtSQL.SelectionColor = poColor;
                txtSQL.Select(0, 0);
                txtSQL.SelectionStart = piCursorPos;
            }
        }

        private void EditorReformatFromPosition(int piPos)
        {
            int xiPos = txtSQL.SelectionStart;

            txtSQL.SelectionStart = piPos;     // 0
            EditorInQuote = false;
            EditorInComment = false;
            ReformatFromCurrentPostionToEnd();

            int xiStart, xiLen;
            bool xbOth;
            bool xbInline = false;
            //EditorInQuote = IsCurrentCursorPositionInCommentOrQuote(false, out xiStart, out xiLen, out xbInline, out xbOth);
            EditorInComment = IsCurrentCursorPositionInCommentOrQuote(true, out xiStart, out xiLen, out xbInline, out xbOth);
            EditorInCommentSingleLine = xbInline;
            EditorInQuote = xbOth;

            txtSQL.SelectionStart = xiPos;
        }

        private void ReformatFromCurrentPostionToEnd()
        {
            int xiPos = txtSQL.SelectionStart;
            bool xbInQt = EditorInQuote;
            bool xbInCmt = EditorInComment;
            bool xbInlineCmt = EditorInCommentSingleLine;

            string xsWord = "";
            int xiWordStart = -1;
            int xiOrgPos = txtSQL.SelectionStart;

            if (xiPos == 0)
            {
                xbInQt = false;
                xbInCmt = false;
            }

            EditorProgrammaticChangeUnderway = true;

            if (!xbInQt && !xbInCmt && xiPos < txtSQL.Text.Length)
            {
                // if in middle of a word, need to work back to beginning of it
                int xii = xiPos;
                while (xii > 0)
                {
                    xii--;
                    string xsCh = txtSQL.Text.Substring(xii, 1);
                    if (ALLOWABLE_KEYWORD_AND_VARIABLE_CHARS.IndexOf(xsCh.ToUpper()) < 0)
                    {
                        break;
                    }
                    else
                    {
                        xiPos--;
                    }
                }
            }

            bool xbRefocus = txtSQL.Focused;

            //txtSQL.SuspendLayout();
            //DateTime xdStart = DateTime.Now;
            txtSQL.Visible = false;                     // updates about 5 times faster when not visible

            while (xiPos < txtSQL.Text.Length)
            {
                string xsCh = txtSQL.Text.Substring(xiPos, 1);

                if (xbInCmt)
                {
                    bool xbEndOfCmt = false;
                    if (xiPos >= 0 && (xsCh == "\r" || xsCh == "\n" || xsCh == "/" || xsCh == "*"))
                    {
                        if (xbInlineCmt && (xsCh == "\r" || xsCh == "\n"))
                        {
                            xbEndOfCmt = true;
                        }
                        else
                        {
                            if (xiPos > 0 && txtSQL.Text.Substring(xiPos - 1, 2) == "*/") xbEndOfCmt = true;
                        }
                    }
                    if (xbEndOfCmt)
                    {
                        xbInCmt = false;
                        if (xsWord.Length > 0)
                        {
                            ColorizeWord(xiWordStart, xsWord, false, true);
                            xsWord = "";
                            xiWordStart = -1;
                        }
                    }
                    else
                    {
                        if (xsWord.Length == 0) xiWordStart = xiPos;
                        xsWord += xsCh;
                    }
                }
                else
                {
                    if (xbInQt)
                    {
                        if (xsCh == QuoteChr)
                        {
                            // found end of quote, write out the current word and switch out of quote mode
                            xbInQt = false;
                            if (xsWord.Length > 0)
                            {
                                ColorizeWord(xiWordStart, xsWord, true, false);
                                xsWord = "";
                                xiWordStart = -1;
                            }
                        }
                        else
                        {
                            if (xsWord.Length == 0) xiWordStart = xiPos;
                            xsWord += xsCh;
                        }
                    }
                    else
                    {
                        if (xsCh == QuoteChr)
                        {
                            // found beginning of quote, so write out the current word and switch to in-quote mode
                            xbInQt = true;
                            if (xsWord.Length > 0)
                            {
                                ColorizeWord(xiWordStart, xsWord, false, false);
                                xiWordStart = -1;
                                xsWord = "";
                            }
                        }
                        else
                        {
                            bool xbStartCmt = false;
                            if (xiPos >= 1 && (xsCh == "-" || xsCh == "/" || xsCh == "*"))
                            {
                                string xsPrevChs = txtSQL.Text.Substring(xiPos - 1, 2);
                                if (xsPrevChs == "/*" || xsPrevChs == "--")
                                {
                                    xbStartCmt = true;
                                    xbInCmt = true;
                                    xbInlineCmt = (xsPrevChs == "--");
                                }
                            }
                            if (xbStartCmt)
                            {
                                // entire comment becomes "word"
                                if (xiWordStart < 0) xiWordStart = xiPos - 1;
                                xsWord += txtSQL.Text.Substring(xiPos - 1, 1) + xsCh;
                                xiPos++;
                                while (xiPos < txtSQL.Text.Length)
                                {
                                    xsCh = txtSQL.Text.Substring(xiPos, 1);
                                    if (xbInlineCmt)
                                    {
                                        if (xsCh == "\r" || xsCh == "\n")
                                        {
                                            xiPos--;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (xsCh == "*" && xiPos + 1 < txtSQL.Text.Length)
                                        {
                                            if (txtSQL.Text.Substring(xiPos + 1, 1) == "/")
                                            {
                                                xsWord += "*/";
                                                break;
                                            }
                                        }
                                    }
                                    xsWord += xsCh;
                                    xiPos++;
                                }
                                if (xsWord.Length > 0)
                                {
                                    ColorizeWord(xiWordStart, xsWord, false, true);
                                    xiWordStart = -1;
                                    xsWord = "";
                                }
                            }
                            else
                            {
                                if (ALLOWABLE_KEYWORD_AND_VARIABLE_CHARS.IndexOf(xsCh.ToUpper()) < 0)
                                {
                                    if (xsWord.Length > 0)
                                    {
                                        ColorizeWord(xiWordStart, xsWord, false, false);
                                        xiWordStart = -1;
                                    }
                                    else
                                    {
                                        string xs = "";
                                    }
                                    xsWord = "";
                                    ColorizeWord(xiPos, xsCh, false, false);
                                    string xs2 = "";
                                    /*if (",()".IndexOf(xsCh) >= 0)
                                    {
                                        ColorizeWord(xiPos, xsCh, false, false);
                                    }*/
                                }
                                else
                                {
                                    if (xsWord.Length == 0) xiWordStart = xiPos;
                                    xsWord += xsCh;
                                }
                            }
                        }

                    }
                }
                xiPos++;
            }

            if (xsWord.Length > 0) ColorizeWord(xiWordStart, xsWord, xbInQt, false);

            txtSQL.Visible = true;
            if (xbRefocus) txtSQL.Focus();
            txtSQL.SelectionStart = xiOrgPos;
            //DateTime xdFinish = DateTime.Now;
            //txtSQL.ResumeLayout();

            //TimeSpan xoSpan = xdFinish - xdStart;
            //this.Text = xoSpan.ToString();

            EditorProgrammaticChangeUnderway = false;
        }

        private void txtSQL_MouseUp(object sender, MouseEventArgs e)
        {
            if (EditorColorizingOn)
            {
                int xiStart, xiLen;
                bool xbOth;
                bool xbInline = false;
                //EditorInQuote = IsCurrentCursorPositionInCommentOrQuote(false, out xiStart, out xiLen, out xbInline, out xbOth);
                EditorInComment = IsCurrentCursorPositionInCommentOrQuote(true, out xiStart, out xiLen, out xbInline, out xbOth);
                EditorInQuote = xbOth;
                //if (EditorInComment) EditorInQuote = false;
                ////this.Text = DateTime.Now.ToString() + "     InQuote=" + EditorInQuote.ToString() + "   InComment=" + EditorInComment.ToString();
                EditorInCommentSingleLine = xbInline;
            }
        }

        private void txtSQL_KeyUp(object sender, KeyEventArgs e)
        {
            /*if (EditorColorizingOn)
            {
                if (EditorReformatFromThisPositionToEnd >= 0)
                {
                    int xiPos = EditorReformatFromThisPositionToEnd;
                    EditorReformatFromThisPositionToEnd = -1;
                    EditorReformatFromPosition(xiPos);
                    EditorForceFontMatching = false;
                }
                else
                {
                    if (EditorNeedReformatFromCurrentPostionToEnd)
                    {
                        EditorNeedReformatFromCurrentPostionToEnd = false;
                        int xiStart, xiLen;
                        bool xbInline = false;
                        EditorInQuote = IsCurrentCursorPositionInQuote(out xiStart, out xiLen);
                        EditorInComment = IsCurrentCursorPositionInComment(out xiStart, out xiLen, out xbInline);
                        EditorInCommentSingleLine = xbInline;
                        ReformatFromCurrentPostionToEnd();
                        EditorForceFontMatching = false;
                    }
                }
            }*/
        }

        private void tsmColorizing_Click(object sender, EventArgs e)
        {
            EditorColorizingOn = !EditorColorizingOn;
            if (txtSQL.Text.Length > 0)
            {
                if (EditorColorizingOn)
                {
                    ColorizeSQL();
                }
                else
                {
                    int xiOrg = txtSQL.SelectionStart;
                    ColorizeText(0, txtSQL.MaxLength, RegularColor, txtSQL.SelectionStart);
                }
            }
        }

        private void tstGetRubyStyleFKs_Click(object sender, EventArgs e)
        {
            const string xcsTitle = "Get Foreign Keys";
            if (Global.ShowMessage("This will examine the underlying database and attempt to find all foreign keys in the Ruby/Rails convention, and optionally store those for later use for this database to attach child windows.  Continue?", xcsTitle, MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                string xsConnStr = moDB.Connection.Info;
                RebusData6.DatabaseForeignKeys xo = new DatabaseForeignKeys();
                Cursor xoOrgCursor = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;
                if (xo.GetForeignKeysFromRubyStyleSqlServerDatabase("", "", "", "", xsConnStr))
                {
                    Cursor.Current = xoOrgCursor;
                    if (xo.ForeignKeys.Count > 0)
                    {
                        string xsMsg = "";
                        foreach (RebusData6.ForeignKey xoFK in xo.ForeignKeys)
                        {
                            if (xsMsg.Length > 0) xsMsg += "\r\n";
                            xsMsg += xoFK.TableA.Table + "." + xoFK.TableA.Column + " -> " + xoFK.TableB.Table + "." + xoFK.TableB.Column;
                        }
                        if (Global.ShowMessage(xsMsg += "\r\n\r\nSave these for later use to link as child windows?", xcsTitle, MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            // insert into ForeignKeys if not already there
                            Cursor.Current = Cursors.WaitCursor;
                            string xsErrMsg = "";
                            DB xoAppDB = new DB();
                            if (Global.OpenThisAppsDatabase(ref xoAppDB))
                            {
                                int xiAdded = 0, xiAlreadyOnFile = 0;
                                foreach (RebusData6.ForeignKey xoFK in xo.ForeignKeys)
                                {
                                    string xsTable = xoFK.TableA.Table;
                                    string xsTableB = xoFK.TableB.Table;
                                    string xsColm = xoFK.TableA.Column;
                                    string xsColmB = xoFK.TableB.Column;
                                    DataTable xoTbl = new DataTable();
                                    string xsSQL = "SELECT * FROM ForeignKeys WHERE databaseID = " + miDbID + " AND tableA = '" + xsTable + "' AND tableB = '" + xsTableB + "' AND fieldA = '" + xsColm + "' AND fieldB = '" + xsColmB + "'";
                                    if (xoAppDB.SQL(xsSQL, xoTbl))
                                    {
                                        if (xoTbl.Rows.Count == 0)
                                        {
                                            xsSQL = "INSERT INTO ForeignKeys (databaseID, tableA, tableB, fieldA, fieldB) VALUES (";
                                            xsSQL += miDbID.ToString();
                                            xsSQL += ",'" + xsTable + "'";
                                            xsSQL += ", '" + xsTableB + "'";
                                            xsSQL += ", '" + xsColm + "'";
                                            xsSQL += ", '" + xsColmB + "')";
                                            if (!xoAppDB.SQL(xsSQL)) xsErrMsg = xoAppDB.Message; else xiAdded++;
                                        }
                                        else
                                        {
                                            xiAlreadyOnFile++;
                                        }
                                    }
                                    else
                                    {
                                        xsErrMsg = xoAppDB.Message;
                                    }
                                    xoTbl.Dispose();
                                    xoTbl = null;
                                    if (xsErrMsg.Length > 0) break;
                                }
                                Cursor.Current = xoOrgCursor;
                                Global.ShowMessage(xsErrMsg.Length > 0 ? xsErrMsg : "Success.  " + xiAdded.ToString() + " added, " + xiAlreadyOnFile.ToString() + " already on file.", xcsTitle);
                            }
                            Cursor.Current = xoOrgCursor;
                            Global.CloseThisAppsDatabase(ref xoAppDB);
                        }
                    }
                    else
                    {
                        Global.ShowMessage("No foreign keys in the ruby/rails convention were found.", xcsTitle);
                    }
                }
                else
                {
                    Cursor.Current = xoOrgCursor;
                    Global.ShowMessage(xo.Message, xcsTitle);
                }
                xo = null;
            }
        }

        private bool GetDateTimeFormatString(out string psValue, out string psErrMsg)
        {
            string xsErrMsg = "", xsValue = "G";

            Global.RetrieveSetting("FormatString", "DateTime", out xsValue, out xsErrMsg);

            psValue = xsValue;
            psErrMsg = xsErrMsg;
            return (xsErrMsg.Length == 0);
        }

        private bool SaveDateTimeFormatString(string psValue, out string psErrMsg)
        {
            Global.StoreSetting("FormatString", "DateTime", psValue, out psErrMsg);
            return (psErrMsg.Length == 0);
        }

        private void tsmDateTimeFormatString_Click(object sender, EventArgs e)
        {
            string xsValue = "", xsErrMsg = "";
            GetDateTimeFormatString(out xsValue, out xsErrMsg);
            if (Global.InputBox("DateTime Format String", "Enter .NET date/time format string:", ref xsValue) == DialogResult.OK)
            {
                if (xsValue.Trim().Length > 0)
                {
                    SaveDateTimeFormatString(xsValue, out xsErrMsg);
                    if (!SetGridDateTimeColumnsFormat(xsValue, out xsErrMsg))
                    {
                        Global.ShowMessage(xsErrMsg, "Set Date/Time Format String");
                    }
                }
            }
        }
    }

    class Keyword
    {
        public string Word;
        public bool IsFunction;

        public Keyword(string psKeyword, bool pbIsFunction)
        {
            Word = psKeyword;
            IsFunction = pbIsFunction;
        }
    }
}
