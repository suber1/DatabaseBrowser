using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

//
// original concept and writing by Lee Tyler, somewhere in the early 90's
//
// the attempt here is to alleviate the developer of a RAD project to
// not have to worry about connection specifics, but instead to
// concentrate on the application...I have been there so many times,
// hence this code...in addition, many times I found myself in
// data-driven project, with little or no docs, so the first
// step was to learn as much about the database as possible...
// and the database "vendor" was various, and perhaps without the tools
// associated with the vendor to sniff around...this code gave me the
// opportunity to examine the data/schemas in an independent fashion, and
// get into the head of developers ASAP...I have used this concept many
// times to successfully design custom software based on a database/application
// of which I was totally unaware at the start of such...
//
// first major verion in VB6 (actually, an original was VFP, no kidding!)
//
// re-engineered DotNet somewhere around 2005?...in VB.Net
//
// tweeked in 2013
//
// re-written in C# late 2013 (with another version ported to VB.Net around the same time)
//
// nearly total re-write in 2014-2015, again in C#
//
// fall 2015 - complete re-engineer of provider interface, locking in the defaults, but opening up the ability to add at will
//
// the latest, vastly extended from the 2014-2105 rewrite, April-September 2015
//
// tweaks, tweaks and more tweaks 2016-2017 (to this day, use the tool often, so continually adding bells and whistles)
//


namespace RebusData6
{
    /// <summary>
    /// database brands support by RebusData6
    /// </summary>
    public enum Brand
    {
        Unknown = -1,       // default, but must be set
        MsAccess = 0,
        SqlSrvr = 1,
        VFP = 2,
        Oracle = 3,
        SQLite = 4
    }

    public enum DatatypeJSON { String, Number, Boolean }

    /// <summary>
    /// method of connection
    /// </summary>
    public enum ConnectivityType
    {
        Unknown = -1,
        OleDB = 0,      // the default
        ODBC = 1,
        DotNet = 2
    }

    public struct DatabaseToOpen
    {
        public string Description;
        public DatabaseProvider Provider;
        public ConnectivityType ConnectionType;
        //public Brand Brand;
        public string DataSource;
        public string Database;
        public string UserID;
        public string Password;
        //public bool NeedPassword;
        public int UniqueID;            // based on the ID auto-incremented/assigned to it when stored (application-specified)
    }

    public enum IntrinsicProviderType
    {
        NotIntrinsic = -1,
        AccessMdbOleDB = 0,
        AccessAccdbDB = 1,
        SqlServer = 2,
        VFP = 3
    }
    
    public struct DatabaseProvider
    {
        //<Name="MySQL" OdbcConnStr="Driver={MySQL ODBC 5.2 ANSI Driver};Server=localhost;Database={DB};User={UID};Password={PSWD};Option=3;" DotNetConnStr="Server=xxx.xxx.xxx.xxx;Database=myDataBase;Uid={UID};Pwd={PSWD};" OleDbConnStr = "Provider=MySQLProv;Data Source={DB};User Id={UID};Password={PSWD};"
        public string Name;
        public ConnectivityType PreferredConnectionType;
        public string OdbcConnStr;
        public string OdbcConnStrTrusted;
        public string OleDbConnStr;
        public string OleDbConnStrTrusted;
        public string DotNetConnStr;
        public string DotNetConnStrTrusted;
        public string FilterExpression;
        public IntrinsicProviderType IntrinsicProvider;
        //public string NonIntrinsicProviderID;
        //public bool Dropable;
        //public bool ServerBased;
        public string ConnectionStringOverride;
        public bool IsMsSQLServer;
        public bool IsAccess;
        public bool IsOracle;
        public string TopPhrase;        // i.e. "TOP(500)"
    }

    public struct DataField
    {
        public string Name;
        public Type Type;
    }

    /// <summary>
    /// connection objects holds various properties of the current connection/attempted or otherwise
    /// </summary>
    public partial class Connection
    {
        #region connectionInterface


        public BindingSource Binding { get { return moBinding; } set { moBinding = value; } }
        private BindingSource moBinding;


        /// <summary>
        /// for simple database systems, the file name, otherwise for i.e. SQL Server, the data server source
        /// </summary>
        public string DataSource { get { return msDataSource; } }   //set { msFileNameOrDataSource = value; } }
        internal string msDataSource;

        public string DataBase { get { return msDataBase; } }   //set { msFileNameOrDataSource = value; } }
        internal string msDataBase;

        /// <summary>
        /// the User ID, if required for a connection
        /// </summary>
        public string UserID { get { return msUserID; } }   // set { msUserID = value; } }
        internal string msUserID;

        /// <summary>
        /// the Password, if required for a connection
        /// </summary>
        public string Password { get { return msPswd; } }   // set { msPswd = value; } }
        internal string msPswd;

        /// <summary>
        /// for now, holds the constructed connection string used to connect to the database
        /// </summary>
        public string Info { get { return msInfo; } }   //set { msInfo = value; } }
        internal string msInfo;

        /// <summary>
        /// for caller's use
        /// </summary>
        public string Tag { get { return msTag; } set { msTag = value; } }
        internal string msTag;

        /// <summary>
        /// connection object, for DotNet-style connections
        /// </summary>
        public SqlConnection Conn { get { return moConn; } }
        internal SqlConnection moConn;

        /// <summary>
        /// the data adapter object, for DotNet-style connections
        /// </summary>
        public SqlDataAdapter Adapter { get { return moAdapter; } }
        internal SqlDataAdapter moAdapter;

        /// <summary>
        /// the command object, for DotNet-style connections
        /// </summary>
        public SqlCommand Command { get { return moCommand; } }
        internal SqlCommand moCommand;

        /// <summary>
        /// the command builder object, for DotNet-style connections
        /// </summary>
        public SqlCommandBuilder Builder { get { return moBldr; } }
        internal SqlCommandBuilder moBldr;

        /// <summary>
        /// the OLEDB connection object
        /// </summary>
        public OleDbConnection OleConnection { get { return moConnOLE; } }
        internal OleDbConnection moConnOLE;

        /// <summary>
        /// the OLEDB data adapter object
        /// </summary>
        public OleDbDataAdapter OleAdapter { get { return moAdapterOLE; } }
        internal OleDbDataAdapter moAdapterOLE;

        /// <summary>
        /// the OLEDB command object
        /// </summary>
        public OleDbCommand OleCommand { get { return moCmdOLE; } }
        internal OleDbCommand moCmdOLE;

        /// <summary>
        /// the OLEDB command builder object
        /// </summary>
        public OleDbCommandBuilder OleCommandBuilder { get { return moBldrOLE; } }
        internal OleDbCommandBuilder moBldrOLE;

        /// <summary>
        /// the ODBC connection object
        /// </summary>
        public OdbcConnection OdbcConnection { get { return moConnODBC; } }
        internal OdbcConnection moConnODBC;

        /// <summary>
        /// the ODBC data adapter object
        /// </summary>
        public OdbcDataAdapter OdbcAdapter { get { return moAdapterODBC; } }
        internal OdbcDataAdapter moAdapterODBC;

        /// <summary>
        /// the ODBC command object
        /// </summary>
        public OdbcCommand OdbcCmd { get { return moCmdODBC; } }
        internal OdbcCommand moCmdODBC;

        /// <summary>
        /// the ODBC command builder object
        /// </summary>
        public OdbcCommandBuilder OdbcBldr { get { return moBldrODBC; } }
        internal OdbcCommandBuilder moBldrODBC;

        internal OdbcTransaction moOdbcTrans;
        internal OleDbTransaction moOleTrans;
        internal SqlTransaction moTrans;

        /// <summary>
        /// the database brand being accessed, i.e. SQL Server, Oracle, Access, etc.
        /// </summary>
        public Brand Brand { get { return meBrand; } }   // set { meBrand = value; } }
        internal Brand meBrand;

        public DatabaseProvider Provider { get; set; }

        /// <summary>
        /// the type of connection to be used, i.e. OLEDB, ODBC, or DotNet (SQL)
        /// </summary>
        public ConnectivityType Connectivity { get { return meConnectivity; } }   // set { meBrand = value; } }
        internal ConnectivityType meConnectivity;

        //
        // constructor
        //
        public Connection()
        {
            InitializeMe();
        }

        //
        // destructor
        //
        ~Connection()
        {
            Clear();
        }

        // basic intialization upon creation of instance
        internal void InitializeMe()
        {
            msDataSource = "";
            msDataBase = "";
            msUserID = "";
            msPswd = "";
            msInfo = "";
            msTag = "";
            moBinding = null;

            meBrand = Brand.Unknown;
            meConnectivity = ConnectivityType.OleDB;

            Prep();
        }

        // prep internals for use
        internal void Prep()
        {
            Clear();
            moAdapterOLE = new OleDbDataAdapter();
            //oBindSrc = new BindingSource();
            moCmdOLE = new OleDbCommand();
            moBldrOLE = new OleDbCommandBuilder();
            moConnOLE = new OleDbConnection();

            moAdapterODBC = new OdbcDataAdapter();
            moCmdODBC = new OdbcCommand();
            moBldrODBC = new OdbcCommandBuilder();
            moConnODBC = new OdbcConnection();

            moAdapter = new SqlDataAdapter();
            moBldr = new SqlCommandBuilder();
            moCommand = new SqlCommand();
            moConn = new SqlConnection();

            moOdbcTrans = null;
            moOleTrans = null;
            moTrans = null;
        }

        // null internal objects in prep for destruction/re-use
        private void Clear()
        {
            if (moConnOLE != null) moConnOLE = null;
            if (moAdapterOLE != null) moAdapterOLE = null;
            if (moCmdOLE != null) moCmdOLE = null;
            if (moBldrOLE != null) moBldrOLE = null;

            if (moConnODBC != null) moConnODBC = null;
            if (moAdapterODBC != null) moAdapterODBC = null;
            if (moCmdODBC != null) moCmdODBC = null;
            if (moBldrODBC != null) moBldrODBC = null;

            if (moConn != null) moConn = null;
            if (moAdapter != null) moAdapter = null;
            if (moCommand != null) moCommand = null;
            if (moBldr != null) moBldr = null;

            moOdbcTrans = null;
            moOleTrans = null;
            moTrans = null;
        }
        #endregion
    }

    /// <summary>
    /// a database access object...you can access a variety of providers, using various types of
    /// connectivity such as ODBC or OLEDB...you can then interact with the instance to extract
    /// and manipulate data, letting this class handle many of the complexities of such
    /// 
    /// class can easily be appended to include support for any number of database brands, given
    /// open-ended design
    /// 
    /// Lee Tyler, April 2015
    /// 
    /// (many prior versions dating from the late 90's to today)
    /// 
    /// </summary>
    public partial class DB
    {
        // delegates
        public delegate void OnExecuteQueryAsyncCompleteArgs(DataTable poTable);    //, string psErrMsg, int piRowsAffected);
        public event OnExecuteQueryAsyncCompleteArgs OnExecuteQueryAsyncComplete;

        #region dbInterface
        string msErrMsg = "";
        string msLastSQL = "";
        int miLastRecsAffected = 0;
        bool mbLastSQLwasAction = false;

        /// <summary>
        /// internal data connection object, holding various properties of the connection
        /// </summary>
        public Connection Connection { get { return moDC; } }
        private Connection moDC;

        public List<DatabaseProvider> Providers { get { return moProviders; } }
        private List<DatabaseProvider> moProviders;




        /// <summary>
        /// for returned index(ices)
        /// </summary>
        public struct IndexInfo
        {
            public string Name;
            public bool IsPrimary;
            public bool IsUnique;
            public string[] KeyedColumns;
        }

        /// <summary>
        /// connection string properties
        /// </summary>
        public struct ConnStrVars
        {
            public Brand DbBrand;
            public string DataSource;
            public string Database;
            public string UserID;
            public string Password;
        }

        public struct SQLparam
        {
            public string ParamName;
            public object ParamValue;
        }

        public List<SQLparam> SQLparams { get { return mrSQLParams; } set { mrSQLParams = value; } }
        internal List<SQLparam> mrSQLParams;

        /// <summary>
        /// database/catalog name currently open/in use
        /// </summary>
        public string DatabaseName { get { return msDatabase; } set { msDatabase = value; } }
        internal string msDatabase;

        /// <summary>
        /// the last error message from this class...should be examined upon failure of any functionality in this class
        /// </summary>
        public string Message
        {
            get
            {
                return msErrMsg;
            }
        }

        /// <summary>
        /// the last SQL statement passed through
        /// </summary>
        public string LastSQL
        {
            get
            {
                return msLastSQL;
            }
        }

        /// <summary>
        /// last number of records affected by the prior SQL-Action call
        /// </summary>
        public long LastNumberOfRecordsActioned
        {
            get
            {
                return miLastRecsAffected;
            }
        }










        //
        // constructor
        //
        public DB()
        {
            moDC = new Connection();
            mrSQLParams = new List<SQLparam>(0);
            SetIntrinsicProviders();
        }



        public bool IntrinsicProvider(IntrinsicProviderType peInstrincicProviderType, out DatabaseProvider prProvider, out string psErrMsg)
        {
            DatabaseProvider xrProvider = new DatabaseProvider();
            string xsErrMsg = "Intrinsic provider type not found.";

            for (int xii = 0; xii < moProviders.Count; xii++)
            {
                if (moProviders[xii].IntrinsicProvider == peInstrincicProviderType)
                {
                    xsErrMsg = "";
                    xrProvider = moProviders[xii];
                    break;
                }
            }

            psErrMsg = xsErrMsg;
            prProvider = xrProvider;
            return (xsErrMsg.Length == 0);
        }

        private void SetIntrinsicProviders()
        {
            moProviders = new List<DatabaseProvider>(0);

            //
            // leave this guy at the top...not deletable
            //
            DatabaseProvider xoProvider = new DatabaseProvider();

            xoProvider.Name = "Microsoft Access (mdb)";
            xoProvider.PreferredConnectionType = ConnectivityType.OleDB;
            //xoProvider.OdbcConnStr = "Provider={Microsoft Access Driver (*.mdb)}";
            xoProvider.OdbcConnStr = "Driver={Microsoft Access Driver (*.mdb)};Dbq={DB};Uid={UID};Pwd={PSWD};";
            xoProvider.OdbcConnStrTrusted = "";
            xoProvider.OleDbConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={DB};User ID={UID};Password={PSWD}";
            xoProvider.OleDbConnStrTrusted = "";
            xoProvider.DotNetConnStr = "";
            xoProvider.DotNetConnStrTrusted = "";
            xoProvider.FilterExpression = "Access files (*.mdb)|*.mdb|All files (*.*)|*.*";
            xoProvider.IntrinsicProvider = IntrinsicProviderType.AccessMdbOleDB;
            //xoProvider.NonIntrinsicProviderID = "";
            xoProvider.ConnectionStringOverride = "";
            xoProvider.IsMsSQLServer = false;
            xoProvider.IsOracle = false;
            xoProvider.IsAccess = true;
            xoProvider.TopPhrase = "TOP 2500";
            moProviders.Add(xoProvider);

            xoProvider.Name = "Microsoft Access (accdb)";
            xoProvider.PreferredConnectionType = ConnectivityType.OleDB;
            xoProvider.OdbcConnStr = "Driver={Microsoft Access Driver (*.accdb)};Dbq={DB};Uid={UID};Pwd={PSWD};";
            xoProvider.OdbcConnStrTrusted = "";
            xoProvider.OleDbConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={DB};User ID={UID};Password={PSWD}";
            xoProvider.OleDbConnStrTrusted = "";
            xoProvider.DotNetConnStr = "";
            xoProvider.DotNetConnStrTrusted = "";
            xoProvider.FilterExpression = "Access files (*.accdb)|*.accd*|All files (*.*)|*.*";
            xoProvider.IntrinsicProvider = IntrinsicProviderType.AccessAccdbDB;
            //xoProvider.NonIntrinsicProviderID = "";
            xoProvider.ConnectionStringOverride = "";
            xoProvider.IsMsSQLServer = false;
            xoProvider.IsOracle = false;
            xoProvider.IsAccess = true;
            xoProvider.TopPhrase = "TOP 2500";
            moProviders.Add(xoProvider);

            xoProvider.Name = "Microsoft SQL Server";
            xoProvider.PreferredConnectionType = ConnectivityType.DotNet;
            xoProvider.OdbcConnStr = "Driver={SQL Server};Server={DS};Database={DB};Uid={UID};Pwd={PSWD}";
            xoProvider.OdbcConnStrTrusted = "Driver={SQL Server};Server={DS};Database={DB};Trusted_Connection=Yes";
            xoProvider.OleDbConnStr = "Provider=SQLOLEDB;Data Source={DS};Initial Catalog={DB};User ID={UID};Password={PSWD}";
            xoProvider.OleDbConnStrTrusted = "Provider=SQLOLEDB;Data Source={DS};Initial Catalog={DB};Integrated Security=SSPI";
            xoProvider.DotNetConnStr = "Server={DS};Database={DB};User Id={UID};Password={PSWD};";
            xoProvider.DotNetConnStrTrusted = "Server={DS};Database={DB};Trusted_Connection=True;";
            xoProvider.FilterExpression = "";
            xoProvider.IntrinsicProvider = IntrinsicProviderType.SqlServer;
            //xoProvider.NonIntrinsicProviderID = "";
            xoProvider.ConnectionStringOverride = "";
            xoProvider.IsMsSQLServer = true;
            xoProvider.IsOracle = false;
            xoProvider.IsAccess = false;
            xoProvider.TopPhrase = "TOP(2500)";
            moProviders.Add(xoProvider);

            xoProvider.Name = "Visual FoxPro";
            xoProvider.PreferredConnectionType = ConnectivityType.OleDB;
            xoProvider.OdbcConnStr = "";
            xoProvider.OdbcConnStrTrusted = "";
            xoProvider.OleDbConnStr = "Provider=vfpoledb;Data Source={DB};";
            xoProvider.OleDbConnStrTrusted = "";
            xoProvider.DotNetConnStr = "";
            xoProvider.DotNetConnStrTrusted = "";
            xoProvider.FilterExpression = "";
            xoProvider.IntrinsicProvider = IntrinsicProviderType.VFP;
            //xoProvider.NonIntrinsicProviderID = "";
            xoProvider.ConnectionStringOverride = "";
            xoProvider.IsMsSQLServer = false;
            xoProvider.IsOracle = false;
            xoProvider.IsAccess = false;
            xoProvider.TopPhrase = "";
            moProviders.Add(xoProvider);

            //Provider=vfpoledb;Data Source=C:\MyDbFolder\MyDbContainer.dbc;            Collating Sequence = machine;

            //                    xsProvider = "vfpoledb";
            //                    if (xsOverrideProvider.Length > 0) xsProvider = xsOverrideProvider;
            //                    xsConnParams = PlugVarsIntoGenericConnectionString("Data Source={DS};User ID={UID};Password={PSWD}", xrConnVars, xsProvider);
        }

        public bool AddProvider(DatabaseProvider poProvider, out string psErrMsg)
        {
            string xsErrMsg = "", xsProvName = poProvider.Name.Trim(), xsProvID = "";   // poProvider.NonIntrinsicProviderID.Trim();
            DatabaseProvider xoProvider = poProvider;

            xoProvider.IntrinsicProvider = IntrinsicProviderType.NotIntrinsic;

            if (xsProvName.Length == 0)
            {
                xsErrMsg = "Must specify a provider name.";
            }
            else
            {
                if (xsProvID.Length == 0)
                {
                    xsErrMsg = "Must specify a unique provider ID.";
                }
                else
                {
                    for (int xii = 0; xii < moProviders.Count; xii++)
                    {
                        //if (moProviders[xii].IntrinsicProvider == IntrinsicProviderType.NotIntrinsic)
                        //{
                        //    if (xsProvID == moProviders[xii].NonIntrinsicProviderID)
                        //    {
                        //        xsErrMsg = "The provider ID is already in use.";
                        //        break;
                        //    }
                        //}
                    }
                    if (xsErrMsg.Length == 0)
                    {
                        string xsName = xsProvName.Trim().ToUpper();
                        for (int xii = 0; xii < moProviders.Count; xii++)
                        {
                            string xs = moProviders[xii].Name.Trim().ToUpper();
                            if (xs == xsName)
                            {
                                xsErrMsg = "Provider name already in use.";
                                break;
                            }
                        }
                    }
                }
            }

            if (xsErrMsg.Length == 0) moProviders.Add(xoProvider);

            psErrMsg = xsErrMsg;
            return (xsErrMsg.Length == 0);
        }

        public void InitProvider(ref DatabaseProvider poProvider)
        {
            poProvider.ConnectionStringOverride = "";
            poProvider.DotNetConnStr = "";
            poProvider.DotNetConnStrTrusted = "";
            poProvider.FilterExpression = "";
            //poProvider.IntrinsicProvider = false;
            poProvider.IntrinsicProvider = IntrinsicProviderType.NotIntrinsic;
            poProvider.IsMsSQLServer = false;
            poProvider.IsOracle = false;
            poProvider.IsAccess = false;
            poProvider.TopPhrase = "";
            poProvider.Name = "";
            //poProvider.NonIntrinsicProviderID = "";
            poProvider.OdbcConnStr = "";
            poProvider.OdbcConnStrTrusted = "";
            poProvider.OleDbConnStrTrusted = "";
            poProvider.PreferredConnectionType = ConnectivityType.Unknown;
        }

        public bool GetExternalProviders(string psExternalProvidersXMLFile, out List<DatabaseProvider> poExtProvs, out string psErrMsg)
        {
            string xsErrMsg = "";
            bool xbIsProvidersFile = false, xbInProvider = false;
            DatabaseProvider xoProvider = new DatabaseProvider();
            poExtProvs = new List<DatabaseProvider>(0);

            if (psExternalProvidersXMLFile.Length > 0)
            {
                if (System.IO.File.Exists(psExternalProvidersXMLFile))
                {
                    StreamReader xoRdr = null;
                    try
                    {
                        xoRdr = new StreamReader(psExternalProvidersXMLFile);

                        while (!xoRdr.EndOfStream)
                        {
                            string xsLine = xoRdr.ReadLine();
                            xsLine = xsLine.Replace("\t", " ");
                            xsLine = xsLine.Trim();
                            if (!xbIsProvidersFile)
                            {
                                if (xsLine.ToUpper() == "<RebusDatabaseProviders>".ToUpper()) xbIsProvidersFile = true;
                            }
                            else
                            {
                                if (xsLine.ToUpper() == "</RebusDatabaseProviders>".ToUpper()) break;
                                if (!xbInProvider)
                                {
                                    if (xsLine.ToUpper() == "<Provider>".ToUpper())
                                    {
                                        xbInProvider = true;
                                        InitProvider(ref xoProvider);
                                    }
                                }
                                else
                                {
                                    if (xsLine.ToUpper() == "</Provider>".ToUpper())
                                    {
                                        //
                                        // validate and add new provider here
                                        // 
                                        xbInProvider = false;
                                        bool xbValid = true;

                                        // need validation here...basically NAME, and at least one connections string
                                        string xsName = xoProvider.Name.Trim().ToUpper();
                                        if (xsName.Length > 0)
                                        {
                                            for (int xii = 0; xii < moProviders.Count; xii++)
                                            {
                                                if (xsName == moProviders[xii].Name.Trim().ToUpper())
                                                {
                                                    xbValid = false;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            xbValid = false;
                                        }
                                        if (xbValid)
                                        {

                                        }


                                        if (xbValid)
                                        {
                                            // actually,
                                            // let's modify this to return a set of
                                            // external providers, so it only runs once (or when needed)
                                            // per application run...the app can do this once, store...
                                            // ...and each time it instantiates an instance, use
                                            // AddProvider ...it will be faster than loading the file
                                            // externally each time

                                            //
                                            // modify method to return the validated external provider(s)
                                            //
                                            poExtProvs.Add(xoProvider);
                                        }
                                    }
                                    else
                                    {

                                        int xiLen = xsLine.Length;
                                        if (xiLen >= 6)
                                        {
                                            if (xsLine.Substring(0, 1) == "<")
                                            {
                                                if (xsLine.Substring(xiLen - 2, 2) == "/>")
                                                {
                                                    if (xsLine.IndexOf("=") > 0)
                                                    {


                                                        //
                                                        // let's parse the line, looking for a valid property
                                                        //
                                                        string xsProp = "";
                                                        string xsVal = "";
                                                        bool xbInVal = false;
                                                        bool xbInQt = false;
                                                        char xcQtChr = '\0';




                                                        for (int xii = 0; xii < xiLen; xii++)
                                                        {
                                                            string xs = xsLine.Substring(xii, 1);
                                                            if (!xbInVal)
                                                            {
                                                                if (xs == "=")
                                                                {
                                                                    xbInVal = true;
                                                                }
                                                                else
                                                                {
                                                                    if (xs != "<")
                                                                    {
                                                                        xsProp += xs;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (xbInQt)
                                                                {
                                                                    if (xs == xcQtChr.ToString())
                                                                    {
                                                                        xbInQt = false;
                                                                        break;
                                                                    }
                                                                    else
                                                                    {
                                                                        xsVal += xs;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (xs == "'" || xs == "\"")
                                                                    {
                                                                        if (xsVal.Trim().Length == 0)
                                                                        {
                                                                            xbInQt = true;
                                                                            xcQtChr = Convert.ToChar(xs);
                                                                        }
                                                                        else
                                                                        {
                                                                            //xsVal += xs;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        xsVal += xs;
                                                                    }
                                                                }
                                                            }
                                                        }       // for

                                                        xsProp = xsProp.Trim().ToUpper();
                                                        switch (xsProp)
                                                        {
                                                            case "NAME":
                                                                xoProvider.Name = xsVal.Trim();
                                                                break;
                                                            case "ODBCCONNSTR":
                                                                xoProvider.OdbcConnStr = xsVal.Trim();
                                                                break;
                                                            case "ODBCCONNSTRTRUSTED":
                                                                xoProvider.OdbcConnStrTrusted = xsVal.Trim();
                                                                break;
                                                            case "OLEDBCONNSTR":
                                                                xoProvider.OleDbConnStr = xsVal.Trim();
                                                                break;
                                                            case "OLEDBCONNSTRTRUSTED":
                                                                xoProvider.OleDbConnStrTrusted = xsVal.Trim();
                                                                break;
                                                            case "DOTNETCONNSTR":
                                                                xoProvider.DotNetConnStr = xsVal.Trim();
                                                                break;
                                                            case "DOTNETCONNSTRTRUSTED":
                                                                xoProvider.DotNetConnStrTrusted = xsVal.Trim();
                                                                break;
                                                            case "FILEEXT":
                                                                xoProvider.FilterExpression = xsVal.Trim();
                                                                break;
                                                            case "TOPPHRASE":
                                                                xoProvider.TopPhrase = xsVal.Trim();
                                                                break;
                                                            case "ISACCESS":
                                                                xoProvider.IsAccess = true;
                                                                break;
                                                            case "ISSQLSERVER":
                                                                xoProvider.IsMsSQLServer = true;
                                                                break;
                                                            case "ISORACLE":
                                                                xoProvider.IsOracle = true;
                                                                break;
                                                            default:
                                                                break;
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
                    catch (Exception xoExc)
                    {
                        xsErrMsg = xoExc.Message;
                    }
                    if (xoRdr != null)
                    {
                        try
                        {
                            xoRdr.Close();
                        }
                        catch { };
                        xoRdr.Dispose();
                    }
                }
                else
                {
                    xsErrMsg = "File not found.";
                }
            }

            if (xsErrMsg.Length > 0) poExtProvs = new List<DatabaseProvider>(0);

            psErrMsg = xsErrMsg;
            return (xsErrMsg.Length == 0);
        }

        public bool DropProvider(string psProviderName, out string psErrMsg)
        {
            string xsErrMsg = "", xsName = psProviderName.Trim().ToUpper();

            if (moProviders.Count <= 1)
            {
                xsErrMsg = "The default MDB provider cannot be removed.";
            }
            else
            {
                for (int xii = 0; xii < moProviders.Count; xii++)
                {
                    string xs = moProviders[xii].Name.Trim().ToUpper();
                    if (xs == xsName)
                    {
                        moProviders.RemoveAt(xii);
                        break;
                    }
                }
            }

            psErrMsg = xsErrMsg;
            return (xsErrMsg.Length == 0);
        }

        public bool CreateMicrosoftDatabase(string psDbFileName)
        {
            string xsErrMsg = "", xsConn = "";

            xsConn = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + psDbFileName + "; Jet OLEDB:Engine Type=5";

            ADOX.Catalog xoCatalog = new ADOX.Catalog();

            try
            {
                xoCatalog.Create(xsConn);
            }
            catch (Exception ex)
            {
                xsErrMsg = ex.Message;
            }

            xoCatalog = null;
            msErrMsg = xsErrMsg;
            return (xsErrMsg.Length == 0);
        }
        #endregion

        #region OpenDatabase Interfaces
        /// <summary>
        /// Pass an existing database filename...the database is assumed to be MS-ACCESS with no security.
        /// </summary>
        /// <param name="psFileName">database file to access</param>
        /// <returns>true on success, otherwise, examine Message as to why failed</returns>
        //public bool OpenDatabase(string psFileName)
        //{
        //    return (InternalOpenDatabase(psFileName, Brand.MsAccess, "", ""));
        //}

        /// <summary>
        /// pass an existing database file...the database is assumed to be MS-ACCESS with security
        /// </summary>
        /// <param name="psFileName">database file to access</param>
        /// <param name="psUserID">userID, if needed for access</param>
        /// <param name="psPswd">password, if needed for access</param>
        /// <returns>true on success, otherwise, examine Message as to why failed</returns>
        //public bool OpenDatabase(string psFileName, string psUserID, string psPswd)
        //{
        //    return (InternalOpenDatabase(psFileName, Brand.MsAccess, psUserID, psPswd));
        //}

        /// <summary>
        /// pass an existing database file name, along with the type of database it is, and the userID and password for a secure connection
        /// </summary>
        /// <param name="psFileName">file name to access</param>
        /// <param name="peBrand">database brand thereof</param>
        /// <param name="psUserID">userID, if needed for access</param>
        /// <param name="psPswd">password, if needed for access</param>
        /// <returns>true on success, otherwise, examine Message as to why failed</returns>
        //public bool OpenDatabase(string psFileName, Brand peBrand, string psUserID, string psPswd)
        //{
        //    return (InternalOpenDatabase(psFileName, peBrand, psUserID, psPswd));
        //}

        /// <summary>
        /// pass along database file name to open, along with brand, connection type, user ID and password
        /// </summary>
        /// <param name="psFileName">file name to access</param>
        /// <param name="peBrand">database brand thereof</param>
        /// <param name="peConnectivity">connection type, i.e. OLEDB, ODBC...</param>
        /// <param name="psUserID">userID, if needed for access</param>
        /// <param name="psPswd">password, if needed for access</param>
        /// <returns>true on success, otherwise, examine Message as to why failed</returns>
        //public bool OpenDatabase(string psFileName, Brand peBrand, ConnectivityType peConnectivity, string psUserID, string psPswd)
        //{
        //    return (InternalOpenDatabase(psFileName, peBrand, psUserID, psPswd, peConnectivity));
        //}

        /// <summary>
        /// pass the database source, the database name, the database supplier/provider, and the authorized user ID and password
        /// </summary>
        /// <param name="psDataSource">data source for SQL Server/Oracle/etc</param>
        /// <param name="psDatabaseName">database/catalog name to connect to</param>
        /// <param name="peBrand">database brand thereof</param>
        /// <param name="psUserID">userID, if needed for access</param>
        /// <param name="psPswd">password, if needed for access</param>
        /// <returns>true on success, otherwise, examine Message as to why failed</returns>
        //public bool OpenDatabase(string psDataSource, string psDatabaseName, Brand peBrand, string psUserID, string psPswd)
        //{
        //    string xsDS = psDataSource.Trim(), xsDB = psDatabaseName.Trim();
        //    if (xsDS.Length > 0 && xsDB.Length > 0)
        //    {
        //        xsDS = xsDS + "::" + xsDB;
        //    }
        //    else
        //    {
        //        if (xsDB.Length > 0) xsDS = xsDB;
        //    }
        //    return (InternalOpenDatabase(xsDS, peBrand, psUserID, psPswd));
        //}

        /// <summary>
        /// pass the database source, the database name, the database supplier/provider, and the authorized user ID and password, and optionally a full connection string in cases where a constructed string needs to be overridden
        /// </summary>
        /// <param name="psDataSource">data source for SQL Server/Oracle/etc</param>
        /// <param name="psDatabaseName">database/catalog name to connect to</param>
        /// <param name="peBrand">database brand thereof</param>
        /// <param name="peConnectivity">connection type, i.e. OLEDB, ODBC...</param>
        /// <param name="psUserID">userID, if needed for access</param>
        /// <param name="psPswd"><password, if needed for access/param>
        /// <param name="psConnectionString">pass your own fully constructed string, and it will be used, instead of the engine generating one for you</param>
        /// <returns>true on success, otherwise, examine Message as to why failed</returns>
        //public bool OpenDatabase(string psDataSource, string psDatabaseName, Brand peBrand, ConnectivityType peConnectivity, string psUserID, string psPswd, string psConnectionString = "", string psOverrideProvider = "")
        //{
        //    string xsDS = psDataSource.Trim(), xsDB = psDatabaseName.Trim();
        //    if (xsDS.Length > 0 && xsDB.Length > 0)
        //    {
        //        xsDS = xsDS + "::" + xsDB;
        //    }
        //    else
        //    {
        //        if (xsDB.Length > 0) xsDS = xsDB;
        //    }
        //    return (InternalOpenDatabase(xsDS, peBrand, psUserID, psPswd, peConnectivity, psConnectionString, psOverrideProvider));
        //}
        #endregion

        public bool OpenDatabase(DatabaseToOpen prDbToOpen)
        {
            return (InternalOpenDatabase(prDbToOpen));
        }

        /// <summary>
        /// open a database (duh)
        /// </summary>
        /// <param name="psFileNameOrDataSource"></param>
        /// <param name="piBrand"></param>
        /// <param name="psUserID"></param>
        /// <param name="psPswd"></param>
        /// <returns></returns>
        private bool InternalOpenDatabase(DatabaseToOpen prDbToOpen)
        {
            string xsErrMsg = "", xsConnectionString = "";
            ConnStrVars xrConnVars = new ConnStrVars();

            CloseDatabase();

            moDC = new Connection();                    // this initializes defaults
            //moDC.meBrand = peBrand;                     // but from here, we override with incoming requests
            moDC.meConnectivity = prDbToOpen.ConnectionType;
            if (prDbToOpen.UserID != null) moDC.msUserID = prDbToOpen.UserID.Trim(); else moDC.msUserID = "";
            if (prDbToOpen.Password != null) moDC.msPswd = prDbToOpen.Password.Trim(); else moDC.msPswd = "";
            if (prDbToOpen.DataSource != null) moDC.msDataSource = prDbToOpen.DataSource.Trim(); else moDC.msDataSource = "";
            if (prDbToOpen.Database != null) moDC.msDataBase = prDbToOpen.Database.Trim(); else moDC.msDataBase = "";
            //msDatabase = psFileNameOrDataSource;

            //xrConnVars.DbBrand = peBrand;
            xrConnVars.Password = moDC.msPswd;
            xrConnVars.UserID = moDC.UserID;
            xrConnVars.DataSource = moDC.msDataSource;
            xrConnVars.Database = moDC.msDataBase;
            //xrConnVars.SQLDatabase = "";
            //if (moDC.Provider == Brand.SqlSrvr && psConnectionString.Trim().Length == 0)
            //{
            //    xi = moDC.FileNameOrDataSource.IndexOf("::");
            //    if (xi >= 0)
            //    {
            //        xrConnVars.SQLDatabase = moDC.FileNameOrDataSource.Substring(xi + 2).Trim();
            //        msDatabase = xrConnVars.SQLDatabase;
            //        xrConnVars.DbFileOrDataSource = moDC.FileNameOrDataSource.Substring(0, xi + 0).Trim();
            //        if (xrConnVars.DbFileOrDataSource.Length == 0)
            //        {
            //            xsErrMsg = "No SQL Server data source specified.";
            //        }
            //    }
            //    else
            //    {
            //        xrConnVars.SQLDatabase = "";
            //        xsErrMsg = "No SQL Server data source specified.";
            //    }
            //}

            if (xsErrMsg.Length == 0)
            {
                try
                {
                    switch (moDC.meConnectivity)
                    {
                        case ConnectivityType.ODBC:
                            if (moDC.OdbcConnection != null)
                            {
                                moDC.OdbcConnection.Close();
                                moDC.OdbcConnection.Dispose();
                            }
                            break;
                        case ConnectivityType.OleDB:
                            if (moDC.OleConnection != null)
                            {
                                moDC.OleConnection.Close();
                                moDC.OleConnection.Dispose();
                            }
                            break;
                        default:
                            if (moDC.Conn != null)
                            {
                                moDC.Conn.Close();
                                moDC.Conn.Dispose();
                            }
                            break;
                    }
                }
                catch (Exception xo)
                {
                    xsErrMsg = xo.Message;
                }
            }

            if (xsErrMsg.Length == 0)
            {
                switch (moDC.meConnectivity)
                {
                    case ConnectivityType.ODBC:
                        moDC.moConnODBC = new OdbcConnection();
                        break;
                    case ConnectivityType.OleDB:
                        moDC.moConnOLE = new OleDbConnection();
                        break;
                    default:
                        moDC.moConn = new SqlConnection();
                        break;
                }

                xsConnectionString = "";
                if (prDbToOpen.Provider.ConnectionStringOverride != null) xsConnectionString = prDbToOpen.Provider.ConnectionStringOverride.Trim();       // user caller fully built string, if supplied
                
                if (xsConnectionString.Length == 0)                   // otherwise, we'll build it for them
                {
                    xsConnectionString = PlugVariablesIntoConnectionString(prDbToOpen);
                }

                //    switch (peBrand)
                //    {
                //        case Brand.MsAccess:
                //            switch (moDC.meConnectivity)
                //            {
                //                case ConnectivityType.ODBC:
                //                    xsProvider = "{Microsoft Access Driver (*.mdb)}";
                //                    if (moDC.msFileNameOrDataSource.ToUpper().IndexOf(".ACCD") > 0) xsProvider = "{Microsoft Access Driver (*.accdb)}";
                //                    //-trolling- xsProvider = "{MS Access Database}";
                //                    if (xsOverrideProvider.Length > 0) xsProvider = xsOverrideProvider;
                //                    xs = "Dbq={DS};Uid=";
                //                    if (moDC.msUserID.Trim().Length > 0) xs += "{UID}";
                //                    xs += ";Pwd=";
                //                    if (moDC.msPswd.Trim().Length > 0) xs += "{PSWD}";
                //                    xs = xs + ";";
                //                    xsConnParams = PlugVarsIntoGenericConnectionString(xs, xrConnVars, xsProvider);
                //                    break;
                //                case ConnectivityType.DotNet:
                //                    xsProvider = "";
                //                    xsConnParams = PlugVarsIntoGenericConnectionString("Database={DS};User Id={UID};Password={PSWD};", xrConnVars, xsProvider);
                //                    break;
                //                default:
                //                    xsProvider = "Microsoft.Jet.OLEDB.4.0";
                //                    xs = moDC.msFileNameOrDataSource.ToUpper();
                //                    if (xs.IndexOf(".ACCD") >= 0) xsProvider = "Microsoft.ACE.OLEDB.12.0";
                //                    if (xsOverrideProvider.Length > 0) xsProvider = xsOverrideProvider;
                //                    xs = moDC.msPswd.Trim();
                //                    xs2 = moDC.msUserID.Trim();
                //                    xs = ((xs.Length > 0) & (xs2.Length == 0)) ? "Data Source={DS};User ID={UID};Jet OLEDB:Database Password={PSWD}" : "Data Source={DS};User ID={UID};Password={PSWD}";
                //                    xsConnParams = PlugVarsIntoGenericConnectionString(xs, xrConnVars, xsProvider);
                //                    break;
                //            }
                //            break;
                //        case Brand.VFP:
                //            switch (moDC.meConnectivity)
                //            {
                //                case ConnectivityType.ODBC:
                //                    xsErrMsg = "ODBC not yet supported for VFP...use OleDB instead.";
                //                    break;
                //                case ConnectivityType.DotNet:
                //                    xsErrMsg = "DotNet not yet supported for VFP...use OleDB instead.";
                //                    break;
                //                default:
                //                    xsProvider = "vfpoledb";
                //                    if (xsOverrideProvider.Length > 0) xsProvider = xsOverrideProvider;
                //                    xsConnParams = PlugVarsIntoGenericConnectionString("Data Source={DS};User ID={UID};Password={PSWD}", xrConnVars, xsProvider);
                //                    break;
                //            }
                //            break;
                //        case Brand.SqlSrvr:
                //            switch (moDC.meConnectivity)
                //            {
                //                case ConnectivityType.ODBC:
                //                    xsProvider = "{SQL Server}";
                //                    if (xsOverrideProvider.Length > 0) xsProvider = xsOverrideProvider;
                //                    xsConnParams = PlugVarsIntoGenericConnectionString("Server={DS};Database={DB};Uid={UID};Pwd={PSWD}", xrConnVars, xsProvider);
                //                    break;
                //                case ConnectivityType.DotNet:
                //                    xsProvider = "";
                //                    xsConnParams = PlugVarsIntoGenericConnectionString("Server={DS};Database={DB};User Id={UID};Password={PSWD};", xrConnVars, xsProvider);
                //                    break;
                //                default:
                //                    xsProvider = "SQLOLEDB";
                //                    if (xsOverrideProvider.Length > 0) xsProvider = xsOverrideProvider;
                //                    xsConnParams = PlugVarsIntoGenericConnectionString("Data Source={DS};Initial Catalog={DB};User ID={UID};Password={PSWD}", xrConnVars, xsProvider);
                //                    break;
                //            }
                //            break;

                //        case Brand.Oracle:
                //            switch (moDC.meConnectivity)
                //            {
                //                case ConnectivityType.ODBC:
                //                    xsProvider = "{Microsoft ODBC for Oracle}";     // 32-bit only
                //                    if (xsOverrideProvider.Length > 0) xsProvider = xsOverrideProvider;
                //                    xsConnParams = PlugVarsIntoGenericConnectionString("Server={DS};Uid={UID};Pwd={PSWD};", xrConnVars, xsProvider);
                //                    xsProvider = "";
                //                    xsConnParams = PlugVarsIntoGenericConnectionString("Data Source={DS};User Id={UID};Password={PSWD};Integrated Security=no;", xrConnVars, xsProvider);
                //                    break;
                //                case ConnectivityType.DotNet:
                //                    xsErrMsg = "DotNet not yet supported for Oracle at this time...use OleDB instead.";
                //                    break;
                //                case ConnectivityType.OleDB:
                //                    xsProvider = "OraOLEDB.Oracle";
                //                    if (xsOverrideProvider.Length > 0) xsProvider = xsOverrideProvider;
                //                    xsConnParams = PlugVarsIntoGenericConnectionString("Data Source={DS};User ID={UID};Password={PSWD}", xrConnVars, xsProvider);
                //                    break;
                //            }
                //            break;


                moDC.msInfo = xsConnectionString;         // so caller can see what was built

                if (xsErrMsg.Length == 0)
                {
                    try
                    {
                        switch (moDC.meConnectivity)
                        {
                            case ConnectivityType.ODBC:
                                moDC.moConnODBC.ConnectionString = xsConnectionString;
                                moDC.moConnODBC.Open();
                                break;
                            case ConnectivityType.DotNet:
                                moDC.moConn.ConnectionString = xsConnectionString;
                                moDC.moConn.Open();
                                break;
                            default:
                                moDC.moConnOLE.ConnectionString = xsConnectionString;
                                moDC.moConnOLE.Open();
                                break;
                        }
                    }
                    catch (Exception xo)
                    {
                        xsErrMsg = xo.Message;
                    }
                }
            }

            // wrapup
            msErrMsg = xsErrMsg;
            if (xsErrMsg.Length > 0) moDC.moConnOLE = null;
            return (xsErrMsg.Length == 0);
        }

        #region Transactions
        /// <summary>
        /// start a transaction
        /// </summary>
        /// <returns>true if successful, otherwise false - review Message for why</returns>
        public bool BeginTrans()
        {
            string xsErrMsg = "";

            try
            {
                switch (moDC.meConnectivity)
                {
                    case ConnectivityType.OleDB:
                        //moDC.moOleTrans = new OleDbTransaction();
                        moDC.moOleTrans = moDC.moConnOLE.BeginTransaction();
                        break;
                    case ConnectivityType.ODBC:
                        //moDC.moOdbcTrans = new OdbcTransaction();
                        moDC.moOdbcTrans = moDC.moConnODBC.BeginTransaction();
                        break;
                    default:
                        //moDC.moTrans = new SqlTransaction();
                        moDC.moTrans = moDC.moConn.BeginTransaction();
                        break;
                }
            }
            catch (Exception xoExc)
            {
                xsErrMsg = xoExc.Message;
            }

            msErrMsg = xsErrMsg;
            return (xsErrMsg.Length > 0);
        }

        /// <summary>
        /// write all to database since begin transaction
        /// </summary>
        /// <returns>true if successful, otherwise false - review Message for why</returns>
        public bool CommitTrans()
        {
            string xsErrMsg = "";

            try
            {
                switch (moDC.meConnectivity)
                {
                    case ConnectivityType.OleDB:
                        moDC.moOleTrans.Commit();
                        moDC.moOleTrans.Dispose();
                        moDC.moOleTrans = null;
                        break;
                    case ConnectivityType.ODBC:
                        moDC.moOdbcTrans.Commit();
                        moDC.moOdbcTrans.Dispose();
                        moDC.moOdbcTrans = null;
                        break;
                    default:
                        moDC.moTrans.Commit();
                        moDC.moTrans.Dispose();
                        moDC.moTrans = null;
                        break;
                }
            }
            catch (Exception xoExc)
            {
                xsErrMsg = xoExc.Message;
            }

            msErrMsg = xsErrMsg;
            return (xsErrMsg.Length > 0);
        }

        /// <summary>
        /// whoops...wipe out any inserts/updates/deletes/etc since last BeginTrans
        /// </summary>
        /// <returns>true if successful, otherwise false - review Message for why</returns>
        public bool RollbackTrans()
        {
            string xsErrMsg = "";

            try
            {
                switch (moDC.meConnectivity)
                {
                    case ConnectivityType.OleDB:
                        moDC.moOleTrans.Rollback();
                        moDC.moOleTrans.Dispose();
                        moDC.moOleTrans = null;
                        break;
                    case ConnectivityType.ODBC:
                        moDC.moOdbcTrans.Rollback();
                        moDC.moOdbcTrans.Dispose();
                        moDC.moOdbcTrans = null;
                        break;
                    default:
                        moDC.moTrans.Dispose();
                        moDC.moTrans = null;
                        moDC.moTrans.Rollback();
                        break;
                }
            }
            catch (Exception xoExc)
            {
                xsErrMsg = xoExc.Message;
            }

            msErrMsg = xsErrMsg;
            return (xsErrMsg.Length > 0);
        }
        #endregion

        #region switching
        /// <summary>
        /// return true or false on whether or not the database is currently open
        /// </summary>
        /// <returns></returns>
        public bool DatabaseIsOpen()
        {
            bool xbIsOpen = false;

            switch (moDC.meConnectivity)
            {
                case ConnectivityType.OleDB:
                    if ((moDC.moConnOLE != null) & (moDC.msInfo.Length > 0))
                    {
                        try
                        {
                            if (moDC.moConnOLE.State != ConnectionState.Broken) xbIsOpen = true;
                        }
                        catch { }
                    }
                    break;
                case ConnectivityType.ODBC:
                    if ((moDC.moConnODBC != null) & (moDC.msInfo.Length > 0))
                    {
                        try
                        {
                            if (moDC.moConnODBC.State != ConnectionState.Broken) xbIsOpen = true;
                        }
                        catch { }
                    }
                    break;
                default:
                    if ((moDC.moConn != null) & (moDC.msInfo.Length > 0))
                    {
                        try
                        {
                            if (moDC.moConn.State != ConnectionState.Broken) xbIsOpen = true;
                        }
                        catch { }
                    }
                    break;
            }

            return (xbIsOpen);
        }

        /// <summary>
        /// close the database (duh)
        /// </summary>
        /// <returns></returns>
        public bool CloseDatabase()
        {
            string xsErrMsg = "";

            if (DatabaseIsOpen())
            {
                switch (moDC.meConnectivity)
                {
                    case ConnectivityType.OleDB:
                        if (moDC.moConnOLE != null)
                        {
                            try
                            {
                                moDC.moConnOLE.Close();
                            }
                            catch (Exception xo)
                            {
                                xsErrMsg = xo.Message;
                            }
                        }
                        break;
                    case ConnectivityType.ODBC:
                        if (moDC.moConnODBC != null)
                        {
                            try
                            {
                                moDC.moConnODBC.Close();
                            }
                            catch (Exception xo)
                            {
                                xsErrMsg = xo.Message;
                            }
                        }
                        break;
                    default:
                        if (moDC.moConn != null)
                        {
                            try
                            {
                                moDC.moConn.Close();
                            }
                            catch (Exception xo)
                            {
                                xsErrMsg = xo.Message;
                            }
                        }
                        break;
                }
            }

            moDC.Prep();        // for potential new connection

            msErrMsg = xsErrMsg;
            return (xsErrMsg.Length == 0);
        }

        /// <summary>
        /// return true or false on whether the connection to the open database is open or not
        /// </summary>
        /// <returns></returns>
        public bool IsConnectionClosed()
        {
            bool xbClosed = true;
            ConnectionState xeState = ConnectionState.Broken;

            if (DatabaseIsOpen())
            {
                switch (moDC.meConnectivity)
                {
                    case ConnectivityType.OleDB:
                        xeState = moDC.moConnOLE.State;
                        break;
                    case ConnectivityType.ODBC:
                        xeState = moDC.moConnODBC.State;
                        break;
                    default:
                        xeState = moDC.moConn.State;
                        break;
                }
            }
            if (xeState != ConnectionState.Broken && xeState != ConnectionState.Closed) xbClosed = false;

            return (xbClosed);
        }

        /// <summary>
        /// open the connection to the currently open database
        /// </summary>
        /// <returns></returns>
        public bool OpenTheConnection()
        {
            string xsErrMsg = "(OpenTheConnection) Database is not open.";

            if (moDC != null)
            {
                switch (moDC.meConnectivity)
                {
                    case ConnectivityType.OleDB:
                        if (moDC.moConnOLE != null)
                        {
                            if (moDC.moConnOLE.State != ConnectionState.Open)
                            {
                                try
                                {
                                    moDC.moConnOLE.Open();
                                    xsErrMsg = "";
                                }
                                catch (Exception xoExc)
                                {
                                    xsErrMsg = xoExc.Message;
                                }
                            }
                            else
                            {
                                xsErrMsg = "";
                            }
                        }
                        break;
                    case ConnectivityType.ODBC:
                        if (moDC.moConnODBC != null)
                        {
                            if (moDC.moConnODBC.State != ConnectionState.Open)
                            {
                                try
                                {
                                    moDC.moConnODBC.Open();
                                    xsErrMsg = "";
                                }
                                catch (Exception xoExc)
                                {
                                    xsErrMsg = xoExc.Message;
                                }
                            }
                            else
                            {
                                xsErrMsg = "";
                            }
                        }
                        break;
                    default:
                        if (moDC.moConn != null)
                        {
                            if (moDC.moConn.State != ConnectionState.Open)
                            {
                                try
                                {
                                    moDC.moConn.Open();
                                    xsErrMsg = "";
                                }
                                catch (Exception xoExc)
                                {
                                    xsErrMsg = xoExc.Message;
                                }
                            }
                            else
                            {
                                xsErrMsg = "";
                            }
                        }
                        break;
                }
            }

            msErrMsg = xsErrMsg;
            return (xsErrMsg.Length == 0);
        }

        /// <summary>
        /// close the connection to the currently open database
        /// </summary>
        /// <returns></returns>
        public bool CloseTheConnection()
        {
            string xsErrMsg = "(CloseTheConnection) Database is not open.";
            bool xbCloseFail = false;
            msErrMsg = "";

            if (moDC != null)
            {
                switch (moDC.meConnectivity)
                {
                    case ConnectivityType.OleDB:
                        if (moDC.moConnOLE != null)
                        {
                            if (moDC.moConnOLE.State == ConnectionState.Open)
                            {
                                try
                                {
                                    moDC.moConnOLE.Close();
                                    xsErrMsg = "";
                                }
                                catch (Exception xoExc)
                                {
                                    xsErrMsg = xoExc.Message;
                                    xbCloseFail = true;
                                }
                            }
                            else
                            {
                                xsErrMsg = "";
                            }
                        }
                        break;
                    case ConnectivityType.ODBC:
                        if (moDC.moConnODBC != null)
                        {
                            if (moDC.moConnODBC.State == ConnectionState.Open)
                            {
                                try
                                {
                                    moDC.moConnODBC.Close();
                                    xbCloseFail = true;
                                    xsErrMsg = "";
                                }
                                catch (Exception xoExc)
                                {
                                    xsErrMsg = xoExc.Message;
                                }
                            }
                            else
                            {
                                xsErrMsg = "";
                            }
                        }
                        break;
                    default:
                        if (moDC.moConn != null)
                        {
                            if (moDC.moConn.State == ConnectionState.Open)
                            {
                                try
                                {
                                    moDC.moConn.Close();
                                    xsErrMsg = "";
                                }
                                catch (Exception xoExc)
                                {
                                    xsErrMsg = xoExc.Message;
                                    xbCloseFail = true;
                                }
                            }
                            else
                            {
                                xsErrMsg = "";
                            }
                        }
                        break;
                }
            }

            msErrMsg = xsErrMsg;
            if (xsErrMsg.Length > 0)
            {
                if (!xbCloseFail) xsErrMsg = "";            // return true, because while the connection IS closed, it is because the database is not "open" in the instance
            }
            return (xsErrMsg.Length == 0);

        }
        #endregion

        #region SQL Interfaces
        /// <summary>
        /// pass action-based SQL statement
        /// </summary>
        /// <param name="xsSQL">the SQL</param>
        /// <returns>true on success, otherwise, examine Message for why failed</returns>
        public bool SQL(string xsSQL)
        {
            OleDbDataReader xoRdrOLE = null;
            OdbcDataReader xoRdrODBC = null;
            SqlDataReader xoRdr = null;
            bool xb = InternalSQL(xsSQL, ref xoRdrOLE, ref xoRdrODBC, ref xoRdr);
            DropOleReader(ref xoRdrOLE);
            DropOdbcReader(ref xoRdrODBC);
            DropSqlReader(ref xoRdr);
            return (xb);
        }

        private void DropOleReader(ref OleDbDataReader poRdr)
        {
            if (poRdr != null)
            {
                try
                {
                    poRdr.Close();
                }
                catch
                {
                }
                finally
                {
                    poRdr.Dispose();
                    poRdr = null;
                }
            }
        }

        private void DropOdbcReader(ref OdbcDataReader poRdr)
        {
            if (poRdr != null)
            {
                try
                {
                    poRdr.Close();
                }
                catch
                {
                }
                finally
                {
                    poRdr.Dispose();
                    poRdr = null;
                }
            }
        }

        private void DropSqlReader(ref SqlDataReader poRdr)
        {
            if (poRdr != null)
            {
                try
                {
                    poRdr.Close();
                }
                catch
                {
                }
                finally
                {
                    poRdr.Dispose();
                    poRdr = null;
                }
            }
        }

        /// <summary>
        /// pass SQL statement to database, returning a result set in a data table
        /// </summary>
        /// <param name="xsSQL">the SQL</param>
        /// <param name="poTbl">the resulting data table</param>
        /// <returns>true on success, otherwise, examine Message for why failed</returns>
        public bool SQL(string xsSQL, DataTable poTbl, bool pbAsynchronously = false)
        {
            OleDbDataReader xoRdrOLE = null;
            OdbcDataReader xoRdrODBC = null;
            SqlDataReader xoRdr = null;
            bool xb = InternalSQL(xsSQL, ref xoRdrOLE, ref xoRdrODBC, ref xoRdr, poTbl, pbAsynchronously);
            DropOleReader(ref xoRdrOLE);
            DropOdbcReader(ref xoRdrODBC);
            DropSqlReader(ref xoRdr);
            return (xb);
        }

        /// <summary>
        /// pass SQL statement to database, returning a forward-only data reader for perusing the data
        /// </summary>
        /// <param name="xsSQL">the SQL</param>
        /// <param name="poRdr">the data reader prepped and ready</param>
        /// <returns>true on success, otherwise, examine Message for why failed</returns>
        public bool SQL(string xsSQL, ref OleDbDataReader poRdr)
        {
            //OleDbDataReader xoRdrOLE = null;
            OdbcDataReader xoRdrODBC = null;
            SqlDataReader xoRdr = null;
            bool xb = InternalSQL(xsSQL, ref poRdr, ref xoRdrODBC, ref xoRdr);
            DropOdbcReader(ref xoRdrODBC);
            DropSqlReader(ref xoRdr);
            return (xb);
        }

        /// <summary>
        /// pass SQL statement to database, returning a forward-only data reader for perusing the data
        /// </summary>
        /// <param name="xsSQL">the SQL</param>
        /// <param name="poRdr">the data reader prepped and ready</param>
        /// <returns>true on success, otherwise, examine Message for why failed</returns>
        public bool SQL(string xsSQL, OdbcDataReader poRdr)
        {
            OleDbDataReader xoRdrOLE = null;
            //OdbcDataReader xoRdrODBC = null;
            SqlDataReader xoRdr = null;
            bool xb = InternalSQL(xsSQL, ref xoRdrOLE, ref poRdr, ref xoRdr);
            DropOleReader(ref xoRdrOLE);
            DropSqlReader(ref xoRdr);
            return (xb);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xsSQL">the SQL</param>
        /// <param name="poRdr">the data reader prepped and ready</param>
        /// <returns>true on success, otherwise, examine Message for why failed</returns>
        public bool SQL(string xsSQL, ref SqlDataReader poRdr)
        {
            OleDbDataReader xoRdrOLE = null;
            OdbcDataReader xoRdrODBC = null;
            //SqlDataReader xoRdr = null;
            bool xb = InternalSQL(xsSQL, ref xoRdrOLE, ref xoRdrODBC, ref poRdr);
            DropOleReader(ref xoRdrOLE);
            DropOdbcReader(ref xoRdrODBC);
            return (xb);
        }
        #endregion

        /// <summary>
        /// send an SQL statement to the open database
        /// </summary>
        /// <param name="psSQL"></param>
        /// <param name="poTbl"></param>
        /// <returns></returns>
        private bool InternalSQL(string psSQL, ref OleDbDataReader poOleRdr, ref OdbcDataReader poOdbcRdr, ref SqlDataReader poSqlRdr, DataTable poTbl = null, bool pbAsynchronously = false)      //, ref object poDataReader)  //, DataTable poTbl)  // = null)
        {
            string xs = "", xsErrMsg = "", xsSQL = "";
            bool xbActionSQL = false, xbUsingDataTable = (poTbl != null);
            mbLastSQLwasAction = false;

            xsSQL = psSQL.Trim();
            if (xsSQL.Length > 0)
            {
                if (xsSQL.Length >= 8)
                {
                    //xs = xsSQL.Substring(0, 8);
                    //xs = xs.ToUpper();
                    //xbSelectStmt = (xs.Substring(0, 7) == "SELECT ");
                    //xbSelectStmt = SQLstatementReturnsData(xsSQL);
                }
                else
                {
                    xsErrMsg = "SQL not specified.";
                }
            }

            if (xsErrMsg.Length == 0)
            {
                try
                {
                    if (IsConnectionClosed())
                    {
                        if (!OpenTheConnection()) xsErrMsg = msErrMsg;
                    }
                }
                catch (Exception xoExc)
                {
                    xsErrMsg = xoExc.Message;
                }
            }

            if (xsErrMsg.Length == 0)
            {
                ScanSQL(xsSQL, out xbActionSQL);
                if (!xbActionSQL)
                {
                    //
                    // SELECT statement handling (returning data to caller)
                    //
                    try
                    {
                        // create a new data adapter based on the supplied query
                        msLastSQL = xsSQL;
                        xs = moDC.msInfo;

                        if (xbUsingDataTable)   //(poTbl != null)
                        {
                            //poTbl = new DataTable();
                            poTbl.Locale = System.Globalization.CultureInfo.InvariantCulture;
                            poTbl.Clear();
                        }

                        switch (moDC.meConnectivity)
                        {
                            case ConnectivityType.OleDB:
                                if (mrSQLParams.Count > 0)
                                {
                                    moDC.moCmdOLE.Parameters.Clear();
                                    for (int xii = 0; xii < mrSQLParams.Count; xii++)
                                    {
                                        OleDbParameter xoParam = new OleDbParameter();
                                        xoParam.ParameterName = mrSQLParams[xii].ParamName;
                                        xoParam.Value = mrSQLParams[xii].ParamValue;
                                        moDC.moCmdOLE.Parameters.Add(xoParam);
                                    }
                                    mrSQLParams = new List<SQLparam>(0);
                                }
                                moDC.moCmdOLE.CommandText = xsSQL;
                                moDC.moCmdOLE.CommandType = CommandType.Text;
                                moDC.moCmdOLE.CommandTimeout = 0;
                                moDC.moCmdOLE.Connection = moDC.moConnOLE;
                                if (xbUsingDataTable)   //(poTbl != null)
                                {
                                    moDC.moAdapterOLE = new OleDbDataAdapter(xsSQL, xs);
                                    moDC.moAdapterOLE.SelectCommand.CommandTimeout = 0;
                                    moDC.moBldrOLE = new OleDbCommandBuilder(moDC.moAdapterOLE);
                                    //moDC.moCmdOLE.CommandTimeout = 0;
                                    moDC.moAdapterOLE.Fill(poTbl);
                                    miLastRecsAffected = poTbl.Rows.Count;
                                }
                                else
                                {
                                    poOleRdr = moDC.moCmdOLE.ExecuteReader();
                                    miLastRecsAffected = poOleRdr.RecordsAffected;
                                }
                                break;

                            case ConnectivityType.ODBC:
                                if (mrSQLParams.Count > 0)
                                {
                                    moDC.moCmdODBC.Parameters.Clear();
                                    for (int xii = 0; xii < mrSQLParams.Count; xii++)
                                    {
                                        OdbcParameter xoParam = new OdbcParameter();
                                        xoParam.ParameterName = mrSQLParams[xii].ParamName;
                                        xoParam.Value = mrSQLParams[xii].ParamValue;
                                        moDC.moCmdODBC.Parameters.Add(xoParam);
                                    }
                                    mrSQLParams = new List<SQLparam>(0);
                                }
                                moDC.moCmdODBC.CommandText = xsSQL;
                                moDC.moCmdODBC.CommandType = CommandType.Text;
                                moDC.moCmdODBC.Connection = moDC.moConnODBC;
                                if (xbUsingDataTable)   //(poTbl != null)
                                {
                                    moDC.moAdapterODBC = new OdbcDataAdapter(xsSQL, xs);
                                    moDC.moBldrODBC = new OdbcCommandBuilder(moDC.moAdapterODBC);
                                    moDC.moAdapterODBC.Fill(poTbl);
                                    miLastRecsAffected = poTbl.Rows.Count;
                                }
                                else
                                {
                                    poOdbcRdr = moDC.moCmdODBC.ExecuteReader();
                                    miLastRecsAffected = poOdbcRdr.RecordsAffected;
                                }
                                break;
                            default:

                                //
                                // NOTE: in my testing, parameters did -NOT- work for DataTable (.Fill)
                                //
                                if (mrSQLParams.Count > 0)
                                {
                                    moDC.moCommand.Parameters.Clear();
                                    for (int xii = 0; xii < mrSQLParams.Count; xii++)
                                    {
                                        SqlParameter xoParam = new SqlParameter();
                                        xoParam.ParameterName = mrSQLParams[xii].ParamName;
                                        xoParam.Value = mrSQLParams[xii].ParamValue;
                                        moDC.moCommand.Parameters.Add(xoParam);
                                    }
                                    mrSQLParams = new List<SQLparam>(0);
                                }

                                moDC.moCommand.CommandText = xsSQL;
                                moDC.moCommand.CommandType = CommandType.Text;
                                moDC.moCommand.CommandTimeout = 0;
                                moDC.moCommand.Connection = moDC.moConn;

                                if (pbAsynchronously)
                                {
                                    if (xbUsingDataTable)
                                    {
                                        AsyncCallback xoCallback = new AsyncCallback(HandleCallback);
                                        moDC.moCommand.BeginExecuteReader(xoCallback, moDC.moCommand, CommandBehavior.Default);
                                    }
                                    else
                                    {
                                        xsErrMsg = "Asnyc calls only supported for DataTable return types at this time.";
                                    }
                                }
                                else
                                {
                                    if (xbUsingDataTable)
                                    {
                                        moDC.moAdapter = new SqlDataAdapter(xsSQL, xs);
                                        moDC.moAdapter.SelectCommand.CommandTimeout = 0;
                                        moDC.moBldr = new SqlCommandBuilder(moDC.moAdapter);
                                        moDC.moAdapter.Fill(poTbl);
                                        miLastRecsAffected = poTbl.Rows.Count;
                                    }
                                    else
                                    {
                                        poSqlRdr = moDC.moCommand.ExecuteReader();
                                        miLastRecsAffected = poSqlRdr.RecordsAffected;
                                    }
                                }
                                break;
                        }
                    }
                    catch (Exception xo)
                    {
                        xsErrMsg = xo.Message;
                    }
                }
                else
                {
                    //
                    // action-based SQL (insert/update/delete/create/alter/drop)
                    //
                    msLastSQL = psSQL;
                    try
                    {
                        switch (moDC.meConnectivity)
                        {
                            case ConnectivityType.OleDB:
                                moDC.moCmdOLE = new OleDbCommand(psSQL, moDC.moConnOLE);
                                miLastRecsAffected = moDC.moCmdOLE.ExecuteNonQuery();
                                break;
                            case ConnectivityType.ODBC:
                                moDC.moCmdODBC = new OdbcCommand(psSQL, moDC.moConnODBC);
                                miLastRecsAffected = moDC.moCmdODBC.ExecuteNonQuery();
                                break;
                            default:
                                if (pbAsynchronously)
                                {
                                    AsyncCallback xoCallback = new AsyncCallback(HandleCallbackNonQuery);
                                    moDC.moCommand = new SqlCommand(psSQL, moDC.moConn);
                                    moDC.moCommand.BeginExecuteNonQuery(xoCallback, moDC.moCommand);
                                }
                                else
                                {
                                    moDC.moCommand = new SqlCommand(psSQL, moDC.moConn);
                                    miLastRecsAffected = moDC.moCommand.ExecuteNonQuery();
                                }
                                break;
                        }
                    }
                    catch (Exception xo)
                    {
                        xsErrMsg = xo.Message;
                    }
                    mbLastSQLwasAction = true;

                }       //else (action instead of SELECT)
            }           // errmsg blank

            // wrapup
            msErrMsg = xsErrMsg;
            return (xsErrMsg.Length == 0);
        }

        private void HandleCallbackNonQuery(IAsyncResult poResult)
        {
            try
            {
                // Retrieve the original command object, passed
                // to this procedure in the AsyncState property
                // of the IAsyncResult parameter.
                SqlCommand xoCommand = (SqlCommand)poResult.AsyncState;
                miLastRecsAffected = xoCommand.EndExecuteNonQuery(poResult);
                OnExecuteQueryAsyncComplete?.Invoke(null);
                xoCommand.Dispose();
                xoCommand = null;
            }
            catch (Exception xoExc)
            {
                msErrMsg = xoExc.Message;
                OnExecuteQueryAsyncComplete?.Invoke(null);
            }
        }

        private void HandleCallback(IAsyncResult poResult)
        {
            try
            {
                // Retrieve the original command object, passed
                // to this procedure in the AsyncState property
                // of the IAsyncResult parameter.

                SqlCommand xoCommand = (SqlCommand)poResult.AsyncState;
                xoCommand.Connection = moDC.moConn;
                SqlDataReader xoReader = xoCommand.EndExecuteReader(poResult);

                // You may not interact with the form and its contents
                // from a different thread, and this callback procedure
                // is all but guaranteed to be running from a different thread
                // than the form. Therefore you cannot simply call code that 
                // fills the grid, like this:
                // FillGrid(reader);
                // Instead, you must call the procedure from the form's thread.
                // One simple way to accomplish this is to call the Invoke
                // method of the form, which calls the delegate you supply
                // from the form's thread. 

                //FillGridDelegate del = new FillGridDelegate(FillGrid);
                //this.Invoke(del, xoReader);

                DataTable xoTable = new DataTable();
                xoTable.Load(xoReader);
                xoReader.Close();
                xoReader = null;
                miLastRecsAffected = xoTable.Rows.Count;
                OnExecuteQueryAsyncComplete?.Invoke(xoTable);
                xoCommand.Dispose();
                xoCommand = null;

            }
            catch (Exception xoExc)
            {
                // Because you are now running code in a separate thread, 
                // if you do not handle the exception here, none of your other
                // code catches the exception. Because there is none of 
                // your code on the call stack in this thread, there is nothing
                // higher up the stack to catch the exception if you do not 
                // handle it here. You can either log the exception or 
                // invoke a delegate (as in the non-error case in this 
                // example) to display the error on the form. In no case
                // can you simply display the error without executing a delegate
                // as in the try block here. 
                // You can create the delegate instance as you 
                // invoke it, like this:
                //this.Invoke(new DisplayStatusDelegate(DisplayStatus),
                //    "Error: " + ex.Message);

                msErrMsg = xoExc.Message;
                OnExecuteQueryAsyncComplete?.Invoke(null);
            }
        }

        public bool Update(DataTable poTbl)
        {
            string xsErrMsg = "";

            try
            {
                switch (moDC.meConnectivity)
                {
                    case ConnectivityType.OleDB:
                        moDC.moAdapterOLE.Update(poTbl);
                        break;
                    case ConnectivityType.ODBC:
                        moDC.moAdapterODBC.Update(poTbl);
                        break;
                    default:
                        moDC.moAdapter.Update(poTbl);
                        break;
                }
            }
            catch (Exception xo)
            {
                xsErrMsg = xo.Message;
            }
            msErrMsg = xsErrMsg;
            return (xsErrMsg.Length == 0);
        }

        #region internalPlumbing

        //
        // DEPRECATED - replaced by ScanSQL()
        //
        private bool SQLstatementReturnsData(string psSQL)
        {
            // this is not fool-proof, and would be difficult to make so...
            // now bypassing this logic and effectively sending all SQL
            // statements through ExecuteReader/Fill (instead of
            // ExecuteNonQuery), regardless
            string xs = psSQL.ToUpper();
            //return (xs.IndexOf("SELECT ") >= 0);
            return (true);
        }

        private string PlugVariablesIntoConnectionString(DatabaseToOpen prDbToOpen)
        {
            string xsConnStr = "";
            bool xbTrusted = (prDbToOpen.UserID == null || prDbToOpen.UserID.Length == 0);

            switch (prDbToOpen.ConnectionType)
            {
                case ConnectivityType.ODBC:
                    if (xbTrusted)
                    {
                        xsConnStr = prDbToOpen.Provider.OdbcConnStrTrusted.TrimEnd();
                        if (xsConnStr.Length == 0) xsConnStr = prDbToOpen.Provider.OdbcConnStr.TrimEnd();
                    }
                    else
                    {
                        xsConnStr = prDbToOpen.Provider.OdbcConnStr.TrimEnd();
                        if (xsConnStr.Length == 0) xsConnStr = prDbToOpen.Provider.OdbcConnStrTrusted.TrimEnd();
                    }
                    //if (xbTrusted) xsConnStr = prDbToOpen.Provider.OdbcConnStrTrusted; else xsConnStr = prDbToOpen.Provider.OleDbConnStr;
                    break;
                case  ConnectivityType.DotNet:
                    if (xbTrusted)
                    {
                        xsConnStr = prDbToOpen.Provider.DotNetConnStrTrusted.TrimEnd();
                        if (xsConnStr.Length == 0) xsConnStr = prDbToOpen.Provider.DotNetConnStr.TrimEnd();
                    }
                    else
                    {
                        xsConnStr = prDbToOpen.Provider.DotNetConnStr.TrimEnd();
                        if (xsConnStr.Length == 0) xsConnStr = prDbToOpen.Provider.DotNetConnStrTrusted.TrimEnd();
                    }
                    //if (xbTrusted) xsConnStr = prDbToOpen.Provider.DotNetConnStrTrusted; else xsConnStr = prDbToOpen.Provider.DotNetConnStr;
                    break;
                default:
                    if (xbTrusted)
                    {
                        xsConnStr = null2str(prDbToOpen.Provider.OleDbConnStrTrusted).TrimEnd();
                        if (xsConnStr.Length == 0) xsConnStr = null2str(prDbToOpen.Provider.OleDbConnStr).TrimEnd();
                    }
                    else
                    {
                        xsConnStr = prDbToOpen.Provider.OleDbConnStr.TrimEnd();
                        if (xsConnStr.Length == 0) xsConnStr = null2str(prDbToOpen.Provider.OleDbConnStrTrusted).TrimEnd();
                    }
                    //if (xbTrusted) xsConnStr = prDbToOpen.Provider.OleDbConnStrTrusted; else xsConnStr = prDbToOpen.Provider.OleDbConnStr;
                    break;
            }
            xsConnStr = xsConnStr.Replace("{DS}", prDbToOpen.DataSource);
            xsConnStr = xsConnStr.Replace("{DB}", prDbToOpen.Database);
            xsConnStr = xsConnStr.Replace("{UID}", prDbToOpen.UserID);
            xsConnStr = xsConnStr.Replace("{PSWD}", prDbToOpen.Password);
            return (xsConnStr);
        }

        //private string PlugVarsIntoGenericConnectionString(string psConnStr, ConnStrVars psConnStrVars, string psProviderOverideStr)
        //{
        //    #region plugSamples
        //    //
        //    // MS ACCESS sample
        //    // sample of a passed psConnStr
        //    // "Data Source={DS};User ID={UID};Password={PSWD}"
        //    //
        //    // samples of a passed psConnStrVars
        //    // DbBrand = Brand.MsAccess
        //    // DbFileOrDataSourc  = "C:\MyApp\MyAppDataFile.mdb"
        //    // SQLDatabase = ""
        //    // UserID = "admin"
        //    // Password = "123"
        //    //
        //    // returned string: "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=MyDataSrc;UserID=admin;Password=123"
        //    //

        //    //
        //    // SQL SERVER sample
        //    // sample of a passed psConnStr
        //    // "Data Source={DS};Database={DB};UserID={UID};Password={PSWD}"
        //    //
        //    // samples of a passed psConnStrVars
        //    // DbBrand = Brand.SqlSrvr
        //    // DbFileOrDataSourc  = "MyDataSrc"
        //    // SQLDatabase = ""
        //    // UserID = "sa"
        //    // Password = "123"
        //    //
        //    // returned string: "Provider=SQLOLEDB;Data Source=MyDataSrc;UserID=sa;Password=123"
        //    //
        //    #endregion

        //    // init
        //    string xs = "", xsConnStr = "", xsVar = "", xsVal = "", xsProviderStr = "Microsoft.Jet.OLEDB.4.0";
        //    bool xbSqlSrvrUserIdIsBlank = false;
        //    int xi = 0, xii = 0;

        //    xsConnStr = psConnStr.Trim();
        //    xs = psProviderOverideStr.Trim();

        //    // simple sample IIF in C#
        //    xs = (xi == 0 | xi == 1) ? "A" : "b";

        //    // begin process, first by determining database provider
        //    xsConnStr = psConnStr.Trim();
        //    xs = psProviderOverideStr.Trim();
        //    if (xs.Length > 0)
        //    {
        //        xsProviderStr = xs;
        //    }
        //    else
        //    {
        //        switch (psConnStrVars.DbBrand)
        //        {
        //            case Brand.SqlSrvr:
        //                xsProviderStr = "SQLOLEDB";
        //                break;
        //            case Brand.VFP:
        //                xsProviderStr = "vfpoledb";
        //                break;
        //            case Brand.Oracle:
        //                xsProviderStr = "OraOLEDB.Oracle";
        //                break;
        //        }
        //    }

        //    xsConnStr = xsConnStr.Replace(" =", "=");

        //    // now, remove "Provider=" (case neutral), since we build this, normally, unless user is overriding because of a new or improved OLE DB provider
        //    if (moDC.meConnectivity == ConnectivityType.ODBC) xsVar = "DRIVER="; else xsVar = "PROVIDER=";
        //    do
        //    {
        //        xs = xsConnStr.ToUpper();
        //        xi = xs.IndexOf(xsVar);
        //        if (xi >= 0)
        //        {
        //            xs = xsConnStr.Substring(0, xi - 1) + xsConnStr.Substring(xi + xsVar.Length);
        //            xsConnStr = xs.Trim();
        //        }

        //    } while (xi >= 0);

        //    // now, remove any extra/preceding parameter separator(s) ";"
        //    do
        //    {
        //        if (xsConnStr.Substring(0, 1) == ";")
        //        {
        //            xsConnStr = xsConnStr.Substring(1).Trim();
        //            xi = 0;
        //        }
        //        else
        //        {
        //            xi = -1;
        //        }
        //    } while (xi >= 0);

        //    // let's put the Provider on the front
        //    switch (moDC.meConnectivity)
        //    {
        //        case ConnectivityType.ODBC:
        //            xsVar = "Driver=";
        //            break;
        //        case ConnectivityType.OleDB:
        //            xsVar = "Provider=";
        //            break;
        //        default:
        //            xsVar = "";
        //            break;
        //    }
        //    if (xsVar.Length > 0) xsConnStr = xsVar + xsProviderStr + ";" + xsConnStr;

        //    // finally, plug in the variables
        //    // "Data Source={DS};Database={DB};UserID={UID};Password={PSWD}"
        //    for (xii = 1; xii <= 4; xii = xii + 1)
        //    {
        //        switch (xii)
        //        {
        //            case 1:
        //                xs = "DS";
        //                xsVal = psConnStrVars.DbFileOrDataSource.Trim();
        //                break;
        //            case 2:
        //                xs = "DB";
        //                xsVal = psConnStrVars.SQLDatabase.Trim();
        //                break;
        //            case 3:
        //                xs = "UID";
        //                xsVal = psConnStrVars.UserID.Trim();
        //                if (psConnStrVars.DbBrand == Brand.SqlSrvr && xsVal.Length == 0) xbSqlSrvrUserIdIsBlank = true;
        //                break;
        //            case 4:
        //                xs = "PSWD";
        //                xsVal = psConnStrVars.Password.Trim();
        //                break;
        //        }
        //        xsVar = "{" + xs.ToUpper() + "}";
        //        xsConnStr = xsConnStr.Replace(xsVar, xsVal);
        //    }

        //    // some additional processing for SQL Server
        //    if (xbSqlSrvrUserIdIsBlank)
        //    {
        //        xs = xsConnStr.ToUpper();

        //        string xsParam = "USER ID=";
        //        int xiParamLen = xsParam.Length;
        //        xi = xs.IndexOf(xsParam);
        //        if (xi < 0)
        //        {
        //            xsParam = "UID=";
        //            xiParamLen = xsParam.Length;
        //            xi = xs.IndexOf(xsParam);
        //        }
        //        if (xi >= 0)
        //        {
        //            string xsSecurity = "";
        //            switch (moDC.meConnectivity)
        //            {
        //                case ConnectivityType.ODBC:
        //                    xsSecurity = "Trusted_Connection=Yes";
        //                    break;
        //                case ConnectivityType.OleDB:
        //                    xsSecurity = "Integrated Security=SSPI";
        //                    break;
        //                default:
        //                    xsSecurity = "Trusted_Connection=True";
        //                    break;
        //            }
        //            xsConnStr = xsConnStr.Substring(0, xi) + xsSecurity + xsConnStr.Substring(xi + xiParamLen);
        //            xs = xsConnStr.ToUpper();
        //            xsParam = "PASSWORD=";
        //            xiParamLen = xsParam.Length;
        //            xi = xs.IndexOf(xsParam);
        //            if (xi < 0)
        //            {
        //                xsParam = "PWD=";
        //                xiParamLen = xsParam.Length;
        //                xi = xs.IndexOf(xsParam);
        //            }
        //            if (xi >= 0) xsConnStr = xsConnStr.Substring(0, xi - 1) + xsConnStr.Substring(xi + xiParamLen);
        //            xs = xsConnStr;
        //        }
        //    }
        //    // lastly, return connection the command line with the variables in place
        //    return xsConnStr;
        //}

        /// <summary>
        /// last SQL statement interpreted as action if true, otherwise data returned
        /// </summary>
        /// <returns></returns>
        public bool LastSQLwasAction()
        {
            return (mbLastSQLwasAction);
        }

        /// <summary>
        /// passes back a string based on the enumerated database brand
        /// </summary>
        /// <param name="peBrand"></param>
        /// <returns></returns>
        public string GetProviderDescription(Brand peBrand)
        {
            string xsDesc = "?";

            switch (peBrand)
            {
                case Brand.MsAccess:
                    xsDesc = "ACCESS";
                    break;
                case Brand.VFP:
                    xsDesc = "VISUAL FOXPRO";
                    break;
                case Brand.SqlSrvr:
                    xsDesc = "SQL Server";
                    break;
                case Brand.Oracle:
                    xsDesc = "Oracle";
                    break;
            }
            return xsDesc;
        }

        /// <summary>
        /// sets a brand based on the passed-in description
        /// </summary>
        /// <param name="psDescription"></param>
        /// <returns></returns>
        public Brand GetProviderFromDescription(string psDescription)
        {
            string xs = psDescription.ToUpper();
            int xi = 0;
            Brand xeProvider = Brand.Unknown;

            xi = xs.IndexOf("ACCESS");
            if (xi >= 0)
            {
                xeProvider = Brand.MsAccess;
            }
            else
            {
                xi = xs.IndexOf("FOX");
                if (xi >= 0)
                {
                    xeProvider = Brand.VFP;
                }
                else
                {
                    xi = xs.IndexOf("SERVER");
                    if (xi >= 0)
                    {
                        xeProvider = Brand.SqlSrvr;
                    }
                    else
                    {
                        xi = xs.IndexOf("ORACLE");
                        if (xi >= 0)
                        {
                            xeProvider = Brand.Oracle;
                        }
                    }
                }
            }
            return xeProvider;
        }

        /// <summary>
        /// determine whether or not the SQL statement returns data, or instead, acts upon data/schema (i.e. INSERT, UPDATE, DELETE, ALTER, etc.)
        /// </summary>
        /// <param name="psSQL"></param>
        /// <param name="pbIsActionSQL"></param>
        private void ScanSQL(string psSQL, out bool pbIsActionSQL)
        {
            string xsKeyWord = "";
            bool xbIsActionSQL = false;
            bool xbInCommentTypeSlashStar = false;
            bool xbInCommentTypeSlashStarMaybe = false;
            bool xbInCommentTypeDashDash = false;
            bool xbKeyWordChar = false;
            int xiPos = -1, xiCommentStartPos = -1, xiQtStartPos = -1, xiKeyWordStartPos = -1;
            char xcLastQtChar = '\0', xcPrevChar = '\0', xcChr;
            bool xbInQt = false, xbInComment = false;

            for (xiPos = 0; xiPos < psSQL.Length; xiPos++)
            {
                xcChr = psSQL[xiPos];
                if (xbInQt)
                {
                    // just need to see if we're going "out of quote"
                    if (xcChr == xcLastQtChar)
                    {
                        xcLastQtChar = xcChr;
                        xbInQt = false;
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
                        //
                        // we're not INCOMMENT or INQUOTE, so take action accordingly
                        //
                        xbKeyWordChar = false;
                        if (xcChr >= 'A' && xcChr <= 'Z')
                        {
                            xbKeyWordChar = true;
                        }
                        else
                        {
                            if (xcChr >= 'a' && xcChr <= 'z')
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
                                            //psSQL.SelectionStart = xiCursorPos;
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
                                    //if (!xbIsActionSQL) xbIsActionSQL = IsActionKeyWord(xsKeyWord);
                                    if (xbIsActionSQL)
                                    {
                                        if (xsKeyWord == "SELECT") xbIsActionSQL = false;
                                    }
                                    else
                                    {
                                        xbIsActionSQL = IsActionKeyWord(xsKeyWord);
                                    }


                                    xiKeyWordStartPos = -1;
                                    xsKeyWord = "";
                                }
                                else
                                {
                                    xsKeyWord = "";
                                    xiKeyWordStartPos = -1;
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
                                                    xbInCommentTypeDashDash = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                xcPrevChar = xcChr;
            }

            pbIsActionSQL = xbIsActionSQL;
            return;
        }

        private bool IsActionKeyWord(string psWord)
        {
            bool xbIs = false;
            const string xcsActionKeywords = "/INSERT/UPDATE/DELETE/CREATE/ALTER/DROP/////";       // needs to have enough "/" on end for longest possible (10 at present, seee SubString below)

            string xsWord = "/" + psWord.ToUpper();

            int xi = xcsActionKeywords.IndexOf(xsWord);
            if (xi >= 0)
            {
                string xsKeyWord = xcsActionKeywords.Substring(xi, 10);
                xi = xsKeyWord.IndexOf("/", 1);
                xsKeyWord = xsKeyWord.Substring(0, xi);
                xbIs = xsWord == xsKeyWord;
            }

            return (xbIs);
        }

        private bool IsKeyWord(string psWord)
        {
            const string xcsKeywords = "/AS/ASC/OR/ORDER/IN/INDEX/INNER/INSERT/IF/BY/TO/TOP/SET/AND/ADD/ON/SELECT/FROM/IS/WHERE/NOT/WITH/UNION/HAVING/EXISTS/NULL/DECLARE/UPDATE/DELETE/VALUES/CREATE/ALTER/TABLE/COLUMN/DROP/DESC/GROUP/LEFT/RIGHT/JOIN/LEFT/CAST/OUTER/DISTINCT/CASE/WHEN/THEN/BEGIN/END/ELSE/IIF/PIVOT/FOR/FOREIGN/OF/OFF/      ";

            bool xb = false;
            string xsWord = psWord.ToUpper();

            int xi = xcsKeywords.IndexOf(xsWord);
            if (xi >= 0)
            {
                string xsKeyWord = xcsKeywords.Substring(xi, 10);
                xi = xsKeyWord.IndexOf("/");
                xsKeyWord = xsKeyWord.Substring(0, xi);
                xb = xsWord == xsKeyWord;
            }

            return (xb);
        }

        #endregion


        #region schemaStuff
        /// <summary>
        /// write to a tab-delimted string list of the schema of columns for the specified table in the database
        /// </summary>
        /// <param name="psTableName">table name to return schema for</param>
        /// <returns>tab-delimited string list of data structure</returns>
        /// 
        public List<string> GetStructure(string psTableName)
        {
            string xsErrMsg = "", xsDB = this.msDatabase;
            List<string> xsStruc = null;
            msErrMsg = "";
            DataTable xoTable = null;

            if (DatabaseIsOpen())
            {
                if (IsConnectionClosed()) OpenTheConnection();
            }
            else
            {
                xsErrMsg = "Database is not open.";
            }
            if (msErrMsg.Length == 0 && xsErrMsg.Length == 0)
            {
                //if (moDC.meBrand == Brand.MsAccess) xsDB = null;
                if (moDC.Provider.IsAccess) xsDB = null;
                try
                {
                    //if (moDC.meBrand == Brand.Oracle)
                    if (moDC.Provider.IsOracle)
                    {
                        xoTable = new DataTable();
                        if (this.SQL("SELECT * FROM user_tab_columns WHERE table_name = '" + psTableName.ToUpper().Trim() + "'", xoTable))
                        {
                            if (xoTable == null) xsErrMsg = "Data not found.";
                        }
                        else
                        {
                            xsErrMsg = msErrMsg;
                        }
                    }
                    else
                    {
                        switch (moDC.meConnectivity)
                        {
                            case ConnectivityType.OleDB:
                                xoTable = moDC.moConnOLE.GetSchema("Columns", new[] { xsDB, null, psTableName });
                                break;
                            case ConnectivityType.ODBC:
                                xoTable = moDC.moConnODBC.GetSchema("Columns", new[] { xsDB, null, psTableName });
                                break;
                            default:
                                xoTable = moDC.moConn.GetSchema("Columns", new[] { xsDB, null, psTableName });
                                break;
                        }
                    }
                }
                catch (Exception xoExc)
                {
                    xsErrMsg = xoExc.Message;
                }
                #region oldway
                //for (int xiRow = 0; xiRow < xoTable.Rows.Count; xiRow++)
                //{
                //    string xsColmName = "", xsDataType = "", xsPrecision = "";

                //    for (int xiCol = 0; xiCol < xoTable.Columns.Count; xiCol++)
                //    {
                //        string xsCol = xoTable.Columns[xiCol].ColumnName.Trim().ToUpper();

                //        switch (moDC.meBrand)
                //        {
                //            //
                //            // SQL Server
                //            //
                //            case Brand.SqlSrvr:
                //                if (xsCol == "COLUMN_NAME")
                //                {
                //                    xsColmName = xoTable.Rows[xiRow][xiCol].ToString();
                //                }
                //                else
                //                {
                //                    if (xsCol == "DATA_TYPE")
                //                    {
                //                        xsDataType = xoTable.Rows[xiRow][xiCol].ToString();
                //                        int xiDataType = -500;
                //                        try
                //                        {
                //                            xiDataType = Convert.ToInt16(xsDataType);
                //                        }
                //                        catch
                //                        {

                //                        }
                //                        //create table numbers (f1 bigint, f2 bit, f3, decimal, f4 int, f5 money, f6 numeric, f7 smallint, f8 smallmoney, f9 tinyInt)
                //                        switch (xiDataType)
                //                        {
                //                            case -500:
                //                                break;
                //                            case 2:
                //                                xsDataType = "smallInt";
                //                                break;
                //                            case 3:
                //                                xsDataType = "int";
                //                                break;
                //                            case 6:
                //                                xsDataType = "money/smallMoney";
                //                                break;
                //                            case 11:
                //                                xsDataType = "bit";
                //                                break;
                //                            case 17:
                //                                xsDataType = "tinyInt";
                //                                break;
                //                            case 19:
                //                                break;
                //                            case 20:
                //                                xsDataType = "bigInt";
                //                                break;
                //                            case 131:
                //                                xsDataType = "decimal/numeric";
                //                                break;
                //                            default:
                //                                break;
                //                        }
                //                    }
                //                    else
                //                    {
                //                        if (xsCol == "NUMERIC_PRECISION")
                //                        {
                //                            if (xoTable.Rows[xiRow][xiCol] != null) xsPrecision = xoTable.Rows[xiRow][xiCol].ToString();
                //                        }
                //                    }
                //                }
                //                break;
                //            default:
                //                break;
                //        }
                //    }
                //    if (xiRow == 0) xsStruc = new List<string>();
                //    string xsItem = "";
                //    xsItem = xsColmName + ", " + xsDataType;
                //    //if (xsPrecision.Length > 0) xsItem = xsItem + ", Precision: " + xsPrecision;
                //    xsStruc.Add(xsItem);

                //    //if (xsInfo.Length > 0) xsInfo += "\r\n";
                //    //xsInfo += xs;
                // }
                #endregion

                if (xsErrMsg.Length == 0)
                {
                    string xsLine = "";
                    xsStruc = new List<string>();
                    for (int xiCol = 0; xiCol < xoTable.Columns.Count; xiCol++)
                    {
                        if (xiCol > 0) xsLine += "\t";
                        xsLine += xoTable.Columns[xiCol].ColumnName;
                    }
                    xsStruc.Add(xsLine);
                    for (int xiRow = 0; xiRow < xoTable.Rows.Count; xiRow++)
                    {
                        xsLine = "";
                        for (int xiCol = 0; xiCol < xoTable.Columns.Count; xiCol++)
                        {
                            if (xiCol > 0) xsLine += "\t";
                            xsLine += xoTable.Rows[xiRow][xiCol].ToString();
                        }
                        xsStruc.Add(xsLine);
                    }
                }

                if (xoTable != null) xoTable.Dispose();
            }

            if (xoTable != null)
            {
                if (xsErrMsg.Length == 0)
                {
                    if (xoTable.Rows.Count == 0) xsErrMsg = "Table " + psTableName + " not found in schema.";
                }
            }
            xoTable = null;

            msErrMsg = xsErrMsg;
            return (xsStruc);
        }

        /// <summary>
        ///  write to a tab-delimted text file the schema of columns for the specified table in the database
        /// </summary>
        /// <param name="psTableName">table name in database to retrieve structure for</param>
        /// <param name="psOutFile">output text file to save structure in</param>
        /// <returns>true if successful, else false</returns>
        public bool SaveStructure(string psTableName, string psOutFile)
        {
            string xsErrMsg = "", xsDB = this.msDatabase;
            //List<string> xsStruc = null;      // oldway
            msErrMsg = "";
            DataTable xoTable = null;

            if (DatabaseIsOpen())
            {
                if (IsConnectionClosed()) OpenTheConnection();
            }
            else
            {
                xsErrMsg = "Database is not open.";
            }
            if (msErrMsg.Length == 0 && xsErrMsg.Length == 0)
            {
                //if (moDC.meBrand == Brand.MsAccess) xsDB = null;
                if (moDC.Provider.IsAccess) xsDB = null;
                try
                {
                    switch (moDC.meConnectivity)
                    {
                        case ConnectivityType.OleDB:
                            xoTable = moDC.moConnOLE.GetSchema("Columns", new[] { xsDB, null, psTableName });
                            break;
                        case ConnectivityType.ODBC:
                            xoTable = moDC.moConnODBC.GetSchema("Columns", new[] { xsDB, null, psTableName });
                            break;
                        default:
                            xoTable = moDC.moConn.GetSchema("Columns", new[] { xsDB, null, psTableName });
                            break;
                    }
                }
                catch (Exception xoExc)
                {
                    xsErrMsg = xoExc.Message;
                }
                if (xsErrMsg.Length == 0)
                {
                    string xsOutFile = psOutFile.Trim();
                    if (xsOutFile.Length > 0)
                    {
                        if (File.Exists(xsOutFile))
                        {
                            try
                            {
                                File.Delete(xsOutFile);
                            }
                            catch (Exception xoExc)
                            {
                                xsErrMsg = xoExc.Message;
                            }
                        }
                        if (xsErrMsg.Length == 0)
                        {
                            if (!DumpTableToCSV(xoTable, psOutFile.Trim())) xsErrMsg = msErrMsg;
                        }
                    }
                    else
                    {
                        xsErrMsg = "No output file specified.";
                    }
                }

                xoTable.Dispose();
            }

            if (xoTable != null)
            {
                if (xsErrMsg.Length == 0)
                {
                    if (xoTable.Rows.Count == 0) xsErrMsg = "Table " + psTableName + " not found in schema.";
                }
            }
            xoTable = null;

            msErrMsg = xsErrMsg;
            return (xsErrMsg.Length == 0);
        }

        /// <summary>
        /// return the tables in the currently open database
        /// </summary>
        /// <param name="psTable"></param>
        /// <param name="pbAll"></param>
        /// <returns></returns>
        public List<string> GetTables(bool pbAll = false)             //(ref List<string> psTbls, bool pbAll = false)
        {
            string xsErrMsg = "", xsTblNm = "";
            int xii = 1, xi2 = 2, xiTables = 0, xiTblTypeColm = -1, xiTblNameColm = -1;
            bool xbIsTbl = false;
            DataTable xoTable = null;
            object xv = null;
            List<string> xsStruc = null;
            //psTbls = null;

            if (xsErrMsg.Length == 0)
            {
                try
                {
                    switch (moDC.meConnectivity)
                    {
                        case ConnectivityType.ODBC:
                            xoTable = moDC.moConnODBC.GetSchema("TABLES");
                            break;
                        case ConnectivityType.OleDB:
                            xoTable = moDC.moConnOLE.GetSchema("TABLES");
                            break;
                        default:
                            xoTable = moDC.moConn.GetSchema("TABLES");
                            break;
                    }
                    xsErrMsg = "";
                    string xsLine = "";
                    xsStruc = new List<string>();
                    for (int xiCol = 0; xiCol < xoTable.Columns.Count; xiCol++)
                    {
                        if (xiCol > 0) xsLine += "\t";
                        string xsColmName = xoTable.Columns[xiCol].ColumnName;
                        xsLine += xsColmName;
                        if (xiTblTypeColm < 0) if (xsColmName.Trim().ToUpper() == "TABLE_TYPE") xiTblTypeColm = xiCol;
                        if (xiTblNameColm < 0) if (xsColmName.Trim().ToUpper() == "TABLE_NAME") xiTblNameColm = xiCol;
                    }
                    xsStruc.Add(xsLine);
                    for (xii = 1; xii <= xoTable.Rows.Count; xii++)
                    {
                        xsTblNm = "";
                        xbIsTbl = false;
                        xsLine = "";
                        for (xi2 = 0; xi2 < xoTable.Columns.Count; xi2++)
                        {
                            xv = xoTable.Rows[xii - 1][xi2];
                            if (xi2 > 0) xsLine += "\t";
                            xsLine += xv.ToString();
                            if (xi2 == xiTblTypeColm)
                            {
                                string xs = xv.ToString().Trim();
                                if (xs == "TABLE" || xs == "BASE TABLE") xbIsTbl = true;
                            }
                            else
                            {
                                if (xi2 == xiTblNameColm)
                                {
                                    xsTblNm = xv.ToString().Trim();
                                }
                            }
                        }
                        if (xbIsTbl || pbAll)
                        {
                            xsStruc.Add(xsLine);
                            xiTables++;
                            //if (xiTables == 1) psTbls = new List<string>();
                            //psTbls.Add(xsTblNm);
                        }
                    }
                }
                catch (Exception xoExc)
                {
                    xsErrMsg = xoExc.Message;
                }
            }
            try { xoTable.Dispose(); }
            catch { }
            xoTable = null;
            msErrMsg = xsErrMsg;
            return (xsStruc);
        }

        /// <summary>
        /// return tab-delimted string list of indexes (and their details) for the specified table
        /// </summary>
        /// <param name="psTableName">table name to return index information for</param>
        /// <returns>string list</returns>
        public List<string> GetIndices(string psTableName)
        {
            string xsErrMsg = "", xsDB = this.msDatabase;
            List<string> xsStruc = null;
            msErrMsg = "";
            DataTable xoTable = null;

            if (DatabaseIsOpen())
            {
                if (IsConnectionClosed()) OpenTheConnection();
            }
            else
            {
                xsErrMsg = "Database is not open.";
            }
            if (msErrMsg.Length == 0 && xsErrMsg.Length == 0)
            {
                //if (moDC.meBrand == Brand.MsAccess) xsDB = null;
                if (moDC.Provider.IsAccess) xsDB = null;
                try
                {
                    //if (moDC.meBrand == Brand.Oracle)
                    if (moDC.Provider.IsOracle)
                    {
                        xoTable = new DataTable();
                        if (this.SQL("SELECT * FROM user_indexes WHERE table_name = '" + psTableName.Trim().ToUpper() + "'", xoTable))
                        {
                            if (xoTable == null) xsErrMsg = "Indices not found.";
                        }
                        else
                        {
                            xsErrMsg = msErrMsg;
                        }
                    }
                    else
                    {
                        //xoTable = moDC.moConn.GetSchema();
                        //xoTable.Clear();
                        switch (moDC.meConnectivity)
                        {
                            case ConnectivityType.OleDB:
                                xoTable = moDC.moConnOLE.GetSchema("Indexes", new[] { xsDB, null, null, null, psTableName });       // this works for Access, SQL server
                                break;
                            case ConnectivityType.ODBC:
                                xoTable = moDC.moConnODBC.GetSchema("Indexes", new[] { xsDB, null, null, null, psTableName });
                                break;
                            default:
                                xoTable = moDC.moConn.GetSchema("Indexes", new[] { null, null, psTableName, null });
                                break;
                        }
                    }
                }
                catch (Exception xoExc)
                {
                    xsErrMsg = xoExc.Message;
                }

                if (xsErrMsg.Length == 0)
                {
                    string xsLine = "";
                    xsStruc = new List<string>();
                    for (int xiCol = 0; xiCol < xoTable.Columns.Count; xiCol++)
                    {
                        if (xiCol > 0) xsLine += "\t";
                        xsLine += xoTable.Columns[xiCol].ColumnName;
                    }
                    xsStruc.Add(xsLine);
                    for (int xiRow = 0; xiRow < xoTable.Rows.Count; xiRow++)
                    {
                        xsLine = "";
                        for (int xiCol = 0; xiCol < xoTable.Columns.Count; xiCol++)
                        {
                            if (xiCol > 0) xsLine += "\t";
                            xsLine += xoTable.Rows[xiRow][xiCol].ToString();
                        }
                        xsStruc.Add(xsLine);
                    }
                }
                if (xoTable != null) xoTable.Dispose();
            }

            if (xoTable != null)
            {
                if (xsErrMsg.Length == 0)
                {
                    if (xoTable.Rows.Count == 0) xsErrMsg = "Table " + psTableName + " not found in schema.";
                }
            }
            try { xoTable.Dispose(); }
            catch { }
            xoTable = null;

            msErrMsg = xsErrMsg;
            return (xsStruc);
        }

        public List<string> GetDatabaseObjectsFromSchema(string psObjects = "Views")
        {
            string xsErrMsg = "";
            int xiViews = 0;
            DataTable xoTable = null;
            object xv = null;
            List<string> xsStruc = null;

            if (xsErrMsg.Length == 0)
            {
                try
                {
                    switch (moDC.meConnectivity)
                    {
                        case ConnectivityType.ODBC:
                            xoTable = moDC.moConnODBC.GetSchema(psObjects);
                            break;
                        case ConnectivityType.OleDB:
                            xoTable = moDC.moConnOLE.GetSchema(psObjects);
                            break;
                        default:
                            xoTable = moDC.moConn.GetSchema(psObjects);
                            break;
                    }
                    xsErrMsg = "";
                    string xsLine = "";
                    xsStruc = new List<string>();
                    for (int xiCol = 0; xiCol < xoTable.Columns.Count; xiCol++)
                    {
                        if (xiCol > 0) xsLine += "\t";
                        string xsColmName = xoTable.Columns[xiCol].ColumnName;
                        xsLine += xsColmName;
                    }
                    xsStruc.Add(xsLine);
                    for (int xii = 1; xii <= xoTable.Rows.Count; xii++)
                    {

                        xsLine = "";
                        for (int xi2 = 0; xi2 < xoTable.Columns.Count; xi2++)
                        {
                            xv = xoTable.Rows[xii - 1][xi2];
                            if (xi2 > 0) xsLine += "\t";
                            xsLine += xv.ToString();
                        }
                        xsStruc.Add(xsLine);
                        xiViews++;
                    }
                }
                catch (Exception xoExc)
                {
                    xsErrMsg = xoExc.Message;
                }
            }
            try { xoTable.Dispose(); }
            catch { }
            xoTable = null;
            msErrMsg = xsErrMsg;
            return (xsStruc);
        }

        #endregion

        #region gridStuff
        /// <summary>
        /// take the tab-delimited list of data and place into the passed data grid
        /// </summary>
        /// <param name="poGrd">data grid to populate</param>
        /// <param name="psLines">the tab-delimited data to populate with</param>
        /// <returns>true on success, otherwise false (examine Message as to why)</returns>
        public bool ToGridFromList(DataGridView poGrd, List<string> psLines)
        {
            string xsErrMsg = "";

            try
            {
                poGrd.Rows.Clear();

                if (psLines != null)
                {
                    if (psLines.Count > 0)
                    {
                        for (int xiLine = 0; xiLine < psLines.Count; xiLine++)
                        {
                            string[] xsItems = psLines[xiLine].ToString().Split(Convert.ToChar("\t"));
                            if (xiLine == 0)
                            {
                                for (int xiCol = 0; xiCol < xsItems.Count(); xiCol++)
                                {
                                    poGrd.Columns.Add("colm" + xiCol.ToString(), xsItems[xiCol]);
                                }
                            }
                            else
                            {
                                poGrd.Rows.Add(xsItems);
                            }
                        }
                    }
                }
            }
            catch (Exception xoExc)
            {
                xsErrMsg = xoExc.Message;
            }
            msErrMsg = xsErrMsg;
            return (xsErrMsg.Length == 0);
        }

        /// <summary>
        /// take the tab-delimited text file, and use it to populate the passed data grid
        /// </summary>
        /// <param name="poGrd">data grid to populate</param>
        /// <param name="psCsvFile">the file containing the tab-delimited data to populate with</param>
        public bool ToGridFromTabDelimFile(DataGridView poGrd, string psCsvFile)
        {
            StreamReader xoRdr = null;
            string xsLine = "", xsErrMsg = "";
            string[] xsItems = { "" };

            try
            {
                poGrd.Rows.Clear();

                xoRdr = new StreamReader(psCsvFile);

                while (!xoRdr.EndOfStream)
                {
                    xsLine = xoRdr.ReadLine();

                    xsItems = xsLine.Split(Convert.ToChar("\t"));

                    if (poGrd.Columns.Count == 0)
                    {
                        for (int xiCol = 0; xiCol < xsItems.Count(); xiCol++)
                        {
                            poGrd.Columns.Add("colm" + xiCol.ToString(), xsItems[xiCol]);
                        }
                    }
                    else
                    {
                        poGrd.Rows.Add(xsItems);
                    }

                    xsLine = "";
                }
            }
            catch (Exception xoExc)
            {
                xsErrMsg = xoExc.Message;
            }

            if (xoRdr != null)
            {
                try { xoRdr.Close(); }
                catch { }
                try { xoRdr.Dispose(); }
                catch { }
            }
            xoRdr = null;
            msErrMsg = xsErrMsg;
            return (xsErrMsg.Length == 0);
        }
        #endregion

        #region utility


        public List<string> GetSQLServersRunning()
        {
            System.Data.Sql.SqlDataSourceEnumerator xoEnum = System.Data.Sql.SqlDataSourceEnumerator.Instance;
            DataTable xoTbl = new DataTable();
            List<string> xsServers = new List<string>(0);

            xoTbl = xoEnum.GetDataSources();
            //if (xoTbl.Rows.Count == 0) xoTbl = xoEnum.GetDataSources();

            for (int xiRow = 0; xiRow < xoTbl.Rows.Count; xiRow++)
            {
                for (int xiCol = 0; xiCol < xoTbl.Columns.Count; xiCol++)
                {
                    if (xoTbl.Columns[xiCol].Caption.ToUpper() == "SERVERNAME")
                    {
                        string xs = "";
                        try
                        {
                            xs = xoTbl.Rows[xiRow][xiCol].ToString();
                        }
                        catch { }
                        if (xs.Length > 0) xsServers.Add(xs);
                    }

                }
            }

            xoTbl.Dispose();
            xoEnum = null;
            xoTbl = null;

            return (xsServers);
        }

        public List<string> GetDBsFromSQLServer(string psSQLServerID)
        {
            List<string> xsDBs = new List<string>(0);

            // disabled for now...too slow
            //try
            //{
            //    using (SqlConnection xoSQL = new SqlConnection("Data Source=" + psSQLServerID + ";Integrated Security=SSPI;"))
            //    //using (SqlConnection xoSQL = new SqlConnection("Data Source=" + @".\SQLEXPRESS" + ";Integrated Security=SSPI;"))
            //    {
            //        xoSQL.Open();
            //        DataTable xoTbl = xoSQL.GetSchema("Databases");
            //        xoSQL.Close();

            //        foreach (DataRow xoRow in xoTbl.Rows)
            //        {
            //            xsDBs.Add(xoRow["database_name"].ToString());
            //        }
            //    }
            //}
            //catch { }

            return (xsDBs);
        }

        /// <summary>
        /// pass in a DataTable, and dump its contents into a tab-delimited text file
        /// </summary>
        /// <param name="poTbl">table name</param>
        /// <param name="xsOutFile">output text path/file name</param>
        /// <returns>true on success, otherwise false (examine Message as to why)</returns>
        public bool DumpTableToCSV(DataTable poTbl, string xsOutFile)
        {
            string xsErrMsg = "";
            try { File.Delete(xsOutFile); }
            catch { };
            StreamWriter xoWrtr = null;

            try
            {
                xoWrtr = File.AppendText(xsOutFile);
                string xsLine = "";

                for (int xiRow = 0; xiRow < poTbl.Rows.Count; xiRow++)
                {
                    if (xiRow == 0)
                    {
                        xsLine = "";
                        for (int xiCol = 0; xiCol < poTbl.Columns.Count; xiCol++)
                        {
                            if (xiCol > 0) xsLine += "\t";
                            xsLine += poTbl.Columns[xiCol].ColumnName;
                        }
                        xoWrtr.WriteLine(xsLine);
                    }
                    xsLine = "";
                    for (int xiCol = 0; xiCol < poTbl.Columns.Count; xiCol++)
                    {
                        if (xiCol > 0) xsLine += "\t";
                        xsLine += poTbl.Rows[xiRow][xiCol].ToString();
                    }
                    xoWrtr.WriteLine(xsLine);
                }
            }
            catch (Exception xoExc)
            {
                xsErrMsg = xoExc.Message;
            }

            try
            {
                xoWrtr.Close();
                xoWrtr.Dispose();
                xoWrtr = null;
            }
            catch { }

            msErrMsg = xsErrMsg;

            return (xsErrMsg.Length == 0);
        }

        /// <summary>
        /// pass in a DataTable, and dump its contents into a JSON text file
        /// </summary>
        /// <param name="poTbl">table name</param>
        /// <param name="xsOutFile">output text path/file name</param>
        /// <returns>true on success, otherwise false (examine Message as to why)</returns>
        public bool DumpTableToJSON(DataTable poTbl, string xsOutFile)
        {
            string xsErrMsg = "";
            try { File.Delete(xsOutFile); }
            catch { };
            StreamWriter xoWrtr = null;
            DatatypeJSON[] xiFldType = new DatatypeJSON[0];

            try
            {
                xoWrtr = File.AppendText(xsOutFile);
                string xsRec = "";

                xoWrtr.Write("[");

                for (int xiRow = 0; xiRow < poTbl.Rows.Count; xiRow++)
                {
                    if (xiRow == 0)
                    {
                        xiFldType = new DatatypeJSON[poTbl.Columns.Count];
                        for (int xiFld = 0; xiFld < poTbl.Columns.Count; xiFld++)
                        {
                            //string xs = Data.GetFieldType(xiFld).Name.ToString().ToLower();
                            string xs = poTbl.Columns[xiFld].DataType.ToString().ToLower();
                            DatatypeJSON xoType = DatatypeJSON.String;
                            if (xs.IndexOf("bool") >= 0)
                            {
                                xoType = DatatypeJSON.Boolean;
                            }
                            else
                            {
                                if (xs.IndexOf("int") >= 0 || xs.IndexOf("long") >= 0 || xs.IndexOf("float") >= 0 || xs.IndexOf("dec") >= 0 || xs.IndexOf("sing") >= 0 || xs.IndexOf("doub") >= 0 || xs.IndexOf("byt") >= 0 || xs.IndexOf("short") >= 0)
                                {
                                    xoType = DatatypeJSON.Number;
                                }
                            }
                            xiFldType[xiFld] = xoType;
                        }
                    }
                    //if (xiRow > 0) xsLine += ",\r\n";
                    //xsLine += "  {\r\n";
                    xsRec = "\r\n  {\r\n";
                    for (int xiCol = 0; xiCol < poTbl.Columns.Count; xiCol++)
                    {
                        if (xiCol > 0) xsRec += ",\r\n";
                        var xv = poTbl.Rows[xiRow][xiCol];
                        string xsVal = "";
                        if (xv == null || xv == DBNull.Value)
                        {
                            xsVal = "null";
                        }
                        else
                        {
                            if (xiFldType[xiCol] == DatatypeJSON.Boolean)
                            {
                                xsVal = ((bool)xv) ? "true" : "false";
                            }
                            else
                            {
                                if (xiFldType[xiCol] == DatatypeJSON.Number)
                                {
                                    xsVal = xv.ToString();
                                }
                                else
                                {
                                    xsVal = "\"" + xv.ToString().Replace("\"", "\\\"") + "\"";
                                }
                            }
                        }
                        xsRec += "    \"" + poTbl.Columns[xiCol].ColumnName + "\": " + xsVal;
                    }
                    xsRec += "\r\n  }";
                    if (xiRow < poTbl.Rows.Count - 1) xsRec += ",";
                    xoWrtr.Write(xsRec);
                }

                xoWrtr.Write("\r\n]");
                xoWrtr.Flush();

            }
            catch (Exception xoExc)
            {
                xsErrMsg = xoExc.Message;
            }

            try
            {
                xoWrtr.Close();
                xoWrtr.Dispose();
                xoWrtr = null;
            }
            catch { }

            msErrMsg = xsErrMsg;

            return (xsErrMsg.Length == 0);
        }

        public string null2str(object po)
        {
            string xs = "";
            if (po != null) xs = po.ToString();
            return (xs);
        }

        public int null2int(object po)
        {
            int xi = 0;
            if (po != null) xi = Convert.ToInt32(po);
            return (xi);
        }

        public long null2long(object po)
        {
            long xi = 0;
            if (po != null) xi = Convert.ToInt64(po);
            return (xi);
        }
        #endregion

    }
}
