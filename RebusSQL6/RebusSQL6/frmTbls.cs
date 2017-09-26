using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RebusData6;

namespace RebusSQL6
{
    public enum DatabaseObjectType { Tables, Views, Procedures, ProcedureParameters }

    public partial class frmTbls : RebusSQL6.frmBaseMDI
    {
        private int miDbID;
        public int DbID { get { return miDbID; } set { miDbID = value; } }

        private DB moDB;
        public DB DB { get { return moDB; } set { moDB = value; } }

        public string Password { get; set; }

        private DataTable moTbl;
        private bool mbNeedInit = true;

        private int miTopStyle = -1;

        public DatabaseObjectType DatabaseObjectTypeShown { get; set; }



        public frmTbls()
        {
            InitializeComponent();
            DatabaseObjectTypeShown = DatabaseObjectType.Tables;
            moDB = null;
            moTbl = null;
        }

        private void frmTbls_FormClosing(object sender, FormClosingEventArgs e)
        {
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

        private void frmTbls_Shown(object sender, EventArgs e)
        {
            if (mbNeedInit)
            {
                mbNeedInit = false;
                List<string> xsObjs = new List<string>(0);
                string xsSortColm = "TABLE_NAME";
                if (DatabaseObjectTypeShown == DatabaseObjectType.Views)
                {
                    xsObjs = moDB.GetDatabaseObjectsFromSchema("Views");
                    xsSortColm = "TABLE_NAME";
                    this.Text = "Database Views";
                }
                else
                {
                    if (DatabaseObjectTypeShown == DatabaseObjectType.Procedures)
                    {
                        xsObjs = moDB.GetDatabaseObjectsFromSchema("Procedures");
                        xsSortColm = "SPECIFIC_NAME";
                        this.Text = "Database Procedures";
                    }
                    else
                    {
                        if (DatabaseObjectTypeShown == DatabaseObjectType.ProcedureParameters)
                        {
                            xsObjs = moDB.GetDatabaseObjectsFromSchema("ProcedureParameters");
                            xsSortColm = "SPECIFIC_NAME";
                            this.Text = "Database Procedure Parameters";
                        }
                        else
                        {
                            statusLbl.Text = "Double-click a table name to launch an SQL window for it.";
                            xsObjs = moDB.GetTables();
                        }
                    }
                }
                
                if (moDB.Message.Length == 0)
                {
                    moDB.ToGridFromList(grd, xsObjs);
                    for (int xii = 0; xii < grd.Columns.Count; xii++)
                    {
                        string xs = grd.Columns[xii].HeaderText.ToUpper();
                        if (xs == xsSortColm)
                        {
                            grd.Sort(grd.Columns[xii], ListSortDirection.Ascending);
                            break;
                        }
                    }
                    RestoreColumnWidths();
                }
                else
                {
                    statusLbl.Text = moDB.Message;
                }
            }
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

        private void grd_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            string xsCaption = e.Column.HeaderText.Trim();
            int xiWidth = Convert.ToInt16(e.Column.Width);

            string xsErrMsg = "";
            Global.StoreColumnWidth(xsCaption, xiWidth, out xsErrMsg);
        }

        private void grd_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string xsTbl = "";
            bool xbOnTable = true;


            try
            {
                xsTbl = grd.CurrentCell.Value.ToString().Trim();
            }
            catch { xbOnTable = false; }

            if (xbOnTable && miTopStyle < 0)
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

            string xsTop = "";
            if (miTopStyle >= 1)
            {
                if (xsTbl.Length > 0 && Global.SelectTop <= 0)
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
                    //if (moDB.Connection.Provider.IsAccess) xsTop = "TOP "  + Global.SelectTop.ToString();
                    if (miTopStyle == 1) xsTop = "TOP(" + Global.SelectTop.ToString() + ")";
                    if (miTopStyle == 2) xsTop = "TOP " + Global.SelectTop.ToString();
                }
            }

            if (xbOnTable)
            {
                if (DatabaseObjectTypeShown == DatabaseObjectType.Procedures || DatabaseObjectTypeShown == DatabaseObjectType.Views)
                {
                    string xsSQL = "SELECT definition FROM sys.sql_modules WHERE object_id = (OBJECT_ID(N'" + xsTbl + "'))";
                    frmMain xoMain = (frmMain)this.MdiParent;
                    xoMain.CreateNewSQLWindow(xsTbl, xsSQL);
                    statusLbl.Text = "";
                }
                else
                {
                    xsTop = xsTop.Trim();
                    if (xsTop.Length > 0) xsTop = " " + xsTop;
                    string xsSQL = "SELECT" + xsTop + " * FROM [" + xsTbl + "]";

                    // user double-clicked a cell with a table name, let's fire up a new SQL window with it
                    frmMain xoMain = (frmMain)this.MdiParent;
                    xoMain.CreateNewSQLWindow(xsTbl, xsSQL);
                    statusLbl.Text = "";
                }
            }
        }

        private void frmTbls_Load(object sender, EventArgs e)
        {
            grd.Top = 0;
            grd.Left = 0;
            ResizeGrid();
        }

        private void ResizeGrid()
        {
            grd.Width = this.Width - 16;
            grd.Height = statusStrip1.Top;
        }

        private void frmTbls_ResizeEnd(object sender, EventArgs e)
        {
            ResizeGrid();
        }

        private void popupMenu_Opening(object sender, CancelEventArgs e)
        {
            menuItemShowViews.Enabled = (moDB.Connection.Provider.IntrinsicProvider == IntrinsicProviderType.SqlServer);
            menuItemShowProcs.Enabled = (moDB.Connection.Provider.IntrinsicProvider == IntrinsicProviderType.SqlServer);
            menuItemProcParams.Enabled = (moDB.Connection.Provider.IntrinsicProvider == IntrinsicProviderType.SqlServer);
        }

        private void menuItemShowViews_Click(object sender, EventArgs e)
        {
            frmMain xoParent = (frmMain)this.MdiParent;
            xoParent.CreateNewTablesWindow(DatabaseObjectType.Views);
            xoParent = null;
        }

        private void menuItemShowProcs_Click(object sender, EventArgs e)
        {
            frmMain xoParent = (frmMain)this.MdiParent;
            xoParent.CreateNewTablesWindow(DatabaseObjectType.Procedures);
            xoParent = null;
        }

        private void menuItemProcParams_Click(object sender, EventArgs e)
        {
            frmMain xoParent = (frmMain)this.MdiParent;
            xoParent.CreateNewTablesWindow(DatabaseObjectType.ProcedureParameters);
            xoParent = null;
        }
    }
}
