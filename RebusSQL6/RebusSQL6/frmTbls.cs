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
        private const string COLUMNS_NODE_TEXT = "Columns";
        private const string INDICES_NODE_TEXT = "Indices";

        private int miDbID;
        public int DbID { get { return miDbID; } set { miDbID = value; } }

        private DB moDB;
        public DB DB { get { return moDB; } set { moDB = value; } }

        public bool TreeViewMode { get; set; }

        public string Password { get; set; }

        private DataTable moTbl;
        private bool mbNeedInit = true;

        private int miTopStyle = -1;
        private string LastMatchText = "";
        //private bool ShowAllIdxsOnDblClick = false;

        public DatabaseObjectType DatabaseObjectTypeShown { get; set; }



        public frmTbls()
        {
            InitializeComponent();
            DatabaseObjectTypeShown = DatabaseObjectType.Tables;
            moDB = null;
            moTbl = null;
            TreeViewMode = false;
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
                            if (TreeViewMode)
                            {
                                grd.Visible = false;

                                trvw.Top = 0;
                                trvw.Left = 0;
                                trvw.Width = this.Width - 16;
                                trvw.Height = statusStrip1.Top;

                                LoadTreeView();

                                trvw.Visible = true;
                            }
                            else
                            {
                                xsObjs = moDB.GetTables();
                            }
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

        private void LoadTreeView(TreeNodeDT poNodeToRefresh = null)
        {
            Cursor xoOrgCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            bool xbSingleRefresh = false;
            string xsRefreshTable = "", xsRefreshSchema = "";

            if (poNodeToRefresh != null)
            {
                xsRefreshSchema = SplitSchemaFromTableName(CleanTableNameOnNode(poNodeToRefresh.NodeType == TreeNodeDT.TypeDT.Table ? poNodeToRefresh : poNodeToRefresh.Parent), out xsRefreshTable);
                xbSingleRefresh = true;
            }

            List<string> xsTbls = moDB.GetTables();
            List<string> xsTbls2 = new List<string>(0);

            if (!xbSingleRefresh) trvw.Nodes.Clear();

            if (xsTbls.Count >= 2)      // first line is columns, remaining lines are actually a row for each table in the database
            {
                string xsColms = xsTbls[0].Trim();

                if (xsColms.IndexOf("\t") >= 0)
                {
                    //
                    // process the first row, which is actually the column names for returned table of tables in GetTables()
                    // (we are trying to determine which column contains the table name, and if supplied by the provider, the schema name for each table)
                    //
                    string[] xsSep = { "\t" };
                    string[] xsVals = xsColms.Split(xsSep, StringSplitOptions.None);
                    int xiTableNameColumn = -1, xiSchemaNameColumn = -1;
                    for (int xiPass = 1; xiPass <= 2; xiPass++)
                    {
                        for (int xii = 0; xii < xsVals.Length; xii++)
                        {
                            if (xiPass == 1)
                            {
                                if (xsVals[xii].ToUpper() == "TABLE_NAME")
                                {
                                    xiTableNameColumn = xii;
                                }
                                else
                                {
                                    if (xsVals[xii].ToUpper() == "TABLE_SCHEMA")
                                    {
                                        xiSchemaNameColumn = xii;
                                    }
                                }
                            }
                            else
                            {
                                string xs = xsVals[xii].ToUpper();
                                if (xs.IndexOf("NAME") >= 0)
                                {
                                    xiTableNameColumn = xii;
                                    break;
                                }
                            }
                        }
                    }

                    //
                    // next, we can process each table, and build
                    // us a list of all tables in the database
                    //
                    for (int xii = 1; xii < xsTbls.Count; xii++)
                    {
                        string xsSchema = "";
                        string xsTbl = xsTbls[xii];
                        string xsTblOnly = "";
                        if (xiTableNameColumn >= 0)
                        {
                            xsVals = xsTbl.Split(xsSep, StringSplitOptions.None);
                            try
                            {
                                xsTbl = xsVals[xiTableNameColumn].Trim();
                                xsTblOnly = xsTbl;
                                if (xiSchemaNameColumn >= 0)
                                {
                                    xsSchema = xsVals[xiSchemaNameColumn].Trim();
                                    if (xsSchema.Length > 0)
                                        xsTbl = xsSchema + "." + xsTbl;
                                }
                            }
                            catch
                            {
                                xsTbl = "";
                            }
                        }
                        if (xbSingleRefresh)
                        {
                            if (xsTblOnly.Trim().ToUpper() == xsRefreshTable.Trim().ToUpper() && xsSchema.Trim().ToUpper() == xsRefreshSchema.Trim().ToUpper())
                                xsTbls2.Add(xsTbl);
                        }
                        else
                        {
                            xsTbls2.Add(xsTbl);
                        }
                    }
                }
            }           // tables >= 2  (actually at least one table in DB, if this is >=2, since first "tbl" is the column definition for the returned table(s) list)


            //
            // sort the table names
            //
            if (xsTbls2.Count >= 2) xsTbls2.Sort();

            TreeNodeDT xoMainNode = new TreeNodeDT("Tables", TreeNodeDT.TypeDT.Main);

            //
            // get all columns for all tables in the database
            // (or a single table if only refreshing for such)
            //
            List<SchemaData> xoColmsAll = new List<SchemaData>(0);
            List<string> xsColmsAll = moDB.GetStructure(xbSingleRefresh ? xsRefreshTable : null);
            int xiTableNameColmsColm = -1;
            int xiColumnNameColmsIdx = -1;
            int xiSchemaNameColmsIdx = -1;
            if (xsColmsAll.Count > 0)
            {
                xiTableNameColmsColm = ColumnIdxFromDelimTabString(xsColmsAll[0], "TABLE_NAME");
                xiColumnNameColmsIdx = ColumnIdxFromDelimTabString(xsColmsAll[0], "COLUMN_NAME");
                xiSchemaNameColmsIdx = ColumnIdxFromDelimTabString(xsColmsAll[0], "TABLE_SCHEMA");
            }
            bool xbColmsAll = (xiTableNameColmsColm >= 0 && xiColumnNameColmsIdx >= 0);
            if (xbColmsAll)
            {
                for (int xii = 1; xii < xsColmsAll.Count; xii++)
                {
                    string[] xsSep = { "\t" };
                    string[] xsVals = xsColmsAll[xii].Split(xsSep, StringSplitOptions.None);
                    string xsData = xsVals[xiColumnNameColmsIdx];
                    string xsData2 = "";
                    string xsSchema = "";
                    if (xiSchemaNameColmsIdx >= 0)
                        xsSchema = xsVals[xiSchemaNameColmsIdx];
                    xoColmsAll.Add(new SchemaData(xsVals[xiTableNameColmsColm], xsData, xsData2, xsSchema));
                }
            }

            //
            // get all indices for all tables in the database
            // (or a single table if only refreshing for such)
            //
            List<SchemaData> xoIdxsAll = new List<SchemaData>(0);
            List<string> xsIdxsAll = moDB.GetIndices(xbSingleRefresh ? xsRefreshTable : null);
            int xiTableNameIdxsColm = -1;
            int xiColumnNameIdxsIdx = -1;
            if (xsIdxsAll.Count > 0)
            {
                xiTableNameIdxsColm = ColumnIdxFromDelimTabString(xsIdxsAll[0], "TABLE_NAME");
                xiColumnNameIdxsIdx = ColumnIdxFromDelimTabString(xsIdxsAll[0], "INDEX_NAME");
                xiSchemaNameColmsIdx = ColumnIdxFromDelimTabString(xsColmsAll[0], "TABLE_SCHEMA");
            }
            bool xbIdxsAll = (xiTableNameIdxsColm >= 0 && xiColumnNameIdxsIdx >= 0);
            if (xbIdxsAll)
            {
                for (int xii = 1; xii < xsIdxsAll.Count; xii++)
                {
                    string[] xsSep = { "\t" };
                    string[] xsVals = xsIdxsAll[xii].Split(xsSep, StringSplitOptions.None);
                    string xsSchema = "";
                    if (xiSchemaNameColmsIdx >= 0)
                        xsSchema = xsVals[xiSchemaNameColmsIdx];
                    xoIdxsAll.Add(new SchemaData(xsVals[xiTableNameIdxsColm], xsVals[xiColumnNameIdxsIdx], "", xsSchema));
                }
            }

            //
            // build a node for each table, with a child node for columns and a child node for indices
            // (or prep to refresh a single table node)
            //
            for (int xii = 0; xii < xsTbls2.Count; xii++)
            {
                string xsSchema = "";
                string xsTbl = xsTbls2[xii];
                int xi = xsTbl.IndexOf("\t");

                TreeNodeDT xoTableNode = new TreeNodeDT(xsTbl, TreeNodeDT.TypeDT.Table);

                xi = xsTbl.IndexOf(".");
                if (xi > 0)
                {
                    xsSchema = xsTbl.Substring(0, xi);
                    xsTbl = xsTbl.Substring(xi + 1);
                }

                TreeNodeDT xoColmsNode = new TreeNodeDT(COLUMNS_NODE_TEXT, TreeNodeDT.TypeDT.Columns);

                //
                // add columns for table
                //
                List<string> xsColumns = new List<string>(0);
                if (xbColmsAll)
                {
                    string xsTableUpCase = xsTbl.Trim().ToUpper();
                    for (int xi2 = 0; xi2 < xoColmsAll.Count; xi2++)
                    {
                        if (xoColmsAll[xi2].Table == xsTableUpCase)
                        {
                            bool xbOK = true;
                            
                            if (xiSchemaNameColmsIdx >= 0 && xsSchema.Length > 0)
                            {
                                if (xsSchema.ToUpper() != xoColmsAll[xi2].Schema.ToUpper())
                                    xbOK = false;
                            }

                            if (xbOK)
                                xsColumns.Add(xoColmsAll[xi2].Data);
                        }
                    }
                }
                else
                {
                    List<string> xsColumnsTabbed = moDB.GetStructure(xsTbl);
                    if (xsColumnsTabbed.Count >= 2)
                    {
                        string[] xsSep = { "\t" };
                        int xiColumnNameIdx = ColumnIdxFromDelimTabString(xsColumnsTabbed[0], "COLUMN_NAME");
                        if (xiColumnNameIdx >= 0)
                        {
                            for (int xi2 = 1; xi2 < xsColumnsTabbed.Count; xi2++)
                            {
                                string[] xsVals = xsColumnsTabbed[xi2].Split(xsSep, StringSplitOptions.None);
                                xsColumns.Add(xsVals[xiColumnNameIdx]);
                            }
                        }
                        else
                        {
                            for (int xi2 = 1; xi2 < xsColumnsTabbed.Count; xi2++)
                            {
                                xsColumns.Add(xsColumnsTabbed[xi2]);
                            }
                        }
                    }
                }
                if (xsColumns.Count >= 1)
                {
                    if (xsColumns.Count > 1) xsColumns.Sort();
                    foreach (string xsColmName in xsColumns)
                    {
                        xoColmsNode.Nodes.Add(xsColmName);
                    }

                }

                xoTableNode.Nodes.Add(xoColmsNode);

                TreeNodeDT xoIdxsNode = new TreeNodeDT(INDICES_NODE_TEXT, TreeNodeDT.TypeDT.Indices);

                //
                // add any indices for table
                //
                List<string> xsIdxs = new List<string>(0);
                if (xbIdxsAll)
                {
                    string xsTableUpCase = xsTbl.Trim().ToUpper();
                    for (int xi2 = 0; xi2 < xoIdxsAll.Count; xi2++)
                    {
                        if (xoIdxsAll[xi2].Table == xsTableUpCase)
                        {
                            bool xbOK = true;

                            if (xiSchemaNameColmsIdx >= 0 && xsSchema.Length > 0)
                            {
                                if (xsSchema.ToUpper() != xoIdxsAll[xi2].Schema.ToUpper())
                                    xbOK = false;
                            }

                            if (xbOK)
                                xsIdxs.Add(xoIdxsAll[xi2].Data);
                        }
                    }
                }
                else
                {
                    List<string> xsIdxsTabbed = moDB.GetIndices(xsTbl);
                    if (xsIdxsTabbed.Count >= 2)
                    {
                        string[] xsSep = { "\t" };
                        int xiIdxNameIdx = ColumnIdxFromDelimTabString(xsIdxsTabbed[0], "COLUMN_NAME");
                        if (xiIdxNameIdx >= 0)
                        {
                            for (int xi2 = 1; xi2 < xsIdxsTabbed.Count; xi2++)
                            {
                                string[] xsVals = xsIdxsTabbed[xi2].Split(xsSep, StringSplitOptions.None);
                                xsIdxs.Add(xsVals[xiIdxNameIdx]);
                            }
                        }
                        else
                        {
                            for (int xi2 = 1; xi2 < xsIdxsTabbed.Count; xi2++)
                            {
                                xsIdxs.Add(xsIdxsTabbed[xi2]);
                            }
                        }
                    }
                }
                if (xsIdxs.Count >= 1)
                {
                    if (xsIdxs.Count > 1) xsIdxs.Sort();
                    foreach (string xsIdxName in xsIdxs)
                    {
                        xoIdxsNode.Nodes.Add(xsIdxName);
                    }
                }
                xoTableNode.Nodes.Add(xoIdxsNode);

                //
                // add the now completed node for this table
                //
                if (xbSingleRefresh)
                {
                    //
                    // replace when refreshing
                    //
                    if (poNodeToRefresh.NodeType == TreeNodeDT.TypeDT.Table)
                    {
                        int xiIdx = poNodeToRefresh.Index;
                        bool xbIsExpanded = poNodeToRefresh.IsExpanded;
                        bool xbFirstIsExpanded = poNodeToRefresh.FirstNode.IsExpanded;
                        bool xbLastIsExpanded = poNodeToRefresh.LastNode.IsExpanded;
                        TreeNodeDT xoParent = (TreeNodeDT)poNodeToRefresh.Parent;
                        xoParent.Nodes.RemoveAt(xiIdx);
                        xoParent.Nodes.Insert(xiIdx, xoTableNode);
                        trvw.SelectedNode = xoTableNode;
                        if (xbIsExpanded != xoTableNode.IsExpanded)
                        {
                            if (xbIsExpanded)
                            {
                                xoTableNode.Expand();
                            }
                            else
                            {
                                xoTableNode.Collapse();
                            }
                        }
                        try
                        {
                            if (xbFirstIsExpanded != xoTableNode.FirstNode.IsExpanded)
                            {
                                if (xbFirstIsExpanded)
                                {
                                    xoTableNode.FirstNode.Expand();
                                }
                                else
                                {
                                    xoTableNode.FirstNode.Collapse();
                                }
                            }
                            if (xbLastIsExpanded != xoTableNode.LastNode.IsExpanded)
                            {
                                if (xbLastIsExpanded)
                                {
                                    xoTableNode.LastNode.Expand();
                                }
                                else
                                {
                                    xoTableNode.LastNode.Collapse();
                                }
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        if (poNodeToRefresh.NodeType == TreeNodeDT.TypeDT.Columns)
                        {
                            int xiIdx = poNodeToRefresh.Index;
                            bool xbIsExpanded = poNodeToRefresh.IsExpanded;
                            TreeNodeDT xoParent = (TreeNodeDT)poNodeToRefresh.Parent;
                            xoParent.Nodes.RemoveAt(xiIdx);
                            xoParent.Nodes.Insert(xiIdx, xoColmsNode);
                            trvw.SelectedNode = xoColmsNode;
                            if (xbIsExpanded != xoColmsNode.IsExpanded)
                            {
                                if (xbIsExpanded)
                                {
                                    xoColmsNode.Expand();
                                }
                                else
                                {
                                    xoColmsNode.Collapse();
                                }
                            }
                        }
                        else
                        {
                            if (poNodeToRefresh.NodeType == TreeNodeDT.TypeDT.Indices)
                            {
                                int xiIdx = poNodeToRefresh.Index;
                                bool xbIsExpanded = poNodeToRefresh.IsExpanded;
                                TreeNodeDT xoParent = (TreeNodeDT)poNodeToRefresh.Parent;
                                xoParent.Nodes.RemoveAt(xiIdx);
                                xoParent.Nodes.Insert(xiIdx, xoIdxsNode);
                                trvw.SelectedNode = xoIdxsNode;
                                if (xbIsExpanded != xoIdxsNode.IsExpanded)
                                {
                                    if (xbIsExpanded)
                                    {
                                        xoIdxsNode.Expand();
                                    }
                                    else
                                    {
                                        xoIdxsNode.Collapse();
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    //
                    // otherwise, add to end
                    //
                    xoMainNode.Nodes.Add(xoTableNode);
                }
            }

            if (!xbSingleRefresh)
            {
                // add the top level (Tables) node...
                trvw.Nodes.Add(xoMainNode);

                // and expand it
                xoMainNode.Expand();
            }

            Cursor.Current = xoOrgCursor;
        }

        private int ColumnIdxFromDelimTabString(string ps, string psColmName)
        {
            int xiColmIdx = -1;
            string[] xsSep = { "\t" };

            string[] xsNames = ps.Split(xsSep, StringSplitOptions.None);

            for (int xii = 0; xii < xsNames.Length; xii++)
            {
                if (xsNames[xii].Trim().ToUpper() == psColmName.Trim().ToUpper())
                {
                    xiColmIdx = xii;
                    break;
                }
            }

            return (xiColmIdx);
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

            CreateNewSQLWindowForTable(xbOnTable, xsTbl);
        }

        private void CreateNewSQLWindowForTable(bool pbOnTable, string psTable, bool pbAllRows = false, string psOverrideSQL = "", string psCaption = "", bool pbAsync = false)
        {
            if (pbOnTable && miTopStyle < 0)
            {
                miTopStyle = 0;
                DataTable xoTbl = new DataTable();
                if (moDB.SQL("SELECT TOP(1) * FROM " + psTable, xoTbl))
                {
                    miTopStyle = 1;
                }
                else
                {
                    if (moDB.SQL("SELECT TOP 1 * FROM " + psTable, xoTbl)) miTopStyle = 2;
                }
                xoTbl.Dispose();
                xoTbl = null;
            }

            string xsTop = "";
            if (miTopStyle >= 1)
            {
                if (psTable.Length > 0 && Global.SelectTop <= 0)
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

            if (pbOnTable || psOverrideSQL.Length > 0)
            {
                if (DatabaseObjectTypeShown == DatabaseObjectType.Procedures || DatabaseObjectTypeShown == DatabaseObjectType.Views)
                {
                    string xsSQL = "SELECT definition FROM sys.sql_modules WHERE object_id = (OBJECT_ID(N'" + psTable + "'))";
                    frmMain xoMain = (frmMain)this.MdiParent;
                    //xoMain.CreateNewSQLWindow(psTable, xsSQL);
                    xoMain.CreateNewSQLWindow(psTable: psTable, psSQL: xsSQL, pbAsnyc: pbAsync);
                    statusLbl.Text = "";
                }
                else
                {
                    xsTop = xsTop.Trim();
                    if (xsTop.Length > 0) xsTop = " " + xsTop;

                    string xsColumns = "*";
                    string xsOrgColumns = xsColumns;
                    bool xbMultiLine = false;
                    if (mnuItemFldNames.Checked)
                    {
                        List<string> xoFlds = moDB.GetStructure(psTable);
                        if (moDB.Message.Length == 0 && xoFlds != null && xoFlds.Count >= 2)
                        {
                            try
                            {
                                int xiColmNameIdx = -1;
                                string[] xsItems = xoFlds[0].ToString().Split(Convert.ToChar("\t"));
                                for (int xii = 0; xii < xoFlds.Count; xii++)
                                {
                                    if (xsItems[xii].ToUpper() == "COLUMN_NAME")
                                    {
                                        xiColmNameIdx = xii;
                                        break;
                                    }
                                }
                                if (xiColmNameIdx >= 0)
                                {
                                    xsColumns = "\r\n";
                                    for (int xii = 1; xii < xoFlds.Count; xii++)
                                    {
                                        xsItems = xoFlds[xii].ToString().Split(Convert.ToChar("\t"));
                                        xsColumns = xsColumns + (xii > 1 ? ",\r\n" : "") + "   [" + xsItems[xiColmNameIdx] + "]";
                                    }
                                    xbMultiLine = true;
                                }
                            }
                            catch { xsColumns = xsOrgColumns; }
                        }
                    }

                    string xsSQL = "SELECT" + (pbAllRows ? "" : xsTop) + " " + xsColumns + (xbMultiLine ? "\r\n\r\n" : "") + " FROM " + Global.BracketizeTableName(psTable);
                    if (psOverrideSQL.Trim().Length > 0) xsSQL = psOverrideSQL;

                    // user double-clicked a cell with a table name, let's fire up a new SQL window with it
                    frmMain xoMain = (frmMain)this.MdiParent;
                    //xoMain.CreateNewSQLWindow(psTable, xsSQL, -1, -1, 0, 0, psCaption);
                    xoMain.CreateNewSQLWindow(psTable: psTable, psSQL: xsSQL, psCaption: psCaption, pbAsnyc: pbAsync);
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
            string xsErrMsg;
            Global.StoreSetting("frmTbls", "LastSize", this.Size.Width.ToString() + "," + this.Size.Height.ToString(), out xsErrMsg);
        }

        private void popupMenu_Opening(object sender, CancelEventArgs e)
        {
            if (trvw.Visible)
            {
                tsmSeleTop.Text = "SELECT TOP " + (Global.SelectTop > 0 ? Global.SelectTop.ToString() : "2500") + " rows";
            }
            tsmSeleAll.Enabled = (trvw.Visible && trvw.SelectedNode != null && trvw.SelectedNode.Level == 1);
            tsmSeleTop.Enabled = (trvw.Visible && trvw.SelectedNode != null && trvw.SelectedNode.Level == 1);
            tsmCount.Enabled = (trvw.Visible && trvw.SelectedNode != null && trvw.SelectedNode.Level == 1);

            tsmSeleDistinct.Enabled = (trvw.Visible && trvw.SelectedNode != null && trvw.SelectedNode.Level == 3 && trvw.SelectedNode.Parent.Text == COLUMNS_NODE_TEXT);
            tsmSeleDistinctCounts.Enabled = tsmSeleDistinct.Enabled;

            tsmRefresh.Enabled = (trvw.Visible && trvw.SelectedNode != null);

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

        private void mnuItemFldNames_Click(object sender, EventArgs e)
        {
            mnuItemFldNames.Checked = !mnuItemFldNames.Checked;
        }

        public string DropSquareBrackets(string ps)
        {
            string xs = ps.Trim();
            if (xs.Length >= 2)
            {
                if (xs[0] == '[' && xs[xs.Length - 1] == ']')
                {
                    xs = xs.Substring(1, xs.Length - 2);
                }
            }
            return (xs);
        }

        public string SplitSchemaFromTableName(string psTable, out string psTableOnly)
        {
            string xsTable = psTable;
            string xsSchema = "";

            int xi = 0;
            if (xsTable != null)
            {
                xsTable = xsTable.Trim();

                xi = xsTable.IndexOf(".");
                if (xi > 0 && xi < xsTable.Length - 1)
                {
                    xsSchema = DropSquareBrackets(xsTable.Substring(0, xi));
                    xsTable = xsTable.Substring(xi + 1);
                }
                xsTable = DropSquareBrackets(xsTable);
            }

            psTableOnly = xsTable;
            return (xsSchema);
        }

        private void trvw_DoubleClick(object sender, EventArgs e)
        {
            TreeNode xoNode;
            string xsTbl = SelectedNodeTableName(out xoNode);
            if (xsTbl.Length > 0)
            {
                CreateNewSQLWindowForTable(true, xsTbl);
            }
            else
            {
                xoNode = trvw.SelectedNode;
                if (xoNode != null && xoNode.Level == 3)
                {
                    if (xoNode.Parent.Text == INDICES_NODE_TEXT)
                    {
                        xsTbl = CleanTableNameOnNode(xoNode.Parent.Parent);

                        string xsTableNameOnly = "";
                        string xsSchema = SplitSchemaFromTableName(xsTbl, out xsTableNameOnly);

                        string xsSQL = "", xsCaption = "";
                        if (moDB.Connection.Provider.IsMsSQLServer)
                        {
                            bool xbIsUnique = false, xbIsPK = false, xbHaveProps = false;
                            string xsType = "", xsIdx = xoNode.Text;

                            xsSQL += "SELECT";
                            xsSQL += "  i.type_desc,";
                            xsSQL += "  i.is_unique,";
                            xsSQL += "  i.is_primary_key";
                            xsSQL += " FROM sys.Objects AS t";
                            xsSQL += "  LEFT JOIN sys.Indexes AS i ON i.object_id = t.object_id";
                            xsSQL += "   WHERE t.name = '" + xsTableNameOnly + "' AND t.type = 'U'";

                            if (!tsmShowAllIdxInfo.Checked)
                            {
                                xsSQL += " AND i.name = '" + xsIdx + "'";
                                DataTable xo = new DataTable();
                                if (moDB.SQL(xsSQL, xo))
                                {
                                    try
                                    {
                                        xsType = xo.Rows[0][0].ToString();
                                        xbIsUnique = (Convert.ToInt16(xo.Rows[0][1]) != 0);
                                        xbIsPK = (Convert.ToInt16(xo.Rows[0][2]) != 0);
                                        xbHaveProps = true;
                                    }
                                    catch { }
                                }
                                if (xo != null)
                                {
                                    try { xo.Dispose(); xo = null; } catch { }
                                }
                            }

                            xsSQL = "SELECT";
                            //xsSQL += "  t.object_id AS [Object ID],";
                            if (tsmShowAllIdxInfo.Checked)
                            {
                                xsSQL += "  t.name AS [Table],";
                                xsSQL += "  i.name AS [Index],";
                            }
                            if (!xbHaveProps)
                            {
                                xsSQL += "  i.type_desc AS [Type],";
                                xsSQL += "  i.is_unique AS [Unique],";
                                xsSQL += "  i.is_primary_key AS [Primary Key],";
                            }
                            xsSQL += "  c.name AS [Column],";
                            xsSQL += "  ic.key_ordinal AS [Ordinal],";
                            xsSQL += "  ic.is_descending_key AS [Descending]";
                            xsSQL += " FROM sys.Objects AS t";
                            xsSQL += " LEFT JOIN sys.Indexes AS i ON i.object_id = t.object_id AND i.data_space_id IN (SELECT data_space_id FROM sys.Data_Spaces WHERE is_default <> 0)";
                            xsSQL += " RIGHT JOIN sys.Index_Columns AS ic ON ic.object_id = t.object_id AND ic.index_id = i.index_id";
                            xsSQL += " LEFT JOIN sys.Columns AS c ON c.object_id = ic.object_id AND c.column_id = ic.column_id";
                            xsSQL += "  WHERE t.name = '" + xsTableNameOnly + "' AND t.type = 'U' AND ic.is_included_column = 0";
                            if (!tsmShowAllIdxInfo.Checked) xsSQL += "    AND i.name = '" + xsIdx + "'";
                            xsSQL += "   ORDER BY t.name, i.name, ic.index_column_id";
                            xsCaption = "Table: " + xsTbl;
                            if (tsmShowAllIdxInfo.Checked)
                            {
                                xsCaption += "  (ALL Indices)";
                            }
                            else
                            {
                                xsCaption += "   Index: " + xsIdx;
                            }
                            if (xbHaveProps)
                            {
                                xsCaption += "  Type: " + xsType;
                                if (xbIsPK)
                                {
                                    xsCaption += "  (primary key)";
                                }
                                else
                                {
                                    xsCaption += "  (" + (xbIsUnique ? "" : "not ") + "unique)";
                                }
                            }
                        }
                        CreateNewSQLWindowForTable(false, "", true, xsSQL, xsCaption);
                    }
                }
            }
        }

        private void tsmSeleTop_Click(object sender, EventArgs e)
        {
            TreeNode xoNode;
            string xsTbl = SelectedNodeTableName(out xoNode);
            if (xsTbl.Length > 0)
                CreateNewSQLWindowForTable(true, xsTbl);
        }

        private void tsmSeleAll_Click(object sender, EventArgs e)
        {
            TreeNode xoNode;
            string xsTbl = SelectedNodeTableName(out xoNode);
            if (xsTbl.Length > 0)
                CreateNewSQLWindowForTable(true, xsTbl, true, "", "", (moDB.Connection.Connectivity == ConnectivityType.DotNet));
        }

        private void tsmCount_Click(object sender, EventArgs e)
        {
            CountRows();
        }

        private string SelectedNodeTableName(out TreeNode poNode)
        {
            string xsTable = "";
            TreeNode xoNode = null;

            if (trvw.SelectedNode != null && trvw.SelectedNode.Level == 1)
            {
                if (trvw.SelectedNode.Text != null)
                {
                    xoNode = trvw.SelectedNode;
                    xsTable = CleanTableNameOnNode(xoNode); // trvw.SelectedNode.Text;
                }
            }

            poNode = xoNode;
            return (xsTable.Trim());
        }

        private string CleanTableNameOnNode(TreeNode poNode)
        {
            string xsTable = poNode.Text;
            int xi = xsTable.IndexOf("\t");
            if (xi >= 1) xsTable = xsTable.Substring(0, xi);
            return (xsTable);
        }

        private void CountRows()
        {
            Cursor xoOrgCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            TreeNode xoNode;
            string xsTbl = SelectedNodeTableName(out xoNode);
            if (xsTbl.Length > 0)
            {
                string xsMsg = "";
                DataTable xoTbl = new DataTable();
                if (moDB.SQL("SELECT COUNT(*) FROM " + Global.BracketizeTableName(xsTbl), xoTbl))
                {
                    if (xoTbl.Rows.Count > 0)
                    {
                        long xiRows = Convert.ToInt64(xoTbl.Rows[0][0]);
                        //xsMsg = "Total # of rows: " + xiRows.ToString("###,###,###,###,##0");
                        xoNode.Text = xsTbl + "\t (" + xiRows.ToString("###,###,###,###,##0") + " row" + (xiRows > 1 ? "s" : "") + ")";
                    }
                    else
                    {
                        xsMsg = "No rows returned.";
                    }
                }
                else
                {
                    xsMsg = moDB.Message;
                }
                if (xsMsg.Length > 0) Global.ShowMessage(xsMsg, "Count: " + xsTbl);
            }

            Cursor.Current = xoOrgCursor;
        }

        private void tsmSeleDistinct_Click(object sender, EventArgs e)
        {
            SelectDistinct();
        }

        private void SelectDistinct(bool pbWithCounts = false)
        {
            if (trvw.SelectedNode.Level == 3 && trvw.SelectedNode.Parent.Text == COLUMNS_NODE_TEXT)
            {
                string xsColm = trvw.SelectedNode.Text.Trim();
                int xi = xsColm.IndexOf("(");
                if (xi > 0)
                {
                    xsColm = xsColm.Substring(0, xi);
                    xsColm = xsColm.Replace("\t", "");
                }
                string xsTbl = CleanTableNameOnNode(trvw.SelectedNode.Parent.Parent);
                string xsResultsColumn1 = "[DISTINCT Values for Column \"" + xsColm + "\"]";
                string xsSQL = "SELECT DISTINCT([" + xsColm + "]) AS " + xsResultsColumn1;
                if (pbWithCounts)
                {
                    xsSQL += ", COUNT(*) AS [# of Occurrences]";
                }
                //xsSQL += " FROM [" + xsTbl + "]";
                xsSQL += " FROM " + Global.BracketizeTableName(xsTbl);
                if (pbWithCounts)
                {
                    xsSQL += " GROUP BY " + "[" + xsColm + "]";
                }

                CreateNewSQLWindowForTable(true, xsTbl, true, xsSQL);
            }
        }

        private void tsmSeleDistinctCounts_Click(object sender, EventArgs e)
        {
            SelectDistinct(true);
        }

        private void trvw_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Level == 2 && e.Node.Tag == null)
            {
                //
                // expanding the columns of the table the first time...get more structure detail
                //
                e.Node.Tag = true;      // so we only do this once per table
                Cursor xoOrgCursor = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;
                string xsTable = CleanTableNameOnNode(e.Node.Parent);
                List<string> xsColmsAll = moDB.GetStructure(xsTable);
                if (xsColmsAll.Count >= 2)
                {
                    int xiColumnNameColmsIdx = ColumnIdxFromDelimTabString(xsColmsAll[0], "COLUMN_NAME");
                    int xiDataTypeColmIdx = ColumnIdxFromDelimTabString(xsColmsAll[0], "DATA_TYPE");
                    int xiMaxLenColmIdx = ColumnIdxFromDelimTabString(xsColmsAll[0], "CHARACTER_MAXIMUM_LENGTH");
                    if (xiColumnNameColmsIdx >= 0 && xiDataTypeColmIdx >= 0)
                    {
                        for (int xii = 1; xii < xsColmsAll.Count; xii++)
                        {
                            string[] xsSep = { "\t" };
                            string[] xsVals = xsColmsAll[xii].Split(xsSep, StringSplitOptions.None);
                            string xsColm = xsVals[xiColumnNameColmsIdx].Trim().ToUpper();
                            for (int xi2 = 0; xi2 < e.Node.Nodes.Count; xi2++)
                            {
                                string xsText = e.Node.Nodes[xi2].Text;
                                if (xsText.Trim().ToUpper() == xsColm)
                                {
                                    xsText += " (" + xsVals[xiDataTypeColmIdx];
                                    if (xiMaxLenColmIdx >= 0)
                                    {
                                        try
                                        {
                                            int xi = int.Parse(xsVals[xiMaxLenColmIdx]);
                                            if (xi > 0) xsText += "[" + xi.ToString() + "]";
                                        }
                                        catch { }
                                    }
                                    xsText += ")";
                                    e.Node.Nodes[xi2].Text = xsText;
                                }
                            }
                        }
                    }
                }
                Cursor.Current = xoOrgCursor;
            }
        }

        private void tsmHighlight_Click(object sender, EventArgs e)
        {
            TreeNode xoNode = trvw.SelectedNode;
            if (xoNode != null)
            {
                dlgFilter xoDlg = new dlgFilter(LastMatchText);
                xoDlg.ShowDialog();
                if (xoDlg.OK)
                {
                    LastMatchText = xoDlg.MatchText;
                    foreach (TreeNode xoChildNode in xoNode.Nodes)
                    {
                        if (xoChildNode.Text.ToLower().IndexOf(LastMatchText.ToLower()) >= 0)
                        {
                            xoChildNode.ForeColor = Color.Black;
                            xoChildNode.BackColor = Color.Yellow;
                        }
                        else
                        {
                            xoChildNode.ForeColor = trvw.TopNode.ForeColor;
                            xoChildNode.BackColor = trvw.TopNode.BackColor;
                        }
                    }
                }
                xoDlg.Dispose();
                xoDlg = null;
            }
        }

        private void tsmUnhighlight_Click(object sender, EventArgs e)
        {
            TreeNode xoNode = trvw.SelectedNode;
            if (xoNode != null)
            {
                foreach (TreeNode xoChildNode in xoNode.Nodes)
                {
                    xoChildNode.ForeColor = trvw.TopNode.ForeColor;
                    xoChildNode.BackColor = trvw.TopNode.BackColor;
                }
            }
        }

        private void tsmShowAllIdxInfo_Click(object sender, EventArgs e)
        {
            tsmShowAllIdxInfo.Checked = !tsmShowAllIdxInfo.Checked;
        }

        private void tsmRefresh_Click(object sender, EventArgs e)
        {
            //this.Text = trvw.SelectedNode.Level.ToString();
            if (trvw.SelectedNode.Level == 0)
            {
                LoadTreeView();
            }
            else
            {
                if (trvw.SelectedNode.Level == 1)
                {
                    LoadTreeView((TreeNodeDT)trvw.SelectedNode);
                }
                else
                {
                    if (trvw.SelectedNode.Level == 2)
                    {
                        LoadTreeView((TreeNodeDT)trvw.SelectedNode);
                    }
                }
            }
        }
    }

    internal class TreeNodeDT : TreeNode
    {
        public enum TypeDT { Main, Table, Columns, Indices, Other }
        
        public TypeDT NodeType { get; set; }


        public TreeNodeDT(string psText, TypeDT peType = TypeDT.Other) : base(psText)
        {
            NodeType = peType;
        }
    }


    internal class SchemaData
    {
        public string Table { get; set; }
        public string Data { get; set; }
        public string Data2 { get; set; }
        public string Schema { get; set; }

        public SchemaData (string psTable, string psData, string psData2 = "", string psSchema = "")
        {
            Table = psTable.Trim().ToUpper();
            Data = psData;
            Data2 = psData2;
            Schema = psSchema;
        }
    }
}
