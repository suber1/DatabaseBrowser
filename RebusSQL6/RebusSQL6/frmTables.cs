using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RebusData4;

namespace RebusSQL4
{
    public partial class frmTables : frmBaseMDI
    {
        //private string msTable;
        //public string Table { get { return msTable; } set { msTable = value; } }

        private int miDbID;
        public int DbID { get { return miDbID; } set { miDbID = value; } }

        private DB moDB;
        public DB DB { get { return moDB; } set { moDB = value; } }

        private DataTable moTbl;
        private bool mbNeedInit = true;

        public frmTables()
        {
            InitializeComponent();
            moDB = null;
            moTbl = null;
        }

        private void frmTables_Load(object sender, EventArgs e)
        {

        }

        private void frmTables_FormClosing(object sender, FormClosingEventArgs e)
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

        private void frmTables_Shown(object sender, EventArgs e)
        {
            if (mbNeedInit)
            {
                mbNeedInit = false;
                if (moDB != null)
                {
                    if (moDB.DatabaseIsOpen())
                    {
                        List<string> xsTbls = moDB.GetTables();
                        if (moDB.Message.Length == 0)
                        {
                            //lstTables.Items.Clear();
                            if (xsTbls != null)
                            {
                                if (xsTbls.Count > 0)
                                {
                                    //foreach (string xsTbl in xsTbls)
                                    //{
                                    //    lstTables.Items.Add(xsTbl);
                                    //}
                                    moDB.ToGridFromList(this.grd, xsTbls);
                                }
                            }
                        }
                        else
                        {
                            //
                        }
                    }
                }
            }
        }
    }

}
