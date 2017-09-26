using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using RebusData6;
using RebusDatabaseMigrator;

namespace RebusSQL6
{
    public struct LoadedDb
    {
        public int ID;
        public string Description;
        public ConnectivityType ConnType;
        public string ProviderName;
        public string DataSource;
        public string Database;
        public string UserID;
        public string ConnStrOverride;
        //public bool NeedPassword;
        //public Brand Brand;
    }

    public partial class frmMain : Form
    {
        private struct AnOpenDatabase
        {
            public DB DB;
            public int ID;
        }

        private bool mbNeedInit;
        private AnOpenDatabase moCurrDB;
        private bool mbMoving = false;
        private bool mbAddingNewDb = false;
        private int miCurrDb_PK_ID = 0;
        private string msCurrentRestoredDsID = "";
        private string msCurrentRestoredDsDesc = "";
        private int miCurrentRestoredDbID = 0;
        private int miMDICount = 0;
        private string msCurrDbPswd = "";
        private long miLastDbIdAdded = 0;
        private bool mbLoadingGrps = false;
        private bool mbMigrate = true;

        //private bool mbSqlServerInstancesLoaded = false;
        private List<string> msSQLs;
        private List<string> msDBs;

        private List<LoadedDb> mrLoadedDbs;

        private List<int> moChildWindowIDs;

        private List<DatabaseProvider> moExtProvs;

        public string LastSQL_Guid = "";





        public frmMain(bool pbMigrate = false)
        {
            InitializeComponent();
            mbMigrate = pbMigrate;
            mbNeedInit = true;
            mrLoadedDbs = new List<LoadedDb>(0);
            moChildWindowIDs = new List<int>(0);
            moCurrDB = new AnOpenDatabase();
            moCurrDB.DB = null;
            moCurrDB.ID = 0;
            moExtProvs = new List<DatabaseProvider>(0);
        }

        private bool CurrentDBisOpen()
        {
            bool xb = false;
            if (moCurrDB.DB != null)
            {
                xb = moCurrDB.DB.DatabaseIsOpen();
            }
            return (xb);
        }

        private void SetControlsAvails()
        {
            bool xb = CurrentDBisOpen();

            btnNewSQLwindow.Enabled = xb;
            //btnOpenDB.Enabled = drpDB.SelectedIndex >= 0;
            btnShowTable.Enabled = xb;
            //btnOpenDB.Enabled = (drpDB.SelectedIndex >= 0);
            //btnRemove.Enabled = btnOpenDB.Enabled;
            btnCloseDB.Enabled = xb;
            //btnOpenDB.Enabled = drpDB.SelectedIndex >= 0;
            //btnRemove.Enabled = drpDB.SelectedIndex >= 0;
            btnSaveSQL.Enabled = xb;

            xb = this.MdiChildren.Count() > 1;
            btnCascade.Enabled = xb;
            btnTileHorz.Enabled = xb;
            btnTileVert.Enabled = xb;
            xb = this.MdiChildren.Count() > 1;
            btnCloseAll.Enabled = xb;
            lblActiveWindow.Enabled = xb;
            drpWindows.Enabled = xb;
            lblActView.Enabled = xb;
            drpViews2.Enabled = xb;
            drpGrps.Enabled = (xb && drpGrps.Items.Count > 0);
            lblGrps.Enabled = drpGrps.Enabled;
        }

        private void ReleaseTable(ref DataTable poTbl)
        {
            if (poTbl != null)
            {
                try
                {
                    poTbl.Dispose();
                }
                catch (Exception xoExc)
                {
                    string xs = xoExc.Message;
                }
                poTbl = null;
            }
        }

        /// <summary>
        /// this loads the drop-down list of databases previously opened, so the operator can quickly re-open
        /// </summary>
        private void LoadDBs()
        {
            //drpDB.Items.Clear();
            mrLoadedDbs.Clear();
            string xsErrMsg = "";
            DB xoAppDB = new DB();
            if (Global.OpenThisAppsDatabase(ref xoAppDB))
            {
                string xsSQL = "SELECT [ID], connType, providerName, source, [database], connStr, userID, [description] FROM [database] ORDER BY [database]";
                DataTable xoTbl = new DataTable();
                if (xoAppDB.SQL(xsSQL, xoTbl))
                {
                    if (xoTbl.Rows.Count > 0)
                    {
                        for (int xiRow = 0; xiRow < xoTbl.Rows.Count; xiRow++)
                        {
                            int xiID = Convert.ToInt32(xoTbl.Rows[xiRow][0]);
                            int xiConnType = Convert.ToInt32(xoTbl.Rows[xiRow][1]);
                            string xsProvName = null2str(xoTbl.Rows[xiRow][2]);
                            string xsSrc = null2str(xoTbl.Rows[xiRow][3]);
                            string xsDB = null2str(xoTbl.Rows[xiRow][4]);
                            string xsConnStr = null2str(xoTbl.Rows[xiRow][5]);
                            string xsUserID = null2str(xoTbl.Rows[xiRow][6]);
                            //bool xbNeedPswd = false;
                            //if (xoTbl.Rows[xiRow][7] != null) xbNeedPswd = Convert.ToBoolean(xoTbl.Rows[xiRow][7]);

                            DatabaseToOpen xrDbToOpen = new DatabaseToOpen();
                            xrDbToOpen.Description = null2str(xoTbl.Rows[xiRow][7]).Trim();
                            xrDbToOpen.ConnectionType = (ConnectivityType)xiConnType;
                            xrDbToOpen.UniqueID = xiID;
                            xrDbToOpen.Provider.Name = xsProvName.Trim();
                            xrDbToOpen.Provider.ConnectionStringOverride = xsConnStr.Trim();
                            xrDbToOpen.Database = xsDB.Trim();
                            xrDbToOpen.DataSource = xsSrc.Trim();
                            xrDbToOpen.UserID = xsUserID.Trim();
                            //xrDbToOpen.NeedPassword = xsUserID.Trim().Length > 0;  // xbNeedPswd;
                            AddLoadedDB(xrDbToOpen);
                        }
                    }
                }

                //xoTbl.Clear();
                xoTbl.Reset();
                xsSQL = "SELECT [value] FROM Customs WHERE userInfo = '" + Environment.UserName + "' AND [section] = 'SQL' AND [entry] = 'SelectTop'";
                if (xoAppDB.SQL(xsSQL, xoTbl))
                {
                    if (xoTbl.Rows.Count > 0)
                    {
                        int xiTop = 0;
                        try
                        {
                            string xs = xoTbl.Rows[0][0].ToString().Trim();
                            xiTop = Convert.ToInt32(xs);
                        }
                        catch { }
                        if (xiTop > 0) Global.SelectTop = xiTop;
                    }
                }

                ReleaseTable(ref xoTbl);
            }
            else
            {
                xsErrMsg = xoAppDB.Message;
            }
            Global.CloseThisAppsDatabase(ref xoAppDB);
            SetControlsAvails();
        }

        /// <summary>
        /// add a database to the drop-down database list, and load the internal list which holds detail for each item in that list
        /// </summary>
        /// <param name="prDbToOpen"></param>
        private void AddLoadedDB(DatabaseToOpen prDbToOpen)
        {
            LoadedDb xrLoadedDb = new LoadedDb();
            xrLoadedDb.ConnType = prDbToOpen.ConnectionType;
            xrLoadedDb.ID = prDbToOpen.UniqueID;
            xrLoadedDb.ProviderName = prDbToOpen.Provider.Name;
            xrLoadedDb.ConnStrOverride = prDbToOpen.Provider.ConnectionStringOverride;
            xrLoadedDb.Database = prDbToOpen.Database;
            xrLoadedDb.DataSource = prDbToOpen.DataSource;
            xrLoadedDb.UserID = prDbToOpen.UserID;
            xrLoadedDb.Description = prDbToOpen.Description.Trim();
            mrLoadedDbs.Add(xrLoadedDb);
        }

        /// <summary>
        /// we do this ONCE per run (since it can be slow)...all calls frmOpenDB should come here first, so as to load
        /// any available SQL Server instances available to them...these are in turn passed to the frmOpenDB dialog
        /// </summary>
        private void LoadSQLServerInstances()
        {
            //if (!mbSqlServerInstancesLoaded)
            //{
                Cursor xoOrgCursor = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;

                // not working consistently, and slow - disable for now with further research in future
                //DB xoDB = new DB();
                //msSQLs = xoDB.GetSQLServersRunning();
                //xoDB = null;
                //mbSqlServerInstancesLoaded = true;

                // change to we save/restore previous entries
                msSQLs = GetHistory(1);

                Cursor.Current = xoOrgCursor;
            //}
        }

        private void LoadPrevDBs()
        {
            Cursor xoOrgCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            msDBs = GetHistory(2);

            Cursor.Current = xoOrgCursor;
        }

        private List<string> GetHistory(int piDataType)
        {
            List<string> xsSrcs = new List<string>(0);

            DB xoAppDB = new DB();
            if (Global.OpenThisAppsDatabase(ref xoAppDB))
            {
                DataTable xoTbl = new DataTable();
                if (xoAppDB.SQL("SELECT histValue FROM [History] WHERE histType = " + piDataType.ToString(), xoTbl))
                {
                    if (xoTbl.Rows.Count > 0)
                    {
                        for (int xii = 0; xii < xoTbl.Rows.Count; xii++)
                        {
                            try
                            {
                                string xs = null2str(xoTbl.Rows[xii][0]);
                                xsSrcs.Add(xs.Trim());
                            }
                            catch { }
                        }
                    }
                }
                xoTbl.Dispose();
                xoTbl = null;
            }
            xoAppDB.CloseDatabase();
            xoAppDB = null;

            return (xsSrcs);
        }

        private void SaveHistory(string psValue, int piDataType)
        {
            if (psValue.Trim().Length > 0)
            {
                DB xoAppDB = new DB();
                if (Global.OpenThisAppsDatabase(ref xoAppDB))
                {
                    bool xbAlreadySaved = false;
                    List<string> xsSrcs = GetHistory(piDataType);
                    string xsValue = psValue.ToLower();
                    for (int xii = 0; xii < xsSrcs.Count; xii++)
                    {
                        if (xsValue == xsSrcs[xii].ToLower())
                        {
                            xbAlreadySaved = true;
                            break;
                        }
                    }
                    if (!xbAlreadySaved)
                    {
                        string xsSQL = "INSERT INTO [History] (histType, histValue) VALUES (" + piDataType.ToString() + ", '" + psValue + "')";
                        if (!xoAppDB.SQL(xsSQL))
                        {
                            string xsErrMsg = xoAppDB.Message;
                            xsErrMsg = "";
                        }
                    }
                }
                xoAppDB.CloseDatabase();
                xoAppDB = null;
            }
        }

        private string GetDrpListLineItem(DatabaseToOpen poDbToOpen)
        {
            string xs = poDbToOpen.Description.TrimEnd();
            if (poDbToOpen.Provider.ConnectionStringOverride == null) poDbToOpen.Provider.ConnectionStringOverride = "";
            if (xs.Length == 0) xs = poDbToOpen.Provider.ConnectionStringOverride.TrimEnd();
            if (poDbToOpen.Database == null) poDbToOpen.Database = "";
            if (xs.Length == 0) xs = poDbToOpen.Database.TrimEnd();
            xs = xs.Replace("\r", " ");
            xs = xs.Replace("\n", " ");
            return (xs);
        }

        /// <summary>
        /// operator has requested to open a database not in the drop-down list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewDB_Click(object sender, EventArgs e)
        {
            LoadSQLServerInstances();
            LoadPrevDBs();
            mbAddingNewDb = true;
            frmOpenDB xoOpenDB = GetOpenDbDialog();
            xoOpenDB.SetSQLServerInstances(msSQLs);
            xoOpenDB.SetDBs(msDBs);
            xoOpenDB.SetAddMode();
            //
            xoOpenDB.ShowDialog();
            if (xoOpenDB.OK)
            {
                SaveHistory(xoOpenDB.DbToOpen.DataSource.Trim(), 1);
                SaveHistory(xoOpenDB.DbToOpen.Database.Trim(), 2);
                string xsDB = xoOpenDB.DbToOpen.Database;                   // this is the one in the list to switch to, and then open, dipshit
                xsDB = GetDrpListLineItem(xoOpenDB.DbToOpen);
                UpdateOpenedDBs(xoOpenDB.DbToOpen, true);
                OpenDbByID(miLastDbIdAdded);    //  xoOpenDB.DbToOpen.UniqueID); unless this is REFed
            }
            xoOpenDB.Close();
            xoOpenDB.Dispose();
            xoOpenDB = null;
            SetControlsAvails();
        }

        /// <summary>
        /// internal IIFS
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="ps1"></param>
        /// <param name="ps2"></param>
        /// <returns></returns>
        private string iifs(bool pb, string ps1, string ps2)
        {
            string xs = "";
            if (pb) xs = ps1; else xs = ps2;
            return (xs);
        }

        private void GetLastAddedDbID()
        {
            DB xoAppDB = new DB();
            if (Global.OpenThisAppsDatabase(ref xoAppDB))
            {
                string xsSQL = "SELECT max(ID) FROM [database]";
                DataTable xoTbl = new DataTable();
                if (xoAppDB.SQL(xsSQL, xoTbl))
                {
                    try
                    {
                        miLastDbIdAdded = Convert.ToInt32(xoTbl.Rows[0][0]);
                    }
                    catch { }
                }
                else
                {
                    xsSQL = xoAppDB.Message + "  (SQL: " + xsSQL + ")";
                    Global.ShowMessage(xsSQL, "Get Last Added DB ID");
                }
                xoTbl.Dispose();
                xoTbl = null;
                xoAppDB.CloseDatabase();
            }
            xoAppDB = null;
        }

        /// <summary>
        /// update the list of available previously opened DBs...this list is kept in an internal
        /// database file, for later retrieval
        /// </summary>
        /// <param name="poDbToOpen"></param>
        private void UpdateOpenedDBs(DatabaseToOpen poDbToOpen, bool pbAdding)
        {
            string xsErrMsg = "";
            //string xsWhere = "";
            string xsSQL = "";
            string xsConnStrOverride = poDbToOpen.Provider.ConnectionStringOverride.TrimEnd();
            bool xbClosed = false;

            DB xoAppDB = new DB();
            if (Global.OpenThisAppsDatabase(ref xoAppDB))
            {
                string xsDesc = poDbToOpen.Description.Trim();
                xsDesc = xsDesc.Replace("'", "");
                if (mbAddingNewDb)
                {
                    // new database...add to list
                    xsSQL = "INSERT INTO [database] (connType, providerName, source, [database], userID, connStr, [description]) VALUES (";
                    xsSQL = xsSQL + Convert.ToInt16(poDbToOpen.ConnectionType).ToString() + ", ";
                    xsSQL = xsSQL + "'" + poDbToOpen.Provider.Name.ToString() + "', ";
                    xsSQL = xsSQL + "'" + poDbToOpen.DataSource + "', ";
                    xsSQL = xsSQL + "'" + poDbToOpen.Database + "', ";
                    xsSQL = xsSQL + "'" + poDbToOpen.UserID + "', ";
                    //xsSQL = xsSQL + iifs(poDbToOpen.Password.Trim().Length > 0, "true", "false") + ", ";
                    xsSQL = xsSQL + "'" + xsConnStrOverride + "'";
                    xsSQL = xsSQL + ",'" + xsDesc + "')";
                    if (xoAppDB.SQL(xsSQL))
                    {
                        //xoAppDB.CloseDatabase();
                        Global.CloseThisAppsDatabase(ref xoAppDB);
                        xbClosed = true;
                        GetLastAddedDbID();
                    }
                    else
                    {
                        xsErrMsg = xoAppDB.Message + "  (SQL: " + xsSQL + ")";
                    }
                }
                else
                {
                    xsSQL = "UPDATE [database] SET userID = '" + poDbToOpen.UserID + "', ";         // +"', needPswd = ";
                    xsSQL += ", connStr = '" + xsConnStrOverride + "'";
                    xsSQL += ", connType = " + Convert.ToInt16(poDbToOpen.ConnectionType).ToString();
                    xsSQL += ", providerName = '" + poDbToOpen.Provider.Name.ToString() + "'";
                    xsSQL += ", [description] = '" + xsDesc + "'";
                    xsSQL += " WHERE ID = " + miCurrDb_PK_ID.ToString();
                    if (!xoAppDB.SQL(xsSQL)) xsErrMsg = xoAppDB.Message;
                }
            }
            else
            {
                xsErrMsg = xoAppDB.Message;
            }

            if (!xbClosed)
            {
                try
                {
                    Global.CloseThisAppsDatabase(ref xoAppDB);
                }
                catch { }
            }
            //Global.CloseThisAppsDatabase(ref xoAppDB);

            if (xsErrMsg.Length == 0)
            {
                LoadDBs();
                for (int xii = 0; xii < mrLoadedDbs.Count; xii++)
                {
                    int xi = mrLoadedDbs[xii].ID;
                    string xs = xi.ToString();
                }
            }
            else
            {
                ShowMessage(xsErrMsg, "Update Opened DBs");
            }
        }

        /// <summary>
        /// return the database path for the currently logged user (this is where the internal database is stored)
        /// </summary>
        /// <param name="psErrMsg"></param>
        /// <returns></returns>
        private string ThisUserDataPath(out string psErrMsg)
        {
            string xsPath = "", xsErrMsg = "";

            Assembly xoAsm = Assembly.GetExecutingAssembly();
            object[] xoAttrs = (xoAsm.GetCustomAttributes(typeof(GuidAttribute), true));
            Guid xoGuid = new Guid((xoAttrs[0] as GuidAttribute).Value);

            string xsBasePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            xsPath = string.Format(@"{0}\{1}\", xsBasePath, "rebusX");  //xoGuid.ToString("B").ToUpper());

            if (!Directory.Exists(xsPath))
            {
                try
                {
                    Directory.CreateDirectory(xsPath);
                    //string xs2 = "success";
                }
                catch (Exception xoExc)
                {
                    xsErrMsg = xoExc.Message;
                }
            }

            psErrMsg = xsErrMsg;
            return (xsPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="psErrMsg"></param>
        /// <returns></returns>
        private DatabaseToOpen InternalDB(out string psErrMsg)
        {
            string xsErrMsg = "";
            DatabaseToOpen xrDbToOpen = new DatabaseToOpen();
            DatabaseProvider xrProvider = new DatabaseProvider();
            DB xoDB = new DB();
            
            if (xoDB.IntrinsicProvider(IntrinsicProviderType.AccessMdbOleDB, out xrProvider, out xsErrMsg))
            xrDbToOpen.ConnectionType = ConnectivityType.OleDB;
            xrDbToOpen.Database = Global.gsAppDb;
            xrDbToOpen.DataSource = "";
            xrDbToOpen.Password = "";
            xrDbToOpen.Provider = xrProvider;
            xrDbToOpen.UserID = "";

            xoDB = null;
            psErrMsg = xsErrMsg;
            return (xrDbToOpen);
        }

        // deprecated
        //private string CreateNewInternalTables(DB poOpenDB)
        //{
        //    string xsErrMsg = "", xsSQL;

        //    xsSQL = "SELECT * FROM History WHERE histType = -1";
        //    DataTable xoTbl = new DataTable();
        //    if (!poOpenDB.SQL(xsSQL, xoTbl))
        //    {
        //        xsSQL = "CREATE TABLE History (histType INTEGER, histValue CHAR(255))";
        //        poOpenDB.SQL(xsSQL);
        //        xsSQL = "CREATE INDEX byHistType ON History (histType)";
        //        poOpenDB.SQL(xsSQL);
        //    }

        //    if (xoTbl != null)
        //    {
        //        xoTbl.Dispose();
        //        xoTbl = null;
        //    }
            
        //    return (xsErrMsg);
        //}

        /// <summary>
        /// this app has its own MDB file which stores various information such as previously opened databases, saved
        /// sets of queries/commands, and personalized settings...here, we make sure such database exists, and if not,
        /// create it (which should only happend the first time run per user, but...)
        /// </summary>
        private void CreateThisAppsDatabase()
        {
            string xsDbFile = "", xsErrMsg = "";
            DB xoDB;

            xsDbFile = ThisUserDataPath(out xsErrMsg) + Global.ThisAppsDatabaseFile;

            if (xsErrMsg.Length == 0)
            {
                Global.gsAppDb = xsDbFile;

                xoDB = new DB();
                if (!System.IO.File.Exists(Global.gsAppDb))
                {
                    if (xoDB.CreateMicrosoftDatabase(Global.gsAppDb))
                    {
                        DatabaseProvider xrProvider = new DatabaseProvider();
                        if (xoDB.IntrinsicProvider(IntrinsicProviderType.AccessMdbOleDB, out xrProvider, out xsErrMsg))
                        {
                            DatabaseToOpen xrDbToOpen = InternalDB(out xsErrMsg);
                            if (xsErrMsg.Length == 0)
                            {
                                if (xoDB.OpenDatabase(xrDbToOpen))
                                {
                                    // create the tables and indices
                                    MigrateDatabase();
                                    #region deprecate1
                                    //if (xoDB.SQL("CREATE TABLE [Database] (ID AUTOINCREMENT PRIMARY KEY, [description] CHAR(80), providerName CHAR(100), connType INTEGER, source CHAR(200), [database] CHAR(200), userID CHAR(50), connStr MEMO)"))
                                    //{
                                    //    if (xoDB.SQL("CREATE TABLE DataSet (dataSetID CHAR(25), sqlText MEMO, leftPos INTEGER, topPos INTEGER, width INTEGER, height INTEGER, [caption] CHAR(128), grpID CHAR(32), sqlDescription MEMO, masterChildInfo MEMO, specificTable CHAR(100), lockSQL LOGICAL)"))
                                    //    {
                                    //        if (xoDB.SQL("CREATE TABLE DataSets (dataSetID CHAR(25) PRIMARY KEY, dbID INTEGER, dataSetDescription MEMO, lev INTEGER)"))
                                    //        {
                                    //            if (xoDB.SQL("CREATE INDEX byDsID ON DataSet (dataSetID)"))
                                    //            {
                                    //                xsErrMsg = "";
                                    //            }

                                    //            //tableA, fieldA, tableB, fieldB FROM ForeignKeys WHERE databaseID
                                    //            xoDB.SQL("CREATE TABLE ForeignKeys (databaseID INTEGER, tableA CHAR(128), fieldA CHAR(128), tableB CHAR(128), fieldB CHAR(128))");
                                    //            xoDB.SQL("CREATE INDEX byDbID ON ForeignKeys(databaseID)");
                                    //            xoDB.SQL("CREATE INDEX byTable1 ON ForeignKeys(tableA)");
                                    //            xoDB.SQL("CREATE INDEX byTable2 ON ForeignKeys(tableB)");

                                    //            //
                                    //            //
                                    //            // need to some more fool-proofing of below...need to restructure all of this whereby
                                    //            // it feeds a number of SQL statements to create the internal database, instead of this
                                    //            // somewhat clumsy never-ending embedded set of ifs
                                    //            //
                                    //            //


                                    //            xoDB.SQL("CREATE TABLE Customs ([UserInfo] CHAR(255), [Section] CHAR(64), [Entry] CHAR(64), [Value] CHAR(255))");
                                    //            xoDB.SQL("CREATE INDEX [main] ON Customs (UserInfo, [Section], [Entry])");
                                    //            xoDB.SQL("CREATE INDEX byDbID ON DataSets (dbID)");
                                    //            //xoDB.SQL("CREATE TABLE Customs ([UserInfo] CHAR(255), [Section] CHAR(64), [Entry] CHAR(64), [Value] CHAR(255))");
                                    //            //xoDB.SQL("CREATE INDEX [main] ON Customs (UserInfo, [Section], [Entry])");
                                    //            //xoDB.SQL("CREATE INDEX byDbID ON DataSets (dbID)");
                                    //            //---xoDB.SQL("CREATE INDEX byProviderName ON [Database] (providerName)");
                                    //            xoDB.SQL("CREATE INDEX byConnType ON [Database] (connType)");
                                    //            xoDB.SQL("CREATE INDEX bySource ON [Database] (source)");
                                    //            xoDB.SQL("CREATE INDEX byDatabase ON [Database] ([database])");
                                    //            //xoDB.SQL("CREATE INDEX byProviderName ON [DataSets] (providerName)");
                                    //            //string xs = "";




                                    //            xsErrMsg = CreateNewInternalTables(xoDB);



                                    //            //{
                                    //            //    xsErrMsg = xoDB.Message;
                                    //            //}
                                    //        }
                                    //        else
                                    //        {
                                    //            xsErrMsg = xoDB.Message;
                                    //        }
                                    //    }
                                    //    else
                                    //    {
                                    //        xsErrMsg = xoDB.Message;
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    xsErrMsg = xoDB.Message + "\r\n" + xoDB.LastSQL;
                                    //}
                                    #endregion
                                }
                                else
                                {
                                    xsErrMsg = xoDB.Message + "\r\n" + xoDB.LastSQL;
                                }
                            }
                            else
                            {
                                xsErrMsg = xoDB.Message;
                            }
                        }
                    }
                    else
                    {
                        xsErrMsg = xoDB.Message;
                        //MessageBox.Show(xsDbFile + " failed to created.");
                    }
                }
                else
                {
                    //
                    // check for database updates
                    //
                    DatabaseToOpen xrDbToOpen = InternalDB(out xsErrMsg);
                    if (xsErrMsg.Length == 0)
                    {
                        if (xoDB.OpenDatabase(xrDbToOpen))
                        {
                            // adjust the database structure as needed
                            if (mbMigrate) MigrateDatabase();
                            #region deprecate2
                            //CreateNewInternalTables(xoDB);

                            //string xsSQL = "SELECT lockSQL FROM Dataset WHERE dataSetID = 'x'";
                            //if (!xoDB.SQL(xsSQL))
                            //{
                            //    xsSQL = "ALTER TABLE Dataset ADD COLUMN lockSQL LOGICAL";
                            //    if (!xoDB.SQL(xsSQL)) xsErrMsg = xoDB.Message + "  (SQL: " + xsSQL + ")";
                            //}

                            //xsSQL = "SELECT [description] FROM [Database] WHERE [ID] = 0";
                            //if (!xoDB.SQL(xsSQL))
                            //{
                            //    xsSQL = "ALTER TABLE [Database] ADD COLUMN [description] CHAR(80)";
                            //    if (!xoDB.SQL(xsSQL)) xsErrMsg = xoDB.Message + "  (SQL: " + xsSQL + ")";
                            //}

                            //xsSQL = "SELECT [masterChildInfo] FROM [DataSet] WHERE dataSetID = 'x'";
                            //if (!xoDB.SQL(xsSQL))
                            //{
                            //    xsSQL = "ALTER TABLE [DataSet] ADD COLUMN [masterChildInfo] MEMO";
                            //    if (!xoDB.SQL(xsSQL)) xsErrMsg = xoDB.Message + "  (SQL: " + xsSQL + ")";
                            //}

                            //xsSQL = "SELECT [grpID] FROM [DataSet] WHERE dataSetID = 'x'";
                            //if (!xoDB.SQL(xsSQL))
                            //{
                            //    xsSQL = "ALTER TABLE [DataSet] ADD COLUMN [grpID] CHAR(32)";
                            //    if (!xoDB.SQL(xsSQL)) xsErrMsg = xoDB.Message + "  (SQL: " + xsSQL + ")";
                            //}

                            //xsSQL = "SELECT [lev] FROM [DataSets] WHERE [lev] = 0";
                            //if (!xoDB.SQL(xsSQL))
                            //{
                            //    xsSQL = "ALTER TABLE DataSets ALTER COLUMN dataSetDescription MEMO";
                            //    if (!xoDB.SQL(xsSQL)) xsErrMsg = xoDB.Message + "  (SQL: " + xsSQL + ")";
                            //    xsSQL = "ALTER TABLE DataSets ADD COLUMN lev INTEGER";
                            //    if (!xoDB.SQL(xsSQL)) xsErrMsg = xoDB.Message + "  (SQL: " + xsSQL + ")";
                            //}
                            #endregion
                        }
                        else
                        {
                            xsErrMsg = xoDB.Message;
                        }
                    }
                }

                try
                {
                    if (xoDB != null) xoDB.CloseDatabase();
                }
                catch (Exception xoExc)
                {
                    string xs = xoExc.Message;
                }
            }

            if (xsErrMsg.Length > 0)
            {
                Global.ShowMessage(xsErrMsg, "Create Application Database");
            }
            xoDB = null;
        }

        private void MigrateDatabase()
        {
            List<Table> xoTbls = new List<Table>(0);

            Table xoTbl = new Table("Database");
            xoTbl.Columns.Add(new Column("ID", ColumnType.AutoIncrement, true));
            xoTbl.Columns.Add(new Column("description", ColumnType.Char, 80));
            xoTbl.Columns.Add(new Column("providerName", ColumnType.Char, 100));
            xoTbl.Columns.Add(new Column("connType", ColumnType.Integer));
            xoTbl.Columns.Add(new Column("source", ColumnType.Char, 200));
            xoTbl.Columns.Add(new Column("database", ColumnType.Char, 200));
            xoTbl.Columns.Add(new Column("userID", ColumnType.Char, 50));
            xoTbl.Columns.Add(new Column("connStr", ColumnType.Memo));
            xoTbl.Indices.Add(new Index("byConnType", "connType"));
            xoTbl.Indices.Add(new Index("bySource", "source"));
            xoTbl.Indices.Add(new Index("byDatabase", "database"));
            xoTbls.Add(xoTbl);

            xoTbl = new Table("DataSets");
            xoTbl.Columns.Add(new Column("dataSetID", ColumnType.Char, 25, true));
            xoTbl.Columns.Add(new Column("dbID", ColumnType.Integer));
            xoTbl.Columns.Add(new Column("dataSetDescription", ColumnType.Memo));
            xoTbl.Columns.Add(new Column("lev", ColumnType.Integer));
            xoTbl.Indices.Add(new Index("byDbID", "dbID"));
            xoTbls.Add(xoTbl);

            xoTbl = new Table("DataSet");
            xoTbl.Columns.Add(new Column("dataSetID", ColumnType.Char, 25));
            xoTbl.Columns.Add(new Column("sqlText", ColumnType.Memo));
            xoTbl.Columns.Add(new Column("leftPos", ColumnType.Integer));
            xoTbl.Columns.Add(new Column("topPos", ColumnType.Integer));
            xoTbl.Columns.Add(new Column("width", ColumnType.Integer));
            xoTbl.Columns.Add(new Column("height", ColumnType.Integer));
            xoTbl.Columns.Add(new Column("caption", ColumnType.Char, 128));
            xoTbl.Columns.Add(new Column("grpID", ColumnType.Char, 32));
            xoTbl.Columns.Add(new Column("sqlDescription", ColumnType.Memo));
            xoTbl.Columns.Add(new Column("masterChildInfo", ColumnType.Memo));
            xoTbl.Columns.Add(new Column("specificTable", ColumnType.Char, 100));
            xoTbl.Columns.Add(new Column("lockSQL", ColumnType.Boolean));
            xoTbl.Indices.Add(new Index("byDsID", "dataSetID"));
            xoTbls.Add(xoTbl);

            xoTbl = new Table("History");
            xoTbl.Columns.Add(new Column("histType", ColumnType.Integer));
            xoTbl.Columns.Add(new Column("histValue", ColumnType.Char, 255));
            xoTbl.Indices.Add(new Index("byHistType", "histType"));
            xoTbls.Add(xoTbl);

            xoTbl = new Table("Customs");
            xoTbl.Columns.Add(new Column("UserInfo", ColumnType.Char, 255));
            xoTbl.Columns.Add(new Column("Section", ColumnType.Char, 64));
            xoTbl.Columns.Add(new Column("Entry", ColumnType.Char, 64));
            xoTbl.Columns.Add(new Column("Value", ColumnType.Char, 255));
            xoTbl.Indices.Add(new Index("main", new List<string> { "UserInfo", "Section", "Entry" }));
            xoTbls.Add(xoTbl);

            xoTbl = new Table("ForeignKeys");
            xoTbl.Columns.Add(new Column("databaseID", ColumnType.Integer));
            xoTbl.Columns.Add(new Column("fieldA", ColumnType.Char, 128));
            xoTbl.Columns.Add(new Column("fieldB", ColumnType.Char, 128));
            xoTbl.Columns.Add(new Column("tableA", ColumnType.Char, 128));
            xoTbl.Columns.Add(new Column("tableB", ColumnType.Char, 128));
            xoTbl.Indices.Add(new Index("byDbID", "databaseID"));
            xoTbl.Indices.Add(new Index("byTableA", "tableA"));
            xoTbl.Indices.Add(new Index("byTableB", "tableB"));
            xoTbls.Add(xoTbl);

            //xoTbls.Clear();
            //xoTbl = new Table("Test");
            //xoTbl.Columns.Add(new Column("UserInfo", ColumnType.Char, 255));
            //xoTbl.Columns.Add(new Column("Section", ColumnType.Char, 64));
            //xoTbl.Columns.Add(new Column("Entry", ColumnType.Char, 64));
            //xoTbl.Columns.Add(new Column("Value", ColumnType.Char, 255));
            //xoTbl.Indices.Add(new Index("test", "Value"));
            //xoTbls.Add(xoTbl);

            Migrator xoMigrator = new Migrator(new DatabaseConnectionInfo(DatabaseType.Access, Global.gsAppDb), xoTbls);

            List<string> xsErrs = xoMigrator.Migrate();

            if (xsErrs.Count > 0)
            {
                string xsMsg = "";
                for (int xi = 0; xi < xsErrs.Count; xi++)
                {
                    xsMsg += "\r\n" + xsErrs[xi];
                }
                Global.ShowMessage(xsMsg, "Migrate Database");
            }
        }

        private void Cosmetics()
        {
            if (mbNeedInit)
            {
                mbNeedInit = false;
                CreateThisAppsDatabase();
                CustomFormSizing();
                mbMoving = true;
                CustomFormPositioning();
                mbMoving = false;
                LoadDBs();
                ShowOpenDB();
            }
            else
            {
                SetControlsAvails();
            }
        }

        /// <summary>
        /// internal call to wrapped message box show
        /// </summary>
        /// <param name="psMsg"></param>
        /// <param name="psTitle"></param>
        /// <param name="poBtns"></param>
        /// <returns></returns>
        private DialogResult ShowMessage(string psMsg, string psTitle = "", MessageBoxButtons poBtns = MessageBoxButtons.OK)
        {
            return (Global.ShowMessage(psMsg, psTitle, poBtns));
        }

        /// <summary>
        /// wrapper for returning a blank string if incoming object is null
        /// </summary>
        /// <param name="po"></param>
        /// <returns></returns>
        private string null2str(object po)
        {
            string xs = "";

            if (po != null) xs = po.ToString();

            return (xs);
        }

        /// <summary>
        /// key function here...we load the database to open structure in prep for passing to
        /// the database handling class, which hides much of the complexity of opening various
        /// brands of databases under varying schemes
        /// </summary>
        /// <param name="prDbToOpen"></param>
        /// <param name="psErrMsg"></param>
        /// <returns></returns>
        private bool GetDbToOpen(ref DatabaseToOpen prDbToOpen, out string psErrMsg)
        {
            string xsErrMsg = "";
            int xiIdx = -1;

            for (int xii = 0; xii < mrLoadedDbs.Count; xii++)
            {
                if (mrLoadedDbs[xii].ID == miCurrDb_PK_ID)
                {
                    xiIdx = xii;
                    break;
                }
            }

            if (xiIdx >= 0)
            {
                DB xoAppDB = new DB();
                int xiID = mrLoadedDbs[xiIdx].ID;     //mrLoadedDbs[drpDB.SelectedIndex].ID;

                if (Global.OpenThisAppsDatabase(ref xoAppDB))
                {
                    DataTable xoTbl = new DataTable();
                    xoTbl.Reset();
                    string xsSQL = "SELECT connType, providerName, source, [database], userID, connStr, [description] FROM [database] WHERE ID = " + xiID.ToString();
                    if (xoAppDB.SQL(xsSQL, xoTbl))
                    {
                        if (xoTbl.Rows.Count > 0)
                        {
                            prDbToOpen.UniqueID = xiID;

                            DatabaseProvider xrProv = new DatabaseProvider();

                            string xsProv = null2str(xoTbl.Rows[0][1]).TrimEnd().ToUpper();

                            if (xsProv.Length > 0)
                            {
                                for (int xii = 0; xii < xoAppDB.Providers.Count; xii++)
                                {
                                    if (xsProv == xoAppDB.Providers[xii].Name.TrimEnd().ToUpper())
                                    {
                                        xrProv = xoAppDB.Providers[xii];
                                        break;
                                    }
                                }
                            }
                            xrProv.ConnectionStringOverride = null2str(xoTbl.Rows[0][5]).Trim();
                            prDbToOpen.Provider = xrProv;

                            prDbToOpen.Description = null2str(xoTbl.Rows[0][6]).Trim();
                            prDbToOpen.ConnectionType = (ConnectivityType)xoAppDB.null2int(xoTbl.Rows[0][0]);

                            prDbToOpen.DataSource = null2str(xoTbl.Rows[0][2]).Trim();
                            prDbToOpen.Database = null2str(xoTbl.Rows[0][3]).Trim();
                            prDbToOpen.UserID = null2str(xoTbl.Rows[0][4]).Trim();
                        }
                        else
                        {
                            xsErrMsg = "Database connection record not found.\r\n(SQL: " + xsSQL + ")  (ID: " + xiID.ToString();
                        }
                    }
                    else
                    {
                        xsErrMsg = xoAppDB.Message + "\r\n(SQL: " + xsSQL + ")";
                    }
                    ReleaseTable(ref xoTbl);
                    Global.CloseThisAppsDatabase(ref xoAppDB);
                }
                else
                {
                    xsErrMsg = xoAppDB.Message;
                }
            }

            psErrMsg = xsErrMsg;

            return (xsErrMsg.Length == 0);
        }

        /// <summary>
        /// user has requested an action which requires the currently open database to be closed, so we verify before continuing
        /// </summary>
        /// <returns></returns>
        private bool VerifyClose()
        {
            bool xbVerified = false;
            if (ShowMessage("The currently open database will be closed.  Would you like to contine?", "Open Database", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                xbVerified = true;
            }
            return (xbVerified);
        }

        /// <summary>
        /// return a frmOpenDB dialog
        /// </summary>
        /// <returns></returns>
        private frmOpenDB GetOpenDbDialog()
        {
            //LoadSQLServerInstances();
            frmOpenDB xoOpenDB = new frmOpenDB();
            xoOpenDB.SetExternalProviders(moExtProvs);
            return (xoOpenDB);
        }

        /// <summary>
        /// another key function, we're getting ready to open a database, this is a front-end for the process...
        /// the poDbToOpen holds the information we need to open the database, if the action is confirmed
        /// </summary>
        /// <param name="psErrMsg"></param>
        /// <param name="pbCanceled"></param>
        /// <param name="poDbToOpen"></param>
        /// <returns></returns>
        private bool ToOpenDatabase(out string psErrMsg, out bool pbCanceled, DatabaseToOpen poDbToOpen, bool pbAlreadyVerified = false, bool pbPasswordVerified = false)
        {
            string xsErrMsg = "";
            bool xbCanceled = false;
            bool xbContinue = true;
            bool xbOK = false;

            if (!pbAlreadyVerified)
            {
                if (CurrentDBisOpen())
                {
                    if (!VerifyClose())
                    {
                        xbCanceled = true;
                        xbContinue = false;
                    }
                }
            }

            if (xbContinue)
            {
                CloseCurrentDatabaseAndRefreshWindow();

                if (GetDbToOpen(ref poDbToOpen, out xsErrMsg))
                {
                    frmOpenDB xoOpenDB = GetOpenDbDialog();
                    xoOpenDB.Text = "Open Database";
                    xoOpenDB.SetSQLServerInstances(msSQLs);
                    xoOpenDB.SetDbToOpen(poDbToOpen);
                    if (poDbToOpen.UserID.Length == 0 || pbPasswordVerified)
                    {
                        // no need to show dialog for password, none needed
                        xoOpenDB.DbToOpen = poDbToOpen;
                        xbOK = true;
                    }
                    else
                    {
                        // databases saved originally opened with a user ID, user must re-supply password, as we do not store it
                        xoOpenDB.ShowDialog();
                        xbOK = xoOpenDB.OK;
                        if (xbOK) msCurrDbPswd = xoOpenDB.Password();
                    }
                    if (xbOK)
                    {
                        OpenCurrDB(xoOpenDB.DbToOpen);
                        CreateNewTablesWindow();
                    }
                    else
                    {
                        xbCanceled = true;
                    }
                    xoOpenDB.Close();
                    xoOpenDB.Dispose();
                    xoOpenDB = null;
                }
                ShowOpenDB();
            }

            psErrMsg = xsErrMsg;
            pbCanceled = xbCanceled;

            return (xsErrMsg.Length == 0);
        }

        private void OpenSelectedDB(LoadedDb poLoadedDB)
        {
            mbAddingNewDb = false;
            //LoadSQLServerInstances();

            string xsErrMsg = "";
            bool xbCanceled = false, xbMatched = false;
            DatabaseToOpen xrDB = new DatabaseToOpen();

            DB xoDB = new DB();

            //LoadedDb xoLoadedDB = mrLoadedDbs[drpDB.SelectedIndex];
            LoadedDb xoLoadedDB = poLoadedDB;
            xoLoadedDB.ConnStrOverride = xoLoadedDB.ConnStrOverride.Trim();
            miCurrDb_PK_ID = xoLoadedDB.ID;

            if (xoLoadedDB.ConnStrOverride.Length == 0)
            {
                string xsLoadedProvider = xoLoadedDB.ProviderName.TrimEnd().ToUpper();
                for (int xii = 0; xii < xoDB.Providers.Count; xii++)
                {
                    if (xoDB.Providers[xii].Name.TrimEnd().ToUpper() == xsLoadedProvider)
                    {
                        xrDB.Provider = xoDB.Providers[xii];
                        xrDB.Provider.ConnectionStringOverride = xoLoadedDB.ConnStrOverride;
                        xrDB.ConnectionType = xoLoadedDB.ConnType;
                        xrDB.Database = xoLoadedDB.Database;
                        xrDB.DataSource = xoLoadedDB.DataSource;
                        xrDB.UniqueID = xoLoadedDB.ID;
                        xrDB.UserID = xoLoadedDB.UserID;
                        xrDB.Password = "";
                        //xrDB.NeedPassword = xoLoadedDB.NeedPassword;
                        xbMatched = true;
                        break;
                    }
                }
            }
            else
            {
                xrDB.Provider.IntrinsicProvider = IntrinsicProviderType.NotIntrinsic;
                xrDB.Provider.IsAccess = false;
                xrDB.Provider.IsMsSQLServer = false;
                xrDB.Provider.IsOracle = true;
                xrDB.Provider.Name = "";
                xrDB.Provider.TopPhrase = "";
                xrDB.Provider.ConnectionStringOverride = xoLoadedDB.ConnStrOverride;
                xrDB.ConnectionType = xoLoadedDB.ConnType;
                xrDB.Database = xoLoadedDB.Database;
                xrDB.DataSource = xoLoadedDB.DataSource;
                xrDB.Description = xoLoadedDB.Description;
                xrDB.UniqueID = xoLoadedDB.ID;
                xrDB.UserID = xoLoadedDB.UserID;
                xrDB.Password = "";
                xbMatched = true;
            }

            if (xbMatched)
            {
                if (!ToOpenDatabase(out xsErrMsg, out xbCanceled, xrDB))
                {
                    if (!xbCanceled) ShowMessage(xsErrMsg, "Open Database");
                }
                else
                {
                    ShowOpenDB();
                }
                SetControlsAvails();
            }
            else
            {
                ShowMessage("Unable to match selected database with available provider.", "Open Database");
            }
        }

        /// <summary>
        /// ???
        /// </summary>
        /// <param name="poDbToOpen"></param>
        private void OpenCurrDB(DatabaseToOpen poDbToOpen)
        {
            string xsConnStrOverride = "";  // poDbToOpen.Provider.ConnectionStringOverride.Trim();
            string xsErrMsg = "", xsManConnStr = iifs(xsConnStrOverride.Length > 0, xsConnStrOverride, "");
            CloseCurrDB();
            if (moCurrDB.DB == null)
            {
                moCurrDB.DB = new DB();
                moCurrDB.ID = poDbToOpen.UniqueID;
            }

            DatabaseToOpen xrDbToOpen = new DatabaseToOpen();

            xrDbToOpen.Provider = poDbToOpen.Provider;
            xrDbToOpen.ConnectionType = poDbToOpen.ConnectionType;
            xrDbToOpen.DataSource = poDbToOpen.DataSource;
            xrDbToOpen.Database = poDbToOpen.Database;
            xrDbToOpen.Description = poDbToOpen.Description;
            xrDbToOpen.UserID = poDbToOpen.UserID;
            xrDbToOpen.Password = poDbToOpen.Password;

            if (moCurrDB.DB.OpenDatabase(xrDbToOpen))
            {
                SetControlsAvails();
            }
            else
            {
                xsErrMsg = moCurrDB.DB.Message;
                moCurrDB.DB = null;
                moCurrDB.ID = 0;
            }

            if (xsErrMsg.Length > 0) ShowMessage(xsErrMsg, "Open Database");
        }

        /// <summary>
        /// close the currently open database
        /// </summary>
        private void CloseCurrDB()
        {
            if (moCurrDB.DB != null)
            {
                CloseSQLWindows();
                CloseTBLWindows();
                moCurrDB.DB.CloseDatabase();
                moCurrDB.ID = 0;
                moCurrDB.DB = null;
                msCurrDbPswd = "";
            }
        }

        /// <summary>
        /// form closing cleanup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseCurrDB();
        }

        /// <summary>
        /// close any open SQL Query/Command windows, in prep for open of another database, or application close
        /// </summary>
        private void CloseSQLWindows()
        {
            foreach (frmBaseMDI xoForm in this.MdiChildren)
            {
                if (xoForm.FormType == MDIType.SQL)
                {
                    frmSQL2 xoSQL = (frmSQL2)xoForm;
                    if (xoSQL.DbID == moCurrDB.ID)
                    {
                        xoForm.Close();
                    }
                    xoSQL.Dispose();
                    xoSQL = null;
                }
            }
        }

        /// <summary>
        /// close any open Database Tables window(s), in prep for open of another database, or application close
        /// </summary>
        private void CloseTBLWindows()
        {
            foreach (frmBaseMDI xoForm in this.MdiChildren)
            {
                if (xoForm.FormType == MDIType.Tables)
                {
                    frmTbls xoTbls = (frmTbls)xoForm;
                    if (xoTbls.DbID == moCurrDB.ID)
                    {
                        xoForm.Close();
                    }
                    xoTbls.Dispose();
                    xoTbls = null;
                }
            }
        }

        ///
        /// <summary>
        /// open a new SQL Query/Command child window
        /// </summary>
        /// <param name="psTable"></param>
        /// <param name="psSQL"></param>
        /// <param name="piX"></param>
        /// <param name="piY"></param>
        /// <param name="piWdt"></param>
        /// <param name="piHgt"></param>
        /// <param name="psCaption"></param>
        /// <param name="psDesc"></param>
        /// <param name="pbLockSQL"></param>
        public frmSQL2 CreateNewSQLWindow(string psTable = "", string psSQL = "", int piX = -1, int piY = -1, int piWdt = 0, int piHgt = 0, string psCaption = "", string psDesc = "", bool pbLockSQL = false, string psGrpID = "", bool pbRestoring = false)
        {
            string xsErrMsg = "";
            bool xbFocus = true;
            frmSQL2 xoFrm = new frmSQL2();
            Rectangle xrRect = new Rectangle();
            xrRect = Screen.PrimaryScreen.WorkingArea;
            int xiDbID = 0; // piDbID;
            if (xiDbID <= 0) xiDbID = moCurrDB.ID;

            DatabaseToOpen xrDbToOpen = new DatabaseToOpen();
            if (GetDbToOpen(ref xrDbToOpen, out xsErrMsg))
            {
                //frmSQL2 xoFrm = new frmSQL2();

                xoFrm.GUID = Guid.NewGuid().ToString();
                LastSQL_Guid = xoFrm.GUID;
                xoFrm.FormType = MDIType.SQL;       // peFormType
                miMDICount++;
                
                xoFrm.InternalWindowID = miMDICount;
                xoFrm.SetSQL(psSQL);
                //bool xb = false;
                //if (psSQL.Length > 0) xoFrm.ColorizeSQL(out xb);
                string xs = psCaption.Trim();
                if (xs.Length > 0) xoFrm.Text = xs;

                xoFrm.SetDesc(psDesc);
                xoFrm.SetGrpID(psGrpID);

                if (piX >= 0 & piY >= 0) xoFrm.Location = new Point(piX, piY);

                if (piWdt > 0 & piHgt > 0 & piWdt >= xoFrm.MinimumSize.Width & piHgt >= xoFrm.MinimumSize.Height)
                {
                    xoFrm.StartPosition = FormStartPosition.Manual;
                    xoFrm.Size = new System.Drawing.Size(piWdt, piHgt);
                }

                DB xoDB = new DB();
                xrDbToOpen.Password = msCurrDbPswd;
                if (xoDB.OpenDatabase(xrDbToOpen))
                {
                    xoFrm.DbID = moCurrDB.ID;
                    xoDB.Connection.Provider = xrDbToOpen.Provider;
                    xoFrm.DB = xoDB;

                    xoFrm.MdiParent = this;

                    string xsTable = psTable.Trim();
                    if (xsTable.Length > 0)
                    {
                        xoFrm.SetTable(psTable, psCaption, psSQL, pbRestoring);            // this will cause ExecuteSQL
                    }
                    else
                    {
                        xoFrm.HideTableInfoTabs();
                        if (psSQL.Trim().Length > 0  && !pbRestoring) xoFrm.ExecuteSQL();
                    }

                    if (pbLockSQL) xoFrm.LockSQL(true);
                    if (psTable == "" && psSQL == "")
                    {
                        //  xoFrm.set
                        xoFrm.UpdateStatusBar("Enter SQL, and click <Execute>");
                        xbFocus = true;
                    }
                    xoFrm.Show();
                    if (xbFocus) xoFrm.FocusSQL();
                    xoDB = null;                // KEEP AN EYE ON THIS FUCK!
                }
                else
                {
                    xsErrMsg = xoDB.Message;
                    xoDB = null;
                    xoFrm.Dispose();
                    xoFrm = null;
                }
            }
            else
            {
                xoFrm = null;
            }
            if (xsErrMsg.Length > 0) ShowMessage(xsErrMsg, "Open New SQL Window");

            RefreshChildWindowList();

            return (xoFrm);
        }

        /// <summary>
        /// open a new Database Tables window for the currently open database
        /// </summary>
        public void CreateNewTablesWindow(DatabaseObjectType piDbObjType = DatabaseObjectType.Tables)
        {
            string xsErrMsg = "";

            DatabaseToOpen xrDbToOpen = new DatabaseToOpen();

            if (GetDbToOpen(ref xrDbToOpen, out xsErrMsg))
            {
                frmTbls xoFrm = new frmTbls();
                xoFrm.FormType = MDIType.Tables;
                xoFrm.DatabaseObjectTypeShown = piDbObjType;
                miMDICount++;
                xoFrm.InternalWindowID = miMDICount;
                DB xoDB = new DB();
                xrDbToOpen.Password = msCurrDbPswd;
                if (xoDB.OpenDatabase(xrDbToOpen))
                {
                    xoFrm.DbID = moCurrDB.ID;
                    xoFrm.Password = msCurrDbPswd;
                    xoDB.Connection.Provider = xrDbToOpen.Provider;
                    xoFrm.DB = xoDB;
                    xoFrm.MdiParent = this;
                    xoFrm.Show();
                }
                else
                {
                    xoDB = null;
                    xoFrm.Dispose();
                    xoFrm = null;
                }
            }
            if (xsErrMsg.Length > 0) ShowMessage(xsErrMsg, "Open New Tables Window");

            RefreshChildWindowList();
        }

        /// <summary>
        /// create new SQL Query/Command child window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewSQLwindow_Click(object sender, EventArgs e)
        {
            CreateNewSQLWindow();
        }

        /// <summary>
        /// as main window size/position changes, we store that in the internal
        /// database, and restore upon re-launch of the application
        /// </summary>
        /// <param name="pbSave"></param>
        private void CustomFormSizing(bool pbSave = false)      // assume restore, unless signaled to save
        {
            string xsSection = "frmMain";
            string xsEntry = "Size";
            string xsValue = "";
            string xsErrMsg = "";

            if (pbSave)
            {
                xsValue = this.Width.ToString() + "," + this.Height.ToString();
                Global.StoreSetting(xsSection, xsEntry, xsValue, out xsErrMsg);
            }
            else
            {
                Global.RetrieveSetting(xsSection, xsEntry, out xsValue, out xsErrMsg);
                if (xsValue.Length > 0)
                {
                    int xi = xsValue.IndexOf(",");
                    if (xi > 0 && xi + 1 < xsValue.Length)
                    {
                        string xsWdt = xsValue.Substring(0, xi);
                        string xsHgt = xsValue.Substring(xi + 1);
                        int xiWdt = -1, xiHgt = -1;
                        try
                        {
                            xiWdt = Convert.ToInt32(xsWdt);
                            xiHgt = Convert.ToInt32(xsHgt);
                        }
                        catch (Exception xoExc)
                        {
                            string xs = xoExc.Message;
                        }
                        Rectangle xrRect = new Rectangle();
                        xrRect = Screen.PrimaryScreen.WorkingArea;
                        if (xiWdt > 100 && xiWdt < xrRect.Width && xiHgt > 100 && xiHgt < xrRect.Height)
                        {
                            this.Width = xiWdt;
                            this.Height = xiHgt;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// as main window size/position changes, we store that in the internal
        /// database, and restore upon re-launch of the application
        /// </summary>
        private void CustomFormPositioning(bool pbSave = false)      // assume restore, unless signaled to save
        {
            string xsSection = "frmMain";
            string xsEntry = "Position";
            string xsValue = "";
            string xsErrMsg = "";

            if (Global.gsAppDb.Length > 0)
            {
                if (pbSave)
                {
                    xsValue = this.Left.ToString() + "," + this.Top.ToString();
                    Global.StoreSetting(xsSection, xsEntry, xsValue, out xsErrMsg);
                }
                else
                {
                    Global.RetrieveSetting(xsSection, xsEntry, out xsValue, out xsErrMsg);
                    if (xsValue.Length > 0)
                    {
                        int xi = xsValue.IndexOf(",");
                        if (xi > 0 && xi + 1 < xsValue.Length)
                        {
                            string xsLft = xsValue.Substring(0, xi);
                            string xsTop = xsValue.Substring(xi + 1);
                            int xiLft = -1, xiTop = -1;
                            try
                            {
                                xiLft = Convert.ToInt32(xsLft);
                                xiTop = Convert.ToInt32(xsTop);
                            }
                            catch (Exception xoExc)
                            {
                                string xs = xoExc.Message;
                            }
                            Rectangle xrRect = new Rectangle();
                            xrRect = Screen.PrimaryScreen.WorkingArea;
                            if (xiLft > 1 & xiLft < xrRect.Width & xiTop > 1 & xiTop < xrRect.Height)
                            {
                                this.Left = xiLft;
                                this.Top = xiTop;
                            }
                        }
                    }
                }
            }
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            CustomFormSizing(true);
        }

        private void frmMain_Move(object sender, EventArgs e)
        {
            if (!mbMoving) CustomFormPositioning(true);
        }

        private void frmMain_Layout(object sender, LayoutEventArgs e)
        {
            Cosmetics();
        }

        /// <summary>
        /// create a new Database Tables child window (user only really needs one of these per database,
        /// of which we create for them automatically when opening a database, but if they close and
        /// need another instance, so be it)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShowTable_Click(object sender, EventArgs e)
        {
            if (CurrentDBisOpen()) CreateNewTablesWindow();
        }

        /// <summary>
        /// status bar text at bottom of main window...update with latest message
        /// </summary>
        /// <param name="psMsg"></param>
        public void UpdateStatusBar(string psMsg)
        {
            //stat.Text = psMsg;
            statusLbl.Text = psMsg;
            stat.Refresh();
        }

        /// <summary>
        /// show text of the currently opened database in appropriate place(s)
        /// </summary>
        private void ShowOpenDB()
        {
            string xsCaption = Application.ProductName.Trim();

            if (CurrentDBisOpen())
            {
                DatabaseToOpen xrDbToOpen = new DatabaseToOpen();
                string xs = "";
                if (GetDbToOpen(ref xrDbToOpen, out xs))
                {
                    xsCaption = xsCaption + ": " + GetDrpListLineItem(xrDbToOpen);  // xrDbToOpen.Database.Trim();
                }
            }
            this.Text = xsCaption;
        }

        /// <summary>
        /// close all child windows
        /// </summary>
        private void CloseAllMdiWindows()
        {
            CloseSQLWindows();
            CloseTBLWindows();
        }

        /// <summary>
        /// close the current database, and destroy the associated windows
        /// </summary>
        private void CloseCurrentDatabaseAndRefreshWindow()
        {
            if (CurrentDBisOpen())
            {
                CloseAllMdiWindows();
                CloseCurrDB();
                //drpDB.SelectedIndex = -1;
                //UpdateStatusBar("");
                SetControlsAvails();
                ShowOpenDB();
            }
            msCurrentRestoredDsDesc = "";
            msCurrentRestoredDsID = "";
            miCurrentRestoredDbID = 0;
            CloseCurrDB();
        }

        /// <summary>
        /// database close confirmation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCloseDB_Click(object sender, EventArgs e)
        {
            if (ShowMessage("Close the currently open database...please confirm...", "Close Database", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                CloseCurrentDatabaseAndRefreshWindow();
            }
        }

        /// <summary>
        /// restore a set of previously saved SQL Query/Command windows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRestoreSQL_Click(object sender, EventArgs e)
        {
            RestoreSQL();
            SetControlsAvails();
        }

        /// <summary>
        /// save a set of open SQL Query/Command windows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveSQL_Click(object sender, EventArgs e)
        {
            SaveSQL();
        }

        /// <summary>
        /// return time as such (used for creating unique keys)
        /// </summary>
        /// <returns></returns>
        private string NowAsYYYYMMDDHHMMSS()
        {
            string xs = "", xsNow = "";
            xs = string.Format("{0:HH:mm:ss}", DateTime.Now);
            xsNow = xs.Replace(":", "");
            xs = string.Format("{0:yyyyMMdd}", DateTime.Now);
            xsNow = xs + xsNow;
            return xsNow;
        }

        private string GenerateNewUniqueSQLID()
        {
            string xs = "";

            xs = string.Format("{0:ffff}", DateTime.Now);
            xs = NowAsYYYYMMDDHHMMSS() + "_" + xs;
            return (xs);
        }

        /// <summary>
        /// save the current set of open SQL Query/Command windows
        /// </summary>
        private void SaveSQL()
        {
            string xsID = "", xsTmpID = "", xsErrMsg = "", xsSQL = "", xsDesc = "", xsDsSQL = ""; //, xsBrand = "";
            bool xbReplacing = false, xbLockSQL = false;
            DialogResult xe;
            DB xoAppDB = new DB();

            xbReplacing = (msCurrentRestoredDsDesc.Trim().Length > 0);
            if (xbReplacing) xsID = msCurrentRestoredDsID; else xsID = GenerateNewUniqueSQLID();
            if (!CurrentDBisOpen())
            {
                xsErrMsg = "No database open at present.";
            }
            else
            {
                int xiCount = 0;
                foreach (frmBaseMDI xoForm in this.MdiChildren)
                {
                    if (xoForm.FormType == MDIType.SQL) xiCount++;
                }
                if (xiCount < 1)
                {
                    xsErrMsg = "There are no SQLs open to save from.";
                }
                else
                {
                    string xs = msCurrentRestoredDsDesc;
                    xe = Global.InputBox("Save SQLs as Set", "Enter a brief description of the SQLs: ", ref xs);
                    if (xe == System.Windows.Forms.DialogResult.OK)
                    {
                        xs = xs.Trim();
                    }
                    else
                    {
                        xs = "";
                    }
                    xsDesc = xs;
                    if (xsDesc.Length == 0)
                    {
                        xsErrMsg = "Save canceled.";
                    }
                }
            }
            
            if (xsErrMsg.Length == 0)
            {
                if (!Global.OpenThisAppsDatabase(ref xoAppDB)) xsErrMsg = xoAppDB.Message;
            }
            if (xsErrMsg.Length == 0)
            {
                if (xbReplacing)
                {
                    xsSQL = "UPDATE DataSets SET datasetDescription = '" + xsDesc + "' WHERE datasetID = '" + xsID + "'";
                    if (xoAppDB.SQL(xsSQL))
                    {
                        //xsSQL = "DELETE * FROM Dataset WHERE datasetID = '" + xsID + "'";
                        // first, we'll "flag" the previous dataset(s) for delete, in case the child dataset insert(s) fail, and restore them, otherwise, go ahead and remove
                        xsTmpID = GenerateNewUniqueSQLID();
                        xsSQL = "UPDATE [Dataset] SET datasetID = '" + xsTmpID + "' WHERE datasetID = '" + xsID + "'";
                        if (!xoAppDB.SQL(xsSQL))
                        {
                            xsErrMsg = "(Temporary dataset id change) " + xoAppDB.Message + "\r\r" + xsSQL;
                        }
                    }
                    else
                    {
                        xsErrMsg = xsErrMsg = xoAppDB.Message + "\r\r" + xsSQL;
                    }
                }
                else
                {
                    xsSQL = "INSERT INTO DataSets (dataSetID, datasetDescription, dbID) VALUES ('" + xsID + "', '" + xsDesc + "', " + moCurrDB.ID.ToString() + ")";
                    if (!xoAppDB.SQL(xsSQL))
                    {
                        xsErrMsg = xsErrMsg = xoAppDB.Message + "\r\r" + xsSQL;
                    }
                }
            }
            if (xsErrMsg.Length == 0)
            {
                foreach (frmBaseMDI xoForm in this.MdiChildren)
                {
                    if (xoForm.FormType == MDIType.SQL)
                    {
                        frmSQL2 xoSQL = (frmSQL2)xoForm;
                        xbLockSQL = xoSQL.IsLocked();
                        xsDsSQL = xoSQL.GetSQL();
                        xsDsSQL = xsDsSQL.Replace("'", Global.gcc251.ToString());
                        xsDsSQL = xsDsSQL.Replace(Global.gccQT.ToString(), Global.gcc252.ToString());
                        xsSQL = "INSERT INTO Dataset (datasetID, sqlText, leftPos, topPos, width, height, caption, sqlDescription, specificTable, lockSQL, masterChildInfo, grpID) VALUES (";
                        xsSQL = xsSQL + "'" + xsID + "', ";
                        //xsSQL = xsSQL + moCurrDB.ID.ToString() + ", ";
                        xsSQL = xsSQL + "'" + xsDsSQL + "', ";
                        xsSQL = xsSQL + xoForm.Location.X.ToString().Trim() + ", " + xoForm.Location.Y.ToString().Trim() + ", " + xoForm.Width.ToString().Trim() + ", " + xoForm.Height.ToString().Trim();
                        string xs = xoSQL.Text.Trim();
                        xs = xs.Replace("'", "");
                        xsSQL = xsSQL + ", '" + xs + "'";

                        xs = xoSQL.GetDesc();
                        xs = xs.Replace("'", "");
                        xsSQL = xsSQL + ", '" + xs + "'";

                        xsSQL = xsSQL + ", '" + xoSQL.SpecificTable();
                        xsSQL = xsSQL + "', " + iifs(xbLockSQL, "-1", "0");
                        string xsMasterChildInfo = xoSQL.MasterChildPrepForDataStorage();
                        xsSQL = xsSQL + ", '" + xsMasterChildInfo + "'";

                        xs = xoSQL.GetGrpID();
                        xs = xs.Replace("'", "");
                        xsSQL = xsSQL + ", '" + xs + "'";

                        xsSQL = xsSQL + ")";
                        if (!xoAppDB.SQL(xsSQL))
                        {
                            xsErrMsg = xoAppDB.Message + "\r\r" + xsSQL;
                            break;
                        }
                    }
                }
                if (xsErrMsg.Length == 0)
                {
                    // new insertions failed, so restore the previous child dataset records
                    xsSQL = "DELETE * FROM Dataset WHERE datasetID = '" + xsTmpID + "'";
                    if (!xoAppDB.SQL(xsSQL)) xsErrMsg = "(Temporary dataset removal) " + xoAppDB.Message + "\r\r" + xsSQL;
                }
                else
                {
                    // replacement dataset record(s) not inserted, so restore the prior ones situated for removal
                    xsSQL = "UPDATE Dataset SET datasetID = '" + xsID + "' WHERE xsID = '" + xsTmpID + "'";
                    if (!xoAppDB.SQL(xsSQL)) xsErrMsg = xsErrMsg + "\r\n\r\n(Temporary dataset restoration) " + xoAppDB.Message + "\r\r" + xsSQL;
                }

                msCurrentRestoredDsID = xsID;
                msCurrentRestoredDsDesc = xsDesc;
                miCurrentRestoredDbID = moCurrDB.ID;
            }

            Global.CloseThisAppsDatabase(ref xoAppDB);

            if (xsErrMsg.Length > 0) Global.ShowMessage(xsErrMsg, "Save SQLs");
        }

        /// <summary>
        /// restore a set of previously saved SQL Query/Command windows
        /// </summary>
        private void RestoreSQL()
        {
            string xs = "", xsID = "", xsSQL = "", xsErrMsg = "", xsDsDesc = "";  //xsProvider = ""
            //bool xbNeedPswd = false;
            DatabaseToOpen xrDB = new DatabaseToOpen();
            int xii = 0, xiDbID = 0;
            bool xbCanceled = false, xbLockSQL = false;
            bool xbOpened = false;
            DataTable xoTbl = new DataTable();
            DB xoAppDB = new DB();
            object xv;

            dlgChooseSQL xoDlgChooseSQL = new dlgChooseSQL();

            if (!xbCanceled)
            {
                if (Global.OpenThisAppsDatabase(ref xoAppDB))
                {
                    //
                    // first, we get the "datasets" (series of SQL query/command windows) previously opened, and subsequently saved, for restoration
                    //
                    xsSQL = "SELECT datasetID, datasetDescription, dbID FROM Datasets ORDER BY datasetDescription"; // ORDER BY dbID DESC";  //datasetDescription";
                    if (xoAppDB.SQL(xsSQL, xoTbl))
                    {
                        if (xoTbl.Rows.Count > 0)
                        {
                            //
                            // OK - we have one or more previously saved sets, let's present a list box to choose from
                            //
                            string xsLine = "";
                            for (xii = 1; xii <= xoTbl.Rows.Count; xii++)
                            {
                                xs = null2str(xoTbl.Rows[xii - 1][1]).Trim();      // get description

                                xoDlgChooseSQL.lstSQL.Items.Add(xs);                                // store in choose list box

                                xsLine = null2str(xoTbl.Rows[xii - 1][0]).Trim();

                                xv = xoTbl.Rows[xii - 1][2];                // db ID
                                if (xv == null) xiDbID = 0; else xiDbID = Convert.ToInt32(xv);
                                xsLine = xsLine + "," + xiDbID.ToString();
                                xoDlgChooseSQL.lstSQLinfo.Items.Add(xsLine);                        // store in hidden list box
                            }
                            xoDlgChooseSQL.ShowDialog();

                            //
                            // if the user chose one, let's attempt to open (after confirmation if DB is open)
                            //
                            bool xbContinue = false;
                            if (xoDlgChooseSQL.DialogResult == System.Windows.Forms.DialogResult.OK)
                            {
                                xbContinue = true;
                                if (CurrentDBisOpen())
                                {
                                    if (VerifyClose())
                                    {
                                        CloseCurrentDatabaseAndRefreshWindow();
                                    }
                                    else
                                    {
                                        xbContinue = false;
                                    }
                                }
                            }
                            if (xbContinue)
                            {
                                xsDsDesc = xoDlgChooseSQL.lstSQL.Items[xoDlgChooseSQL.lstSQL.SelectedIndex].ToString();
                                xsLine = xoDlgChooseSQL.lstSQLinfo.Items[xoDlgChooseSQL.lstSQL.SelectedIndex].ToString();

                                List<string> xsInfo = xsLine.Split(',').ToList<string>();
                                xsID = xsInfo[0];
                                xiDbID = Convert.ToInt16(xsInfo[1]);

                                xoTbl.Reset();
                                xsSQL = "SELECT ID, providerName, connType, source, [database], userID, connStr, [description] FROM [database] WHERE ID = " + xiDbID.ToString();
                                if (xoAppDB.SQL(xsSQL, xoTbl))
                                {
                                    if (xoTbl.Rows.Count == 0)
                                    {
                                        xsErrMsg = "Database associated to the selected item is no longer in the list.";
                                    }
                                    else
                                    {
                                        DatabaseProvider xrProv = new DatabaseProvider();
                                        xrProv.ConnectionStringOverride = null2str(xoTbl.Rows[0][6]).Trim();
                                        xv = xoTbl.Rows[0][2];
                                        if (xv == null) xv = 0;
                                        int xiConnType = Convert.ToInt16(xv);
                                        xrDB.ConnectionType = (ConnectivityType)xv;
                                        xrDB.Database = null2str(xoTbl.Rows[0][4]).Trim();
                                        xrDB.DataSource = null2str(xoTbl.Rows[0][3]).Trim();
                                        xrDB.Description = null2str(xoTbl.Rows[0][7]).Trim();
                                        xrDB.Password = "";
                                        xrDB.UniqueID = xiDbID;
                                        xrDB.UserID = null2str(xoTbl.Rows[0][5]).Trim();
                                        xrDB.Provider = xrProv;
                                        if (!OpenDbByID(xiDbID)) xsErrMsg = "Restore failed."; else xbOpened = true;
                                        xrDB.Password = msCurrDbPswd;
                                    }
                                }
                                else
                                {
                                    xsErrMsg = xoAppDB.Message + "  (SQL: " + xsSQL + ")";
                                }

                                if (xsErrMsg.Length == 0)
                                {
                                    xoTbl.Reset();
                                    xsSQL = "SELECT sqlText, leftPos, topPos, [width], [height], [caption], sqlDescription, specificTable, lockSQL, masterChildInfo, grpID FROM dataset WHERE datasetID = '" + xsID + "'";
                                    if (xoAppDB.SQL(xsSQL, xoTbl))
                                    {
                                        int xiCol = 0;
                                        string xsCol = xoTbl.Columns[xiCol].ColumnName;
                                        if (xoTbl.Rows.Count > 0)
                                        {
                                            miCurrDb_PK_ID = xiDbID;
                                            for (xii = 1; xii <= xoTbl.Rows.Count; xii++)
                                            {
                                                xsSQL = null2str(xoTbl.Rows[xii - 1][0]).Trim();
                                                xsSQL = xsSQL.Replace(Global.gcc251.ToString(), "'");
                                                xsSQL = xsSQL.Replace(Global.gcc252.ToString(), Global.gccQT.ToString());
                                                string xsCaption = "";
                                                xv = xoTbl.Rows[xii - 1][5];
                                                if (xv != null) xsCaption = xv.ToString().Trim();

                                                string xsDesc = "";
                                                xv = xoTbl.Rows[xii - 1][6];
                                                if (xv != null) xsDesc = xv.ToString().Trim();

                                                string xsGrpID = "";
                                                xv = xoTbl.Rows[xii - 1][10];
                                                if (xv != null) xsGrpID = xv.ToString().Trim();

                                                string xsSpecTbl = "";
                                                xv = xoTbl.Rows[xii - 1][7];
                                                if (xv != null) xsSpecTbl = xv.ToString().Trim();

                                                xbLockSQL = false;
                                                xv = xoTbl.Rows[xii - 1][8];
                                                if (xv != null) xbLockSQL = Convert.ToBoolean(xv);

                                                string xsMasterChildInfo = null2str(xoTbl.Rows[xii - 1][9]).Trim();
                                                // eventually, need to pass the stored GUID to this call, so as to keep master/child linkages when restoring SQLs
                                                frmSQL2 xoFrm = CreateNewSQLWindow(xsSpecTbl, xsSQL, Convert.ToInt16(xoTbl.Rows[xii - 1][1]), Convert.ToInt16(xoTbl.Rows[xii - 1][2]), Convert.ToInt16(xoTbl.Rows[xii - 1][3]), Convert.ToInt16(xoTbl.Rows[xii - 1][4]), xsCaption, xsDesc, xbLockSQL, xsGrpID, true);

                                                if (xoFrm != null)
                                                {
                                                    xoFrm.DbID = xiDbID;
                                                    if (xsMasterChildInfo.Length > 0)
                                                    {
                                                        xoFrm.MasterChildBreakdownFromDataStorage(xsMasterChildInfo);
                                                    }
                                                    xoFrm = null;
                                                }
                                                else
                                                {
                                                    xsErrMsg = "Failed to create new SQL window.";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            xsErrMsg = "No SQLs found for the selected saved set.";
                                        }
                                        msCurrentRestoredDsDesc = xsDsDesc;
                                        msCurrentRestoredDsID = xsID;
                                        moCurrDB.ID = xiDbID;
                                    }
                                    else
                                    {
                                        xsErrMsg = xoAppDB.Message + "  (SQL: " + xsSQL + ")";
                                    }
                                }
                            }
                        }
                        else
                        {
                            xsErrMsg = "There are no saved SQLs to restore.";
                        }
                    }
                    else
                    {
                        xsErrMsg = xoAppDB.Message + "  (SQL: " + xsSQL + ")";
                    }
                }
                else
                {
                     xsErrMsg = xoAppDB.Message;
                }
            }

            ReleaseTable(ref xoTbl);

            Global.CloseThisAppsDatabase(ref xoAppDB);

            xoDlgChooseSQL.Dispose();
            xoDlgChooseSQL = null;

            if (xsErrMsg.Length > 0) Global.ShowMessage(xsErrMsg, "");
        }

        private void btnCascade_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.Cascade);
        }

        private void btnTileVert_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileVertical);
        }

        private void btnTileHorz_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void btnCloseAll_Click(object sender, EventArgs e)
        {
            if (ShowMessage("Close ALL currently open windows/SQLs...please confirm...", "Close All Windows", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                CloseAllMdiWindows();
                RefreshChildWindowList();
                SetControlsAvails();
            }
        }

        /// <summary>
        /// launch the ODBC adminstrator for Windows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnODBC_Click(object sender, EventArgs e)
        {
            string xsErrMsg = "";

            try
            {
                Process.Start("ODBCAD32.EXE");
            }
            catch (Exception xoExc)
            {
                xsErrMsg = xoExc.Message;
            }
            if (xsErrMsg.Length > 0) Global.ShowMessage(xsErrMsg, "ODBC");
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            string xs = "This app allows you to open databases from various vendors, and execute SQL statements and the like on such.";
            xs += "\r\rOriginal version written somewhere in the early '90s with Visual FoxPro.  Subsequent versions written in VB6, VS 2008 VB and C++.  This version written from scratch using Microsoft Visual Studio 2013 in C# in 2015.";

            xs += "\r\n\r\nGoals:\r\n";
            xs += "\r\n   1) Open/peruse any database OLEDB, ODBC, and/or .Net compliant";
            xs += "\r\n   2) Extend ability to the future via an open architecture";
            xs += "\r\n   3) Self-documenting (no help file required)";
            xs += "\r\n   4) Never meant to replace tools for a specific provider";
            xs += "\r\n   5) To learn from user interaction, acting accordingly";
            xs += "\r\n   6) Built upon pure .Net...no third-party mechanisms (independent)";
            xs += "\r\n   7) Allow developers to concentrate on data, not technical access";
            xs += "\r\n\tto such using the associated RebusDataX class";
            xs += "\r\n\tin their own projects";
            xs += "\r\n   8) Improve developer productivity";
            xs += "\r\n   9) Most importantly, as open source, to be further improved upon...";
            xs += "\r\n";
            xs += "\r\rVersion: " + Application.ProductVersion + "  " + Application.ProductName;
            xs += "\r\rExecutable: (" + Application.ExecutablePath + ")";
            xs += "\r\rApp's database (" + Global.gsAppDb + ")";
            ShowMessage(xs, "About " + Application.ProductName);
        }

        /// <summary>
        /// this provides visual linkage between tables, and act accordingly as changes in one show up in another
        /// </summary>
        public void RefreshChildWindowList()
        {
            LoadChildWindows();
            MatchChildWindow();
        }

        public void NeedRefresh()
        {
            tmr.Enabled = true;
        }

        private void tabMenu_Click(object sender, EventArgs e)
        {
            RefreshChildWindowList();
        }

        private void MatchChildWindow()
        {
            int xii = -1;
            frmBaseMDI xoForm = (frmBaseMDI)this.ActiveMdiChild;
            if (xoForm != null)
            {
                int xiActiveID = xoForm.InternalWindowID;
                foreach (int xoID in moChildWindowIDs)
                {
                    xii++;
                    if (xoID == xiActiveID)
                    {
                        drpWindows.SelectedIndex = xii;
                        drpViews2.SelectedIndex = xii;
                        break;
                    }
                }
            }
        }

        private void LoadChildWindows()
        {
            if (drpWindows.Items.Count > 0) drpWindows.Items.Clear();
            if (drpViews2.Items.Count > 0) drpViews2.Items.Clear();
            mbLoadingGrps = true;
            if (drpGrps.Items.Count > 0) drpGrps.Items.Clear();
            moChildWindowIDs = new List<int>(0);
            foreach (frmBaseMDI xoForm in this.MdiChildren)
            {
                drpWindows.Items.Add(xoForm.Text);
                drpViews2.Items.Add(xoForm.Text);
                try
                {
                    frmSQL2 xoSQL = (frmSQL2)xoForm;
                    string xsGrpID = xoSQL.GetGrpID().Trim();
                    if (xsGrpID.Length > 0)
                    {
                        bool xbExists = false;
                        for (int xii = 0; xii < drpGrps.Items.Count; xii++)
                        {
                            if (xsGrpID.ToUpper() == drpGrps.Items[xii].ToString().ToUpper())
                            {
                                xbExists = true;
                                break;
                            }
                        }
                        if (!xbExists)
                        {
                            drpGrps.Items.Add(xsGrpID);
                        }
                    }
                    xoSQL = null;
                }
                catch { }
                // do some "linkage" so when an item selected from this list, it jumps to that window
                int xiID = xoForm.InternalWindowID;
                moChildWindowIDs.Add(xiID);
            }
            mbLoadingGrps = false;
        }

        private void drpWindows_SelectedIndexChanged(object sender, EventArgs e)
        {
            // see "linkage" above
            int xii = drpWindows.SelectedIndex;
            SelectView(xii);
        }

        private void SelectView(int piChildIdx)
        {
            if (piChildIdx >= 0)
            {
                int xiMDIChildIdx = moChildWindowIDs[piChildIdx];
                foreach (frmBaseMDI xoForm in this.MdiChildren)
                {
                    if (xoForm.InternalWindowID == xiMDIChildIdx)
                    {
                        if (xoForm.WindowState == FormWindowState.Minimized)
                        {
                            //xoForm.Activate();
                            //xoForm.Show();
                            //xoForm.Select();
                            //xoForm.Focus();
                            xoForm.BringToFront();
                            xoForm.WindowState = FormWindowState.Normal;
                        }
                        xoForm.Focus();
                        break;
                    }
                }
            }
        }

        private void drpViews2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int xii = drpViews2.SelectedIndex;
            SelectView(xii);
        }

        public void ChildWindowFocused()
        {
            SetControlsAvails();
        }

        /// <summary>
        /// just an internal for development
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stat_DoubleClick(object sender, EventArgs e)
        {
            //test();
            //Global.ShowMessage(System.IO.File.ReadAllText(@"C:\Lee\special_sql.txt"), "Just Testing Around");
        }

        private void LoadExternalProviders()
        {
            string xsErrMsg = "";
            DB xoDB = new DB();

            string xsExternalProvidersFile = Application.StartupPath + @"\" + Global.ExternalDbProvidersFile;
            if (System.IO.File.Exists(xsExternalProvidersFile))
            {
                xoDB.GetExternalProviders(xsExternalProvidersFile, out moExtProvs, out xsErrMsg);
            }

            if (xsErrMsg.Length > 0) Global.ShowMessage(xsErrMsg, "Load External Providers");
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            LoadExternalProviders();
        }

        private void btnExtProvs_Click(object sender, EventArgs e)
        {
            frmManageExtProvs xoFrm = new frmManageExtProvs();
            xoFrm.ShowDialog();
            LoadExternalProviders();
        }

        private void frmMain_ControlRemoved(object sender, ControlEventArgs e)
        {
            RefreshChildWindowList();
        }

        private void tmr_Tick(object sender, EventArgs e)
        {
            tmr.Enabled = false;
            RefreshChildWindowList();
        }

        private bool OpenDbByID(long piDbID)
        {
            bool xbMatched = false;
            for (int xii = 0; xii < mrLoadedDbs.Count; xii++)
            {
                if (mrLoadedDbs[xii].ID == piDbID)
                {
                    OpenSelectedDB(mrLoadedDbs[xii]);
                    xbMatched = true;
                    break;
                }
            }
            if (!xbMatched) ShowMessage("Internal error...did not match database to open.");
            return (xbMatched);
        }

        private void btnPrevDB_Click(object sender, EventArgs e)
        {
            using (frmChooseDB xoFrm = new frmChooseDB())
            {
                xoFrm.LoadDBs(mrLoadedDbs);
                xoFrm.ShowDialog();
                if (xoFrm.OK())
                {
                    long xiDbID = xoFrm.SelectedDbID();
                    OpenDbByID(xiDbID);
                    moCurrDB.ID = (int)xiDbID;
                }
                if (xoFrm.DbsRemoved() || xoFrm.DbListChanged()) LoadDBs();
                xoFrm.Close();
            }
        }

        private void drpGrps_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!mbLoadingGrps)
            {
                if (drpGrps.SelectedIndex >= 0)
                {
                    string xsGrpID = drpGrps.Items[drpGrps.SelectedIndex].ToString().Trim().ToUpper();
                    foreach (frmBaseMDI xoForm in this.MdiChildren)
                    {
                        if (xoForm.FormType == MDIType.SQL)
                        {
                            frmSQL2 xoSQL = (frmSQL2)xoForm;
                            if (xoSQL.GetGrpID().Trim().ToUpper() == xsGrpID)
                            {
                                xoSQL.Focus();
                            }
                            xoSQL = null;
                        }
                    }
                }
            }
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            //Global.ShowMessage("Coming soon... (so far, just to set TOP)");

            frmOptions xoDlg = new frmOptions();
            xoDlg.Top = Global.SelectTop;
            xoDlg.ShowDialog();
            if (xoDlg.OK)
            {
                if (Global.SelectTop != xoDlg.Top)
                {
                    Global.SelectTop = xoDlg.Top;

                    DB xoAppDB = new DB();
                    if (Global.OpenThisAppsDatabase(ref xoAppDB))
                    {
                        string xsWhere =  "WHERE [userInfo] = '" + Environment.UserName + "' AND [section] = 'SQL' AND [entry] = 'SelectTop'";
                        string xsSQL = "DELETE * FROM [Customs] " + xsWhere;
                        if (xoAppDB.SQL(xsSQL))
                        {
                            xsSQL = "INSERT INTO [Customs] ([userInfo], [section], [entry], [value]) VALUES (";
                            xsSQL += "'" + Environment.UserName + "'";
                            xsSQL += ", 'SQL', 'SelectTop'";
                            xsSQL += ", " + Global.SelectTop.ToString() + ")";
                            if (!xoAppDB.SQL(xsSQL)) Global.ShowMessage(xoAppDB.Message + "  (SQL: " + xsSQL + ")", "Save Settings");
                        }
                        else
                        {
                            Global.ShowMessage(xoAppDB.Message + "  (SQL: " + xsSQL + ")", "Save Settings");
                        }
                    }
                    Global.CloseThisAppsDatabase(ref xoAppDB);

                }
            }
            xoDlg.Dispose();
            xoDlg = null;
        }
    }

}
