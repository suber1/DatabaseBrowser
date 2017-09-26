using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RebusData6;

namespace RebusSQL6
{
    public enum MDIType
    {
        Unknown = 0,
        SQL = 1,
        Tables = 2
    }

        //    private bool mbOK;
        //public bool OK { get { return mbOK; } set { mbOK = value; } }

    #region DbToOpen

    public partial class DbToOpenParams
    {
        private RebusData6.ConnectivityType miConnType;
        public RebusData6.ConnectivityType ConnType { get { return miConnType; } set { miConnType = value; } }

        private RebusData6.Brand miBrand;
        public RebusData6.Brand DataBrand  { get { return miBrand; } set { miBrand = value; } }

        private string msDbSource;
        public string DataSource { get { return msDbSource; } set { msDbSource = value; } }

        private string msDatabase;
        public string Database { get { return msDatabase; } set { msDatabase = value; } }

        private string msUserID;
        public string UserID { get { return msUserID; } set { msUserID = value; } }

        private string msPswd;
        public string Password { get { return msPswd; } set { msPswd = value; } }

        private bool mbUseManConnStr;
        public bool UseManConnStr { get { return mbUseManConnStr; } set { mbUseManConnStr = value; } }

        private string msManConnStr;
        public string ManualConnStr { get { return msManConnStr; } set { msManConnStr = value; } }

        private int miDbID;
        public int DbID { get { return miDbID; } set { miDbID = value; } }

        //
        // constructor
        //
        public DbToOpenParams()
        {
            miConnType = ConnectivityType.DotNet;
            miBrand = Brand.SqlSrvr;
            msDbSource = "";
            msDatabase = "";
            msUserID = "";
            msPswd = "";
            mbUseManConnStr = false;
            msManConnStr = "";
        }


    }

    #endregion
}
