using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
//using Microsoft.SqlServer.Management.Smo;
using RebusData6;

namespace RebusSQL6
{
    public partial class frmOpenDB : Form
    {
        private bool mbOK;
        public bool OK { get { return mbOK; } set { mbOK = value; } }

        private bool mbAddMode;
        public bool AddMode { get { return mbAddMode; } }

        private DatabaseToOpen moDbToOpen;
        public DatabaseToOpen DbToOpen { get { return moDbToOpen; } set { moDbToOpen = value; } }

        private List<DatabaseProvider> moExtProvs;
        //public List<DatabaseProvider> ExternalProviders { get { return moExtProvs; } set { moExtProvs = value; } }

        private bool mbSqlServerInstancesSet = false;
        private bool mbProgrammaticChangeUnderway = false;

        private bool mbUserChangeConnType = false;

        private string msPswd = "";

        private string msLastConnStr = "";
        //private DB moLastDB;

        //private DB moDB;


        private DB moDummyDB;







        private List<DatabaseProvider> moProviders;

        //
        // constructor
        //
        public frmOpenDB()
        {
            InitializeComponent();
            mbOK = false;
            mbAddMode = false;
            moExtProvs = new List<DatabaseProvider>(0);
            //moDB = GetDB();
            //ProviderFill();
            moDummyDB = new DB();
            moDummyDB.CommandTimeoutSeconds = Global.TimeoutSecs;
            ConnTypeFill();
        }

        public void SetExternalProviders(List<DatabaseProvider> poExtProvs)
        {
            if (poExtProvs != null)
            {
                moExtProvs = poExtProvs;
                LoadProviders();
            }
        }

        public void SetAddMode()
        {
            mbAddMode = true;
            UpdateControlsAvails();
            this.Text = "Add/Open Database";
            this.statLbl.Text = "Select a database previously unopened by this application...it will then be opened and added to the list of previous databases.";
        }

        public void SetDbToOpen(DatabaseToOpen poDbToOpen)
        {
            moDbToOpen = poDbToOpen;
            MatchConnType(poDbToOpen.ConnectionType);
            txtSrc.Text = poDbToOpen.DataSource;
            MatchSrcToDropdown();
            txtDB.Text = poDbToOpen.Database;
            MatchDbToDropdown();
            txtUserID.Text = poDbToOpen.UserID;
            txtPswd.Text = poDbToOpen.Password;
            string xsConnStrOverride = poDbToOpen.Provider.ConnectionStringOverride.Trim().Replace(Global.EmbeddedSingleQt, "'");
            chkManual.Checked = xsConnStrOverride.Length > 0;
            txtConnStr.Text = xsConnStrOverride;
        }

        public void SetSQLServerInstances(List<string> psSQLs)
        {
            if (psSQLs != null)
            {
                drpSQLServerSrc.Items.Clear();
                for (int xii = 0; xii < psSQLs.Count; xii++)
                {
                    drpSQLServerSrc.Items.Add(psSQLs[xii]);
                }
            }

            mbSqlServerInstancesSet = true;
        }

        public void SetDBs(List<string> psDBs)
        {
            if (psDBs != null)
            {
                drpSQLServerDB.Items.Clear();
                for (int xii = 0; xii < psDBs.Count; xii++)
                {
                    drpSQLServerDB.Items.Add(psDBs[xii]);
                }
            }
        }

        public string Password()
        {
            return (msPswd.Trim());
        }

        private void MatchConnType(ConnectivityType piConnType)
        {
            switch (piConnType)
            {
                case ConnectivityType.OleDB:
                    MatchConnType2("ole");
                    break;
                case ConnectivityType.ODBC:
                    MatchConnType2("odbc");
                    break;
                default:
                    MatchConnType2("sql");
                    break;
            }
        }

        private void MatchConnType2(string psLike)
        {
            string xsLike = psLike.ToUpper();

            for (int xii = 0; xii < drpConnType.Items.Count; xii++)
            {
                if (drpConnType.Items[xii].ToString().ToUpper().Contains(xsLike))
                {
                    bool xbSave = mbProgrammaticChangeUnderway;
                    mbProgrammaticChangeUnderway = true;
                    drpConnType.SelectedIndex = xii;
                    mbProgrammaticChangeUnderway = xbSave;
                    break;
                }
            }
        }

        private void MatchDbToDropdown()
        {
            string xsDb = txtDB.Text.Trim().ToUpper();

            for (int xii = 0; xii < drpSQLServerDB.Items.Count; xii++)
            {
                string xs = drpSQLServerDB.Items[xii].ToString().Trim().ToUpper();
                if (xsDb == xs)
                {
                    drpSQLServerDB.SelectedIndex = xii;
                    break;
                }
            }
        }

        private void MatchSrcToDropdown()
        {
            string xsDs = txtSrc.Text.Trim().ToUpper();

            for (int xii = 0; xii < drpSQLServerSrc.Items.Count; xii++)
            {
                string xs = drpSQLServerSrc.Items[xii].ToString().Trim().ToUpper();
                if (xsDs == xs)
                {
                    drpSQLServerSrc.SelectedIndex = xii;
                    break;
                }
            }
        }

        private void MatchBrandToProvider2(string psLike)
        {
            string xsLike = psLike.ToUpper();

            for (int xii = 0; xii < drpProv.Items.Count; xii++)
            {
                if (drpProv.Items[xii].ToString().ToUpper().Contains(xsLike))
                {
                    drpProv.SelectedIndex = xii;
                    break;
                }
            }
        }

        private void ProviderFill()
        {
            drpProv.Items.Clear();

            int xiSelIdx = -1;

            for (int xii = 0; xii < moProviders.Count; xii++)
            {
                string xs = moProviders[xii].Name;
                drpProv.Items.Add(xs);
                if (xiSelIdx < 0)
                {
                    string xs2 = xs.ToUpper();
                    if (xs2.IndexOf("SQL SERV") > 0) xiSelIdx = xii;
                }
            }
            drpProv.SelectedIndex = xiSelIdx;
        }

        private void ConnTypeFill()
        {
            bool xbSave = mbProgrammaticChangeUnderway;
            mbProgrammaticChangeUnderway = true;
            drpConnType.Items.Clear();
            drpConnType.Items.Add("OleDB");
            drpConnType.Items.Add("ODBC");
            drpConnType.Items.Add("SQL");
            drpConnType.SelectedIndex = 2;
            mbProgrammaticChangeUnderway = xbSave;
        }

        private void UpdateControlsAvails()
        {
            bool xbTest = false;

            if (mbAddMode)
            {
                lblDesc.Enabled = true;
                txtDesc.Enabled = true;

                if (chkManual.Checked)
                {
                    lblConnType.Enabled = true;
                    drpConnType.Enabled = true;
                    lblProv.Enabled = false;
                    drpProv.Enabled = false;
                    lblSrc.Enabled = false;
                    txtSrc.Enabled = false;
                    lblDB.Enabled = false;
                    txtDB.Enabled = false;
                    btnBrowse.Enabled = false;
                    drpSQLServerDB.Enabled = false;
                    drpSQLServerSrc.Enabled = false;
                    lblUser.Enabled = false;
                    txtUserID.Enabled = false;
                    lblPswd.Enabled = false;
                    txtPswd.Enabled = false;
                    lblConnStr.Enabled = true;
                    txtConnStr.Enabled = true;
                    btnTest.Enabled = txtConnStr.Text.Trim().Length > 0;
                    btnOpen.Enabled = btnTest.Enabled;
                }
                else
                {
                    if (drpProv.SelectedIndex >= 0)
                    {
                        if (moProviders[drpProv.SelectedIndex].IsMsSQLServer)
                        {
                            lblConnType.Enabled = true;
                            drpConnType.Enabled = true;
                            lblProv.Enabled = true;
                            drpProv.Enabled = true;
                            lblSrc.Enabled = true;
                            txtSrc.Enabled = false;
                            txtSrc.Visible = false;
                            drpSQLServerSrc.Visible = true;
                            drpSQLServerSrc.Enabled = true;
                            lblDB.Enabled = true;
                            txtDB.Enabled = false;
                            txtDB.Visible = false;
                            drpSQLServerDB.Visible = true;
                            drpSQLServerDB.Enabled = true;
                            btnBrowse.Enabled = false;
                            lblUser.Enabled = true;
                            txtUserID.Enabled = true;
                            lblPswd.Enabled = true;
                            txtPswd.Enabled = true;

                            xbTest = (drpConnType.SelectedIndex >= 0 && drpProv.SelectedIndex >= 0 && drpSQLServerSrc.Text.Trim().Length > 0 && drpSQLServerDB.Text.Trim().Length > 0);

                            btnTest.Enabled = xbTest;
                            btnOpen.Enabled = xbTest;
                        }
                        else
                        {
                            lblConnType.Enabled = true;
                            drpConnType.Enabled = true;
                            lblProv.Enabled = true;
                            drpProv.Enabled = true;
                            lblSrc.Enabled = true;
                            txtSrc.Enabled = true;
                            txtSrc.Visible = true;
                            drpSQLServerSrc.Visible = false;
                            drpSQLServerSrc.Enabled = false;
                            lblDB.Enabled = true;
                            txtDB.Enabled = true;
                            txtDB.Visible = true;
                            drpSQLServerDB.Visible = false;
                            drpSQLServerDB.Enabled = false;
                            btnBrowse.Enabled = true;
                            lblUser.Enabled = true;
                            txtUserID.Enabled = true;
                            lblPswd.Enabled = true;
                            txtPswd.Enabled = true;

                            xbTest = (drpConnType.SelectedIndex >= 0 && drpProv.SelectedIndex >= 0 && txtDB.Text.Trim().Length > 0);

                            btnTest.Enabled = xbTest;
                            btnOpen.Enabled = xbTest;
                        }
                    }
                    else
                    {
                        if (drpProv.SelectedIndex >= 0 && !moProviders[drpProv.SelectedIndex].IsMsSQLServer)
                        {
                            txtSrc.Enabled = false;
                            btnBrowse.Enabled = true;
                        }
                        else
                        {
                            txtSrc.Enabled = true;
                            btnBrowse.Enabled = txtSrc.Text.Trim().Length > 0;
                        }
                        lblConnType.Enabled = true;
                        drpConnType.Enabled = true;
                        lblProv.Enabled = true;
                        drpProv.Enabled = true;
                        lblSrc.Enabled = true;
                        txtSrc.Visible = true;
                        //txtSrc.Enabled = true;
                        drpSQLServerSrc.Visible = false;
                        lblDB.Enabled = true;
                        txtDB.Visible = true;
                        txtDB.Enabled = true;
                        drpSQLServerDB.Visible = false;
                        lblUser.Enabled = true;
                        txtUserID.Enabled = true;
                        lblPswd.Enabled = true;
                        txtPswd.Enabled = true;
                    }
                    panManual.Enabled = true;
                    lblConnStr.Enabled = (chkManual.Checked);
                    txtConnStr.Enabled = (chkManual.Checked);
                }
            }

            else
            {
                lblConnType.Enabled = false;
                drpConnType.Enabled = false;
                lblProv.Enabled = false;
                drpProv.Enabled = false;
                lblSrc.Enabled = false;
                txtSrc.Enabled = false;
                lblUser.Enabled = false;
                txtUserID.Enabled = false;
                lblDB.Enabled = false;
                txtDB.Enabled = false;
                btnBrowse.Enabled = false;
                panManual.Enabled = false;
                lblDesc.Enabled = false;
                txtDesc.Enabled = false;
            }
        }

        private void frmOpenDB_Load(object sender, EventArgs e)
        {
            LoadProviders();
            //ProviderFill();
            drpSQLServerDB.Left = txtDB.Left;
            drpSQLServerDB.Top = txtDB.Top;
            drpSQLServerSrc.Left = txtSrc.Left;
            drpSQLServerSrc.Top = txtSrc.Top;
        }

        private void btnCanc_Click(object sender, EventArgs e)
        {
            mbOK = false;
            this.Hide();
        }

        private ConnectivityType GetSelectedConnType()
        {
            ConnectivityType xeType = ConnectivityType.Unknown;

            if (OleDbSelected())
            {
                xeType = ConnectivityType.OleDB;
            }
            else
            {
                if (OdbcSelected())
                {
                    xeType = ConnectivityType.ODBC;
                }
                else
                {
                    if (SqlSelected()) xeType = ConnectivityType.DotNet;
                }
            }

            return (xeType);
        }
        
        private ConnectivityType GetConnTypeFromSelectedConnType()
        {
            ConnectivityType xoConnType = ConnectivityType.Unknown;

            if (drpConnType.SelectedIndex >= 0)
            {
                if (OleDbSelected())
                {
                    xoConnType = ConnectivityType.OleDB;
                }
                else
                {
                    if (OdbcSelected())
                    {
                        xoConnType = ConnectivityType.ODBC;
                    }
                    else
                    {
                        if (SqlSelected()) xoConnType = ConnectivityType.DotNet;
                    }
                }
            }

            return (xoConnType);
        }

        private string GetSelectedProviderName()
        {
            string xsName = "";

            if (drpProv.SelectedIndex >= 0)
            {
                xsName = moProviders[drpProv.SelectedIndex].Name;
            }

            return (xsName);
        }

        private string GetSelectedDataSource()
        {
            string xsDs = "";

            if (mbAddMode)
            {
                if (drpSQLServerSrc.Enabled)
                {
                    if (drpSQLServerSrc.SelectedIndex >= 0)
                    {
                        xsDs = drpSQLServerSrc.Items[drpSQLServerSrc.SelectedIndex].ToString();
                    }
                    else
                    {
                        xsDs = drpSQLServerSrc.Text.Trim();
                    }
                }
                else
                {
                    xsDs = txtSrc.Text.Trim();
                }
            }
            else
            {
                xsDs = txtSrc.Text.Trim();
            }

            return (xsDs);
        }

        private string GetSelectedDatabase()
        {
            string xsDb = "";

            if (mbAddMode)
            {
                if (drpSQLServerDB.Enabled)
                {
                    if (drpSQLServerDB.SelectedIndex >= 0)
                    {
                        xsDb = drpSQLServerDB.Items[drpSQLServerDB.SelectedIndex].ToString();
                    }
                    else
                    {
                        xsDb = drpSQLServerDB.Text.Trim();
                    }
                }
                else
                {
                    xsDb = txtDB.Text.Trim();
                }
            }
            else
            {
                xsDb = txtDB.Text.Trim();
            }

            return (xsDb);
        }

        //private DatabaseProvider GetSelectedProvider()
        //{
        //    DatabaseProvider xoProv = new DatabaseProvider();

        //    xoProv.

        //    return (xoProv);
        //}

        private void btnOpen_Click(object sender, EventArgs e)
        {
            const string xcsTitle = "Open Connection";
            string xsErrMsg = "", xsSuccMsg = "";

            msPswd = txtPswd.Text.Trim();

            xsErrMsg = CheckForConnStrMatch();

            if (xsErrMsg.Length == 0)
            {
                if (TestOpen(out xsErrMsg, out xsSuccMsg))
                {
                    moDbToOpen = GetSelectedDatabaseToOpen();
                    //if (moDbToOpen.UserID.Trim().Length > 0) moDbToOpen.NeedPassword = true;

                    mbOK = true;
                    this.Hide();
                }
                else
                {
                    Global.ShowMessage(xsErrMsg, xcsTitle);
                }
            }
            else
            {
                Global.ShowMessage(xsErrMsg, xcsTitle);
            }
        }

        private void LoadProviders()
        {
            DB xoDB = GetDB();

            moProviders = new List<DatabaseProvider>(0);

            // load intrinsic providers fro the DB object
            for (int xii = 0; xii < xoDB.Providers.Count; xii++)
            {
                moProviders.Add(xoDB.Providers[xii]);
            }

            // load external providers provided by the caller
            if (moExtProvs.Count > 0)
            {
                for (int xii = 0; xii < moExtProvs.Count; xii++)
                {
                    moProviders.Add(moExtProvs[xii]);
                }
            }

            // cleanup
            xoDB = null;

            // load the providers drow-down list
            ProviderFill();
        }

        private DB GetDB()
        {
            //string xsErrMsg = "";
            DB xoDB = new DB();
            xoDB.CommandTimeoutSeconds = Global.TimeoutSecs;

            //string xsExternalProvidersXMLFile = Application.StartupPath + @"\" + Global.ExternalDbProvidersFile;
            //xoDB.AddExternalProviders(xsExternalProvidersXMLFile, out xsErrMsg);


            //RebusNew.Things xo = new RebusNew.Things();

            //xo.GetThingsFromFile(@"c:\temp\rebusdatabaseproviders.txt");

            //string xs = xo.Thingies[0].GetPropertyValue("OleDbConnStr");



            return (xoDB);
        }

        private DatabaseToOpen GetSelectedDatabaseToOpen()
        {
            DatabaseToOpen xoDbToOpen = new DatabaseToOpen();
            string xsConnStrOverride = txtConnStr.Text.Trim();
            if (!chkManual.Checked) xsConnStrOverride = "";

            xoDbToOpen.Description = txtDesc.Text.Trim();
            xoDbToOpen.Provider = moProviders[drpProv.SelectedIndex];
            xoDbToOpen.Provider.ConnectionStringOverride = xsConnStrOverride;
            xoDbToOpen.Database = GetSelectedDatabase();
            xoDbToOpen.DataSource = GetSelectedDataSource();
            xoDbToOpen.ConnectionType = GetSelectedConnType();
            xoDbToOpen.UserID = txtUserID.Text.Trim();
            xoDbToOpen.Password = txtPswd.Text.Trim();

            return (xoDbToOpen);
        }

        private bool TestOpen(out string psErrMsg, out string psSuccMsg)
        {
            string xsErrMsg = "", xsSuccMsg = "";
            string xsSrc = txtSrc.Text.Trim();

            Cursor xoOrgCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            DB xoDB = new DB();
            DatabaseToOpen xoDbToOpen = GetSelectedDatabaseToOpen();
            string xs = xoDbToOpen.Provider.ConnectionStringOverride.Trim();
            if (xs.Length > 0)
            {
                xs = xs.Replace("\r", "");
                xoDbToOpen.Provider.ConnectionStringOverride = xs.Replace("\n", "");
            }

            if (xoDB.OpenDatabase(xoDbToOpen))
            {
                xsSuccMsg = "Database successfully connected.";
            }
            else
            {
                xsErrMsg = xoDB.Message;
            }
            msLastConnStr = xoDB.Connection.Info;

            Cursor.Current = xoOrgCursor;

            psErrMsg = xsErrMsg;
            psSuccMsg = xsSuccMsg;
            return (xsErrMsg.Length == 0);
        }

        private string CheckForConnStrMatch()
        {
            string xsErrMsg = "";

            if (!chkManual.Checked)
            {
                if (drpProv.SelectedIndex >= 0)
                {
                    DatabaseProvider xoProv = moProviders[drpProv.SelectedIndex];
                    if (drpConnType.SelectedIndex >= 0)
                    {
                        ConnectivityType xeConnType = GetConnTypeFromSelectedConnType();
                        string xs = "";
                        switch (xeConnType)
                        {
                            case ConnectivityType.DotNet:
                                xs = xoProv.DotNetConnStr.Trim() + xoProv.DotNetConnStrTrusted.Trim();
                                if (xs.Length == 0) xsErrMsg = "Connection type is SQL, but a connection string has not been provided for the selected provider.";
                                break;
                            case ConnectivityType.ODBC:
                                xs = xoProv.OdbcConnStr.Trim() + xoProv.OdbcConnStrTrusted.Trim();
                                if (xs.Length == 0) xsErrMsg = "Connection type is ODBC, but a connection string has not been provided for the selected provider.";
                                break;
                            case ConnectivityType.OleDB:
                                xs = xoProv.OleDbConnStr.Trim() + xoProv.OleDbConnStrTrusted.Trim();
                                if (xs.Length == 0) xsErrMsg = "Connection type is OLEDB, but a connection string has not been provided for the selected provider.";
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return (xsErrMsg);
        }

        private void UpdateHistory()
        {
            //History (histType INTEGER, histValue CHAR(255))";
            //string xsSQL = "SELECT histValue FROM History WHERE histType = 1";
            // link to app's db and process accordingly...
            // thisapp
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            const string xcsTitle = "Test Connection";
            string xsErrMsg = "", xsSuccMsg;

            xsErrMsg = CheckForConnStrMatch();

            if (xsErrMsg.Length == 0)
            {
                Cursor xoOrgCursor = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;

                if (TestOpen(out xsErrMsg, out xsSuccMsg))
                {
                    Cursor.Current = xoOrgCursor;
                    Global.ShowMessage(xsSuccMsg, xcsTitle);
                    UpdateHistory();
                }
                else
                {
                    Cursor.Current = xoOrgCursor;
                    Global.ShowMessage(xsErrMsg, xcsTitle);
                }
            }
            else
            {
                Global.ShowMessage(xsErrMsg, xcsTitle);
            }
        }

        private bool OleDbSelected()
        {
            string xs = "";
            if (drpConnType.SelectedIndex >= 0)
            {
                xs = drpConnType.Items[drpConnType.SelectedIndex].ToString().ToUpper();
            }
            return (xs.IndexOf("OLE") >= 0);
        }

        private bool OdbcSelected()
        {
            string xs = "";
            if (drpConnType.SelectedIndex >= 0)
            {
                xs = drpConnType.Items[drpConnType.SelectedIndex].ToString().ToUpper();
            }
            return (xs.IndexOf("ODBC") >= 0);
        }

        private bool SqlSelected()
        {
            string xs = "";
            if (drpConnType.SelectedIndex >= 0)
            {
                xs = drpConnType.Items[drpConnType.SelectedIndex].ToString().ToUpper();
            }
            return (xs.IndexOf("SQL") >= 0);
        }

        private void frmOpenDB_Activated(object sender, EventArgs e)
        {
            UpdateControlsAvails();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            //if (MsAccessSelected())
            //{
            //    dlg.Filter = "Access files (*.mdb, *.accdb)|*.mdb;*.accd*|All files (*.*)|*.*";
            //}
            //else
            //{
            //    dlg.Filter = "Database files (*.db*)|*.db*";
            //}
            //DialogResult xe = dlg.ShowDialog();
            //if (xe == DialogResult.OK || xe == DialogResult.Yes)
            //{
            //    txtDB.Text = dlg.FileName;
            //}
            if (drpProv.SelectedIndex >= 0)
            {
                string xsFilter = moProviders[drpProv.SelectedIndex].FilterExpression.Trim();
                if (xsFilter.Length > 0)
                {
                    dlg.InitialDirectory = Application.ExecutablePath;
                    dlg.Multiselect = false;
                    dlg.Title = "Choose Database File";
                    dlg.Filter = xsFilter;
                    DialogResult xe = dlg.ShowDialog();
                    if (xe == DialogResult.OK || xe == DialogResult.Yes)
                    {
                        txtDB.Text = dlg.FileName;
                    }
                }
            }
            UpdateControlsAvails();
        }

        private void drpConnType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!mbProgrammaticChangeUnderway)
            {
                mbUserChangeConnType = true;
                UpdateControlsAvails();
            }
        }

        private void MatchProvderToConnectionType(string psID)
        {
            for (int xii = 0; xii < moDummyDB.Providers.Count; xii++)
            {
                if (psID == "OLE")
                {
                    if (moDummyDB.Providers[xii].OleDbConnStr.Length > 0)
                    {
                        MatchConnType2("OLE");
                        break;
                    }
                }
            }
        }

        private string SingleConnType(DatabaseProvider poProv)
        {
            string xsID = "";
            string xsOLE = "", xsODBC = "", xsNET = "";

            xsOLE = poProv.OleDbConnStr.Trim() + poProv.OleDbConnStrTrusted.Trim();
            xsODBC = poProv.OdbcConnStr.Trim() + poProv.OdbcConnStrTrusted.Trim();
            xsNET = poProv.DotNetConnStr.Trim() + poProv.DotNetConnStrTrusted.Trim();

            int xiConnTypes = 0;
            if (xsOLE.Length > 0) xiConnTypes++;
            if (xsODBC.Length > 0) xiConnTypes++;
            if (xsNET.Length > 0) xiConnTypes++;

            if (xiConnTypes == 1)
            {
                if (xsOLE.Length > 0) xsID = "OLE";
                if (xsODBC.Length > 0) xsID = "ODBC";
                if (xsNET.Length > 0) xsID = "SQL";
            }

            return (xsID);
        }

        private void drpProv_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!mbUserChangeConnType)      // only do this if user has not yet manually changed the connection type
            {
                if (drpProv.SelectedIndex >= 0)
                {
                    if (drpProv.Enabled)
                    {
                        // let's select "preferred" type
                        string xsID = "";
                        ConnectivityType xeConnType = moProviders[drpProv.SelectedIndex].PreferredConnectionType;
                        switch (xeConnType)
                        {
                            case ConnectivityType.OleDB:
                                xsID = "OLE";
                                break;
                            case ConnectivityType.ODBC:
                                xsID = "ODBC";
                                break;
                            case ConnectivityType.DotNet:
                                xsID = "SQL";
                                break;
                            default:
                                xsID = SingleConnType(moProviders[drpProv.SelectedIndex]);      // if selected provide only has one conn type, we'll choose it for them
                                break;
                        }
                        if (xsID.Length > 0) MatchConnType2(xsID);
                    }
                }
            }
            UpdateControlsAvails();
        }

        private void txtSrc_TextChanged(object sender, EventArgs e)
        {
            UpdateControlsAvails();
        }

        private void txtDB_TextChanged(object sender, EventArgs e)
        {
            UpdateControlsAvails();
        }

        private void drpSQLServerSrc_SelectedIndexChanged(object sender, EventArgs e)
        {
            //DB xoDB = GetDB();
            //drpSQLServerDB.Items.Clear();
            //txtSrc.Text = "";
            //if (drpSQLServerSrc.SelectedIndex >= 0)
            //{
            //    txtSrc.Text = this.drpSQLServerSrc.Items[drpSQLServerSrc.SelectedIndex].ToString() + @"\SQLEXPRESS";
            //    List<string> xsDBs = xoDB.GetDBsFromSQLServer(txtSrc.Text);
            //    for (int xii = 0; xii < xsDBs.Count; xii++)
            //    {
            //        drpSQLServerDB.Items.Add(xsDBs[xii]);
            //    }
            //}
            //xoDB = null;
            //UpdateControlsAvails();
        }

        private void drpSQLServerDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!mbProgrammaticChangeUnderway)
            {
                txtDB.Text = "";
                if (drpSQLServerDB.SelectedIndex >= 0)
                {
                    txtDB.Text = this.drpSQLServerDB.Items[drpSQLServerDB.SelectedIndex].ToString();
                }
                UpdateControlsAvails();
            }
        }

        private void frmOpenDB_Shown(object sender, EventArgs e)
        {
            dlg.FileName = "";
            if (!mbSqlServerInstancesSet)
            {
                Cursor xoOrgCursor = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;

                //DB xoDB = new DB();
                DB xoDB = GetDB();
                List<string> xsSQLs = xoDB.GetSQLServersRunning();
                drpSQLServerSrc.Items.Clear();
                for (int xii = 0; xii < xsSQLs.Count; xii++)
                {
                    drpSQLServerSrc.Items.Add(xsSQLs[xii]);
                }
                xoDB = null;

                Cursor.Current = xoOrgCursor;
            }
            //LoadProviders();
        }

        private void ShowLastConnStr()
        {
            Global.ShowMessage(msLastConnStr, "Last Connection String");
        }

        private void frmOpenDB_DoubleClick(object sender, EventArgs e)
        {
            //DataTable xo = SmoApplication.EnumAvailableSqlServers();
            //DataTable xo = System.Data.Sql.SqlDataSourceEnumerator.Instance.GetDataSources();
            //xo.Dispose();
            //xo = null;

            ShowLastConnStr();
        }

        private void chkManual_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControlsAvails();
        }

        private void chkManual_Click(object sender, EventArgs e)
        {
            UpdateControlsAvails();
        }

        private void txtConnStr_TextChanged(object sender, EventArgs e)
        {
            UpdateControlsAvails();
        }

        private void frmOpenDB_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (moDummyDB != null)
            {
                moDummyDB.CloseDatabase();
                moDummyDB = null;
            }
         }

        private void drpSQLServerSrc_TextChanged(object sender, EventArgs e)
        {
            UpdateControlsAvails();
        }

        private void drpSQLServerDB_TextChanged(object sender, EventArgs e)
        {
            if (!mbProgrammaticChangeUnderway)
            {
                UpdateControlsAvails();
            }
        }

        private void panBtns_DoubleClick(object sender, EventArgs e)
        {
            ShowLastConnStr();
        }

        private void GetSSI()
        {
            System.Data.Sql.SqlDataSourceEnumerator xoDbEnumerator = System.Data.Sql.SqlDataSourceEnumerator.Instance;
            System.Data.DataTable xoDataTable = xoDbEnumerator.GetDataSources();
            //DataTable xoDataTable = SmoApplication.EnumAvailableSqlServers();

            if (xoDataTable.Rows.Count > 0)
            {
                for (int xiRow = 0; xiRow < xoDataTable.Rows.Count; xiRow++)
                {
                    string xsInfo = "";
                    for (int xiFld = 0; xiFld < xoDataTable.Columns.Count; xiFld++)
                    {
                        if (xiFld > 0) xsInfo += "\r\n";
                        xsInfo += xoDataTable.Columns[xiFld].ColumnName + ": ";
                        xsInfo += xoDataTable.Rows[xiRow][xiFld].ToString();
                    }
                    MessageBox.Show(xsInfo);

                }

                SqlConnection xoConn = new SqlConnection();

                //"Server={DS};Database={DB};User Id={UID};Password={PSWD};";
                xoConn.ConnectionString = "Server=LEE-PC\\ENTERPRISE2012;Trusted_Connection=True;";

                xoConn.Open();

                SqlCommand xoCmd = new SqlCommand();
                xoCmd.CommandText = "SELECT [name] FROM master.sys.databases";
                xoCmd.CommandType = CommandType.Text;
                xoCmd.Connection = xoConn;
                SqlDataReader xoRdr = xoCmd.ExecuteReader();

                if (xoRdr.HasRows)
                {
                    string xsDBs = "";
                    xoRdr.Read();
                    while (true)
                    {
                        object xo = xoRdr.GetValue(0);
                        xsDBs = xsDBs + "\r\n" + xo.ToString();
                        if (!xoRdr.Read()) break;
                    }
                    MessageBox.Show(xsDBs);
                }

                xoRdr.Dispose();
                xoCmd.Dispose();


            }

            xoDbEnumerator = null;

            xoDataTable.Dispose();
            xoDataTable = null;

        }

        private void lblSrc_Click(object sender, EventArgs e)
        {
            GetSSI();
        }

        private void drpSQLServerSrc_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.Delete)
            {
                Global.DropHistory(1, drpSQLServerSrc.Text);
            }
            else
            {
                if (e.Alt && e.KeyCode == Keys.S)
                {
                    SortList(drpSQLServerSrc);
                }
            }
        }

        private void drpSQLServerDB_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.Delete)
            {
                Global.DropHistory(2, drpSQLServerDB.Text);
            }
            else
            {
                if (e.Alt && e.KeyCode == Keys.S)
                {
                    SortList(drpSQLServerDB);
                }
            }
        }

        private void SortList(ComboBox xoLst)
        {
            bool xbOrgProgrammaticChangeUnderway = mbProgrammaticChangeUnderway;
            mbProgrammaticChangeUnderway = true;

            if (xoLst.Items.Count > 1)
            {
                int xiOrgIdx = xoLst.SelectedIndex;
                string xsOrgTxt = xoLst.Text;
                List<string> xsItems = new List<string>(0);
                for (int xii = 0; xii < xoLst.Items.Count; xii++)
                {
                    xsItems.Add(xoLst.Items[xii].ToString());
                }
                xsItems.Sort();
                xoLst.Items.Clear();
                int xiNewIdx = -1, xiCount = 0;
                foreach (string xsItem in xsItems)
                {
                    xiCount++;
                    if (xiOrgIdx >= 0 && xiNewIdx < 0)
                    {
                        if (xsItem == xsOrgTxt) xiNewIdx = xiCount - 1;
                    }
                    xoLst.Items.Add(xsItem);
                }
                if (xiNewIdx >= 0)
                {
                    xoLst.SelectedIndex = xiNewIdx;
                }
            }

            mbProgrammaticChangeUnderway = xbOrgProgrammaticChangeUnderway;
        }

    }
}
